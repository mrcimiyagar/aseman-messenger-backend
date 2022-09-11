using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bugsnag;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using SharedArea.Commands.Notifs;
using SharedArea.Commands.Requests.Questions;
using SharedArea.Entities;
using SharedArea.Middles;
using SharedArea.Utils;

namespace DataKeeperPeer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.Setup();

            using (var dbContext = new DatabaseContext())
            {
                DatabaseConfig.ConfigDatabase(dbContext);
            }

            var configs = new List<ConsumerConfig>()
            {
                new ConsumerConfig
                {
                    BootstrapServers = Variables.SelfPeerAddress,
                    SaslUsername = SharedArea.GlobalVariables.KafkaUsername,
                    SaslPassword = SharedArea.GlobalVariables.KafkaPassword,
                    SecurityProtocol = SecurityProtocol.SaslPlaintext,
                    SaslMechanism = SaslMechanism.Plain,
                    GroupId = "main-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                }
            };

            if (string.IsNullOrEmpty(Variables.BugSnagToken))
            {
                KafkaExtension.SetupConsumer<QuestionConsumer, KafkaTransport>("Request", configs);
                KafkaExtension.SetupConsumer<NotifConsumerAsync, KafkaTransport>("Notifications", configs);
            }
            else
            {
                KafkaExtension.SetupConsumer<QuestionConsumer, KafkaTransport>("Request", configs,
                    new Bugsnag.Client(new Configuration(Variables.BugSnagToken)));
                KafkaExtension.SetupConsumer<NotifConsumerAsync, KafkaTransport>("Notifications", configs,
                    new Bugsnag.Client(new Configuration(Variables.BugSnagToken)));
            }

            Logger.Log("Info", $"Peer {Variables.SelfPeerAddress} loaded");

            using (var dbContext = new DatabaseContext())
            {
                if (Variables.SelfClusterCode == "guilan" && Variables.SelfPeerCode == "rasht" &&
                    !dbContext.BaseUsers.OfType<Bot>().Include(b => b.BotSecret).Any(b => b.BotSecret.Token == "+ABC"))
                {
                    dbContext.BotCreations.DeleteFromQuery();
                    dbContext.BotSubscriptions.DeleteFromQuery();
                    dbContext.BotSecrets.DeleteFromQuery();
                    dbContext.Sessions.Where(s => s.BaseUser is Bot).DeleteFromQuery();
                    dbContext.BaseUsers.OfType<Bot>().DeleteFromQuery();

                    var p1 = PreparePhoto();
                    var p2 = PreparePhoto();
                    var p3 = PreparePhoto();
                    var p4 = PreparePhoto();
                    
                    dbContext.AddRange(p1, p2, p3, p4);
                    dbContext.SaveChanges();

                    var bc1 = PrepareBot("Clock", "+ABC", p1.FileId);
                    var bc2 = PrepareBot("Calendar", "+DEF", p2.FileId);
                    var bc3 = PrepareBot("Assistant", "+GHI", p3.FileId);
                    var bc4 = PrepareBot("Assistant", "+JKL", p4.FileId);
                    
                    dbContext.AddRange(bc1, bc2, bc3, bc4);
                    dbContext.SaveChanges();
                }
            }
        }

        private static Photo PreparePhoto()
        {
            var kt = new KafkaTransport();
            var botIdTak = kt.AskForNewId(new AskIndexEntity() {EntityType = typeof(File).Name});

            Task.WaitAll(botIdTak);

            var photo = new Photo()
            {
                FileId = botIdTak.Result.Index,
                AsRawFile = true,
                FileTransferFinished = true,
                Width = 256,
                Height = 256,
                IsAvatar = true,
                IsPublic = true,
                Size = 100,
                Uploader = null,
            };

            kt.NotifyAllClusters(new PhotoCreatedNotif()
            {
                Packet = new Packet()
                {
                    Photo = photo
                }
            });
            
            return photo;
        }

        private static BotCreation PrepareBot(string title, string token, long avatarId)
        {
            var kt = new KafkaTransport();
            var botIdTak = kt.AskForNewId(new AskIndexEntity() {EntityType = typeof(BaseUser).Name});
            var botSecretIdTak = kt.AskForNewId(new AskIndexEntity() {EntityType = typeof(BotSecret).Name});
            var botCreationIdTak = kt.AskForNewId(new AskIndexEntity() {EntityType = typeof(BotCreation).Name});
            var botSubscriptionIdTak = kt.AskForNewId(new AskIndexEntity() {EntityType = typeof(BotSubscription).Name});
            var sessionIdTak = kt.AskForNewId(new AskIndexEntity() {EntityType = typeof(Session).Name});

            Task.WaitAll(botIdTak, botSecretIdTak, botCreationIdTak, botSubscriptionIdTak, sessionIdTak);

            var bot = new Bot()
            {
                BaseUserId = botIdTak.Result.Index,
                BotSecret = new BotSecret()
                {
                    BotSecretId = botSecretIdTak.Result.Index,
                    Creator = null,
                    Token = token
                },
                Subscriptions = new List<BotSubscription>()
                {
                    new BotSubscription()
                    {
                        BotSubscriptionId = botSubscriptionIdTak.Result.Index,
                        Subscriber = null
                    }
                },
                Sessions = new List<Session>()
                {
                    new Session()
                    {
                        SessionId = sessionIdTak.Result.Index,
                        Token = token
                    }
                },
                Title = title,
                Description = "This is Aseman Home Bot",
                Avatar = avatarId
            };
            bot.BotSecret.Bot = bot;
            bot.Subscriptions[0].Bot = bot;
            bot.Sessions[0].BaseUser = bot;

            var botCreation = new BotCreation()
            {
                BotCreationId = botCreationIdTak.Result.Index,
                Bot = bot,
                Creator = null
            };

            kt.NotifyApiGateway(new SessionCreatedNotif()
            {
                Packet = new Packet()
                {
                    Session = bot.Sessions[0]
                }
            });

            Console.WriteLine($"SessionId : {bot.Sessions[0].SessionId}{Environment.NewLine}" +
                              $"Token : {bot.BotSecret.Token}{Environment.NewLine}" +
                              $"Avatar : {bot.Avatar}");

            return botCreation;
        }
    }
}
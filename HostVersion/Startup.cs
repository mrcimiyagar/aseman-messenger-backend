using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bugsnag;
using Bugsnag.AspNet.Core;
using Confluent.Kafka;
using HostVersion.Commands.Notifs;
using HostVersion.Commands.Requests.Questions;
using HostVersion.Consumers;
using HostVersion.DbContexts;
using HostVersion.Entities;
using HostVersion.Hubs;
using HostVersion.Middles;
using HostVersion.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace HostVersion
{
    public class Startup
    {
        public static Pusher Pusher { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = 4294967296;
            });
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            if (!string.IsNullOrEmpty(Variables.BugSnagToken))
            {
                services.AddBugsnag(configuration => { configuration.ApiKey = Variables.BugSnagToken; });
            }

            services
                .AddSignalR(hubOptions => { hubOptions.EnableDetailedErrors = true; })
                .AddNewtonsoftJsonProtocol(options =>
                    {
                        options.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            IHubContext<NotificationsHub> notificationsHub)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseSignalR(route =>
            {
                route.MapHub<NotificationsHub>("/NotificationsHub");
            });

            using (var dbContext = new DatabaseContext())
            {
                DatabaseConfig.ConfigDatabase(dbContext);

                foreach (var session in dbContext.Sessions)
                {
                    session.Online = false;
                    session.ConnectionId = "";
                }

                dbContext.SaveChanges();
            }
            
            app.UseHttpsRedirection();
            app.UseMvc();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new UppercaseContractResolver()
            };

            Logger.Setup();
            Pusher = new Pusher(notificationsHub);
            MongoLayer.Setup();
            
            KafkaExtension.SetupConsumers(new QuestionConsumer(), new ApiGatewayConsumerAsync());
                
            Console.WriteLine("Bus loaded");

            var rawDbContext = new RawDbContext();

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

            Console.WriteLine($"SessionId : {bot.Sessions[0].SessionId}{Environment.NewLine}" +
                              $"Token : {bot.BotSecret.Token}{Environment.NewLine}" +
                              $"Avatar : {bot.Avatar}");

            return botCreation;
        }
    }
}
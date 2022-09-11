using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using SharedArea;
using SharedArea.Commands;
using SharedArea.Commands.Requests;
using SharedArea.Utils;
using SharedArea.Wrappers;

namespace SuperPeer
{
    public class KafkaExtension
    {
        public static void SetupConsumer(IEnumerable<ConsumerConfig> configs, Bugsnag.Client bugsnag = null)
        {
            Task.Run(() =>
            {
                foreach (var config in configs)
                {
                    new Thread(async () =>
                    {
                        await SubscribeToType<BasePack>(config, bugsnag);
                    }).Start();
                }
            });
        }

        public static async Task SubscribeToType<K>(
            ConsumerConfig config,
            Bugsnag.Client bugsnag)
            where K : class
        {
            using (var c = new ConsumerBuilder<Ignore, MessageWrapper<K>>(config)
                .SetKeyDeserializer(null)
                .SetValueDeserializer(new ProtoDeserializer<MessageWrapper<K>>())
                .Build())
            {
                c.Subscribe("Queue");

                Console.WriteLine($"Subscribed to type : {typeof(K).FullName}");

                while (true)
                {
                    try
                    {
                        var cr = c.Consume();

                        if (cr?.Message?.Value?.Message != null)
                        {
                            switch (cr.Message.Value.MessageType)
                            {
                                case WrapperType.Question:
                                    Console.WriteLine($"Received question for type {cr.Value.Message.GetType().FullName}");
                                    break;
                                case WrapperType.Answer:
                                    Console.WriteLine($"Received answer for type {cr.Value.Message.GetType().FullName}");
                                    break;
                                case WrapperType.Notification:
                                    Console.WriteLine($"Received internal notification for type {cr.Value.Message.GetType().FullName}");
                                    break;
                                case WrapperType.OutNetNotif:
                                    Console.WriteLine($"Received external notification for type {cr.Value.Message.GetType().FullName}");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            new Thread(async () =>
                            {
                                if (cr.Message.Value.DestClusterCode == Variables.SelfClusterCode)
                                {
                                    var kt = new KafkaTransport();
                                    if (cr.Message.Value.DestPeerCode == "all")
                                    {
                                        await kt.NotifyAllInSelfCluster(cr.Value);
                                    }
                                    else
                                    {
                                        await kt.NotifyPeerInSelfCluster(cr.Value);
                                    }
                                }
                                else
                                {
                                    var kt = new KafkaTransport();
                                    if (cr.Message.Value.DestPeerCode == "all")
                                    {
                                        await kt.NotifyAllClusters(cr.Value.Message);
                                    }
                                    else
                                    {
                                        if (cr.Message.Value.DestPeerCode == "all")
                                        {
                                            await kt.NotifySingleCluster(cr.Value.DestClusterCode,
                                                cr.Value.Message);
                                        }
                                        else
                                        {
                                            await kt.NotifySinglePeer(cr.Value.DestClusterCode,
                                                cr.Value.DestPeerCode,
                                                cr.Value.Message);
                                        }
                                    }
                                }
                            }).Start();
                        }

                        c.Commit(cr);
                    }
                    catch (ConsumeException e)
                    {
                        Logger.Log("Error", $"Error occured: {e.Error.Reason}");
                        bugsnag?.Notify(e);
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Error", $"Error occured: {e.Message}");
                        bugsnag?.Notify(e);
                    }
                }
            }
        }
    }
}
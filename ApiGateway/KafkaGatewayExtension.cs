using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Confluent.Kafka;
using SharedArea;
using SharedArea.Commands;
using SharedArea.Commands.Notifs;
using SharedArea.Commands.Pushes;
using SharedArea.Commands.Requests;
using SharedArea.Utils;
using SharedArea.Wrappers;

namespace ApiGateway
{
    public class KafkaGatewayExtension
    {
        public static void SetupGatewayConsumer<T, V>(string consumerType,
            IEnumerable<ConsumerConfig> configs,
            Bugsnag.Client bugsnag = null) where V : Answerable
        {
            var consumer = Activator.CreateInstance<T>();

            var transport = Activator.CreateInstance<V>();

            foreach (var config in configs)
            {
                switch (consumerType)
                {
                    case "All":
                        new Thread(() => { SubscribeToType<T, V, BasePack>(config, bugsnag, consumer, transport); })
                            .Start();
                        break;
                    case "Request":
                        new Thread(() => { SubscribeToType<T, V, ReqRes>(config, bugsnag, consumer, transport); })
                            .Start();
                        break;
                    case "Notification":
                        new Thread(() => { SubscribeToType<T, V, Notification>(config, bugsnag, consumer, transport); })
                            .Start();
                        break;
                    case "Push":
                        new Thread(() => { SubscribeToType<T, V, Push>(config, bugsnag, consumer, transport); })
                            .Start();
                        break;
                    default:
                        break;
                }
            }
            
            Console.WriteLine("Subscription done.");
        }

        private static void SubscribeToType<T, V, K>(
            ConsumerConfig config,
            Bugsnag.Client bugsnag,
            T consumer,
            V transport)
            where V : Answerable
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

                        if (cr.Message?.Value?.Message != null)
                        {
                            switch (cr.Value.MessageType)
                            {
                                case WrapperType.Question:
                                {
                                    Console.WriteLine(
                                        $"Received question for type {cr.Value.Message.GetType().FullName}");
                                    new Thread(() =>
                                    {
                                        var acT = typeof(AnswerContext<>);
                                        var gens = new Type[] {cr.Value.Message.GetType()};
                                        var constructed = acT.MakeGenericType(gens);
                                        var answerContext = Activator.CreateInstance(constructed, cr.Value.Message);
                                        var methodInfo = consumer.GetType().GetMethod("AnswerQuestion",
                                            BindingFlags.Instance | BindingFlags.Public,
                                            null,
                                            new Type[] {answerContext.GetType()},
                                            null);
                                        var answer = methodInfo?.Invoke(consumer, new object[] {answerContext});
                                        transport.SendAnswerToQuestionaire(cr.Value.SrcClusterCode,
                                            cr.Value.SrcPeerCode, cr.Value.QuestionId, (Response) answer);
                                    }).Start();
                                    break;
                                }

                                case WrapperType.Answer:
                                {
                                    Console.WriteLine($"Received answer for type {cr.Value.Message.GetType().FullName}");
                                    new Thread(() =>
                                    {
                                        transport.NotifyAnswerReceived(cr.Value.QuestionId, cr.Value.Message);
                                    }).Start();
                                    break;
                                }

                                case WrapperType.Notification:
                                {
                                    Console.WriteLine($"Received internal notification for type {cr.Value.Message.GetType().FullName}");
                                    new Thread(() =>
                                    {
                                        var acT = typeof(ConsumeContext<>);
                                        var gens = new Type[] {cr.Value.Message.GetType()};
                                        var constructed = acT.MakeGenericType(gens);
                                        var consumeContext = Activator.CreateInstance(constructed, cr.Value.Message);
                                        var methodInfo = consumer.GetType().GetMethod("Consume",
                                            BindingFlags.Instance | BindingFlags.Public,
                                            null,
                                            new Type[] {consumeContext.GetType()},
                                            null);
                                        methodInfo?.Invoke(consumer, new object[] {consumeContext});
                                    }).Start();
                                    break;
                                }

                                case WrapperType.OutNetNotif:
                                {
                                    Console.WriteLine($"Received external notification for type {cr.Value.Message.GetType().FullName}");
                                    new Thread(() =>
                                    {
                                        var acT = typeof(ConsumeContext<>);
                                        var gens = new Type[] {cr.Value.Message.GetType()};
                                        var constructed = acT.MakeGenericType(gens);
                                        var consumeContext = Activator.CreateInstance(constructed, cr.Value.Message);
                                        var methodInfo = consumer.GetType().GetMethod("Consume",
                                            BindingFlags.Instance | BindingFlags.Public,
                                            null,
                                            new Type[] {consumeContext.GetType()},
                                            null);
                                        methodInfo?.Invoke(consumer, new object[] {consumeContext});
                                    }).Start();
                                    break;
                                }
                            }
                        }

                        c.Commit(cr);
                    }
                    catch (ConsumeException e)
                    {
                        Logger.Log("Error", $"Error occured: {e}");
                        bugsnag?.Notify(e);
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Error", $"Error occured: {e}");
                        bugsnag?.Notify(e);
                    }
                }
            }
        }
    }
}
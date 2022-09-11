using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using SharedArea.Commands.Notifs;
using SharedArea.Commands.Pushes;
using SharedArea.Commands.Requests;
using SharedArea.Commands.Requests.Auth;
using SharedArea.Wrappers;

namespace SharedArea.Utils
{
    public class KafkaExtension
    {
        public static void SetupConsumer<T, V>(string consumerType, IEnumerable<ConsumerConfig> configs,
            Bugsnag.Client bugsnag)
            where V : Answerable
        {
            var consumer = Activator.CreateInstance<T>();

            var transport = Activator.CreateInstance<V>();

            Task.Run(() =>
            {
                foreach (var config in configs)
                {
                    if (consumerType == "Request")
                    {
                        SubscribeToType<T, V, ReqRes>(config, bugsnag, consumer, transport);
                    }
                    else if (consumerType == "Notification")
                    {
                        SubscribeToType<T, V, Notification>(config, bugsnag, consumer, transport);
                    }
                    else if (consumerType == "Push")
                    {
                        SubscribeToType<T, V, Push>(config, bugsnag, consumer, transport);
                    }
                }
            });
        }

        public static void SetupConsumer<T, V>(string consumerType, IEnumerable<ConsumerConfig> configs)
            where V : Answerable
        {
            SetupConsumer<T, V>(consumerType, configs, null);
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

                        if (cr?.Value?.Message != null)
                        {
                            switch (cr.Value.MessageType)
                            {
                                case WrapperType.Question:
                                {
                                    Console.WriteLine(
                                        $"Received question for type {cr.Value.Message.GetType().FullName}");
                                    new Thread(async () =>
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
                                        var isAwaitable = methodInfo?.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
                                        object answer;
                                        if (isAwaitable)
                                            answer = await (dynamic) methodInfo.Invoke(consumer,
                                                new object[] {answerContext});
                                        else
                                            answer = methodInfo?.Invoke(consumer, new object[] {answerContext});
                                        await transport.SendAnswerToQuestionaire(cr.Value.SrcClusterCode,
                                            cr.Value.SrcPeerCode, cr.Value.QuestionId, (Response)answer);
                                    }).Start();
                                    break;
                                }
                                case WrapperType.Answer:
                                    new Thread(() =>
                                    {
                                        Console.WriteLine($"Received answer for type {cr.Value.Message.GetType().FullName}");
                                        transport.NotifyAnswerReceived(cr.Value.QuestionId, cr.Value.Message);
                                    }).Start();
                                    break;
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
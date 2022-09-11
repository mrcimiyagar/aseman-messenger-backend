using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using HostVersion.Commands;
using HostVersion.Commands.Notifs;
using HostVersion.Commands.Pushes;
using HostVersion.Commands.Requests;
using HostVersion.Consumers;
using HostVersion.SharedArea;
using HostVersion.Utils;
using HostVersion.Wrappers;

namespace HostVersion
{
    public class KafkaExtension
    {
        private static QuestionConsumer _questionConsumer;
        private static ApiGatewayConsumerAsync _apiGatewayConsumer;
        private static KafkaTransport _transport = new KafkaTransport();

        public static void SetupConsumers(QuestionConsumer questionConsumer, ApiGatewayConsumerAsync apiGatewayConsumer)
        {
            _questionConsumer = questionConsumer;
            _apiGatewayConsumer = apiGatewayConsumer;
        }
        
        public static void Packet<T>(MessageWrapper<T> packet)
        {
            if (packet.Message != null)
            {
                switch (packet.MessageType)
                {
                    case WrapperType.OutNetNotif:
                    {
                        Console.WriteLine($"Received external notification for type {packet.Message.GetType().FullName}");
                        new Thread(() =>
                        {
                            var acT = typeof(ConsumeContext<>);
                            var gens = new Type[] {packet.Message.GetType()};
                            var constructed = acT.MakeGenericType(gens);
                            var consumeContext = Activator.CreateInstance(constructed, packet.Message);
                            var methodInfo = _apiGatewayConsumer.GetType().GetMethod("Consume",
                                BindingFlags.Instance | BindingFlags.Public,
                                null,
                                new Type[] {consumeContext.GetType()},
                                null);
                            methodInfo?.Invoke(_apiGatewayConsumer, new object[] {consumeContext});
                        }).Start();
                        break;
                    }
                    case WrapperType.Question:
                    {
                        Console.WriteLine(
                            $"Received question for type {packet.Message.GetType().FullName}");
                        new Thread(async () =>
                        {
                            var acT = typeof(AnswerContext<>);
                            var gens = new Type[] {packet.Message.GetType()};
                            var constructed = acT.MakeGenericType(gens);
                            var answerContext = Activator.CreateInstance(constructed, packet.Message);
                            var methodInfo = _questionConsumer.GetType().GetMethod("AnswerQuestion",
                                BindingFlags.Instance | BindingFlags.Public,
                                null,
                                new Type[] {answerContext.GetType()},
                                null);
                            var isAwaitable = methodInfo?.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
                            object answer;
                            if (isAwaitable)
                                answer = await (dynamic) methodInfo.Invoke(_questionConsumer,
                                    new object[] {answerContext});
                            else
                                answer = methodInfo?.Invoke(_questionConsumer, new object[] {answerContext});
                            await _transport.SendAnswerToQuestionaire(packet.SrcClusterCode,
                                packet.SrcPeerCode, packet.QuestionId, (Response) answer);
                        }).Start();
                        break;
                    }
                    case WrapperType.Answer:
                        new Thread(() =>
                        {
                            Console.WriteLine($"Received answer for type {packet.Message.GetType().FullName}");
                            _transport.NotifyAnswerReceived(packet.QuestionId, packet.Message);
                        }).Start();
                        break;
                }
            }
        }
    }
}
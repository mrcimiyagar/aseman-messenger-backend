using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using SharedArea;
using SharedArea.Commands.Requests;
using SharedArea.Utils;
using SharedArea.Wrappers;

namespace ApiGateway
{
    public class KafkaTransport : Answerable
    {
        private static readonly KafkaTransport Instance = new KafkaTransport();
        private static ProducerConfig _config;
        private static readonly ConcurrentDictionary<string, AutoResetEvent> PendingQuestions = new ConcurrentDictionary<string, AutoResetEvent>();
        private static readonly ConcurrentDictionary<string, object> ReceivedAnswers = new ConcurrentDictionary<string, object>();

        public KafkaTransport()
        {
            if (_config == null)
            {
                _config = new ProducerConfig()
                {
                    BootstrapServers = Variables.PairedPeerAddress,
                    SaslUsername = SharedArea.GlobalVariables.KafkaUsername,
                    SaslPassword = SharedArea.GlobalVariables.KafkaPassword,
                    SecurityProtocol = SecurityProtocol.SaslPlaintext,
                    SaslMechanism = SaslMechanism.Plain
                };
            }
        }

        public async void NotifyPairedPeer<T>(T message)
        {
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(_config)
                .SetKeySerializer(new ProtoSerializer<Null>())
                .SetValueSerializer(new ProtoSerializer<MessageWrapper<T>>())
                .Build())
                await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<T>>()
                {
                    Value = new MessageWrapper<T>()
                    {
                        Message = message,
                        MessageType = WrapperType.Notification,
                        SrcClusterCode = Variables.SelfClusterCode,
                        SrcPeerCode = Variables.SelfPeerCode,
                        DestClusterCode = Variables.SelfClusterCode,
                        DestPeerCode = Variables.SelfPeerCode
                    }
                });
        }

        public async Task<A> AskPairedPeer<T, A>(T message)
        {
            var questionId = Guid.NewGuid().ToString();
            var question = new AutoResetEvent(false);
            PendingQuestions.TryAdd(questionId, question);
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(_config)
                .SetValueSerializer(new ProtoSerializer<MessageWrapper<T>>())
                .Build())
            {
                await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<T>>()
                {
                    Value = new MessageWrapper<T>()
                    {
                        Message = message,
                        MessageType = WrapperType.Question,
                        SrcClusterCode = Variables.SelfClusterCode,
                        SrcPeerCode = Variables.SelfPeerCode,
                        DestClusterCode = Variables.SelfClusterCode,
                        DestPeerCode = Variables.SelfPeerCode,
                        QuestionId = questionId
                    }
                });
            }
            Console.WriteLine($"Asked paired peer a question about {message.GetType().FullName}");

            question.WaitOne();

            ReceivedAnswers.TryRemove(questionId, out var answer);
            PendingQuestions.TryRemove(questionId, out question);
            return (A) answer;
        }

        public void NotifyAnswerReceived(string questionId, object answer)
        {
            if (PendingQuestions.TryGetValue(questionId, out var question))
            {
                ReceivedAnswers.TryAdd(questionId, answer);
                question.Set();
            }
        }

        public async Task SendAnswerToQuestionaire<K>(string clusterCode, string peerCode, string questionId, K answer)
            where K : Response
        {
            using (var p = new ProducerBuilder<Null, MessageWrapper<K>>(_config)
                .SetKeySerializer(new ProtoSerializer<Null>())
                .SetValueSerializer(new ProtoSerializer<MessageWrapper<K>>())
                .Build())
                await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<K>>()
                {
                    Value = new MessageWrapper<K>()
                    {
                        Message = answer,
                        MessageType = WrapperType.Answer,
                        SrcClusterCode = Variables.SelfClusterCode,
                        SrcPeerCode = Variables.SelfPeerCode,
                        DestClusterCode = clusterCode,
                        DestPeerCode = peerCode
                    }
                });
        }
    }
}
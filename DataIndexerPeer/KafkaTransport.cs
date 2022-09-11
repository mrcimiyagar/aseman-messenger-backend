using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using SharedArea;
using SharedArea.Commands.Requests;
using SharedArea.Utils;
using SharedArea.Wrappers;

namespace DataIndexerPeer
{
    public class KafkaTransport : Answerable
    {
        private static readonly KafkaTransport Instance = new KafkaTransport();
        private static Dictionary<string, ProducerConfig> _configs;
        private static readonly ConcurrentDictionary<string, AutoResetEvent> PendingQuestions = new ConcurrentDictionary<string, AutoResetEvent>();
        private static readonly ConcurrentDictionary<string, object> ReceivedAnswers = new ConcurrentDictionary<string, object>();
        
        public KafkaTransport()
        {
            if (_configs == null)
            {
                _configs = new Dictionary<string, ProducerConfig>();
                
                foreach (var address in SharedArea.GlobalVariables.SuperPeerAddresses)
                {
                    _configs.Add(address.Key, new ProducerConfig()
                    {
                        BootstrapServers = address.Value,
                        SaslUsername = SharedArea.GlobalVariables.KafkaUsername,
                        SaslPassword = SharedArea.GlobalVariables.KafkaPassword,
                        SecurityProtocol = SecurityProtocol.SaslPlaintext,
                        SaslMechanism = SaslMechanism.Plain
                    
                    });
                }
            }
        }

        public async Task<A> AskSinglePeer<T, A>(string clusterCode, string peerCode, T message)
        {
            var questionId = Guid.NewGuid().ToString();
            var question = new AutoResetEvent(false);
            PendingQuestions.TryAdd(questionId, question);
            var conf = _configs[clusterCode];
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(conf)
                .SetKeySerializer(new ProtoSerializer<Null>())
                .SetValueSerializer(new ProtoSerializer<MessageWrapper<T>>())
                .Build())
                await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<T>>()
                {
                    Value = new MessageWrapper<T>()
                    {
                        Message = message,
                        MessageType = WrapperType.Question,
                        DestClusterCode = clusterCode,
                        DestPeerCode = peerCode,
                        QuestionId = questionId
                    }
                });
            question.WaitOne();
            ReceivedAnswers.TryRemove(questionId, out var answer);
            PendingQuestions.TryRemove(questionId, out question);
            return (A)answer;
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
            var conf = _configs[clusterCode];
            using (var p = new ProducerBuilder<Null, MessageWrapper<K>>(conf)
                .SetKeySerializer(new ProtoSerializer<Null>())
                .SetValueSerializer(new ProtoSerializer<MessageWrapper<K>>()).Build())
                await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<K>>()
                {
                    Value = new MessageWrapper<K>()
                    {
                        Message = answer,
                        QuestionId = questionId,
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
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

namespace SuperPeer
{
    public class KafkaTransport : Answerable
    {
        private static readonly KafkaTransport Instance = new KafkaTransport();
        private static Dictionary<string, ProducerConfig> _superPeerConfigs;
        private static Dictionary<string, ProducerConfig> _peerConfigs;
        private static readonly ConcurrentDictionary<string, AutoResetEvent> PendingQuestions = new ConcurrentDictionary<string, AutoResetEvent>();
        private static readonly ConcurrentDictionary<string, object> ReceivedAnswers = new ConcurrentDictionary<string, object>();
        
        public KafkaTransport()
        {
            if (_superPeerConfigs == null)
            {
                _superPeerConfigs = new Dictionary<string, ProducerConfig>();
                
                foreach (var address in SharedArea.GlobalVariables.SuperPeerAddresses)
                {
                    if (address.Value != Variables.SelfPeerAddress)
                    {
                        _superPeerConfigs.Add(address.Key, new ProducerConfig()
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
            
            if (_peerConfigs == null)
            {
                _peerConfigs = new Dictionary<string, ProducerConfig>();
                
                foreach (var address in Variables.PeerAddresses)
                {
                    if (address.Value != Variables.SelfPeerAddress)
                    {
                        _peerConfigs.Add(address.Key, new ProducerConfig()
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
        }

        public async Task NotifyAllInSelfCluster<T>(MessageWrapper<T> message)
        {
            foreach (var conf in _peerConfigs.Values)
            {
                using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(conf)
                    .SetKeySerializer(new ProtoSerializer<Null>())
                    .SetValueSerializer(new ProtoSerializer<MessageWrapper<T>>())
                    .Build())
                    await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<T>>()
                    {
                        Value = message
                    });
            }
        }

        public async Task NotifyPeerInSelfCluster<T>(MessageWrapper<T> message)
        {
            var conf = _peerConfigs[message.DestPeerCode];
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(conf)
                .SetValueSerializer(new ProtoSerializer<MessageWrapper<T>>())
                .Build())
                await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<T>>()
                {
                    Value = message
                });
        }
        
        public async Task NotifyAllClusters<T>(T message)
        {
            foreach (var conf in _superPeerConfigs.Values)
            {
                using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(conf)
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
                            SrcPeerCode = Variables.SelfPeerAddress,
                            DestClusterCode = "all",
                            DestPeerCode = "all"
                        }
                    });
            }
        }

        public async Task NotifySingleCluster<T>(string clusterCode, T message)
        {
            var conf = _superPeerConfigs[clusterCode];
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(conf)
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
                        SrcPeerCode = Variables.SelfPeerAddress, 
                        DestClusterCode = clusterCode, 
                        DestPeerCode = "all"
                    }
                });
        }

        public async Task NotifySinglePeer<T>(string clusterCode, string peerCode, T message)
        {
            var conf = _superPeerConfigs[clusterCode];
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(conf)
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
                        SrcPeerCode = Variables.SelfPeerAddress, 
                        DestClusterCode = clusterCode,
                        DestPeerCode = peerCode
                    }
                });
        }

        public async Task<A> AskSinglePeer<T, A>(string clusterCode, string peerCode, T message)
        {
            var questionId = Guid.NewGuid().ToString();
            var question = new AutoResetEvent(false);
            PendingQuestions.TryAdd(questionId, question);
            var conf = _superPeerConfigs[clusterCode];
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
                        SrcClusterCode = Variables.SelfClusterCode, 
                        SrcPeerCode = Variables.SelfPeerAddress, 
                        DestClusterCode = clusterCode,
                        DestPeerCode = peerCode,
                        QuestionId = questionId
                    }
                });
            lock (question)
            {
                Monitor.Wait(question);
            }
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
            var conf = _superPeerConfigs[clusterCode];
            using (var p = new ProducerBuilder<Null, MessageWrapper<K>>(conf)
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
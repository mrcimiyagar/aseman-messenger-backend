using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using SharedArea;
using SharedArea.Commands.Requests;
using SharedArea.Commands.Requests.Answers;
using SharedArea.Commands.Requests.Auth;
using SharedArea.Commands.Requests.Questions;
using SharedArea.Utils;
using SharedArea.Wrappers;

namespace DataKeeperPeer
{
    public class KafkaTransport : Answerable
    {
        private static readonly KafkaTransport Instance = new KafkaTransport();
        private static ProducerConfig _superPeerConfig;
        private static ProducerConfig _indexerPeerConfig;
        private static ProducerConfig _apiGatewayConfig;
        private static readonly ConcurrentDictionary<string, AutoResetEvent> PendingQuestions = new ConcurrentDictionary<string, AutoResetEvent>();
        private static readonly ConcurrentDictionary<string, object> ReceivedAnswers = new ConcurrentDictionary<string, object>();
        
        public KafkaTransport()
        {
            if (_superPeerConfig == null)
            {
                _superPeerConfig = new ProducerConfig()
                {
                    BootstrapServers = Variables.SuperPeerAddress,
                    SaslUsername = SharedArea.GlobalVariables.KafkaUsername,
                    SaslPassword = SharedArea.GlobalVariables.KafkaPassword,
                    SecurityProtocol = SecurityProtocol.SaslPlaintext,
                    SaslMechanism = SaslMechanism.Plain
                    
                };;
                
                _indexerPeerConfig = new ProducerConfig()
                {
                    BootstrapServers = Variables.IndexerPeerAddress,
                    SaslUsername = SharedArea.GlobalVariables.KafkaUsername,
                    SaslPassword = SharedArea.GlobalVariables.KafkaPassword,
                    SecurityProtocol = SecurityProtocol.SaslPlaintext,
                    SaslMechanism = SaslMechanism.Plain
                    
                };;
                
                _apiGatewayConfig = new ProducerConfig()
                {
                    BootstrapServers = Variables.ApiGatewayAddress,
                    SaslUsername = SharedArea.GlobalVariables.KafkaUsername,
                    SaslPassword = SharedArea.GlobalVariables.KafkaPassword,
                    SecurityProtocol = SecurityProtocol.SaslPlaintext,
                    SaslMechanism = SaslMechanism.Plain
                    
                };;
            }
        }
        
        public async void NotifyAllClusters<T>(T message)
        {
            if (Variables.HasSuperPeer)
            {
                using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(_superPeerConfig)
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
                            DestClusterCode = "all",
                            DestPeerCode = "all"
                        }
                    });
            }
        }

        public async void NotifySingleCluster<T>(string clusterCode, T message)
        {
            if (Variables.HasSuperPeer)
            {
                using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(_superPeerConfig)
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
                            DestClusterCode = clusterCode,
                            DestPeerCode = "all"
                        }
                    });
            }
        }

        public async void NotifySinglePeer<T>(string clusterCode, string peerCode, T message)
        {
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(_superPeerConfig)
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
                        DestClusterCode = clusterCode,
                        DestPeerCode = peerCode
                    }
                });
        }

        public async Task<AnswerIndexEntity> AskForNewId(AskIndexEntity askIndexEntity)
        {
            var questionId = Guid.NewGuid().ToString();
            var ar = new AutoResetEvent(false);
            PendingQuestions.TryAdd(questionId, ar);
            using (var p = new ProducerBuilder<Null, MessageWrapper<AskIndexEntity>>(_indexerPeerConfig)
                .SetKeySerializer(new ProtoSerializer<Null>())
                .SetValueSerializer(new ProtoSerializer<MessageWrapper<AskIndexEntity>>())
                .Build())
                await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<AskIndexEntity>>()
                {
                    Value = new MessageWrapper<AskIndexEntity>()
                    {
                        Message = askIndexEntity,
                        MessageType = WrapperType.Question,
                        SrcClusterCode = Variables.SelfClusterCode,
                        SrcPeerCode = Variables.SelfPeerCode,
                        DestClusterCode = "globe",
                        DestPeerCode = "globe",
                        QuestionId = questionId
                    }
                });

            ar.WaitOne();
            
            ReceivedAnswers.TryRemove(questionId, out var answer);
            PendingQuestions.TryRemove(questionId, out ar);
            return (AnswerIndexEntity)answer;
        }

        public async Task<A> AskSinglePeer<T, A>(string clusterCode, string peerCode, T message)
        {
            var questionId = Guid.NewGuid().ToString();
            var ar = new AutoResetEvent(false);
            PendingQuestions.TryAdd(questionId, ar);
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(_superPeerConfig)
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
                        SrcPeerCode = Variables.SelfPeerCode,
                        DestClusterCode = clusterCode,
                        DestPeerCode = peerCode,
                        QuestionId = questionId
                    }
                });

            ar.WaitOne();

            ReceivedAnswers.TryRemove(questionId, out var answer);
            PendingQuestions.TryRemove(questionId, out ar);
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
            if (clusterCode == Variables.SelfClusterCode && peerCode == Variables.SelfPeerCode)
            {
                using (var p = new ProducerBuilder<Null, MessageWrapper<K>>(_apiGatewayConfig)
                    .SetValueSerializer(new ProtoSerializer<MessageWrapper<K>>())
                    .Build())
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
            else
            {
                using (var p = new ProducerBuilder<Null, MessageWrapper<K>>(_superPeerConfig)
                    .SetValueSerializer(new ProtoSerializer<MessageWrapper<K>>())
                    .Build())
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

        public async void PushNotifToApiGateway<T>(T notif)
        {
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(_apiGatewayConfig)
                .SetKeySerializer(new ProtoSerializer<Null>())
                .SetValueSerializer(new ProtoSerializer<MessageWrapper<T>>())
                .Build())
                await p.ProduceAsync("Queue", new Message<Null, MessageWrapper<T>>()
                {
                    Value = new MessageWrapper<T>()
                    {
                        Message = notif,
                        MessageType = WrapperType.OutNetNotif,
                        SrcClusterCode = Variables.SelfClusterCode,
                        SrcPeerCode = Variables.SelfPeerCode,
                        DestClusterCode = Variables.SelfClusterCode,
                        DestPeerCode = Variables.SelfPeerCode
                    }
                });
        }

        public async void NotifyApiGateway<T>(T message)
        {
            using (var p = new ProducerBuilder<Null, MessageWrapper<T>>(_apiGatewayConfig)
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
    }
}
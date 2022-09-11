using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using HostVersion.Commands;
using HostVersion.Commands.Requests;
using HostVersion.Commands.Requests.Answers;
using HostVersion.Commands.Requests.Questions;
using HostVersion.SharedArea;
using HostVersion.Utils;
using HostVersion.Wrappers;

namespace HostVersion
{
    public class KafkaTransport : Answerable
    {
        private static readonly KafkaTransport Instance = new KafkaTransport();

        private static readonly ConcurrentDictionary<string, AutoResetEvent> PendingQuestions =
            new ConcurrentDictionary<string, AutoResetEvent>();

        private static readonly ConcurrentDictionary<string, object> ReceivedAnswers =
            new ConcurrentDictionary<string, object>();

        public async Task<A> AskPairedPeer<T, A>(T message)
        {
            var questionId = Guid.NewGuid().ToString();
            var question = new AutoResetEvent(false);
            PendingQuestions.TryAdd(questionId, question);

            var wrapper = new MessageWrapper<T>()
            {
                Message = message,
                MessageType = WrapperType.Question,
                SrcClusterCode = Variables.SelfClusterCode,
                SrcPeerCode = Variables.SelfPeerCode,
                DestClusterCode = Variables.SelfClusterCode,
                DestPeerCode = Variables.SelfPeerCode,
                QuestionId = questionId
            };

            KafkaExtension.Packet(wrapper);

            Console.WriteLine($"Asked paired peer a question about {message.GetType().FullName}");

            question.WaitOne();

            ReceivedAnswers.TryRemove(questionId, out var answer);
            PendingQuestions.TryRemove(questionId, out question);
            return (A) answer;
        }

        public Task<AnswerIndexEntity> AskForNewId(AskIndexEntity askIndexEntity)
        {
            return IdGenerator.Generate(askIndexEntity);
        }

        public void NotifyAnswerReceived(string questionId, object answer)
        {
            if (PendingQuestions.TryGetValue(questionId, out var question))
            {
                ReceivedAnswers.TryAdd(questionId, answer);
                question.Set();
            }
        }

        public Task SendAnswerToQuestionaire<K>(string clusterCode, string peerCode, string questionId, K answer)
            where K : Response
        {
            KafkaExtension.Packet(new MessageWrapper<K>
            {
                Message = answer,
                QuestionId = questionId,
                MessageType = WrapperType.Answer,
                SrcClusterCode = Variables.SelfClusterCode,
                SrcPeerCode = Variables.SelfPeerCode,
                DestClusterCode = clusterCode
            });

            return Task.CompletedTask;
        }

        public void PushNotifToApiGateway<T>(T notif)
        {
            KafkaExtension.Packet(new MessageWrapper<T>()
            {
                Message = notif,
                MessageType = WrapperType.OutNetNotif,
                SrcClusterCode = Variables.SelfClusterCode,
                SrcPeerCode = Variables.SelfPeerCode,
                DestClusterCode = Variables.SelfClusterCode,
                DestPeerCode = Variables.SelfPeerCode
            });
        }
    }
}
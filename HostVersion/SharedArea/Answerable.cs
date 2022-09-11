using System.Threading.Tasks;
using HostVersion.Commands.Requests;

namespace HostVersion.SharedArea
{
    public interface Answerable
    {
        void NotifyAnswerReceived(string questionId, object answer); 
        Task SendAnswerToQuestionaire<K>(string clusterCode, string peerCode, string questionId, K answer)
            where K : Response;
    }
}
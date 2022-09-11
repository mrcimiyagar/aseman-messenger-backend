using System.Threading.Tasks;
using SharedArea.Commands.Requests;

namespace SharedArea
{
    public interface Answerable
    {
        void NotifyAnswerReceived(string questionId, object answer); 
        Task SendAnswerToQuestionaire<K>(string clusterCode, string peerCode, string questionId, K answer)
            where K : Response;
    }
}
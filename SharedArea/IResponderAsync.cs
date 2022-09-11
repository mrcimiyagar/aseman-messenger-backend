using System.Threading.Tasks;

namespace SharedArea
{
    public interface IResponderAsync<Q, A> 
        where Q : class
        where A : class
    {
        Task<A> AnswerQuestion(AnswerContext<Q> question);
    }
}
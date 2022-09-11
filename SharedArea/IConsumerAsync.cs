using System.Threading.Tasks;

namespace SharedArea
{
    public interface IConsumerAsync<T> where T : class
    {
        Task Consume(ConsumeContext<T> context);
    }
}
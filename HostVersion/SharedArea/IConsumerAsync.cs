using System.Threading.Tasks;

namespace HostVersion.SharedArea
{
    public interface IConsumerAsync<T> where T : class
    {
        Task Consume(ConsumeContext<T> context);
    }
}
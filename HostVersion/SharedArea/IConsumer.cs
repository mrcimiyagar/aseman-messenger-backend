namespace HostVersion.SharedArea
{
    public interface IConsumer<T> where T : class
    {
        void Consume(ConsumeContext<T> context);
    }
}
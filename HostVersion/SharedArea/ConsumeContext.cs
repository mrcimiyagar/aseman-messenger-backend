namespace HostVersion.SharedArea
{
    public class ConsumeContext<T> where T : class
    {
        public T Message { get; set; }
        
        public ConsumeContext(T message)
        {
            this.Message = message;
        }
    }
}
namespace HostVersion.SharedArea
{
    public class AnswerContext<Q> where Q : class
    {
        public Q Question { get; set; }
        
        public AnswerContext(Q question)
        {
            this.Question = question;
        }
    }
}
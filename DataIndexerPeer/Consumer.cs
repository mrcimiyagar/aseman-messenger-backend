using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using SharedArea;
using SharedArea.Commands.Requests.Answers;
using SharedArea.Commands.Requests.Questions;

namespace DataIndexerPeer
{
    public class Consumer : IResponder<AskIndexEntity, AnswerIndexEntity>
    {
        public AnswerIndexEntity AnswerQuestion(AnswerContext<AskIndexEntity> question)
        {
            var query = $"REPLACE INTO Tickets (stub) VALUES ('{question.Question.EntityType}'); SELECT LAST_INSERT_ID();";
            lock (RawDbContext.Instance.Connection())
            {
                using (var cmd = new MySqlCommand(query, RawDbContext.Instance.Connection()))
                {
                    var id = cmd.ExecuteScalar();
                    return new AnswerIndexEntity() {Index = Convert.ToInt64(id)};
                }
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using HostVersion.Commands.Requests.Answers;
using HostVersion.Commands.Requests.Questions;
using MySql.Data.MySqlClient;

namespace HostVersion
{
    public class IdGenerator
    {
        public static async Task<AnswerIndexEntity> Generate(AskIndexEntity question)
        {
            var query = $"REPLACE INTO Tickets (stub) VALUES ('{question.EntityType}'); SELECT LAST_INSERT_ID();";
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
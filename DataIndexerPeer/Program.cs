using System;
using System.Collections.Generic;
using Bugsnag;
using Confluent.Kafka;
using MySql.Data.MySqlClient;
using SharedArea;
using SharedArea.Utils;

namespace DataIndexerPeer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.Setup();

            using (var dbContext = new DatabaseContext())
            {
                dbContext.Database.EnsureCreated();
            }

            var query = @"CREATE TABLE IF NOT EXISTS Tickets (
              id bigint(20) NOT NULL auto_increment,
              stub char(50) NOT NULL default '',
              PRIMARY KEY  (id),
              UNIQUE KEY stub (stub)) ENGINE=MyISAM";
            var cmd = new MySqlCommand(query, RawDbContext.Instance.Connection());
            cmd.ExecuteNonQuery();

            var configs = new List<ConsumerConfig>()
            {
                new ConsumerConfig
                {
                    BootstrapServers = Variables.SelfPeerAddress,
                    SaslUsername = SharedArea.GlobalVariables.KafkaUsername,
                    SaslPassword = SharedArea.GlobalVariables.KafkaPassword,
                    SecurityProtocol = SecurityProtocol.SaslPlaintext,
                    SaslMechanism = SaslMechanism.Plain,
                    GroupId = "main-group",
                    EnableAutoCommit = false,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                }
            };

            if (string.IsNullOrEmpty(Variables.BugSnagToken))
            {
                KafkaExtension.SetupConsumer<Consumer, KafkaTransport>("Request", configs);
            }
            else
            {
                KafkaExtension.SetupConsumer<Consumer, KafkaTransport>("Request", configs,
                    new Bugsnag.Client(new Configuration(Variables.BugSnagToken)));
            }

            Logger.Log("Info", $"Peer { Variables.SelfPeerAddress } loaded");
        }
    }
}
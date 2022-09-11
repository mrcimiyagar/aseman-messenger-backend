using System;
using System.Collections.Generic;
using Bugsnag;
using Confluent.Kafka;
using SharedArea;
using SharedArea.Utils;

namespace SuperPeer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.Setup();

            var configs = new List<ConsumerConfig>
            {
                new ConsumerConfig
                {
                    BootstrapServers = Variables.SelfPeerAddress,
                    SaslUsername = SharedArea.GlobalVariables.KafkaUsername,
                    SaslPassword = SharedArea.GlobalVariables.KafkaPassword,
                    SecurityProtocol = SecurityProtocol.SaslPlaintext,
                    SaslMechanism = SaslMechanism.Plain,
                    GroupId = "main-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                }
            };

            if (string.IsNullOrEmpty(Variables.BugSnagToken))
            {
                KafkaExtension.SetupConsumer(configs);
            }
            else
            {
                KafkaExtension.SetupConsumer(configs,
                    new Bugsnag.Client(new Configuration(Variables.BugSnagToken)));   
            }

            Logger.Log("Info", $"Peer { Variables.SelfPeerAddress } loaded");
        }
    }
}
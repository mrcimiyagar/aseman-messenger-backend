using System;
using System.Collections.Generic;
using System.Threading;
using ApiGateway.Consumers;
using ApiGateway.DbContexts;
using ApiGateway.Hubs;
using ApiGateway.Utils;
using Bugsnag;
using Bugsnag.AspNet.Core;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SharedArea.Commands;
using SharedArea.Utils;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Serialization;
using SharedArea.Entities;

namespace ApiGateway
{
    public class Startup
    {
        public static Pusher Pusher { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = 4294967296;
            });
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            if (!string.IsNullOrEmpty(Variables.BugSnagToken))
            {
                services.AddBugsnag(configuration => { configuration.ApiKey = Variables.BugSnagToken; });
            }
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
            }).AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            IHubContext<NotificationsHub> notificationsHub)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseSignalR(route =>
            {
                route.MapHub<NotificationsHub>("/NotificationsHub");
            });

            using (var dbContext = new DatabaseContext())
            {
                DatabaseConfig.ConfigDatabase(dbContext);

                foreach (var session in dbContext.Sessions)
                {
                    session.Online = false;
                    session.ConnectionId = "";
                }

                dbContext.SaveChanges();
            }
            
            app.UseHttpsRedirection();
            app.UseMvc();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new UppercaseContractResolver()
            };

            Logger.Setup();
            Pusher = new Pusher(notificationsHub);
            MongoLayer.Setup();

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
                    AutoOffsetReset = AutoOffsetReset.Earliest
                }
            };

            if (string.IsNullOrEmpty(Variables.BugSnagToken))
            {
                KafkaGatewayExtension.SetupGatewayConsumer<ApiGatewayConsumerAsync, KafkaTransport>("All", configs);
            }
            else
            {
                KafkaGatewayExtension.SetupGatewayConsumer<ApiGatewayConsumerAsync, KafkaTransport>("All", configs,
                    new Bugsnag.Client(new Configuration(Variables.BugSnagToken)));
            }
            
            Console.WriteLine("Bus loaded");
        }
    }
}
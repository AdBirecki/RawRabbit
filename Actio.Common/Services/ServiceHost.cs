using Actio.Common.Commands;
using Actio.Common.Events;
using Actio.Common.RabbitMQ;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RawRabbit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Actio.Common.Services
{
    public class ServiceHost : IServiceHost
    {
        private readonly IWebHost _webHost;

        public ServiceHost(IWebHost webHost) {
            _webHost = webHost;
        }

        public static HostBuilder Create<TStartup>(string[] args) where TStartup : class {
            Console.Title = typeof(TStartup).Namespace;
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .Build();

            var webHostBuilder = WebHost
                .CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<TStartup>();

            return new HostBuilder(webHostBuilder.Build());
        }

        public Task Run()
        {
            return _webHost.RunAsync();
        }


        public abstract class BuilderBase {
            public abstract ServiceHost Build();
        }

        public class HostBuilder : BuilderBase
        {
            private readonly IWebHost _webHost;
            private IBusClient _busClient;

            public HostBuilder(IWebHost webHost) {
                _webHost = webHost;
            }


            public BusBuilder UseRabbitMQ() {
                _busClient = (IBusClient)_webHost.Services.GetService(typeof(IBusClient));
                return new BusBuilder(_webHost, _busClient);
            }

            public override ServiceHost Build()
            {
                return new ServiceHost(_webHost);
            }
        }

        public class BusBuilder : BuilderBase
        {
            private readonly IWebHost _webHost;
            private IBusClient _busClient;

            public BusBuilder(IWebHost webHost, IBusClient busClient)
            {
                _webHost = webHost;
                _busClient = busClient;
            }

            public BusBuilder SubscribeToCommand<TCommand>() where TCommand: ICommand {
                ICommandHandler<TCommand> handler = (ICommandHandler<TCommand>)
                    _webHost.Services
                    .GetService(typeof(ICommandHandler<TCommand>));

                _busClient.WithCommandHandlerAsync(handler);
                return this;
            }

            public BusBuilder SubscribeToEvent<TEvent>() where TEvent : IEvent
            {
                IEventHandler<TEvent> handler = (IEventHandler<TEvent>)
                    _webHost.Services
                    .GetService(typeof(IEventHandler<TEvent>));

                _busClient.WithEventHandlerAsync(handler);
                return this;
            }

            public override ServiceHost Build()
            {
                return new ServiceHost(_webHost);
            }
        }
    }
}

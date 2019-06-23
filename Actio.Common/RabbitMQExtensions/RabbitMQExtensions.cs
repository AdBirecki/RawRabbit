using Actio.Common.Commands;
using Actio.Common.Events;
using Actio.Common.RabbitMQExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Instantiation;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Actio.Common.RabbitMQ
{
    public  static class RabbitMQExtensions
    {
        public static Task WithCommandHandlerAsync<TCommand>(
            this IBusClient bus, 
            ICommandHandler<TCommand> handler) where TCommand : ICommand
        {
           return  bus.SubscribeAsync<TCommand>(
                    msg => handler.HandleAsync(msg),
                    ctx => ctx.UseConsumeConfiguration(
                    cfg => cfg.FromQueue(GetQueryName<TCommand>())));
        }

        public static Task WithEventHandlerAsync<TEvent>(
            this IBusClient bus,
            IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            return bus.SubscribeAsync<TEvent>(
                     msg => handler.HandleAsync(msg),
                     ctx => ctx.UseConsumeConfiguration(
                     cfg => cfg.FromQueue(GetQueryName<TEvent>())));
        }

        private static string GetQueryName<T>() {
            return $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}";
        }

        public static void AddRabbitMQ(
            this IServiceCollection serviceCollection,
            IConfiguration configuration) {
            RabbitMQOptions options = new RabbitMQOptions();
            IConfigurationSection section = configuration.GetSection("RabbitMQ");
            section.Bind(options);

            var client = RawRabbitFactory
                .CreateSingleton(
                new RawRabbitOptions {
                    ClientConfiguration = options
            });

            serviceCollection
                .AddSingleton<IBusClient>( item => client);
        }
    }
}

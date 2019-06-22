using Actio.Common.Commands;
using Actio.Common.Events;
using RawRabbit;
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
    }
}

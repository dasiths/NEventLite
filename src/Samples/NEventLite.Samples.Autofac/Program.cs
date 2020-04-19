using System;
using System.Threading.Tasks;
using Autofac;
using NEventLite.Repository;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Handlers;
using Newtonsoft.Json;

namespace NEventLite.Samples.Autofac
{
    public static class Program
    {
        static void Main(string[] args)
        {
            RunSample().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task RunSample()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<NEventLiteModule>();
            var container = builder.Build();

            Guid scheduleId;
            Guid todoId;

            await using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<CreateScheduleHandler>();
                scheduleId = await handler.HandleAsync("test schedule");
            }

            await using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<CreateTodoHandler>();
                todoId = await handler.HandleAsync(scheduleId, "todo item 1");
            }

            await using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<CreateTodoHandler>();
                await handler.HandleAsync(scheduleId, "todo item 2");
            }

            await using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<CreateTodoHandler>();
                await handler.HandleAsync(scheduleId, "todo item 3");
            }

            await using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<UpdateTodoNameHandler>();
                await handler.HandleAsync(scheduleId, todoId, "todo item 1 updated");
            }

            await using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<CompleteTodoHandler>();
                await handler.HandleAsync(scheduleId, todoId);
            }

            await using (var scope = container.BeginLifetimeScope())
            {
                var session = scope.Resolve<ISession<Schedule>>();
                var result = await session.GetByIdAsync(scheduleId);
                Console.WriteLine("--------");
                Console.WriteLine("Final result after applying all events...");
                PrintToConsole(result, ConsoleColor.Green);
            }
        }

        public static void PrintToConsole(object @object, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(JsonConvert.SerializeObject(@object, Formatting.Indented));
            Console.ResetColor();
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NEventLite.Core;
using NEventLite.Repository;
using NEventLite.Samples.Common;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;
using NEventLite.Samples.ConsoleApp.Handlers;
using NEventLite.Storage;
using NEventLite.StorageProviders.EventStore;
using NEventLite.StorageProviders.InMemory;

namespace NEventLite.Samples.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            var container = DependencyInjection.GetContainer(false);

            Guid scheduleId;
            Guid todoId;

            using (var scope = container.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<CreateScheduleHandler>();
                scheduleId = await handler.HandleAsync("test schedule");
            }

            using (var scope = container.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<CreateTodoHandler>();
                todoId = await handler.HandleAsync(scheduleId, "todo item 1");
            }

            using (var scope = container.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<CreateTodoHandler>();
                await handler.HandleAsync(scheduleId, "todo item 2");
            }

            using (var scope = container.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<CreateTodoHandler>();
                await handler.HandleAsync(scheduleId, "todo item 3");
            }

            using (var scope = container.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<UpdateTodoNameHandler>();
                await handler.HandleAsync(scheduleId, todoId, "todo item 1 updated");
            }

            using (var scope = container.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<CompleteTodoHandler>();
                await handler.HandleAsync(scheduleId, todoId);
            }
            
            using (var scope = container.CreateScope())
            {
                var session = scope.ServiceProvider.GetService<ISession<Schedule>>();
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

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NEventLite.Repository;
using NEventLite.Samples.Common;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;
using NEventLite.StorageProviders.EventStore;
using NEventLite.StorageProviders.InMemory;

namespace NEventLite.Samples.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
            Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            //This path is used to save in memory storage
            string strTempDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";

            //create temp directory if it doesn't exist
            new FileInfo(strTempDataFolderPath).Directory?.Create();

            var inMemoryEventStorePath = $@"{strTempDataFolderPath}events.stream.dump";
            var inMemorySnapshotStorePath = $@"{strTempDataFolderPath}events.snapshot.dump";
            var inMemoryReadModelStorePath = $@"{strTempDataFolderPath}events.readmodel.dump";

            File.Delete(inMemoryEventStorePath);
            File.Delete(inMemorySnapshotStorePath);
            File.Delete(inMemoryReadModelStorePath);

            using (var connectionProvider = new EventStoreStorageConnectionProvider())
            {
                var clock = new MyClock();
                var eventStorage = new EventStoreEventStorageProvider<Schedule, Guid>(connectionProvider);
                // new InMemoryEventStorageProvider<Schedule>(inMemoryEventStorePath);

                var snapshotStorage = new EventStoreSnapshotStorageProvider<Schedule, ScheduleSnapshot, Guid>(connectionProvider);
                // new InMemorySnapshotStorageProvider<ScheduleSnapshot>(2, inMemorySnapshotStorePath);

                var eventPublisher = new EventPublisher<Schedule>();

                var repository =
                    new Repository<Schedule, ScheduleSnapshot>(clock, eventStorage, eventPublisher, snapshotStorage);

                // repository = new EventOnlyRepository<Schedule>(clock, eventStorage, eventPublisher);

                Session<Schedule> NewSessionFunc() => new Session<Schedule>(repository);
                Guid id;
                Schedule result;

                using (var session = NewSessionFunc())
                {
                    var schedule = new Schedule("test schedule");
                    session.Attach(schedule);
                    await session.SaveAsync();
                    id = schedule.Id;
                }

                using (var session = NewSessionFunc())
                {
                    var schedule = await session.GetByIdAsync(id);
                    schedule.AddTodo("test todo 1");
                    await session.SaveAsync();
                }

                using (var session = NewSessionFunc())
                {
                    var schedule = await session.GetByIdAsync(id);
                    schedule.AddTodo("test todo 2");
                    await session.SaveAsync();
                }

                using (var session = NewSessionFunc())
                {
                    var schedule = await session.GetByIdAsync(id);
                    schedule.AddTodo("test todo 3");
                    await session.SaveAsync();
                }

                using (var session = NewSessionFunc())
                {
                    var schedule = await session.GetByIdAsync(id);
                    var todo = schedule.Todos.First();
                    schedule.UpdateTodo(todo.Id, todo.Text + " updated");
                    await session.SaveAsync();
                }

                using (var session = NewSessionFunc())
                {
                    var schedule = await session.GetByIdAsync(id);
                    var todo = schedule.Todos.Last();
                    await schedule.CompleteTodoAsync(todo.Id);
                    await session.SaveAsync();
                    result = schedule;
                }

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

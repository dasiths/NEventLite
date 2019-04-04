using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Repository;
using NEventLite.Samples.Common;
using NEventLite.Samples.Common.Domain;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;
using NEventLite.Storage;
using NEventLite.StorageProviders.InMemory;

namespace NEventLite.Samples.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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

            var clock = new MyClock();
            var eventStorage = new InMemoryEventStorageProvider<Schedule>(inMemoryEventStorePath);
            var snapshotStorage = new InMemorySnapshotStorageProvider<ScheduleSnapshot>(2, inMemorySnapshotStorePath);
            var eventPublisher = new EventPublisher<Schedule>();
            var repository = new Repository<Schedule, ScheduleSnapshot>(clock, eventStorage, eventPublisher, snapshotStorage);

            // repository = new EventOnlyRepository<Schedule>(clock, eventStorage, eventPublisher);

            Session<Schedule> NewSessionFunc() => new Session<Schedule>(repository);
            Guid id;
            Schedule result;

            using (var session = NewSessionFunc())
            {
                var schedule = new Schedule("test schedule");
                session.Attach(schedule);
                await session.CommitChangesAsync();
                id = schedule.Id;
            }

            using (var session = NewSessionFunc())
            {
                var schedule = await session.GetByIdAsync(id);
                schedule.AddTodo("test todo 1");
                await session.CommitChangesAsync();
            }

            using (var session = NewSessionFunc())
            {
                var schedule = await session.GetByIdAsync(id);
                schedule.AddTodo("test todo 2");
                await session.CommitChangesAsync();
            }

            using (var session = NewSessionFunc())
            {
                var schedule = await session.GetByIdAsync(id);
                schedule.AddTodo("test todo 3");
                await session.CommitChangesAsync();
            }

            using (var session = NewSessionFunc())
            {
                var schedule = await session.GetByIdAsync(id);
                var todo = schedule.Todos.First();
                schedule.UpdateTodo(todo.Id, todo.Text + " updated");
                await session.CommitChangesAsync();
            }

            using (var session = NewSessionFunc())
            {
                var schedule = await session.GetByIdAsync(id);
                var todo = schedule.Todos.Last();
                await schedule.CompleteTodoAsync(todo.Id);
                result = schedule;
            }

            Console.WriteLine();
            Console.WriteLine("Schedule loaded from Repository:");
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}

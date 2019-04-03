using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Repository;
using NEventLite.Samples.Common.Domain;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Domain.Schedule.Snapshot;
using NEventLite.Storage;
using NEventLite.StorageProviders.InMemory;
using NEventLite.Tests.Integration.Mocks;
using Shouldly;
using Xunit;

namespace NEventLite.Tests.Integration
{
    public class EndToEndTests
    {
        private readonly MockEventPublisher<Schedule, Guid, Guid> _eventPublisher;
        private readonly MockClock _clock;
        private readonly IRepository<Schedule, Guid, Guid> _repository;
        private readonly IRepository<Schedule, Guid, Guid> _eventOnlyRepository;

        public EndToEndTests()
        {
            //This path is used to save in memory storage
            string strTempDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";

            //create temp directory if it doesn't exist
            new FileInfo(strTempDataFolderPath).Directory?.Create();

            var inMemoryEventStorePath = $@"{strTempDataFolderPath}events.stream.dump";
            var inMemorySnapshotStorePath = $@"{strTempDataFolderPath}events.snapshot.dump";
            
            File.Delete(inMemoryEventStorePath);
            File.Delete(inMemorySnapshotStorePath);

            IEventStorageProvider<Schedule, Guid, Guid> eventStorage = 
                new InMemoryEventStorageProvider<Schedule, Guid, Guid>(inMemoryEventStorePath);
            ISnapshotStorageProvider<ScheduleSnapshot, Guid, Guid> snapshotStorage = 
                new InMemorySnapshotStorageProvider<ScheduleSnapshot, Guid, Guid>(2, inMemorySnapshotStorePath);

            _clock = new MockClock();
            _eventPublisher = new MockEventPublisher<Schedule, Guid, Guid>();
            _repository = new Repository<Schedule, ScheduleSnapshot, Guid, Guid, Guid>(_clock, eventStorage, _eventPublisher, snapshotStorage);
            _eventOnlyRepository = new EventOnlyRepository<Schedule, Guid, Guid>(_clock, eventStorage, _eventPublisher);
        }

        [Fact]
        public async Task WhenCreatingSchedule_ItGetsCreated_AndCanBe_ReloadedFromTheRepository()
        {
            var scheduleName = "test schedule";
            var schedule = new Schedule(scheduleName);
            await _repository.SaveAsync(schedule);

            var reloadedSchedule = await _repository.GetByIdAsync(schedule.Id);
            reloadedSchedule.Id.ShouldBe(schedule.Id);
            reloadedSchedule.ScheduleName.ShouldBe(scheduleName);
            reloadedSchedule.StreamState.ShouldBe(StreamState.HasStream);
            reloadedSchedule.CurrentVersion.ShouldBe(0);
            reloadedSchedule.LastCommittedVersion.ShouldBe(0);

            var events = _eventPublisher.Events[reloadedSchedule.Id];
            events.Count.ShouldBe(1);
            events.First().EventCommittedTimestamp.ShouldBe(_clock.Value);
        }

        [Fact]
        public async Task WhenCreatingSchedule_AndDoingChanges_ItGetsSaved_AndCanBe_ReloadedFromTheEventOnlyRepository()
        {
            var scheduleName = "test schedule";
            var schedule = new Schedule(scheduleName);
            await _eventOnlyRepository.SaveAsync(schedule);

            Enumerable.Range(1, 5).ToList().ForEach(async i =>
            {
                var tmpSchedule = await _eventOnlyRepository.GetByIdAsync(schedule.Id);
                tmpSchedule.AddTodo($"Todo {i}");
                await _eventOnlyRepository.SaveAsync(tmpSchedule);
            });

            var reloadedSchedule = await _eventOnlyRepository.GetByIdAsync(schedule.Id);
            reloadedSchedule.Id.ShouldBe(schedule.Id);
            reloadedSchedule.ScheduleName.ShouldBe(scheduleName);
            reloadedSchedule.Todos.Count.ShouldBe(5);
            reloadedSchedule.Todos.All(t => t.Text == $"Todo {reloadedSchedule.Todos.IndexOf(t) + 1}").ShouldBeTrue();
            reloadedSchedule.StreamState.ShouldBe(StreamState.HasStream);
            reloadedSchedule.CurrentVersion.ShouldBe(5);
            reloadedSchedule.LastCommittedVersion.ShouldBe(5);

            var events = _eventPublisher.Events[reloadedSchedule.Id];
            events.Count.ShouldBe(6);
            events.First().EventCommittedTimestamp.ShouldBe(_clock.Value);
            events.Last().EventCommittedTimestamp.ShouldBe(_clock.Value);
        }

        [Fact]
        public async Task WhenCreatingSchedule_AndDoingChanges_ItGetsSaved_AndCanBe_ReloadedFromTheRepository()
        {
            var scheduleName = "test schedule";
            var schedule = new Schedule(scheduleName);
            await _repository.SaveAsync(schedule);

            Enumerable.Range(1, 5).ToList().ForEach(async i =>
            {
                var tmpSchedule = await _eventOnlyRepository.GetByIdAsync(schedule.Id);
                tmpSchedule.AddTodo($"Todo {i}");
                await _eventOnlyRepository.SaveAsync(tmpSchedule);
            });

            var reloadedSchedule = await _eventOnlyRepository.GetByIdAsync(schedule.Id);
            reloadedSchedule.Id.ShouldBe(schedule.Id);
            reloadedSchedule.ScheduleName.ShouldBe(scheduleName);
            reloadedSchedule.Todos.Count.ShouldBe(5);
            reloadedSchedule.Todos.All(t => t.Text == $"Todo {reloadedSchedule.Todos.IndexOf(t) + 1}").ShouldBeTrue();
            reloadedSchedule.StreamState.ShouldBe(StreamState.HasStream);
            reloadedSchedule.CurrentVersion.ShouldBe(5);
            reloadedSchedule.LastCommittedVersion.ShouldBe(5);

            var events = _eventPublisher.Events[reloadedSchedule.Id];
            events.Count.ShouldBe(6);
            events.First().EventCommittedTimestamp.ShouldBe(_clock.Value);
            events.Last().EventCommittedTimestamp.ShouldBe(_clock.Value);
        }
    }
}

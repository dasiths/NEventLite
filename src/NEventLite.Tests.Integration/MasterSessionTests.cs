using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Repository;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;
using NEventLite.Storage;
using NEventLite.StorageProviders.InMemory;
using NEventLite.Tests.Integration.Mocks;
using Shouldly;
using Xunit;

namespace NEventLite.Tests.Integration
{
    public class MasterSessionTests
    {
        private class MockObject : IAggregateRoot
        {
        }

        private readonly MockEventPublisher _eventPublisher;
        private readonly MockClock _clock;
        private readonly IRepository<Schedule, Guid, Guid> _repository;
        private readonly IRepository<Schedule, Guid, Guid> _eventOnlyRepository;
        const int SnapshotFrequency = 2;

        public MasterSessionTests()
        {
            //This path is used to save in memory storage
            var strTempDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";

            //create temp directory if it doesn't exist
            new FileInfo(strTempDataFolderPath).Directory?.Create();

            var inMemoryEventStorePath = $@"{strTempDataFolderPath}events.stream.dump";
            var inMemorySnapshotStorePath = $@"{strTempDataFolderPath}events.snapshot.dump";

            File.Delete(inMemoryEventStorePath);
            File.Delete(inMemorySnapshotStorePath);

            IEventStorageProvider<Guid> eventStorage =
                new InMemoryEventStorageProvider(inMemoryEventStorePath);

            ISnapshotStorageProvider<Guid> snapshotStorage =
                new InMemorySnapshotStorageProvider(SnapshotFrequency, inMemorySnapshotStorePath);

            _clock = new MockClock();
            _eventPublisher = new MockEventPublisher();
            _repository = new Repository<Schedule, ScheduleSnapshot, Guid, Guid, Guid>(_clock, eventStorage, _eventPublisher, snapshotStorage);
            _eventOnlyRepository = new EventOnlyRepository<Schedule, Guid, Guid>(_clock, eventStorage, _eventPublisher);
        }

        [Fact]
        public async Task WhenCreatingSchedule_ItGetsCreated_AndCanBe_ReloadedFromTheRepository()
        {
            Func<Type, object> factory = (t) =>
            {
                if (t == typeof(IRepository<Schedule, Guid, Guid>))
                {
                    return _repository;
                }

                throw new Exception("Invalid type requested");
            };

            using (var session = new MasterSession(new MasterSession.ServiceFactory(factory)))
            {
                var scheduleName = "test schedule";
                var schedule = new Schedule(scheduleName);
                session.Attach(schedule);
                await session.SaveAsync();
                session.DetachAll();

                var reloadedSchedule = await session.GetByIdAsync<Schedule>(schedule.Id);
                reloadedSchedule.Id.ShouldBe(schedule.Id);
                reloadedSchedule.ScheduleName.ShouldBe(scheduleName);
                reloadedSchedule.StreamState.ShouldBe(StreamState.HasStream);
                reloadedSchedule.CurrentVersion.ShouldBe(0);
                reloadedSchedule.LastCommittedVersion.ShouldBe(0);

                var events = _eventPublisher.Events[reloadedSchedule.Id];
                events.Count.ShouldBe(1);
                events.First().EventCommittedTimestamp.ShouldBe(_clock.Value);
            }
        }

        [Fact]
        public async Task WhenCreatingSchedule_ItGetsCreated_AndCanBe_ReloadedFromTheEventOnlyRepository()
        {
            Func<Type, object> factory = (t) =>
            {
                if (t == typeof(IRepository<Schedule, Guid, Guid>))
                {
                    return _eventOnlyRepository;
                }

                throw new Exception("Invalid type requested");
            };

            using (var session = new MasterSession(new MasterSession.ServiceFactory(factory)))
            {
                var scheduleName = "test schedule";
                var schedule = new Schedule(scheduleName);
                session.Attach(schedule);
                await session.SaveAsync();
                session.DetachAll();

                var reloadedSchedule = await session.GetByIdAsync<Schedule>(schedule.Id);
                reloadedSchedule.Id.ShouldBe(schedule.Id);
                reloadedSchedule.ScheduleName.ShouldBe(scheduleName);
                reloadedSchedule.StreamState.ShouldBe(StreamState.HasStream);
                reloadedSchedule.CurrentVersion.ShouldBe(0);
                reloadedSchedule.LastCommittedVersion.ShouldBe(0);

                var events = _eventPublisher.Events[reloadedSchedule.Id];
                events.Count.ShouldBe(1);
                events.First().EventCommittedTimestamp.ShouldBe(_clock.Value);
            }
        }

        [Fact]
        public void WhenCreatingSchedule_MasterSessionThrowException_IfInvalidAggregateTypeIsPassed()
        {
            Func<Type, object> factory = (t) => throw new Exception("Invalid type requested");

            using (var session = new MasterSession(new MasterSession.ServiceFactory(factory)))
            {
                Should.Throw<ArgumentException>(() =>
                {
                    session.Attach(new MockObject());
                });
            }
        }

        [Fact]
        public void WhenCreatingSchedule_MasterSessionThrowException_IfNoRepositoryIsFound()
        {
            Func<Type, object> factory = (t) => null;

            using (var session = new MasterSession(new MasterSession.ServiceFactory(factory)))
            {
                var scheduleName = "test schedule";
                var schedule = new Schedule(scheduleName);

                Should.Throw<ArgumentException>(() =>
                {
                    session.Attach(schedule);
                });
            }
        }

        [Fact]
        public async Task WhenCreatingSchedule_MasterSessionThrowException_IfInvalidIdTypeIsPassed()
        {
            Func<Type, object> factory = (t) =>
            {
                if (t == typeof(IRepository<Schedule, Guid, Guid>))
                {
                    return _repository;
                }

                throw new Exception("Invalid type requested");
            };

            using (var session = new MasterSession(new MasterSession.ServiceFactory(factory)))
            {
                var scheduleName = "test schedule";
                var schedule = new Schedule(scheduleName);
                session.Attach(schedule);
                await session.SaveAsync();
                session.DetachAll();

                await Should.ThrowAsync<ArgumentException>(async () =>
                {
                    await session.GetByIdAsync<Schedule>(schedule.Id.ToString());
                });
            }
        }
    }
}

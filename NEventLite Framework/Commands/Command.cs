using System;

namespace NEventLite.Commands
{
    public class Command:ICommand
    {
        public Guid Id { get; private set; }
        public Guid AggregateId { get; }
        public int TargetVersion { get; private set; }

        public Command(Guid id, Guid aggregateId, int targetVersion)
        {
            Id = id;
            AggregateId = aggregateId;
            TargetVersion = targetVersion;
        }
    }
}

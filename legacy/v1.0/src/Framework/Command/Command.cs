using System;

namespace NEventLite.Command
{
    public class Command:ICommand
    {
        public Guid CorrelationId { get; private set; }
        public Guid AggregateId { get; }
        public int TargetVersion { get; private set; }

        public Command(Guid CorrelationId, Guid aggregateId, int targetVersion)
        {
            this.CorrelationId = CorrelationId;
            AggregateId = aggregateId;
            TargetVersion = targetVersion;
        }
    }
}

using System;

namespace NEventLite.Commands
{
    public class Command:ICommand
    {
        public Guid Id { get; private set; }
        public int TargetVersion { get; private set; }

        public Command(Guid id, int targetVersion)
        {
            Id = id;
            TargetVersion = targetVersion;
        }
    }
}

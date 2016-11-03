using System;

namespace NEventLite_Example.Snapshot
{
    public class NoteSnapshot : NEventLite.Snapshot.Snapshot
    {
        public DateTime CreatedDate { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }

        public NoteSnapshot(Guid id, Guid aggregateId, int version, DateTime createdDate, string title, string description, string category) :
            base(id, aggregateId, version)
        {
            CreatedDate = createdDate;
            Title = title;
            Description = description;
            Category = category;
        }
    }
}

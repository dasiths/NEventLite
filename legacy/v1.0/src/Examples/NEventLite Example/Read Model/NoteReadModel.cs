using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite_Example.Read_Model
{
    public class NoteReadModel
    {

        public Guid Id { get; private set; }
        public int CurrentVersion { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public NoteReadModel(Guid id, DateTime createdDate, string title, string description, string category)
        {
            Id = id;
            CreatedDate = createdDate;
            Title = title;
            Description = description;
            Category = category;
        }
    }
}

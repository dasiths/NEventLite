using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NEventLite_Example.Read_Model;

namespace NEventLite_Example_MVC.Models
{
    public class NoteDto
    {
        public Guid Id { get; private set; }
        public int CurrentVersion { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public NoteDto(Guid id, DateTime createdDate, string title, string description, string category)
        {
            Id = id;
            CreatedDate = createdDate;
            Title = title;
            Description = description;
            Category = category;
            CurrentVersion = 0;
        }

        public NoteDto(NoteReadModel note) : this(note.Id, note.CreatedDate, note.Title, note.Description, note.Category)
        {
            this.CurrentVersion = note.CurrentVersion;
        }
    }
}
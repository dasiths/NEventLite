using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite_Example.Read_Model
{
    public class MyReadModelStorage
    {
        private readonly List<NoteReadModel> _allNotes = new List<NoteReadModel>();

        public void AddOrUpdate(NoteReadModel note)
        {
            if (_allNotes.Any(o => o.Id==note.Id))
            {
                _allNotes.Remove(_allNotes.Single((o => o.Id == note.Id)));
                _allNotes.Add(note);
            }
            else
            {
                _allNotes.Add(note);
            }
            
        }

        public NoteReadModel GetByID(Guid Id)
        {
            return _allNotes.FirstOrDefault(o => o.Id == Id);
        }

        public IEnumerable<NoteReadModel> GetAll()
        {
            return _allNotes.ToList();
        }
    }
}

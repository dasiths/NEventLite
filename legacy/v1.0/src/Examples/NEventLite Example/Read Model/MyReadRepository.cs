using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite_Example.Read_Model
{
    public class MyReadRepository
    {
        private readonly MyInMemoryReadModelStorage _stoage;

        public MyReadRepository(MyInMemoryReadModelStorage stoage)
        {
            _stoage = stoage;
        }

        public void AddNote(NoteReadModel note)
        {
            _stoage.AddOrUpdate(note);
        }

        public void SaveNote(NoteReadModel note)
        {
            _stoage.AddOrUpdate(note);
        }

        public NoteReadModel GetNote(Guid Id)
        {
            return _stoage.GetByID(Id);
        }

        public IEnumerable<NoteReadModel> GetAllNotes()
        {
            return _stoage.GetAll();
        }
    }
}

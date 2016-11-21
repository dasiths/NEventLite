using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite_Storage_Providers.Util;

namespace NEventLite_Example.Read_Model
{
    public class MyInMemoryReadModelStorage
    {
        private readonly string _memoryDumpFile;
        private readonly List<NoteReadModel> _allNotes = new List<NoteReadModel>();

        public MyInMemoryReadModelStorage(string memoryDumpFile)
        {
            _memoryDumpFile = memoryDumpFile;

            if (File.Exists(_memoryDumpFile))
            {
                _allNotes = SerializerHelper.LoadListFromFile<NoteReadModel>(_memoryDumpFile);
            }
        }

        public void AddOrUpdate(NoteReadModel note)
        {
            if (_allNotes.Any(o => o.Id == note.Id))
            {
                _allNotes.Remove(_allNotes.Single((o => o.Id == note.Id)));
                _allNotes.Add(note);
            }
            else
            {
                _allNotes.Add(note);
            }
            
            SerializerHelper.SaveListToFile<NoteReadModel>(_memoryDumpFile, _allNotes);
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

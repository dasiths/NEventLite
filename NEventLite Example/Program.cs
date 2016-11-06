using System;
using System.Runtime.InteropServices;
using NEventLite.Session;
using NEventLite.Storage;
using NEventLite_Example.Domain;
using NEventLite_Example.Unit_Of_Work;
using NEventLite_Example.Util;

namespace NEventLite_Example
{
    class Program
    {

        //We keep a static variable for storage incase we decide to use the InMemoeryEventStorageProvider
        private static IEventStorageProvider EventStorage = null;
        private static ISnapshotStorageProvider SnapshotStorage = null;

        static void Main(string[] args)
        {
            //Load dependency resolver
            var resolver = new DependencyResolver();

            //We are reusing static data stores here because of InMemoryStorage example requires it. Don't use same storage instance in production.
            //You can create a new instance of Storage for other providers.
            EventStorage = resolver.Resolve<IEventStorageProvider>();
            SnapshotStorage = resolver.Resolve<ISnapshotStorageProvider>();

            //Set snapshot frequency
            SnapshotStorage.SnapshotFrequency = 5;

            Guid SavedItemID = CreateAndDoSomeChanges();

            if (SavedItemID != Guid.Empty)
            {
                LoadAndDisplayPreviouslySaved(SavedItemID);
            }

            Console.WriteLine();
            Console.WriteLine("Press enter key to exit.");

            Console.ReadLine();

        }

        #region Create And Change

        public static Guid CreateAndDoSomeChanges()
        {
            var UnitWork = GetUnitOfWork();

            Note tmpNote = null;

            //Try to load a given guid. Example: 76bc9edb-9857-4e2a-9fa0-762b90844119
            Console.WriteLine("Enter a GUID to try to load or leave blank and press enter:");
            string strGUID = Console.ReadLine();
            Guid LoadID;

            if ((string.IsNullOrEmpty(strGUID) == false) && (Guid.TryParse(strGUID, out LoadID)))
            {
                tmpNote = LoadNote(LoadID, UnitWork);

                if (tmpNote == null)
                {
                    Console.WriteLine($"No Note found with provided Guid of {LoadID.ToString()}. Press enter key to exit.");
                    return Guid.Empty;
                }
            }
            else
            {
                //Create new note
                tmpNote = CreateNewNote(UnitWork);

                Console.WriteLine("After Creation: This is version 0 of the AggregateRoot.");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNote));
                Console.WriteLine();
            }

            Console.WriteLine("Doing some changes now...");
            Console.WriteLine("");

            //Do some changes
            DoChanges(tmpNote, UnitWork);

            Console.WriteLine("");
            Console.WriteLine("After Committing Events:");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNote));

            return tmpNote.Id;

        }

        private static Note CreateNewNote(MyUnitOfWork rep)
        {
            Note tmpNote = new Note("Test Note", "Event Sourcing System Demo", "Event Sourcing");
            rep.NoteRepository.Add(tmpNote);
            return tmpNote;
        }

        private static void DoChanges(Note tmpNote, MyUnitOfWork rep)
        {
            //Do 3 x 5 events cycle to check snapshots too.
            for (int i = 0; i < 3; i++)
            {
                for (int x = 0; x < 5; x++)
                {
                    tmpNote.ChangeTitle($"Test Note 123 Event ({tmpNote.CurrentVersion + 1})");
                    tmpNote.ChangeCategory($"Event Sourcing in .NET Example. Event ({tmpNote.CurrentVersion + 1})");
                }

                Console.WriteLine($"Committing Changes Now For Cycle {i}");

                //Commit changes to the repository
                rep.Commit();
            }

            //Do some changes that don't get caught in the snapshot
            tmpNote.ChangeTitle($"Test Note 123 Event ({tmpNote.CurrentVersion + 1})");
            tmpNote.ChangeCategory($"Event Sourcing in .NET Example. Event ({tmpNote.CurrentVersion + 1})");
            //Commit changes to the repository
            rep.Commit();
        }

        #endregion

        #region Load Aggregate

        public static void LoadAndDisplayPreviouslySaved(Guid AggregateID)
        {
            //Load same note using the aggregate id
            //This will replay the saved events and construct a new note
            var tmpNoteToLoad = LoadNote(AggregateID, GetUnitOfWork());

            Console.WriteLine("");
            Console.WriteLine("After Replaying:");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNoteToLoad));
        }

        private static Note LoadNote(Guid NoteID, MyUnitOfWork rep)
        {
            var tmpNoteToLoad = rep.NoteRepository.GetById(NoteID);
            return tmpNoteToLoad;
        }

        #endregion

        private static MyUnitOfWork GetUnitOfWork()
        {
            return new MyUnitOfWork(EventStorage, SnapshotStorage);
        }


    }
}

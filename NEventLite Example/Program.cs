using System;
using System.Runtime.InteropServices;
using NEventLite.Repository;
using NEventLite.Storage;
using NEventLite_Example.Commands;
using NEventLite_Example.Command_Handlers;
using NEventLite_Example.Domain;
using NEventLite_Example.Util;

namespace NEventLite_Example
{
    class Program
    {

        static void Main(string[] args)
        {
            //Load dependency resolver
            var resolver = new DependencyResolver();
            
            //Set snapshot frequency
            resolver.Resolve<ISnapshotStorageProvider>().SnapshotFrequency = 5;

            //Get ioc conatainer to create our repository
            IRepository<Note> rep = resolver.Resolve<IRepository<Note>>();
            NoteCommandHandler commandHandler = new NoteCommandHandler(rep);

            DoMockRun(rep,commandHandler);

            Console.WriteLine();
            Console.WriteLine("Press enter key to exit.");

            Console.ReadLine();

        }

        private static Guid DoMockRun(IRepository<Note> rep, NoteCommandHandler commandHandler)
        {
            Note tmpNote = null;
            Guid LoadID = Guid.Empty;

            //Try to load a given guid.
            Console.WriteLine("Enter a GUID to try to load or leave blank and press enter:");
            string strGUID = Console.ReadLine();


            if ((string.IsNullOrEmpty(strGUID) == false) && (Guid.TryParse(strGUID, out LoadID)))
            {
                tmpNote = rep.GetById(LoadID);

                if (tmpNote == null)
                {
                    Console.WriteLine($"No Note found with provided Guid of {LoadID.ToString()}. Press enter key to exit.");
                    return Guid.Empty;
                }
            }
            else
            {
                //Create new note
                CreateNewNote(rep, commandHandler);
                tmpNote = rep.GetById(commandHandler.LastCreatedNoteGuid);

                Console.WriteLine("After Creation: This is version 0 of the AggregateRoot.");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNote));
                Console.WriteLine();
            }

            Console.WriteLine("Doing some changes now...");
            Console.WriteLine("");

            //Do some changes
            DoChanges(tmpNote.Id, rep,commandHandler);

            //Load to display
            var noteToLoad = rep.GetById(tmpNote.Id);

            Console.WriteLine("");
            Console.WriteLine("After Committing Events:");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(noteToLoad));

            return tmpNote.Id;

        }

        private static void CreateNewNote(IRepository<Note> rep, NoteCommandHandler commandHandler)
        {
            commandHandler.Handle(new CreateNoteCommand(Guid.NewGuid(),-1, "Test Note", "Event Sourcing System Demo", "Event Sourcing"));
        }

        private static void DoChanges(Guid NoteID, IRepository<Note> rep, NoteCommandHandler commandHandler)
        {
            var tmpNote = rep.GetById(NoteID);
            int LastVersion = tmpNote.CurrentVersion;

            //Do 12 events cycle to check snapshots too.
            for (int i = 1; i <= 12; i++)
            {
                Console.WriteLine($"Applying Changes For Cycle {i}");

                LastVersion = commandHandler.Handle(
                                    new EditNoteCommand(
                                        Guid.NewGuid(), 
                                        tmpNote.Id,
                                        LastVersion,
                                        $"Test Note 123 Event ({LastVersion + 1})",
                                        $"Event Sourcing in .NET Example. Event ({LastVersion + 2})"));
            }

        }

    }
}

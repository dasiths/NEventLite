using System;
using System.Runtime.InteropServices;
using NEventLite.Logger;
using NEventLite.Repository;
using NEventLite.Storage;
using NEventLite_Example.Commands;
using NEventLite_Example.Command_Handlers;
using NEventLite_Example.Domain;
using NEventLite_Example.Repository;
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

            //Set logger
            LogManager.Logger = resolver.Resolve<ILogger>();

            //Get ioc container to create our repository
            NoteRepository rep = new NoteRepository(resolver.Resolve<IRepository<Note>>());
            NoteCommandHandler commandHandler = new NoteCommandHandler(rep);

            DoMockRun(rep,commandHandler);

            Console.WriteLine();
            Console.WriteLine("Press enter key to exit.");

            Console.ReadLine();

        }

        private static void DoMockRun(NoteRepository rep, NoteCommandHandler commandHandler)
        {
            Guid SavedItemID = Guid.Empty;

            //Try to load a given guid.
            Console.WriteLine("Enter a GUID to try to load or leave blank and press enter:");
            string strGUID = Console.ReadLine();


            if ((string.IsNullOrEmpty(strGUID) == false) && (Guid.TryParse(strGUID, out SavedItemID)))
            {
                Note tmpNote = rep.GetById(SavedItemID);

                if (tmpNote == null)
                {
                    Console.WriteLine($"No Note found with provided Guid of {SavedItemID.ToString()}. Press enter key to exit.");
                    return;
                }
            }
            else
            {
                //Create new note
                commandHandler.Handle(new CreateNoteCommand(Guid.NewGuid(), -1, "Test Note","Event Sourcing System Demo", "Event Sourcing"));

                SavedItemID = commandHandler.LastCreatedNoteGuid;
                Note tmpNote = rep.GetById(SavedItemID);

                Console.WriteLine("After Creation: This is version 0 of the AggregateRoot.");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNote));
                Console.WriteLine();
            }

            Console.WriteLine("Doing some changes now...");
            Console.WriteLine("");

            //Reload and do some changes
            int LastVersion = rep.GetById(SavedItemID).CurrentVersion;

            //Do 12 events cycle to check snapshots too.
            for (int i = 1; i <= 12; i++)
            {
                Console.WriteLine($"Applying Changes For Cycle {i}");

                LastVersion = commandHandler.Handle(
                                    new EditNoteCommand(
                                        Guid.NewGuid(),
                                        SavedItemID,
                                        LastVersion,
                                        $"Test Note 123 Event ({LastVersion + 1})",
                                        $"Event Sourcing in .NET Example. Event ({LastVersion + 2})"));
            }

            //Load to display
            var noteToLoad = rep.GetById(SavedItemID);

            Console.WriteLine("");
            Console.WriteLine("After Committing Events:");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(noteToLoad));

        }
    }
}

using System;
using System.Runtime.InteropServices;
using NEventLite.Command_Handlers;
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
            using (var container = new DependencyResolver())
            {
                //Set logger
                LogManager.AddLogger(container.Resolve<ILogger>());

                using (new MyLifeTimeScope())
                {
                    DoMockRun(container);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press enter key to exit.");

            Console.ReadLine();

        }

        private static void DoMockRun(DependencyResolver container)
        {
            //Get ioc container to create our repository
            NoteRepository rep = container.Resolve<NoteRepository>();
            var createCommandHandler = container.Resolve<ICommandHandler<CreateNoteCommand>>();
            var editCommandHandler = container.Resolve<ICommandHandler<EditNoteCommand>>();

            Guid SavedItemID = Guid.Empty;

            //Try to load a given guid.
            Console.WriteLine("Enter a GUID to try to load or leave blank and press enter:");
            string strGuid = Console.ReadLine();


            if ((string.IsNullOrEmpty(strGuid) == false) && (Guid.TryParse(strGuid, out SavedItemID)))
            {
                Note tmpNote = rep.GetById(SavedItemID);

                if (tmpNote == null)
                {
                    Console.WriteLine($"No Note found with provided Guid of {SavedItemID}. Press enter key to exit.");
                    return;
                }
            }
            else
            {
                //Create new note
                Guid newItemId = Guid.NewGuid();

                createCommandHandler.Handle(
                    new CreateNoteCommand(Guid.NewGuid(), newItemId, -1,
                    "Test Note", "Event Sourcing System Demo", "Event Sourcing"));

                Note tmpNote = rep.GetById(newItemId);

                Console.WriteLine("After Creation: This is version 0 of the AggregateRoot.");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNote));
                Console.WriteLine();

                SavedItemID = newItemId;
            }

            Console.WriteLine("Doing some changes now...");

            //Reload and do some changes
            int LastVersion = rep.GetById(SavedItemID).CurrentVersion;

            //Do 12 events cycle to check snapshots too.
            for (int i = 1; i <= 12; i++)
            {
                Console.WriteLine($"Applying Changes For Cycle {i}");

                LastVersion = editCommandHandler.Handle(
                                new EditNoteCommand(Guid.NewGuid(), SavedItemID, LastVersion,
                                    $"Test Note 123 Event ({LastVersion + 1})",
                                    $"Event Sourcing in .NET Example. Event ({LastVersion + 2})")).AggregateVersion;
            }

            Console.WriteLine("Finished applying changes.");

            //Load to display
            var noteToLoad = rep.GetById(SavedItemID);

            Console.WriteLine("");
            Console.WriteLine("After Committing Events:");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(noteToLoad));

        }
    }
}

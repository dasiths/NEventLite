using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NEventLite.Command_Bus;
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
                    DoMockRun(container).Wait();
                }
            }

            Console.WriteLine("Press enter key to exit. \n");
            Console.Read();
        }

        private static async Task DoMockRun(DependencyResolver container)
        {
            //Get ioc container to create our repository
            NoteRepository rep = container.Resolve<NoteRepository>();
            var commandBus = container.Resolve<ICommandBus>();

            Guid SavedItemID = Guid.Empty;

            //Try to load a given guid.
            LogManager.Log("Enter a GUID to try to load or leave blank and press enter:", LogSeverity.Warning);
            string strGuid = Console.ReadLine();


            if ((string.IsNullOrEmpty(strGuid) == false) && (Guid.TryParse(strGuid, out SavedItemID)))
            {
                Note tmpNote = await rep.GetById<Note>(SavedItemID);

                if (tmpNote == null)
                {
                    LogManager.Log($"No Note found with Guid: {SavedItemID}.", LogSeverity.Critical);
                    return;
                }
            }
            else
            {
                //Create new note
                Guid newItemId = Guid.NewGuid();

                await commandBus.ExecuteAsync(
                    new CreateNoteCommand(Guid.NewGuid(), newItemId, -1,
                    "Test Note", "Event Sourcing System Demo", "Event Sourcing"));

                Note tmpNote = await rep.GetById<Note>(newItemId);

                LogManager.Log("After Creation: This is version 0 of the AggregateRoot.", LogSeverity.Information);
                LogManager.Log(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNote) + "\n", LogSeverity.Debug);

                SavedItemID = newItemId;
            }

            LogManager.Log("Doing some changes now... \n", LogSeverity.Debug);

            //Reload and do some changes
            int LastVersion = (await rep.GetById<Note>(SavedItemID)).CurrentVersion;

            //Do 12 events cycle to check snapshots too.
            for (int i = 1; i <= 12; i++)
            {
                LogManager.Log($"Applying Changes For Cycle {i}", LogSeverity.Debug);

                var result = await commandBus.ExecuteAsync(
                                new EditNoteCommand(Guid.NewGuid(), SavedItemID, LastVersion,
                                    $"Test Note 123 Event ({LastVersion + 1})",
                                    $"Event Sourcing in .NET Example. Event ({LastVersion + 2})"));

                LastVersion = result.AggregateVersion;
            }

            LogManager.Log("Finished applying changes. \n", LogSeverity.Debug);

            //Load to display
            Note noteToLoad = await rep.GetById<Note>(SavedItemID);

            LogManager.Log("After Committing Events:", LogSeverity.Information);
            LogManager.Log(Newtonsoft.Json.JsonConvert.SerializeObject(noteToLoad) + "\n", LogSeverity.Debug);

        }
    }
}

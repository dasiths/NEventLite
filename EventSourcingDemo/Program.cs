using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;

namespace EventSourcingDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            Note tmpNote = new Note("Test Note","Event Sourcing System Demo","Event Sourcing");

            Console.WriteLine("After Creation:");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNote));
            Console.WriteLine();

            tmpNote.ChangeTitle("Test Note 123");
            tmpNote.ChangeCategory("Event Sourcing in .NET Example");

            Console.WriteLine("After Events:");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tmpNote));

            Console.WriteLine();
            Console.WriteLine("Press enter key to exit.");

            Console.ReadLine();

        }
    }
}

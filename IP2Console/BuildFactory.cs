using System;
using System.Diagnostics;
using GeoNet;

namespace GeoNetConsole
{
    internal static class BuildFactory
    {
        public static void BuildLists()
        {

            Console.WriteLine("Building Binary File.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var fileLocation = Retriever.BuildProtoBufTrie(new[]
                                                               {
                                                                   NicLists.arin,
                                                                   NicLists.apnic,
                                                                   NicLists.lacnic,
                                                                   NicLists.ripencc,
                                                                   NicLists.afrinic
                                                               });

            stopwatch.Stop();
            Console.WriteLine("File Build in {0}ms", stopwatch.ElapsedMilliseconds);

            Console.WriteLine("FileLocation:");
            Console.WriteLine(fileLocation);
            Console.WriteLine(@"
IMPORTANT:

You must now place the geonet.bin file in the resources 
directory of your GeoNet project and compile.

");


            //GeoLookup.BuildFrom(lists);

        }
    }
}

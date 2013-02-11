using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GeoNet;

namespace GeoNetConsole
{
    internal static class TestFactory
    {

        public static void TestGeoNet()
        {

            var stopwatch = new Stopwatch();

            Console.WriteLine("CLR {0}", Environment.Version);
            Console.WriteLine();


            //  initialize
            Console.WriteLine("Initializing...");
            stopwatch.Start();
            GeoLookup.Initialize();
            stopwatch.Stop();
            Console.WriteLine("... Data Load took {0}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine();

            // test
            Console.WriteLine("Testing 98 ipv4 addresses against known countries...");
            using (var t = new StringReader(IPLists.IP4Test))
            {
                LookupTest(t);
            }

            // compare
            Console.WriteLine();
            Console.WriteLine("Testing country equality...");
            var dk = GeoLookup.GetCountry("80.163.202.190"); // denmark
            var dk2 = GeoLookup.GetCountry("83.88.145.100"); // denmark

            if (dk.Equals(dk2) && dk == dk2)
            {
                Console.WriteLine("... Success!");
            }
            else
            {
                Console.WriteLine("... Failed to equate countries.");
            }

            Console.WriteLine();

            // speed test
            IEnumerable<string> countries;
            using (var t = new StringReader(IPLists.IP4List))
            {
                countries = LookupList(t);
            }
            Console.WriteLine("Reading and analysing 30,000 IP4 records");
            stopwatch.Reset();
            stopwatch.Start();
            foreach (var country in countries)
            {
                GeoLookup.GetCountry(country);
            }
            stopwatch.Stop();
            Console.WriteLine("... {0}ms @ {1} lookups per second", stopwatch.ElapsedMilliseconds, Math.Round(30000 / (stopwatch.ElapsedMilliseconds / 1000d)));

            Console.WriteLine();
            Console.WriteLine("Tests Complete, Press any key to Exit.");

        }

        // get our list of 30,000 IP addresses 
        private static IEnumerable<string> LookupList(TextReader t)
        {
            var list = new List<string>(30000);
            string address;
            while ((address = t.ReadLine()) != null)
            {
                list.Add(address);
            }
            return list;
        }

        // get our list of IPs and known countries
        private static void LookupTest(TextReader t)
        {
            string line;
            while ((line = t.ReadLine()) != null)
            {
                var parts = line.Split(',');
                if (GeoLookup.GetCountry(parts[0]).TwoLetterCode == parts[1]) continue;

                // failed
                Console.WriteLine("Test Failed on IP: {0} - expected '{1}', got '{2}'", parts[0], parts[1], GeoLookup.GetCountry(parts[0]).TwoLetterCode);
                return;
            }
            Console.WriteLine("... Success!");

        }

    }
}

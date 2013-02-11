/*

Copyright (C) 2013 A.J. Potts / Party Ark Ltd <alistair.potts@partyark.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy of this 
software and associated documentation files (the "Software"), to deal in the Software 
without restriction, including without limitation the rights to use, copy, modify, 
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies 
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ProtoBuf;

// allow the static method BuildProtoBufTrie to be accessed by our 
// Console builder
[assembly: InternalsVisibleTo("GeoNetConsole")]

namespace GeoNet
{

    internal sealed class Retriever
    {


        private readonly BitVectorTrie mTrie;

        // constructor - load the lists
        public Retriever()
        {
            using (var trieStream = new MemoryStream(Resources.geonet))
            {
                mTrie = Serializer.Deserialize<BitVectorTrie>(trieStream);
            }
        }

        // static utility to build the trie file
        public static string BuildProtoBufTrie(IEnumerable<byte[]> lists)
        {
            // initialize
            var trie = new BitVectorTrie();

            foreach (var list in lists)
            {
                using (var ms = new MemoryStream(list)) Read(ms, trie);
            }

            // serialize
            using (var trieStream = new MemoryStream())
            {
                Serializer.Serialize(trieStream, trie);
                using (var file = new FileStream("geonet.bin", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    trieStream.WriteTo(file);
                    return file.Name;
                }
            }
            // don't forget to add it to the resources!
        }


        // read from a nic stream into a trie - only used when
        // generating the binary file
        private static void Read(Stream ms, BitVectorTrie trie)
        {

            using (var nccin = new StreamReader(ms))
            {

                string line;
                while ((line = nccin.ReadLine()) != null)
                {
                    var data = line.Split('|');

                    // Make the following assumption:
                    // if 2nd entry is 2 chars, it's a country code, 
                    // and 3rd entry is 4 chars (ipv4 / ipv6) we're good to look up
                    if ((data.Length <= 3)) continue;

                    // the country is always upper case, so don't waste time upper-casing
                    // the scheme is always lower case too
                    var country = data[1];
                    var scheme = data[2];

                    if (country.Length != 2 || scheme.Length != 4) continue;

                    var ip = data[3];
                    switch (scheme)
                    {
                        case "ipv4":
                            trie.Add(Ip4ToBitVector(ip), country);
                            break;
                        case "ipv6":
                            trie.Add(Ip6ToBitVector(ip), country);
                            break;
                    }
                }
            }

        }


        // the key method
        public Country GetCountry(IPAddress ip)
        {


            BitVector key = null;
            switch (ip.AddressFamily)
            {
                case AddressFamily.InterNetwork:

                    // don't try and find a loopback
                    if (IPAddress.IsLoopback(ip)) { return new UnknownCountry(); }

                    key = Ip4ToBitVector(ip.ToString());
                    break;
                case AddressFamily.InterNetworkV6:
                    key = Ip6ToBitVector(ip.ToString());
                    break;
            }

            return CountryLookup.Lookup(mTrie.GetBest(key));

        }

        private static BitVector Ip4ToBitVector(string ip)
        {
            var elements = ip.Split('.');
            var bv = new BitVector();

            // only the first three bits are important
            for (var index = 0; index < Math.Min(elements.Length, 3); index++)
            {
                var s = elements[index];
                if (string.IsNullOrEmpty(s)) break;
                bv.AddData(int.Parse(s, CultureInfo.InvariantCulture), 8);
            }
            return bv;
        }


        private static BitVector Ip6ToBitVector(string ip)
        {
            var elements = ip.Split(':');
            var bv = new BitVector();

            // only the first three? parts are significant for geolocation
            for (var index = 0; index < Math.Min(elements.Length, 3); index++)
            {
                var h = elements[index];
                bv.AddData(
                    string.IsNullOrEmpty(h)
                        ? 0
                        : int.Parse(h, NumberStyles.HexNumber, CultureInfo.InvariantCulture), 16);

            }
            return bv;
        }


    }
}
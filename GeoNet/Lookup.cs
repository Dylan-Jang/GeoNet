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

using System.Net;

namespace GeoNet
{


    public static class GeoLookup
    {

        /* IpToCountry Class - guaranteed only to be instantiated once */
        private static readonly Retriever Retriever = new Retriever();


        /* initializer - not necessary but might be run in application start code */
        public static void Initialize() {
            // no body - but executing the method ensures that the Retriever is initialised...
        }

        /*  one method, two signatures */
        public static Country GetCountry(IPAddress address)
        {
            return Retriever.GetCountry(address);
        }

        public static Country GetCountry(string address)
        {
            IPAddress ipAddress;
            return IPAddress.TryParse(address, out ipAddress) ? 
                    GetCountry(ipAddress) : 
                    new UnknownCountry();
        }



    }

}

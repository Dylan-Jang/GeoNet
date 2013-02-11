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

namespace GeoNet
{

    public class Country : IEquatable<Country>
    {
        private readonly string name;
        public string Name { get { return name; } }

        private readonly string twoLetterCode;
        public string TwoLetterCode { get { return twoLetterCode; } }

        private readonly string threeLetterCode;
        public string ThreeLetterCode { get { return threeLetterCode; } }

        private readonly int numericCode;
        public int NumericCode { get { return numericCode; } }

        protected internal Country(string name, string twoLetterCode, string threeLetterCode, int numericCode)
        {
            this.name = name;
            this.twoLetterCode = twoLetterCode;
            this.threeLetterCode = threeLetterCode;
            this.numericCode = numericCode;
        }

        // comparison - numeric code should be unique so we'll use that
        public bool Equals(Country other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return numericCode == other.numericCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Country)obj);
        }

        public override int GetHashCode()
        {
            return numericCode;
        }

        public static bool operator ==(Country left, Country right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Country left, Country right)
        {
            return !Equals(left, right);
        }
    }


    // create an 'unknown country' to allow us to always return a country, even if we don't find a match

    public sealed class UnknownCountry : Country
    {
        internal UnknownCountry() : base(null, null, null, 0) { }
    }

}

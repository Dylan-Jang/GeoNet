/*

Copyright (C) 2013 A.J. Potts / Party Ark Ltd <alistair.potts@partyark.co.uk>
Portions Copyright (C) 2003 Rodrigo Reyes <reyes@charabia.net>

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
using System.Text;
using ProtoBuf;

namespace GeoNet
{

    [ProtoContract(SkipConstructor = true)]
    internal sealed class BitVector
    {
        [ProtoMember(1)]
        private byte[] mData;
        
        [ProtoMember(2)]
        private int mMaxoffset;

        public BitVector()
        {
            this.mData = new byte[0];
            this.mMaxoffset = -1;
        }

        public void AddData(long val, int length)
        {
            var offset = mMaxoffset + 1;
            for (var i = 0; i < length; i++)
            {
                Set(offset + i, (val & 1 << (length - i - 1)) != 0);
            }
        }

        public int Length
        {
            get { return mMaxoffset + 1; }
        }

        public void Set(int offset, bool val)
        {
            var byteoffset = offset / 8;
            var bitoffset = offset % 8;

            if (byteoffset >= mData.Length)
            {
                var data = new byte[byteoffset + 1];
                for (var i = 0; i < mData.Length; i++)
                {
                    data[i] = mData[i];
                }
                mData = data;
                mMaxoffset = offset;
            }
            else if (offset > mMaxoffset)
            {
                mMaxoffset = offset;
            }

            if (val)
                mData[byteoffset] |= (byte)(1 << (7 - bitoffset));
            else
                mData[byteoffset] &= (byte)(0xff - (1 << (7 - bitoffset)));

        }

        public bool Get(int offset)
        {
            if (offset > mMaxoffset)
            {
                throw new ArgumentOutOfRangeException("Out Of Bound offset " + offset);
            }
            var byteoffset = offset / 8;
            var bitoffset = offset % 8;

            if (byteoffset >= mData.Length)
            {
                throw new ArgumentOutOfRangeException("Out Of Bound byteoffset " + offset);
            }
            return (mData[byteoffset] & (1 << (7 - bitoffset))) != 0;
        }

        public BitVector Range(int start, int length)
        {
            var result = new BitVector();
            for (var i = start; i < (start + length); i++)
            {
                result.Set(i - start, Get(i));
            }
            return result;
        }

        public int LongestCommonPrefix(BitVector other)
        {
            var i = 0;
            while ((i <= other.mMaxoffset) && (i <= mMaxoffset))
            {
                if (other.Get(i) != Get(i))
                {
                    return i;
                }
                i++;
            }
            return i;
        }

        override public string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i <= mMaxoffset; i++)
            {
                sb.Append(Get(i) ? "1" : "0");
                if (i % 8 == 7)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }


    }
}
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

using System.Collections.Generic;
using ProtoBuf;

namespace GeoNet
{

    [ProtoContract]
    internal sealed class BitVectorTrie
    {

        [ProtoMember(2)]
        private readonly Node root;

        public BitVectorTrie()
        {
            this.root = new Node();
        }


        [ProtoContract]
        private class Node
        {
            [ProtoMember(1)]
            public List<Node> Children;
            [ProtoMember(2)]
            public BitVector Key;
            [ProtoMember(3)]
            public string Data;
        }
        
        internal void Add(BitVector key, string data)
        {
            Add(root, key, data);
        }

        //
        // This Add method attach an object "data" to a node "n" using
        // the BitVector key as path.
        //
        private void Add(Node n, BitVector key, string data)
        {
            if (n.Key == null)
            {
                AddAsChildren(n, key, data);
                return;
            }

            //
            // First, calculate the longest common prefix for the key
            // and the BitVector stored in this node.
            //
            var longest = key.LongestCommonPrefix(n.Key);

            //System.Diagnostics.Debug.Assert(longest != 0);

            if (longest == n.Key.Length)
            {
                //
                // If the current node is a perfect prefix of the
                // key, then remove the prefix from the key, and
                // we continue our walk on the children.
                //
                key = key.Range(longest, key.Length - longest);
                AddAsChildren(n, key, data);
                return;
            }
            //
            // Here, n.Key and key share a common prefix. So we:
            //
            // - Create a new node with this common prefix
            //   held there,
            //
            // - make n.Key and a new node with key as
            // children of this new node.
            var common = n.Key.Range(0, longest);

            var c1 = new Node
            {
                Key = n.Key.Range(longest, n.Key.Length - longest),
                Data = n.Data,
                Children = n.Children
            };

            var c2 = new Node { Key = key.Range(longest, key.Length - longest), Data = data };

            n.Key = common;
            n.Data = null;

            // initialize the list with a capacity: we save ourself
            // a few ms 
            n.Children = new List<Node>(2) { c1, c2 };


        }

        //
        // The AddAsChildren() method create a new node with key
        // "key", attach a data "data" to it, and finally link it to
        // the node "n".
        //
        private void AddAsChildren(Node n, BitVector key, string data)
        {


            // If "n" has no children, just add a new one
            //
            if (n.Children == null)
            {
                // initialize the list with a capacity: we save ourself
                // a few ms 
                n.Children = new List<Node>(2) { new Node { Key = key, Data = data } };
                return;
            }

            //
            // From here, the node n already has at least 1
            // children.

            // Check the one that has a common prefix with our key
            //(if there is none, the bestindex variable stays at -1).
            var bestindex = -1;
            var bestlength = 0;

            for (var i = 0; i < n.Children.Count; i++)
            {
                var b = n.Children[i].Key.LongestCommonPrefix(key);

                if (b <= bestlength) continue;
                bestlength = b;
                bestindex = i;
            }

            //
            // The node n has no children that have a common prefix
            // with our key, so we create a new children node and
            // attach our data there.
            if (bestindex == -1)
            {
                n.Children.Add(new Node { Key = key, Data = data });
                return;
            }
            // There is a children node that can hold our
            // data: continue our walk with this node.
            Add(n.Children[bestindex], key, data);

        }



        //
        // Returns the object held in the node that best matches our
        // key.
        //
        internal string GetBest(BitVector key)
        {
            var curnode = root;
            while (curnode != null)
            {
                if (curnode.Children == null)
                    return curnode.Data;

                // Get the best fitting index
                var bestindex = -1;
                var bestlength = 0;

                for (var i = 0; i < curnode.Children.Count; i++)
                {
                    var b = curnode.Children[i].Key.LongestCommonPrefix(key);
                    if (b <= bestlength) continue;
                    bestlength = b;
                    bestindex = i;
                }

                if (bestindex == -1)
                {
                    return curnode.Data;
                }

                key = key.Range(bestlength, key.Length - bestlength);
                curnode = curnode.Children[bestindex];
                if (key.Length == 0)
                {
                    return curnode.Data;
                }
            }

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ManagedMemory
{
    public class PointerPath
    {
        protected string baseModule;
        protected Address baseModuleAddress;
        protected int baseOffset;
        protected int[] pathOffsets;
        protected int destinationOffset;
        protected ProcessInterface callback;

        public PointerPath(string baseModule, int baseOffset, int[] pathOffsets, int destinationOffset, ProcessInterface callback)
        {
            this.baseModule = baseModule;
            this.pathOffsets = pathOffsets;
            this.destinationOffset = destinationOffset;
            this.callback = callback;
            this.baseOffset = baseOffset;
            baseModuleAddress = callback.GetModuleBase(baseModule);
            if (baseModuleAddress == null) throw new InvalidOperationException("the specified base module does not exist");
        }

        /*Creates a PointerPath from the formal notation. General notation template:
         * [[[BaseModuleName.Extension + 0xBaseOffset] + 0xLayerOneOffset] + 0xLayerTwoOffset ] + 0xFinalValueOffset
         * Each encapsulation by [] represents dereferencing the inner pointer
         * If you wish to dereference multiple times without adding offsets simply encapsulate multiple times.
         */
        public static PointerPath CreateFromFormalNotation(string expression, ProcessInterface callback)
        {
            expression = RemoveAll(expression, ' ');
            string moduleName = RemoveAllRange(expression, new char[] { '[', ']' });
            moduleName = moduleName.Substring(0, moduleName.IndexOf('+'));
            int baseOffset = HexToInt(expression.Substring(expression.IndexOf('+') + 1, expression.IndexOf(']') - expression.IndexOf('+') - 1));
            string offsets = expression.Remove(expression.IndexOf(moduleName[0]) - 1, expression.IndexOf(']') - expression.IndexOf(moduleName[0]) + 2);
            offsets = RemoveAll(offsets, ' ');
            ArrayList offsetCollection = new ArrayList();
            while (offsets.Contains(']'))
            {
                if (offsets[offsets.IndexOf(']') - 1] == '[' && offsets.IndexOf('+') > offsets.IndexOf(']'))
                {
                    offsetCollection.Add(0x0);
                    offsets = offsets.Remove(offsets.IndexOf(']') - 1, 2);
                }
                else
                {
                    string curOffset = offsets.Substring(offsets.IndexOf('+') + 1, offsets.IndexOf(']') - offsets.IndexOf('+') - 1);
                    offsetCollection.Add(HexToInt(curOffset));
                    offsets = offsets.Remove(offsets.IndexOf(']') - curOffset.Length - 2, curOffset.Length + 3);
                }

            }
            int finalOffset = HexToInt(offsets.Remove(offsets.IndexOf('+'), 1));
            int[] finOffsets = new int[offsetCollection.Count];
            for (int i = 0; i < offsetCollection.Count; i++)
            {
                finOffsets[i] = (int)offsetCollection[i];
            }
            return new PointerPath(moduleName, baseOffset, finOffsets, finalOffset, callback);
        }

        protected static int HexToInt(string hex)
        {
            hex = hex.Substring(2);
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        protected static string RemoveAllRange(string input, char[] targets)
        {
            string res = input;
            foreach (char c in targets)
            {
                res = RemoveAll(res, c);
            }
            return res;
        }

        protected static string RemoveAll(string input, char target)
        {
            string res = input;
            while (res.Contains(target))
            {
                res = res.Remove(res.IndexOf(target), 1);
            }
            return res;
        }

        //Traverses the PointerPath to find the final address, returns an External Variable initialized at the final address
        public ExternalVariable Traverse()
        {
            Address entryPointerAddress = baseModuleAddress.OffsetBy(baseOffset);
            Pointer entryPointer = new Pointer(entryPointerAddress, callback);
            Pointer currentPointer = new Pointer(entryPointer.GetDestination(), callback);

            foreach (int i in pathOffsets)
            {
                currentPointer = new Pointer(currentPointer.GetSource().OffsetBy(i), callback);
                currentPointer = new Pointer(currentPointer.GetDestination(), callback);
            }
            return new ExternalVariable(currentPointer.GetSource().OffsetBy(destinationOffset), callback);
        }

        public string GetFormalNotation()
        {
            string res = "[" + baseModule + " + 0x" + Convert.ToString(baseOffset, 16) + "] ";
            foreach (int i in pathOffsets)
            {
                if (i != 0)
                {
                    res = "[" + res + " + 0x" + Convert.ToString(i, 16) + " ]";
                }
                else
                {
                    res = "[" + res + "]";
                }
            }
            if (destinationOffset != 0)
            {
                return res + " + 0x" + Convert.ToString(destinationOffset, 16);
            }
            else return res;

        }

        public override string ToString()
        {
            Address entryPointerAddress = baseModuleAddress.OffsetBy(baseOffset);
            Pointer entryPointer = new Pointer(entryPointerAddress, callback);
            Pointer currentPointer = new Pointer(entryPointer.GetDestination(), callback);
            string res = "" + baseModuleAddress + " + " + baseOffset + " -> " + entryPointerAddress + "\n";
            foreach (int i in pathOffsets)
            {
                res += currentPointer.GetSource() + "+" + i + " -> ";
                currentPointer = new Pointer(currentPointer.GetSource().OffsetBy(i), callback);
                res += currentPointer.GetDestination() + "\n";
                currentPointer = new Pointer(currentPointer.GetDestination(), callback);
            }
            return res;
        }
    }
}

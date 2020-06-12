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
        private string baseModule;
        private Address baseModuleAddress;
        private int baseOffset;
        private int[] pathOffsets;
        private int destinationOffset;
        private ProcessInterface callback;

        public PointerPath(string baseModule, int baseOffset, int[] pathOffsets, int destinationOffset, ProcessInterface callback)
        {
            this.baseModule = baseModule;
            this.pathOffsets = pathOffsets;
            this.destinationOffset = destinationOffset;
            this.callback = callback;
            this.baseOffset = baseOffset;
            baseModuleAddress = callback.getModuleBase(baseModule);
            if (baseModuleAddress == null) throw new InvalidOperationException("the specified base module does not exist");
        }

        /*Creates a PointerPath from the formal notation. General notation template:
         * [[[BaseModuleName.Extension + 0xBaseOffset] + 0xLayerOneOffset] + 0xLayerTwoOffset ] + 0xFinalValueOffset
         * Where each layer that is encapsulated by [] represents one pointer jump.
         * In case of a double jump do not use +0x0 instead immediately close the current bracket 
         * For Example this expression contains a double jump: [[[[explorer.exe + 0x2570] + 0xC]] + 0xA] 0x10
         * Jumps may be stacked like this indefinitely.
         */
        public static PointerPath createFromFormalNotation(string expression, ProcessInterface callback)
        {
            expression = removeAll(expression, ' ');
            string moduleName = removeAllRange(expression, new char[] { '[', ']' });
            moduleName = moduleName.Substring(0, moduleName.IndexOf('+'));
            int baseOffset = hexToInt(expression.Substring(expression.IndexOf('+') + 1, expression.IndexOf(']') - expression.IndexOf('+') - 1));
            string offsets = expression.Remove(expression.IndexOf(moduleName[0]) - 1, expression.IndexOf(']') - expression.IndexOf(moduleName[0]) + 2);
            offsets = removeAll(offsets, ' ');
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
                    offsetCollection.Add(hexToInt(curOffset));
                    offsets = offsets.Remove(offsets.IndexOf(']') - curOffset.Length - 2, curOffset.Length + 3);
                }

            }
            int finalOffset = hexToInt(offsets.Remove(offsets.IndexOf('+'), 1));
            int[] finOffsets = new int[offsetCollection.Count];
            for (int i = 0; i < offsetCollection.Count; i++)
            {
                finOffsets[i] = (int)offsetCollection[i];
            }
            return new PointerPath(moduleName, baseOffset, finOffsets, finalOffset, callback);
        }

        private static int hexToInt(string hex)
        {
            hex = hex.Substring(2);
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        private static string removeAllRange(string input, char[] targets)
        {
            string res = input;
            foreach (char c in targets)
            {
                res = removeAll(res, c);
            }
            return res;
        }

        private static string removeAll(string input, char target)
        {
            string res = input;
            while (res.Contains(target))
            {
                res = res.Remove(res.IndexOf(target), 1);
            }
            return res;
        }

        //Traverses the PointerPath to find the final address, returns an External Variable initialized at the final address
        public ExternalVariable traverse()
        {
            Address entryPointerAddress = baseModuleAddress.offsetBy(baseOffset);
            Pointer entryPointer = new Pointer(entryPointerAddress, callback);
            Pointer currentPointer = new Pointer(entryPointer.getDestination(), callback);

            foreach (int i in pathOffsets)
            {
                currentPointer = new Pointer(currentPointer.getSource().offsetBy(i), callback);
                currentPointer = new Pointer(currentPointer.getDestination(), callback);
            }
            return new ExternalVariable(currentPointer.getSource().offsetBy(destinationOffset), callback);
        }

        public string getFormalNotation()
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
            Address entryPointerAddress = baseModuleAddress.offsetBy(baseOffset);
            Pointer entryPointer = new Pointer(entryPointerAddress, callback);
            Pointer currentPointer = new Pointer(entryPointer.getDestination(), callback);
            string res = "" + baseModuleAddress + " + " + baseOffset + " -> " + entryPointerAddress + "\n";
            foreach (int i in pathOffsets)
            {
                res += currentPointer.getSource() + "+" + i + " -> ";
                currentPointer = new Pointer(currentPointer.getSource().offsetBy(i), callback);
                res += currentPointer.getDestination() + "\n";
                currentPointer = new Pointer(currentPointer.getDestination(), callback);
            }
            return res;
        }
    }
}

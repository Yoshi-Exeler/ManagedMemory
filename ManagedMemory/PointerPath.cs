using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class PointerPath
    {
        private string baseModule;
        private Address64 baseModuleAddress;
        private int baseOffset;
        private int[] pathOffsets;
        private int destinationOffset;
        private ProcessInterface callback;

        public PointerPath(string baseModule,int baseOffset,int[] pathOffsets,int destinationOffset,ProcessInterface callback)
        {
            this.baseModule = baseModule;
            this.pathOffsets = pathOffsets;
            this.destinationOffset = destinationOffset;
            this.callback = callback;
            this.baseOffset = baseOffset;
            baseModuleAddress = callback.getModuleBase(baseModule);
            if (baseModuleAddress == null) throw new InvalidOperationException("the specified base module does not exist");
        }

        //Traverses the PointerPath to find the final address, returns an External Variable initialized at the final address
        public ExternalVariable traverse()
        {
            Address64 entryPointerAddress = baseModuleAddress.offsetBy(baseOffset);
            Pointer entryPointer = new Pointer(entryPointerAddress, callback);
            Pointer currentPointer = new Pointer(entryPointer.getDestination(), callback);

            foreach(int i in pathOffsets)
            {
                currentPointer = new Pointer(currentPointer.getSource().offsetBy(i), callback);
                currentPointer = new Pointer(currentPointer.getDestination(), callback);
            }
            return new ExternalVariable(currentPointer.getSource().offsetBy(destinationOffset),callback);
        }

        public override string ToString()
        {
            Address64 entryPointerAddress = baseModuleAddress.offsetBy(baseOffset);
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

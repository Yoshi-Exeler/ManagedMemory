using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    class Address64
    {
        private long address;

        public Address64(long adr)
        {
            address = adr;
        }
        public Address64(IntPtr adr)
        {
            address = (long)adr;
        }
        
        public static Address64 getAddressAtOffset(Address64 adr,int offset)
        {
            return new Address64(adr.getAddress() + offset);
        }

        //returns a new address which is offset by the parameter from the current address
        public Address64 offsetBy(int offset)
        {
            return new Address64(address + offset);
        }

        public long getAddress()
        {
            return address;
        }

        public IntPtr getAsPointer()
        {
            return (IntPtr)address;
        }

        public void setAddress(int adr)
        {
            address = adr;
        }

        public void setAddress(IntPtr adr)
        {
            address = (long)adr;
        }
    }
}

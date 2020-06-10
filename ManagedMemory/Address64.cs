using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class Address
    {
        private long address;

        public Address(long adr)
        {
            address = adr;
        }
        public Address(IntPtr adr)
        {
            address = (long)adr;
        }
        
        public static Address getAddressAtOffset(Address adr,int offset)
        {
            return new Address(adr.getAddress() + offset);
        }

        //returns a new address which is offset by the parameter from the current address
        public Address offsetBy(int offset)
        {
            return new Address(address + offset);
        }

        public long getAddress()
        {
            return address;
        }

        public IntPtr getAsPointer()
        {
            return (IntPtr)address;
        }

        public void setAddress(long adr)
        {
            address = adr;
        }

        public void setAddress(IntPtr adr)
        {
            address = (long)adr;
        }

        public override string ToString()
        {
            return "0x"+Convert.ToString(address, 16);
        }
    }
}

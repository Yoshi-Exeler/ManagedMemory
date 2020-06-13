using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class Address
    {
        protected long address;
        public Address(long adr)
        {
            address = adr;
        }
        public Address(IntPtr adr)
        {
            address = (long)adr;
        }

        public static Address GetAddressAtOffset(Address adr, int offset)
        {
            return new Address(adr.GetAddress() + offset);
        }

        public static Address Zero()
        {
            return new Address(IntPtr.Zero);
        }

        //returns a new address which is offset by the parameter from the current address
        public Address OffsetBy(int offset)
        {
            return new Address(address + offset);
        }

        public long GetAddress()
        {
            return address;
        }

        public IntPtr GetAsPointer()
        {
            return (IntPtr)address;
        }

        public void SetAddress(long adr)
        {
            address = adr;
        }

        public void SetAddress(IntPtr adr)
        {
            address = (long)adr;
        }

        public override string ToString()
        {
            return "0x" + Convert.ToString(address, 16);
        }
    }
}

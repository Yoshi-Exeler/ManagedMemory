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

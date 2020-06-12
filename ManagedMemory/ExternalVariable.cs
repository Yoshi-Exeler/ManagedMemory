using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class ExternalVariable
    {
        private Address address;
        private ProcessInterface callback;

        public ExternalVariable(Address adr, ProcessInterface pi)
        {
            callback = pi;
            address = adr;
        }

        public Address getAddress()
        {
            return address;
        }

        public void changeProtection(WINAPI.MemoryProtection newProtection, int sizeOfVariable)
        {
            uint oldProtection = 0;
            WINAPI.VirtualProtectEx(callback.getHandle(), getAddress().getAsPointer(), sizeOfVariable, (uint)newProtection, out oldProtection);
        }

        public int GetAsInt32()
        {
            return callback.ReadInt32(address);
        }

        public long GetAsInt64()
        {
            return callback.ReadInt64(address);
        }

        public float GetAsFloat()
        {
            return callback.ReadFloat(address);
        }

        public double GetAsDouble()
        {
            return callback.ReadDouble(address);
        }

        public byte GetAsByte()
        {
            return callback.ReadByte(address);
        }

        public byte[] GetAsByteArray(int size)
        {
            return callback.ReadByteArray(address, size);
        }

        public void WriteInt32(int val)
        {
            callback.WriteInt32(address, val);
        }

        public void WriteInt64(long val)
        {
            callback.WriteInt64(address, val);
        }

        public void WriteFloat(float val)
        {
            callback.WriteFloat(address, val);
        }

        public void WriteDouble(double val)
        {
            callback.WriteDouble(address, val);
        }

        public void WriteByte(byte val)
        {
            callback.WriteByte(address, val);
        }

        public void WriteByteArray(byte[] val)
        {
            callback.WriteByteArray(address, val);
        }
    }
}

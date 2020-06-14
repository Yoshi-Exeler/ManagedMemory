using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class ExternalVariable
    {
        protected Address address;
        protected ProcessInterface callback;

        public ExternalVariable(Address adr, ProcessInterface pi)
        {
            callback = pi;
            address = adr;
        }

        public Address GetAddress()
        {
            return address;
        }

        public uint ChangeProtection(APIProxy.MemoryProtection newProtection, int sizeOfVariable)
        {
            return APIProxy.VirtualProtectEx(callback.GetHandle(), GetAddress(), sizeOfVariable, (uint)newProtection);
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

        public Vec2D GetAsVec2D()
        {
            return callback.ReadVec2D(address);
        }

        public Vec3D GetAsVec3D()
        {
            return callback.ReadVec3D(address);
        }

        public Vec4D GetAsVec4D()
        {
            return callback.ReadVec4D(address);
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

        public void WriteVec2D(Vec2D val)
        {
            callback.WriteVec2D(address,val);
        }

        public void WriteVec3D(Vec3D val)
        {
            callback.WriteVec3D(address, val);
        }

        public void WriteVec4D(Vec4D val)
        {
            callback.WriteVec4D(address, val);
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

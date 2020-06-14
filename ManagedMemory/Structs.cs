using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public struct MemoryRegion
    {
        public Address start;
        public int lenght;
    }

    public struct Vec2D
    {
        public float x, y;
    }

    public struct Vec3D
    {
        public float x, y, z;
    }

    public struct Vec4D
    {
        public float x, y, z, w;
    }
}

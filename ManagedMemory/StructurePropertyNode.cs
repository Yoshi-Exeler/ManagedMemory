using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class StructurePropertyNode : MemoryGraphNode
    {
        //The structure base which this node is offset from
        protected StructureBaseNode parentStructure;
        protected int offset;

        public StructurePropertyNode(StructureBaseNode parent)
        {
            this.parentStructure = parent;
        }

        //returns the parent structure node
        public StructureBaseNode GetParentStructure()
        {
            return parentStructure;
        }

        //returns the offset from the Structure base
        public int GetOffset()
        {
            return offset;
        }



        
    }
}

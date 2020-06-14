using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class PointerPropertyNode : StructurePropertyNode
    {
        protected StructureBaseNode targetNode;
        //Forward the structueBaseNode to the base class constructor
        public PointerPropertyNode(StructureBaseNode structureBaseNode) : base(structureBaseNode) { }

        //Return the node this pointer is pointing to
        public StructureBaseNode GetTarget()
        {
            return targetNode;
        }

    }
}

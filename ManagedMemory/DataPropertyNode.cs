using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class DataPropertyNode : StructurePropertyNode
    {
        protected string name;

        //Forward the structueBaseNode to the base class constructor
        public DataPropertyNode(StructureBaseNode structureBaseNode) : base (structureBaseNode) { }

        //Returns the name of the data property
        public string GetName()
        {
            return name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ManagedMemory
{
    public class ModuleBaseNode : MemoryGraphNode
    {
        protected string moduleName;

        //Returns the name of the module this node represents
        public string GetModuleName()
        {
            return moduleName;
        }
    }
}

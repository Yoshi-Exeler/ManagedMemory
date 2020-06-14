using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ManagedMemory
{
    public class GraphRootNode
    {
        protected ArrayList moduleNodes;

        //Adds a module node to the moduleNodes collection
        public void addModuleNode(ModuleBaseNode moduleNode)
        {
            moduleNodes.Add(moduleNode);
        }

        //Returns the module with the specified name or null if no such module exists
        public ModuleBaseNode GetModuleByName(string name)
        {
            throw new NotImplementedException();
        } 
    }
}

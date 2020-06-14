using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace ManagedMemory
{
    public class MemoryGraphNode
    {
        protected ArrayList children;
        protected ArrayList parents;
        protected Address address;

        //Returns the address of the node in the current allocation
        public Address GetAddress()
        {
            return address;
        }

        //Returns the formal notation of all paths leading to this node
        public string[] GetPathsHere()
        {
            throw new NotImplementedException();
        }

        //Returns the formal notation for the shortest path leading to this node
        public string GetShortestPathHere()
        {
            throw new NotImplementedException();
        }

        //Returns the formal notation for the most used path that leads to this node
        public string GetMostCommonPathHere()
        {
            throw new NotImplementedException();
        }

        /*Returns the length of the shortest known path leading to this node
         *Length is the sum of the amount of path offsets and the amount of pointer dereferences needed to arrive at this node
        */
        public int GetDepth()
        {
            throw new NotImplementedException();
        }

        //Returns the number of nodes that directly lead to this node
        public int GetParentCount()
        {
            throw new NotImplementedException();
        }

        //Add the specified node to the parents of this node
        public void AddParent(MemoryGraphNode parent)
        {
            parents.Add(parent);
        }

        //Returns all parent nodes
        public ArrayList GetParents()
        {
            return parents;
        }

        //Returns the number of nodes that this node directly leads to
        public int GetChildCount()
        {
            throw new NotImplementedException();
        }

        //Adds the specified node to the children of this node
        public void AddChild(MemoryGraphNode child)
        {
            children.Add(child);
        }

        //Returns all child nodes
        public ArrayList GetChildren()
        {
            return children;
        }

        //Returns the number of nodes that only this node directly leads to
        public int GetExclusiveDependencyCount()
        {
            throw new NotImplementedException();
        }

    }
}

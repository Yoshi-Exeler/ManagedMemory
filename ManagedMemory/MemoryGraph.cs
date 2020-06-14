using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class MemoryGraph
    {
        protected GraphRootNode root;
        protected PointerPath[] paths;
        protected bool isConstructed = false;


        public MemoryGraph(PointerPath[] paths)
        {
            this.paths = paths;
        }

        //constructs the graph with the current paths or reconstructs the graph if it already exists
        public void Construct()
        {
            throw new NotImplementedException();
        }

        //If the Graph is constructed, the graph will be used to analyse the specified path
        public PathAnalysisResult AnalysePath(PointerPath path)
        {
            throw new NotImplementedException();
        }

        //Returns the root object of the graph
        public GraphRootNode GetGraph()
        {
            throw new NotImplementedException();
        }

        //Returns the shortest path to the specified address or null if no path was found
        public PointerPath GetShortestPathTo(Address adr)
        {
            throw new NotImplementedException();
        }

        /*Searches the procces for possible pointers to Nodes that are already part of the tree
         * The more nodes are in your graph the Better this will work. Can be called multiple times
         * in a row to search again, utilizing the new found nodes. 
         * WARNING: This function is memory and cpu intensive and makes a lot of API calls.
         * Returns the amount of new nodes that were discovered
         */ 
        public int searchNewPaths()
        {
            throw new NotImplementedException();
        }


        /*This function traverses all paths in the tree, and verifies that they still work.
         *All paths that do not work anymore will be removed from the Graph.
         *Returns the amount of removed nodes.
         */ 
        public int verifyPaths()
        {
            throw new NotImplementedException();
        }

        /*This function will cause the Graph to purge all data related to a specific allocation
         *and rescan the data from relative offsets.
         */
        public void dynamicReset()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class Pointer
    {
        protected Address source;
        protected Address destination;
        protected ProcessInterface callback;

        public Pointer(Address src, ProcessInterface callback)
        {
            this.callback = callback;
            source = src;
            destination = new Address(callback.ReadInt64(source));
        }

        public Address GetSource()
        {
            return source;
        }

        public Address GetDestination()
        {
            return destination;
        }

        public void Update()
        {
            destination = new Address(callback.ReadInt64(source));
        }

        public ExternalVariable GetDestinationVariable()
        {
            return new ExternalVariable(destination, callback);
        }

        public override string ToString()
        {
            return source + " -> " + destination;
        }
    }
}

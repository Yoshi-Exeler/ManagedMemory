using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class Pointer
    {
        private Address source;
        private Address destination;
        private ProcessInterface callback;

        public Pointer(Address src, ProcessInterface callback)
        {
            this.callback = callback;
            source = src;
            destination = new Address(callback.ReadInt64(source));
        }

        public Address getSource()
        {
            return source;
        }

        public Address getDestination()
        {
            return destination;
        }

        public void update()
        {
            destination = new Address(callback.ReadInt64(source));
        }

        public ExternalVariable getDestinationVariable()
        {
            return new ExternalVariable(destination, callback);
        }

        public override string ToString()
        {
            return source + " -> " + destination;
        }
    }
}

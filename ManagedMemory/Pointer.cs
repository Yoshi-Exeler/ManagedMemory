﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    class Pointer
    {
        private Address64 source;
        private Address64 destination;
        private ProcessInterface callback;

        public Pointer(Address64 src, ProcessInterface callback)
        {
            source = src;
            destination = new Address64(callback.ReadInt64(source));
        }

        public Address64 getSource()
        {
            return source;
        }

        public Address64 getDestination()
        {
            return destination;
        }

        public void update()
        {
            destination = new Address64(callback.ReadInt64(source));
        }

        public ExternalVariable getDestinationVariable()
        {
            throw new NotImplementedException();
        }
    }
}

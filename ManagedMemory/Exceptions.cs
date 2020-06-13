using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedMemory
{
    public class ReadProcessMemoryException : Exception { public ReadProcessMemoryException(string ex) : base(ex) { } }

    public class WriteProcessMemoryException : Exception { public WriteProcessMemoryException(string ex) : base(ex) { } }

    public class VirtualAllocationException : Exception { public VirtualAllocationException(string ex) : base(ex) { } }

    public class VirtualProtectionException : Exception { public VirtualProtectionException(string ex) : base(ex) { } }

    public class CloseHandleException : Exception { public CloseHandleException(string ex) : base(ex) { } }

    public class CreateRemoteThreadException : Exception { public CreateRemoteThreadException(string ex) : base(ex) { } }

    public class OpenProcessException : Exception { public OpenProcessException(string ex) : base(ex) { } }

    public class OpenThreadException : Exception { public OpenThreadException(string ex) : base(ex) { } }
}

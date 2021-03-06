# ManagedMemory
This is a C# library that provides access to low level memory operations in a managed and simple environment.

# Disclaimer
To use this library you need to load it into a 64Bit process. <br>
This library is made to interact only with 64Bit processes. <br>

# Features
° Read and write process memory <br>
° Change memory protection <br>
° Allocate and free memory regions <br>
° Patternscanning with masking <br>
° Native DLL injection <br>
° APIProxy that makes Windows API calls availabe while also forwarding their errors properly <br>
° Neat classes to model Addresses, Handles, Pointers, PointerPaths and External Variables <br>
° Create PointerPaths from Formal Notation and convert existing ones to formal notation for easy saving and constructing <br>
Example: [[[somemodule.extension + 0xbaseOffset] + 0xLayerOneOffset] + 0xLayerTwoOffset] + 0x FinalOffset <br>

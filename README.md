# ManagedMemory
This is a C# library that provides access to low level memory operations in a managed and simple environment.

# Disclaimer
To use this library you need to load it into a 64Bit process. <br>
This library is made to interact only with 64Bit processes. <br>

# Features
° Read and write process memory <br>
° Change memory protection <br>
° Patternscanning with Masking <br>
° Native Dll Injection <br>
° Neat classes to model Addresses, Pointers, PointerPaths and External Variables <br>
° Create PointerPaths from Formal Notation and convert existing ones to formal notation for easy saving and constructing <br>
Example: [[[somemodule.extension + 0xbaseOffset] + 0xLayerOneOffset] + 0xLayerTwoOffset] + 0x FinalOffset <br>

# ManagedMemory
This is a C# Library that provides access to low level memory operations in a managed environment.
It is important to understand that when dealing with memory outside of your own process, operations are generally unsafe
as you can never be sure when something will be modified by the process you are reading from.
This Library only works when it is both loaded inside a 64 Bit Process and the Memory you are interacting with
belongs to a 64 Bit process. I do not plan on making this library 32 Bit compatible, however you are welcome to do so.

# Creating an Interface
To create a ProcessInterface object you must specify a valid process name WITHOUT the file extension. So for example if you wanted to create an interface for 
explorer.exe you would use "explorer" as the argument for the constructor.

# Addresses, External Variables and PointerPaths
Use the Address64 class instead of long or IntPtr when dealing with addresses so that data and addresses are easily distinguishable.
External variables are used to manage variables outside of your process. If you want to read a data type that does not exist as a preset
read a bytearray and marshal into your type. PointerPath Objects are used to model a chain of pointers leading to a final address in memory.

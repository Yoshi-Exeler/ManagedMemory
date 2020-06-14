using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;

namespace ManagedMemory
{
    public class ProcessInterface
    {
        protected Process managedProcess;
        protected Handle handle;

        public ProcessInterface(string name)
        {
            managedProcess = FindProcess(name);
            handle = Handle.GetProcessHandle(name, APIProxy.ProcessAccessFlags.All);
        }

        public static Process FindProcess(string name)
        {
            Process[] procs = Process.GetProcessesByName(name);
            if (procs.Length != 1) throw new Exception("process does not exist or is not unique");
            return procs[0];
        }

        /*Searches the module for the specified pattern, then returns the address of the first byte of the pattern offset by the FinalOffset or null if the pattern was not found.
         * In most cases you should simply pass the name of the main modulue for the "module" argument.
         */
        public Address FindPattern(string module, byte[] pattern, string mask, int finalOffset)
        {
            ProcessModule mod = null;
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                if (pm.ModuleName == module) mod = pm;
            }
            if (mod == null) throw new ArgumentException("The Specified module " + module + " was not found");
            byte[] dump = new byte[mod.ModuleMemorySize];
            API_ReadProcessMemory(new Address(mod.BaseAddress), dump.Length);
            bool found = false;
            long resIndex = -1;
            for (int i = 0; i < dump.Length; i++)
            {
                for (int x = 0; x < pattern.Length; x++)
                {
                    if (i + x >= dump.Length || !(mask[x] == '?' || dump[i + x] == pattern[x])) break;
                    if (x == pattern.Length)
                    {
                        found = true;
                        resIndex = i;
                    }

                }
                if (found) break;
            }
            if (!found) return null;
            return new Address((long)mod.BaseAddress + resIndex + finalOffset);
        }

        public Handle GetHandle()
        {
            return handle;
        }

        public void InjectDll(string libraryPath)
        {
            MemoryRegion pathRegion = APIProxy.VirtualAllocEx(handle, Address.Zero(), libraryPath.Length, APIProxy.AllocationType.Reserve | APIProxy.AllocationType.Commit, APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE);
            APIProxy.WriteProcessMemory(handle, pathRegion.start, Encoding.ASCII.GetBytes(libraryPath));
            Handle kernelLibraryHandle = Handle.GetModuleHandle("Kernel32.dll");
            Address loadLibraryAddress = APIProxy.GetProcedureAddress(kernelLibraryHandle, "LoadLibraryA");
            Handle threadHandle = APIProxy.CreateRemoteThread(handle, Address.Zero(), 0, loadLibraryAddress, pathRegion.start, 0, 0);
            FreeMemoryRegion(pathRegion, APIProxy.FreeType.Release);
            threadHandle.Close();
        }

        public MemoryRegion AllocateMemory(int size, APIProxy.AllocationType allocationType = APIProxy.AllocationType.Reserve | APIProxy.AllocationType.Commit, APIProxy.MemoryProtection memoryProtection = APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE)
        {
            MemoryRegion allocation = APIProxy.VirtualAllocEx(handle, Address.Zero(), size, allocationType, memoryProtection);
            return allocation;
        }

        public MemoryRegion AllocateMemoryAt(Address adr, int size, APIProxy.AllocationType allocationType = APIProxy.AllocationType.Reserve | APIProxy.AllocationType.Commit, APIProxy.MemoryProtection memoryProtection = APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE)
        {
            MemoryRegion allocation = APIProxy.VirtualAllocEx(handle, adr, size, allocationType, memoryProtection);
            return allocation;
        }

        public void FreeMemoryRegion(MemoryRegion region, APIProxy.FreeType freeType)
        {
            if (freeType == APIProxy.FreeType.Release)
            {
                APIProxy.VirtualFreeEx(handle, region.start, 0, freeType);
            }
            else
            {
                APIProxy.VirtualFreeEx(handle, region.start, region.lenght, freeType);
            }

        }

        protected byte[] API_ReadProcessMemory(Address adr, int size)
        {
            uint oldProtection = APIProxy.VirtualProtectEx(handle, adr, size, (uint)APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE);
            byte[] buffer = APIProxy.ReadProcessMemory(handle, adr, size);
            APIProxy.VirtualProtectEx(handle, adr, size, oldProtection);
            return buffer;
        }

        protected void API_WriteProcessMemory(Address adr, byte[] val)
        {
            uint oldProtection = APIProxy.VirtualProtectEx(handle, adr, val.Length, (uint)APIProxy.MemoryProtection.PAGE_EXECUTE_READ_WRITE);
            APIProxy.WriteProcessMemory(handle, adr, val);
            APIProxy.VirtualProtectEx(handle, adr, val.Length, oldProtection);
        }

        public Address GetModuleBase(string name)
        {
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                if (pm.ModuleName == name) return new Address(pm.BaseAddress);
            }
            return null;
        }

        public ArrayList GetLoadedModuleNames()
        {
            ArrayList res = new ArrayList();
            foreach (ProcessModule pm in managedProcess.Modules)
            {
                res.Add(pm.ModuleName);
            }
            return res;
        }

        public ProcessModuleCollection GetLoadedModules()
        {
            return managedProcess.Modules;
        }

        public int GetPid()
        {
            return managedProcess.Id;
        }

        public void WriteInt32(Address adr, int val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteInt64(Address adr, long val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteFloat(Address adr, float val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteDouble(Address adr, double val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val));
        }

        public void WriteVec2D(Address adr, Vec2D val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val.x));
            API_WriteProcessMemory(adr.OffsetBy(4), BitConverter.GetBytes(val.y));
        }

        public void WriteVec3D(Address adr,Vec3D val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val.x));
            API_WriteProcessMemory(adr.OffsetBy(4), BitConverter.GetBytes(val.y));
            API_WriteProcessMemory(adr.OffsetBy(8), BitConverter.GetBytes(val.z));
        }

        public void WriteVec4D(Address adr,Vec4D val)
        {
            API_WriteProcessMemory(adr, BitConverter.GetBytes(val.x));
            API_WriteProcessMemory(adr.OffsetBy(4), BitConverter.GetBytes(val.y));
            API_WriteProcessMemory(adr.OffsetBy(8), BitConverter.GetBytes(val.z));
            API_WriteProcessMemory(adr.OffsetBy(12), BitConverter.GetBytes(val.w));
        }

        public void WriteByte(Address adr, byte val)
        {
            byte[] buf = { val };
            API_WriteProcessMemory(adr, buf);
        }

        public void WriteByteArray(Address adr, byte[] val)
        {
            API_WriteProcessMemory(adr, val);
        }

        public int ReadInt32(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, sizeof(int));
            return BitConverter.ToInt32(buffer, 0);
        }

        public long ReadInt64(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, sizeof(long));
            return BitConverter.ToInt64(buffer, 0);
        }

        public float ReadFloat(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, sizeof(float));
            return BitConverter.ToSingle(buffer, 0);
        }

        public double ReadDouble(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, sizeof(double));
            return BitConverter.ToDouble(buffer, 0);
        }

        public Vec2D ReadVec2D(Address adr)
        {
            byte[] bufferX = API_ReadProcessMemory(adr, sizeof(float));
            byte[] bufferY = API_ReadProcessMemory(adr.OffsetBy(4), sizeof(float));
            Vec2D res = new Vec2D();
            res.x = BitConverter.ToSingle(bufferX, 0);
            res.y = BitConverter.ToSingle(bufferY, 0);
            return res;
        }

        public Vec3D ReadVec3D(Address adr)
        {
            byte[] bufferX = API_ReadProcessMemory(adr, sizeof(float));
            byte[] bufferY = API_ReadProcessMemory(adr.OffsetBy(4), sizeof(float));
            byte[] bufferZ = API_ReadProcessMemory(adr.OffsetBy(8), sizeof(float));
            Vec3D res = new Vec3D();
            res.x = BitConverter.ToSingle(bufferX, 0);
            res.y = BitConverter.ToSingle(bufferY, 0);
            res.z = BitConverter.ToSingle(bufferZ, 0);
            return res;
        }

        public Vec4D ReadVec4D(Address adr)
        {
            byte[] bufferX = API_ReadProcessMemory(adr, sizeof(float));
            byte[] bufferY = API_ReadProcessMemory(adr.OffsetBy(4), sizeof(float));
            byte[] bufferZ = API_ReadProcessMemory(adr.OffsetBy(8), sizeof(float));
            byte[] bufferW = API_ReadProcessMemory(adr.OffsetBy(12), sizeof(float));
            Vec4D res = new Vec4D();
            res.x = BitConverter.ToSingle(bufferX, 0);
            res.y = BitConverter.ToSingle(bufferY, 0);
            res.z = BitConverter.ToSingle(bufferZ, 0);
            res.w = BitConverter.ToSingle(bufferW, 0);
            return res;
        }

        public byte ReadByte(Address adr)
        {
            byte[] buffer = API_ReadProcessMemory(adr, 1);
            return buffer[0];
        }

        public byte[] ReadByteArray(Address adr, int size)
        {
            byte[] buffer = API_ReadProcessMemory(adr, size);
            return buffer;
        }

        public void FreezeProcess()
        {
            foreach (ProcessThread pt in managedProcess.Threads)
            {
                Handle cHandle = Handle.Zero();
                cHandle = Handle.GetThreadHandle((uint)pt.Id, APIProxy.ThreadAccessFlags.THREAD_SUSPEND_RESUME);
                APIProxy.SuspendThread(cHandle);
            }
        }

        public void UnfreezeProcess()
        {
            foreach (ProcessThread pt in managedProcess.Threads)
            {
                Handle cHandle = Handle.Zero();
                cHandle = Handle.GetThreadHandle((uint)pt.Id, APIProxy.ThreadAccessFlags.THREAD_SUSPEND_RESUME);
                APIProxy.ResumeThread(cHandle);
            }
        }

        public void Terminate()
        {
            managedProcess.Kill();
        }
    }
}

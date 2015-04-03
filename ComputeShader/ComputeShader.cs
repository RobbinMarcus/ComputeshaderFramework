using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace ComputeShaderAddon
{
    /// <summary>
    /// Compute Shader Helper
    /// Currently only supports Unordered Acces View binds
    /// </summary>
    public class ComputeShaderHelper
    {
        Device Device { get; set; }
        ComputeShader Shader { get; set; }

        ViewBuffer[] Data;
        int[] DataCount;

        int currentDataset = 0;

        /// <summary>
        /// Compute Shader 
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="filename">Path to uncompiled shaderfile.fx</param>
        /// <param name="main">Main function in shader</param>
        /// <param name="shaderversion">Compute shader version</param>
        /// <param name="compile">Compile the shader, if false, uses previous compiled file</param>
        public ComputeShaderHelper(Device device, string filename, string shaderversion = "cs_5_0", string main = "CSMain", bool compile = true)
        {
            Device = device;

            // The maximum amount of unordered acces views that can be bound to the shader
            // More info here: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476331%28v=vs.85%29.aspx
            if (Device.FeatureLevel == SharpDX.Direct3D.FeatureLevel.Level_11_0)
            {
                Data = new ViewBuffer[8];
                DataCount = new int[8];
            }
            else
            {
                Data = new ViewBuffer[1];
                DataCount = new int[1];
            }

            string existing = filename.Substring(0, filename.Length - 3) + ".hlsl";
            if (File.Exists(existing) && !compile)
            {
                ShaderBytecode code = ShaderBytecode.FromFile(existing);
                Shader = new ComputeShader(device, code);
            }
            else
            {
                ShaderBytecode code = ShaderBytecode.CompileFromFile(filename, main, shaderversion, ShaderFlags.None, EffectFlags.None);
                Shader = new ComputeShader(device, code);

                code.Save(existing);
            }
        }


        /// <summary>
        /// Initialize the buffer with correct size
        /// </summary>
        void InitializeBuffer(ref ViewBuffer buffer, int length, int elemSize)
        {
            buffer.Buffer = new Buffer(Device, elemSize * length, ResourceUsage.Default,
                BindFlags.ShaderResource | BindFlags.UnorderedAccess, CpuAccessFlags.None,
                ResourceOptionFlags.BufferStructured, elemSize);

            buffer.View = new UnorderedAccessView(Device, buffer.Buffer);
        }


        /// <summary>
        /// Execute the compute shader
        /// </summary>
        public void Execute(int nThreads)
        {
            Device.ImmediateContext.ComputeShader.Set(Shader);
            for (int i = 0; i < currentDataset; i++)
            {
                Device.ImmediateContext.ComputeShader.SetUnorderedAccessView(i, Data[i].View);
            }
            Device.ImmediateContext.Dispatch(nThreads, 1, 1);

            // Reset so the next data can be bound
            currentDataset = 0;
        }


        /// <summary>
        /// Fills the buffer with data
        /// <returns>Index of the array to retrieve the data</returns>
        /// </summary>
        public int SetData<T>(T[] data) where T : struct
        {
            if (currentDataset >= Data.Length)
                throw new Exception("You can't bind more buffers to the shader!");

            DataCount[currentDataset] = data.Length;

            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            int totalSize = size * data.Length;

            InitializeBuffer(ref Data[currentDataset], data.Length, size);

            var stagingBuffer = new Buffer(Device, totalSize, ResourceUsage.Staging, BindFlags.None, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            DataStream stream;
            Device.ImmediateContext.MapSubresource(stagingBuffer, 0, MapMode.Write, MapFlags.None, out stream);
            {
                stream.WriteRange(data, 0, data.Length);
            }
            Device.ImmediateContext.UnmapSubresource(stagingBuffer, 0);
            Device.ImmediateContext.CopySubresourceRegion(stagingBuffer, 0, new ResourceRegion(0, 0, 0, totalSize, 1, 1), Data[currentDataset].Buffer, 0);
            stagingBuffer.Dispose();

            return currentDataset++;
        }


        /// <summary>
        /// Returns data from the buffer
        /// </summary>
        public T[] GetData<T>(int index) where T : struct
        {
            ViewBuffer buffer = Data[index];
            int count = DataCount[index];

            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            int totalSize = size * count;
            var data = new T[count];
            DataStream stream;

            var stagingBuffer = new Buffer(Device, totalSize, ResourceUsage.Staging, BindFlags.None, CpuAccessFlags.Read, ResourceOptionFlags.None, 0);
            Device.ImmediateContext.CopySubresourceRegion(buffer.Buffer, 0, new ResourceRegion(0, 0, 0, totalSize, 1, 1), stagingBuffer, 0);
            Device.ImmediateContext.MapSubresource(stagingBuffer, MapMode.Read, MapFlags.None, out stream);
            {
                stream.Position = 0;
                stream.ReadRange(data, 0, count);
            }
            stagingBuffer.Dispose();

            return data;
        }
    }
}
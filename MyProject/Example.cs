using SharpDX;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputeShaderAddon;
using SharpDX.Toolkit.Graphics;

namespace MyProject
{
    struct ExampleStruct
    {
        public Vector3 Data;

        public ExampleStruct(Vector3 data)
        {
            Data = data;
        }
    }


    class Example
    {
        int amount = 10000;
        Stopwatch timer;
        ComputeShaderHelper CSHelper;

        public Example(GraphicsDevice device)
        {
            // Initialize the helper
            CSHelper = new ComputeShaderHelper(device, "Effects/ComputeShaderExample.fx");
            timer = new Stopwatch();

            // Set some test data
            ExampleStruct[] data = new ExampleStruct[amount];
            for (int i = 0; i < amount; i++)
            {
                data[i] = new ExampleStruct(new Vector3(1, 1, 1));
            }

            timer.Start();
            data = CalculateGPU(data);
            timer.Stop();

            Console.WriteLine("Last data: " + data[amount - 1].Data);
            Console.WriteLine("It took the GPU: {0} milliseconds", timer.ElapsedMilliseconds);

            timer.Reset();
            timer.Start();
            CalculateCPU(data);
            timer.Stop();

            Console.WriteLine("Last data: " + data[amount - 1].Data);
            Console.WriteLine("It took the CPU: {0} milliseconds", timer.ElapsedMilliseconds);
        }


        /// <summary>
        /// Calculate using the GPU
        /// </summary>
        ExampleStruct[] CalculateGPU(ExampleStruct[] data)
        {
            int index = CSHelper.SetData<ExampleStruct>(data);
            CSHelper.Execute(50);
            return CSHelper.GetData<ExampleStruct>(index);
        }


        /// <summary>
        /// Calculate using the CPU
        /// </summary>
        ExampleStruct[] CalculateCPU(ExampleStruct[] data)
        {
            for (int i = 0; i < amount; i++)
            {
                int result = 0;
                for (int j = 0; j < amount; j++)
                    result += j;
                data[i].Data = new Vector3(result, result, result);
            }
            return data;
        }
    }
}

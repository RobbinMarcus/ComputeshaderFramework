using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderAddon
{
    public struct ViewBuffer
    {
        public SharpDX.Direct3D11.Buffer Buffer;
        public SharpDX.Direct3D11.UnorderedAccessView View;

        public ViewBuffer(SharpDX.Direct3D11.Buffer buffer, SharpDX.Direct3D11.UnorderedAccessView view)
        {
            Buffer = buffer;
            View = view;
        }
    }
}

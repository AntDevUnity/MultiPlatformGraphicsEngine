using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Maths;

using System.Runtime.InteropServices;
namespace MpGe.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vertex
    {

        public Vector3 Position;
        public Vector2 UV;
 


    };
}

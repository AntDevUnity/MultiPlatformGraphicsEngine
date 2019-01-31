using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MpGe.Draw2D;
using MpGe.Maths;
using MpGe.Data;

namespace MpGEPlatformDX12.Draw2D
{
    public class Draw2D : Draw2DBase
    {
        public Effect.Effect FXSimple2D;
        public override void Init()
        {

            FXSimple2D = Effect.CreateEffect.CreateSimple2D();

            var verts = new[]
       {
                    new Vertex() {Position=new Vector3(0.0f, 0.25f  , 0.0f ),Color=new Vector4(1.0f, 0.0f, 0.0f, 1.0f ) },
                    new Vertex() {Position=new Vector3(0.25f, -0.25f , 0.0f),Color=new Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
                    new Vertex() {Position=new Vector3(-0.25f, -0.25f , 0.0f),Color=new Vector4(0.0f, 0.0f, 1.0f, 1.0f ) },
                    
            };
            vb = new Buffer.VertexBufferDX12(verts, 0, 0);
            FXSimple2D.commandList.Close();

        }
        private Buffer.VertexBufferDX12 vb;

        public override void Rect(float x,float y,float w,float h)
        {



            vb.commandList = FXSimple2D.commandList;

            FXSimple2D.BeginRen();

            DXGlobal.Display.DrawBuffer(vb);

            FXSimple2D.EndRen();

            Console.WriteLine("Drawing!");


        }

    }
}

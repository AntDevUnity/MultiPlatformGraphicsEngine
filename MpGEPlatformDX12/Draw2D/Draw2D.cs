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

            verts = new[]
       {
                    new Vertex() {Position=new Vector3(200,100f  , 0.0f ),Color=new Vector4(1.0f, 0.0f, 0.0f, 1.0f ) },
                    new Vertex() {Position=new Vector3(440f,100f , 0.0f),Color=new Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
                    new Vertex() {Position=new Vector3(440f,300 , 0.0f),Color=new Vector4(0.0f, 0.0f, 1.0f, 1.0f ) },
                    new Vertex() {Position= new Vector3(400,500,0),Color = new Vector4(1,1,1,1)}
            };

            var verts2 = new[]
      {
                    new Vertex() {Position=new Vector3(0.7f, 0.15f  , 0.1f ),Color=new Vector4(1.0f, 0.0f, 0.0f, 1.0f ) },
                    new Vertex() {Position=new Vector3(200f, 2f , 0.1f),Color=new Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
                    new Vertex() {Position=new Vector3(200f, 200f , 0.1f),Color=new Vector4(0.0f, 0.0f, 1.0f, 1.0f ) },
                    new Vertex() { Position = new Vector3(20,200,0.1f),Color = new Vector4(1,1,1,1)}
            };

            short[] ind = {
                0,1,2,2,3,0
            };

            indices = ind;

            vb = new Buffer.VertexBufferDX12(verts,indices, 0, 0);



          //  vb2 = new Buffer.VertexBufferDX12(verts2, 0, 0);

            FXSimple2D.commandList.Close();

        }
        private Buffer.VertexBufferDX12 vb2;
        private Buffer.VertexBufferDX12 vb;
        private Vertex[] verts;
        private short[] indices;

        public override void Rect(float x, float y, float w, float h)
        {



            vb.commandList = FXSimple2D.commandList;
            //vb2.commandList = FXSimple2D.commandList;

            verts = new[]
      {
                    new Vertex() {Position=new Vector3(x,y  , 0.0f ),Color=new Vector4(1.0f, 0.0f, 0.0f, 1.0f ) },
                    new Vertex() {Position=new Vector3(x+w,y , 0.0f),Color=new Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
                    new Vertex() {Position=new Vector3(x+w,y+h , 0.0f),Color=new Vector4(0.0f, 0.0f, 1.0f, 1.0f ) },
                    new Vertex() {Position=new Vector3(x,y+h,0.0f),Color = new Vector4(1,1,1,1)}

            };



            vb.Update(verts, indices);

            FXSimple2D.BeginRen();

            Effect.Simple2DConst s2 = new Effect.Simple2DConst
            {
                Proj = SharpDX.Matrix.OrthoOffCenterLH(0, 800, 600,0,0, 1)
            };

            FXSimple2D.cbuf.CopyData(0, ref s2);

            DXGlobal.Display.DrawBuffer(vb,FXSimple2D);
      //      DXGlobal.Display.DrawBuffer(vb2,FXSimple2D);

            FXSimple2D.EndRen();

            Console.WriteLine("Drawing!");


        }

    }
}

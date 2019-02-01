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

    using SharpDX.Direct3D12;
    using SharpDX.Windows;
    public class Draw2D : Draw2DBase
    {
        public Effect.Effect FXSimple2D;
        public Effect.Effect FXTextured2D;
        public override void Init()
        {
            //return;

            FXTextured2D = Effect.CreateEffect.CreateTextured2D();
        //    FXSimple2D = Effect.CreateEffect.CreateSimple2D();

            float x = 20, y = 20, w = 100, h = 100;
            verts = new[]
{
                    new Vertex() {Position=new Vector3(x,y  , 0.0f ),UV=new Vector2(0.0f, 0.0f) },
                    new Vertex() {Position=new Vector3(x+w,y , 0.0f),UV=new Vector2(1.0f,0) },
                    new Vertex() {Position=new Vector3(x+w,y+h , 0.0f),UV=new Vector2(1.0f, 1.0f ) },
                    new Vertex() {Position=new Vector3(x,y+h,0.0f),UV = new Vector2(0,1)}

            };



            short[] ind = {
                0,1,2,2,3,0
            };

            indices = ind;

            vb = new Buffer.VertexBufferDX12(verts,indices, 0, 0);



          //  vb2 = new Buffer.VertexBufferDX12(verts2, 0, 0);

          //  FXSimple2D.commandList.Close();

        }
        private Buffer.VertexBufferDX12 vb2;
        private Buffer.VertexBufferDX12 vb;
        private Vertex[] verts;
        private short[] indices;
        bool first = true;
        public override void RectTex(float x,float y,float w,float h,MpGe.Texture.Texture2DBase tex)
        {

            var t2 = tex as Texture.Texture2D;

       //     vb.commandList = FXSimple2D.commandList;
            //vb2.commandList = FXSimple2D.commandList;

            verts = new[]
      {
                    new Vertex() {Position=new Vector3(x,y  , 0.0f ),UV=new Vector2(0.0f, 0.0f) },
                    new Vertex() {Position=new Vector3(x+w,y , 0.0f),UV=new Vector2(1.0f,0) },
                    new Vertex() {Position=new Vector3(x+w,y+h , 0.0f),UV=new Vector2(1.0f, 1.0f ) },
                    new Vertex() {Position=new Vector3(x,y+h,0.0f),UV = new Vector2(0,1)}

            };




            vb = new Buffer.VertexBufferDX12(verts, indices, 0, 0);



          
        

            //t2.SetHeap();


                FXTextured2D._descriptorHeaps = new[] { FXTextured2D._cbvHeap };
      
            //            FXSimple2D.BeginRen();

            Effect.Simple2DConst s2 = new Effect.Simple2DConst
            {
                Proj = SharpDX.Matrix.OrthoOffCenterLH(0, 800, 600, 0, 0, 1)
            };

            // FXSimple2D.cbuf.CopyData(0, ref s2);
            FXTextured2D.cbuf.CopyData(0, ref s2);
            DXGlobal.Display.DrawBuffer(vb,FXTextured2D);
            //      DXGlobal.Display.DrawBuffer(vb2,FXSimple2D);

  //          FXSimple2D.EndRen();
        }

        public override void Rect(float x, float y, float w, float h)
        {



            vb.commandList = FXSimple2D.commandList;
            //vb2.commandList = FXSimple2D.commandList;

            verts = new[]
  {
                    new Vertex() {Position=new Vector3(x,y  , 0.0f ),UV=new Vector2(0.0f, 0.0f) },
                    new Vertex() {Position=new Vector3(x+w,y , 0.0f),UV=new Vector2(1.0f,0) },
                    new Vertex() {Position=new Vector3(x+w,y+h , 0.0f),UV=new Vector2(1.0f, 1.0f ) },
                    new Vertex() {Position=new Vector3(x,y+h,0.0f),UV = new Vector2(0,1)}

            };




            vb = new Buffer.VertexBufferDX12(verts, indices, 0, 0);


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

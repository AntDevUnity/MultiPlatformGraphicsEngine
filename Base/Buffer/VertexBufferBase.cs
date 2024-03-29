﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpGe.Buffer
{
    public class VertexBufferBase
    {

        public dynamic Resource
        {
            get;
            set;
        }
        public int Begin
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public int Stride
        {
            get;
            set;
        }

        public int Size
        {
            get;
            set;
        }

        public VertexBufferBase(Data.Vertex[] verts,short[] indices, int size=0,int stride = 0)
        {
            InitBuffer(verts,indices,size, stride);
        }

        public virtual void InitBuffer(Data.Vertex[] verts,short[] indices,int size,int stride)
        {
           
        }

        public virtual void Update(Data.Vertex[] verts,short[] indices)
        {

        }

    }
}

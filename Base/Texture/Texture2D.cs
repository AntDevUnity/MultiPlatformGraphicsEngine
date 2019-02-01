using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpGe.Texture
{
    public class Texture2DBase
    {

        public virtual void Load(string path)
        {

        }

        public virtual void Bind(int unit)
        {

        }

        public virtual void Release(int unit)
        {

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    public class Edge
    {
        public Vector3 start;
        public Vector3 end;

        public Edge()
        {
        }

        public Edge(Vector3 s, Vector3 e)
        {
            start = s;
            end = e;
        }

        public void Move(Vector3 delta)
        {
            start += delta;
            end += delta;
        }
    }
}

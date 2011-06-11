using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    public class Block
    {
        public List<Edge> edges;

        public Block()
        {
            edges = new List<Edge>();
        }

        public void Move(Vector3 delta)
        {
            foreach (Edge e in edges)
            {
                e.Move(delta);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    public class Face
    {
        public Vector3 normal;        
        public Vector3 center;
        public List<Block> blocks;

        public Face()
        {
        }

        public Face(Vector3 normal, Vector3 center)
        {
            this.normal = normal;
            this.center = center;

            blocks = new List<Block>();
            Block b = new Block();

            Vector3 xDir = Vector3.UnitX;
            if(Vector3.Cross(xDir, normal).Equals(Vector3.Zero))
               xDir = Vector3.UnitZ;
            Vector3 yDir = Vector3.Cross(normal, xDir);
            yDir.Normalize();
            xDir.Normalize();
                        
            b.edges.Add(new Edge(center,center+xDir*2));
            b.edges.Add(new Edge(center+xDir*2,center+xDir*2+yDir*2));
            b.edges.Add(new Edge(center+xDir*2+yDir*2,center+yDir*2));
            b.edges.Add(new Edge(center + yDir * 2, center));
            blocks.Add(b);
        }

        public void Move(Vector3  delta)
        {
            center+=delta;
            foreach (Block b in blocks)
            {
                b.Move(delta);
            }
        }
    }
}

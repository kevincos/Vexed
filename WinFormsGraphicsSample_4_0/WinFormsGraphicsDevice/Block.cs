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

        public void Resize(Vector3 mousePos, Edge dragEdge, Vector3 normal)
        {
            Vector3 edgeDir = dragEdge.end - dragEdge.start;
            edgeDir.Normalize();
            Vector3 edgeNormal = Vector3.Cross(edgeDir, normal);
            int delta = (int)Vector3.Dot(edgeNormal, mousePos - dragEdge.start);
            Vector3 oldStart = dragEdge.start;
            Vector3 oldEnd = dragEdge.end;
            foreach (Edge e in edges)
            {
                if (e.start == oldStart || e.start == oldEnd)
                    e.start += delta * edgeNormal;
                if (e.end == oldStart || e.end == oldEnd)
                    e.end += delta * edgeNormal;      
            }
        }
    }
}

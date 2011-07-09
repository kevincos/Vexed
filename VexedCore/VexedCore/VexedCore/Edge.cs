using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VexedCore
{
    public class Edge
    {
        public Vertex start;
        public Vertex end;

        public Edge()
        {
            start = new Vertex();
            end = new Vertex();
        }

        public Edge(VexedLib.Edge xmlEdge, Vector3 normal)
        {
            start = new Vertex(xmlEdge.start, normal, Vector3.Zero, xmlEdge.end - xmlEdge.start);
            end = new Vertex(xmlEdge.end, normal, Vector3.Zero, xmlEdge.start - xmlEdge.end);
            start.direction.Normalize();
            end.direction.Normalize();
            start.normal.Normalize();
            end.normal.Normalize();
        }
    }
}

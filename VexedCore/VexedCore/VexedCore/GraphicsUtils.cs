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
    public struct VertexPositionColorNormal
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        public VertexPositionColorNormal(Vector3 p, Color c, Vector3 n)
        {
            Color = c;
            Position = p;
            Normal = n;            
        }
    }

    public struct VertexPositionColorNormalTexture
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;
        public Vector2 TextureCoordinates;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 6 + 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        public VertexPositionColorNormalTexture(Vector3 p, Color c, Vector3 n, Vector2 t)
        {
            Color = c;
            Position = p;
            Normal = n;
            TextureCoordinates = t;
        }
    }

    public class TrasnparentSquare
    {
        public VertexPositionColorNormalTexture v1;
        public VertexPositionColorNormalTexture v2;
        public VertexPositionColorNormalTexture v3;
        public VertexPositionColorNormalTexture v4;
        public VertexPositionColorNormalTexture v5;
        public VertexPositionColorNormalTexture v6;
        
        public Vector3 averagePos;

        public TrasnparentSquare(VertexPositionColorNormalTexture v1, VertexPositionColorNormalTexture v2, VertexPositionColorNormalTexture v3, VertexPositionColorNormalTexture v4, VertexPositionColorNormalTexture v5, VertexPositionColorNormalTexture v6)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
            this.v5 = v5;
            this.v6 = v6;
            averagePos = (v1.Position + v2.Position + v3.Position + v4.Position + v5.Position + v6.Position) / 6;
        }
    }

    public class FaceSorter : Comparer<TrasnparentSquare>
    {
        Vector3 unit = Vector3.Zero;
        public FaceSorter(Vector3 unit)
        {
            this.unit = unit;
            this.unit.Normalize();
        }
        // Compares by Length, Height, and Width.
        public override int Compare(TrasnparentSquare t1, TrasnparentSquare t2)
        {
            float t1Center = Vector3.Dot(t1.averagePos, unit);
            float t2Center = Vector3.Dot(t2.averagePos, unit);
            return -t1Center.CompareTo(t2Center);            
        }

    }    

}

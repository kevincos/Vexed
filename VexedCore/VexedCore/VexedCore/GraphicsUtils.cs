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
}

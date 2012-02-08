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
    public class Wormhole
    {
        Vector3 position;
        Vector3 up;
        Vector3 right;
        Vector3 normal;

        public int time = 0;
        public int maxSegmentTime = 500;
        public int currentIndex = 0;

        public static Texture2D wormholeTexture;

        public static List<Vector2> wormholeRingCoords;

        static Wormhole()
        {
            wormholeRingCoords = new List<Vector2>();
            for (int i = 0; i < 8; i++)
            {
                float theta = (float)(Math.PI / 8 + i * Math.PI / 4);
                float upComponent = (float)(Math.Cos(theta));
                float rightComponent = (float)(Math.Sin(theta));
                wormholeRingCoords.Add(new Vector2(upComponent, rightComponent));
            }
        }

        public Wormhole(Vector3 position, Vector3 up, Vector3 normal)
        {
            this.position = position;
            this.up = up;
            this.normal = normal;
            this.right = Vector3.Cross(normal, up);
        }

        public void Update(int gameTime)
        {
            time += gameTime;
            while (time > maxSegmentTime)
            {
                time -= maxSegmentTime;
                currentIndex++;
            }
            while (currentIndex >= 8)
                currentIndex -= 8;
        }

        public void Draw()
        {
            int index1 = currentIndex%8;
            int index2 = (index1 + 1)%8;
            Vector2 u = ((maxSegmentTime-time) * wormholeRingCoords[index1] + time * wormholeRingCoords[index2])/maxSegmentTime;
            u.Normalize();
            u *= 7;
            Vector2 r = new Vector2(u.Y, -u.X);

            Vector2 upperLeft = u - r;
            Vector2 upperRight = u + r;
            Vector2 lowerLeft = -u - r;
            Vector2 lowerRight = -u + r;

            Color wormholeColor = Color.White;

            List<VertexPositionColorNormalTexture> vertexList = new List<VertexPositionColorNormalTexture>();
            vertexList.Add(new VertexPositionColorNormalTexture(position + upperLeft.X * up + upperLeft.Y * right, wormholeColor, normal, new Vector2(0, 0)));
            vertexList.Add(new VertexPositionColorNormalTexture(position + lowerLeft.X * up + lowerLeft.Y * right, wormholeColor, normal, new Vector2(0, 1)));
            vertexList.Add(new VertexPositionColorNormalTexture(position + lowerRight.X * up + lowerRight.Y * right, wormholeColor, normal, new Vector2(1, 1)));

            vertexList.Add(new VertexPositionColorNormalTexture(position + upperLeft.X * up + upperLeft.Y * right, wormholeColor, normal, new Vector2(0, 0)));
            vertexList.Add(new VertexPositionColorNormalTexture(position + lowerRight.X * up + lowerRight.Y * right, wormholeColor, normal, new Vector2(1, 1)));
            vertexList.Add(new VertexPositionColorNormalTexture(position + upperRight.X * up + upperRight.Y * right, wormholeColor, normal, new Vector2(1, 0)));

            Engine.playerTextureEffect.Texture = wormholeTexture;
            Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();

            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                vertexList.ToArray(), 0, vertexList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);

        }
    }
}

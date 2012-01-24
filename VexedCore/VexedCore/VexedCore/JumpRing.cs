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
    public enum RingType
    {
        JumpRing,
        JumpTube,
        TunnelRing,
        TunnelTube,
        BridgeRing,
        BridgeBack,
        BridgeWarp
    }

    public class JumpRing
    {
        Vector3 position;
        Vector3 up;
        Vector3 right;
        Vector3 direction;
        Color c;

        public RingType type = RingType.JumpRing;
        public static int ringSegments = 6;
        public float radius = 1.5f;
        public float width = .5f;
        public static List<Vector2> ringHexVectorList;
        public static List<Vector2> ringSquareVectorList;
        public static List<Vector2> ringOctVectorList;


        public static List<Vector2> ringTexCoords;
        public static List<Vector2> tubeTexCoords;
        public static List<Vector2> diskTexCoords;
        public static List<Vector2> warpTexCoords;        

        static JumpRing()
        {

            ringHexVectorList = new List<Vector2>();
            for (int i = 0; i < ringSegments; i++)
            {
                float theta = (float)(i*(Math.PI * 2) / (1f * ringSegments));
                float upComponent = (float)( Math.Cos(theta));
                float rightComponent = (float)( Math.Sin(theta));
                ringHexVectorList.Add(new Vector2(upComponent, rightComponent));
            }

            ringSquareVectorList = new List<Vector2>();
            for (int i = 0; i < 4; i++)
            {                
                float theta = (float)(Math.PI / 4 + i * Math.PI / 2);
                float upComponent = (float)(Math.Cos(theta));
                float rightComponent = (float)(Math.Sin(theta));
                ringSquareVectorList.Add(new Vector2(upComponent, rightComponent));
            }

            ringOctVectorList = new List<Vector2>();
            for (int i = 0; i < 8; i++)
            {
                float theta = (float)(Math.PI / 8 + i * Math.PI / 4);
                float upComponent = (float)(Math.Cos(theta));
                float rightComponent = (float)(Math.Sin(theta));
                ringOctVectorList.Add(new Vector2(upComponent, rightComponent));
            }

            ringTexCoords = new List<Vector2>();
            ringTexCoords.Add(new Vector2(.8f, 0));
            ringTexCoords.Add(new Vector2(.2f, 0));
            ringTexCoords.Add(new Vector2(.2f, 1));
            ringTexCoords.Add(new Vector2(.8f, 1));

            tubeTexCoords = new List<Vector2>();
            tubeTexCoords.Add(new Vector2(1f, .2f));
            tubeTexCoords.Add(new Vector2(0f, .2f));
            tubeTexCoords.Add(new Vector2(0f, .8f));
            tubeTexCoords.Add(new Vector2(1f, .8f));

            diskTexCoords = new List<Vector2>();
            diskTexCoords.Add(new Vector2(1, 0));
            diskTexCoords.Add(new Vector2(.2f, 0));
            diskTexCoords.Add(new Vector2(.2f, 1));
            diskTexCoords.Add(new Vector2(1, 1));

            warpTexCoords = new List<Vector2>();
            warpTexCoords.Add(new Vector2(.8f, .2f));
            warpTexCoords.Add(new Vector2(.2f, .2f));
            warpTexCoords.Add(new Vector2(.2f, 1));
            warpTexCoords.Add(new Vector2(.8f, 1));
        }

        public JumpRing(RingType type, Vector3 pos, Vector3 up, Vector3 normal)
        {
            this.type = type;
            this.right = Vector3.Cross(normal, up);
            this.direction = normal;
            this.up = up;
            this.position = pos;
        }

        public JumpRing(RingType type, Vector3 pos, Vector3 up, Vector3 normal, float width)
        {
            this.type = type;
            this.width = width;
            this.right = Vector3.Cross(normal, up);
            this.direction = normal;
            this.up = up;
            this.position = pos;
        }

        public JumpRing(RingType type, Vector3 pos, Vector3 up, Vector3 normal, float width, float radius)
        {
            this.type = type;
            this.radius = radius;
            this.width = width;
            this.right = Vector3.Cross(normal, up);
            this.direction = normal;
            this.up = up;
            this.position = pos;
        }

        public JumpRing(JumpRing j)
        {
            this.type = j.type;
            this.c = j.c;
            this.right = j.right;
            this.direction = j.direction;
            this.up = j.up;
            this.position = j.position;
            this.width = j.width;
        }


        public List<VertexPositionColorNormalTexture> ringList;

        public VertexPositionColorNormalTexture[] ringArray;

        public void UpdateVertexList(Room r)
        {
            List<Vector2> sideTexCoords = ringTexCoords;
            List<Vector2> frontTexCoords = ringTexCoords;
            if (type == RingType.TunnelTube || type == RingType.JumpTube) 
                sideTexCoords = tubeTexCoords;
            if (type == RingType.BridgeRing)
                sideTexCoords = Room.plateTexCoords;
            if (type == RingType.BridgeBack)
                frontTexCoords = diskTexCoords;
            if (type == RingType.BridgeWarp)
                frontTexCoords = warpTexCoords;
            if (ringList == null)
            {
                c = r.currentColor;
                float outerRadiusMod = 1f;
                if (type == RingType.TunnelRing || type == RingType.BridgeBack || type == RingType.BridgeRing)
                {
                    c = Color.Gray;
                }
                if (type == RingType.BridgeWarp)
                {
                    c = Color.DarkBlue;
                    c.A = 180;
                    
                }

                if(type == RingType.TunnelTube || type == RingType.JumpTube)
                {
                    outerRadiusMod = .9f;
                }

                List<Vector2> ringVectorList = null;
                if (type == RingType.JumpRing || type == RingType.JumpTube)
                {
                    ringVectorList = ringHexVectorList;
                }
                if (type == RingType.TunnelRing || type == RingType.TunnelTube)
                {
                    ringVectorList = ringSquareVectorList;
                }
                if (type == RingType.BridgeRing || type == RingType.BridgeBack || type == RingType.BridgeWarp)
                {
                    ringVectorList = ringOctVectorList;
                }
                

                ringList = new List<VertexPositionColorNormalTexture>();
                for(int i = 0; i < ringVectorList.Count; i++)
                {

                    int nextIndex = (i + 1) % ringVectorList.Count;


                    float outerRadius = outerRadiusMod * radius;
                    float innerRadius = .8f * radius;
                    if (type == RingType.BridgeRing)
                        innerRadius = radius - .6f;
                    if (type == RingType.BridgeBack || type == RingType.BridgeWarp)
                        innerRadius = 0;

                    Vector3 pos = position + outerRadius * (ringVectorList[i].X * up + ringVectorList[i].Y * right);
                    Vector3 nextPos = position + outerRadius * (ringVectorList[nextIndex].X * up + ringVectorList[nextIndex].Y * right);
                    Vector3 normal = Vector3.Cross(nextPos - pos, direction);
                    Vector3 innerPos = position + innerRadius * (ringVectorList[i].X * up + ringVectorList[i].Y * right);
                    Vector3 nextinnerPos = position + innerRadius * (ringVectorList[nextIndex].X * up + ringVectorList[nextIndex].Y * right);

                    Color shadedColor = c;
                    if (type == RingType.JumpTube || type == RingType.TunnelTube)
                    {
                        shadedColor.R = (Byte)(.8f * shadedColor.R);
                        shadedColor.G = (Byte)(.8f * shadedColor.G);
                        shadedColor.B = (Byte)(.8f * shadedColor.B);
                    }


                    if (type != RingType.BridgeWarp)
                    {
                        ringList.Add(new VertexPositionColorNormalTexture(pos, shadedColor, normal, sideTexCoords[0]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextPos + width * direction, shadedColor, normal, sideTexCoords[2]));
                        ringList.Add(new VertexPositionColorNormalTexture(pos + width * direction, shadedColor, normal, sideTexCoords[3]));

                        ringList.Add(new VertexPositionColorNormalTexture(pos, shadedColor, normal, sideTexCoords[0]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextPos, shadedColor, normal, sideTexCoords[1]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextPos + width * direction, shadedColor, normal, sideTexCoords[2]));

                        ringList.Add(new VertexPositionColorNormalTexture(innerPos, shadedColor, normal, sideTexCoords[0]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos + width * direction, shadedColor, normal, sideTexCoords[2]));
                        ringList.Add(new VertexPositionColorNormalTexture(innerPos + width * direction, shadedColor, normal, sideTexCoords[3]));

                        ringList.Add(new VertexPositionColorNormalTexture(innerPos, shadedColor, normal, sideTexCoords[0]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos, shadedColor, normal, sideTexCoords[1]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos + width * direction, shadedColor, normal, sideTexCoords[2]));


                        ringList.Add(new VertexPositionColorNormalTexture(innerPos + width * direction, c, normal, frontTexCoords[0]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextPos + width * direction, c, normal, frontTexCoords[2]));
                        ringList.Add(new VertexPositionColorNormalTexture(pos + width * direction, c, normal, frontTexCoords[3]));

                        ringList.Add(new VertexPositionColorNormalTexture(innerPos + width * direction, c, normal, frontTexCoords[0]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos + width * direction, c, normal, frontTexCoords[1]));
                        ringList.Add(new VertexPositionColorNormalTexture(nextPos + width * direction, c, normal, frontTexCoords[2]));
                    }

                    ringList.Add(new VertexPositionColorNormalTexture(innerPos, c, normal, frontTexCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos, c, normal, frontTexCoords[2]));
                    ringList.Add(new VertexPositionColorNormalTexture(pos, c, normal, frontTexCoords[3]));

                    ringList.Add(new VertexPositionColorNormalTexture(innerPos, c, normal, frontTexCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos, c, normal, frontTexCoords[1]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos, c, normal, frontTexCoords[2]));
                }                
            }
            if (type != RingType.TunnelRing && type != RingType.BridgeWarp && type != RingType.BridgeRing && type != RingType.BridgeBack && r.currentColor != c)
            {
                c = r.currentColor;
                List<VertexPositionColorNormalTexture> newList = new List<VertexPositionColorNormalTexture>();
                for (int i = 0; i < ringList.Count; i++)
                {
                    newList.Add(new VertexPositionColorNormalTexture(ringList[i].Position,c, ringList[i].Normal, ringList[i].TextureCoordinates));
                }
                ringList = newList;
            }
        }

        public void Draw(Room r)
        {            
            if (r.refreshVertices == true || ringList ==null )
            {
                UpdateVertexList(r);
                r.staticRings.AddRange(ringList);
            }
        }
    }
}

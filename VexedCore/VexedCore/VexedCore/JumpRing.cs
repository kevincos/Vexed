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
    public class JumpRing
    {
        Vector3 position;
        Vector3 up;
        Vector3 right;
        Vector3 direction;
        Color c;

        public static int ringSegments = 6;
        public static float radius = 1.5f;
        public float width = .5f;
        public static List<Vector2> ringVectorList;


        public static List<Vector2> ringTexCoords;
        public static List<Vector2> tubeTexCoords;        

        static JumpRing()
        {

            ringVectorList = new List<Vector2>();
            for (int i = 0; i < ringSegments; i++)
            {
                float theta = (float)(i*(Math.PI * 2) / (1f * ringSegments));
                float upComponent = (float)( Math.Cos(theta));
                float rightComponent = (float)( Math.Sin(theta));
                ringVectorList.Add(new Vector2(upComponent, rightComponent));
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
        }

        public JumpRing(Vector3 pos, Vector3 up, Vector3 normal)
        {
            this.right = Vector3.Cross(normal, up);
            this.direction = normal;
            this.up = up;
            this.position = pos;
        }

        public JumpRing(Vector3 pos, Vector3 up, Vector3 normal, float width)
        {
            this.width = width;
            this.right = Vector3.Cross(normal, up);
            this.direction = normal;
            this.up = up;
            this.position = pos;
        }

        public JumpRing(JumpRing j)
        {
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
            List<Vector2> texCoords = ringTexCoords;
            if (width > .5f) texCoords = tubeTexCoords;
            if (ringList == null)
            {
                c = r.currentColor;
                ringList = new List<VertexPositionColorNormalTexture>();
                for(int i = 0; i < ringSegments; i++)
                {
                    
                    int nextIndex = (i + 1) % ringSegments;
                    Vector3 pos = position + radius * (ringVectorList[i].X * up + ringVectorList[i].Y * right);
                    Vector3 nextPos = position + radius * (ringVectorList[nextIndex].X * up + ringVectorList[nextIndex].Y * right);
                    Vector3 normal = Vector3.Cross(nextPos - pos, direction);
                    Vector3 innerPos = position + .8f * radius * (ringVectorList[i].X * up + ringVectorList[i].Y * right);
                    Vector3 nextinnerPos = position + .8f * radius * (ringVectorList[nextIndex].X * up + ringVectorList[nextIndex].Y * right);

                    Color shadedColor = c;
                    
                    ringList.Add(new VertexPositionColorNormalTexture(pos, c, normal, texCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos + width * direction, c, normal, texCoords[2]));
                    ringList.Add(new VertexPositionColorNormalTexture(pos + width * direction, c, normal, texCoords[3]));

                    ringList.Add(new VertexPositionColorNormalTexture(pos, c, normal, texCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos, c, normal, texCoords[1]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos + width * direction, c, normal, texCoords[2]));

                    ringList.Add(new VertexPositionColorNormalTexture(innerPos, c, normal, texCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos + width * direction, c, normal, texCoords[2]));
                    ringList.Add(new VertexPositionColorNormalTexture(innerPos + width * direction, c, normal, texCoords[3]));

                    ringList.Add(new VertexPositionColorNormalTexture(innerPos, c, normal, texCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos, c, normal, texCoords[1]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos + width * direction, c, normal, texCoords[2]));


                    ringList.Add(new VertexPositionColorNormalTexture(innerPos, shadedColor, normal, ringTexCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos, shadedColor, normal, ringTexCoords[2]));
                    ringList.Add(new VertexPositionColorNormalTexture(pos, shadedColor, normal, ringTexCoords[3]));

                    ringList.Add(new VertexPositionColorNormalTexture(innerPos, shadedColor, normal, ringTexCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos, shadedColor, normal, ringTexCoords[1]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos, shadedColor, normal, ringTexCoords[2]));

                    ringList.Add(new VertexPositionColorNormalTexture(innerPos + width * direction, shadedColor, normal, ringTexCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos + width * direction, shadedColor, normal, ringTexCoords[2]));
                    ringList.Add(new VertexPositionColorNormalTexture(pos + width * direction, shadedColor, normal, ringTexCoords[3]));

                    ringList.Add(new VertexPositionColorNormalTexture(innerPos + width * direction, shadedColor, normal, ringTexCoords[0]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextinnerPos + width * direction, shadedColor, normal, ringTexCoords[1]));
                    ringList.Add(new VertexPositionColorNormalTexture(nextPos + width * direction, shadedColor, normal, ringTexCoords[2]));
                }                
            }
            if (r.currentColor != c)
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

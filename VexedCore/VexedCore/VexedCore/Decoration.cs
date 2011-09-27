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
    public class Decoration
    {
        public bool dynamic = false;
        public bool refreshVertexData = false;
        public bool shouldRender = true;
        public float halfWidth = .5f;
        public float halfHeight = .5f;
        public Vertex position;
        public string id = "";

        public Texture2D decorationTexture;
        public static Texture2D defaultTexture;

        public static List<Vector2> fullTexCoords;

        static Decoration()
        {
            fullTexCoords = new List<Vector2>();
            fullTexCoords.Add(new Vector2(1, 0));
            fullTexCoords.Add(new Vector2(0, 0));
            fullTexCoords.Add(new Vector2(0, 1));
            fullTexCoords.Add(new Vector2(1, 1));
        }

        public Decoration(Decoration d)
        {
            position = new Vertex(d.position);
            id = d.id;
            dynamic = d.dynamic;
            shouldRender = d.shouldRender;
            halfWidth = d.halfWidth;
            halfHeight = d.halfHeight;
            decorationTexture = d.decorationTexture;
            
        }

        public Decoration()
        {
        }

        public Decoration(VexedLib.Decoration xmlDecoration, Vector3 normal)
        {
            this.position = new Vertex(xmlDecoration.position, normal, Vector3.Zero, xmlDecoration.up);
            id = xmlDecoration.IDString;
            if ("Default" == xmlDecoration.texture)
            {
                decorationTexture = null;
            }
            else
            {
                halfWidth = 1f * decorationTexture.Width / 256f;
                halfHeight = 1f * decorationTexture.Height / 256f;
            }

            
        }

        public float right_mag
        {
            get
            {
                return halfWidth;
            }
        }
        public float left_mag
        {
            get
            {
                return -halfWidth;
            }
        }
        public float up_mag
        {
            get
            {
                return halfHeight;
            }
        }
        public float down_mag
        {
            get
            {
                return -halfHeight;
            }
        }

        public Vector3 right
        {
            get
            {
                return right_mag * rightUnit;
            }
        }
        public Vector3 left
        {
            get
            {
                return left_mag * rightUnit;
            }
        }
        public Vector3 up
        {
            get
            {
                return up_mag * upUnit;
            }
        }
        public Vector3 down
        {
            get
            {
                return down_mag * upUnit;
            }
        }
        public Vector3 upUnit
        {
            get
            {
                return position.direction;
            }
        }
        public Vector3 rightUnit
        {
            get
            {
                return Vector3.Cross(position.direction, position.normal);
            }
        }

        public float depth
        {
            get
            {
                return .5f;
            }
        }

        public List<VertexPositionColorNormalTexture> decalList;
        public VertexPositionColorNormalTexture[] triangleArray;

        public void UpdateVertexData(Room currentRoom)
        {
            if (decalList == null || Engine.staticDoodadsInitialized == false || dynamic == true || refreshVertexData == true)
            {
                refreshVertexData = false;
                decalList = new List<VertexPositionColorNormalTexture>();

                List<Vertex> vList = new List<Vertex>();
                vList.Add(new Vertex(position, up + right));
                vList.Add(new Vertex(position, up + left));
                vList.Add(new Vertex(position, down + left));
                vList.Add(new Vertex(position, down + right));


                currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth, fullTexCoords, decalList, true);

                triangleArray = new VertexPositionColorNormalTexture[decalList.Count];
                for (int i = 0; i < decalList.Count; i++)
                {
                    triangleArray[i] = decalList[i];
                }
            }
        }

        public void Draw(Room currentRoom)
        {
            if (shouldRender == true)
            {
                UpdateVertexData(currentRoom);

                if (decorationTexture != null)
                    Engine.playerTextureEffect.Texture = decorationTexture;
                else
                    Engine.playerTextureEffect.Texture = defaultTexture;
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
        }
    }
}

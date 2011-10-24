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
        public float _depth = 0f;
        public Color color = Color.White;
        public bool wrap = false;

        public Texture2D decorationTexture;
        public static Texture2D defaultTexture;
        public string fileName = "Default";

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
            //decorationTexture = d.decorationTexture;
            _depth = d._depth;
            color = d.color;
            wrap = d.wrap;
            fileName = d.fileName;
            
        }

        public Decoration()
        {
        }

        public Decoration(VexedLib.Decoration xmlDecoration, Vector3 normal)
        {
            this.position = new Vertex(xmlDecoration.position, normal, Vector3.Zero, xmlDecoration.up);
            id = xmlDecoration.IDString;
            decorationTexture = null;
            fileName = xmlDecoration.texture;
            _depth = (1f*xmlDecoration.depth)/100f - .5f;
            color = xmlDecoration.color;
            wrap = xmlDecoration.wrap;
        }

        public void SetTexture()
        {            
            if (decorationTexture == null)
            {
                decorationTexture = DecorationImage.FetchTexture(fileName);
            }
        }

        public void UpdateSizeData()
        {
            if (decorationTexture != null)
            {
                halfWidth = 1f * decorationTexture.Width / 256;
                halfHeight = 1f * decorationTexture.Height / 256;
                position.position += position.direction * (halfHeight - .5f);
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
                return _depth;
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
                if (wrap == true)
                {
                    foreach (Vertex v in vList)
                    {
                        v.Update(currentRoom, 1);
                    }
                }

                currentRoom.AddTextureToTriangleList(vList, color, depth, decalList, fullTexCoords, false);
                //currentRoom.AddBlockFrontToTriangleList(vList, color, depth, fullTexCoords, decalList, true);

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

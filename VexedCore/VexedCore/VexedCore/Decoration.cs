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
        public int frame = 0;
        public int maxFrame = 1;
        public int animationTime = 0;
        public int maxAnimationTime = 24;
        public int maxSpinAnimationTime = 35;
        public bool freeSpin = false;
        public int spinTargetFrame = 0;
        public bool reverseAnimation = false;

        public List<Texture2D> decorationTexture;
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
            freeSpin = d.freeSpin;
            frame = d.frame;
            reverseAnimation = d.reverseAnimation;
            
        }

        public Decoration()
        {
        }

        public Decoration(Vector3 position, Vector3 normal, Vector3 up, String texture, float depth)
        {
            this.position = new Vertex(position, normal, Vector3.Zero, up);
            id = "BeamBase";
            decorationTexture = null;
            fileName = texture;
            _depth = (1f * depth) / 100f - .5f;
            color = Color.White;
            wrap = true;
            freeSpin = false;
            frame = 0;
            reverseAnimation = false;
        }

        public Decoration(VL.Decoration xmlDecoration, Vector3 normal)
        {
            this.position = new Vertex(xmlDecoration.position, normal, Vector3.Zero, xmlDecoration.up);
            id = xmlDecoration.IDString;
            decorationTexture = null;
            fileName = xmlDecoration.texture;
            _depth = (1f*xmlDecoration.depth)/100f - .5f;
            color = xmlDecoration.color;
            wrap = xmlDecoration.wrap;
            freeSpin = xmlDecoration.freespin;
            frame = xmlDecoration.startFrame;
            reverseAnimation = xmlDecoration.reverseAnimation;
        }

        public void SetTexture()
        {            
            if (decorationTexture == null)
            {
                DecorationTexture imageData = DecorationImage.FetchTexture(fileName);


                if (imageData != null)
                {
                    decorationTexture = imageData.texture;
                    if (imageData.forceSpin)
                        freeSpin = true;
                    maxFrame = decorationTexture.Count;
                }
            }
        }

        public void UpdateSizeData()
        {
            if (decorationTexture != null)
            {
                halfWidth = 1f * decorationTexture[frame].Width / 256;
                halfHeight = 1f * decorationTexture[frame].Height / 256;
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

        public void Update(GameTime gameTime)
        {
            if(freeSpin == false)
            {
                animationTime += gameTime.ElapsedGameTime.Milliseconds;
                if (animationTime > maxAnimationTime)
                {
                    if (reverseAnimation == false)
                    {
                        animationTime = 0;
                        frame++;
                        frame %= maxFrame;
                    }
                    else
                    {
                        animationTime = 0;
                        frame--;
                        frame += maxFrame;
                        frame %= maxFrame;
                    }
                }
            }
            else
            {
                Vector3 effectiveUp = Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false);
                if (Vector3.Dot(effectiveUp, upUnit) > .95f)
                {
                    spinTargetFrame = 0;
                }
                else if (Vector3.Dot(effectiveUp, upUnit) < -.95f)
                {
                    spinTargetFrame = 4;
                }
                else if (Vector3.Dot(effectiveUp, rightUnit) > .95f)
                {
                    spinTargetFrame = 2;
                }
                else if (Vector3.Dot(effectiveUp, rightUnit) < -.95f)
                {
                    spinTargetFrame = 6;
                }
                animationTime += gameTime.ElapsedGameTime.Milliseconds;
                if (animationTime > maxSpinAnimationTime)
                {
                    animationTime = 0;
                    if (spinTargetFrame == frame)
                    {
                    }
                    else if ( (spinTargetFrame - frame + maxFrame) % maxFrame > maxFrame/2)
                    {
                        frame--;
                        frame += maxFrame;                        
                        frame %= maxFrame;
                    }
                    else
                    {
                        frame++;
                        frame %= maxFrame;
                    }
                }
            }            
        }

        public void Draw(Room currentRoom)
        {
            if (shouldRender == true)
            {
                UpdateVertexData(currentRoom);
                
                if (decorationTexture != null)
                    Engine.playerTextureEffect.Texture = decorationTexture[frame];
                else
                    Engine.playerTextureEffect.Texture = defaultTexture;
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);

            }
        }
    }
}

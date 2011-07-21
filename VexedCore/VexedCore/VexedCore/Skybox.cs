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
    class Skybox
    {

        public static Texture2D[] skyBoxTextures;
        public static BasicEffect[] skyBoxEffects;

        public static void Init()
        {
            skyBoxEffects = new BasicEffect[6];
            for (int i = 0; i < 6; i++)
            {
                skyBoxEffects[i] = new BasicEffect(Game1.graphicsDevice);                
                skyBoxEffects[i].TextureEnabled = true;
            }            
        }

        public static void Draw(Player player)
        {

            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
            Game1.graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            for (int i = 0; i < 6; i++)
            {
                skyBoxEffects[i].World = Matrix.Identity;
                skyBoxEffects[i].View = Matrix.CreateLookAt(Vector3.Zero, player.cameraTarget - player.cameraPos, player.cameraUp);
                skyBoxEffects[i].Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI/4, Game1.graphicsDevice.Viewport.AspectRatio, .1f, 10000);
                
                skyBoxEffects[i].Texture = skyBoxTextures[i];
             
            }
            List<VertexPositionTexture> skyBoxVertexList = new List<VertexPositionTexture>();

            // Pos X
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, 100, 100),new Vector2(0,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, 100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, 100, -100),new Vector2(0,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, 100, -100),new Vector2(0,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, 100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, -100),new Vector2(1,0)));

            // Neg X
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, 100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, -100, 100),new Vector2(0,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, -100),new Vector2(1,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, -100),new Vector2(1,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, -100, 100),new Vector2(0,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, -100, -100),new Vector2(0,0)));

            // Pos Y
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, 100, 100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, 100),new Vector2(0,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, 100, -100),new Vector2(1,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, 100, -100),new Vector2(1,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, 100),new Vector2(0,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, -100),new Vector2(0,0)));

            // Neg Y
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, 100), new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, -100, 100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, -100),new Vector2(0,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, -100),new Vector2(0,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, -100, 100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, -100, -100),new Vector2(1,0)));

            // Pos Z
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, 100, 100),new Vector2(1,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, 100),new Vector2(0,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, 100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, 100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, 100),new Vector2(0,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, -100, 100),new Vector2(0,1)));

            // Neg Z
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, 100, -100),new Vector2(1,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, -100),new Vector2(0,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, -100),new Vector2(1,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(100, -100, -100),new Vector2(1,0)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, 100, -100),new Vector2(0,1)));
            skyBoxVertexList.Add(new VertexPositionTexture(new Vector3(-100, -100, -100),new Vector2(0,0)));

            /*skyBoxEffects[0].CurrentTechnique.Passes[0].Apply();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    skyBoxVertexList.ToArray(), 0, 12, VertexPositionTexture.VertexDeclaration);*/
            for (int i = 0; i < 6; i++)
            {
                skyBoxEffects[i].CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    skyBoxVertexList.ToArray(), i*6, 2, VertexPositionTexture.VertexDeclaration);
            }
        }
    }
}

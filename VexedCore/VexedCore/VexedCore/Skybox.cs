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
        
        public static Texture2D[] skyBoxGreen;
        public static Texture2D[] skyBoxRed;
        public static Texture2D[] skyBoxBlue;
        public static Texture2D[] skyBoxDeepSpace;
        public static Texture2D[] skyBoxSolar;
        public static Texture2D[] skyBoxGalaxy;
        public static BasicEffect[] skyBoxEffects;
        public static BasicEffect[] skyBoxEffectsAlt;

        public static void Init()
        {
            skyBoxEffects = new BasicEffect[6];
            for (int i = 0; i < 6; i++)
            {
                skyBoxEffects[i] = new BasicEffect(Game1.graphicsDevice);
                skyBoxEffects[i].TextureEnabled = true;
                skyBoxEffects[i].VertexColorEnabled = true;                
            }
            skyBoxEffectsAlt = new BasicEffect[6];
            for (int i = 0; i < 6; i++)
            {
                skyBoxEffectsAlt[i] = new BasicEffect(Game1.graphicsDevice);
                skyBoxEffectsAlt[i].TextureEnabled = true;
                skyBoxEffectsAlt[i].VertexColorEnabled = true;
            }            
        }

        public static void LoadTextures(ContentManager Content)
        {

            skyBoxGreen = new Texture2D[6];
            skyBoxGreen[0] = Content.Load<Texture2D>("Skybox\\Green\\skybox_right");
            skyBoxGreen[1] = Content.Load<Texture2D>("Skybox\\Green\\skybox_left");
            skyBoxGreen[2] = Content.Load<Texture2D>("Skybox\\Green\\skybox_front");
            skyBoxGreen[3] = Content.Load<Texture2D>("Skybox\\Green\\skybox_back");
            skyBoxGreen[4] = Content.Load<Texture2D>("Skybox\\Green\\skybox_bottom");
            skyBoxGreen[5] = Content.Load<Texture2D>("Skybox\\Green\\skybox_top");

            skyBoxRed = new Texture2D[6];
            skyBoxRed[0] = Content.Load<Texture2D>("Skybox\\Red\\skybox_right");
            skyBoxRed[1] = Content.Load<Texture2D>("Skybox\\Red\\skybox_left");
            skyBoxRed[2] = Content.Load<Texture2D>("Skybox\\Red\\skybox_front");
            skyBoxRed[3] = Content.Load<Texture2D>("Skybox\\Red\\skybox_back");
            skyBoxRed[4] = Content.Load<Texture2D>("Skybox\\Red\\skybox_bottom");
            skyBoxRed[5] = Content.Load<Texture2D>("Skybox\\Red\\skybox_top");

            skyBoxBlue = new Texture2D[6];
            skyBoxBlue[0] = Content.Load<Texture2D>("Skybox\\Blue\\skybox_right");
            skyBoxBlue[1] = Content.Load<Texture2D>("Skybox\\Blue\\skybox_left");
            skyBoxBlue[2] = Content.Load<Texture2D>("Skybox\\Blue\\skybox_front");
            skyBoxBlue[3] = Content.Load<Texture2D>("Skybox\\Blue\\skybox_back");
            skyBoxBlue[4] = Content.Load<Texture2D>("Skybox\\Blue\\skybox_bottom");
            skyBoxBlue[5] = Content.Load<Texture2D>("Skybox\\Blue\\skybox_top");

            skyBoxDeepSpace = new Texture2D[6];
            skyBoxDeepSpace[0] = Content.Load<Texture2D>("Skybox\\DeepSpace\\skybox_right");
            skyBoxDeepSpace[1] = Content.Load<Texture2D>("Skybox\\DeepSpace\\skybox_left");
            skyBoxDeepSpace[2] = Content.Load<Texture2D>("Skybox\\DeepSpace\\skybox_front");
            skyBoxDeepSpace[3] = Content.Load<Texture2D>("Skybox\\DeepSpace\\skybox_back");
            skyBoxDeepSpace[4] = Content.Load<Texture2D>("Skybox\\DeepSpace\\skybox_bottom");
            skyBoxDeepSpace[5] = Content.Load<Texture2D>("Skybox\\DeepSpace\\skybox_top");

            skyBoxSolar = new Texture2D[6];
            skyBoxSolar[0] = Content.Load<Texture2D>("Skybox\\Solar\\skybox_right");
            skyBoxSolar[1] = Content.Load<Texture2D>("Skybox\\Solar\\skybox_left");
            skyBoxSolar[2] = Content.Load<Texture2D>("Skybox\\Solar\\skybox_front");
            skyBoxSolar[3] = Content.Load<Texture2D>("Skybox\\Solar\\skybox_back");
            skyBoxSolar[4] = Content.Load<Texture2D>("Skybox\\Solar\\skybox_bottom");
            skyBoxSolar[5] = Content.Load<Texture2D>("Skybox\\Solar\\skybox_top");

            skyBoxGalaxy = new Texture2D[6];
            skyBoxGalaxy[0] = Content.Load<Texture2D>("Skybox\\Galaxy\\skybox_right");
            skyBoxGalaxy[1] = Content.Load<Texture2D>("Skybox\\Galaxy\\skybox_left");
            skyBoxGalaxy[2] = Content.Load<Texture2D>("Skybox\\Galaxy\\skybox_front");
            skyBoxGalaxy[3] = Content.Load<Texture2D>("Skybox\\Galaxy\\skybox_back");
            skyBoxGalaxy[4] = Content.Load<Texture2D>("Skybox\\Galaxy\\skybox_bottom");
            skyBoxGalaxy[5] = Content.Load<Texture2D>("Skybox\\Galaxy\\skybox_top");

        }

        public static VertexPositionColorNormalTexture[] skyBoxVerticesMain;
        public static VertexPositionColorNormalTexture[] skyBoxVerticesAlt;

        public static VertexPositionColorNormalTexture[] GenerateSkyBoxVertices(Color c)
        {
            VertexPositionColorNormalTexture[] skyBoxVertices;

            List<VertexPositionColorNormalTexture> skyBoxVertexList = new List<VertexPositionColorNormalTexture>();
            // Pos X
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, 100, 100), c, Vector3.UnitZ, new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, 100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, 100, -100), c, Vector3.UnitZ, new Vector2(0, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, 100, -100), c, Vector3.UnitZ, new Vector2(0, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, 100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, -100), c, Vector3.UnitZ, new Vector2(1, 0)));

            // Neg X
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, 100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, -100, 100), c, Vector3.UnitZ, new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, -100), c, Vector3.UnitZ, new Vector2(1, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, -100), c, Vector3.UnitZ, new Vector2(1, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, -100, 100), c, Vector3.UnitZ, new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, -100, -100), c, Vector3.UnitZ, new Vector2(0, 0)));

            // Pos Y
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, 100, 100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, 100), c, Vector3.UnitZ, new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, 100, -100), c, Vector3.UnitZ, new Vector2(1, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, 100, -100), c, Vector3.UnitZ, new Vector2(1, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, 100), c, Vector3.UnitZ, new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, -100), c, Vector3.UnitZ, new Vector2(0, 0)));

            // Neg Y
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, 100), c, Vector3.UnitZ, new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, -100, 100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, -100), c, Vector3.UnitZ, new Vector2(0, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, -100), c, Vector3.UnitZ, new Vector2(0, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, -100, 100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, -100, -100), c, Vector3.UnitZ, new Vector2(1, 0)));

            // Pos Z
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, 100, 100), c, Vector3.UnitZ, new Vector2(1, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, 100), c, Vector3.UnitZ, new Vector2(0, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, 100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, 100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, 100), c, Vector3.UnitZ, new Vector2(0, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, -100, 100), c, Vector3.UnitZ, new Vector2(0, 1)));

            // Neg Z
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, 100, -100), c, Vector3.UnitZ, new Vector2(1, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, -100), c, Vector3.UnitZ, new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, -100), c, Vector3.UnitZ, new Vector2(1, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(100, -100, -100), c, Vector3.UnitZ, new Vector2(1, 0)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, 100, -100), c, Vector3.UnitZ, new Vector2(0, 1)));
            skyBoxVertexList.Add(new VertexPositionColorNormalTexture(new Vector3(-100, -100, -100), c, Vector3.UnitZ, new Vector2(0, 0)));

            skyBoxVertices = skyBoxVertexList.ToArray();

            return skyBoxVertices;
        }

        public static void Draw(Player player)
        {

            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
            Game1.graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            Texture2D[] skyBoxTextures = skyBoxDeepSpace;
            if(Engine.player.currentRoom.parentSector.skyboxType == SkyBoxType.Blue)
                skyBoxTextures = skyBoxBlue;
            if (Engine.player.currentRoom.parentSector.skyboxType == SkyBoxType.Green)
                skyBoxTextures = skyBoxGreen;
            if (Engine.player.currentRoom.parentSector.skyboxType == SkyBoxType.Red)
                skyBoxTextures = skyBoxRed;
            if (Engine.player.currentRoom.parentSector.skyboxType == SkyBoxType.Solar)
                skyBoxTextures = skyBoxSolar;
            if (Engine.player.currentRoom.parentSector.skyboxType == SkyBoxType.Galaxy)
                skyBoxTextures = skyBoxGalaxy;

            for (int i = 0; i < 6; i++)
            {
                skyBoxEffects[i].World = Matrix.Identity;
                if (Engine.state == EngineState.Active)
                    skyBoxEffects[i].View = Matrix.CreateLookAt(Vector3.Zero, Engine.cameraTarget - Engine.cameraPos, Engine.cameraUp);
                if (Engine.state == EngineState.Map)
                    skyBoxEffects[i].View = Matrix.CreateLookAt(Vector3.Zero, WorldMap.cameraTarget - WorldMap.cameraPosition, WorldMap.cameraUp);
                skyBoxEffects[i].Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4, Game1.graphicsDevice.Viewport.AspectRatio, .1f, 10000);

                skyBoxEffects[i].Texture = skyBoxTextures[i];

            }

            Texture2D[] skyBoxTexturesAlt = skyBoxDeepSpace;
            float jumpProgress = 0f;
            if ((Engine.player.state == State.Jump || Engine.player.state == State.BridgeJump) && Engine.player.jumpRoom.parentSector != Engine.player.currentRoom.parentSector)
            {
                jumpProgress = (1f * Engine.player.launchTime) / Player.launchMaxTime;
                if (Engine.player.jumpRoom.parentSector.skyboxType == SkyBoxType.Blue)
                    skyBoxTexturesAlt = skyBoxBlue;
                if (Engine.player.jumpRoom.parentSector.skyboxType == SkyBoxType.Green)
                    skyBoxTexturesAlt = skyBoxGreen;
                if (Engine.player.jumpRoom.parentSector.skyboxType == SkyBoxType.Red)
                    skyBoxTexturesAlt = skyBoxRed;
                if (Engine.player.jumpRoom.parentSector.skyboxType == SkyBoxType.Solar)
                    skyBoxTexturesAlt = skyBoxSolar;
                if (Engine.player.jumpRoom.parentSector.skyboxType == SkyBoxType.Galaxy)
                    skyBoxTexturesAlt = skyBoxGalaxy;

                for (int i = 0; i < 6; i++)
                {
                    skyBoxEffectsAlt[i].World = Matrix.Identity;
                    if (Engine.state == EngineState.Active)
                        skyBoxEffectsAlt[i].View = Matrix.CreateLookAt(Vector3.Zero, Engine.cameraTarget - Engine.cameraPos, Engine.cameraUp);
                    if (Engine.state == EngineState.Map)
                        skyBoxEffectsAlt[i].View = Matrix.CreateLookAt(Vector3.Zero, WorldMap.cameraTarget - WorldMap.cameraPosition, WorldMap.cameraUp);
                    skyBoxEffectsAlt[i].Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4, Game1.graphicsDevice.Viewport.AspectRatio, .1f, 10000);

                    skyBoxEffectsAlt[i].Texture = skyBoxTexturesAlt[i];

                }
            }
                

            List<VertexPositionColorNormalTexture> skyBoxVertexList = new List<VertexPositionColorNormalTexture>();

            if (jumpProgress == 0)
            {
                skyBoxVerticesMain = GenerateSkyBoxVertices(Color.White);
            }
            if (skyBoxVerticesMain == null || jumpProgress != 0)
            {
                Color transparent = Color.White;
                transparent.G = (Byte)((1f - jumpProgress) * 255);
                transparent.R = (Byte)((1f - jumpProgress) * 255);
                transparent.B = (Byte)((1f - jumpProgress) * 255);
                transparent.A = (Byte)((1f - jumpProgress) * 255);
                skyBoxVerticesMain = GenerateSkyBoxVertices(transparent);
            }
            if (jumpProgress != 0)
            {
                Color transparent = Color.Transparent;
                transparent.R = (Byte)((jumpProgress) * 255);
                transparent.G = (Byte)((jumpProgress) * 255);
                transparent.B = (Byte)((jumpProgress) * 255);
                transparent.A = (Byte)((jumpProgress) * 255);

                skyBoxVerticesAlt = GenerateSkyBoxVertices(transparent);
            }

            /*skyBoxEffects[0].CurrentTechnique.Passes[0].Apply();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    skyBoxVertexList.ToArray(), 0, 12, VertexPositionTexture.VertexDeclaration);*/
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
            
            for (int i = 0; i < 6; i++)
            {
                skyBoxEffects[i].CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    skyBoxVerticesMain, i * 6, 2, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            if (jumpProgress != 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    skyBoxEffectsAlt[i].CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        skyBoxVerticesAlt, i * 6, 2, VertexPositionColorNormalTexture.VertexDeclaration);
                }
            }
        }
    }
}

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
    public enum EngineState
    {
        Active,
        Paused,
        ZoomOut,
        ZoomIn,
        Warp,
        Map        
    }

    public class Engine
    {
        public static EngineState state = EngineState.Active;
        public static float zoomLevel = 0f;
        public static float zoomSpeed = .001f;
        public static Vector3 zoomOutCameraPos;
        public static Vector3 zoomOutCameraTarget;
        public static Vector3 zoomOutCameraUp;

        public static List<Room> roomList;
        public static Player player;
        public static DialogBox dialogBox;
        public static SpriteFont spriteFont;

        public static int selectedRoomIndex = 0;

        public static SpriteBatch spriteBatch;
        public BasicEffect translucentEffect = null;
        public BasicEffect mapEffect = null;
        public AlphaTestEffect playerTextureEffect = null;
        public BasicEffect worldTextureEffect = null;
        public BasicEffect skyBoxEffect = null;
        public Effect cartoonEffect = null;
        public Effect postprocessEffect = null;

        public static bool transparencyEnabled = true;
        public static int lightingLevel = 0;
        public static bool toonShadingEnabled = false;
        public static float drawDistance = 150f;
        public int optionToggleCooldown = 0;
        public static bool reDraw = false;
        public static bool detailTextures = true;
        public static int depthCount =80;

        /*public static List<VertexPositionColorNormalTexture> staticOpaqueObjects;
        public static List<VertexPositionColorNormalTexture> dynamicOpaqueObjects;
        public static List<VertexPositionColorNormalTexture> staticDetailObjects;
        public static List<VertexPositionColorNormalTexture> dynamicDetailObjects;*/


        public static VertexPositionColorNormalTexture[] detailVertexArray;
        public static int detailVertexArrayCount = 0;
        public static VertexPositionColorNormalTexture[] doodadVertexArray;
        public static int doodadVertexArrayCount =0;
        public static VertexPositionColorNormalTexture[] decalVertexArray;
        public static int decalVertexArrayCount = 0;
        public static VertexPositionColorNormalTexture[] staticBlockVertexArray;
        public static int staticBlockVertexArrayCount = 0;
        public static VertexPositionColorNormalTexture[] dynamicBlockVertexArray;
        public static int dynamicBlockVertexArrayCount = 0;
        public static List<TrasnparentSquare> staticTranslucentObjects;
        public static List<TrasnparentSquare> mapShellObjects;
        public static VertexBuffer staticObjectBuffer;
        public static bool staticObjectsInitialized = false;
        public static bool staticDoodadsInitialized = false;

        public static RenderTarget2D sceneRenderTarget;
        public static RenderTarget2D normalDepthRenderTarget;
        NonPhotoRealisticSettings Settings
        {
            get { return NonPhotoRealisticSettings.PresetSettings[settingsIndex]; }
        }
        int settingsIndex = 5;     

        Vector3 currentTarget = Vector3.Zero;
        Vector3 currentCamera = new Vector3(30, 30, 30);
        Vector3 currentUp = new Vector3(0, 0, 1);
        float currentRotate = 0;
        float currentPitch = 0;

        public Engine()
        {
            staticTranslucentObjects = new List<TrasnparentSquare>();
            mapShellObjects = new List<TrasnparentSquare>();

            zoomOutCameraUp = Vector3.UnitZ;
            zoomOutCameraTarget = Vector3.Zero;
            zoomOutCameraPos = new Vector3(80, 70, 100);
        }

        public void Init()
        {
            translucentEffect = new BasicEffect(Game1.graphicsDevice);
            translucentEffect.VertexColorEnabled = true;
            translucentEffect.Alpha = 1f;
            translucentEffect.SpecularPower = 0.1f;
            translucentEffect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            translucentEffect.DiffuseColor = new Vector3(1, 1, 1);
            translucentEffect.SpecularColor = new Vector3(0, 1f, 1f);

            mapEffect = new BasicEffect(Game1.graphicsDevice);
            mapEffect.VertexColorEnabled = true;
            mapEffect.Alpha = 1f;
            mapEffect.SpecularPower = 0.1f;
            mapEffect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            mapEffect.DiffuseColor = new Vector3(1, 1, 1);
            mapEffect.SpecularColor = new Vector3(1f, 1f, 1f);
            mapEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-10, -5, -1));
            mapEffect.DirectionalLight0.DiffuseColor = Color.Gray.ToVector3();
            mapEffect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();
            mapEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(10, 5, 1));
            mapEffect.DirectionalLight1.DiffuseColor = Color.Gray.ToVector3();
            mapEffect.DirectionalLight1.SpecularColor = Color.Black.ToVector3();
            mapEffect.LightingEnabled = true;
            mapEffect.DirectionalLight1.Enabled = true;
            mapEffect.DirectionalLight0.Enabled = true;
            /*if (Engine.lightingLevel > 0)
                mapEffect.LightingEnabled = true;
            if (Engine.lightingLevel > 1)
            {
                mapEffect.DirectionalLight1.Enabled = true;
                mapEffect.DirectionalLight0.Enabled = true;
            }*/

            playerTextureEffect = new AlphaTestEffect(Game1.graphicsDevice);
            playerTextureEffect.VertexColorEnabled = true;
            playerTextureEffect.Alpha = 1f;

            worldTextureEffect = new BasicEffect(Game1.graphicsDevice);
            worldTextureEffect.TextureEnabled = true;
            worldTextureEffect.VertexColorEnabled = true;
            worldTextureEffect.Alpha = 1f;
            worldTextureEffect.SpecularPower = 0.1f;
            worldTextureEffect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            worldTextureEffect.DiffuseColor = new Vector3(1, 1, 1);
            worldTextureEffect.SpecularColor = new Vector3(0, 1f, 1f);
            worldTextureEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-10, -5, -1));
            worldTextureEffect.DirectionalLight0.DiffuseColor = Color.Gray.ToVector3();
            worldTextureEffect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();
            worldTextureEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(10, 5, 1));
            worldTextureEffect.DirectionalLight1.DiffuseColor = Color.Gray.ToVector3();
            worldTextureEffect.DirectionalLight1.SpecularColor = Color.Black.ToVector3();
            if (Engine.lightingLevel > 0)
                worldTextureEffect.LightingEnabled = true;
            if (Engine.lightingLevel > 1)
            {
                worldTextureEffect.DirectionalLight1.Enabled = true;
                worldTextureEffect.DirectionalLight0.Enabled = true;
            }
            
        }

        public void DrawScene(bool depthShader)
        {
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            if (state != EngineState.Warp)
            {
                if (depthShader == false)
                {
                    worldTextureEffect.Texture = Room.blockTexture;
                    worldTextureEffect.CurrentTechnique.Passes[0].Apply();
                }
                else
                {

                    cartoonEffect.CurrentTechnique = cartoonEffect.Techniques["NormalDepth"];
                    cartoonEffect.CurrentTechnique.Passes[0].Apply();
                }

                if (doodadVertexArrayCount > 0)
                {
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        doodadVertexArray, 0, doodadVertexArrayCount / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

                if (staticBlockVertexArrayCount > 0)
                {                    
                    if (staticObjectsInitialized == false)
                    {
                        staticObjectBuffer = new VertexBuffer(Game1.graphicsDevice, VertexPositionColorNormalTexture.VertexDeclaration, staticBlockVertexArray.Length, BufferUsage.WriteOnly);
                        staticObjectBuffer.SetData<VertexPositionColorNormalTexture>(staticBlockVertexArray);                        

                    }
                    Game1.graphicsDevice.SetVertexBuffer(staticObjectBuffer);
                    Game1.graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList,
                        0, staticBlockVertexArrayCount / 3);
                }
                if (dynamicBlockVertexArrayCount > 0)
                {
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        dynamicBlockVertexArray, 0, dynamicBlockVertexArrayCount / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }



                Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
                playerTextureEffect.Texture = Room.blockTexture;
                playerTextureEffect.CurrentTechnique.Passes[0].Apply();


                if (detailVertexArrayCount > 0)
                {
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        detailVertexArray, 0, detailVertexArrayCount / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

                Game1.graphicsDevice.BlendState = BlendState.Opaque;
                playerTextureEffect.Texture = Ability.ability_textures;
                playerTextureEffect.CurrentTechnique.Passes[0].Apply();



                if (decalVertexArrayCount > 0)
                {
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        decalVertexArray, 0, decalVertexArrayCount / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }


                Game1.graphicsDevice.BlendState = BlendState.Opaque;

                if (detailTextures)
                    playerTextureEffect.Texture = Monster.monsterTextureDetail;
                else
                    playerTextureEffect.Texture = Monster.monsterTexture;
                playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Engine.player.currentRoom.DrawMonsters();

                playerTextureEffect.Texture = Projectile.projectileTexture;
                playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Engine.player.currentRoom.DrawProjectiles();

                Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

                if (player.insideBox == true)
                    player.DrawTexture(playerTextureEffect);

                if (depthShader == false && transparencyEnabled == true)
                {
                    if (Room.innerBlockMode > 0)
                    {
                        translucentEffect.CurrentTechnique.Passes[0].Apply();
                        // Sort Triangles
                        staticTranslucentObjects.Sort(new FaceSorter(player.cameraTarget - player.cameraPos));

                        List<VertexPositionColorNormalTexture> translucentList = new List<VertexPositionColorNormalTexture>();
                        for (int i = 0; i < staticTranslucentObjects.Count; i++)
                        {

                            translucentList.Add(staticTranslucentObjects[i].v1);
                            translucentList.Add(staticTranslucentObjects[i].v2);
                            translucentList.Add(staticTranslucentObjects[i].v3);
                            translucentList.Add(staticTranslucentObjects[i].v4);
                            translucentList.Add(staticTranslucentObjects[i].v5);
                            translucentList.Add(staticTranslucentObjects[i].v6);
                        }

                        if (translucentList.Count > 0)
                        {
                            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                                translucentList.ToArray(), 0, translucentList.Count / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                        }


                    }
                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
                }

                if (player.insideBox == false)
                    player.DrawTexture(playerTextureEffect);
            }
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
            
            if (transparencyEnabled == true)
            {
                //Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
                if (state == EngineState.ZoomOut || state == EngineState.ZoomIn || state == EngineState.Warp)
                {
                    if(depthShader == false)
                        mapEffect.CurrentTechnique.Passes[0].Apply();
                    else
                    {
                        cartoonEffect.CurrentTechnique = cartoonEffect.Techniques["NormalDepth"];
                        cartoonEffect.CurrentTechnique.Passes[0].Apply();
                    }
                    //translucentEffect.CurrentTechnique.Passes[0].Apply();
                    // Sort Triangles
                    mapShellObjects.Sort(new FaceSorter(-(zoomOutCameraPos-zoomOutCameraTarget)));

                    List<VertexPositionColorNormalTexture> translucentList = new List<VertexPositionColorNormalTexture>();
                    for (int i = 0; i < mapShellObjects.Count; i++)
                    {

                        translucentList.Add(mapShellObjects[i].v1);
                        translucentList.Add(mapShellObjects[i].v2);
                        translucentList.Add(mapShellObjects[i].v3);
                        translucentList.Add(mapShellObjects[i].v4);
                        translucentList.Add(mapShellObjects[i].v5);
                        translucentList.Add(mapShellObjects[i].v6);
                    }
                    if (selectedRoomIndex != -1)
                    {
                        Room selectedRoom = roomList[selectedRoomIndex];
                        Vector3 adjustedSize = adjustedSize = new Vector3(selectedRoom.size.X + 7f, selectedRoom.size.Y + 7f, selectedRoom.size.Z + 7f);
                        List<TrasnparentSquare> selectedBlockList = selectedRoom.GetMapBlock(adjustedSize, Color.White);
                        selectedBlockList.Sort(new FaceSorter(-(zoomOutCameraPos - zoomOutCameraTarget)));
                        for (int i = 0; i < selectedBlockList.Count; i++)
                        {

                            translucentList.Add(selectedBlockList[i].v1);
                            translucentList.Add(selectedBlockList[i].v2);
                            translucentList.Add(selectedBlockList[i].v3);
                            translucentList.Add(selectedBlockList[i].v4);
                            translucentList.Add(selectedBlockList[i].v5);
                            translucentList.Add(selectedBlockList[i].v6);
                        }
                    }

                    if (translucentList.Count > 0 && depthCount > 0)
                    {                       
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            translucentList.ToArray(), 0, mapShellObjects.Count * 2, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                    if (selectedRoomIndex != -1)
                    {
                        Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
                        if (translucentList.Count > 0 && depthCount > 0)
                        {
                            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                                translucentList.ToArray(), mapShellObjects.Count * 6, translucentList.Count() / 3 - mapShellObjects.Count * 2, VertexPositionColorNormalTexture.VertexDeclaration);
                        }
                    }
                }
                Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }

        public void Draw(GameTime gameTime)
        {
            Engine.staticTranslucentObjects = new List<TrasnparentSquare>();
            Engine.mapShellObjects = new List<TrasnparentSquare>();
            if(Engine.detailVertexArray == null)
                Engine.detailVertexArray = new VertexPositionColorNormalTexture[30000];
            detailVertexArrayCount = 0;
            if (Engine.doodadVertexArray == null)
                Engine.doodadVertexArray = new VertexPositionColorNormalTexture[30000];
            doodadVertexArrayCount = 0;
            if (Engine.decalVertexArray == null)
                Engine.decalVertexArray = new VertexPositionColorNormalTexture[30000];
            decalVertexArrayCount = 0;

            if (Engine.dynamicBlockVertexArray == null)
                Engine.dynamicBlockVertexArray = new VertexPositionColorNormalTexture[60000];
            dynamicBlockVertexArrayCount = 0;

            if (Engine.staticBlockVertexArray == null || reDraw == true)
            {
                staticObjectsInitialized = false;
                staticDoodadsInitialized = false;

                if (Engine.staticBlockVertexArray == null)
                    Engine.staticBlockVertexArray = new VertexPositionColorNormalTexture[60000];
                staticBlockVertexArrayCount = 0;
            }

            //bloom.BeginDraw();            

            // Set transform matrices.
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            Vector3 cameraPos = player.cameraPos * (1-zoomLevel) + zoomOutCameraPos * zoomLevel;
            
            Vector3 cameraUp = player.cameraUp * (1 - zoomLevel) + zoomOutCameraUp * zoomLevel;
            if (player.cameraUp == -zoomOutCameraUp)
            {
                Vector3 cameraSide = Vector3.UnitX;
                if(Math.Abs(Vector3.Dot(cameraSide, cameraUp)) == 1) 
                {
                    cameraSide = Vector3.UnitY;
                }
                cameraUp += zoomLevel * (1 - zoomLevel) * cameraSide;
                cameraUp.Normalize();
            }            
            Vector3 cameraTarget = player.cameraTarget * (1 - zoomLevel) + zoomOutCameraTarget * zoomLevel;

            translucentEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            translucentEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp);
            translucentEffect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);
            mapEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            mapEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp);
            mapEffect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);


            playerTextureEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            playerTextureEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp);
            playerTextureEffect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);

            worldTextureEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            worldTextureEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp);
            worldTextureEffect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);

            cartoonEffect.Parameters["World"].SetValue(Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch));
            cartoonEffect.Parameters["View"].SetValue(Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp));
            cartoonEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000));


            // Set renderstates.
            Game1.graphicsDevice.RasterizerState = RasterizerState.CullNone;
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
            
            
            foreach (Room r in roomList)
            {
                r.Draw(gameTime);
            }



            if (toonShadingEnabled)
            {
                Game1.graphicsDevice.SetRenderTarget(normalDepthRenderTarget);

                Game1.graphicsDevice.Clear(Color.Black);


                DrawScene(true);
            }




            if (toonShadingEnabled)
                Game1.graphicsDevice.SetRenderTarget(sceneRenderTarget);
            else
                Game1.graphicsDevice.SetRenderTarget(null);

            Game1.graphicsDevice.Clear(Color.Black);


            Skybox.Draw(player);
            DrawScene(false);



            // Run the postprocessing filter over the scene that we just rendered.
            if (toonShadingEnabled)
            {
                Game1.graphicsDevice.SetRenderTarget(null);

                ApplyPostprocess();
            }            
            
            Engine.staticObjectsInitialized = true;
            Engine.staticDoodadsInitialized = true;
            Engine.reDraw = false;


            if (Engine.player.secondaryAbility.isPassive == false)
                Ability.Draw(.825f, .02f, AbilityType.YButton);
            else
                Ability.Draw(.825f, .02f, AbilityType.Passive);

            if (Engine.player.primaryAbility.isPassive == false)            
                Ability.Draw(.75f, .12f, AbilityType.XButton);
            else
                Ability.Draw(.75f, .12f, AbilityType.Passive);
            Ability.Draw(.825f, .22f, AbilityType.AButton);
            Ability.Draw(.9f, .12f, AbilityType.BButton);

            Engine.player.secondaryAbility.Draw(.825f, .02f);
            Engine.player.primaryAbility.Draw(.75f, .12f);
            Ability.Draw(.825f, .22f, AbilityType.NormalJump, player.naturalShield.ammo, player.naturalShield.maxAmmo);
            Ability.Draw(.9f, .12f, AbilityType.Use);

            if (Engine.player.state == State.Dialog)
            {
                if (dialogBox == null)
                    dialogBox = new DialogBox();
                dialogBox.Draw();
            }
        }



        /// <summary>
        /// Helper applies the edge detection and pencil sketch postprocess effect.
        /// </summary>
        void ApplyPostprocess()
        {
            EffectParameterCollection parameters = postprocessEffect.Parameters;
            string effectTechniqueName;

            // Set effect parameters controlling the pencil sketch effect.
            if (Settings.EnableSketch)
            {
                parameters["SketchThreshold"].SetValue(Settings.SketchThreshold);
                parameters["SketchBrightness"].SetValue(Settings.SketchBrightness);
            }

            // Set effect parameters controlling the edge detection effect.
            if (Settings.EnableEdgeDetect)
            {
                Vector2 resolution = new Vector2(sceneRenderTarget.Width,
                                                 sceneRenderTarget.Height);

                Texture2D normalDepthTexture = normalDepthRenderTarget;

                parameters["EdgeWidth"].SetValue(Settings.EdgeWidth);
                parameters["EdgeIntensity"].SetValue(Settings.EdgeIntensity);
                parameters["ScreenResolution"].SetValue(resolution);
                parameters["NormalDepthTexture"].SetValue(normalDepthTexture);

                // Choose which effect technique to use.
                if (Settings.EnableSketch)
                {
                    if (Settings.SketchInColor)
                        effectTechniqueName = "EdgeDetectColorSketch";
                    else
                        effectTechniqueName = "EdgeDetectMonoSketch";
                }
                else
                    effectTechniqueName = "EdgeDetect";
            }
            else
            {
                // If edge detection is off, just pick one of the sketch techniques.
                if (Settings.SketchInColor)
                    effectTechniqueName = "ColorSketch";
                else
                    effectTechniqueName = "MonoSketch";
            }

            // Activate the appropriate effect technique.
            postprocessEffect.CurrentTechnique = postprocessEffect.Techniques[effectTechniqueName];

            // Draw a fullscreen sprite to apply the postprocessing effect.
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postprocessEffect);
            spriteBatch.Draw(sceneRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (state == EngineState.ZoomOut)
            {
                zoomLevel += zoomSpeed * gameTime.ElapsedGameTime.Milliseconds;
                if (zoomLevel > 1f)
                {
                    zoomLevel = 1f;
                    state = EngineState.Warp;
                }
            }
            if (state == EngineState.ZoomIn)
            {                
                zoomLevel -= zoomSpeed*gameTime.ElapsedGameTime.Milliseconds;
                if (zoomLevel < 0f)
                {
                    zoomLevel = 0f;
                    state = EngineState.Active;
                }
            }
            // Profiling help
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                System.Threading.Thread.Sleep(1);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                return;

            optionToggleCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (optionToggleCooldown < 0) optionToggleCooldown = 0;

            if (GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftTrigger))
            {
                player.baseCameraDistance += .03f * gameTime.ElapsedGameTime.Milliseconds;
                if (player.baseCameraDistance > 80f) player.baseCameraDistance = 80f;
            }
            if (GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightTrigger))
            {
                player.baseCameraDistance -= .03f * gameTime.ElapsedGameTime.Milliseconds;
                if (player.baseCameraDistance < 5f) player.baseCameraDistance = 5f;
            }

            if (optionToggleCooldown == 0)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.OemPlus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.A) && state == EngineState.Warp && selectedRoomIndex != -1)
                {
                    player.Warp(roomList[selectedRoomIndex]);
                    state = EngineState.ZoomIn;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.OemPlus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightShoulder) && state == EngineState.Warp && selectedRoomIndex == -1)
                {
                    state = EngineState.ZoomIn;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftShoulder) && state == EngineState.Active)
                {
                    selectedRoomIndex = -1;
                    state = EngineState.ZoomOut;

                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemPlus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightShoulder))
                {
                    if (state == EngineState.Warp && selectedRoomIndex != -1)
                    {
                        selectedRoomIndex++;
                        selectedRoomIndex %= roomList.Count();
                        while (roomList[selectedRoomIndex].hasWarp == false)
                        {
                            selectedRoomIndex++;
                            selectedRoomIndex %= roomList.Count();
                        }

                        optionToggleCooldown = 100;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftShoulder))
                {
                    if (state == EngineState.Warp && selectedRoomIndex != -1)
                    {
                        selectedRoomIndex--;
                        selectedRoomIndex += roomList.Count();
                        selectedRoomIndex %= roomList.Count();
                        while (roomList[selectedRoomIndex].hasWarp == false)
                        {
                            selectedRoomIndex--;
                            selectedRoomIndex += roomList.Count();
                            selectedRoomIndex %= roomList.Count();
                        }

                        optionToggleCooldown = 100;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D1))
                {
                    lightingLevel++;
                    lightingLevel %= 3;
                    if (lightingLevel == 0)
                    {
                        worldTextureEffect.LightingEnabled = false;
                        worldTextureEffect.DirectionalLight1.Enabled = false;
                        worldTextureEffect.DirectionalLight0.Enabled = false;
                    }
                    else if (lightingLevel == 1)
                    {
                        worldTextureEffect.LightingEnabled = true;
                        worldTextureEffect.DirectionalLight1.Enabled = false;
                        worldTextureEffect.DirectionalLight0.Enabled = true;
                    }
                    if (lightingLevel > 1)
                    {
                        worldTextureEffect.LightingEnabled = true;
                        worldTextureEffect.DirectionalLight1.Enabled = true;
                        worldTextureEffect.DirectionalLight0.Enabled = true;
                    }
                    optionToggleCooldown = 100;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D2))
                {
                    settingsIndex = 0;
                    toonShadingEnabled = !toonShadingEnabled;
                    optionToggleCooldown = 100;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D3))
                {
                    transparencyEnabled = !transparencyEnabled;
                    optionToggleCooldown = 100;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D4))
                {
                    if (Player.player_textures_detail == Player.player_textures)
                        Player.player_textures = Player.player_textures_clean;
                    else
                        Player.player_textures = Player.player_textures_detail;
                    optionToggleCooldown = 100;
                }


                if (Keyboard.GetState().IsKeyDown(Keys.D7))
                {
                    detailTextures = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D8))
                {
                    detailTextures = true;
                }

            }



            if (state == EngineState.Active)
            {

                // TODO: Add your update logic here
                player.Update(gameTime);

                foreach (Room r in roomList)
                    r.Update(gameTime);

                player.currentRoom.UpdateMonsters(gameTime);

                if(Engine.player.state != State.Death)
                    Physics.CollisionCheck(player.currentRoom, player, gameTime);
            }

            if (Engine.player.state == State.Dialog)
            {
                if (dialogBox != null)
                {
                    dialogBox.Update(gameTime);
                    if (Game1.controller.AButton.Pressed)
                    {
                        if (false == dialogBox.Next())
                        {
                            dialogBox = null;
                            Engine.player.jumpRecovery = 500;
                            Engine.player.state = State.Normal;
                        }
                    }
                }
            }
        }
    }
}

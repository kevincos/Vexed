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
        Map,
        Pause
    }

    public enum ResolutionSettings
    {
        R_400x300,
        R_800x600,
        R_1920x1080,
        R_1280x720        
    }

    public enum ControlType
    {
        MouseAndKeyboard,
        KeyboardOnly,
        GamePad
    }

    public class Engine
    {

        public static int debug_blocksGenerated;
        public static int debug_updateDoodadVertexData;
        public static DebugDataMonitor debug_doodadVertexUpdateMonitor;
        public static int debug_physicsRefresh;
        public static DebugDataMonitor debug_physicsRefreshMonitor;

        public static int saveFileIndex = 0;

        public static EngineState state = EngineState.Active;

        public static bool quit = false;

        public static Vector3 worldCenter = Vector3.Zero;
        public static List<Room> roomList;
        public static List<Sector> sectorList;
        
        public static Player player;
        public static DialogBox dialogBox;
        public static SpriteFont spriteFont;

        public SaveGameText saveGameText;

        public static bool soundEffectsEnabled = true;
        public static bool musicEnabled = false;

        
        public static SpriteBatch spriteBatch;
        public BasicEffect translucentEffect = null;
        public BasicEffect mapEffect = null;
        public static AlphaTestEffect playerTextureEffect = null;
        public BasicEffect worldTextureEffect = null;
        public BasicEffect skyBoxEffect = null;
        public Effect cartoonEffect = null;
        public Effect postprocessEffect = null;


        public static bool transparencyEnabled = true;
        public static int lightingLevel = 0;
        public static bool toonShadingEnabled = false;
        public static float drawDistance = 100f;
        public static ControlType controlType = ControlType.MouseAndKeyboard;
        public int optionToggleCooldown = 0;
        public static bool reDraw = false;
        public static bool detailTextures = true;
        public static bool fullScreen = false;
        public static int resWidth = 1920;
        public static int resHeight = 1280;
        public static ResolutionSettings res = ResolutionSettings.R_800x600;
        

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
        public static VertexPositionColorNormalTexture[] spriteVertexArray;
        public static int spriteVertexArrayCount = 0;
        public static VertexPositionColorNormalTexture[] beamVertexArray;
        public static int beamVertexArrayCount = 0;
        public static VertexPositionColorNormalTexture[] staticBlockVertexArray;
        public static int staticBlockVertexArrayCount = 0;
        public static VertexPositionColorNormalTexture[] dynamicBlockVertexArray;
        public static int dynamicBlockVertexArrayCount = 0;
        public static VertexPositionColorNormalTexture[] translucentBlockVertexArray;
        public static int translucentBlockVertexArrayCount = 0;
        

        public static List<TrasnparentSquare> staticTranslucentObjects;
        List<VertexPositionColorNormalTexture> translucentList;
        public static List<TrasnparentSquare> mapShellObjects;
        public static VertexBuffer staticObjectBuffer;
        public static bool staticObjectsInitialized = false;
        public static bool staticDoodadsInitialized = false;

        public static Vector3 cameraPos;
        public static Vector3 cameraUp;
        public static Vector3 cameraTarget;
        public static Vector3 playerCameraPos;
        public static Vector3 playerCameraUp;
        public static Vector3 playerCameraTarget;

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
          
        }

        public void Init()
        {
            debug_doodadVertexUpdateMonitor = new DebugDataMonitor();
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

            saveGameText = new SaveGameText();
            
        }

        public void DrawScene(bool depthShader)
        {
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            //if (WorldMap.state == ZoomState.None || WorldMap.state == ZoomState.ZoomFromSector || WorldMap.state == ZoomState.ZoomToSector)
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

                playerTextureEffect.Texture = Ability.ability_textures;
                playerTextureEffect.CurrentTechnique.Passes[0].Apply();



                if (decalVertexArrayCount > 0)
                {
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        decalVertexArray, 0, decalVertexArrayCount / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

                Game1.graphicsDevice.BlendState = BlendState.Opaque;

                if (spriteVertexArrayCount > 0)
                {
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        spriteVertexArray, 0, spriteVertexArrayCount / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

                playerTextureEffect.Texture = Doodad.beam_textures;
                playerTextureEffect.CurrentTechnique.Passes[0].Apply();

                if (beamVertexArrayCount > 0)
                {
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        beamVertexArray, 0, beamVertexArrayCount / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }


                
                if (detailTextures)
                    playerTextureEffect.Texture = Monster.monsterTextureDetail;
                else
                    playerTextureEffect.Texture = Monster.monsterTexture;
                playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Engine.player.currentRoom.DrawMonsters();

                playerTextureEffect.Texture = Projectile.projectileTexture;
                playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Engine.player.currentRoom.DrawProjectiles();

                foreach (Room r in Engine.roomList)
                {
                    r.DrawDecorations();
                }

                Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

                if (player.insideBox == true)
                    player.DrawTexture(playerTextureEffect);

                if (depthShader == false && transparencyEnabled == true)
                {
                    if (Room.innerBlockMode > 0)
                    {

                        //if (staticObjectsInitialized == false || (Engine.player.state == State.Jump) || Engine.player.state == State.Tunnel || Engine.player.state == State.Phase || Engine.player.state == State.PhaseFail)
                        {
                            // Sort Triangles
                            staticTranslucentObjects.Sort(new FaceSorter(player.cameraTarget - player.cameraPos));

                            translucentList = new List<VertexPositionColorNormalTexture>();
                            Engine.translucentBlockVertexArrayCount = 0;
                            for (int i = 0; i < staticTranslucentObjects.Count; i++)
                            {

                                /*translucentList.Add(staticTranslucentObjects[i].v1);
                                translucentList.Add(staticTranslucentObjects[i].v2);
                                translucentList.Add(staticTranslucentObjects[i].v3);
                                translucentList.Add(staticTranslucentObjects[i].v4);
                                translucentList.Add(staticTranslucentObjects[i].v5);
                                translucentList.Add(staticTranslucentObjects[i].v6);*/
                                translucentBlockVertexArray[Engine.translucentBlockVertexArrayCount] = staticTranslucentObjects[i].v1;
                                translucentBlockVertexArray[Engine.translucentBlockVertexArrayCount+1] = staticTranslucentObjects[i].v2;
                                translucentBlockVertexArray[Engine.translucentBlockVertexArrayCount+2] = staticTranslucentObjects[i].v3;
                                translucentBlockVertexArray[Engine.translucentBlockVertexArrayCount+3] = staticTranslucentObjects[i].v4;
                                translucentBlockVertexArray[Engine.translucentBlockVertexArrayCount+4] = staticTranslucentObjects[i].v5;
                                translucentBlockVertexArray[Engine.translucentBlockVertexArrayCount+5] = staticTranslucentObjects[i].v6;
                                Engine.translucentBlockVertexArrayCount += 6;
                            }
                        }

                        if (translucentBlockVertexArrayCount > 0)
                        {
                            translucentEffect.CurrentTechnique.Passes[0].Apply();
                            
                            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                                translucentBlockVertexArray, 0, translucentBlockVertexArrayCount / 3, VertexPositionColorNormalTexture.VertexDeclaration);
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
                if (WorldMap.state != ZoomState.None)
                {
                    playerTextureEffect.Texture = Ability.ability_textures;
                    playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    foreach (Room r in roomList)
                    {
                        r.DrawMapIcons();                       
                    }

                    if(depthShader == false)
                        mapEffect.CurrentTechnique.Passes[0].Apply();
                    else
                    {
                        cartoonEffect.CurrentTechnique = cartoonEffect.Techniques["NormalDepth"];
                        cartoonEffect.CurrentTechnique.Passes[0].Apply();
                    }
                    //translucentEffect.CurrentTechnique.Passes[0].Apply();
                    // Sort Triangles
                    if (WorldMap.selectedRoomIndex != -1)
                    {
                        Room selectedRoom = roomList[WorldMap.selectedRoomIndex];
                        Vector3 adjustedSize = adjustedSize = new Vector3(selectedRoom.size.X + 7f, selectedRoom.size.Y + 7f, selectedRoom.size.Z + 7f);
                        List<TrasnparentSquare> selectedBlockList = selectedRoom.GetMapBlock(adjustedSize, Color.White, true);
                        selectedBlockList.Sort(new FaceSorter(-(WorldMap.cameraPosition - WorldMap.cameraTarget)));
                        mapShellObjects.AddRange(selectedBlockList);
                    }

                    mapShellObjects.Sort(new FaceSorter(-(WorldMap.cameraPosition-WorldMap.cameraTarget)));

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
                    if (WorldMap.selectedRoomIndex != -1)
                    {
                        /*Room selectedRoom = roomList[WorldMap.selectedRoomIndex];
                        Vector3 adjustedSize = adjustedSize = new Vector3(selectedRoom.size.X + 7f, selectedRoom.size.Y + 7f, selectedRoom.size.Z + 7f);
                        List<TrasnparentSquare> selectedBlockList = selectedRoom.GetMapBlock(adjustedSize, Color.White, true);
                        selectedBlockList.Sort(new FaceSorter(-(WorldMap.cameraPosition - WorldMap.cameraTarget)));
                        for (int i = 0; i < selectedBlockList.Count; i++)
                        {

                            translucentList.Add(selectedBlockList[i].v1);
                            translucentList.Add(selectedBlockList[i].v2);
                            translucentList.Add(selectedBlockList[i].v3);
                            translucentList.Add(selectedBlockList[i].v4);
                            translucentList.Add(selectedBlockList[i].v5);
                            translucentList.Add(selectedBlockList[i].v6);
                        }*/
                    }

                    if (translucentList.Count > 0 && DepthControl.depthCount > 0 && mapShellObjects.Count * 2 > 0)
                    {                       
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            translucentList.ToArray(), 0, mapShellObjects.Count * 2, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                    if (WorldMap.selectedRoomIndex != -1)
                    {
                        Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
                        if (translucentList.Count > 0 && DepthControl.depthCount > 0 && translucentList.Count() / 3 - mapShellObjects.Count * 2 > 0)
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
            saveGameText.RenderTextures();
            DialogBox.RenderTextures();

            Engine.mapShellObjects = new List<TrasnparentSquare>();
            if(Engine.detailVertexArray == null)
                Engine.detailVertexArray = new VertexPositionColorNormalTexture[80000];
            detailVertexArrayCount = 0;
            if (Engine.doodadVertexArray == null)
                Engine.doodadVertexArray = new VertexPositionColorNormalTexture[150000];
            doodadVertexArrayCount = 0;
            if (Engine.decalVertexArray == null)
                Engine.decalVertexArray = new VertexPositionColorNormalTexture[60000];
            decalVertexArrayCount = 0;
            if (Engine.spriteVertexArray == null)
                Engine.spriteVertexArray = new VertexPositionColorNormalTexture[60000];
            spriteVertexArrayCount = 0;
            if (Engine.beamVertexArray == null)
                Engine.beamVertexArray = new VertexPositionColorNormalTexture[60000];
            beamVertexArrayCount = 0;
            if (Engine.translucentBlockVertexArray == null)
            {
                Engine.translucentBlockVertexArray = new VertexPositionColorNormalTexture[2000];
                translucentBlockVertexArrayCount = 0;
            }

            if (Engine.dynamicBlockVertexArray == null)
                Engine.dynamicBlockVertexArray = new VertexPositionColorNormalTexture[60000];
            dynamicBlockVertexArrayCount = 0;

            if (Engine.staticBlockVertexArray == null || reDraw == true)
            {
                Engine.staticTranslucentObjects = new List<TrasnparentSquare>();
                staticObjectsInitialized = false;
                staticDoodadsInitialized = false;

                if (Engine.staticBlockVertexArray == null)
                    Engine.staticBlockVertexArray = new VertexPositionColorNormalTexture[300000];
                staticBlockVertexArrayCount = 0;
            }

            //bloom.BeginDraw();            

            // Set transform matrices.
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            Engine.cameraPos = player.cameraPos;
            Engine.cameraUp = player.cameraUp;
            Engine.cameraTarget = player.cameraTarget;

            if (Engine.player.currentRoom.id.Contains("MenuRoom"))
            {
                Vector3 menuCameraTarget = cameraTarget;
                Vector3 menuCameraUp = cameraUp;
                Vector3 menuCameraPos = cameraPos;
                float shift = 0f;
                float dist = (Engine.player.center.position - Engine.player.currentRoom.center).Length();
                if (dist < 18f && Engine.player.center.normal == Vector3.UnitY)
                {
                    if (dist < 14f)
                    {
                        shift = 1f;
                        DepthControl.depthTrigger = false;
                    }
                    else
                    {
                        shift = (18 - dist) / 4f;
                        DepthControl.depthTrigger = true;
                    }
                    menuCameraTarget = Engine.player.currentRoom.center;
                    menuCameraPos = Engine.player.currentRoom.center + 20 * Vector3.UnitY;
                    menuCameraUp = Vector3.UnitZ;
                }
                dist = (Engine.player.center.position - (Engine.player.currentRoom.center - 5 * Vector3.UnitZ - 18 * Vector3.UnitX)).Length();
                if (dist < 10f && Engine.player.center.normal == -Vector3.UnitX)
                {
                    if (dist < 6)
                        shift = 1f;
                    else
                        shift = (10 - dist) / 4f;
                    menuCameraTarget = Engine.player.currentRoom.center + 1.5f * Vector3.UnitZ;
                    menuCameraPos = Engine.player.currentRoom.center - 33f * Vector3.UnitX + 1.5f * Vector3.UnitZ;
                    menuCameraUp = Vector3.UnitZ;
                }
                cameraPos = shift * menuCameraPos + (1 - shift) * cameraPos;
                cameraUp = shift * menuCameraUp + (1 - shift) * cameraUp;
                cameraTarget = shift * menuCameraTarget + (1 - shift) * cameraTarget;
            }
            else
            {
                DepthControl.depthTrigger = true;
            }
            playerCameraPos = cameraPos;
            playerCameraUp = cameraUp;
            playerCameraTarget = cameraTarget;
            if (state == EngineState.Map)
            {
                cameraPos = WorldMap.cameraPosition;
                cameraUp = WorldMap.cameraUp;
                cameraTarget = WorldMap.cameraTarget;
            }

            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);

            DepthControl.Update(gameTime);
            float cameraDistance = (cameraTarget - cameraPos).Length();
            projectionMatrix = ((1f - DepthControl.DepthFactor) * projectionMatrix + DepthControl.DepthFactor * Matrix.CreateOrthographic(cameraDistance * aspect, cameraDistance, 1, 2000)) / 2f;
            
            translucentEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            translucentEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp);
            translucentEffect.Projection = projectionMatrix;
            mapEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            mapEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp);
            mapEffect.Projection = projectionMatrix;


            playerTextureEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            playerTextureEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp);
            playerTextureEffect.Projection = projectionMatrix;

            worldTextureEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            worldTextureEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp);
            worldTextureEffect.Projection = projectionMatrix; 
            

            cartoonEffect.Parameters["World"].SetValue(Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch));
            cartoonEffect.Parameters["View"].SetValue(Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp));
            cartoonEffect.Parameters["Projection"].SetValue(projectionMatrix);


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

            DepthControl.Draw();

            Hud.Draw();

            
            if (dialogBox == null)
                dialogBox = new DialogBox();
            dialogBox.Draw();
            
            saveGameText.Draw();
            WorldMap.DrawMetaData();
            PauseMenu.Draw();
            if (Engine.player.currentRoom.id.Contains("Menu"))
                IntroOverlay.Draw();
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
            Engine.debug_doodadVertexUpdateMonitor.AddData(Engine.debug_updateDoodadVertexData);
            Engine.debug_doodadVertexUpdateMonitor.Update(gameTime);
            Engine.debug_updateDoodadVertexData = 0;

            if(Engine.player.currentRoom.id.Contains("Menu"))
                IntroOverlay.Update(gameTime);
            PauseMenu.Update(gameTime);
            if (PauseMenu.paused == true)
                return;
            WorldMap.Update(gameTime);
            // Profiling help
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                System.Threading.Thread.Sleep(1);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                return;

            optionToggleCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (optionToggleCooldown < 0) optionToggleCooldown = 0;

            if (GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftTrigger) || Keyboard.GetState().IsKeyDown(Keys.OemCloseBrackets))
            {
                player.baseCameraDistance += .03f * gameTime.ElapsedGameTime.Milliseconds;                
            }
            if (GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightTrigger) || Keyboard.GetState().IsKeyDown(Keys.OemOpenBrackets))
            {
                player.baseCameraDistance -= .03f * gameTime.ElapsedGameTime.Milliseconds;                
            }
            int currentScrollWheel = Mouse.GetState().ScrollWheelValue;
            //if (Mouse.GetState().RightButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                if (currentScrollWheel > Controls.scrollWheelPrev)
                {
                    player.baseCameraDistance -= .1f * gameTime.ElapsedGameTime.Milliseconds;
                }
                if (currentScrollWheel < Controls.scrollWheelPrev)
                {
                    player.baseCameraDistance += .1f * gameTime.ElapsedGameTime.Milliseconds;
                }
            }
            if (player.baseCameraDistance < 5f) player.baseCameraDistance = 5f;
            if (player.baseCameraDistance > 80f) player.baseCameraDistance = 80f;
            


            WorldMap.RotateWorldMap(gameTime);
            if (optionToggleCooldown == 0)
            {                
                int resultCooldown = WorldMap.ParseInput();
                if (resultCooldown != 0)
                    optionToggleCooldown = resultCooldown;
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
                player.currentRoom.UpdateDecorations(gameTime);

                Physics.CollisionCheck(player.currentRoom, player, gameTime);
            }


            if (dialogBox != null)
            {
                dialogBox.Update(gameTime);
                if (Engine.player.state == State.Dialog && Game1.controller.AButton.Pressed)
                {
                    if (false == dialogBox.Next())
                    {
                        Engine.player.jumpRecovery = 500;
                        Engine.player.state = State.Normal;
                    }
                }
            }
        }
    }
}

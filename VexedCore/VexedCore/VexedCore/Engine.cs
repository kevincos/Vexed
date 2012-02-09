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
        R_1024x768,
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
        public static List<Wormhole> wormholeList;
        
        public static Player player;
        public static DialogBox dialogBox;
        public static SpriteFont spriteFont;
        public static SpriteFont loadFont;
        public static SpriteFont loadFontBold;

        public SaveGameText saveGameText;

        public static bool soundEffectsEnabled = true;
        public static bool musicEnabled = true;
        public static bool justLoaded = false;

        
        public static SpriteBatch spriteBatch;
        public BasicEffect translucentEffect = null;
        public static BasicEffect mapEffect = null;
        public static BasicEffect loadMapEffect = null;
        public static AlphaTestEffect playerTextureEffect = null;
        public BasicEffect worldTextureEffect = null;
        public BasicEffect skyBoxEffect = null;
        public Effect cartoonEffect = null;
        public Effect postprocessEffect = null;


        public static bool transparencyEnabled = false;
        public static int lightingLevel = 0;
        public static bool toonShadingEnabled = false;
        public static int drawDepth = 2;
        public static ControlType controlType = ControlType.MouseAndKeyboard;
        public int optionToggleCooldown = 0;
        public static bool reDraw = false;
        public static bool detailTextures = true;
        public static bool fullScreen = false;
        public static int resWidth = 1920;
        public static int resHeight = 1280;
        public static ResolutionSettings res = ResolutionSettings.R_800x600;
        


        /*public static VertexPositionColorNormalTexture[] detailVertexArray;
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
        public static int translucentBlockVertexArrayCount = 0;*/
        

        public static List<TransparentSquare> staticTranslucentObjects;
        
        public static List<TransparentSquare> mapShellObjects;
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
            staticTranslucentObjects = new List<TransparentSquare>();
            mapShellObjects = new List<TransparentSquare>();
          
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

            loadMapEffect = new BasicEffect(Game1.graphicsDevice);
            loadMapEffect.VertexColorEnabled = true;
            loadMapEffect.Alpha = 1f;
            loadMapEffect.SpecularPower = 0.1f;
            loadMapEffect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            loadMapEffect.DiffuseColor = new Vector3(1, 1, 1);
            loadMapEffect.SpecularColor = new Vector3(1f, 1f, 1f);
            loadMapEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-10, -5, -1));
            loadMapEffect.DirectionalLight0.DiffuseColor = Color.Gray.ToVector3();
            loadMapEffect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();
            loadMapEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(10, 5, 1));
            loadMapEffect.DirectionalLight1.DiffuseColor = Color.Gray.ToVector3();
            loadMapEffect.DirectionalLight1.SpecularColor = Color.Black.ToVector3();
            loadMapEffect.LightingEnabled = true;
            loadMapEffect.DirectionalLight1.Enabled = true;
            loadMapEffect.DirectionalLight0.Enabled = true;

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

        public void DrawScene(int gameTime)
        {

            foreach (Room r in roomList)
            {
                r.adjacent = false;
            }
            Engine.player.currentRoom.MarkAdjacentRooms(Engine.drawDepth);
            if (Engine.player.jumpRoom != null)
                Engine.player.jumpRoom.MarkAdjacentRooms(Engine.drawDepth);


            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            
            worldTextureEffect.Texture = Room.blockTexture;
            worldTextureEffect.CurrentTechnique.Passes[0].Apply();


 


            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            List<DepthIndex> depthIndexer = new List<DepthIndex>();
            for(int i = 0; i < roomList.Count; i++)
            {
                if(roomList[i].shouldRender)
                    depthIndexer.Add(new DepthIndex(roomList[i].center, i, DepthIndexType.Room));                
            }
            
            for (int i = 0; i < wormholeList.Count; i++)
            {
                if(wormholeList[i].shouldRender)
                    depthIndexer.Add(new DepthIndex(wormholeList[i].position, i, DepthIndexType.Wormhole));
            }
            depthIndexer.Sort(new DepthIndexSorter(cameraTarget - cameraPos));
            for (int i = 0; i < depthIndexer.Count; i++)
            {
                if (depthIndexer[i].type == DepthIndexType.Room)
                {
                    roomList[depthIndexer[i].index].Draw();
                    roomList[depthIndexer[i].index].DrawDecorations();
                    if (roomList[depthIndexer[i].index] == player.currentRoom)
                    {
                        player.currentRoom.DrawMonsters();
                        player.currentRoom.DrawProjectiles();
                        player.DrawTexture(playerTextureEffect);
                    }

                }
                else if (depthIndexer[i].type == DepthIndexType.Wormhole)
                    wormholeList[depthIndexer[i].index].Draw();

            }



            /*foreach (Room r in roomList)
            {                    
                r.Draw();
            }*/
            foreach (Monster m in Engine.player.currentRoom.monsters)
            {
                if (m.moveType == VL.MovementType.FaceBoss)
                {
                    m.faceBoss.Render();
                }
            }


            /*foreach (Room r in Engine.roomList)
            {
                if(r != player.currentRoom)
                    r.DrawDecorations();
            }
            player.currentRoom.DrawDecorations();
            Engine.player.currentRoom.DrawMonsters();
            player.currentRoom.DrawProjectiles();*/


            
            /*foreach (Wormhole w in wormholeList)
            {
                if(w.shouldRender)
                    w.Draw();
            }*/

            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            //if (player.insideBox == true)
               // player.DrawTexture(playerTextureEffect);

            

            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            //Game1.graphicsDevice.BlendState = BlendState.Opaque;
            //if (player.insideBox == false)
              //  player.DrawTexture(playerTextureEffect);
            
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            if (WorldMap.state != ZoomState.None)
            {
                foreach (Room r in roomList)
                {
                    r.DrawMapIcons();                       
                }

                mapEffect.CurrentTechnique.Passes[0].Apply();
                                    
                // Sort Triangles
                if (WorldMap.selectedRoomIndex != -1)
                {
                    Room selectedRoom = roomList[WorldMap.selectedRoomIndex];
                    Vector3 adjustedSize = adjustedSize = new Vector3(selectedRoom.size.X + 7f, selectedRoom.size.Y + 7f, selectedRoom.size.Z + 7f);
                    List<TransparentSquare> selectedBlockList = selectedRoom.GetMapBlock(adjustedSize, Color.White, true);
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
                    Room selectedRoom = roomList[WorldMap.selectedRoomIndex];
                    Vector3 adjustedSize = adjustedSize = new Vector3(selectedRoom.size.X + 7f, selectedRoom.size.Y + 7f, selectedRoom.size.Z + 7f);
                    List<TransparentSquare> selectedBlockList = selectedRoom.GetMapBlock(adjustedSize, Color.White, true);
                    selectedBlockList.Sort(new FaceSorter(-(WorldMap.cameraPosition - WorldMap.cameraTarget)));
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
                    Engine.roomList[WorldMap.selectedRoomIndex].DrawMapIcons();
                }
                    
                Engine.player.currentRoom.DrawMapArrow();
                    
            }
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
        

        public void Draw(int gameTime)
        {
            saveGameText.RenderTextures();
            
            Engine.mapShellObjects = new List<TransparentSquare>();
       

            // Set transform matrices.
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            Engine.cameraPos = player.cameraPos;
            Engine.cameraUp = player.cameraUp;
            Engine.cameraTarget = player.cameraTarget;


            Hud.hidden = false;
            MapHud.hiddenFrame = true;
            MapHud.hiddenMap = true;
            
            if (WorldMap.state != ZoomState.None)
            {
                Hud.hidden = true;
                MapHud.hiddenFrame = false;
                MapHud.hiddenMap = false;
                
            }
            
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
                        Hud.hidden = true;
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
                    {
                        shift = 1f;
                        Hud.hidden = true;
                    }
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

            Matrix mapTranslation = Matrix.CreateTranslation(0, -WorldMap.zoomLevel * 5, 0);

            mapEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            mapEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp)*mapTranslation;
            mapEffect.Projection = projectionMatrix;

            playerTextureEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            playerTextureEffect.View = Matrix.CreateLookAt(cameraPos, cameraTarget, cameraUp) * mapTranslation;
            playerTextureEffect.Projection = projectionMatrix;

            


            // Set renderstates.
            Game1.graphicsDevice.RasterizerState = RasterizerState.CullNone;
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
            



            Game1.graphicsDevice.SetRenderTarget(null);

            Game1.graphicsDevice.Clear(Color.Black);


            Skybox.Draw(player);
            DrawScene(gameTime);
         
            
            Engine.staticObjectsInitialized = true;
            Engine.staticDoodadsInitialized = true;
            Engine.reDraw = false;

            DepthControl.Draw();

            Hud.Draw();
            MapHud.Draw();

            
            if (dialogBox == null)
                dialogBox = new DialogBox();
            dialogBox.Draw();
            
            saveGameText.Draw();
            WorldMap.DrawMetaData();
            PauseMenu.Draw();
            if (Engine.player.currentRoom.id.Contains("Menu"))
                IntroOverlay.Draw();
        }




        public void Update(int gameTime)
        {


            Engine.debug_doodadVertexUpdateMonitor.AddData(Engine.debug_updateDoodadVertexData);
            Engine.debug_doodadVertexUpdateMonitor.Update(gameTime);
            Engine.debug_updateDoodadVertexData = 0;

            if(Engine.player.currentRoom.id.Contains("Menu"))
                IntroOverlay.Update(gameTime);
            PauseMenu.Update(gameTime);
            Hud.Update(gameTime);
            MapHud.Update(gameTime);
            if (PauseMenu.paused == true)
                return;
            WorldMap.Update(gameTime);
            // Profiling help
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                System.Threading.Thread.Sleep(1);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                return;

            if (wormholeList == null || justLoaded == true)
            {
                wormholeList = new List<Wormhole>();
                foreach (Room r in roomList)
                    r.BuildWormholes();
            }

            foreach (Wormhole w in wormholeList)
            {
                w.Update(gameTime);
            }

            if (player.state == State.Normal)
                justLoaded = false;

            

            optionToggleCooldown -= gameTime;
            if (optionToggleCooldown < 0) optionToggleCooldown = 0;

            if (GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftTrigger) || Keyboard.GetState().IsKeyDown(Keys.OemCloseBrackets))
            {
                player.baseCameraDistance += .03f * gameTime;                
            }
            if (GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightTrigger) || Keyboard.GetState().IsKeyDown(Keys.OemOpenBrackets))
            {
                player.baseCameraDistance -= .03f * gameTime;                
            }
            int currentScrollWheel = Mouse.GetState().ScrollWheelValue;
            //if (Mouse.GetState().RightButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                if (currentScrollWheel > Controls.scrollWheelPrev)
                {
                    player.baseCameraDistance -= .1f * gameTime;
                }
                if (currentScrollWheel < Controls.scrollWheelPrev)
                {
                    player.baseCameraDistance += .1f * gameTime;
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

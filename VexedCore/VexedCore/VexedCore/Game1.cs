using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VexedCore
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static List<Room> roomList;
        public static Player player;

        public GraphicsDeviceManager graphics;
        public static GraphicsDevice graphicsDevice;
        public static BloomComponent bloom;
        SpriteBatch spriteBatch;
        public BasicEffect effect = null;
        public BasicEffect translucentEffect = null;
        public AlphaTestEffect playerTextureEffect = null;
        public BasicEffect skyBoxEffect = null;
        public Effect cartoonEffect = null;
        Effect postprocessEffect = null;

        Vector3 currentTarget = Vector3.Zero;
        Vector3 currentCamera = new Vector3(30, 30, 30);
        Vector3 currentUp = new Vector3(0, 0, 1);
        float currentRotate = 0;
        float currentPitch = 0;
        public static float controlStickTrigger = .25f;
        public static PlayerIndex activePlayer = PlayerIndex.One;

        public static List<VertexPositionColorNormalTexture> staticOpaqueObjects;
        public static List<VertexPositionColorNormalTexture> dynamicOpaqueObjects;
        public static List<VertexPositionColorNormalTexture> texturedObjects;
        public static List<TrasnparentSquare> staticTranslucentObjects;
        public static bool staticObjectsInitialized = false;

        RenderTarget2D sceneRenderTarget;
        RenderTarget2D normalDepthRenderTarget;
        NonPhotoRealisticSettings Settings
        {
            get { return NonPhotoRealisticSettings.PresetSettings[settingsIndex]; }
        }
        int settingsIndex = 5;

        public static bool detailTextures = true;

        public Game1()
        {
            staticOpaqueObjects = new List<VertexPositionColorNormalTexture>();
            dynamicOpaqueObjects = new List<VertexPositionColorNormalTexture>();
            texturedObjects = new List<VertexPositionColorNormalTexture>();
            staticTranslucentObjects = new List<TrasnparentSquare>();

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;

#if XBOX
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
#endif
                //graphics.IsFullScreen = true;
                //graphics.PreferredBackBufferWidth = 1920;
                //graphics.PreferredBackBufferHeight = 1080;

                


            //bloom = new BloomComponent(this);
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphicsDevice = GraphicsDevice;            

            LevelLoader.Load("LevelData\\spikeelevator");
            //LevelLoader.Load("LevelData\\spiral2");
            
            //LevelLoader.Load("LevelData\\movingplatform");
            //LevelLoader.Load("LevelData\\awesome");
            //LevelLoader.Load("LevelData\\debug");
            
            Components.Add(new FrameRateCounter(this));
            //Components.Add(bloom);
            //bloom.Settings = BloomSettings.PresetSettings[0];
            
            base.Initialize();
            
            effect = new BasicEffect(Game1.graphicsDevice);
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = true;
            effect.Texture = Room.blockTexture;
            effect.LightingEnabled = true;
            effect.Alpha = 1f;
            effect.SpecularPower = 0.1f;
            effect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            effect.DiffuseColor = new Vector3(1, 1, 1);
            effect.SpecularColor = new Vector3(0, 1f, 1f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-10, -5, -1));
            effect.DirectionalLight0.DiffuseColor = Color.Gray.ToVector3();
            effect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();

            translucentEffect = new BasicEffect(Game1.graphicsDevice);
            translucentEffect.VertexColorEnabled = true;
            translucentEffect.LightingEnabled = true;
            translucentEffect.Alpha = 1f;
            translucentEffect.SpecularPower = 0.1f;
            translucentEffect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            translucentEffect.DiffuseColor = new Vector3(1, 1, 1);
            translucentEffect.SpecularColor = new Vector3(0, 1f, 1f);
            translucentEffect.DirectionalLight0.Enabled = true;
            translucentEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-10, -5, -1));
            translucentEffect.DirectionalLight0.DiffuseColor = Color.Gray.ToVector3();
            translucentEffect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();

            playerTextureEffect = new AlphaTestEffect(Game1.graphicsDevice);
            playerTextureEffect.VertexColorEnabled = true;
            playerTextureEffect.Alpha = 1f;
            
            
            Skybox.Init();

            Monster.InitTexCoords();
            Projectile.InitTexCoords();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cartoonEffect = Content.Load<Effect>("CartoonEffect");
            postprocessEffect = Content.Load<Effect>("PostprocessEffect");
            Room.blockTexture = Content.Load<Texture2D>("plate_texture");
            Player.neutralTexture = Content.Load<Texture2D>("p_neutral");
            Player.fallTexture = Content.Load<Texture2D>("p_fall");
            Player.wallJumpTexture = Content.Load<Texture2D>("p_walljump");
            Player.runTexture2 = Content.Load<Texture2D>("p_run2");
            Player.runTexture1 = Content.Load<Texture2D>("p_run1");
            Player.runTexture3 = Content.Load<Texture2D>("p_run3");
            Player.runTexture4 = Content.Load<Texture2D>("p_run4");
            Player.neutralTexture_detail = Content.Load<Texture2D>("p_neutral_detail");
            Player.fallTexture_detail = Content.Load<Texture2D>("p_fall_detail");
            Player.wallJumpTexture_detail = Content.Load<Texture2D>("p_walljump_detail");
            Player.runTexture2_detail = Content.Load<Texture2D>("p_run2_detail");
            Player.runTexture1_detail = Content.Load<Texture2D>("p_run1_detail");
            Player.runTexture3_detail = Content.Load<Texture2D>("p_run3_detail");
            Player.runTexture4_detail = Content.Load<Texture2D>("p_run4_detail");
            Monster.monsterTexture = Content.Load<Texture2D>("m_body");
            Monster.monsterTextureDetail = Content.Load<Texture2D>("m_body_detail");
            Projectile.projectileTexture = Content.Load<Texture2D>("projectiles");
            Skybox.skyBoxTextures = new Texture2D[6];
            Skybox.skyBoxTextures[0] = Content.Load<Texture2D>("skybox_right");
            Skybox.skyBoxTextures[1] = Content.Load<Texture2D>("skybox_left");
            Skybox.skyBoxTextures[2] = Content.Load<Texture2D>("skybox_front");
            Skybox.skyBoxTextures[3] = Content.Load<Texture2D>("skybox_back");
            Skybox.skyBoxTextures[4] = Content.Load<Texture2D>("skybox_bottom");
            Skybox.skyBoxTextures[5] = Content.Load<Texture2D>("skybox_top");

            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;

            sceneRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);

            normalDepthRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                         pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (sceneRenderTarget != null)
            {
                sceneRenderTarget.Dispose();
                sceneRenderTarget = null;
            }

            if (normalDepthRenderTarget != null)
            {
                normalDepthRenderTarget.Dispose();
                normalDepthRenderTarget = null;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Profiling help
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                System.Threading.Thread.Sleep(1);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                return;

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                settingsIndex = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                settingsIndex = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                settingsIndex = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                settingsIndex = 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D5))
            {
                settingsIndex = 4;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D6))
            {
                settingsIndex = 5;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D7))
            {
                detailTextures = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D8))
            {
                detailTextures = true;
            }

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(activePlayer).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            
            

            // TODO: Add your update logic here
            player.Update(gameTime);
            
            foreach (Room r in roomList)
                r.Update(gameTime);

            player.currentRoom.UpdateMonsters(gameTime);
            

            Physics.CollisionCheck(player.currentRoom, player, gameTime);
            

            base.Update(gameTime);
        }


        public void DrawScene()
        {
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            effect.CurrentTechnique.Passes[0].Apply();
            if (staticOpaqueObjects.Count > 0)
            {
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    Game1.staticOpaqueObjects.ToArray(), 0, staticOpaqueObjects.Count / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            if (dynamicOpaqueObjects.Count > 0)
            {
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    Game1.dynamicOpaqueObjects.ToArray(), 0, dynamicOpaqueObjects.Count / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            if(detailTextures)
                playerTextureEffect.Texture = Monster.monsterTextureDetail;
            else
                playerTextureEffect.Texture = Monster.monsterTexture;
            playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            Game1.player.currentRoom.DrawMonsters();
            playerTextureEffect.Texture = Projectile.projectileTexture;
            playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            Game1.player.currentRoom.DrawProjectiles();

            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            if (Room.innerBlockMode > 0)
            {
                translucentEffect.CurrentTechnique.Passes[0].Apply();
                // Sort Triangles
                staticTranslucentObjects.Sort(new FaceSorter(player.cameraTarget - player.cameraPos));

                List<VertexPositionColorNormal> translucentList = new List<VertexPositionColorNormal>();
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
                        translucentList.ToArray(), 0, translucentList.Count / 3, VertexPositionColorNormal.VertexDeclaration);
                }
            }

            playerTextureEffect.Texture = player.currentTexture;
            playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            player.DrawTexture();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Game1.staticTranslucentObjects = new List<TrasnparentSquare>();
            Game1.dynamicOpaqueObjects = new List<VertexPositionColorNormalTexture>();
            if(Game1.staticOpaqueObjects == null)
                Game1.staticOpaqueObjects = new List<VertexPositionColorNormalTexture>();
            Game1.texturedObjects = new List<VertexPositionColorNormalTexture>();

            //bloom.BeginDraw();            
            
            //Game1.graphicsDevice.Clear(Color.Black);
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;   

            // Set transform matrices.
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            effect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            effect.View = Matrix.CreateLookAt(player.cameraPos, player.cameraTarget, player.cameraUp);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);


            translucentEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            translucentEffect.View = Matrix.CreateLookAt(player.cameraPos, player.cameraTarget, player.cameraUp);
            translucentEffect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);
            

            playerTextureEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            playerTextureEffect.View = Matrix.CreateLookAt(player.cameraPos, player.cameraTarget, player.cameraUp);
            playerTextureEffect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);
            

            
            // Set renderstates.
            Game1.graphicsDevice.RasterizerState = RasterizerState.CullNone;
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (Room r in roomList)
            {
                r.Draw(gameTime);
            }



            if (Settings.EnableEdgeDetect)
            {
                Game1.graphicsDevice.SetRenderTarget(normalDepthRenderTarget);

                Game1.graphicsDevice.Clear(Color.Black);


                DrawScene();
            }

            


            if (Settings.EnableEdgeDetect || Settings.EnableSketch)
                Game1.graphicsDevice.SetRenderTarget(sceneRenderTarget);
            else
                Game1.graphicsDevice.SetRenderTarget(null);

            Game1.graphicsDevice.Clear(Color.White);


            Skybox.Draw(player);
            DrawScene();



            // Run the postprocessing filter over the scene that we just rendered.
            if (Settings.EnableEdgeDetect || Settings.EnableSketch)
            {
                Game1.graphicsDevice.SetRenderTarget(null);

                ApplyPostprocess();
            }
            
            base.Draw(gameTime);

            Game1.staticObjectsInitialized = true;
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
    }
}

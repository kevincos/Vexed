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
        public BasicEffect playerTextureEffect = null;
        public BasicEffect skyBoxEffect = null;

        Vector3 currentTarget = Vector3.Zero;
        Vector3 currentCamera = new Vector3(30, 30, 30);
        Vector3 currentUp = new Vector3(0, 0, 1);
        float currentRotate = 0;
        float currentPitch = 0;
        public static float controlStickTrigger = .25f;
        public static PlayerIndex activePlayer = PlayerIndex.One;

        public static List<VertexPositionColorNormal> staticOpaqueObjects;
        public static List<VertexPositionColorNormal> dynamicOpaqueObjects;
        public static List<VertexPositionColorNormalTexture> texturedObjects;
        public static List<TrasnparentSquare> staticTranslucentObjects;
        public static bool staticObjectsInitialized = false;

        public Game1()
        {
            staticOpaqueObjects = new List<VertexPositionColorNormal>();
            dynamicOpaqueObjects = new List<VertexPositionColorNormal>();
            texturedObjects = new List<VertexPositionColorNormalTexture>();
            staticTranslucentObjects = new List<TrasnparentSquare>();

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;            
            
            bloom = new BloomComponent(this);
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
            Components.Add(bloom);
            bloom.Settings = BloomSettings.PresetSettings[0];
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Player.neutralTexture = Content.Load<Texture2D>("p_neutral");
            Player.fallTexture = Content.Load<Texture2D>("p_fall");
            Player.wallJumpTexture = Content.Load<Texture2D>("p_walljump");
            Player.runTexture2 = Content.Load<Texture2D>("p_run2");
            Player.runTexture1 = Content.Load<Texture2D>("p_run1");
            Player.runTexture3 = Content.Load<Texture2D>("p_run3");
            Player.runTexture4 = Content.Load<Texture2D>("p_run4");
            Skybox.skyBoxTextures = new Texture2D[6];
            Skybox.skyBoxTextures[0] = Content.Load<Texture2D>("skybox_right");
            Skybox.skyBoxTextures[1] = Content.Load<Texture2D>("skybox_left");
            Skybox.skyBoxTextures[2] = Content.Load<Texture2D>("skybox_front");
            Skybox.skyBoxTextures[3] = Content.Load<Texture2D>("skybox_back");
            Skybox.skyBoxTextures[4] = Content.Load<Texture2D>("skybox_bottom");
            Skybox.skyBoxTextures[5] = Content.Load<Texture2D>("skybox_top");
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(activePlayer).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                Room.innerBlockMode = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                Room.innerBlockMode = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                Room.innerBlockMode = 2;
            }

            
            player.Update(gameTime);
            

            // TODO: Add your update logic here
            foreach(Room r in roomList)
                r.Update(gameTime);

            Physics.CollisionCheck(player.currentRoom, player, gameTime);
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Game1.staticTranslucentObjects = new List<TrasnparentSquare>();
            Game1.dynamicOpaqueObjects = new List<VertexPositionColorNormal>();
            if(Game1.staticOpaqueObjects == null)
                Game1.staticOpaqueObjects = new List<VertexPositionColorNormal>();
            Game1.texturedObjects = new List<VertexPositionColorNormalTexture>();

            bloom.BeginDraw();            
            
            Game1.graphicsDevice.Clear(Color.Black);
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            if (effect == null)
            {
                effect = new BasicEffect(Game1.graphicsDevice);
                effect.VertexColorEnabled = true;
                playerTextureEffect = new BasicEffect(Game1.graphicsDevice);
                playerTextureEffect.VertexColorEnabled = true;
                Skybox.Init();
            }

            // Set transform matrices.
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            effect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            effect.View = Matrix.CreateLookAt(player.cameraPos, player.cameraTarget, player.cameraUp);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);
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

            playerTextureEffect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);
            playerTextureEffect.View = Matrix.CreateLookAt(player.cameraPos, player.cameraTarget, player.cameraUp);
            playerTextureEffect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);
            playerTextureEffect.LightingEnabled = true;
            playerTextureEffect.Alpha = 1f;
            playerTextureEffect.SpecularPower = 0.1f;
            playerTextureEffect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            playerTextureEffect.DiffuseColor = new Vector3(1, 1, 1);
            playerTextureEffect.SpecularColor = new Vector3(0, 1f, 1f);
            playerTextureEffect.DirectionalLight0.Enabled = true;            
            playerTextureEffect.DirectionalLight0.Direction = Vector3.Normalize(player.cameraTarget - player.cameraPos);            
            playerTextureEffect.DirectionalLight0.DiffuseColor = Color.Gray.ToVector3();
            playerTextureEffect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();
            playerTextureEffect.TextureEnabled = true;
            playerTextureEffect.Texture = player.currentTexture;

            
            
            // Set renderstates.
            Game1.graphicsDevice.RasterizerState = RasterizerState.CullNone;
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
            Game1.graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            Skybox.Draw(player);

            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (Room r in roomList)
            {
                r.Draw(gameTime);
            }
            //player.Draw(gameTime);
            //Physics.DebugDraw(player.currentRoom, player.center.normal, player.center.direction);            

            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                Game1.staticOpaqueObjects.ToArray(), 0, staticOpaqueObjects.Count / 3, VertexPositionColorNormal.VertexDeclaration);


            if (Room.innerBlockMode > 0)
            {
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
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    translucentList.ToArray(), 0, translucentList.Count / 3, VertexPositionColorNormal.VertexDeclaration);
            }

            

            playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            player.DrawTexture(gameTime);

                  

            base.Draw(gameTime);

            Game1.staticObjectsInitialized = true;
        }
    }
}

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

        Vector3 currentTarget = Vector3.Zero;
        Vector3 currentCamera = new Vector3(30, 30, 30);
        Vector3 currentUp = new Vector3(0, 0, 1);
        float currentRotate = 0;
        float currentPitch = 0;
        public static float controlStickTrigger = .25f;
        public static PlayerIndex activePlayer = PlayerIndex.One;


        public Game1()
        {
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
            //Components.Add(bloom);
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
            //bloom.BeginDraw();            
            
            Game1.graphicsDevice.Clear(Color.Black);
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            if (effect == null)
            {
                effect = new BasicEffect(Game1.graphicsDevice);
                effect.VertexColorEnabled = true;
            }

            // Set transform matrices.
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            //effect.World = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            effect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);

            /*effect.View = Matrix.CreateLookAt(currentCamera,
                                              currentTarget,
                                              currentUp);*/
            effect.View = Matrix.CreateLookAt(player.cameraPos, player.cameraTarget, player.cameraUp);

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);
            effect.LightingEnabled = true;
            effect.Alpha = 1f;
            effect.SpecularPower = 0.1f;
            effect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            effect.DiffuseColor = new Vector3(1, 1, 1);
            effect.SpecularColor = new Vector3(0, 1f, 1f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-5, -3, -1));
            effect.DirectionalLight0.DiffuseColor = Color.Gray.ToVector3();
            effect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();


            // Set renderstates.
            Game1.graphicsDevice.RasterizerState = RasterizerState.CullNone;

            // Draw the triangle.
            effect.CurrentTechnique.Passes[0].Apply();

            foreach (Room r in roomList)
            {
                r.Draw(gameTime);
            }
            player.Draw(gameTime);
            //physics.DebugDraw(player.currentRoom, player.center.normal, player.center.direction);

            base.Draw(gameTime);
        }
    }
}

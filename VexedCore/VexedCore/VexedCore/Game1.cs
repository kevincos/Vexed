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

        public GraphicsDeviceManager graphics;
        public static GraphicsDevice graphicsDevice;
        public static BloomComponent bloom;

        public static float controlStickTrigger = .25f;
        public static PlayerIndex activePlayer = PlayerIndex.One;

        public static Rectangle titleSafeRect;
        public static Engine engine;
        public static Controls controller;

  

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;

#if XBOX
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
#endif
            int resWidth = 800;
            int resHeight = 600;
            bool fullScreen = true;
            //resWidth = 1920;
            //resHeight = 1080;
            //fullScreen = true;
            graphics.IsFullScreen = fullScreen;
            graphics.PreferredBackBufferWidth = resWidth;
            graphics.PreferredBackBufferHeight = resHeight;
            Window.BeginScreenDeviceChange(fullScreen);
            Window.EndScreenDeviceChange(Window.ScreenDeviceName, resWidth, resHeight);
            graphics.ApplyChanges();

            titleSafeRect = graphics.GraphicsDevice.Viewport.TitleSafeArea;

            engine = new Engine();
            controller = new Controls(activePlayer);
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

            engine.Init();
            Skybox.Init();
            AnimationControl.Init();

            Monster.InitTexCoords();
            Player.InitTexCoords();
            Projectile.InitTexCoords();
            Ability.InitTexCoords();
            Room.InitTexCoords();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Engine.spriteBatch = new SpriteBatch(GraphicsDevice);
            engine.cartoonEffect = Content.Load<Effect>("CartoonEffect");
            engine.postprocessEffect = Content.Load<Effect>("PostprocessEffect");
            Room.blockTexture = Content.Load<Texture2D>("plate_texture");
            Player.player_textures = Content.Load<Texture2D>("p_texture");
            Player.player_gun_textures = Content.Load<Texture2D>("p_gun_texture");
            Player.player_boots_textures = Content.Load<Texture2D>("p_texture_boots");
            Player.player_jetpack_textures = Content.Load<Texture2D>("p_texture_jetpack");
            Player.player_booster_textures = Content.Load<Texture2D>("p_texture_booster");
            Player.player_textures_detail = Content.Load<Texture2D>("p_texture");
            Player.player_textures_clean = Content.Load<Texture2D>("p_texture_clean");
            Ability.ability_textures = Content.Load<Texture2D>("abilities");
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

            Engine.sceneRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);

            Engine.normalDepthRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
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
            if (Engine.sceneRenderTarget != null)
            {
                Engine.sceneRenderTarget.Dispose();
                Engine.sceneRenderTarget = null;
            }

            if (Engine.normalDepthRenderTarget != null)
            {
                Engine.normalDepthRenderTarget.Dispose();
                Engine.normalDepthRenderTarget = null;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (GamePad.GetState(activePlayer).IsButtonDown(Buttons.Back))
                this.Exit();

            AnimationControl.Update(gameTime);
            controller.Update(gameTime);
            engine.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            engine.Draw(gameTime);

            base.Draw(gameTime);
        }
        
    }
}

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

        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice graphicsDevice;
        public static BloomComponent bloom;

        public static float controlStickTrigger = .25f;
        public static PlayerIndex activePlayer = PlayerIndex.One;

        public static Rectangle titleSafeRect;
        public static Engine engine;
        public static Controls controller;

        public static bool resetTimer = false;

        public static ContentManager contentManager;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;

            Engine.res = ResolutionSettings.R_800x600;
            Engine.fullScreen = false;
            
#if XBOX
            Engine.res = ResolutionSettings.R_1280x720;
#endif

            Engine.res = ResolutionSettings.R_800x600; 
            Engine.fullScreen = false;

            SetGraphicsSettings();

            titleSafeRect = graphics.GraphicsDevice.Viewport.TitleSafeArea;

            engine = new Engine();
            controller = new Controls(activePlayer);
            //bloom = new BloomComponent(this);
            Content.RootDirectory = "Content";

            contentManager = Content;
            
        }

        public static void SetGraphicsSettings()
        {
            if (Engine.res == ResolutionSettings.R_800x600)
            {
                Engine.resWidth = 800;
                Engine.resHeight = 600;
            }
            if (Engine.res == ResolutionSettings.R_1024x768)
            {
                Engine.resWidth = 1024;
                Engine.resHeight = 768;
            }
            if (Engine.res == ResolutionSettings.R_1920x1080)
            {
                Engine.resWidth = 1920;
                Engine.resHeight = 1080;
            }
            if (Engine.res == ResolutionSettings.R_1280x720)
            {
                Engine.resWidth = 1280;
                Engine.resHeight = 720;
            }
            if (Engine.res == ResolutionSettings.R_400x300)
            {
                Engine.resWidth = 400;
                Engine.resHeight = 300;
            }

            graphics.IsFullScreen = Engine.fullScreen;
            graphics.PreferredBackBufferWidth = Engine.resWidth;
            graphics.PreferredBackBufferHeight = Engine.resHeight;
            ///Window.BeginScreenDeviceChange(Engine.fullScreen);
            //Window.EndScreenDeviceChange(Window.ScreenDeviceName, Engine.resWidth, Engine.resHeight);
            graphics.ApplyChanges();

            titleSafeRect = graphics.GraphicsDevice.Viewport.TitleSafeArea;
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

            //LevelLoader.Load("LevelData\\menu");
            
            
            Components.Add(new FrameRateCounter(this));
            //Components.Add(bloom);
            //bloom.Settings = BloomSettings.PresetSettings[0];
            
            base.Initialize();

            engine.Init();
            Skybox.Init();
            AnimationControl.Init();
            

            Monster.InitTexCoords();
            Player.InitTexCoords();            
            Ability.InitTexCoords();
            Room.InitTexCoords();
            Doodad.InitTexCoords();
            DialogBox.InitTexCoords();

            MusicControl.PlayGameMusic();
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
            DialogBox.box = Content.Load<Texture2D>("dialogbox");
            DialogBox.portraits = Content.Load<Texture2D>("dialogportraits");
            Room.blockTexture = Content.Load<Texture2D>("plate_texture");
            Player.player_textures = Content.Load<Texture2D>("p_texture");
            Player.player_gun_textures = Content.Load<Texture2D>("p_gun_texture");
            Player.player_boots_textures = Content.Load<Texture2D>("p_texture_boots");
            Player.player_jetpack_textures = Content.Load<Texture2D>("p_texture_jetpack");
            Player.player_booster_textures = Content.Load<Texture2D>("p_texture_booster");
            Player.player_textures_detail = Content.Load<Texture2D>("p_texture");
            Player.player_textures_clean = Content.Load<Texture2D>("p_texture_clean");
            Ability.ability_textures = Content.Load<Texture2D>("abilities");
            PauseMenu.pauseBackground = Content.Load<Texture2D>("pausebackground");
            PauseMenu.mouseCursor = Content.Load<Texture2D>("mouse");
            PauseMenu.mouseAndKeyboardHelp = Content.Load<Texture2D>("mouseandkeyboard");
            PauseMenu.gamePadHelp = Content.Load<Texture2D>("gamepadhelp");
            PauseMenu.keyboardOnlyHelp = Content.Load<Texture2D>("keyboardonlyhelp");
            Decoration.defaultTexture = Content.Load<Texture2D>("abilities");
            //Doodad.beam_textures = Content.Load<Texture2D>("beams");
            Monster.monsterTexture = Content.Load<Texture2D>("m_body");
            Monster.monsterTextureDetail = Content.Load<Texture2D>("m_body_detail");
            WorldMap.changeArrow = Content.Load<Texture2D>("screenchangearrow");
            Projectile.blastTexture = Content.Load<Texture2D>("Projectiles\\blast");
            Projectile.plasmaTexture = Content.Load<Texture2D>("Projectiles\\plasma");
            Projectile.laserTexture = Content.Load<Texture2D>("Projectiles\\laser");
            Projectile.missileTexture = Content.Load<Texture2D>("Projectiles\\missile");
            Monster.Init(Content);
            Block.wallTexture = Content.Load<Texture2D>("wall_texture");
            Block.circuitTexture = Content.Load<Texture2D>("circuit_wall");
            Block.crackedTexture = Content.Load<Texture2D>("cracked_wall");
            Block.vineTexture = Content.Load<Texture2D>("vine_wall");
            Block.cobblestoneTexture = Content.Load<Texture2D>("cobblestone_wall");
            Block.fancyPlateTexture = Content.Load<Texture2D>("fancyPlate_wall");
            Block.cargoTexture = Content.Load<Texture2D>("cargo_wall");
            Block.crateTexture = Content.Load<Texture2D>("crate_wall");
            Block.iceTexture = Content.Load<Texture2D>("ice_wall");
            Block.crystalTexture = Content.Load<Texture2D>("crystal_wall");
            Block.gearslotTexture = Content.Load<Texture2D>("gearslot_wall");
            Edge.iceEdge = Content.Load<Texture2D>("ice_edge");
            Edge.spikeEdge = Content.Load<Texture2D>("spike_edge");
            Edge.magnetEdge = Content.Load<Texture2D>("magnet_edge");
            Edge.lavaCoolEdge = Content.Load<Texture2D>("lavacool_edge");
            Edge.lavaHotEdge = Content.Load<Texture2D>("lavahot_edge");
            Skybox.LoadTextures(Content);
            DecorationImage.LoadTextures(Content);
            Engine.spriteFont = Content.Load<SpriteFont>("Font");
            Engine.loadFont = Content.Load<SpriteFont>("LoadFont");
            Engine.loadFontBold = Content.Load<SpriteFont>("LoadFontBold");
            Doodad.InitBeamTextures(Content);
            SaveGameText.confirmOptions = Content.Load<Texture2D>("icon_confirm");
            SaveGameText.loadOptions = Content.Load<Texture2D>("icon_load");
            SaveGameText.InitTexCoords();
            Doodad.InitDecalTextures(Content);
            Doodad.useButton = Content.Load<Texture2D>("decal_ekey");
            Doodad.leftButton = Content.Load<Texture2D>("decal_leftbutton");
            Doodad.rightButton = Content.Load<Texture2D>("decal_rightbutton");
            
            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;

            Engine.sceneRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);

            Engine.normalDepthRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                         pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);

            SoundFX.Init(Content);
            Mouse.SetPosition(Game1.titleSafeRect.Center.X, Game1.titleSafeRect.Center.Y);
            
            MusicControl.music_menu = Content.Load<Song>("Sounds\\music_menu");
            MusicControl.music_game = Content.Load<Song>("Sounds\\music_game");
            MusicControl.greenSector = Content.Load<Song>("Sounds\\GreenMusic");
            MusicControl.hubSector = Content.Load<Song>("Sounds\\HubMusic");
            MusicControl.storageSector = Content.Load<Song>("Sounds\\StorageMusic");
            MusicControl.engineSector = Content.Load<Song>("Sounds\\EngineMusic");
            MusicControl.coreSector = Content.Load<Song>("Sounds\\CoreMusic");

            MusicControl.boss = Content.Load<Song>("Sounds\\BossMusic");
            MusicControl.finalBoss = Content.Load<Song>("Sounds\\FinalBoss");
            MusicControl.firstLoad = Content.Load<Song>("Sounds\\FirstLoad");
            MusicControl.itemRoom = Content.Load<Song>("Sounds\\ItemRoomPreUpgrade");
            MusicControl.upgrade = Content.Load<Song>("Sounds\\UpgradeMusic");
            MusicControl.intro = Content.Load<Song>("Sounds\\IntroMusic");
            MusicControl.death = Content.Load<Song>("Sounds\\DeathMusic");


            LevelLoader.Load("LevelData\\world");
            LevelLoader.QuickSave(true);
            
            //LevelLoader.Load("LevelData\\spikeelevator");
            LevelLoader.Load("LevelData\\menu");
            
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
            if (Engine.quit == true)
                this.Exit();
            

            int elapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            if (resetTimer)
            {
                resetTimer = false;
                elapsedTime = 0;
            }

            AnimationControl.Update(elapsedTime);
            controller.Update(elapsedTime);
            engine.Update(elapsedTime);
            SaveGameText.Update(elapsedTime);
            ObjectiveControl.UpdateObjectiveStatus(elapsedTime);
            Controls.ResetMouse();
            MusicControl.PlayGameMusic();
            base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            int elapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            engine.Draw(elapsedTime);

            base.Draw(gameTime);
        }
        
    }
}

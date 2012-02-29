using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Runtime.InteropServices;


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
        public static GameWindow gameWindow;

        public static bool resetTimer = false;

        public static ContentManager contentManager;

        

        public Game1()
        {

            

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;

            PauseMenu.resolutionOptions = new List<Vector2>();

            int i = 0;
            Engine.resIndex = 0;
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {                
                // Use display mode as required...
                PauseMenu.resolutionOptions.Add(new Vector2(mode.Width, mode.Height));
                if (mode.Width == GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width && mode.Height ==
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
                {
                    Engine.resIndex = i;
                }
                i++;
                
            }
            

            try
            {                
                Stream stream = new FileStream("settings", FileMode.Open, FileAccess.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                Settings s = (Settings)serializer.Deserialize(stream);
                stream.Close();
                Engine.fullScreen = s.fullscreen;
                Engine.resIndex = s.resIndex;
                Engine.soundEffectsEnabled = s.soundeffects;
                Engine.musicEnabled = s.music;
                Engine.drawDepth = s.drawDistance;
                if (Engine.resIndex >= PauseMenu.resolutionOptions.Count) Engine.resIndex = 0;
            }
            catch (Exception) { }


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
            Engine.resWidth = (int)(PauseMenu.resolutionOptions[Engine.resIndex].X);
            Engine.resHeight = (int)(PauseMenu.resolutionOptions[Engine.resIndex].Y);

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
            DialogBox.box = Content.Load<Texture2D>("UI\\dialogbox");
            DialogBox.portraits = Content.Load<Texture2D>("UI\\dialogportraits");
            Player.player_textures = Content.Load<Texture2D>("Player\\p_texture");
            Player.player_gun_textures = Content.Load<Texture2D>("Player\\p_gun_texture");
            Player.player_boots_textures = Content.Load<Texture2D>("Player\\p_texture_boots");
            Player.player_jetpack_textures = Content.Load<Texture2D>("Player\\p_texture_jetpack");
            Player.player_booster_textures = Content.Load<Texture2D>("Player\\p_texture_booster");
            Player.player_doublejump = Content.Load<Texture2D>("Player\\p_doublejump");
            Player.player_boosterthrust = Content.Load<Texture2D>("Player\\p_boosterthrust");
            Player.player_jetpackthrust = Content.Load<Texture2D>("Player\\p_jetpackthrust");
            Player.player_spinhook = Content.Load<Texture2D>("Player\\p_spinhook");
            Player.player_spinlink = Content.Load<Texture2D>("Player\\p_spinlink");
            //Ability.ability_textures = Content.Load<Texture2D>("abilities");
            Wormhole.wormholeTexture = Content.Load<Texture2D>("Decorations\\wormhole");
            PauseMenu.pauseBackground = Content.Load<Texture2D>("UI\\pausebackground");
            PauseMenu.mouseCursor = Content.Load<Texture2D>("UI\\mouse");
            PauseMenu.mouseAndKeyboardHelp = Content.Load<Texture2D>("UI\\mouseandkeyboard");
            PauseMenu.gamePadHelp = Content.Load<Texture2D>("UI\\gamepadhelp");
            PauseMenu.keyboardOnlyHelp = Content.Load<Texture2D>("UI\\keyboardonlyhelp");
            Decoration.defaultTexture = Content.Load<Texture2D>("abilities");
            Hud.fuelCover = Content.Load<Texture2D>("Decals\\decal_fuelcover");
            Hud.fuelMeter = Content.Load<Texture2D>("Decals\\decal_fuelmeter");
            Projectile.blastTexture = Content.Load<Texture2D>("Projectiles\\blast");
            Projectile.plasmaTexture = Content.Load<Texture2D>("Projectiles\\plasma");
            Projectile.laserTexture = Content.Load<Texture2D>("Projectiles\\laser");
            Projectile.missileTexture = Content.Load<Texture2D>("Projectiles\\missile");
            Monster.Init(Content);
            Block.wallTexture = Content.Load<Texture2D>("Walls\\wall_texture");
            Block.circuitTexture = Content.Load<Texture2D>("Walls\\circuit_wall");
            Block.crackedTexture = Content.Load<Texture2D>("Walls\\cracked_wall");
            Block.vineTexture = Content.Load<Texture2D>("Walls\\vine_wall");
            Block.cobblestoneTexture = Content.Load<Texture2D>("Walls\\cobblestone_wall");
            Block.fancyPlateTexture = Content.Load<Texture2D>("Walls\\fancyPlate_wall");
            Block.cargoTexture = Content.Load<Texture2D>("Walls\\cargo_wall");
            Block.crateTexture = Content.Load<Texture2D>("Walls\\crate_wall");
            Block.iceTexture = Content.Load<Texture2D>("Walls\\ice_wall");
            Block.crystalTexture = Content.Load<Texture2D>("Walls\\crystal_wall");
            Block.gearslotTexture = Content.Load<Texture2D>("Walls\\gearslot_wall");

            Block.masterTextureList = new Texture2D[Block.maxWallTextureTypes];
            Block.masterTextureList[(int)VL.WallType.Cargo] = Content.Load<Texture2D>("Walls\\cargo_wall");
            Block.masterTextureList[(int)VL.WallType.Plate] = Content.Load<Texture2D>("Walls\\wall_texture");
            Block.masterTextureList[(int)VL.WallType.Circuit] = Content.Load<Texture2D>("Walls\\circuit_wall");
            Block.masterTextureList[(int)VL.WallType.Rock] = Content.Load<Texture2D>("Walls\\cracked_wall");
            Block.masterTextureList[(int)VL.WallType.Vines] = Content.Load<Texture2D>("Walls\\vine_wall");
            Block.masterTextureList[(int)VL.WallType.Cobblestone] = Content.Load<Texture2D>("Walls\\cobblestone_wall");
            Block.masterTextureList[(int)VL.WallType.FancyPlate] = Content.Load<Texture2D>("Walls\\fancyPlate_wall");
            Block.masterTextureList[(int)VL.WallType.Crate] = Content.Load<Texture2D>("Walls\\crate_wall");
            Block.masterTextureList[(int)VL.WallType.Ice] = Content.Load<Texture2D>("Walls\\ice_wall");           
            Block.masterTextureList[(int)VL.WallType.Crystal] = Content.Load<Texture2D>("Walls\\crystal_wall");
            Block.masterTextureList[(int)VL.WallType.Gearslot] = Content.Load<Texture2D>("Walls\\gearslot_wall");
            
            Edge.iceEdge = Content.Load<Texture2D>("Walls\\ice_edge");
            Edge.spikeEdge = Content.Load<Texture2D>("Walls\\spike_edge");
            Edge.magnetEdge = Content.Load<Texture2D>("Walls\\magnet_edge");
            Edge.lavaCoolEdge = Content.Load<Texture2D>("Walls\\lavacool_edge");
            Edge.lavaHotEdge = Content.Load<Texture2D>("Walls\\lavahot_edge");
            Hud.hudBase = Content.Load<Texture2D>("UI\\hudbase");
            Hud.hudBaseInfo = Content.Load<Texture2D>("UI\\hudbaseinfo");
            Hud.healthmeter_bottom = Content.Load<Texture2D>("UI\\healthmeter_bottom");
            Hud.healthmeter_color = Content.Load<Texture2D>("UI\\healthmeter_color");
            Hud.healthmeter_mid = Content.Load<Texture2D>("UI\\healthmeter_mid");
            Hud.healthmeter_top = Content.Load<Texture2D>("UI\\healthmeter_top");
            Hud.healthmeter_slot = Content.Load<Texture2D>("UI\\healthmeter_slot");
            Hud.expansion_back = Content.Load<Texture2D>("UI\\expansion_back");
            Hud.expansion_front = Content.Load<Texture2D>("UI\\expansion_front");
            Hud.expansion_top = Content.Load<Texture2D>("UI\\expansion_top");
            Hud.expansion_red = Content.Load<Texture2D>("UI\\expansion_red");
            Hud.expansion_bottom = Content.Load<Texture2D>("UI\\expansion_bottom");
            MapHud.hudLeft = Content.Load<Texture2D>("UI\\maphud_left");
            MapHud.hudRight = Content.Load<Texture2D>("UI\\maphud_right");
            MapHud.leftArrow = Content.Load<Texture2D>("UI\\hudarrowleft");
            MapHud.rightArrow = Content.Load<Texture2D>("UI\\hudarrowright");
            MapHud.inventoryDataMonitor = Content.Load<Texture2D>("UI\\maphud_inventorydata");
            MapHud.inventoryListMonitor = Content.Load<Texture2D>("UI\\maphud_inventorylist");
            MapHud.mapDataMonitor = Content.Load<Texture2D>("UI\\maphud_datamonitor");
            MapHud.mapObjectiveMonitor = Content.Load<Texture2D>("UI\\maphud_objectivemonitor");
            MapHud.mapFilterMonitor = Content.Load<Texture2D>("UI\\maphud_filter");
            MapHud.smallFont = Content.Load<SpriteFont>("Fonts\\HudFontSmall");
            MapHud.largeFont = Content.Load<SpriteFont>("Fonts\\HudFontLarge");
            Skybox.LoadTextures(Content);
            DecorationImage.LoadTextures(Content);
            Doodad.powerCubeTexture = Content.Load<Texture2D>("Decorations\\powercube");
            Engine.spriteFont = Content.Load<SpriteFont>("Fonts\\Font");
            Engine.loadFont = Content.Load<SpriteFont>("Fonts\\LoadFont");
            Engine.loadFontBold = Content.Load<SpriteFont>("Fonts\\LoadFontBold");
            DialogBox.smallFont = Content.Load<SpriteFont>("Fonts\\DialogFontSmall");
            DialogBox.largeFont = Content.Load<SpriteFont>("Fonts\\DialogFontLarge");
            DialogBox.InitPortraits(Content);

            Doodad.InitBeamTextures(Content);
            SaveGameText.confirmOptions = Content.Load<Texture2D>("UI\\icon_confirm");
            SaveGameText.loadOptions = Content.Load<Texture2D>("UI\\icon_load");
            SaveGameText.InitTexCoords();
            Doodad.InitDecalTextures(Content);
            Doodad.useButton = Content.Load<Texture2D>("UI\\ekey");
            Doodad.leftButton = Content.Load<Texture2D>("UI\\leftbutton");
            Doodad.rightButton = Content.Load<Texture2D>("UI\\rightbutton");
            Doodad.hologram_oldMan = Content.Load<Texture2D>("Decorations\\hologram_oldMan");
            Doodad.hologram_finalBoss = Content.Load<Texture2D>("Decorations\\hologram_finalBoss");

            DialogBox.textArea = Content.Load<Texture2D>("UI\\dialogtextarea");
            DialogBox.monitor = Content.Load<Texture2D>("UI\\dialogmonitor");
            DialogBox.monitorFrame = Content.Load<Texture2D>("UI\\dialogmonitorframe");
            DialogBox.rightEdge = Content.Load<Texture2D>("UI\\dialogrightedge");

            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;

            Engine.sceneRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                   pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);

            Engine.normalDepthRenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                                         pp.BackBufferWidth, pp.BackBufferHeight, false,
                                                         pp.BackBufferFormat, pp.DepthStencilFormat);

            SoundFX.Init(Content);
            Mouse.SetPosition(Game1.titleSafeRect.Center.X, Game1.titleSafeRect.Center.Y);
            
            MusicControl.music_menu = Content.Load<Song>("Music\\music_menu");
            MusicControl.music_game = Content.Load<Song>("Music\\music_game");
            MusicControl.greenSector = Content.Load<Song>("Music\\GreenMusic");
            MusicControl.hubSector = Content.Load<Song>("Music\\HubMusic");
            MusicControl.storageSector = Content.Load<Song>("Music\\StorageMusic");
            MusicControl.engineSector = Content.Load<Song>("Music\\EngineMusic");
            MusicControl.coreSector = Content.Load<Song>("Music\\CoreMusic");

            MusicControl.boss = Content.Load<Song>("Music\\BossMusic");
            MusicControl.finalBoss = Content.Load<Song>("Music\\FinalBoss");
            MusicControl.firstLoad = Content.Load<Song>("Music\\FirstLoad");
            MusicControl.itemRoom = Content.Load<Song>("Music\\ItemRoomPreUpgrade");
            MusicControl.upgrade = Content.Load<Song>("Music\\UpgradeMusic");
            MusicControl.intro = Content.Load<Song>("Music\\IntroMusic");
            MusicControl.death = Content.Load<Song>("Music\\DeathMusic");

            ObjectiveControl.InitObjectiveControl();

            LevelLoader.Load("LevelData\\world");
            LevelLoader.QuickSave(true);
            
            //LevelLoader.Load("LevelData\\spikeelevator");
            LevelLoader.Load("LevelData\\menu");

            SpriteUtil.SetBestFont();            
            
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
            if (IsActive == false)
            {
                return;
            }
            gameWindow = this.Window;
            
            
            // Allows the game to exit
            if (Engine.quit == true)
            {
                // Save settings
                Settings s = new Settings();
                s.drawDistance = Engine.drawDepth;
                s.resIndex = Engine.resIndex;
                s.music = Engine.musicEnabled;
                s.soundeffects = Engine.soundEffectsEnabled;
                s.fullscreen = Engine.fullScreen;

                Stream stream = new FileStream("settings", FileMode.Create, FileAccess.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                
                serializer.Serialize(stream, s);
                stream.Close();
                this.Exit();
            }
            

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

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
    public enum MenuItems
    {
        Continue,
        RestartRoom,
        LoadLastSave,
        MainMenu,
        Music,
        SoundEffects,
        Transparency,
        DrawDistance,
        FullScreen,
        Resolution,
        Quit
    }

    public class MenuOption
    {
        int x = 0;
        int y = 0;

        String text;

        public MenuItems type;

        public MenuOption(String text, int x, int y, MenuItems type)
        {
            this.type = type;
            this.x = x;
            this.y = y;
            this.text = text;
        }

        public String DisplayString(String extraData)
        {
            return text + extraData;
        }

        public Vector2 TextLocation
        {
            get
            {
                return new Vector2(x + 30, y);
            }
        }

        public Vector2 XLocation
        {
            get
            {
                return new Vector2(x, y);
            }
        }
    }

    class PauseMenu
    {
        public static int cooldown = 0;
        public static int maxCooldown = 100;
        public static bool paused = false;
        public static Texture2D pauseBackground;
        public static Texture2D mouseCursor;
        public static bool mouseSelect = false;

        public static int selectIndex = 0;
        public static GamePadState prevGamePadState = new GamePadState();
        public static KeyboardState prevKeyboardState = new KeyboardState();

        public static Vector2 mousePos = Vector2.Zero;

        public static List<MenuOption> options;

        public static void Draw()
        {
            int optionBaseX = Game1.titleSafeRect.Center.X - 100;
            int optionBaseY = Game1.titleSafeRect.Center.Y - 80;
            if (paused == true)
            {
                if (options == null)
                {
                    options = new List<MenuOption>();
                    options.Add(new MenuOption("CONTINUE", optionBaseX, optionBaseY+0, MenuItems.Continue));

                    options.Add(new MenuOption("RESTART ROOM", optionBaseX, optionBaseY+40, MenuItems.RestartRoom));
                    options.Add(new MenuOption("LOAD LAST SAVE", optionBaseX, optionBaseY+60, MenuItems.LoadLastSave));
                    options.Add(new MenuOption("RETURN TO MAIN MENU", optionBaseX, optionBaseY+80, MenuItems.MainMenu));

                    options.Add(new MenuOption("MUSIC: ", optionBaseX, optionBaseY+120, MenuItems.Music));
                    options.Add(new MenuOption("SOUND EFFECTS: ", optionBaseX, optionBaseY+140, MenuItems.SoundEffects));
                    options.Add(new MenuOption("TRANSPARENCY: ", optionBaseX, optionBaseY+160, MenuItems.Transparency));
                    options.Add(new MenuOption("DRAW DISTANCE: ", optionBaseX, optionBaseY + 180, MenuItems.DrawDistance));
                    options.Add(new MenuOption("FULL SCREEN: ", optionBaseX, optionBaseY + 200, MenuItems.FullScreen));
                    options.Add(new MenuOption("RESOLUTION: ", optionBaseX, optionBaseY + 220, MenuItems.Resolution));

                    options.Add(new MenuOption("QUIT", optionBaseX, optionBaseY+260, MenuItems.Quit));
                
                }
                mousePos.X = Mouse.GetState().X-32;
                mousePos.Y = Mouse.GetState().Y;
                Engine.spriteBatch.Begin();
                Color transparentBlack = new Color(0, 0, 0, 128);

                Engine.spriteBatch.Draw(pauseBackground, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), transparentBlack);
                Engine.spriteBatch.Draw(mouseCursor, mousePos, Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "PAUSED", new Vector2(optionBaseX-20, optionBaseY - 40), Color.YellowGreen);

                for(int i = 0; i < options.Count; i++)
                {
                    if(i==selectIndex)
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "X", options[i].XLocation, Color.YellowGreen);
                    String extraData = "";
                    if (options[i].type == MenuItems.SoundEffects)
                    {
                        if (Engine.soundEffectsEnabled == true)
                            extraData = "On";
                        else
                            extraData = "Off";
                    }
                    if (options[i].type == MenuItems.SoundEffects)
                    {
                        if (Engine.soundEffectsEnabled == true)
                            extraData = "On";
                        else
                            extraData = "Off";
                    }
                    if (options[i].type == MenuItems.Music)
                    {
                        if (Engine.musicEnabled == true)
                            extraData = "On";
                        else
                            extraData = "Off";
                    }
                    if (options[i].type == MenuItems.Transparency)
                    {
                        if (Engine.transparencyEnabled == true)
                            extraData = "On";
                        else
                            extraData = "Off";
                    }
                    if (options[i].type == MenuItems.FullScreen)
                    {
                        if (Engine.fullScreen == true)
                            extraData = "On";
                        else
                            extraData = "Off";
                    }
                    if (options[i].type == MenuItems.Resolution)
                    {
                        if(Engine.res == ResolutionSettings.R_800x600)
                            extraData = "800x600";
                        if (Engine.res == ResolutionSettings.R_1920x1080)
                            extraData = "1920x1080";
                    }
                    if (options[i].type == MenuItems.DrawDistance)
                    {
                        extraData = Engine.drawDistance + "";
                    }
                    Engine.spriteBatch.DrawString(Engine.spriteFont, options[i].DisplayString(extraData), options[i].TextLocation, Color.YellowGreen);                

                }
                                                

                Engine.spriteBatch.End();
            }
            
        }

        public static void Pause()
        {
            selectIndex = 0;
            if (cooldown != 0)
                return;
            paused = true;
            cooldown = maxCooldown;
        }

        public static void Update(GameTime gameTime)
        {
            int optionBaseX = Game1.titleSafeRect.Center.X - 100;
            int optionBaseY = Game1.titleSafeRect.Center.Y - 80;
            GamePadState currentGamePadState = GamePad.GetState(Game1.activePlayer);
            KeyboardState currentKeyboardState = Keyboard.GetState();
            
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown < 0)
                cooldown = 0;

            mouseSelect = false;
            if (mousePos.X > optionBaseX - 20 && mousePos.X < optionBaseX + 250 && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    if (Math.Abs(mousePos.Y - 10 - options[i].TextLocation.Y) < 10)
                    {
                        selectIndex = i;
                        mouseSelect = true;
                    }
                }
            }

            if (cooldown == 0 && paused == false && ((currentKeyboardState.IsKeyDown(Keys.Escape) && prevKeyboardState.IsKeyDown(Keys.Escape) == false) || ((prevGamePadState.IsButtonDown(Buttons.Start) == false && currentGamePadState.IsButtonDown(Buttons.Start) == true))))
            {
                PauseMenu.Pause();
            }
            else if (cooldown == 0 && paused == true)
            {
                if (paused == true && cooldown == 0 && ((currentKeyboardState.IsKeyDown(Keys.Escape) && prevKeyboardState.IsKeyDown(Keys.Escape) == false) || ((prevGamePadState.IsButtonDown(Buttons.Start) == false && currentGamePadState.IsButtonDown(Buttons.Start) == true) || currentGamePadState.IsButtonDown(Buttons.B))))
                {
                    cooldown = maxCooldown;
                    paused = false;
                }
                if (GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.Y < -.5f || currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S))
                {
                    selectIndex++;
                    selectIndex %= 11;
                    cooldown = 2*maxCooldown;
                }
                if (GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.Y > .5f || currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
                {
                    selectIndex--;
                    selectIndex += 11;
                    selectIndex %= 11;
                    cooldown = 2*maxCooldown;
                }
                
                if (mouseSelect == true || (currentGamePadState.IsButtonDown(Buttons.A) == false && prevGamePadState.IsButtonDown(Buttons.A) == true) || currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    MenuItems result = (MenuItems)selectIndex;
                    if (result == MenuItems.Continue)
                        paused = false;
                    if (result == MenuItems.RestartRoom)
                    {
                        Engine.player.Respawn();
                        Physics.refresh = true;
                        Engine.reDraw = true;
                        paused = false;
                    }
                    if (result == MenuItems.LoadLastSave)
                    {
                        if (Engine.saveFileIndex != 0)
                        {
                            LevelLoader.LoadFromDisk(Engine.saveFileIndex);
                            Physics.refresh = true;
                            Engine.reDraw = true;
                            paused = false;
                        }                        
                    }
                    if (result == MenuItems.MainMenu)
                    {
                        LevelLoader.Load("LevelData\\menu");
                        WorldMap.selectedRoomIndex = 0;
                        WorldMap.selectedSectorIndex = 0;
                        Physics.refresh = true;
                        Engine.reDraw = true;
                        paused = false;
                    }
                    if (result == MenuItems.Music)
                    {
                        Engine.musicEnabled = !Engine.musicEnabled;
                        if (Engine.musicEnabled)
                            MusicControl.PlayGameMusic();
                        else
                            MusicControl.Pause();
                        cooldown = maxCooldown;
                    }
                    if (result == MenuItems.SoundEffects)
                    {
                        Engine.soundEffectsEnabled = !Engine.soundEffectsEnabled;
                        cooldown = maxCooldown;
                    }
                    if (result == MenuItems.Transparency)
                    {
                        Engine.transparencyEnabled = !Engine.transparencyEnabled;
                        Engine.reDraw = true;
                        cooldown = maxCooldown;
                    }
                    if (result == MenuItems.DrawDistance)
                    {
                        Engine.drawDistance += 1;
                        Engine.reDraw = true;
                        cooldown = maxCooldown;
                    }
                    if (result == MenuItems.Quit)
                    {
                        Engine.quit = true;
                        paused = false;
                    }
                    if (result == MenuItems.FullScreen)
                    {
                        Engine.fullScreen = !Engine.fullScreen;
                        Game1.SetGraphicsSettings();
                        cooldown = maxCooldown;

                    }
                    if (result == MenuItems.Resolution)
                    {
                        if (Engine.res == ResolutionSettings.R_800x600)
                            Engine.res = ResolutionSettings.R_1920x1080;
                        else
                            Engine.res = ResolutionSettings.R_800x600;
                        Game1.SetGraphicsSettings();
                        options = null;
                        cooldown = maxCooldown;
                    }
                }
            }

            if (paused == true)
            {
                if (selectIndex == 7 && currentGamePadState.ThumbSticks.Left.X != 0)
                {
                    Engine.drawDistance += currentGamePadState.ThumbSticks.Left.X / 2;
                    Engine.reDraw = true;
                }
            }
            if (Engine.drawDistance > 1000)
                Engine.drawDistance = 1000;
            if (Engine.drawDistance < 10)
                Engine.drawDistance = 10;
            prevGamePadState = currentGamePadState;
            prevKeyboardState = currentKeyboardState;
        }
    }
}

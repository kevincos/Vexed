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
        ControlScheme, 
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
        public static int maxCooldown = 200;
        public static bool paused = false;
        public static Texture2D pauseBackground;
        public static Texture2D mouseCursor;
        public static Texture2D mouseAndKeyboardHelp;
        public static Texture2D gamePadHelp;
        public static Texture2D keyboardOnlyHelp;
        public static bool mouseSelect = false;
        public static bool displayControlInfo = false;

        public static int selectIndex = 0;
        public static GamePadState prevGamePadState = new GamePadState();
        public static KeyboardState prevKeyboardState = new KeyboardState();
        public static MouseState prevMouseState = new MouseState();

        public static Vector2 mousePos = Vector2.Zero;

        public static List<MenuOption> options;

        public static ControlType oldControlType;

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

                    //options.Add(new MenuOption("CONTROLS:", optionBaseX, optionBaseY + 120, MenuItems.ControlScheme));
                    options.Add(new MenuOption("MUSIC: ", optionBaseX, optionBaseY+120, MenuItems.Music));
                    options.Add(new MenuOption("SOUND EFFECTS: ", optionBaseX, optionBaseY+140, MenuItems.SoundEffects));
                    //options.Add(new MenuOption("TRANSPARENCY: ", optionBaseX, optionBaseY+180, MenuItems.Transparency));
                    options.Add(new MenuOption("DRAW DISTANCE: ", optionBaseX, optionBaseY + 160, MenuItems.DrawDistance));
                    options.Add(new MenuOption("FULL SCREEN: ", optionBaseX, optionBaseY + 180, MenuItems.FullScreen));
                    options.Add(new MenuOption("RESOLUTION: ", optionBaseX, optionBaseY + 200, MenuItems.Resolution));

                    options.Add(new MenuOption("QUIT", optionBaseX, optionBaseY+280, MenuItems.Quit));
                
                }
                mousePos.X = Mouse.GetState().X-32;
                mousePos.Y = Mouse.GetState().Y;
                Engine.spriteBatch.Begin();
                Color transparentBlack = new Color(0, 0, 0, 128);

                Engine.spriteBatch.Draw(pauseBackground, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), transparentBlack);
                Engine.spriteBatch.Draw(mouseCursor, mousePos, Color.YellowGreen);

                if (displayControlInfo == true)
                {
                    if(Engine.controlType == ControlType.MouseAndKeyboard)
                        Engine.spriteBatch.Draw(mouseAndKeyboardHelp, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), Color.YellowGreen);
                    if (Engine.controlType == ControlType.KeyboardOnly)
                        Engine.spriteBatch.Draw(keyboardOnlyHelp, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), Color.YellowGreen);
                    if (Engine.controlType == ControlType.GamePad)
                        Engine.spriteBatch.Draw(gamePadHelp, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), Color.YellowGreen);
                }
                else
                {

                    Engine.spriteBatch.DrawString(Engine.spriteFont, "PAUSED", new Vector2(optionBaseX - 20, optionBaseY - 40), Color.YellowGreen);

                    for (int i = 0; i < options.Count; i++)
                    {
                        if ((int)options[i].type == selectIndex)
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
                        if (options[i].type == MenuItems.ControlScheme)
                        {
                            if (Engine.controlType == ControlType.MouseAndKeyboard)
                            {
                                extraData = "Mouse and Keybaord";
                            }
                            else if (Engine.controlType == ControlType.KeyboardOnly)
                            {
                                extraData = "Keyboard Only";
                            }
                            else if (Engine.controlType == ControlType.GamePad)
                            {
                                extraData = "Gamepad";
                            }
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
                            if (Engine.res == ResolutionSettings.R_800x600)
                                extraData = "800x600";
                            if (Engine.res == ResolutionSettings.R_1920x1080)
                                extraData = "1920x1080";
                        }
                        if (options[i].type == MenuItems.DrawDistance)
                        {
                            if (Engine.drawDepth == 0)
                                extraData = "Minimum";
                            else if (Engine.drawDepth == 1)
                                extraData = "Normal";
                            else if (Engine.drawDepth == 2)
                                extraData = "High";
                            else
                                extraData = "Maximum";                            
                        }
                        Engine.spriteBatch.DrawString(Engine.spriteFont, options[i].DisplayString(extraData), options[i].TextLocation, Color.YellowGreen);

                    }
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
            oldControlType = Engine.controlType;
            displayControlInfo = false;
        }

        public static void Update(int gameTime)
        {
            int optionBaseX = Game1.titleSafeRect.Center.X - 100;
            int optionBaseY = Game1.titleSafeRect.Center.Y - 80;
            GamePadState currentGamePadState = GamePad.GetState(Game1.activePlayer);
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            cooldown -= gameTime;
            if (cooldown < 0)
                cooldown = 0;

            mouseSelect = false;
            if (mousePos.X > optionBaseX - 20 && mousePos.X < optionBaseX + 250 && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    if (Math.Abs(mousePos.Y - 10 - options[i].TextLocation.Y) < 10)
                    {
                        selectIndex = (int)(options[i].type);
                        mouseSelect = true;
                    }
                }
            }

            if (cooldown == 0 && paused == false && ((currentKeyboardState.IsKeyDown(Keys.Escape) && prevKeyboardState.IsKeyDown(Keys.Escape) == false) || ((prevGamePadState.IsButtonDown(Buttons.Start) == false && currentGamePadState.IsButtonDown(Buttons.Start) == true))))
            {
                PauseMenu.Pause();
            }
            else if (cooldown == 0 && paused == true && displayControlInfo == true)
            {
                if (paused == true && cooldown == 0 && ((currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) || (currentKeyboardState.IsKeyDown(Keys.Escape) && prevKeyboardState.IsKeyDown(Keys.Escape) == false) || ((prevGamePadState.IsButtonDown(Buttons.Start) == false && currentGamePadState.IsButtonDown(Buttons.Start) == true) || currentGamePadState.IsButtonDown(Buttons.B))))
                {
                    cooldown = maxCooldown;
                    paused = false;
                }
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
                    selectIndex %= 12;
                    cooldown = 2 * maxCooldown;
                }
                if (GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.Y > .5f || currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
                {
                    selectIndex--;
                    selectIndex += 12;
                    selectIndex %= 12;
                    cooldown = 2 * maxCooldown;
                }

                if (mouseSelect == true || (currentGamePadState.IsButtonDown(Buttons.A) == false && prevGamePadState.IsButtonDown(Buttons.A) == true) || currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    MenuItems result = (MenuItems)selectIndex;
                    if (result == MenuItems.Continue)
                    {
                        SoundFX.MenuSelect();
                        Game1.controller.AButton.Invalidate();
                        Game1.controller.XButton.Invalidate();
                        Game1.controller.YButton.Invalidate();
                        paused = false;
                    }
                    if (result == MenuItems.RestartRoom)
                    {
                        SoundFX.MenuSelect();
                        Game1.controller.AButton.Invalidate();
                        Game1.controller.XButton.Invalidate();
                        Game1.controller.YButton.Invalidate();
                        Engine.player.Respawn();
                        Physics.refresh = true;
                        Engine.reDraw = true;
                        paused = false;
                    }
                    if (result == MenuItems.LoadLastSave)
                    {
                        SoundFX.MenuSelect();
                        if (Engine.saveFileIndex != 0)
                        {
                            Game1.controller.AButton.Invalidate();
                            Game1.controller.XButton.Invalidate();
                            Game1.controller.YButton.Invalidate();
                            LevelLoader.LoadFromDisk(Engine.saveFileIndex);
                            Physics.refresh = true;
                            Engine.reDraw = true;
                            paused = false;
                        }
                    }
                    if (result == MenuItems.MainMenu)
                    {
                        SoundFX.MenuSelect();
                        Game1.controller.AButton.Invalidate();
                        Game1.controller.XButton.Invalidate();
                        Game1.controller.YButton.Invalidate();
                        SaveGameText.GenerateSummaries();
                        LevelLoader.Load("LevelData\\menu");
                        WorldMap.selectedRoomIndex = 0;
                        WorldMap.selectedSectorIndex = 0;
                        Physics.refresh = true;
                        Engine.reDraw = true;
                        paused = false;
                    }
                    if (result == MenuItems.ControlScheme)
                    {
                        SoundFX.MenuSelect();
                        int newControlType = (int)Engine.controlType + 1;
                        newControlType %= 3;
                        Engine.controlType = (ControlType)newControlType;
                        cooldown = maxCooldown;
                    }
                    if (result == MenuItems.Music)
                    {
                        SoundFX.MenuSelect();
                        Engine.musicEnabled = !Engine.musicEnabled;
                        if (Engine.musicEnabled)
                            MusicControl.PlayGameMusic();
                        else
                            MusicControl.Pause();
                        cooldown = maxCooldown;
                    }
                    if (result == MenuItems.SoundEffects)
                    {
                        SoundFX.MenuSelect();
                        Engine.soundEffectsEnabled = !Engine.soundEffectsEnabled;
                        SoundFX.MenuSelect();
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
                        SoundFX.MenuSelect();
                        Engine.drawDepth += 1;
                        Engine.drawDepth %= 4;
                            
                        Engine.reDraw = true;
                        cooldown = maxCooldown;
                    }
                    if (result == MenuItems.Quit)
                    {
                        SoundFX.MenuSelect();
                        Engine.quit = true;
                        paused = false;
                    }
                    if (result == MenuItems.FullScreen)
                    {
                        SoundFX.MenuSelect();
                        Engine.fullScreen = !Engine.fullScreen;
                        Game1.SetGraphicsSettings();
                        cooldown = maxCooldown;

                    }
                    if (result == MenuItems.Resolution)
                    {
                        SoundFX.MenuSelect();
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
                    if (currentGamePadState.ThumbSticks.Left.X > 0)
                        Engine.drawDepth++;
                    else
                        Engine.drawDepth--;
                    
                    Engine.reDraw = true;
                }
            }
            Engine.drawDepth %= 4;
            prevGamePadState = currentGamePadState;
            prevKeyboardState = currentKeyboardState;
            prevMouseState = currentMouseState;

            if (paused == false && displayControlInfo == false && Engine.controlType != oldControlType)
            {
                paused = true;
                displayControlInfo = true;
            }
        }
    }
}

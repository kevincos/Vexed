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
            if (paused == true)
            {
                if (options == null)
                {
                    options = new List<MenuOption>();
                    options.Add(new MenuOption("CONTINUE", 300, 220, MenuItems.Continue));

                    options.Add(new MenuOption("RESTART ROOM", 300, 260, MenuItems.RestartRoom));
                    options.Add(new MenuOption("LOAD LAST SAVE",300, 280, MenuItems.LoadLastSave));
                    options.Add(new MenuOption("RETURN TO MAIN MENU",300, 300, MenuItems.MainMenu));

                    options.Add(new MenuOption("MUSIC: ",300, 340, MenuItems.Music));
                    options.Add(new MenuOption("SOUND EFFECTS: ",300, 360, MenuItems.SoundEffects));
                    options.Add(new MenuOption("TRANSPARENCY: ",300, 380, MenuItems.Transparency));
                    options.Add(new MenuOption("DRAW DISTANCE: ",300, 400, MenuItems.DrawDistance));

                    options.Add(new MenuOption("QUIT", 300, 440, MenuItems.Quit));
                
                }
                mousePos.X = Mouse.GetState().X-32;
                mousePos.Y = Mouse.GetState().Y;
                Engine.spriteBatch.Begin();
                Color transparentBlack = new Color(0, 0, 0, 128);

                Engine.spriteBatch.Draw(pauseBackground, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), transparentBlack);
                Engine.spriteBatch.Draw(mouseCursor, mousePos, Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "PAUSED", new Vector2(280, 180), Color.YellowGreen);

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
            
            GamePadState currentGamePadState = GamePad.GetState(Game1.activePlayer);
            KeyboardState currentKeyboardState = Keyboard.GetState();
            
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown < 0)
                cooldown = 0;

            mouseSelect = false;
            if (mousePos.X > 280 && mousePos.X < 450 && Mouse.GetState().LeftButton == ButtonState.Pressed)
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
                    selectIndex %= 9;
                    cooldown = 2*maxCooldown;
                }
                if (GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.Y > .5f || currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
                {
                    selectIndex--;
                    selectIndex += 9;
                    selectIndex %= 9;
                    cooldown = 2*maxCooldown;
                }
                
                if (mouseSelect == true || (currentGamePadState.IsButtonDown(Buttons.A) == false && prevGamePadState.IsButtonDown(Buttons.A) == true) || currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Enter))
                {
                    if (selectIndex == 0)
                        paused = false;
                    if (selectIndex == 1)
                    {
                        Engine.player.Respawn();
                        Physics.refresh = true;
                        Engine.reDraw = true;
                        paused = false;
                    }
                    if (selectIndex == 2)
                    {
                        if (Engine.saveFileIndex != 0)
                        {
                            LevelLoader.LoadFromDisk(Engine.saveFileIndex);
                            Physics.refresh = true;
                            Engine.reDraw = true;
                            paused = false;
                        }                        
                    }
                    if (selectIndex == 3)
                    {
                        LevelLoader.Load("LevelData\\menu");
                        Physics.refresh = true;
                        Engine.reDraw = true;
                        paused = false;
                    }
                    if (selectIndex == 4)
                    {
                        Engine.musicEnabled = !Engine.musicEnabled;
                        if (Engine.musicEnabled)
                            MusicControl.PlayGameMusic();
                        else
                            MusicControl.Pause();
                        cooldown = maxCooldown;
                    }
                    if (selectIndex == 5)
                    {
                        Engine.soundEffectsEnabled = !Engine.soundEffectsEnabled;
                        cooldown = maxCooldown;
                    }
                    if (selectIndex == 6)
                    {
                        Engine.transparencyEnabled = !Engine.transparencyEnabled;
                        Engine.reDraw = true;
                        cooldown = maxCooldown;
                    }
                    if (selectIndex == 7)
                    {
                        Engine.drawDistance += 1;
                        Engine.reDraw = true;
                    }
                    if (selectIndex == 8)
                    {
                        Engine.quit = true;
                        paused = false;
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

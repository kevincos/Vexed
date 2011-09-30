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
    class PauseMenu
    {
        public static int cooldown = 0;
        public static int maxCooldown = 100;
        public static bool paused = false;
        public static Texture2D pauseBackground;

        public static int selectIndex = 0;
        public static GamePadState prevGamePadState = new GamePadState();

        public static void Draw()
        {
            if (paused == true)
            {
                Engine.spriteBatch.Begin();
                Color transparentBlack = new Color(0, 0, 0, 128);

                Engine.spriteBatch.Draw(pauseBackground, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), transparentBlack);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "PAUSED", new Vector2(280, 180), Color.YellowGreen);

                Engine.spriteBatch.DrawString(Engine.spriteFont, "RESUME", new Vector2(300, 220), Color.YellowGreen);                
                

                Engine.spriteBatch.DrawString(Engine.spriteFont, "RESTART ROOM", new Vector2(300, 260), Color.YellowGreen);                
                Engine.spriteBatch.DrawString(Engine.spriteFont, "LOAD LAST SAVE", new Vector2(300, 280), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "RETURN TO MAIN MENU", new Vector2(300, 300), Color.YellowGreen);

                Engine.spriteBatch.DrawString(Engine.spriteFont, "MUSIC: " + Engine.musicEnabled, new Vector2(300, 340), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "SOUND EFFECTS: " + Engine.soundEffectsEnabled, new Vector2(300, 360), Color.YellowGreen);                
                Engine.spriteBatch.DrawString(Engine.spriteFont, "TRANSPARENCY: " + Engine.transparencyEnabled, new Vector2(300, 380), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "DRAW DISTANCE: " + Engine.drawDistance , new Vector2(300, 400), Color.YellowGreen);

                Engine.spriteBatch.DrawString(Engine.spriteFont, "QUIT", new Vector2(300, 440), Color.YellowGreen);
                

                int cursorY = 220 + 20 * selectIndex;
                if (selectIndex > 0) cursorY += 20;
                if (selectIndex > 3) cursorY += 20;
                if (selectIndex > 7) cursorY += 20;

                Engine.spriteBatch.DrawString(Engine.spriteFont, "X", new Vector2(280, cursorY), Color.YellowGreen);

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
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown < 0)
                cooldown = 0;

            if (cooldown == 0 && paused == false && ((prevGamePadState.IsButtonDown(Buttons.Start) == false && currentGamePadState.IsButtonDown(Buttons.Start) == true)))
            {
                PauseMenu.Pause();
            }
            else if (cooldown == 0 && paused == true)
            {
                if (paused == true && cooldown == 0 &&  ((prevGamePadState.IsButtonDown(Buttons.Start) == false && currentGamePadState.IsButtonDown(Buttons.Start) == true) || currentGamePadState.IsButtonDown(Buttons.B)))
                {
                    cooldown = maxCooldown;
                    paused = false;
                }
                if (GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.Y < -.5f)
                {
                    selectIndex++;
                    selectIndex %= 9;
                    cooldown = maxCooldown;
                }
                if (GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.Y > .5f)
                {
                    selectIndex--;
                    selectIndex += 9;
                    selectIndex %= 9;
                    cooldown = maxCooldown;
                }
                
                if (currentGamePadState.IsButtonDown(Buttons.A) == false && prevGamePadState.IsButtonDown(Buttons.A) == true)
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
            if (Engine.drawDistance > 300)
                Engine.drawDistance = 300;
            if (Engine.drawDistance < 10)
                Engine.drawDistance = 10;
            prevGamePadState = currentGamePadState;
        }
    }
}

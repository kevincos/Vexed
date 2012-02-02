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
    public enum IntroState
    {
        FadeIn,
        SpaceBar,
        MoveHelp,
        LookHelp,
        LoadHelp,
        Play
    }
    class IntroOverlay
    {
        public static IntroState state = IntroState.FadeIn;
        public static int introTime = 0;
        public static int fadeTime = 2000;
        public static int moveHelpTime = 3000;
        public static int lookHelpTime = 7000;
        public static int useHelpTime = 20000;

        public static void Draw()
        {
            Engine.spriteBatch.Begin();
            Color transparentBlack = new Color(0, 0, 0, 0);
            if (state == IntroState.FadeIn)
            {
                transparentBlack.A = (Byte)(255 - (255f * introTime) / fadeTime);
            }

            Engine.spriteBatch.Draw(PauseMenu.pauseBackground, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), transparentBlack);
            Engine.spriteBatch.End();
        }

        public static void Update(int gameTime)
        {
            if (state == IntroState.FadeIn || state == IntroState.SpaceBar)
            {
                Engine.player.state = State.Dialog;
            }
            if (state != IntroState.SpaceBar)
            {
                introTime += gameTime;
            }
            if (state == IntroState.FadeIn && introTime > fadeTime)
            {
                state = IntroState.SpaceBar;
                DialogBox.SetDialog("pressspacebar");                
            }
            if (state == IntroState.SpaceBar && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                state = IntroState.MoveHelp;
                Game1.controller.JumpInvalidate();
                Engine.player.state = State.Normal;
                DialogBox.state = BoxState.Close;
                DialogBox.animationTime = 0;
                SoundFX.DialogExtend();
            }
            if (state == IntroState.MoveHelp && introTime > moveHelpTime)
            {
                state = IntroState.LookHelp;
                DialogBox.SetDialog("intromovehelp");
            }
            /*if (state == IntroState.LookHelp && introTime > moveHelpTime)
            {
                state = IntroState.LoadHelp;
                DialogBox.SetDialog("introlookhelp");
            }*/
            if (state == IntroState.LoadHelp && introTime > useHelpTime)
            {
                state = IntroState.Play;
                DialogBox.SetDialog("introusehelp");
            }
        }
    }
}

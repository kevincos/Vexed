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
using System.Xml.Serialization;

namespace VexedCore
{
    public class GameButton
    {
        public int buttonTime = 0;
        public Buttons button;
        public Keys key;
        public int mouseButton = 0;
        
        public bool released = false;
        public bool pressed = false;

        public GameButton(Buttons b, Keys k)
        {
            button = b;
            key = k;
            mouseButton = 0;
        }

        public GameButton(Buttons b, Keys k, int m)
        {
            button = b;
            key = k;
            mouseButton = m;
        }


        public void Invalidate()
        {
            pressed = false;
        }

        public bool Pressed
        {
            get
            {               
                return buttonTime != 0;
            }
        }

        public bool NewPressed
        {
            get
            {
                return pressed == true && buttonTime != 0;
            }
        }

        public bool Released
        {
            get
            {
                return released;
            }
        }

        public int HoldTime
        {
            get
            {
                return buttonTime;
            }
        }
    }

    public class Controls
    {
        public static int scrollWheelBase = 0;
        public static int scrollWheelMin = -10;
        public static int scrollWheelMax = 100;
        public static int scrollWheelPrev = 0;

        public static Vector2 lastMousePos = Vector2.Zero;

        public List<GameButton> buttons;
        public GameButton BackButton
        {
            get
            {
                return buttons[6];
            }
        }
        public GameButton XButton
        {
            get
            {
                return buttons[0];
            }
        }
        public GameButton YButton
        {
            get
            {
                return buttons[1];
            }
        }
        public GameButton AButton
        {
            get
            {
                return buttons[2];
            }
        }
        public GameButton BButton
        {
            get
            {
                return buttons[3];
            }
        }
        public PlayerIndex activePlayer;


        public bool JumpButtonPressed()
        {
            return AButton.Pressed || buttons[4].Pressed || buttons[5].Pressed || (YButton.Pressed && Engine.player.secondaryAbility.isJump) || (XButton.Pressed && Engine.player.primaryAbility.isJump);
        }

        public bool JumpButtonNewPressed()
        {
            return AButton.NewPressed || buttons[4].NewPressed || buttons[5].NewPressed || (YButton.NewPressed && Engine.player.secondaryAbility.isJump) || (XButton.NewPressed && Engine.player.primaryAbility.isJump);
        }

        public bool JumpButtonReleased()
        {
            return AButton.Released && buttons[4].Released && buttons[5].Released && (YButton.Released && Engine.player.secondaryAbility.isJump) && (XButton.Released && Engine.player.primaryAbility.isJump);
        }

        public void JumpInvalidate()
        {
            AButton.Invalidate();
            if (Engine.player.secondaryAbility.isJump)
                YButton.Invalidate();
            if (Engine.player.primaryAbility.isJump)
                XButton.Invalidate();
            buttons[4].Invalidate();
            buttons[5].Invalidate();
        }

        public int JumpHoldTime()
        {
            int aHold = AButton.HoldTime;
            int xHold = 0;
            int yHold = 0;
            if (Engine.player.primaryAbility.isJump) xHold = XButton.HoldTime;
            if (Engine.player.secondaryAbility.isJump) yHold = YButton.HoldTime;
            return Math.Max(aHold, Math.Max(xHold, yHold));

        }

        public Controls(PlayerIndex activePlayer)
        {
            this.activePlayer = activePlayer;
            buttons = new List<GameButton>();
            buttons.Add(new GameButton(Buttons.X, Keys.Z, 1));
            buttons.Add(new GameButton(Buttons.Y, Keys.X, 2));
            buttons.Add(new GameButton(Buttons.A, Keys.Space));
            buttons.Add(new GameButton(Buttons.B, Keys.E));
            buttons.Add(new GameButton(Buttons.A, Keys.Up));
            buttons.Add(new GameButton(Buttons.A, Keys.W));
            buttons.Add(new GameButton(Buttons.Back, Keys.Back));
            
        }

        public static Vector2 LeftStick()
        {
            if(Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                return Vector2.Zero;
            return GamePad.GetState(Game1.activePlayer).ThumbSticks.Left;            
        }

        public static bool IsLeftKeyDown()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                return false;
            return Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A);
        }
        public static bool IsUpKeyDown()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                return false;
            return Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W);
        }
        public static bool IsDownKeyDown()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                return false;
            return Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S);
        }
        public static bool IsRightKeyDown()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                return false;
            return Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D);
        }

        public static void ResetMouse()
        {
            Controls.scrollWheelPrev = Mouse.GetState().ScrollWheelValue;
            if (Engine.state == EngineState.Active && PauseMenu.paused == false && Keyboard.GetState().IsKeyDown(Keys.LeftControl) == false)
            {
                //Mouse.SetPosition(400, 300);
            }
            if (Engine.state != EngineState.Active)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    if (Controls.lastMousePos == Vector2.Zero)
                        Controls.lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    Mouse.SetPosition(Game1.titleSafeRect.Center.X, Game1.titleSafeRect.Center.Y);
                }
                else
                {
                    if (Controls.lastMousePos != Vector2.Zero)
                        Mouse.SetPosition((int)Controls.lastMousePos.X, (int)Controls.lastMousePos.Y);
                    Controls.lastMousePos = Vector2.Zero;
                }
            }
        }

        public static Vector2 GetCameraHelper()
        {
            Vector2 rightStick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Right;
            float yShift = rightStick.Y;
            float xShift = rightStick.X;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                    xShift -= 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                    xShift += 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
                    yShift -= 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
                    yShift += 1f;
                xShift += .15f * (Game1.titleSafeRect.Center.X - Mouse.GetState().X);
                yShift -= .15f * (Game1.titleSafeRect.Center.Y - Mouse.GetState().Y);
                if (xShift > 1)
                    xShift = 1;
                if (yShift > 1)
                    yShift = 1;
                if (xShift < -1)
                    xShift = -1;
                if (yShift < -1)
                    yShift = -1;
            }
            return new Vector2(xShift, yShift);
        }


        public static Vector2 GetStaticCameraHelper()
        {
            Vector2 rightStick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Right;
            float yShift = rightStick.Y;
            float xShift = rightStick.X;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                    xShift -= 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                    xShift += 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
                    yShift -= 1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
                    yShift += 1f;
            }
            xShift += .0025f * (Mouse.GetState().X - Game1.titleSafeRect.Center.X);
            yShift -= .0025f * (Mouse.GetState().Y - Game1.titleSafeRect.Center.Y);
            if (xShift > 1)
                xShift = 1;
            if (yShift > 1)
                yShift = 1;
            if (xShift < -1)
                xShift = -1;
            if (yShift < -1)
                yShift = -1;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                if (xShift > .5f)
                    xShift = .5f;
                if (yShift > .5f)
                    yShift = .5f;
                if (xShift < -.5f)
                    xShift = -.5f;
                if (yShift < -.5f)
                    yShift = -.5f;
            }
            return new Vector2(xShift, yShift);
        }


        public void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(Game1.activePlayer);
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            foreach (GameButton b in buttons)
            {
                if (gamePadState.IsButtonDown(b.button) || keyboardState.IsKeyDown(b.key) || (b.mouseButton == 1 && mouseState.LeftButton == ButtonState.Pressed) || (b.mouseButton == 2 && mouseState.RightButton == ButtonState.Pressed))
                {
                    if (b.buttonTime == 0)
                        b.pressed = true;
                    b.buttonTime += gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    b.pressed = false;
                    if (b.buttonTime != 0)
                    {
                        if (b.released == false)
                        {
                            b.released = true;
                        }
                        else
                        {
                            b.released = false;
                            b.buttonTime = 0;
                        }
                    }                   
                }
            }

            if (Engine.state == EngineState.Active && PauseMenu.paused == false)
            {
                if (Mouse.GetState().X < 0)
                    Mouse.SetPosition(0, Mouse.GetState().Y);
                if (Mouse.GetState().Y < 0)
                    Mouse.SetPosition(Mouse.GetState().X, 0);
                if (Mouse.GetState().X > Game1.titleSafeRect.Right)
                    Mouse.SetPosition(Game1.titleSafeRect.Right, Mouse.GetState().Y);
                if (Mouse.GetState().Y > Game1.titleSafeRect.Bottom)
                    Mouse.SetPosition(Mouse.GetState().X, Game1.titleSafeRect.Bottom);
                
            }
            
        }
    }
}

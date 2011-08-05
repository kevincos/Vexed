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
        public List<GameButton> buttons;
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

        public Controls(PlayerIndex activePlayer)
        {
            this.activePlayer = activePlayer;
            buttons = new List<GameButton>();
            buttons.Add(new GameButton(Buttons.X, Keys.LeftControl, 1));
            buttons.Add(new GameButton(Buttons.Y, Keys.LeftShift, 2));
            buttons.Add(new GameButton(Buttons.A, Keys.Space));
            buttons.Add(new GameButton(Buttons.B, Keys.E));
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
            
        }
    }
}

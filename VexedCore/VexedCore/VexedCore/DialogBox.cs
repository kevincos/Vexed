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
    public class DialogBox
    {
        public static Texture2D box;

        public int lifeTime = 0;
        int cooldown = 0;
        public int stage = 0;

        public void Draw()
        {
            Engine.spriteBatch.Begin();
            float xPercent = .1f;
            float yPercent = .7f;
            float textXPercent = .25f;
            float textYPercent = .72f;
            float boxWidth = .9f;
            float boxHeight = .1125f;
            String help1 = "Hey there! I'm a helpful NPC character. I'm \nhere to give you hints and useful advice.";
            String help2 = "Don't worry. You know you can trust me \nbecause I've got this sweet beard.";
            String help = "";
            if (stage == 0)
                help = help1;
            else
                help = help2;
            Engine.spriteBatch.Draw(box, new Rectangle((int)(Game1.titleSafeRect.Left + xPercent * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)(boxWidth * Game1.titleSafeRect.Width), (int)(boxHeight * Game1.titleSafeRect.Width)), new Rectangle(0,0,512,64), Color.White);
            Engine.spriteBatch.DrawString(Engine.spriteFont, help, new Vector2(Game1.titleSafeRect.Left + textXPercent * Game1.titleSafeRect.Width, Game1.titleSafeRect.Top + textYPercent * Game1.titleSafeRect.Height), Color.YellowGreen);
            Engine.spriteBatch.End();
        }

        public bool Next()
        {
            if (cooldown > 0)
                return true;
            stage++;
            cooldown = 400;            
            if (stage > 1)
                return false;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            lifeTime += gameTime.ElapsedGameTime.Milliseconds;
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown < 0)
                cooldown = 0;
        }
    }
}

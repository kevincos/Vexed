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
    public class SpriteUtil
    {
        //public static float targetFontToScreenRatio = .0483f;
        public static float targetFontToScreenRatio = .0433f;
        public static float fontScale = 1f;
        public static SpriteFont currentFont;

        public static void SetBestFont()
        {
            // Compute ideal font height
            float screenHeight = Game1.titleSafeRect.Height;
            float targetTextHeight = targetFontToScreenRatio * screenHeight;
            //Select closest existing font
            float fontDifference = 10000f;
            SpriteFont fontCandidate;
            float newFontDifference;

            fontCandidate = DialogBox.smallFont;
            newFontDifference = Math.Abs(fontCandidate.MeasureString("X").Y - targetTextHeight);
            if (newFontDifference < fontDifference)
            {
                currentFont = fontCandidate;
                fontDifference = newFontDifference;
            }

            fontCandidate = DialogBox.largeFont;
            newFontDifference = Math.Abs(fontCandidate.MeasureString("X").Y - targetTextHeight);
            if (newFontDifference < fontDifference)
            {
                currentFont = fontCandidate;
                fontDifference = newFontDifference;
            }

            // Calculate scale correction
            fontScale = targetTextHeight / currentFont.MeasureString("X").Y;


        }

        public static void DrawString(SpriteBatch spriteBatch, String text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(currentFont, text, position, color,0f,Vector2.Zero,fontScale,SpriteEffects.None,0f);
        }

        public static String TextFit(String input, float boxLength)
        {
            // Purge of existing newline chars
            String revisedText = "";
            String currentLine = "";


            String[] wordList = input.Split(new char[] { ' ', '\n' });
            for (int j = 0; j < wordList.Length; j++)
            {
                if (wordList[j].Length == 0)
                    continue;
                
                if (currentFont.MeasureString(currentLine + ' ' + wordList[j]).X*fontScale > boxLength)
                {
                    revisedText += currentLine + "\n";
                    currentLine = "";
                }
                currentLine += wordList[j] + " ";
            }
            revisedText += currentLine + "\n";
            return revisedText;
        }

        public static String TextFit(String input, float boxLength, SpriteFont font)
        {
            // Purge of existing newline chars
            String revisedText = "";
            String currentLine = "";


            String[] wordList = input.Split(new char[] { ' ', '\n' });
            for (int j = 0; j < wordList.Length; j++)
            {
                if (wordList[j].Length == 0)
                    continue;

                if (font.MeasureString(currentLine + ' ' + wordList[j]).X > boxLength)
                {
                    revisedText += currentLine + "\n";
                    currentLine = "";
                }
                currentLine += wordList[j] + " ";
            }
            revisedText += currentLine + "\n";
            return revisedText;
        }


    }
}

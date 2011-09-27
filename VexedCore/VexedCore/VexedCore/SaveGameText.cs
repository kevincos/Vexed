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

    public class SaveGameText
    {
        public static RenderTarget2D textRenderTarget;

        public SaveGameText()
        {
            PresentationParameters pp = Game1.graphicsDevice.PresentationParameters;
            textRenderTarget = new RenderTarget2D(Game1.graphicsDevice,
                                                   256,128, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);
        }

        public void RenderTextures()
        {
            Game1.graphicsDevice.SetRenderTarget(textRenderTarget);
            Game1.graphicsDevice.Clear(Color.Transparent);
            Engine.spriteBatch.Begin();

            foreach (Doodad d in Engine.player.currentRoom.doodads)
            {
                if (d.type == VexedLib.DoodadType.LoadStation && d.active == true)
                {
                    int saveSlot = 0;
                    if(d.id.Contains("Slot1"))
                        saveSlot = 1;
                    if (d.id.Contains("Slot2"))
                        saveSlot = 2;
                    if (d.id.Contains("Slot3"))
                        saveSlot = 3;
                    if (d.id.Contains("Slot4"))
                        saveSlot = 4;

                    if (saveSlot < 3)
                    {
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Mission Log " + saveSlot, new Vector2(10, 5), Color.YellowGreen);
                        if(saveSlot == 2)
                            Engine.spriteBatch.DrawString(Engine.spriteFont, "Difficulty: Expert", new Vector2(10, 25), Color.YellowGreen);
                        else
                            Engine.spriteBatch.DrawString(Engine.spriteFont, "Difficulty: Normal", new Vector2(10, 25), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Completion: "+(18*saveSlot) +"%", new Vector2(10, 45), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Map Exploration: " + (12 * saveSlot) + "%", new Vector2(10, 65), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Equipment: " + (3 * saveSlot) + " / 23", new Vector2(10, 85), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Time: 0" + (4 * saveSlot) + ":" + (1 * saveSlot) + "3:0" + (3 * saveSlot) + "", new Vector2(10, 105), Color.YellowGreen);
                    }
                    else
                    {
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "New Game", new Vector2(10, 5), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Difficulty: -", new Vector2(10, 25), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Completion: 0%", new Vector2(10, 45), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Map Exploration: 0%", new Vector2(10, 65), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Equipment: 0 / 23", new Vector2(10, 85), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Time: 00:00:00", new Vector2(10, 105), Color.YellowGreen);
                    }
                }
            }

            Engine.spriteBatch.End();
            Game1.graphicsDevice.SetRenderTarget(null);
            foreach (Decoration d in Engine.player.currentRoom.decorations)
            {
                if (d.id.Contains("SaveInfo"))
                {
                    d.decorationTexture = textRenderTarget;
                    d.halfWidth = 7;
                    d.halfHeight = 3.5f;
                }
            }
        }

        public void Draw()
        {

        }
        
    }
}

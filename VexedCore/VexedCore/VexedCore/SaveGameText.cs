using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VexedCore
{
    public class SaveSummary
    {
        public bool empty = true;
        public bool expertMode = false;
        public int completionPercentage = 0;
        public int explorePercentage = 0;
        public int equipment = 0;
        public int gameTime = 0;
    }

    public class SaveGameText
    {
        public static RenderTarget2D textRenderTarget;

        public List<SaveSummary> saveSummaryData;

        public SaveGameText()
        {
            int totalOrbs = 0;
            for(int i = 0; i < LevelLoader.worldPreLoad.roomList.Count; i++)
            {
                totalOrbs += LevelLoader.worldPreLoad.roomList[i].maxOrbs;
            }
            saveSummaryData = new List<SaveSummary>();
            for (int saveSlot = 1; saveSlot < 5; saveSlot++)
            {
                SaveSummary summary = new SaveSummary();
                if (File.Exists("altSaveFile" + saveSlot))
                {
                    Stream stream = new FileStream("altSaveFile" + saveSlot, FileMode.Open, FileAccess.ReadWrite);
                    XmlSerializer serializer = new XmlSerializer(typeof(CompactSaveData));
                    CompactSaveData saveFile = (CompactSaveData)serializer.Deserialize(stream);
                    stream.Close();
                    for (int i = 0; i < saveFile.player.upgrades.Length; i++)
                    {
                        if(saveFile.player.upgrades[i] == true)
                        summary.equipment++;
                    }
                    
                    int playerOrbs = 0;
                    foreach (Rm r in saveFile.rmLst)
                    {
                        playerOrbs += r.co;
                    }
                    summary.completionPercentage = 100*playerOrbs / totalOrbs;
                    summary.empty = false;
                }
                saveSummaryData.Add(summary);
            }
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
                if (d.type == VL.DoodadType.LoadStation && d.active == true)
                {
                    int saveSlot = 0;
                    if(d.id.Contains("Slot1"))
                        saveSlot = 0;
                    if (d.id.Contains("Slot2"))
                        saveSlot = 1;
                    if (d.id.Contains("Slot3"))
                        saveSlot = 2;
                    if (d.id.Contains("Slot4"))
                        saveSlot = 3;

                    if(saveSummaryData[saveSlot].empty == true)
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "New Game", new Vector2(10, 5), Color.YellowGreen);
                    else
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Mission Log " + (saveSlot+1), new Vector2(10, 5), Color.YellowGreen);

                    if(saveSummaryData[saveSlot].expertMode == true)
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Difficulty: Expert", new Vector2(10, 25), Color.YellowGreen);
                    else if(saveSummaryData[saveSlot].empty == true)
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Difficulty: -", new Vector2(10, 25), Color.YellowGreen);
                    else
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "Difficulty: Normal", new Vector2(10, 25), Color.YellowGreen);

                    Engine.spriteBatch.DrawString(Engine.spriteFont, "Completion: " + saveSummaryData[saveSlot].completionPercentage + "%", new Vector2(10, 45), Color.YellowGreen);
                    Engine.spriteBatch.DrawString(Engine.spriteFont, "Map Exploration: " + saveSummaryData[saveSlot].explorePercentage + "%", new Vector2(10, 65), Color.YellowGreen);
                    Engine.spriteBatch.DrawString(Engine.spriteFont, "Equipment: " + saveSummaryData[saveSlot].equipment + " / 23", new Vector2(10, 85), Color.YellowGreen);
                    Engine.spriteBatch.DrawString(Engine.spriteFont, "Time: " + TimeSpan.FromMilliseconds(saveSummaryData[saveSlot].gameTime), new Vector2(10, 105), Color.YellowGreen);
                    
                }
            }

            Engine.spriteBatch.End();
            Game1.graphicsDevice.SetRenderTarget(null);
            foreach (Decoration d in Engine.player.currentRoom.decorations)
            {
                if (d.id.Contains("SaveInfo"))
                {
                    if (d.decorationTexture == null)
                    {
                        d.decorationTexture.Add(textRenderTarget);
                    }
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

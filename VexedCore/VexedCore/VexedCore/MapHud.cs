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
    class MapHud
    {
        public static Texture2D hudLeft;
        public static Texture2D hudRight;
        public static Texture2D leftArrow;
        public static Texture2D rightArrow;
        public static Texture2D mapDataMonitor;
        public static Texture2D mapObjectiveMonitor;
        public static Texture2D inventoryDataMonitor;
        public static Texture2D inventoryListMonitor;

        public static List<int> inventoryIndexList;

        public static bool hiddenFrame = true;
        public static bool hiddenMap = true;
        public static bool hiddenInventory = true;
        public static int hiddenFrameOffsetTime = 0;
        public static int hiddenFrameMaxOffsetTime = 1000; 
        public static int hiddenMapOffsetTime = 0;
        public static int hiddenMapMaxOffsetTime = 1000;
        public static int hiddenInventoryOffsetTime = 1200;
        public static int hiddenInventoryMaxOffsetTime = 1200;

        public static int inventoryListIncrement = 0;
        public static int inventoryListTop;
        public static int inventoryListBottom;
        public static int inventoryListLeft;
        public static int inventoryListWidth;
        

        public static SpriteFont smallFont;
        public static SpriteFont largeFont;

        public static SpriteFont currentFont
        {
            get
            {
                if (Engine.res == ResolutionSettings.R_1920x1080)
                {
                    return largeFont;
                }
                return smallFont;
            }
        }

        public static void Draw()
        {
            Engine.spriteBatch.Begin();


            // Map Monitors
            int w = Game1.titleSafeRect.Width;
            int monitorTop = Game1.titleSafeRect.Top - hiddenMapOffsetTime;// +hiddenInventoryOffsetTime - hiddenInventoryMaxOffsetTime;
            int objectiveTop = Game1.titleSafeRect.Top - hiddenMapOffsetTime;
            int objectiveLeft = Game1.titleSafeRect.Center.X;
            int monitorLeft = Game1.titleSafeRect.Center.X - w/2;
            

            int rightEdge = Game1.titleSafeRect.Right;
            int topEdge = Game1.titleSafeRect.Top;
            int bottomEdge = Game1.titleSafeRect.Bottom;
            int hudSideWidth = (int)(.1f * w);
            int hudRightSide = Game1.titleSafeRect.Right - hudSideWidth + hiddenFrameOffsetTime;
            int hudLeftSide = Game1.titleSafeRect.Left - hiddenFrameOffsetTime;
            int dataMonitorWidth = w / 2;
            int dataMonitorHeight = w / 4;
            int objectiveMonitorHeight = w / 8;
            int objectiveMonitorWidth = w / 2;


            Engine.spriteBatch.Draw(mapDataMonitor, new Rectangle(monitorLeft, monitorTop, dataMonitorWidth, dataMonitorHeight), Color.White);
            Engine.spriteBatch.Draw(mapObjectiveMonitor, new Rectangle(objectiveLeft, objectiveTop, objectiveMonitorWidth, objectiveMonitorHeight), Color.White);



            // Inventory Monitors
            inventoryListTop = Game1.titleSafeRect.Top + hiddenInventoryOffsetTime;
            inventoryListBottom = Game1.titleSafeRect.Bottom + hiddenInventoryOffsetTime;
            inventoryListLeft = Game1.titleSafeRect.Center.X - w/2;
            inventoryListWidth = w / 2;

            int inventoryDataTop = Game1.titleSafeRect.Top + objectiveMonitorHeight + hiddenInventoryOffsetTime;
            int inventoryDataBottom = Game1.titleSafeRect.Bottom + hiddenInventoryOffsetTime;
            int inventoryDataLeft = objectiveLeft;
            int inventoryDataRight = rightEdge;

            

            // Sides


            Engine.spriteBatch.Draw(hudLeft, new Rectangle(hudLeftSide, topEdge, hudSideWidth, bottomEdge - topEdge), Color.White);
            Engine.spriteBatch.Draw(hudRight, new Rectangle(hudRightSide, topEdge, hudSideWidth, bottomEdge - topEdge), Color.White);

            Engine.spriteBatch.Draw(inventoryListMonitor, new Rectangle(inventoryListLeft, inventoryListTop, inventoryListWidth, inventoryListBottom - inventoryListTop), Color.White);
            Engine.spriteBatch.Draw(inventoryDataMonitor, new Rectangle(inventoryDataLeft, inventoryDataTop, inventoryDataRight - inventoryDataLeft, inventoryDataBottom - inventoryDataTop), Color.White);


            if (WorldMap.warp == false)
            {
                Color leftArrowColor = Color.Gray;
                Color rightArrowColor = Color.Gray;
                if ((Mouse.GetState().X > hudRightSide && Mouse.GetState().X < hudRightSide + hudSideWidth))
                {
                    rightArrowColor = Color.Yellow;
                }
                if ((Mouse.GetState().X > hudLeftSide && Mouse.GetState().X < hudLeftSide + hudSideWidth))
                {
                    leftArrowColor = Color.Yellow;
                }
                if (WorldMap.state == ZoomState.Sector || WorldMap.state == ZoomState.Inventory)
                {
                    Engine.spriteBatch.Draw(leftArrow, new Rectangle(hudLeftSide, topEdge, hudSideWidth, bottomEdge - topEdge), leftArrowColor);
                }
                if (WorldMap.state == ZoomState.Sector || WorldMap.state == ZoomState.World)
                {
                    Engine.spriteBatch.Draw(rightArrow, new Rectangle(hudRightSide, topEdge, hudSideWidth, bottomEdge - topEdge), rightArrowColor);
                }
            }
            
            
            SpriteFont spriteFont = currentFont;




            if (WorldMap.state == ZoomState.Sector)
            {
                Room r = Engine.roomList[WorldMap.selectedRoomIndex];
                Sector s = r.parentSector;
                String outputStringTitle = "";
                String outputStringBase = "";
                if (r.explored)
                {
                    outputStringTitle += r.id;
                }
                else
                {
                    outputStringTitle += "???";
                }
                outputStringBase += "Power Level: " + r.currentOrbs + " / " + r.maxOrbs;


                if (r.hasWarp == true)
                {
                    outputStringBase += "\nWarp Access: " + s.currentBlueOrbs + " / " + r.warpCost;
                }
                else
                {
                    outputStringBase += "\nWarp Cubes: " + r.currentBlueOrbs + " / " + r.maxBlueOrbs;
                    outputStringBase += "\nHealth Cubes: " + r.currentRedOrbs + " / " + r.maxRedOrbs;
                }

                Engine.spriteBatch.DrawString(spriteFont, outputStringTitle, new Vector2(monitorLeft + .137f * w, monitorTop + .027f * w), Color.YellowGreen);
                Engine.spriteBatch.DrawString(spriteFont, outputStringBase, new Vector2(monitorLeft + .167f * w, monitorTop + .062f * w), Color.YellowGreen);
            }
            if (WorldMap.state == ZoomState.World)
            {
                Sector s = Engine.sectorList[WorldMap.selectedSectorIndex];

                String outputStringTitle = "";
                String outputStringBase = "";
                outputStringTitle+= "Sector: " + s.id;
                outputStringBase += "Power Level: " + s.currentOrbs + " / " + s.maxOrbs;
                outputStringBase += "\nWarp Level: " + s.currentBlueOrbs + " / " + s.maxBlueOrbs;
                outputStringBase += "\nRed Cubes: " + s.currentRedOrbs + " / " + s.maxRedOrbs;

                Engine.spriteBatch.DrawString(spriteFont, outputStringTitle, new Vector2(monitorLeft + .137f * w, monitorTop + .027f * w), Color.YellowGreen);
                Engine.spriteBatch.DrawString(spriteFont, outputStringBase, new Vector2(monitorLeft + .167f * w, monitorTop + .062f * w), Color.YellowGreen);
            }
            if (inventoryIndexList == null)
            {
                inventoryIndexList = new List<int>();
                inventoryIndexList.Add(0);
                inventoryIndexList.Add(0);
                for (int i = 0; i < Engine.player.upgrades.Length; i++)
                {
                    Ability a = new Ability();
                    a.type = (AbilityType)i;
                    if (a.isItem)
                        inventoryIndexList.Add(i);
                }
                for (int i = 0; i < Engine.player.upgrades.Length; i++)
                {
                    Ability a = new Ability();
                    a.type = (AbilityType)i;
                    if (a.isUpgrade)
                        inventoryIndexList.Add(i);
                }

            }
            inventoryIndexList[0] = (int)Engine.player.primaryAbility.type;
            inventoryIndexList[1] = (int)Engine.player.secondaryAbility.type;
            if (WorldMap.state == ZoomState.Inventory)
            {
                int drawOffset = 5;
                inventoryListIncrement = (inventoryListBottom - inventoryListTop) / (28);
                int increment = inventoryListIncrement;

                Engine.spriteBatch.DrawString(spriteFont, "Inventory", new Vector2(monitorLeft + .127f * w, inventoryListTop + 4 * increment), Color.YellowGreen);                


                Engine.spriteBatch.DrawString(spriteFont, "Items Unlocked", new Vector2(inventoryListLeft + .127f * w, inventoryListTop + 8 * increment), Color.YellowGreen);
                Engine.spriteBatch.DrawString(spriteFont, "Suit Upgrades", new Vector2(inventoryListLeft + .127f * w, inventoryListTop + 20 * increment), Color.YellowGreen);

                for (int i = 0; i < inventoryIndexList.Count; i++)
                {
                    Ability a = new Ability();
                    a.type = (AbilityType)inventoryIndexList[i];
                    if (WorldMap.selectedInventory == i)
                    {
                        Engine.spriteBatch.DrawString(spriteFont, "X", new Vector2(inventoryListLeft + .127f * w, inventoryListTop + increment * drawOffset), Color.YellowGreen);

                        if (Engine.player.upgrades[inventoryIndexList[i]] == true)
                        {
                            Engine.spriteBatch.DrawString(spriteFont, a.FriendlyName(), new Vector2(inventoryDataLeft + .01f * w, inventoryDataTop + .08f * w), Color.YellowGreen);
                            Engine.spriteBatch.DrawString(spriteFont, DialogChunk.TextFit(a.Description(), inventoryDataRight - inventoryDataLeft - .08f * w, spriteFont), new Vector2(inventoryDataLeft + .01f * w, inventoryDataTop + .15f * w), Color.YellowGreen);
                        }

                    }
                    if (Engine.player.upgrades[inventoryIndexList[i]] == true)
                    {
                        Engine.spriteBatch.DrawString(spriteFont, "  " + a.FriendlyName(), new Vector2(inventoryListLeft + .140f * w, inventoryListTop + increment * drawOffset), Color.YellowGreen);
                    }
                    else
                    {
                        Engine.spriteBatch.DrawString(spriteFont, "  ???", new Vector2(inventoryListLeft + .140f * w, inventoryListTop + increment * drawOffset), Color.YellowGreen);
                    }
                    drawOffset += 1;
                    if (i == 1)
                        drawOffset += 2;
                    if (i == 11)
                        drawOffset += 2;
                }

            }

            if (WorldMap.state != ZoomState.None)
            {
                Engine.spriteBatch.DrawString(spriteFont, "Current Objective: ", new Vector2(objectiveLeft + .037f * w, objectiveTop + .027f * w), Color.YellowGreen);

                Engine.spriteBatch.DrawString(spriteFont, ObjectiveControl.objectives[Engine.player.currentObjective].text, new Vector2(objectiveLeft + .067f * w, objectiveTop + .062f * w), Color.YellowGreen);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) == false && WorldMap.state != ZoomState.None)
            {
                Vector2 mousePos = new Vector2(Mouse.GetState().X - 32, Mouse.GetState().Y);
                Engine.spriteBatch.Draw(PauseMenu.mouseCursor, mousePos, Color.YellowGreen);
            }
            Engine.spriteBatch.End();
        }

        public static void Update(int gameTime)
        {
            if (WorldMap.state == ZoomState.Inventory)
            {
                hiddenInventory = false;
            }
            else
            {
                hiddenInventory = true;
            }
            if (hiddenMap && hiddenMapOffsetTime < hiddenMapMaxOffsetTime)
            {
                hiddenMapOffsetTime += gameTime;
                if (hiddenMapOffsetTime > hiddenMapMaxOffsetTime) hiddenMapOffsetTime = hiddenMapMaxOffsetTime;
            }
            if (hiddenMap == false && hiddenMapOffsetTime > 0)
            {
                hiddenMapOffsetTime -= gameTime;
                if (hiddenMapOffsetTime < 0) hiddenMapOffsetTime = 0;
            }

            if (hiddenInventory && hiddenInventoryOffsetTime < hiddenInventoryMaxOffsetTime)
            {
                hiddenInventoryOffsetTime += gameTime;
                if (hiddenInventoryOffsetTime > hiddenInventoryMaxOffsetTime) hiddenInventoryOffsetTime = hiddenInventoryMaxOffsetTime;
            }
            if (hiddenInventory == false && hiddenInventoryOffsetTime > 0)
            {
                hiddenInventoryOffsetTime -= gameTime;
                if (hiddenInventoryOffsetTime < 0) hiddenInventoryOffsetTime = 0;
            }

            if (hiddenFrame && hiddenFrameOffsetTime < hiddenFrameMaxOffsetTime)
            {
                hiddenFrameOffsetTime += gameTime;
                if (hiddenFrameOffsetTime > hiddenFrameMaxOffsetTime) hiddenFrameOffsetTime = hiddenFrameMaxOffsetTime;
            }
            if (hiddenFrame == false && hiddenFrameOffsetTime > 0)
            {
                hiddenFrameOffsetTime -= gameTime;
                if (hiddenFrameOffsetTime < 0) hiddenFrameOffsetTime = 0;
            }
        }
    }
}

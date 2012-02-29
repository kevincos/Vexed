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
        public static Texture2D mapFilterMonitor;
        public static Texture2D inventoryDataMonitor;
        public static Texture2D inventoryListMonitor;

        public static List<int> inventoryIndexList;        

        public static bool hiddenFrame = true;
        public static bool hiddenMap = true;
        public static bool hiddenInventory = true;
        public static bool hiddenObjective = true;
        public static int hiddenFrameOffsetTime = 0;
        public static int hiddenFrameMaxOffsetTime = 1000; 
        public static int hiddenMapOffsetTime = 0;
        public static int hiddenMapMaxOffsetTime = 1000;
        public static int hiddenInventoryOffsetTime = 1200;
        public static int hiddenInventoryMaxOffsetTime = 1200;
        public static int hiddenObjectiveOffsetTime = 1200;
        public static int hiddenObjectiveMaxOffsetTime = 1200;
        public static int filterCooldown;
       

        public static int inventoryListIncrement = 0;
        public static int inventoryListTop;
        public static int inventoryListBottom;
        public static int inventoryListLeft;
        public static int inventoryListWidth;

        public static int objectiveListIncrement = 0;
        public static int objectiveListTop;
        public static int objectiveListBottom;
        public static int objectiveListLeft;
        public static int objectiveListWidth;


        public static int hudLeftSide;
        public static int hudRightSide;
        public static int hudSideWidth;
        public static int filterMonitorWidth;
        public static int filterMonitorHeight;
        public static int filterMonitorTop;
        public static int filterMonitorLeft;
        public static int filterMonitorMax;
        public static int filterMonitor3;
        public static int filterMonitor2;
        public static int filterMonitor1;
        public static Vector2 mousePos;

        public static SpriteFont smallFont;
        public static SpriteFont largeFont;

        public static bool hudClick = false;

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

            SpriteFont spriteFont = currentFont;
            // Map Monitors
            int w = Game1.titleSafeRect.Width;
            int h = Game1.titleSafeRect.Height;
            int monitorTop = Game1.titleSafeRect.Top - hiddenMapOffsetTime;
            int objectiveTop = Game1.titleSafeRect.Top - hiddenMapOffsetTime;
            int objectiveLeft = Game1.titleSafeRect.Center.X;
            int monitorLeft = Game1.titleSafeRect.Center.X - w/2;
            

            int rightEdge = Game1.titleSafeRect.Right;
            int topEdge = Game1.titleSafeRect.Top;
            int bottomEdge = Game1.titleSafeRect.Bottom;
            hudSideWidth = (int)(.1f * w);
            hudRightSide = Game1.titleSafeRect.Right - hudSideWidth + hiddenFrameOffsetTime;
            hudLeftSide = Game1.titleSafeRect.Left - hiddenFrameOffsetTime;
            int dataMonitorWidth = w / 2;
            int dataMonitorHeight = h / 4;
            int objectiveMonitorHeight = h / 6;
            int objectiveMonitorWidth = w / 2;
            filterMonitorWidth = 4*w / 5;
            filterMonitorHeight = h / 6;
            int filterMonitorBottom  = bottomEdge + hiddenMapOffsetTime;
            filterMonitorTop = filterMonitorBottom - filterMonitorHeight;
            filterMonitorLeft = rightEdge - filterMonitorWidth;
            filterMonitor1 = (int)(filterMonitorLeft + .08f * filterMonitorWidth);
            filterMonitor2 = (int)(filterMonitorLeft + .32f * filterMonitorWidth);
            filterMonitor3 = (int)(filterMonitorLeft + .6f * filterMonitorWidth);
            filterMonitorMax = (int)(filterMonitorLeft + .9f * filterMonitorWidth);

            Engine.spriteBatch.Draw(mapDataMonitor, new Rectangle(monitorLeft, monitorTop, dataMonitorWidth, dataMonitorHeight), Color.White);
            Engine.spriteBatch.Draw(mapObjectiveMonitor, new Rectangle(objectiveLeft, objectiveTop, objectiveMonitorWidth, objectiveMonitorHeight), Color.White);
            
            // Filter Monitor
            Engine.spriteBatch.Draw(mapFilterMonitor, new Rectangle(rightEdge - filterMonitorWidth, filterMonitorBottom - filterMonitorHeight, filterMonitorWidth, filterMonitorHeight), Color.White);

            Color filterColor = Color.YellowGreen;

            filterColor = Color.YellowGreen;
            if (Engine.player.objectiveFilter)
                filterColor = new Color(80, 80, 80);
            //Engine.spriteBatch.DrawString(spriteFont, "Objectives", new Vector2(filterMonitor1, filterMonitorTop + .045f * h), filterColor);
            SpriteUtil.DrawString(Engine.spriteBatch, "Objectives", new Vector2(filterMonitor1, filterMonitorTop + .045f * h), filterColor);

            filterColor = Color.YellowGreen;
            if (Engine.player.stationFilter)
                filterColor = new Color(80, 80, 80);
            SpriteUtil.DrawString(Engine.spriteBatch, "Waypoints", new Vector2(filterMonitor1, filterMonitorTop + .085f * h), filterColor);

            filterColor = Color.YellowGreen;
            if (Engine.player.saveFilter)
                filterColor = new Color(80, 80, 80);
            SpriteUtil.DrawString(Engine.spriteBatch, "Save Stations", new Vector2(filterMonitor2, filterMonitorTop + .045f * h), filterColor);

            filterColor = Color.YellowGreen;
            if (Engine.player.itemFilter)
                filterColor = new Color(80, 80, 80);
            SpriteUtil.DrawString(Engine.spriteBatch, "Item Stations", new Vector2(filterMonitor2, filterMonitorTop + .085f * h), filterColor);

            filterColor = Color.YellowGreen;
            if (Engine.player.healthFilter)
                filterColor = new Color(80, 80, 80);
            SpriteUtil.DrawString(Engine.spriteBatch, "Health Stations", new Vector2(filterMonitor3, filterMonitorTop + .045f * h), filterColor);

            filterColor = Color.YellowGreen;
            if (Engine.player.warpFilter)
                filterColor = new Color(80, 80, 80);
            SpriteUtil.DrawString(Engine.spriteBatch, "Warp Nodes", new Vector2(filterMonitor3, filterMonitorTop + .085f * h), filterColor);


            // Inventory Monitors
            inventoryListTop = Game1.titleSafeRect.Top + hiddenInventoryOffsetTime;
            inventoryListBottom = Game1.titleSafeRect.Bottom + hiddenInventoryOffsetTime;
            inventoryListLeft = Game1.titleSafeRect.Center.X - w/2;
            inventoryListWidth = w / 2;

            int inventoryDataTop = Game1.titleSafeRect.Top + objectiveMonitorHeight + hiddenInventoryOffsetTime;
            int inventoryDataBottom = Game1.titleSafeRect.Bottom + hiddenInventoryOffsetTime;
            int inventoryDataLeft = objectiveLeft;
            int inventoryDataRight = rightEdge;
            
            // Objective Monitors
            objectiveListTop = Game1.titleSafeRect.Top + hiddenObjectiveOffsetTime;
            objectiveListBottom = Game1.titleSafeRect.Bottom + hiddenObjectiveOffsetTime;
            objectiveListLeft = Game1.titleSafeRect.Center.X - w / 2 - hudSideWidth;
            objectiveListWidth = w / 2 + hudSideWidth;

            int objectiveDataTop = Game1.titleSafeRect.Top + objectiveMonitorHeight + hiddenObjectiveOffsetTime;
            int objectiveDataBottom = Game1.titleSafeRect.Bottom + hiddenObjectiveOffsetTime;
            int objectiveDataLeft = objectiveLeft;
            int objectiveDataRight = rightEdge - hudSideWidth;


            

            // Sides
            Engine.spriteBatch.Draw(hudLeft, new Rectangle(hudLeftSide, topEdge, hudSideWidth, bottomEdge - topEdge), Color.White);
            Engine.spriteBatch.Draw(hudRight, new Rectangle(hudRightSide, topEdge, hudSideWidth, bottomEdge - topEdge), Color.White);


            Engine.spriteBatch.Draw(inventoryListMonitor, new Rectangle(inventoryListLeft, inventoryListTop, inventoryListWidth, inventoryListBottom - inventoryListTop), Color.White);
            Engine.spriteBatch.Draw(inventoryDataMonitor, new Rectangle(inventoryDataLeft, inventoryDataTop, inventoryDataRight - inventoryDataLeft, inventoryDataBottom - inventoryDataTop), Color.White);

            Engine.spriteBatch.Draw(inventoryListMonitor, new Rectangle(objectiveListLeft, objectiveListTop, objectiveListWidth, objectiveListBottom - objectiveListTop), Color.White);
            Engine.spriteBatch.Draw(inventoryDataMonitor, new Rectangle(objectiveDataLeft, objectiveDataTop, objectiveDataRight - objectiveDataLeft, objectiveDataBottom - objectiveDataTop), Color.White);


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
                if (WorldMap.state == ZoomState.Sector || WorldMap.state == ZoomState.Inventory || WorldMap.state == ZoomState.World)
                {
                    Engine.spriteBatch.Draw(leftArrow, new Rectangle(hudLeftSide, topEdge, hudSideWidth, bottomEdge - topEdge), leftArrowColor);
                }
                if (WorldMap.state == ZoomState.Sector || WorldMap.state == ZoomState.World || WorldMap.state == ZoomState.Objectives)
                {
                    Engine.spriteBatch.Draw(rightArrow, new Rectangle(hudRightSide, topEdge, hudSideWidth, bottomEdge - topEdge), rightArrowColor);
                }
            }
            
            
            




            if (WorldMap.state == ZoomState.Sector)
            {
                Room r = Engine.roomList[WorldMap.selectedRoomIndex];
                Sector s = r.parentSector;
                String outputStringTitle = "";
                String outputStringBase = "";
                if (r.explored)
                {
                    outputStringTitle += r.friendlyName;
                }
                else
                {
                    outputStringTitle += "???";
                }
                outputStringBase += "Power Level: " + r.currentOrbs + " / " + r.maxOrbs;


                if (r.hasWarp == true && WorldMap.warp == true)
                {
                    outputStringBase += "\nWarp Access: " + s.currentBlueOrbs + " / " + r.warpCost;
                }
                else
                {
                    outputStringBase += "\nWarp Cubes: " + r.currentBlueOrbs + " / " + r.maxBlueOrbs;
                    outputStringBase += "\nHealth Cubes: " + r.currentRedOrbs + " / " + r.maxRedOrbs;
                }

                SpriteUtil.DrawString(Engine.spriteBatch, outputStringTitle, new Vector2(monitorLeft + .137f * w, monitorTop + .032f * h), Color.YellowGreen);
                SpriteUtil.DrawString(Engine.spriteBatch, outputStringBase, new Vector2(monitorLeft + .167f * w, monitorTop + .074f * h), Color.YellowGreen);

                
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

                SpriteUtil.DrawString(Engine.spriteBatch, outputStringTitle, new Vector2(monitorLeft + .137f * w, monitorTop + .032f * h), Color.YellowGreen);
                SpriteUtil.DrawString(Engine.spriteBatch, outputStringBase, new Vector2(monitorLeft + .167f * w, monitorTop + .074f * h), Color.YellowGreen);
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

                SpriteUtil.DrawString(Engine.spriteBatch, "Inventory", new Vector2(monitorLeft + .127f * w, inventoryListTop + 4 * increment), Color.YellowGreen);                


                SpriteUtil.DrawString(Engine.spriteBatch, "Items Unlocked", new Vector2(inventoryListLeft + .127f * w, inventoryListTop + 8 * increment), Color.YellowGreen);
                SpriteUtil.DrawString(Engine.spriteBatch, "Suit Upgrades", new Vector2(inventoryListLeft + .127f * w, inventoryListTop + 20 * increment), Color.YellowGreen);

                for (int i = 0; i < inventoryIndexList.Count; i++)
                {
                    Ability a = new Ability();
                    a.type = (AbilityType)inventoryIndexList[i];
                    if (WorldMap.selectedInventory == i)
                    {
                        SpriteUtil.DrawString(Engine.spriteBatch, "X", new Vector2(inventoryListLeft + .127f * w, inventoryListTop + increment * drawOffset), Color.YellowGreen);

                        if (Engine.player.upgrades[inventoryIndexList[i]] == true)
                        {
                            SpriteUtil.DrawString(Engine.spriteBatch, a.FriendlyName(), new Vector2(inventoryDataLeft + .01f * w, inventoryDataTop + .12f * h), Color.YellowGreen);
                            SpriteUtil.DrawString(Engine.spriteBatch, SpriteUtil.TextFit(a.Description(), inventoryDataRight - inventoryDataLeft - .12f * h), new Vector2(inventoryDataLeft + .01f * w, inventoryDataTop + .15f * w), Color.YellowGreen);
                        }

                    }
                    if (Engine.player.upgrades[inventoryIndexList[i]] == true)
                    {
                        SpriteUtil.DrawString(Engine.spriteBatch, "  " + a.FriendlyName(), new Vector2(inventoryListLeft + .140f * w, inventoryListTop + increment * drawOffset), Color.YellowGreen);
                    }
                    else
                    {
                        SpriteUtil.DrawString(Engine.spriteBatch, "  ???", new Vector2(inventoryListLeft + .140f * w, inventoryListTop + increment * drawOffset), Color.YellowGreen);
                    }
                    drawOffset += 1;
                    if (i == 1)
                        drawOffset += 2;
                    if (i == 11)
                        drawOffset += 2;
                }

            }

            if (WorldMap.state == ZoomState.Objectives)
            {
                int drawOffset = 5;
                objectiveListIncrement = (objectiveListBottom - objectiveListTop) / (28);
                int increment = objectiveListIncrement;

                SpriteUtil.DrawString(Engine.spriteBatch, "Objectives", new Vector2(objectiveListLeft + .167f * w, objectiveListTop + 4 * increment), Color.YellowGreen);

                for (int i = 0; i < ObjectiveControl.objectives.Count; i++)
                {
                    if (WorldMap.selectedObjective == i)
                    {
                        SpriteUtil.DrawString(Engine.spriteBatch, "X", new Vector2(objectiveListLeft + .167f * w, objectiveListTop + increment * drawOffset), Color.YellowGreen);

                        if(Engine.player.currentObjective >= i)
                        {
                            SpriteUtil.DrawString(Engine.spriteBatch, ObjectiveControl.objectives[i].shorttext, new Vector2(objectiveDataLeft + .01f * w, objectiveDataTop + .12f * h), Color.YellowGreen);
                            SpriteUtil.DrawString(Engine.spriteBatch, SpriteUtil.TextFit(ObjectiveControl.objectives[i].longtext, objectiveDataRight - objectiveDataLeft - .12f * h), new Vector2(objectiveDataLeft + .01f * w, objectiveDataTop + .15f * w), Color.YellowGreen);
                        }

                    }
                    if (i <= Engine.player.currentObjective == true)
                    {
                        SpriteUtil.DrawString(Engine.spriteBatch, "  " + ObjectiveControl.objectives[i].shorttext, new Vector2(objectiveListLeft + .180f * w, objectiveListTop + increment * drawOffset), Color.YellowGreen);
                    }
                    else
                    {
                        SpriteUtil.DrawString(Engine.spriteBatch, "  ???", new Vector2(objectiveListLeft + .180f * w, objectiveListTop + increment * drawOffset), Color.YellowGreen);
                    }
                    drawOffset++;
                }

            }

            if (WorldMap.state != ZoomState.None)
            {
                SpriteUtil.DrawString(Engine.spriteBatch, "Current Objective: ", new Vector2(objectiveLeft + .037f * w, objectiveTop + .037f * h), Color.YellowGreen);

                SpriteUtil.DrawString(Engine.spriteBatch, ObjectiveControl.objectives[Engine.player.currentObjective].text, new Vector2(objectiveLeft + .067f * w, objectiveTop + .082f * h), Color.YellowGreen);
            }
            mousePos = new Vector2(Mouse.GetState().X - 32, Mouse.GetState().Y);
            if (WorldMap.state != ZoomState.None)
            {
                
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
            if (WorldMap.state == ZoomState.Objectives)
            {
                hiddenObjective = false;
            }
            else
            {
                hiddenObjective = true;
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

            if (hiddenObjective && hiddenObjectiveOffsetTime < hiddenObjectiveMaxOffsetTime)
            {
                hiddenObjectiveOffsetTime += gameTime;
                if (hiddenObjectiveOffsetTime > hiddenObjectiveMaxOffsetTime) hiddenObjectiveOffsetTime = hiddenObjectiveMaxOffsetTime;
            }
            if (hiddenObjective == false && hiddenObjectiveOffsetTime > 0)
            {
                hiddenObjectiveOffsetTime -= gameTime;
                if (hiddenObjectiveOffsetTime < 0) hiddenObjectiveOffsetTime = 0;
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

            filterCooldown -= gameTime;
            if (filterCooldown < 0) filterCooldown = 0;
            hudClick = false;
            if ((WorldMap.state == ZoomState.Sector || WorldMap.state == ZoomState.World) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                
                if (mousePos.X > filterMonitorLeft && mousePos.Y > filterMonitorTop)
                {
                    hudClick = true;
                    if (filterCooldown == 0 && mousePos.X < filterMonitorMax && mousePos.X > filterMonitor1)
                    {
                        if (mousePos.X > filterMonitor3)
                        {
                            if (mousePos.Y > filterMonitorTop + filterMonitorHeight / 2)
                            {
                                Engine.player.warpFilter = !Engine.player.warpFilter;
                                SoundFX.MapSelect();
                                filterCooldown = 300;
                            }
                            else
                            {
                                Engine.player.healthFilter = !Engine.player.healthFilter;
                                SoundFX.MapSelect();
                                filterCooldown = 300;
                            }
                        }
                        else if (mousePos.X > filterMonitor2)
                        {
                            if (mousePos.Y > filterMonitorTop + filterMonitorHeight / 2)
                            {
                                Engine.player.itemFilter = !Engine.player.itemFilter;
                                SoundFX.MapSelect();
                                filterCooldown = 300;
                            }
                            else
                            {
                                Engine.player.saveFilter = !Engine.player.saveFilter;
                                SoundFX.MapSelect();
                                filterCooldown = 300;
                            }
                        }
                        else
                        {
                            if (mousePos.Y > filterMonitorTop + filterMonitorHeight / 2)
                            {
                                Engine.player.stationFilter = !Engine.player.stationFilter;
                                SoundFX.MapSelect();
                                filterCooldown = 300;
                            }
                            else
                            {
                                Engine.player.objectiveFilter = !Engine.player.objectiveFilter;
                                SoundFX.MapSelect();
                                filterCooldown = 300;
                            }
                        }
                    }
                }
            }
        }
    }
}

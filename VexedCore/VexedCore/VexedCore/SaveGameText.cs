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
        public int redOrbs = 0;
        public int blueOrbs = 0;
        public int totalRedOrbs = 0;
        public int totalBlueOrbs = 0;
        public int equipment = 0;
        public int gameTime = 0;
        public List<int> roomOrbs;
    }

    public enum LoadState
    {
        Normal,
        ConfirmNewGame,
        ConfirmNewGameExpert,
        ConfirmContinue,
        ConfirmClear,        
    }

    public class SaveGameText
    {
        public static RenderTarget2D textRenderTarget;
        public static RenderTarget2D mapRenderTarget;

        public static Texture2D confirmOptions;
        public static Texture2D loadOptions;

        public static List<Vector2> okTexCoords;
        public static List<Vector2> cancelTexCoords;
        public static List<Vector2> extraTexCoords;

        public static float mapRotate = .3f;

        public static LoadState state = LoadState.Normal;
        public static bool expertAvailable = true;

        public static int activeSaveSlot = -1;

        public static List<SaveSummary> saveSummaryData;

        public SaveGameText()
        {
            GenerateSummaries();
            PresentationParameters pp = Game1.graphicsDevice.PresentationParameters;
            textRenderTarget = new RenderTarget2D(Game1.graphicsDevice,
                                                   440,180, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);
            mapRenderTarget = new RenderTarget2D(Game1.graphicsDevice,
                                                   1024,768, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);
        }

        public static void GenerateSummaries()
        {
            int totalOrbs = 0;
            int totalRedOrbs = 0;
            int totalBlueOrbs = 0;
            for (int i = 0; i < LevelLoader.worldPreLoad.roomList.Count; i++)
            {
                totalOrbs += LevelLoader.worldPreLoad.roomList[i].maxOrbs;
                totalRedOrbs += LevelLoader.worldPreLoad.roomList[i].maxRedOrbs;
                totalBlueOrbs += LevelLoader.worldPreLoad.roomList[i].maxBlueOrbs;
            }
            saveSummaryData = new List<SaveSummary>();            
            for (int saveSlot = 1; saveSlot < 5; saveSlot++)
            {
                SaveSummary summary = new SaveSummary();
                summary.roomOrbs = new List<int>();
                summary.totalBlueOrbs = totalBlueOrbs;
                summary.totalRedOrbs = totalRedOrbs;
                
                if (File.Exists("altSaveFile" + saveSlot))
                {
                    Stream stream = new FileStream("altSaveFile" + saveSlot, FileMode.Open, FileAccess.ReadWrite);
                    XmlSerializer serializer = new XmlSerializer(typeof(CompactSaveData));
                    CompactSaveData saveFile = (CompactSaveData)serializer.Deserialize(stream);
                    stream.Close();
                    for (int i = 0; i < saveFile.player.upgrades.Length; i++)
                    {
                        if (saveFile.player.upgrades[i] == true)
                            summary.equipment++;
                    }

                    int playerOrbs = 0;
                    int playerRedOrbs = 0;
                    int playerBlueOrbs = 0;
                    int playerRoomsExplored = 0;
                    for(int i = 0; i < saveFile.rmLst.Count; i++)
                    {
                        Rm r = saveFile.rmLst[i];
                        summary.roomOrbs.Add(r.co);
                        playerOrbs += r.co;
                        playerRedOrbs += r.cro;
                        playerBlueOrbs += r.cbo;
                        if(r.e)
                            playerRoomsExplored++;
                        
                    }
                    summary.redOrbs = playerRedOrbs;
                    summary.blueOrbs = playerBlueOrbs;
                    summary.completionPercentage = 100 * playerOrbs / totalOrbs;
                    summary.gameTime = saveFile.player.totalGameTime;
                    summary.empty = false;
                    summary.expertMode = saveFile.player.expertLevel;
                    summary.explorePercentage = (100 * playerRoomsExplored) / saveFile.rmLst.Count;
                }
                saveSummaryData.Add(summary);
            }
        }

        public static void InitTexCoords()
        {            
            extraTexCoords = new List<Vector2>();            
            extraTexCoords.Add(new Vector2(1f, 0.5f));
            extraTexCoords.Add(new Vector2(0, 0.5f));
            extraTexCoords.Add(new Vector2(0, 1));
            extraTexCoords.Add(new Vector2(1f, 1));   

            cancelTexCoords = new List<Vector2>();
            cancelTexCoords.Add(new Vector2(1, 0));
            cancelTexCoords.Add(new Vector2(.5f, 0));
            cancelTexCoords.Add(new Vector2(.5f, .5f));
            cancelTexCoords.Add(new Vector2(1f, .5f));


            okTexCoords = new List<Vector2>();
            okTexCoords.Add(new Vector2(.5f, 0));
            okTexCoords.Add(new Vector2(.0f, 0));
            okTexCoords.Add(new Vector2(.0f, .5f));
            okTexCoords.Add(new Vector2(.5f, .5f));
        }

        public static void Update(int gameTime)
        {
            mapRotate += gameTime * .0003f;
        }

        public void RenderMap()
        {
            Game1.graphicsDevice.SetRenderTarget(mapRenderTarget);
            Game1.graphicsDevice.Clear(Color.Transparent);
            
            if (activeSaveSlot == -1)
                return;
            Game1.graphicsDevice.RasterizerState = RasterizerState.CullNone;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            #region drawmap

            float currentRotate = mapRotate;            
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            Vector3 mapCameraPos = new Vector3(1050,350,700);
            Vector3 mapCameraUp = new Vector3(0, 0, 1);
            Vector3 mapCameraTarget = new Vector3(0, 0, 0);

            Engine.loadMapEffect.World =  Matrix.CreateTranslation(-188, 93, -55) * Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateTranslation(0, 300, 0);
            Engine.loadMapEffect.View = Matrix.CreateLookAt(mapCameraPos, mapCameraTarget, mapCameraUp);
            Engine.loadMapEffect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 5000);
            Engine.loadMapEffect.Texture = Block.crateTexture;
            Engine.loadMapEffect.CurrentTechnique.Passes[0].Apply();
            

            /*playerTextureEffect.Texture = Ability.ability_textures;
            playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            foreach (Room r in roomList)
            {
                r.DrawMapIcons();
            }*/
            for(int i = 0; i < LevelLoader.worldPreLoad.roomList.Count; i++)
            {
                Room r = LevelLoader.worldPreLoad.roomList[i];

                Vector3 outerAdjustedSize = new Vector3(r.size.X + 15f, r.size.Y + 15f, r.size.Z + 15f);
                if (Room.innerBlockMode > 0)
                {
                    Color powerUpColor = r.currentColor;
                    if (saveSummaryData[activeSaveSlot].roomOrbs.Count > 0)
                    {
                        powerUpColor = r.currentColor;
                        if (r.maxOrbs != 0)
                        {
                            powerUpColor.R = (Byte)(40 + saveSummaryData[activeSaveSlot].roomOrbs[i] * (r.color.R - 40) / r.maxOrbs);
                            powerUpColor.G = (Byte)(40 + saveSummaryData[activeSaveSlot].roomOrbs[i] * (r.color.G - 40) / r.maxOrbs);
                            powerUpColor.B = (Byte)(40 + saveSummaryData[activeSaveSlot].roomOrbs[i] * (r.color.B - 40) / r.maxOrbs);
                        }
                    }

                    Engine.mapShellObjects.AddRange(r.GetMapBlockHelper(outerAdjustedSize, powerUpColor));
                }
            }

            
            Engine.mapShellObjects.Sort(new FaceSorter(-(mapCameraPos - mapCameraTarget)));

            List<VertexPositionColorNormalTexture> translucentList = new List<VertexPositionColorNormalTexture>();
            for (int i = 0; i < Engine.mapShellObjects.Count; i++)
            {

                translucentList.Add(Engine.mapShellObjects[i].v1);
                translucentList.Add(Engine.mapShellObjects[i].v2);
                translucentList.Add(Engine.mapShellObjects[i].v3);
                translucentList.Add(Engine.mapShellObjects[i].v4);
                translucentList.Add(Engine.mapShellObjects[i].v5);
                translucentList.Add(Engine.mapShellObjects[i].v6);
            }
            if (translucentList.Count > 0 && DepthControl.depthCount > 0 && Engine.mapShellObjects.Count * 2 > 0)
            {
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    translucentList.ToArray(), 0, Engine.mapShellObjects.Count * 2, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            
            #endregion

        }

        public void RenderTextures()
        {
            RenderMap();
            Game1.graphicsDevice.SetRenderTarget(textRenderTarget);
            Game1.graphicsDevice.Clear(Color.Transparent);

            Engine.spriteBatch.Begin();

            activeSaveSlot = -1;
            foreach (Doodad d in Engine.player.currentRoom.doodads)
            {
                if (d.type == VL.DoodadType.LoadStation && d.active == true)
                {
                    int xBase = 10;
                    int yBase = 5;
                    int yInc = 15;
                    int xHalf = 150;

                    
                    if(d.id.Contains("Slot1"))
                        activeSaveSlot = 0;
                    if (d.id.Contains("Slot2"))
                        activeSaveSlot = 1;
                    if (d.id.Contains("Slot3"))
                        activeSaveSlot = 2;
                    if (d.id.Contains("Slot4"))
                        activeSaveSlot = 3;

                    String greekSuffix = "Alpha";
                    if (activeSaveSlot == 1)
                        greekSuffix = "Beta";
                    if (activeSaveSlot == 2)
                        greekSuffix = "Delta";
                    if (activeSaveSlot == 3)
                        greekSuffix = "Gamma";

                    if (state == LoadState.Normal)
                    {
                        Color textColor = Color.YellowGreen;
                        if (saveSummaryData[activeSaveSlot].expertMode == true)
                            textColor = Color.Orange;
                        if (saveSummaryData[activeSaveSlot].empty == true)
                            Engine.spriteBatch.DrawString(Engine.loadFontBold, "----- New Game " + greekSuffix + " -----", new Vector2(xBase + 100, yBase - 3), textColor);
                        else
                            Engine.spriteBatch.DrawString(Engine.loadFontBold, "----- Mission Log " + greekSuffix + " -----", new Vector2(xBase + 100, yBase - 3), textColor);

                        if (saveSummaryData[activeSaveSlot].expertMode == true)
                            Engine.spriteBatch.DrawString(Engine.loadFont, "Difficulty: Expert", new Vector2(xBase, yBase + yInc), textColor);
                        else if (saveSummaryData[activeSaveSlot].empty == true)
                            Engine.spriteBatch.DrawString(Engine.loadFont, "Difficulty: -", new Vector2(xBase, yBase + yInc), textColor);
                        else
                            Engine.spriteBatch.DrawString(Engine.loadFont, "Difficulty: Normal", new Vector2(xBase, yBase + yInc), textColor);

                        Engine.spriteBatch.DrawString(Engine.loadFont, "Completion: " + saveSummaryData[activeSaveSlot].completionPercentage + "%", new Vector2(xBase, yBase + 2 * yInc), textColor);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Map: " + saveSummaryData[activeSaveSlot].explorePercentage + "%", new Vector2(xBase, yBase + 3 * yInc), textColor);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Equipment: " + saveSummaryData[activeSaveSlot].equipment + " / 23", new Vector2(xBase, yBase + 4 * yInc), textColor);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Health Cubes: " + saveSummaryData[activeSaveSlot].redOrbs + " / " + saveSummaryData[activeSaveSlot].totalRedOrbs, new Vector2(xBase, yBase + 5 * yInc), textColor);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Warp Level: " + saveSummaryData[activeSaveSlot].blueOrbs + " / " + saveSummaryData[activeSaveSlot].totalBlueOrbs, new Vector2(xBase, yBase + 6 * yInc), textColor);
                        TimeSpan totalTime = TimeSpan.FromMilliseconds(saveSummaryData[activeSaveSlot].gameTime);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Time: " + totalTime.Days + ":" + totalTime.Hours.ToString("D2") + ":" + totalTime.Minutes.ToString("D2") + ":" + totalTime.Seconds.ToString("D2"), new Vector2(xBase, yBase + 7 * yInc), textColor);

                        Engine.spriteBatch.DrawString(Engine.loadFont, "Left Mouse Button", new Vector2(xBase, yBase + 9 * yInc), Color.Blue);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Select", new Vector2(xBase + xHalf, yBase + 9 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Right Mouse Button", new Vector2(xBase, yBase + 10 * yInc), Color.Yellow);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Clear Log", new Vector2(xBase + xHalf, yBase + 10 * yInc), Color.YellowGreen);
                        
                    }
                    else if (state == LoadState.ConfirmClear)
                    {
                        Engine.spriteBatch.DrawString(Engine.loadFontBold, "----- WARNING!!! -----", new Vector2(xBase+100, yBase-3), Color.OrangeRed);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "This will erase all data saved", new Vector2(xBase, yBase + 2 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "in Mission Log " + greekSuffix + ".", new Vector2(xBase, yBase + 3 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Are you sure you wish to continue?", new Vector2(xBase, yBase + 8 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Right Mouse Button", new Vector2(xBase, yBase + 9 * yInc), Color.Yellow);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Cancel", new Vector2(xBase + xHalf, yBase + 9 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Left Mouse Button", new Vector2(xBase, yBase + 10 * yInc), Color.Blue);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Confirm", new Vector2(xBase + xHalf, yBase + 10 * yInc), Color.YellowGreen);
                    }
                    else if (state == LoadState.ConfirmNewGame)
                    {
                        Engine.spriteBatch.DrawString(Engine.loadFontBold, "----- Mission Log " + greekSuffix + " -----", new Vector2(xBase + 100, yBase - 3), Color.YellowGreen);

                        Engine.spriteBatch.DrawString(Engine.loadFont, "Record new mission on", new Vector2(xBase, yBase + 2 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "log file " + greekSuffix + "?", new Vector2(xBase, yBase + 3 * yInc), Color.YellowGreen);
                        
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Left Mouse Button", new Vector2(xBase, yBase + 9 * yInc), Color.Blue);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Confirm", new Vector2(xBase + xHalf, yBase + 9 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Right Mouse Button", new Vector2(xBase, yBase + 10* yInc), Color.Yellow);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Cancel", new Vector2(xBase + xHalf, yBase + 10 * yInc), Color.YellowGreen);
                        
                    }
                    else if (state == LoadState.ConfirmContinue)
                    {
                        Engine.spriteBatch.DrawString(Engine.loadFontBold, "----- Mission Log " + greekSuffix + " -----", new Vector2(xBase + 100, yBase - 3), Color.YellowGreen);

                        Engine.spriteBatch.DrawString(Engine.loadFont, "Continue mission on", new Vector2(xBase, yBase + 2 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "log file " + greekSuffix + "?", new Vector2(xBase, yBase + 3 * yInc), Color.YellowGreen);

                        Engine.spriteBatch.DrawString(Engine.loadFont, "Left Mouse Button", new Vector2(xBase, yBase + 9 * yInc), Color.Blue);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Confirm", new Vector2(xBase + xHalf, yBase + 9 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Right Mouse Button", new Vector2(xBase, yBase + 10 * yInc), Color.Yellow);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Cancel", new Vector2(xBase + xHalf, yBase + 10 * yInc), Color.YellowGreen);

                    }
                    else if (state == LoadState.ConfirmNewGameExpert)
                    {
                        Engine.spriteBatch.DrawString(Engine.loadFontBold, "----- Mission Log " + greekSuffix + " -----", new Vector2(xBase + 100, yBase - 3), Color.Orange);

                        Engine.spriteBatch.DrawString(Engine.loadFont, "Record new EXPERT mission on", new Vector2(xBase, yBase + 2 * yInc), Color.Orange);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "log file " + greekSuffix + "?", new Vector2(xBase, yBase + 3 * yInc), Color.Orange);                       

                        Engine.spriteBatch.DrawString(Engine.loadFont, "Left Mouse Button", new Vector2(xBase, yBase + 9 * yInc), Color.Blue);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Confirm", new Vector2(xBase + xHalf, yBase + 9 * yInc), Color.YellowGreen);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "Right Mouse Button", new Vector2(xBase, yBase + 10 * yInc), Color.Yellow);
                        Engine.spriteBatch.DrawString(Engine.loadFont, "-   Cancel", new Vector2(xBase + xHalf, yBase + 10 * yInc), Color.YellowGreen);
                    }
                    
                }
            }
            if (activeSaveSlot == -1)
                state = LoadState.Normal;

            Engine.spriteBatch.End();
            Game1.graphicsDevice.SetRenderTarget(null);
            foreach (Decoration d in Engine.player.currentRoom.decorations)
            {
                if (d.id.Contains("SaveInfo"))
                {
                    if (d.decorationTexture == null)
                    {
                        d.decorationTexture = new List<Texture2D>();
                        d.decorationTexture.Add(textRenderTarget);                        
                    }
                    d.halfWidth = 8.5f;
                    d.halfHeight = 3.5f;
                    d._depth = .15f;
                }
                if (d.id.Contains("MapInfo"))
                {
                    if (d.decorationTexture == null)
                    {
                        d.decorationTexture = new List<Texture2D>();
                        d.decorationTexture.Add(mapRenderTarget);
                    }
                    d.halfWidth = 8.5f;
                    d.halfHeight = 3.5f;
                    d._depth = .15f;
                }
            }
        }

        public void Draw()
        {

        }

        public static void Confirm()
        {
            if (activeSaveSlot != -1)
            {
                if (state == LoadState.ConfirmNewGame || state == LoadState.ConfirmContinue)
                {
                    Engine.saveFileIndex = activeSaveSlot + 1;
                    LevelLoader.LoadFromDisk(Engine.saveFileIndex);
                    MusicControl.loadedMusic = true;
                    Physics.refresh = true;
                }
                else if (state == LoadState.ConfirmNewGameExpert)
                {
                    Engine.saveFileIndex = activeSaveSlot + 1;
                    LevelLoader.LoadFromDisk(Engine.saveFileIndex);
                    Physics.refresh = true;
                    MusicControl.loadedMusic = true;
                    Engine.player.expertLevel = true;
                }
                else if (state == LoadState.Normal)
                {
                    if (saveSummaryData[activeSaveSlot].empty)
                        state = LoadState.ConfirmNewGame;
                    else
                        state = LoadState.ConfirmContinue;
                }
                else if (state == LoadState.ConfirmClear)
                {
                    int saveSlot = activeSaveSlot + 1;
                    String fileName = "altSaveFile" + saveSlot;
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    GenerateSummaries();
                    state = LoadState.Normal;                    
                }
            }
        }

        public static void Cancel()
        {
            if (activeSaveSlot != -1)
            {
                if (state == LoadState.Normal)
                {
                    state = LoadState.ConfirmClear;
                }
                else
                {
                    state = LoadState.Normal;
                }
            }
        }

        public static void Extra()
        {
            if (activeSaveSlot != -1 && expertAvailable && saveSummaryData[activeSaveSlot].empty)
            {
                state = LoadState.ConfirmNewGameExpert;
            }
        }
        
    }
}

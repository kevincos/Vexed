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
    public enum ZoomState
    {
        None,
        ZoomToSector,
        ZoomFromSector,
        Sector,
        ZoomToWorld,
        ZoomFromWorld,
        World,
        Inventory
    }

    public class WorldMap
    {
        public static Vector3 cameraTarget;
        public static float cameraDistance = 150f;
        public static Vector3 cameraUp = Vector3.UnitZ;
        public static Vector3 cameraPosition;

        public static Vector3 worldCameraPosition;
        public static Vector3 sectorCameraPosition;
        public static Vector3 playerCameraPosition;
        public static Vector3 worldCameraTarget;
        public static Vector3 sectorCameraTarget;
        public static Vector3 sectorCameraUp;
        public static Vector3 playerCameraTarget;
        public static Vector3 playerCameraUp;
        public static Vector3 inventoryCameraTarget;
        public static Vector3 inventoryCameraUp;

        public static int selectedInventory = 0;
        public static List<int> inventoryIndexList;

        public static bool warp = false;

        public static float zoomLevel = 0f;
        public static float zoomSpeed = .001f;

        public static float worldZoomLevel = 0f;

        public static Vector2 mousePos;

        public static ZoomState state = ZoomState.None;

        public static int selectedRoomIndex = 0;
        public static int selectedSectorIndex = 0;

        public static bool skipToInventory = false;

        public static Texture2D changeArrow;

        public static void DrawMetaData()
        {
            if (Engine.state == EngineState.Active)
                return;
            Engine.spriteBatch.Begin();
            mousePos.X = Mouse.GetState().X-32;
            mousePos.Y = Mouse.GetState().Y;

            float leftArrowScale = 1f;
            int leftColorBase = 40;
            float rightArrowScale = 1f;
            int rightColorBase = 40;
            if ((Mouse.GetState().X > 750))                
            {
                rightArrowScale = 1.3f;
                rightColorBase = 80;
            }
            if ((Mouse.GetState().X < 50))
            {
                leftArrowScale = 1.3f;
                leftColorBase = 80;
            }
            
            if (state == ZoomState.Sector || state == ZoomState.Inventory)
            {
                Engine.spriteBatch.Draw(changeArrow, new Vector2(25, 280), null, new Color(leftColorBase, leftColorBase, leftColorBase), 0, new Vector2(16,64), leftArrowScale, SpriteEffects.None, 0);
            }
            if (state == ZoomState.Sector || state == ZoomState.World)
            {
                Engine.spriteBatch.Draw(changeArrow, new Vector2(765, 280), null, new Color(rightColorBase, rightColorBase, rightColorBase), 0, new Vector2(16, 64), rightArrowScale, SpriteEffects.FlipHorizontally, 0);
            }


            Engine.spriteBatch.Draw(PauseMenu.mouseCursor, mousePos, Color.YellowGreen);
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
            if (state == ZoomState.Inventory)
            {                
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Inventory", new Vector2(50, 90), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Primary   : ", new Vector2(90, 110), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Secondary : ", new Vector2(90, 130), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Suit Upgrades", new Vector2(90, 430), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Items Unlocked", new Vector2(90, 170), Color.YellowGreen);

                int drawOffset = 0;
                for(int i = 0; i < inventoryIndexList.Count; i++)
                {
                    Ability a = new Ability();
                    a.type = (AbilityType)inventoryIndexList[i];
                    if (selectedInventory == i)
                    {
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "X", new Vector2(60, 110 + 20 * drawOffset), Color.YellowGreen);

                        if (Engine.player.upgrades[inventoryIndexList[i]] == true)
                        {
                            Engine.spriteBatch.DrawString(Engine.spriteFont, a.FriendlyName(), new Vector2(300, 200), Color.YellowGreen);
                            Engine.spriteBatch.DrawString(Engine.spriteFont, a.Description(), new Vector2(300, 230), Color.YellowGreen);
                        }

                    }
                    if (Engine.player.upgrades[inventoryIndexList[i]] == true)
                    {
                        if(i < 2)
                            Engine.spriteBatch.DrawString(Engine.spriteFont, "  " + a.FriendlyName(), new Vector2(200, 110 + 20 * drawOffset), Color.YellowGreen);
                        else
                            Engine.spriteBatch.DrawString(Engine.spriteFont, "  " + a.FriendlyName(), new Vector2(90, 110 + 20 * drawOffset), Color.YellowGreen);
                    }
                    else
                    {
                        Engine.spriteBatch.DrawString(Engine.spriteFont, "  ???", new Vector2(90, 110 + 20 * drawOffset), Color.YellowGreen);
                    }
                    drawOffset += 1;
                    if (i == 1)
                        drawOffset += 2;
                    if (i == 11)
                        drawOffset += 3;
                }

                // Selected Item Info
                

            }
            if(state == ZoomState.Sector)
            {
                Room r = Engine.roomList[selectedRoomIndex];
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Room: " + r.id, new Vector2(10,90), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Power Level: " + r.currentOrbs + " / " + r.maxOrbs, new Vector2(10, 110), Color.YellowGreen);                
            }
            if (state == ZoomState.World)
            {
                
                Sector s = Engine.sectorList[selectedSectorIndex];
       
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Sector: " + s.id, new Vector2(10, 90), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Power Level: " + s.currentOrbs + " / " + s.maxOrbs, new Vector2(10, 110), Color.YellowGreen);                
            }
            Engine.spriteBatch.End();
        }

        public static ZoomState Update(GameTime gameTime)
        {

            if (state == ZoomState.Sector && GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftTrigger))
            {
                cameraDistance += .1f * gameTime.ElapsedGameTime.Milliseconds;
            }
            if (state == ZoomState.Sector && GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightTrigger))
            {
                cameraDistance -= .1f * gameTime.ElapsedGameTime.Milliseconds;
            }
            int currentScrollWheel = Mouse.GetState().ScrollWheelValue;
            if (state == ZoomState.Sector && currentScrollWheel > Controls.scrollWheelPrev)
            {
                cameraDistance -= .5f * gameTime.ElapsedGameTime.Milliseconds;
            }
            if (state == ZoomState.Sector && currentScrollWheel < Controls.scrollWheelPrev)
            {
                cameraDistance += .5f * gameTime.ElapsedGameTime.Milliseconds;
            }

            if (state == ZoomState.ZoomToWorld)
            {

                WorldMap.worldZoomLevel += WorldMap.zoomSpeed * gameTime.ElapsedGameTime.Milliseconds;
                cameraPosition = (1-worldZoomLevel) * sectorCameraPosition + (worldZoomLevel) * worldCameraPosition;
                cameraTarget = (1-worldZoomLevel) * sectorCameraTarget + (worldZoomLevel) * worldCameraTarget;
                cameraDistance = (1 - worldZoomLevel) * 150 + (worldZoomLevel) * 500;
                if (WorldMap.worldZoomLevel > 1f)
                {
                    WorldMap.worldZoomLevel = 1f;
                    state = ZoomState.World;
                }
            }
            if (state == ZoomState.ZoomFromWorld)
            {

                WorldMap.worldZoomLevel -= WorldMap.zoomSpeed * gameTime.ElapsedGameTime.Milliseconds;
                cameraPosition = (1 - worldZoomLevel) * sectorCameraPosition + (worldZoomLevel) * worldCameraPosition;
                cameraTarget = (1 - worldZoomLevel) * sectorCameraTarget + (worldZoomLevel) * worldCameraTarget;
                cameraDistance = (1 - worldZoomLevel) * 150 + (worldZoomLevel) * 500;
                if (WorldMap.worldZoomLevel < 0f)
                {
                    WorldMap.worldZoomLevel = 0f;                    
                    state = ZoomState.Sector;
                    Engine.roomList[selectedRoomIndex].roomHighlight = true;
                }
            }
            if (state == ZoomState.ZoomToSector)
            {
                
                WorldMap.zoomLevel += WorldMap.zoomSpeed * gameTime.ElapsedGameTime.Milliseconds;
                cameraPosition = (1 - zoomLevel) * playerCameraPosition + (zoomLevel) * sectorCameraPosition;
                cameraTarget = (1 - zoomLevel) * playerCameraTarget + (zoomLevel) * sectorCameraTarget;
                cameraUp = (1 - zoomLevel) * playerCameraUp + (zoomLevel) * sectorCameraUp;
                if (WorldMap.zoomLevel > 1f)
                {
                    //cameraDistance = 150f;
                    WorldMap.zoomLevel = 1f;
                    state = ZoomState.Sector;
                    if (skipToInventory == true)
                        state = ZoomState.Inventory;
                    Engine.reDraw = true;
                    skipToInventory = false;
                }
            }
            if (state == ZoomState.ZoomFromSector)
            {
                if(WorldMap.zoomLevel == 1f) Engine.reDraw = true;
                WorldMap.zoomLevel -= WorldMap.zoomSpeed * gameTime.ElapsedGameTime.Milliseconds;
                cameraPosition = (1 - zoomLevel) * playerCameraPosition + (zoomLevel) * sectorCameraPosition;
                cameraTarget = (1 - zoomLevel) * playerCameraTarget + (zoomLevel) * sectorCameraTarget;
                cameraUp = (1 - zoomLevel) * playerCameraUp + (zoomLevel) * sectorCameraUp;
                if (WorldMap.zoomLevel < 0f)
                {
                    WorldMap.zoomLevel = 0f;
                    state = ZoomState.None;
                    Engine.state = EngineState.Active;
                    Engine.roomList[selectedRoomIndex].roomHighlight = false;
                }
            }

            return state;
        }

        public static void RotateWorldMap(GameTime gameTime)
        {
            if (state == ZoomState.Sector || state == ZoomState.World || state == ZoomState.Inventory)
            {
                Vector2 rightStick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Right;
                if (cameraPosition != Vector3.Zero)
                {
                    Vector3 dif = cameraPosition - cameraTarget;
                    Vector3 right = Vector3.Cross(-dif, cameraUp);

                    Vector2 shift = Controls.GetCameraHelper();
                    

                    right.Normalize();
                    dif.Normalize();
                    dif += gameTime.ElapsedGameTime.Milliseconds * .0025f * cameraUp * shift.Y + gameTime.ElapsedGameTime.Milliseconds * .0025f * right * shift.X;
                    dif.Normalize();
                    cameraPosition = cameraTarget + dif * cameraDistance;
                    cameraUp = Vector3.Cross(right, -dif);

                }



            }
        }

        public static void HightlightSector()
        {
            foreach (Room r in Engine.roomList)
            {
                if (r.parentSector == Engine.sectorList[selectedSectorIndex])
                {
                    r.sectorHighlight = true;
                }
            }
        }

        public static void UnHightlightSector()
        {
            foreach (Room r in Engine.roomList)
            {
                if (r.parentSector == Engine.sectorList[selectedSectorIndex])
                {
                    r.sectorHighlight = false;
                }
            }
        }

        public static void ZoomToSector()
        {
            UnHightlightSector();
            worldZoomLevel = 0;
            zoomLevel = 0;
            cameraTarget = playerCameraTarget = Engine.player.cameraTarget;
            cameraPosition = playerCameraPosition = Engine.player.cameraPos;
            cameraUp = playerCameraUp = Engine.player.cameraUp;

            for (int i = 0; i < Engine.roomList.Count; i++)
            {
                if (Engine.roomList[i] == Engine.player.currentRoom)
                {
                    selectedRoomIndex = i;
                    Engine.player.currentRoom.roomHighlight = true;
                }
            }
            for (int i = 0; i < Engine.sectorList.Count; i++)
            {
                if (Engine.roomList[selectedRoomIndex].parentSector == Engine.sectorList[i])
                {
                    selectedSectorIndex = i;
                }
            }
            sectorCameraUp = Engine.player.up;
            sectorCameraTarget = Engine.sectorList[selectedSectorIndex].center;
            Vector3 dif = Engine.player.center.normal + .55f * Engine.player.up + .45f * Engine.player.right;
            dif.Normalize();
            cameraDistance = 150f;
            sectorCameraPosition = sectorCameraTarget + dif * cameraDistance;
            state = ZoomState.ZoomToSector;
        }

        public static int ParseInput()
        {
            int resultCooldown = 0;
            bool leftSreenChange = false;
            bool rightScreenChange = false;
            bool scrollBack = (Keyboard.GetState().IsKeyDown(Keys.LeftControl) == false && Controls.scrollWheelPrev > Mouse.GetState().ScrollWheelValue);
            bool scrollForward = (Keyboard.GetState().IsKeyDown(Keys.LeftControl) == false && Controls.scrollWheelPrev < Mouse.GetState().ScrollWheelValue);
            bool zoomOutCommand = (Keyboard.GetState().IsKeyDown(Keys.OemMinus) || (Keyboard.GetState().IsKeyDown(Keys.LeftControl) == false && Controls.scrollWheelPrev > Mouse.GetState().ScrollWheelValue) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftShoulder) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightShoulder));
            
            // Generate room projections
            if (state == ZoomState.Sector && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                float bestDistance = 10000;
                                        
                for (int i = 0; i < Engine.roomList.Count; i++)
                {
                    if (Engine.roomList[i].parentSector == Engine.sectorList[selectedSectorIndex])
                    {
                        
                        Vector3 projection = Game1.graphicsDevice.Viewport.Project(Engine.roomList[i].center, Engine.playerTextureEffect.Projection, Engine.playerTextureEffect.View, Engine.playerTextureEffect.World);
                        Vector2 position2D = new Vector2(projection.X, projection.Y);
                        if ((position2D - mousePos).Length() < 40)
                        {
                            float distance = (Engine.roomList[i].center - cameraPosition).Length();
                            if (distance < bestDistance)
                            {
                                bestDistance = distance;
                                
                                selectedRoomIndex = i;
                            }
                        }
                    }                   
                }
            }
            // Generate Sector Projections
            if (state == ZoomState.World && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                float bestDistance = 10000;

                int newSelect = selectedSectorIndex;
                for (int i = 0; i < Engine.sectorList.Count; i++)
                {                    
                    Vector3 projection = Game1.graphicsDevice.Viewport.Project(Engine.sectorList[i].center, Engine.playerTextureEffect.Projection, Engine.playerTextureEffect.View, Engine.playerTextureEffect.World);
                    Vector2 position2D = new Vector2(projection.X, projection.Y);
                    if ((position2D - mousePos).Length() < 100)
                    {
                        float distance = (Engine.sectorList[i].center - cameraPosition).Length();
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            newSelect = i;
                        }
                    }                    
                }
                if (newSelect != selectedSectorIndex)
                {
                    UnHightlightSector();
                    selectedSectorIndex = newSelect;
                    HightlightSector();
                }
                
            }

            if (Mouse.GetState().X < 50 && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                leftSreenChange = true;
            }

            if (Mouse.GetState().X > 750 && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                rightScreenChange = true;
            }

            if (state == ZoomState.Inventory && (Keyboard.GetState().IsKeyDown(Keys.Down) || Controls.LeftStick().Y < -.1f))
            {
                resultCooldown = 100;
                selectedInventory++;
                selectedInventory %= inventoryIndexList.Count;
            }
            if (state == ZoomState.Inventory && (Keyboard.GetState().IsKeyDown(Keys.Up) || Controls.LeftStick().Y > .1f))
            {
                resultCooldown = 100;
                selectedInventory--;
                if (selectedInventory < 0)
                    selectedInventory += inventoryIndexList.Count;
            }
            if (state == ZoomState.Inventory && (Mouse.GetState().LeftButton == ButtonState.Pressed))
            {
                int drawOffset = 0;
                for (int i = 0; i < inventoryIndexList.Count; i++)
                {
                    if (Math.Abs(Mouse.GetState().Y - (drawOffset * 20 + 120)) < 10f && Mouse.GetState().X > 110 && Mouse.GetState().X < 300)
                    {
                        selectedInventory = i;
                    }
                    drawOffset += 1;
                    if (i == 1)
                        drawOffset += 2;
                    if (i == 11)
                        drawOffset += 3;

                    
                }
                
            }
            if (warp == true && (Keyboard.GetState().IsKeyDown(Keys.OemPlus) || scrollForward || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.A)) && WorldMap.state == ZoomState.Sector)
            {
                bool validWarpTarget = false;
                foreach (Doodad d in Engine.roomList[selectedRoomIndex].doodads)
                {
                    if (d.type == VexedLib.DoodadType.WarpStation && d.powered == true)
                    {
                        validWarpTarget = true;
                    }
                }
                if (validWarpTarget == false)
                    return 0;
                warp = false;
                Engine.player.Warp(Engine.roomList[selectedRoomIndex]);
                sectorCameraTarget = cameraTarget;
                sectorCameraPosition = cameraPosition;
                sectorCameraUp = cameraUp;
                playerCameraPosition = Engine.player.cameraPos;
                playerCameraTarget = Engine.player.cameraTarget;
                playerCameraUp = Engine.player.cameraUp;
                state = ZoomState.ZoomFromSector;                
            }
            else if (warp == false && (Keyboard.GetState().IsKeyDown(Keys.OemPlus) || scrollForward || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.A)) && (WorldMap.state == ZoomState.Sector || WorldMap.state == ZoomState.Inventory))
            {
                state = ZoomState.ZoomFromSector;
                sectorCameraTarget = cameraTarget;
                sectorCameraPosition = cameraPosition;
                sectorCameraUp = cameraUp;
                playerCameraPosition = Engine.player.cameraPos;
                playerCameraTarget = Engine.player.cameraTarget;
                playerCameraUp = Engine.player.cameraUp;
            }
            else if (warp == false && (Keyboard.GetState().IsKeyDown(Keys.OemPlus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightShoulder) || rightScreenChange || Keyboard.GetState().IsKeyDown(Keys.OemCloseBrackets)) && WorldMap.state == ZoomState.Sector)
            {
                WorldMap.state = ZoomState.Inventory;
                resultCooldown = 150;
            }
            else if (warp == false && (Keyboard.GetState().IsKeyDown(Keys.OemPlus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftShoulder) || leftSreenChange || Keyboard.GetState().IsKeyDown(Keys.OemOpenBrackets)) && WorldMap.state == ZoomState.Inventory)
            {
                WorldMap.state = ZoomState.Sector;
                resultCooldown = 150;

            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.Space) || scrollForward || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.OemPlus) || rightScreenChange || Keyboard.GetState().IsKeyDown(Keys.OemCloseBrackets) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightShoulder)) && WorldMap.state == ZoomState.World)
            {
                state = ZoomState.ZoomFromWorld;
                worldCameraTarget = cameraTarget;
                worldCameraPosition = cameraPosition;
                sectorCameraTarget = Engine.sectorList[selectedSectorIndex].center;
                Vector3 dif = cameraPosition - cameraTarget;
                dif.Normalize();
                sectorCameraPosition = sectorCameraTarget + dif * 150f;
                UnHightlightSector();
            }
            else if (zoomOutCommand == true && WorldMap.state == ZoomState.None)
            {
                skipToInventory = false;
                if (GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightShoulder))
                    skipToInventory = true;
                Engine.state = EngineState.Map;
                ZoomToSector();
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.OemMinus) || scrollBack || Keyboard.GetState().IsKeyDown(Keys.OemOpenBrackets) || leftSreenChange || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftShoulder)) && WorldMap.state == ZoomState.Sector && warp == false)
            {
                state = ZoomState.ZoomToWorld;
                sectorCameraTarget = cameraTarget;
                sectorCameraPosition = cameraPosition;
                worldCameraTarget = Engine.worldCenter;
                Vector3 dif = cameraPosition - cameraTarget;
                dif.Normalize();
                worldCameraPosition = worldCameraTarget + dif * 500f;
                Engine.roomList[selectedRoomIndex].roomHighlight = false;
                HightlightSector();
                
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D) || Controls.LeftStick().X > .1f))
            {
                if (state == ZoomState.Sector && selectedRoomIndex != -1)
                {
                    Engine.roomList[selectedRoomIndex].roomHighlight = false;
                    selectedRoomIndex++;
                    selectedRoomIndex %= Engine.roomList.Count();

                    while (Engine.roomList[selectedRoomIndex].parentSector != Engine.sectorList[selectedSectorIndex] || (warp == true && Engine.roomList[selectedRoomIndex].hasWarp == false))
                    {
                        selectedRoomIndex++;
                        selectedRoomIndex %= Engine.roomList.Count();
                    }

                    /*while (Engine.roomList[selectedRoomIndex].hasWarp == false)
                    {
                        selectedRoomIndex++;
                        selectedRoomIndex %= Engine.roomList.Count();
                    }*/
                    Engine.roomList[selectedRoomIndex].roomHighlight = true;
                    Engine.reDraw = true;
                    resultCooldown = 100;
                }
                if (state == ZoomState.World && selectedSectorIndex != -1)
                {
                    UnHightlightSector();
                    selectedSectorIndex++;
                    selectedSectorIndex %= Engine.sectorList.Count();

                    Engine.reDraw = true;
                    resultCooldown = 100;
                    HightlightSector();
                }
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A) || Controls.LeftStick().X < -.1f))
            {
                if (state == ZoomState.Sector && selectedRoomIndex != -1)
                {
                    Engine.roomList[selectedRoomIndex].roomHighlight = false;
                    selectedRoomIndex--;
                    selectedRoomIndex += Engine.roomList.Count();
                    selectedRoomIndex %= Engine.roomList.Count();

                    while (Engine.roomList[selectedRoomIndex].parentSector != Engine.sectorList[selectedSectorIndex]  || (warp == true && Engine.roomList[selectedRoomIndex].hasWarp == false))
                    {
                        selectedRoomIndex--;
                        selectedRoomIndex += Engine.roomList.Count();
                        selectedRoomIndex %= Engine.roomList.Count();
                    }
                    /*while (Engine.roomList[selectedRoomIndex].hasWarp == false)
                    {
                        selectedRoomIndex--;
                        selectedRoomIndex += Engine.roomList.Count();
                        selectedRoomIndex %= Engine.roomList.Count();
                    }*/
                    Engine.roomList[selectedRoomIndex].roomHighlight = true;
                    Engine.reDraw = true;
                    resultCooldown = 100;
                }
                if (state == ZoomState.World && selectedSectorIndex != -1)
                {
                    UnHightlightSector();
                    selectedSectorIndex--;
                    selectedSectorIndex += Engine.sectorList.Count();
                    selectedSectorIndex %= Engine.sectorList.Count();                    
                    
                    Engine.reDraw = true;
                    resultCooldown = 100;
                    HightlightSector();
                }
            }
         

            return resultCooldown;
        }

    }
}

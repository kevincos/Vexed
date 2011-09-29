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
        World
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
        public static Vector3 playerCameraTarget;
        public static Vector3 playerCameraUp;
        public static Vector3 sectorCameraUp;

        public static float zoomLevel = 0f;
        public static float zoomSpeed = .001f;

        public static float worldZoomLevel = 0f;

        public static ZoomState state = ZoomState.None;

        public static int selectedRoomIndex = 0;
        public static int selectedSectorIndex = 0;

        public static void DrawMetaData()
        {
            if(state == ZoomState.Sector)
            {
                Engine.spriteBatch.Begin();
                Room r = Engine.roomList[selectedRoomIndex];
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Room: " + r.id, new Vector2(10,90), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Power Level: " + r.currentOrbs + " / " + r.maxOrbs, new Vector2(10, 110), Color.YellowGreen);
                Engine.spriteBatch.End();
            }
            if (state == ZoomState.World)
            {
                Engine.spriteBatch.Begin();

                Sector s = Engine.sectorList[selectedSectorIndex];
                int totalSectorOrbs = 0;
                int currentSectorOrbs = 0;
                foreach (Room r in Engine.roomList)
                {
                    if (r.parentSector == s)
                    {
                        totalSectorOrbs += r.maxOrbs;
                        currentSectorOrbs += r.currentOrbs;
                    }
                }
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Sector: " + s.id, new Vector2(10, 90), Color.YellowGreen);
                Engine.spriteBatch.DrawString(Engine.spriteFont, "Power Level: " + currentSectorOrbs + " / " + totalSectorOrbs, new Vector2(10, 110), Color.YellowGreen);
                Engine.spriteBatch.End();
            }
        }

        public static ZoomState Update(GameTime gameTime)
        {
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
                    Engine.reDraw = true;
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

        public static void RotateWorldMap()
        {
            if (state == ZoomState.Sector || state == ZoomState.World)
            {
                Vector2 rightStick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Right;
                if (cameraPosition != Vector3.Zero)
                {
                    Vector3 dif = cameraPosition - cameraTarget;
                    Vector3 right = Vector3.Cross(-dif, cameraUp);
                    right.Normalize();
                    dif.Normalize();
                    dif += .04f * cameraUp * rightStick.Y + .04f * right * rightStick.X;
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

        public static int ParseInput()
        {
            int resultCooldown = 0;
            if ((Keyboard.GetState().IsKeyDown(Keys.OemPlus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.A)) && WorldMap.state == ZoomState.Sector)
            {
                Engine.player.Warp(Engine.roomList[selectedRoomIndex]);
                sectorCameraTarget = cameraTarget;
                sectorCameraPosition = cameraPosition;
                sectorCameraUp = cameraUp;
                playerCameraPosition = Engine.player.cameraPos;
                playerCameraTarget = Engine.player.cameraTarget;
                playerCameraUp = Engine.player.cameraUp;
                state = ZoomState.ZoomFromSector;                
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.OemPlus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightShoulder)) && WorldMap.state == ZoomState.Sector)
            {
                state = ZoomState.ZoomFromSector;
                sectorCameraTarget = cameraTarget;
                sectorCameraPosition = cameraPosition;
                sectorCameraUp = cameraUp;
                playerCameraPosition = Engine.player.cameraPos;
                playerCameraTarget = Engine.player.cameraTarget;
                playerCameraUp = Engine.player.cameraUp;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.OemPlus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.RightShoulder)) && WorldMap.state == ZoomState.World)
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
            if ((Keyboard.GetState().IsKeyDown(Keys.OemMinus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftShoulder)) && WorldMap.state == ZoomState.None)
            {
                Engine.state = EngineState.Map;
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
            if ((Keyboard.GetState().IsKeyDown(Keys.OemMinus) || GamePad.GetState(Game1.activePlayer).IsButtonDown(Buttons.LeftShoulder)) && WorldMap.state == ZoomState.Sector)
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
            if ((Keyboard.GetState().IsKeyDown(Keys.Right) || GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.X > .1f))
            {
                if (state == ZoomState.Sector && selectedRoomIndex != -1)
                {
                    Engine.roomList[selectedRoomIndex].roomHighlight = false;
                    selectedRoomIndex++;
                    selectedRoomIndex %= Engine.roomList.Count();

                    while (Engine.roomList[selectedRoomIndex].parentSector != Engine.sectorList[selectedSectorIndex])
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
            if ((Keyboard.GetState().IsKeyDown(Keys.Left) || GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.X < -.1f))
            {
                if (state == ZoomState.Sector && selectedRoomIndex != -1)
                {
                    Engine.roomList[selectedRoomIndex].roomHighlight = false;
                    selectedRoomIndex--;
                    selectedRoomIndex += Engine.roomList.Count();
                    selectedRoomIndex %= Engine.roomList.Count();

                    while (Engine.roomList[selectedRoomIndex].parentSector != Engine.sectorList[selectedSectorIndex])
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

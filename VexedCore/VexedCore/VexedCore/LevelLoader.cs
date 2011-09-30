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
    public class SaveData
    {
        public List<Room> roomList;
        public List<Sector> sectorList;
        public Player player;
    }

    public class LevelLoader
    {
        public static SaveData lastSave;
    

        public static void Load(String filename)
        {
            Engine.sectorList = new List<Sector>();
            Engine.roomList = new List<Room>();
            Engine.player = new Player();
            
            //FileStream stream = new FileStream(filename, FileMode.Open);
            
            Stream stream = TitleContainer.OpenStream(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(VexedLib.World));

            #region initialLoad
            VexedLib.World world = (VexedLib.World)serializer.Deserialize(stream);            
            foreach (VexedLib.Sector xmlSector in world.sectors)
            {
                Sector newSector = new Sector(xmlSector);
                Engine.sectorList.Add(newSector);
                foreach (VexedLib.Room xmlRoom in xmlSector.rooms)
                {
                    Room newRoom = new Room(xmlRoom);
                    newRoom.sectorID = newSector.id;
                    foreach (VexedLib.Face xmlFace in xmlRoom.faceList)
                    {
                        foreach (VexedLib.Monster xmlMonster in xmlFace.monsters)
                        {
                            
                            if (xmlMonster.movement == VexedLib.MovementType.SnakeBoss)
                            {
                                
                                int snakeLen = 20;
                                newRoom.maxOrbs += 5*snakeLen;
                                for (int i = 0; i < snakeLen; i++)
                                {
                                    Monster snakeLink = new Monster(xmlMonster, xmlFace.normal);
                                    if(i == snakeLen-1)
                                        snakeLink.id = "TS" + i + "_X";
                                    else
                                        snakeLink.id = "S" + i + "_X";
                                    newRoom.monsters.Add(snakeLink);
                                }
                            }
                            else
                            {
                                newRoom.maxOrbs += 5;
                                newSector.maxOrbs += 5;
                                newRoom.monsters.Add(new Monster(xmlMonster, xmlFace.normal));
                            }
                        }
                        foreach (VexedLib.Decoration xmlDecoaration in xmlFace.decorations)
                        {
                            Decoration newDecoration = new Decoration(xmlDecoaration, xmlFace.normal);
                            newRoom.decorations.Add(newDecoration);
                        }
                        foreach (VexedLib.Doodad xmlDoodad in xmlFace.doodads)
                        {
                            Doodad newDoodad = null;

                            if (xmlDoodad.type == VexedLib.DoodadType.PowerOrb)
                            {
                                newRoom.maxOrbs++;
                                newSector.maxOrbs++;
                            }
                            if (xmlDoodad.type == VexedLib.DoodadType.PowerStation)
                            {
                                newRoom.maxOrbs += 10;
                                newSector.maxOrbs += 10;
                            }

                            if (xmlDoodad.type == VexedLib.DoodadType.PlayerSpawn)
                            {
                                Engine.player.center = new Vertex(xmlDoodad.position, xmlFace.normal, Vector3.Zero, xmlDoodad.up);                                
                                Engine.player.currentRoom = newRoom;
                                Engine.player.respawnPlayer = new Player();
                                Engine.player.respawnPlayer.center = new Vertex(xmlDoodad.position, xmlFace.normal, Vector3.Zero, xmlDoodad.up);
                                Engine.player.respawnPlayer.currentRoom = newRoom;
                                Engine.player.respawnPoint = new Doodad(VexedLib.DoodadType.Checkpoint, xmlDoodad.position, xmlFace.normal, xmlDoodad.up);
                                Engine.player.respawnPoint.targetRoom = newRoom;
                            }
                            else if (xmlDoodad.type == VexedLib.DoodadType.BridgeGate)
                            {
                                Doodad bridge = new Doodad(VexedLib.DoodadType.BridgeGate,  xmlDoodad.position + .5f * xmlDoodad.up, xmlFace.normal, Vector3.Normalize(xmlDoodad.up));
                                bridge.targetObject = xmlDoodad.targetObject;
                                bridge.id = xmlDoodad.IDString;
                                newRoom.doodads.Add(bridge);
                                
                                Vector3 right = Vector3.Cross(xmlDoodad.up, xmlFace.normal);
                                newRoom.doodads.Add(new Doodad(VexedLib.DoodadType.BridgeBack, xmlDoodad.position + 1.25f*xmlDoodad.up, xmlFace.normal, xmlDoodad.up));
                                newRoom.doodads.Add(new Doodad(VexedLib.DoodadType.BridgeCover, xmlDoodad.position + .5f * xmlDoodad.up, xmlFace.normal, xmlDoodad.up));
                                newRoom.doodads.Add(new Doodad(VexedLib.DoodadType.BridgeSide, xmlDoodad.position + 1.25f * right + .25f * xmlDoodad.up, xmlFace.normal, xmlDoodad.up));
                                newRoom.doodads.Add(new Doodad(VexedLib.DoodadType.BridgeSide, xmlDoodad.position - 1.25f * right + .25f * xmlDoodad.up, xmlFace.normal, xmlDoodad.up));                                
                            }
                            else if (xmlDoodad.type == VexedLib.DoodadType.Vortex)
                            {
                                Vector3 right = Vector3.Cross(xmlDoodad.up, xmlFace.normal);
                                Doodad entrance = new Doodad(xmlDoodad, xmlFace.normal);
                                Doodad exit = new Doodad(VexedLib.DoodadType.Vortex, xmlDoodad.position + -1f*Math.Abs(Vector3.Dot(newRoom.size, xmlFace.normal))*xmlFace.normal, -xmlFace.normal, xmlDoodad.up);
                                exit.activationCost = entrance.activationCost;
                                exit.id = entrance.id + "_X";
                                entrance.targetObject = exit.id;
                                exit.targetObject = entrance.id;

                                Doodad leftSide1 = new Doodad(VexedLib.DoodadType.TunnelSide, entrance.position.position + .8f * right, entrance.position.normal, entrance.position.direction);
                                Doodad rightSide1 = new Doodad(VexedLib.DoodadType.TunnelSide, entrance.position.position - .8f * right, entrance.position.normal, entrance.position.direction);
                                Doodad leftDoor1 = new Doodad(VexedLib.DoodadType.LeftTunnelDoor, entrance.position.position + .35f * right, entrance.position.normal, entrance.position.direction);
                                Doodad rightDoor1 = new Doodad(VexedLib.DoodadType.RightTunnelDoor, entrance.position.position - .35f * right, entrance.position.normal, entrance.position.direction);
                                leftDoor1.targetObject = entrance.id;
                                rightDoor1.targetObject = entrance.id;
                                Doodad topSide1 = new Doodad(VexedLib.DoodadType.TunnelTop, entrance.position.position + .8f * entrance.position.direction, entrance.position.normal, entrance.position.direction);
                                Doodad bottomSide1 = new Doodad(VexedLib.DoodadType.TunnelTop, entrance.position.position - .8f * entrance.position.direction, entrance.position.normal, entrance.position.direction);

                                Doodad leftSide2 = new Doodad(VexedLib.DoodadType.TunnelSide, exit.position.position + .8f * right, exit.position.normal, exit.position.direction);
                                Doodad rightSide2 = new Doodad(VexedLib.DoodadType.TunnelSide, exit.position.position - .8f * right, exit.position.normal, exit.position.direction);
                                Doodad leftDoor2 = new Doodad(VexedLib.DoodadType.RightTunnelDoor, exit.position.position + .35f * right, exit.position.normal, exit.position.direction);
                                Doodad rightDoor2 = new Doodad(VexedLib.DoodadType.LeftTunnelDoor, exit.position.position - .35f * right, exit.position.normal, exit.position.direction);
                                Doodad topSide2 = new Doodad(VexedLib.DoodadType.TunnelTop, exit.position.position + .8f * exit.position.direction, entrance.position.normal, exit.position.direction);
                                Doodad bottomSide2 = new Doodad(VexedLib.DoodadType.TunnelTop, exit.position.position - .8f * exit.position.direction, entrance.position.normal, exit.position.direction);
                                leftDoor2.targetObject = exit.id;
                                rightDoor2.targetObject = exit.id;
                                

                                newRoom.doodads.Add(entrance);
                                newRoom.doodads.Add(exit);
                                newRoom.doodads.Add(leftSide1);
                                newRoom.doodads.Add(rightSide1);
                                newRoom.doodads.Add(topSide1);
                                newRoom.doodads.Add(bottomSide1);
                                newRoom.doodads.Add(leftDoor1);
                                newRoom.doodads.Add(rightDoor1);
                                newRoom.doodads.Add(leftSide2);
                                newRoom.doodads.Add(rightSide2);
                                newRoom.doodads.Add(topSide2);
                                newRoom.doodads.Add(bottomSide2);
                                newRoom.doodads.Add(leftDoor2);
                                newRoom.doodads.Add(rightDoor2);
                                
                            }
                            else if (xmlDoodad.type == VexedLib.DoodadType.JumpStation || xmlDoodad.type == VexedLib.DoodadType.ItemStation || xmlDoodad.type == VexedLib.DoodadType.SaveStation || xmlDoodad.type == VexedLib.DoodadType.WarpStation || xmlDoodad.type == VexedLib.DoodadType.SwitchStation || xmlDoodad.type == VexedLib.DoodadType.UpgradeStation || xmlDoodad.type == VexedLib.DoodadType.PowerStation || xmlDoodad.type == VexedLib.DoodadType.LoadStation || xmlDoodad.type == VexedLib.DoodadType.MenuStation)
                            {
                                Vector3 right = Vector3.Cross(xmlDoodad.up, xmlFace.normal);
                                Doodad station = new Doodad(xmlDoodad, xmlFace.normal);
                                Doodad icon = new Doodad(VexedLib.DoodadType.StationIcon, xmlDoodad.position + .9f * xmlDoodad.up, xmlFace.normal, xmlDoodad.up);
                                icon.targetObject = station.id;
                                Doodad leftDoor = new Doodad(VexedLib.DoodadType.LeftDoor, xmlDoodad.position - .3f * right, xmlFace.normal, xmlDoodad.up);
                                leftDoor.targetObject = station.id;
                                Doodad rightDoor = new Doodad(VexedLib.DoodadType.RightDoor, xmlDoodad.position + .3f * right, xmlFace.normal, xmlDoodad.up);
                                rightDoor.targetObject = station.id;
                                newRoom.doodads.Add(station);
                                newRoom.doodads.Add(icon);
                                newRoom.doodads.Add(leftDoor);
                                newRoom.doodads.Add(rightDoor);
                            }
                            else
                            {
                                newDoodad = new Doodad(xmlDoodad, xmlFace.normal);
                            }

                            if (newDoodad != null && newDoodad.type == VexedLib.DoodadType.Beam)
                            {
                                if (xmlDoodad.behaviors[0].secondaryValue == 1)
                                    newDoodad.style = RoomStyle.Flame;
                                else
                                    newDoodad.style = RoomStyle.Electric;
                            }

                            if (xmlDoodad.type == VexedLib.DoodadType.WarpStation)
                                newRoom.hasWarp = true;
                            
                            if (newDoodad != null)
                            {
                                foreach (VexedLib.Behavior xmlBehavior in xmlDoodad.behaviors)
                                {
                                    Behavior newBehavior = new Behavior(xmlBehavior);
                                    newDoodad.behaviors.Add(newBehavior);
                                }
                                newDoodad.UpdateBehavior();
                                newRoom.doodads.Add(newDoodad);
                            }
                        }
                        foreach (VexedLib.Block xmlBlock in xmlFace.blocks)
                        {
                            Block newBlock = new Block(xmlBlock);
                            if (newBlock.color == Color.Black)
                                newBlock.color = xmlRoom.color;

                            foreach (VexedLib.Edge xmlEdge in xmlBlock.edges)
                            {
                                Edge newEdge = new Edge(xmlEdge, xmlFace.normal);
                                foreach (VexedLib.Behavior xmlBehavior in xmlEdge.behaviors)
                                {
                                    newEdge.behaviors.Add(new Behavior(xmlBehavior));
                                }
                                newEdge.UpdateBehavior();
                                newBlock.edges.Add(newEdge);                                
                            }
                            foreach (VexedLib.Behavior xmlBehavior in xmlBlock.behaviors)
                            {
                                if (xmlBehavior.destination != Vector3.Zero)
                                    newBlock.staticObject = false;
                                Behavior newBehavior = new Behavior(xmlBehavior);
                                newBlock.behaviors.Add(newBehavior);
                            }
                            newBlock.UpdateBehavior();
                            newRoom.blocks.Add(newBlock);
                        }
                    }
                    newRoom.color = xmlRoom.color;
                    Engine.roomList.Add(newRoom);
                }
            }
            #endregion

            FixDoodads(Engine.roomList);

            QuickSave();
        }

        public static void FixDoodads(List<Room> roomList)
        {
            Engine.worldCenter = Vector3.Zero;
            foreach (Room r in roomList)
            {
                Engine.worldCenter += r.center;
                foreach (Sector s in Engine.sectorList)
                {
                    if (s.id == r.sectorID)
                    {
                        r.parentSector = s;
                    }
                }
                foreach (Doodad d in r.doodads)
                {
                    d.currentRoom = r;
                    if (d.type == VexedLib.DoodadType.JumpPad || d.type == VexedLib.DoodadType.JumpStation)
                    {
                        float baseLineValue = Vector3.Dot(d.position.position, d.position.normal);
                        float minPosValue = 0;
                        foreach (Room destinationRoom in roomList)
                        {
                            float roomSize = Math.Abs(Vector3.Dot(destinationRoom.size / 2, d.position.normal));
                            float roomValue = Vector3.Dot(destinationRoom.center - roomSize * d.position.normal, d.position.normal);
                            if (roomValue > baseLineValue)
                            {
                                if (minPosValue == 0 || roomValue < minPosValue)
                                {
                                    // Verify that line crosses cube
                                    Vector3 right = Vector3.Cross(d.position.normal, d.position.direction);
                                    float upValue = Vector3.Dot(d.position.position, d.position.direction);
                                    float rightValue = Vector3.Dot(d.position.position, right);
                                    float upSize = Math.Abs(Vector3.Dot(destinationRoom.size, d.position.direction));
                                    float rightSize = Math.Abs(Vector3.Dot(destinationRoom.size, right));
                                    float upCenter = Vector3.Dot(destinationRoom.center, d.position.direction);
                                    float rightCenter = Vector3.Dot(destinationRoom.center, right);

                                    if (!(upValue > upCenter + upSize || upValue < upCenter - upSize || rightValue > rightCenter + rightSize || rightValue < rightCenter - rightSize))
                                    {
                                        minPosValue = roomValue;
                                        d.targetRoom = destinationRoom;
                                    }
                                }
                            }
                        }
                    }
                    else if (d.type == VexedLib.DoodadType.BridgeGate)
                    {
                        foreach (Room destinationRoom in roomList)
                        {
                            foreach (Doodad destinationDoodad in destinationRoom.doodads)
                            {
                                if (destinationDoodad.id == d.targetObject)
                                {
                                    d.targetRoom = destinationRoom;
                                    d.targetDoodad = destinationDoodad;
                                }
                            }
                        }
                    }
                    else if (d.type == VexedLib.DoodadType.Checkpoint)
                    {
                        d.targetRoom = r;
                    }
                    else
                    {
                        foreach (Doodad dTarget in r.doodads)
                        {
                            if (dTarget.id == d.targetObject)
                            {
                                d.targetDoodad = dTarget;
                            }
                        }
                        foreach (Block bTarget in r.blocks)
                        {
                            if (bTarget.id == d.targetObject)
                            {
                                d.targetBlock = bTarget;
                            }

                            foreach (Edge eTarget in bTarget.edges)
                            {
                                if (eTarget.id == d.targetObject)
                                {
                                    d.targetEdge = eTarget;
                                }
                            }
                        }
                    }
                }
                foreach (Doodad d in r.doodads)
                {
                    d.Update(new GameTime(TimeSpan.Zero, TimeSpan.Zero));
                }
                foreach (Monster m in r.monsters)
                {
                    if (m.aiType == VexedLib.AIType.Waypoint)
                    {
                        String nextWaypoint = m.firstWaypoint;
                        while (nextWaypoint != "")
                        {
                            foreach (Doodad d in r.doodads)
                            {
                                if (d.id == nextWaypoint)
                                {
                                    if (m.waypoints.Contains(d.position.position))
                                    {
                                        nextWaypoint = "";
                                        m.waypointLoop = true;
                                        break;
                                    }
                                    else
                                    {
                                        m.waypoints.Add(d.position.position);
                                        nextWaypoint = d.targetObject;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }                
            }
            Engine.worldCenter = Engine.worldCenter / roomList.Count;
        }

        public static void QuickSave()
        {
            lastSave = new SaveData();
            lastSave.roomList = new List<Room>();
            lastSave.sectorList = new List<Sector>();
            foreach (Room r in Engine.roomList)
                lastSave.roomList.Add(new Room(r));
            foreach (Sector s in Engine.sectorList)
                lastSave.sectorList.Add(new Sector(s));
            lastSave.player = new Player(Engine.player);
        }

        public static void SaveToDisk(int saveFileIndex)
        {
            QuickSave();

            Stream stream = new FileStream("saveFile"+saveFileIndex, FileMode.Create, FileAccess.ReadWrite);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            serializer.Serialize(stream, lastSave);
            stream.Close();
        }

        public static void LoadFromDisk(int saveFileIndex)
        {

            if (File.Exists("saveFile" + saveFileIndex))
            {
                Stream stream = new FileStream("saveFile" + saveFileIndex, FileMode.Open, FileAccess.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                lastSave = (SaveData)serializer.Deserialize(stream);
                QuickLoad();
                stream.Close();
            }
            else
            {
                LevelLoader.Load("LevelData\\world");
            }
        }

        public static void QuickLoad()
        {
            Engine.roomList = new List<Room>();
            foreach (Room r in lastSave.roomList)
                Engine.roomList.Add(new Room(r));
            Engine.player = new Player(lastSave.player);
            Engine.sectorList = new List<Sector>();
            foreach (Sector s in lastSave.sectorList)
                Engine.sectorList.Add(new Sector(s));
            foreach (Room r in Engine.roomList)
            {
                if (r.id == Engine.player.currentRoomId)
                    Engine.player.currentRoom = r;

                foreach (Block b in r.blocks)
                {
                    if (b.currentBehavior == null)
                    {
                        foreach (Behavior behavior in b.behaviors)
                        {
                            if (behavior.id == b.currentBehaviorId)
                                b.currentBehavior = behavior;
                        }
                    }
                    foreach (Edge e in b.edges)
                    {
                        if (e.currentBehavior == null)
                        {
                            foreach (Behavior behavior in e.behaviors)
                            {
                                if (behavior.id == e.currentBehaviorId)
                                    e.currentBehavior = behavior;
                            }
                        }
                    }
                }
                foreach (Doodad d in r.doodads)
                {
                    if (d.currentBehavior == null)
                    {
                        foreach (Behavior behavior in d.behaviors)
                        {
                            if (behavior.id == d.currentBehaviorId)
                                d.currentBehavior = behavior;
                        }
                    }
                    
                }
            }
            FixDoodads(Engine.roomList);
        }
    }
}

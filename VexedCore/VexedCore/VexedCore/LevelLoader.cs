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
        public static SaveData worldPreLoad;


        

        public static void Load(String filename)
        {
            Engine.sectorList = new List<Sector>();
            Engine.roomList = new List<Room>();
            Engine.player = new Player();
                        
            Stream stream = TitleContainer.OpenStream(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(VL.World));

            #region initialLoad
            VL.World world = (VL.World)serializer.Deserialize(stream);            
            foreach (VL.Sector xmlSector in world.sectors)
            {
                Sector newSector = new Sector(xmlSector);
                Engine.sectorList.Add(newSector);
                foreach (VL.Room xmlRoom in xmlSector.rooms)
                {
                    Room newRoom = new Room(xmlRoom);
                    newRoom.sectorID = newSector.id;
                    foreach (VL.Face xmlFace in xmlRoom.faceList)
                    {
                        foreach (VL.Monster xmlMonster in xmlFace.monsters)
                        {
                            if (xmlMonster.movement == VL.MovementType.RockBoss && xmlMonster.IDString.Contains("Snow"))
                            {
                                newRoom.maxOrbs += 10;
                                newSector.maxOrbs += 10;
                                newRoom.monsters.Add(new Monster(xmlMonster, xmlFace.normal));
                            }
                            else if (xmlMonster.movement == VL.MovementType.BattleBoss)
                            {
                                newRoom.maxOrbs += 15;
                                newSector.maxOrbs += 15;
                                newRoom.monsters.Add(new Monster(xmlMonster, xmlFace.normal));
                            }
                            else if (xmlMonster.movement == VL.MovementType.SnakeBoss)
                            {

                                int snakeLen = 20;
                                newRoom.maxOrbs += 5 * snakeLen;
                                for (int i = 0; i < snakeLen; i++)
                                {
                                    Monster snakeLink = new Monster(xmlMonster, xmlFace.normal);
                                    if (i == snakeLen - 1)
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
                        foreach (VL.Decoration xmlDecoaration in xmlFace.decorations)
                        {
                            Decoration newDecoration = new Decoration(xmlDecoaration, xmlFace.normal);
                            newDecoration.SetTexture();
                            newDecoration.UpdateSizeData();
                            newRoom.decorations.Add(newDecoration);
                        }
                        foreach (VL.Doodad xmlDoodad in xmlFace.doodads)
                        {
                            Doodad newDoodad = null;

                            if (xmlDoodad.type == VL.DoodadType.BlueCube)
                            {
                                newRoom.maxBlueOrbs++;
                                newSector.maxBlueOrbs++;
                            }
                            if (xmlDoodad.type == VL.DoodadType.RedCube)
                            {
                                newRoom.maxRedOrbs++;
                                newSector.maxRedOrbs++;
                            }
                            if (xmlDoodad.type == VL.DoodadType.BluePowerStation)
                            {
                                newRoom.maxBlueOrbs++;
                                newSector.maxBlueOrbs++;
                            }
                            if (xmlDoodad.type == VL.DoodadType.RedPowerStation)
                            {
                                newRoom.maxRedOrbs++;
                                newSector.maxRedOrbs++;
                            }
                            if (xmlDoodad.type == VL.DoodadType.PowerOrb)
                            {
                                newRoom.maxOrbs++;
                                newSector.maxOrbs++;
                            }
                            if (xmlDoodad.type == VL.DoodadType.PowerStation)
                            {
                                newRoom.maxOrbs += 10;
                                newSector.maxOrbs += 10;
                            }

                            if (xmlDoodad.type == VL.DoodadType.PlayerSpawn)
                            {
                                Engine.player.center = new Vertex(xmlDoodad.position, xmlFace.normal, Vector3.Zero, xmlDoodad.up);                                
                                Engine.player.currentRoom = newRoom;
                                Engine.player.respawnPlayer = new Player();
                                Engine.player.respawnPlayer.center = new Vertex(xmlDoodad.position, xmlFace.normal, Vector3.Zero, xmlDoodad.up);
                                Engine.player.respawnPlayer.currentRoom = newRoom;
                                Engine.player.respawnPoint = new Doodad(VL.DoodadType.Checkpoint, xmlDoodad.position, xmlFace.normal, xmlDoodad.up);
                                Engine.player.respawnPoint.targetRoom = newRoom;
                            }
                            else if (xmlDoodad.type == VL.DoodadType.BridgeGate)
                            {
                                Doodad bridge = new Doodad(VL.DoodadType.BridgeGate,  xmlDoodad.position + .5f * xmlDoodad.up, xmlFace.normal, Vector3.Normalize(xmlDoodad.up));
                                bridge.targetObject = xmlDoodad.targetObject;
                                bridge.id = xmlDoodad.IDString;
                                newRoom.doodads.Add(bridge);
                                
                                Vector3 right = Vector3.Cross(xmlDoodad.up, xmlFace.normal);
                                newRoom.doodads.Add(new Doodad(VL.DoodadType.BridgeBack, xmlDoodad.position + 1.25f*xmlDoodad.up, xmlFace.normal, xmlDoodad.up));
                                newRoom.doodads.Add(new Doodad(VL.DoodadType.BridgeCover, xmlDoodad.position + .5f * xmlDoodad.up, xmlFace.normal, xmlDoodad.up));
                                newRoom.doodads.Add(new Doodad(VL.DoodadType.BridgeSide, xmlDoodad.position + 1.25f * right + .25f * xmlDoodad.up, xmlFace.normal, xmlDoodad.up));
                                newRoom.doodads.Add(new Doodad(VL.DoodadType.BridgeSide, xmlDoodad.position - 1.25f * right + .25f * xmlDoodad.up, xmlFace.normal, xmlDoodad.up));                                
                            }
                            else if (xmlDoodad.type == VL.DoodadType.Vortex)
                            {
                                Vector3 right = Vector3.Cross(xmlDoodad.up, xmlFace.normal);
                                Doodad entrance = new Doodad(xmlDoodad, xmlFace.normal);
                                Doodad exit = new Doodad(VL.DoodadType.Vortex, xmlDoodad.position + -1f*Math.Abs(Vector3.Dot(newRoom.size, xmlFace.normal))*xmlFace.normal, -xmlFace.normal, xmlDoodad.up);
                                exit.activationCost = entrance.activationCost;
                                exit.id = entrance.id + "_X";
                                entrance.targetObject = exit.id;
                                exit.targetObject = entrance.id;

                                Doodad leftSide1 = new Doodad(VL.DoodadType.TunnelSide, entrance.position.position + .8f * right, entrance.position.normal, entrance.position.direction);
                                Doodad rightSide1 = new Doodad(VL.DoodadType.TunnelSide, entrance.position.position - .8f * right, entrance.position.normal, entrance.position.direction);
                                Doodad leftDoor1 = new Doodad(VL.DoodadType.LeftTunnelDoor, entrance.position.position + .35f * right, entrance.position.normal, entrance.position.direction);
                                Doodad rightDoor1 = new Doodad(VL.DoodadType.RightTunnelDoor, entrance.position.position - .35f * right, entrance.position.normal, entrance.position.direction);
                                leftDoor1.targetObject = entrance.id;
                                rightDoor1.targetObject = entrance.id;
                                Doodad topSide1 = new Doodad(VL.DoodadType.TunnelTop, entrance.position.position + .8f * entrance.position.direction, entrance.position.normal, entrance.position.direction);
                                Doodad bottomSide1 = new Doodad(VL.DoodadType.TunnelTop, entrance.position.position - .8f * entrance.position.direction, entrance.position.normal, entrance.position.direction);

                                Doodad leftSide2 = new Doodad(VL.DoodadType.TunnelSide, exit.position.position + .8f * right, exit.position.normal, exit.position.direction);
                                Doodad rightSide2 = new Doodad(VL.DoodadType.TunnelSide, exit.position.position - .8f * right, exit.position.normal, exit.position.direction);
                                Doodad leftDoor2 = new Doodad(VL.DoodadType.RightTunnelDoor, exit.position.position + .35f * right, exit.position.normal, exit.position.direction);
                                Doodad rightDoor2 = new Doodad(VL.DoodadType.LeftTunnelDoor, exit.position.position - .35f * right, exit.position.normal, exit.position.direction);
                                Doodad topSide2 = new Doodad(VL.DoodadType.TunnelTop, exit.position.position + .8f * exit.position.direction, entrance.position.normal, exit.position.direction);
                                Doodad bottomSide2 = new Doodad(VL.DoodadType.TunnelTop, exit.position.position - .8f * exit.position.direction, entrance.position.normal, exit.position.direction);
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
                            else if (xmlDoodad.type == VL.DoodadType.JumpStation || xmlDoodad.type == VL.DoodadType.HealthStation || xmlDoodad.type == VL.DoodadType.ItemStation || xmlDoodad.type == VL.DoodadType.SaveStation || xmlDoodad.type == VL.DoodadType.WarpStation || xmlDoodad.type == VL.DoodadType.SwitchStation || xmlDoodad.type == VL.DoodadType.UpgradeStation || xmlDoodad.type == VL.DoodadType.PowerStation || xmlDoodad.type == VL.DoodadType.LoadStation || xmlDoodad.type == VL.DoodadType.MenuStation || xmlDoodad.type == VL.DoodadType.RedPowerStation || xmlDoodad.type == VL.DoodadType.BluePowerStation)
                            {
                                Vector3 right = Vector3.Cross(xmlDoodad.up, xmlFace.normal);
                                Doodad station = new Doodad(xmlDoodad, xmlFace.normal);
                                Doodad icon = new Doodad(VL.DoodadType.StationIcon, xmlDoodad.position + .9f * xmlDoodad.up, xmlFace.normal, xmlDoodad.up);
                                icon.targetObject = station.id;
                                Doodad leftDoor = new Doodad(VL.DoodadType.LeftDoor, xmlDoodad.position - .3f * right, xmlFace.normal, xmlDoodad.up);
                                leftDoor.targetObject = station.id;
                                Doodad rightDoor = new Doodad(VL.DoodadType.RightDoor, xmlDoodad.position + .3f * right, xmlFace.normal, xmlDoodad.up);
                                rightDoor.targetObject = station.id;
                                newRoom.doodads.Add(station);
                                newRoom.doodads.Add(icon);
                                newRoom.doodads.Add(leftDoor);
                                newRoom.doodads.Add(rightDoor);

                                if (xmlDoodad.type == VL.DoodadType.JumpPad || xmlDoodad.type == VL.DoodadType.JumpStation)
                                {
                                    Doodad leftSide = new Doodad(VL.DoodadType.RingSide, station.position.position + .8f * right, station.position.normal, station.position.direction);
                                    Doodad rightSide = new Doodad(VL.DoodadType.RingSide, station.position.position - .8f * right, station.position.normal, station.position.direction);
                                    Doodad topSide = new Doodad(VL.DoodadType.RingTop, station.position.position + .8f * station.position.direction, station.position.normal, station.position.direction);
                                    Doodad bottomSide = new Doodad(VL.DoodadType.RingTop, station.position.position - .8f * station.position.direction, station.position.normal, station.position.direction);
                                    leftSide.targetObject = station.id;
                                    rightSide.targetObject = station.id;
                                    topSide.targetObject = station.id;
                                    bottomSide.targetObject = station.id;
                                    newRoom.doodads.Add(leftSide);
                                    newRoom.doodads.Add(rightSide);
                                    newRoom.doodads.Add(topSide);
                                    newRoom.doodads.Add(bottomSide);
                                }
                            }
                            else
                            {
                                newDoodad = new Doodad(xmlDoodad, xmlFace.normal);
                            }

                            if (newDoodad != null && newDoodad.type == VL.DoodadType.Beam)
                            {
                                if (xmlDoodad.behaviors[0].secondaryValue == 1)
                                {
                                    newDoodad.style = RoomStyle.Flame;
                                    Decoration flameSrc = new Decoration(xmlDoodad.position, xmlFace.normal, xmlDoodad.up, "beam_flame_src", 75.5f);
                                    newRoom.decorations.Add(flameSrc);
                                    
                                }
                                else
                                {
                                    newDoodad.style = RoomStyle.Electric;
                                    Decoration electricSrc = new Decoration(xmlDoodad.position, xmlFace.normal, xmlDoodad.up, "beam_electric_src", 75.5f);
                                    newRoom.decorations.Add(electricSrc);
                                }
                            }

                            if (xmlDoodad.type == VL.DoodadType.WarpStation)
                            {
                                newRoom.hasWarp = true;
                                newRoom.warpCost = xmlDoodad.activationCost;
                            }
                            
                            if (newDoodad != null)
                            {
                                foreach (VL.Behavior xmlBehavior in xmlDoodad.behaviors)
                                {
                                    Behavior newBehavior = new Behavior(xmlBehavior);
                                    newDoodad.behaviors.Add(newBehavior);
                                }
                                newDoodad.UpdateBehavior();
                                newRoom.doodads.Add(newDoodad);
                            }
                        }
                        foreach (VL.Block xmlBlock in xmlFace.blocks)
                        {
                            Block newBlock = new Block(xmlBlock);
                            if (newBlock.color == Color.Black)
                                newBlock.color = xmlRoom.color;

                            foreach (VL.Edge xmlEdge in xmlBlock.edges)
                            {
                                Edge newEdge = new Edge(xmlEdge, xmlFace.normal);
                                foreach (VL.Behavior xmlBehavior in xmlEdge.behaviors)
                                {
                                    newEdge.behaviors.Add(new Behavior(xmlBehavior));
                                }
                                newEdge.UpdateBehavior();
                                newBlock.edges.Add(newEdge);                                
                            }
                            foreach (VL.Behavior xmlBehavior in xmlBlock.behaviors)
                            {
                                if (xmlBehavior.destination != Vector3.Zero)
                                    newBlock.staticObject = false;
                                Behavior newBehavior = new Behavior(xmlBehavior);
                                newBlock.behaviors.Add(newBehavior);
                            }
                            newBlock.UpdateBehavior();
                            newBlock.length = (xmlBlock.edges[0].end - xmlBlock.edges[0].start).Length();
                            newBlock.height = (xmlBlock.edges[1].end - xmlBlock.edges[1].start).Length();
                            newBlock.area = newBlock.length * newBlock.height;
                            newBlock.depth = xmlBlock.depth;
                            newBlock.scales = xmlBlock.scales;
                            newBlock.wallType = xmlBlock.type;
                            
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
                    if (d.type == VL.DoodadType.JumpPad || d.type == VL.DoodadType.JumpStation)
                    {
                        if (d.id.Contains("Diamond"))
                            d.doorDecal = DoorDecal.Diamond;
                        else if (d.id.Contains("Cherry"))
                            d.doorDecal = DoorDecal.Cherry;
                        else
                            d.doorDecal = DoorDecal.Target;
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
                    else if (d.type == VL.DoodadType.BridgeGate)
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
                    else if (d.type == VL.DoodadType.Checkpoint)
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
                    if (m.aiType == VL.AIType.Waypoint)
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
                foreach (Decoration d in r.decorations)
                {
                    d.SetTexture();
                }
            }

            Engine.worldCenter = Engine.worldCenter / roomList.Count;
        }

        public static void QuickSave()
        {
            QuickSave(false);
        }

        public static void QuickSave(bool preLoad)
        {
            lastSave = new SaveData();
            lastSave.roomList = new List<Room>();
            lastSave.sectorList = new List<Sector>();
            foreach (Room r in Engine.roomList)
                lastSave.roomList.Add(new Room(r));
            foreach (Sector s in Engine.sectorList)
                lastSave.sectorList.Add(new Sector(s));
            lastSave.player = new Player(Engine.player);

            if (preLoad)
            {
                worldPreLoad = new SaveData();
                worldPreLoad.roomList = new List<Room>();
                worldPreLoad.sectorList = new List<Sector>();
                foreach (Room r in Engine.roomList)
                    worldPreLoad.roomList.Add(new Room(r));
                foreach (Sector s in Engine.sectorList)
                    worldPreLoad.sectorList.Add(new Sector(s));
                worldPreLoad.player = new Player(Engine.player);
            }
        }

        public static void PreLoad()
        {
            lastSave = worldPreLoad;
        }

        public static void SaveToDisk(int saveFileIndex)
        {
            QuickSave();



            /*Stream stream = new FileStream("saveFile"+saveFileIndex, FileMode.Create, FileAccess.ReadWrite);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            
            serializer.Serialize(stream, lastSave);
            stream.Close();*/

            Stream altStream = new FileStream("altSaveFile" + saveFileIndex, FileMode.Create, FileAccess.ReadWrite);
            XmlSerializer altSerializer = new XmlSerializer(typeof(CompactSaveData));

            altSerializer.Serialize(altStream, new CompactSaveData(lastSave));
            altStream.Close();
        }

        public static void LoadFromDisk(int saveFileIndex)
        {
            LevelLoader.PreLoad();
            QuickLoad();
            
            if (File.Exists("altSaveFile" + saveFileIndex))
            {
                /*Stream stream = new FileStream("saveFile" + saveFileIndex, FileMode.Open, FileAccess.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                lastSave = (SaveData)serializer.Deserialize(stream);
                QuickLoad();
                stream.Close();*/

                Stream altStream = new FileStream("altSaveFile" + saveFileIndex, FileMode.Open, FileAccess.ReadWrite);
                XmlSerializer altSerializer = new XmlSerializer(typeof(CompactSaveData));
                CompactSaveData loadData = (CompactSaveData)altSerializer.Deserialize(altStream);                
                altStream.Close();
                loadData.LoadCompactSaveData();
            }

            FixDoodads(Engine.roomList);
            QuickSave();
            QuickLoad();
            Engine.reDraw = true;

            
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

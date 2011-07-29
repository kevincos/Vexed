﻿using System;
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
    class LevelLoader
    {
        public static void Load(String filename)
        {
            Engine.roomList = new List<Room>();
            Engine.player = new Player();
            
            //FileStream stream = new FileStream(filename, FileMode.Open);
            
            Stream stream = TitleContainer.OpenStream(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(VexedLib.World));

            #region initialLoad
            VexedLib.World world = (VexedLib.World)serializer.Deserialize(stream);            
            foreach (VexedLib.Sector xmlSector in world.sectors)
            {
                foreach (VexedLib.Room xmlRoom in xmlSector.rooms)
                {
                    Room newRoom = new Room(xmlRoom);
                    foreach (VexedLib.Face xmlFace in xmlRoom.faceList)
                    {
                        foreach (VexedLib.Monster xmlMonster in xmlFace.monsters)
                        {
                            newRoom.monsters.Add(new Monster(xmlMonster, xmlFace.normal));
                        }
                        foreach (VexedLib.Doodad xmlDoodad in xmlFace.doodads)
                        {
                            Doodad newDoodad = null;
                            
                            if (xmlDoodad.type == VexedLib.DoodadType.PlayerSpawn)
                            {
                                Engine.player.center = new Vertex(xmlDoodad.position, xmlFace.normal, Vector3.Zero, xmlDoodad.up);                                
                                Engine.player.currentRoom = newRoom;
                                Engine.player.respawnCenter = new Vertex(xmlDoodad.position, xmlFace.normal, Vector3.Zero, xmlDoodad.up);
                                Engine.player.respawnPoint = new Doodad(VexedLib.DoodadType.Checkpoint, xmlDoodad.position, xmlFace.normal, xmlDoodad.up);
                                Engine.player.respawnPoint.targetRoom = newRoom;
                            }
                            else if (xmlDoodad.type == VexedLib.DoodadType.JumpPad)
                            {
                                newRoom.jumpPads.Add(new JumpPad(xmlDoodad.position, xmlFace.normal, xmlDoodad.up));
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
                            else
                            {
                                newDoodad = new Doodad(xmlDoodad, xmlFace.normal);
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

            #region fix extra doodad data
            foreach (Room r in Engine.roomList)
            {
                foreach (Bridge b in r.bridges)
                {
                    foreach (Room destinationRoom in Engine.roomList)
                    {
                        foreach (Bridge destinationBridge in destinationRoom.bridges)
                        {
                            if (destinationBridge.id == b.targetId)
                            {
                                b.targetRoom = destinationRoom;
                                b.targetBridge = destinationBridge;
                            }
                        }
                    }
                }
                foreach (JumpPad j in r.jumpPads)
                {                    
                    float baseLineValue = Vector3.Dot(j.position.position, j.position.normal);
                    float minPosValue = 0;
                    foreach (Room destinationRoom in Engine.roomList)
                    {
                        float roomSize = Math.Abs(Vector3.Dot(destinationRoom.size / 2, j.position.normal));
                        float roomValue = Vector3.Dot(destinationRoom.center - roomSize * j.position.normal, j.position.normal);
                        if (roomValue > baseLineValue)
                        {
                            if (minPosValue == 0 || roomValue < minPosValue)
                            {
                                // Verify that line crosses cube
                                Vector3 right = Vector3.Cross(j.position.normal, j.position.direction);
                                float upValue = Vector3.Dot(j.position.position, j.position.direction);
                                float rightValue = Vector3.Dot(j.position.position, right);
                                float upSize = Math.Abs(Vector3.Dot(destinationRoom.size, j.position.direction));
                                float rightSize = Math.Abs(Vector3.Dot(destinationRoom.size, right));
                                float upCenter = Vector3.Dot(destinationRoom.center, j.position.direction);
                                float rightCenter = Vector3.Dot(destinationRoom.center, right);

                                if (!(upValue > upCenter + upSize || upValue < upCenter - upSize || rightValue > rightCenter + rightSize || rightValue < rightCenter - rightSize))
                                {
                                    minPosValue = roomValue;
                                    j.targetRoom = destinationRoom;    
                                }
                            }
                        }
                    }
                }
                foreach (Doodad d in r.doodads)
                {
                    if (d.type == VexedLib.DoodadType.BridgeGate)
                    {
                        foreach (Room destinationRoom in Engine.roomList)
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
                    }
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
            #endregion
        }
    }
}

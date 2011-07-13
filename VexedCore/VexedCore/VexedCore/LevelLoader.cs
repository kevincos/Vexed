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
    class LevelLoader
    {
        public static void Load(String filename)
        {
            Game1.roomList = new List<Room>();
            Game1.player = new Player();
            
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
                        foreach (VexedLib.Doodad xmlDoodad in xmlFace.doodads)
                        {
                            if (xmlDoodad.type == VexedLib.DoodadType.PlayerSpawn)
                            {
                                Game1.player.center = new Vertex(xmlDoodad.position, xmlFace.normal, Vector3.Zero, xmlDoodad.up);                                
                                Game1.player.currentRoom = newRoom;
                            }
                            if (xmlDoodad.type == VexedLib.DoodadType.JumpPad)
                            {
                                newRoom.jumpPads.Add(new JumpPad(xmlDoodad.position, xmlFace.normal, xmlDoodad.up));
                            }
                            if (xmlDoodad.type == VexedLib.DoodadType.BridgeGate)
                            {
                                newRoom.bridges.Add(new Bridge(xmlDoodad.position, xmlFace.normal, xmlDoodad.up, xmlDoodad.IDString, xmlDoodad.targetObject));
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
                    Game1.roomList.Add(newRoom);
                }
            }
            #endregion

            #region fix extra doodad data
            foreach (Room r in Game1.roomList)
            {
                foreach (Bridge b in r.bridges)
                {
                    foreach (Room destinationRoom in Game1.roomList)
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
                    foreach (Room destinationRoom in Game1.roomList)
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
            }
            #endregion
        }
    }
}

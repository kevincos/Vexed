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
        public static List<Room> Load(String filename)
        {
            List<Room> roomList = new List<Room>();
            FileStream stream = new FileStream(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(VexedLib.World));
            VexedLib.World world = (VexedLib.World)serializer.Deserialize(stream);
            
            foreach (VexedLib.Sector xmlSector in world.sectors)
            {
                foreach (VexedLib.Room xmlRoom in xmlSector.rooms)
                {
                    Room testRoom = new Room(xmlRoom);
                    foreach (VexedLib.Face xmlFace in xmlRoom.faceList)
                    {
                        foreach (VexedLib.Block xmlBlock in xmlFace.blocks)
                        {
                            Block newBlock = new Block(xmlBlock);
                            if (newBlock.color == Color.Black)
                                newBlock.color = xmlRoom.color;

                            foreach (VexedLib.Edge xmlEdge in xmlBlock.edges)
                            {
                                Edge newEdge = new Edge(xmlEdge, xmlFace.normal);
                                newBlock.edges.Add(newEdge);
                            }
                            foreach (VexedLib.Behavior xmlBehavior in xmlBlock.behaviors)
                            {
                                Behavior newBehavior = new Behavior(xmlBehavior);
                                newBlock.behaviors.Add(newBehavior);
                            }
                            newBlock.UpdateBehavior();
                            testRoom.blocks.Add(newBlock);
                        }
                    }
                    roomList.Add(testRoom);
                }
            }

            return roomList;
        }
    }
}

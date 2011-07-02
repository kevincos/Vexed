using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace VexedCore
{
    class LevelLoader
    {
        public static Room Load(String filename)
        {
            FileStream stream = new FileStream(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(VexedLib.World));
            VexedLib.World world = (VexedLib.World)serializer.Deserialize(stream);

            Room testRoom = null;
            foreach (VexedLib.Sector xmlSector in world.sectors)
            {
                foreach (VexedLib.Room xmlRoom in xmlSector.rooms)
                {
                    testRoom = new Room(xmlRoom);
                    foreach (VexedLib.Face xmlFace in xmlRoom.faceList)
                    {
                        foreach (VexedLib.Block xmlBlock in xmlFace.blocks)
                        {
                            Block newBlock = new Block(xmlBlock);
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
                }
            }

            return testRoom;
        }
    }
}

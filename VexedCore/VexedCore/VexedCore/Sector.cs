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
using System.Xml.Serialization;

namespace VexedCore
{
    public enum SkyBoxType
    {
        DeepSpace,
        Galaxy,
        Solar,
        Red,
        Blue,
        Green
    }

    public class Sector
    {
        public string id;
        public Vector3 center;

        public SkyBoxType skyboxType
        {
            get
            {
                if (id.Contains("Green"))
                    return SkyBoxType.Green;
                if (id.Contains("Blue"))
                    return SkyBoxType.Blue;
                if (id.Contains("Red"))
                    return SkyBoxType.Red;
                if (id.Contains("Solar"))
                    return SkyBoxType.Solar;
                if (id.Contains("Galaxy"))
                    return SkyBoxType.Galaxy;
                return SkyBoxType.DeepSpace;
            }
        }
        

        public List<Room> roomList;

        public Sector()
        {
            roomList = new List<Room>();
        }

        public Sector(Sector s)
        {
            id = s.id;
            center = s.center;
            roomList = new List<Room>();
            foreach (Room r in s.roomList)
            {
                roomList.Add(new Room(r));
            }
        }

        public Sector(VexedLib.Sector xmlSector)
        {
            id = xmlSector.IDString;
            center = xmlSector.center;
            roomList = new List<Room>();
        }
    }
}

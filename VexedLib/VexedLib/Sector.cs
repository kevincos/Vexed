using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VexedLib
{
    public enum EditMode
    {
        None,
        Block,
        BlockDrag,
        Line,
        LineDrag,
        Point,
        PointDrag,
        LineSelect,
        BlockSelect,
        Doodad,
        DoodadDrag,
        Monster,
        MonsterDrag,
        Decoration,
        DecorationDrag
    }

    public class Sector
    {
        public int id;
        public String name;
        public List<Room> rooms;
        public Vector3 center;

        public Sector()
        {
            name = "Sector";
            id = IDControl.GetID();
            rooms = new List<Room>();
        }

        public void UpdateSectorCenter()
        {
            Vector3 roomPositionAverage = Vector3.Zero;
            foreach (Room r in rooms)
            {
                roomPositionAverage += new Vector3(r.centerX, r.centerY, r.centerZ);
            }
            center = roomPositionAverage / rooms.Count;
        }

        public Sector(Sector s)
        {
            name = s.name;
            id = s.id;
            center = s.center;
            rooms = new List<Room>();
            foreach (Room r in s.rooms)
            {
                rooms.Add(new Room(r));
            }
        }

        public String IDString
        {
            get
            {
                return name + "_" + id;
            }
        }

        public Room FindRoomByIDString(string idString)
        {
            foreach (Room r in rooms)
            {
                if (idString == (r.name + "_" + r.id))
                {
                    return r;
                }
            }
            throw new Exception("Room with IDString[" + idString + "] not found");
        }
    }
}

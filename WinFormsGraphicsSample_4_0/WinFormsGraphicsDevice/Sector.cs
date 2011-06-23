using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormsGraphicsDevice
{
    public class Sector
    {
        public int id;
        public String name;
        public List<Room> rooms;

        public Sector()
        {
            name = "Sector";
            id = IDControl.GetID();
            rooms = new List<Room>();
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

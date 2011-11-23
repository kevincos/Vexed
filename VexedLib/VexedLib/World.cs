using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace VL
{
    //[Serializable]
    public class World
    {
        public int id;
        public List<Sector> sectors;

        public World()
        {
            id = IDControl.GetID();
            sectors = new List<Sector>();
        }

        public World(World w)
        {
            id = w.id;
            sectors = new List<Sector>();
            foreach (Sector s in w.sectors)
            {
                sectors.Add(new Sector(s));
            }
        }

        public Sector FindSectorByIDString(string idString)
        {
            foreach (Sector s in sectors)
            {
                if (idString == (s.IDString))
                {
                    return s;
                }
            }
            throw new Exception("Sector with IDString[" + idString + "] not found");
        }
    }
}

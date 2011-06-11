using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormsGraphicsDevice
{
    public class World
    {
        public int id;
        public List<Sector> sectors;

        public World()
        {
            id = IDControl.GetID();
            sectors = new List<Sector>();
        }

        public Sector FindSectorByIDString(string idString)
        {
            foreach(Sector s in sectors)
            {
                if(idString==(s.name+"_"+s.id))
                {
                    return s;
                }
            }
            throw new Exception("Sector with IDString[" + idString + "] not found");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VexedLib
{

    public class Decoration
    {
        public int id;
        public String name;
        public Vector3 position;
        public Vector3 up;
        public String texture;

        public Decoration()
        {
            name = "Decoration";
            id = IDControl.GetID();
            texture = "Default";
        }

        public void Init()
        {
                
        }

        public Decoration(Decoration d)
        {
            name = d.name;
            id = d.id;
            position = d.position;
            up = d.up;                
        }

        public String IDString
        {
            get
            {
                return name + "_" + id;
            }
        }

        public void Move(Vector3 delta)
        {
            position += delta;
        }
    
    }
}

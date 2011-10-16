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
        //public String name;
        public String _name;
        public Vector3 position;
        public Vector3 up;
        public String texture;

        public Decoration()
        {
            id = IDControl.GetID();
            _name = "Decoration_"+id;
            texture = "Default";
        }

        public void Init()
        {
                
        }

        public Decoration(Decoration d)
        {
            id = d.id;
            position = d.position;
            up = d.up;                
        }

        public void Update()
        {
            //_name = name + "_" + id;
        }

        public String IDString
        {
            get
            {
                return _name;
                //return name + "_" + id;
            }
        }

        public void Move(Vector3 delta)
        {
            position += delta;
        }
    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace VL
{
    //[Serializable]
    public class Decoration
    {
        public int id;
        //public String name;
        public String _name;
        public Vector3 position;
        public Vector3 up;
        public String texture;
        public int depth = 20;
        public Color color = Color.White;
        public bool wrap = false;
        public int startFrame = 0;
        public bool freespin = false;
        public bool reverseAnimation = false;

        public Decoration()
        {
            id = IDControl.GetID();
            _name = "Decoration_"+id;
            texture = "Default";            
        }

        public void Init()
        {
            id = IDControl.GetID();
            _name = "Decoration_" + id;
        }

        public Decoration(Decoration d)
        {
            texture = d.texture;
            id = d.id;
            position = d.position;
            up = d.up;
            color = d.color;
            wrap = d.wrap;
            freespin = d.freespin;
            startFrame = 0;
            reverseAnimation = d.reverseAnimation;
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

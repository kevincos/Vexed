using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace VL
{
    //[Serializable]
    public class Behavior
    {
        public int id;
        //public String name;
        public String _name;
        public int offset;
        public int duration;
        public int period;
        public Vector3 destination;
        public int primaryValue;
        public int secondaryValue;
        public bool toggle;
        public String nextBehavior;

        public Behavior()
        {
            id = IDControl.GetID();
            _name = "Behavior_"+id;
        }

        public Behavior(Behavior b)
        {
            _name = b._name;
            id = b.id;
            offset = b.offset;
            duration = b.duration;
            period = b.period;
            destination = b.destination;
            primaryValue = b.primaryValue;
            secondaryValue = b.secondaryValue;
            toggle = b.toggle;
            nextBehavior = b.nextBehavior;
        }

        public void Init()
        {
            id = IDControl.GetID();
            if(_name == null)
                _name = "Behavior_" + id;
        }

        public void Update()
        {
            
        }

        public String IDString
        {
            get
            {
                return _name;
                //return name + "_" + id;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    public class Behavior
    {
        public int id;
        public String name;
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
            name = "Behavior";
            id = IDControl.GetID();
        }

        public Behavior(Behavior b)
        {
            id = b.id;
            name = b.name;
            offset = b.offset;
            duration = b.duration;
            period = b.period;
            destination = b.destination;
            primaryValue = b.primaryValue;
            secondaryValue = b.secondaryValue;
            toggle = b.toggle;
            nextBehavior = b.nextBehavior;
        }

        public String IDString
        {
            get
            {
                return name + "_" + id;
            }
        }
    }
}

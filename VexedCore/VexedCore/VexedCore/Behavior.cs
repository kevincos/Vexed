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

namespace VexedCore
{
    public class Behavior
    {
        public String id = "";
        public String nextBehavior;
        public Vector3 destination;
        public int duration;
        public int offSet;
        public int period;
        public int primaryValue;
        public int secondaryValue;
        public bool toggle;

        public Behavior(VexedLib.Behavior xmlBehavior)
        {
            id = xmlBehavior.IDString;
            destination = xmlBehavior.destination;
            duration = xmlBehavior.duration;
            nextBehavior = xmlBehavior.nextBehavior;
            primaryValue = xmlBehavior.primaryValue;
            secondaryValue = xmlBehavior.secondaryValue;
            offSet = xmlBehavior.offset;
            period = xmlBehavior.period;
            toggle = xmlBehavior.toggle;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VexedLib
{
    public enum EdgeType
    {
        Normal = 0,
        Spikes = 1,
        Ice = 2,
        ConveyorBelt = 3,
        Bounce = 4,
        Electric = 5,
        Magnet = 6
    }

    public class Edge
    {
        public int id;
        public String name;
        public Vector3 start;
        public Vector3 end;
        public EdgeType type;

        public List<Behavior> behaviors;

        public Edge()
        {
            type = EdgeType.Normal;
            name = "Edge";
            id = 0;
            behaviors = new List<Behavior>();
        }

        public Edge(Edge e)
        {
            id = e.id;
            name = e.name;
            start = e.start;
            end = e.end;
            type = e.type;
            behaviors = new List<Behavior>();
            foreach (Behavior b in e.behaviors)
            {
                behaviors.Add(new Behavior(b));
            }
        }

        public void Init()
        {
            id = IDControl.GetID();            
            behaviors.Add(new Behavior());
        }

        public String IDString
        {
            get
            {
                return name + "_" + id;
            }
        }

        public Edge(Vector3 s, Vector3 e)
        {
            behaviors = new List<Behavior>();
            name = "Edge";            
            start = s;
            end = e;
        }

        public void Move(Vector3 delta)
        {
            start += delta;
            end += delta;
        }

        public Behavior FindBehaviorByIDString(String idString)
        {
            foreach (Behavior b in behaviors)
            {
                if (b.IDString == idString)
                    return b;
            }
            return null;
        }
    }
}

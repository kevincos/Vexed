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
using System.Xml.Serialization;

namespace VexedCore
{
    public class EdgeProperties
    {
        public VexedLib.EdgeType type = VexedLib.EdgeType.Normal;
        public int primaryValue =0;
        public int secondaryValue =0;
    }

    public class Edge
    {
        public Vertex start;
        public Vertex end;

        public EdgeProperties properties;
        public List<Behavior> behaviors;
        [XmlIgnore]public Behavior currentBehavior = null;
        public String currentBehaviorId = null;
        public int currentTime = 0;
        public bool nextBehavior = false;
        public bool behaviorStarted = false;
        public bool toggleOn = true;
        public string id;

        public Edge()
        {
            start = new Vertex();
            end = new Vertex();
            properties = new EdgeProperties();
            properties.type = VexedLib.EdgeType.Normal;
            behaviors = new List<Behavior>();
        }

        public Edge(Edge e)
        {
            id = e.id;
            start = new Vertex(e.start);
            end = new Vertex(e.end);
            properties = e.properties;
            currentBehavior = null;
            currentBehaviorId = e.currentBehaviorId;
            if(e.currentBehavior != null)
                currentBehaviorId = e.currentBehavior.id;
            currentTime = e.currentTime;
            nextBehavior = e.nextBehavior;
            behaviorStarted = e.behaviorStarted;
            toggleOn = e.toggleOn;
            behaviors = new List<Behavior>();
            foreach (Behavior b in e.behaviors)
            {
                behaviors.Add(new Behavior(b));
            }
        }

        public Edge(VexedLib.Edge xmlEdge, Vector3 normal)
        {
            start = new Vertex(xmlEdge.start, normal, Vector3.Zero, xmlEdge.end - xmlEdge.start);
            end = new Vertex(xmlEdge.end, normal, Vector3.Zero, xmlEdge.start - xmlEdge.end);
            
            start.direction.Normalize();
            end.direction.Normalize();
            start.normal.Normalize();
            end.normal.Normalize();

            properties = new EdgeProperties();
            properties.type = xmlEdge.type;

            behaviors = new List<Behavior>();

            id = xmlEdge.IDString;
        }

        public Edge(Vector3 s, Vector3 e, Vector3 normal)
        {
            start = new Vertex(s, normal, Vector3.Zero, e-s);
            end = new Vertex(e, normal, Vector3.Zero, s-e);

            start.direction.Normalize();
            end.direction.Normalize();
            start.normal.Normalize();
            end.normal.Normalize();

            properties = new EdgeProperties();
            properties.type = VexedLib.EdgeType.Normal;

            behaviors = new List<Behavior>();
        }

        public int UpdateBehavior(GameTime gameTime)
        {
            if (currentBehavior == null)
                return 0;
            if (behaviorStarted == false && currentTime > currentBehavior.offSet)
            {
                properties.primaryValue = currentBehavior.primaryValue;
                properties.secondaryValue = currentBehavior.secondaryValue;
                currentTime = gameTime.ElapsedGameTime.Milliseconds;
                behaviorStarted = true;
                nextBehavior = false;
            }
            if (nextBehavior == true && behaviorStarted == true)
            {
                behaviorStarted = true;
                foreach (Behavior b in behaviors)
                {
                    if (b.id == currentBehavior.nextBehavior)
                    {
                        currentBehavior = b;
                        break;
                    }
                }
                properties.primaryValue = currentBehavior.primaryValue;
                properties.secondaryValue = currentBehavior.secondaryValue;
                currentTime = 0;
                nextBehavior = false;
                return gameTime.ElapsedGameTime.Milliseconds;
            }
            currentTime += gameTime.ElapsedGameTime.Milliseconds;
            if (behaviorStarted)
            {
                if (currentBehavior.duration != 0 && currentTime > currentBehavior.duration)
                {
                    nextBehavior = true;
                    return currentBehavior.duration - (currentTime - gameTime.ElapsedGameTime.Milliseconds);
                }
                if (currentBehavior.period != 0 && currentTime > currentBehavior.period)
                {
                    currentTime = 0;
                    toggleOn = !toggleOn;
                    if (!toggleOn)
                    {
                        properties.primaryValue = 0;
                        properties.secondaryValue = 0;
                    }
                    else
                    {
                        properties.primaryValue = currentBehavior.primaryValue;
                        properties.secondaryValue = currentBehavior.secondaryValue;
                    }                    
                }
            }
            return gameTime.ElapsedGameTime.Milliseconds;
        }

        public void SetBehavior(Behavior b)
        {
            currentBehavior = b;
            if (!toggleOn)
            {
                properties.primaryValue = 0;
                properties.secondaryValue = 0;
            }
            else
            {
                properties.primaryValue = currentBehavior.primaryValue;
                properties.secondaryValue = currentBehavior.secondaryValue;
            }    
            currentTime = 0;
            nextBehavior = false;
        }

        public void UpdateBehavior()
        {
            if (currentBehavior == null)
            {
                currentBehavior = behaviors[0];
                if (currentBehavior.offSet == 0)
                {
                    properties.primaryValue = currentBehavior.primaryValue;
                    properties.secondaryValue = currentBehavior.secondaryValue;
                    behaviorStarted = true;
                }
            }
        }
    }
}

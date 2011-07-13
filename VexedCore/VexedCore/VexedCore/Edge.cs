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
    public class EdgeProperties
    {
        public VexedLib.EdgeType type;
        public int primaryValue =0;
        public int secondaryValue =0;
    }

    public class Edge
    {
        public Vertex start;
        public Vertex end;

        public EdgeProperties properties;
        public List<Behavior> behaviors;
        public Behavior currentBehavior = null;
        public int currentTime = 0;
        bool nextBehavior = false;
        bool behaviorStarted = false;
        bool toggleOn = true;

        public Edge()
        {
            start = new Vertex();
            end = new Vertex();
            properties = new EdgeProperties();
            properties.type = VexedLib.EdgeType.Normal;
            behaviors = new List<Behavior>();
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
        }

        public int UpdateBehavior(GameTime gameTime)
        {
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
                currentTime = gameTime.ElapsedGameTime.Milliseconds;
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

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
    public class Block
    {
        bool nextBehavior;

        public Block(VexedLib.Block xmlBlock)
        {
            edges = new List<Edge>();
            behaviors = new List<Behavior>();
            color = xmlBlock.color;
        }

        public int UpdateBehavior(GameTime gameTime)
        {
            if (nextBehavior == true)
            {
                foreach (Behavior b in behaviors)
                {
                    if (b.id == currentBehavior.nextBehavior)
                    {
                        currentBehavior = b;
                        break;
                    }
                }
                foreach (Edge e in edges)
                {
                    if (currentBehavior.duration != 0)
                    {
                        e.start.velocity = currentBehavior.destination / currentBehavior.duration;
                        e.end.velocity = currentBehavior.destination / currentBehavior.duration;
                    }
                }
                currentTime = gameTime.ElapsedGameTime.Milliseconds;
                nextBehavior = false;
                return gameTime.ElapsedGameTime.Milliseconds;
            }
            currentTime += gameTime.ElapsedGameTime.Milliseconds;
            if (currentTime > currentBehavior.duration)
            {                
                nextBehavior = true;
                return currentBehavior.duration - (currentTime - gameTime.ElapsedGameTime.Milliseconds);
            }
            return gameTime.ElapsedGameTime.Milliseconds;
        }
       
        public void UpdateBehavior()
        {
            if (currentBehavior == null)
            {
                currentBehavior = behaviors[0];
                if (currentBehavior.destination != Vector3.Zero)
                {
                    foreach (Edge e in edges)
                    {
                        if (currentBehavior.duration != 0)
                        {
                            e.start.velocity = currentBehavior.destination / currentBehavior.duration;
                            e.end.velocity = currentBehavior.destination / currentBehavior.duration;
                        }
                    }
                    currentTime = 0;
                }
            }
        }

        public List<Edge> edges;
        public List<Behavior> behaviors;

        public Behavior currentBehavior = null;        
        public int currentTime = 0;
        public Color color;
    }
}

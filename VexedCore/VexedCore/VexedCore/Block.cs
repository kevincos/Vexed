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
        public bool staticObject = true;
        bool nextBehavior;
        public float boundingBoxTop = 0;
        public float boundingBoxLeft = 0;
        public float boundingBoxRight = 0;
        public float boundingBoxBottom = 0;

        public Block()
        {
            edges = new List<Edge>();
        }

        public Block(VexedLib.Block xmlBlock)
        {
            edges = new List<Edge>();
            behaviors = new List<Behavior>();
            color = xmlBlock.color;
        }

        public void UpdateBoundingBox(Vector3 up, Vector3 right)
        {
            boundingBoxBottom = Vector3.Dot(up, edges[0].start.position);
            boundingBoxTop = Vector3.Dot(up, edges[0].start.position);
            boundingBoxLeft = Vector3.Dot(right, edges[0].start.position);
            boundingBoxRight = Vector3.Dot(right, edges[0].start.position);
            for (int i = 1; i < edges.Count; i++)
            {
                float y = Vector3.Dot(up, edges[i].start.position);
                float x = Vector3.Dot(right, edges[i].start.position);
                if (x < boundingBoxLeft)
                    boundingBoxLeft = x;                
                if (x > boundingBoxRight)
                    boundingBoxRight = x;
                if (y < boundingBoxBottom)
                    boundingBoxBottom = y;
                if (y > boundingBoxTop)
                    boundingBoxTop = y;
            }
        }

        public Block(List<Vertex> vList, List<EdgeProperties> edgePropertiesList, Room r, Vector3 n, Vector3 u)
        {
            Vector3 anchor = r.center + Math.Abs(Vector3.Dot(r.size/2,n))*n;
            edges = new List<Edge>();
            for (int i = 0; i < 4; i++)
            {                
                Edge newEdge = new Edge();
                newEdge.start = vList[i].Unfold(r, n, u);
                newEdge.end = vList[(i+1)%4].Unfold(r, n, u);
                newEdge.properties = edgePropertiesList[i];
                edges.Add(newEdge);
            }
        }

        public List<Vector3> GetCollisionRect()
        {
            List<Vector3> blockVertexList = new List<Vector3>();
            foreach (Edge e in edges)
            {
                if (e.start.position != e.end.position)
                    blockVertexList.Add(e.start.position);
            }
            return blockVertexList;
        }

        public Vector3 GetVelocity()
        {
            return edges[0].start.velocity;
        }

        public EdgeProperties GetProperties(Vector3 projection)
        {
            EdgeProperties properties = new EdgeProperties();
            properties.type = VexedLib.EdgeType.Normal;
            foreach (Edge e in edges)
            {
                Vector3 edgeNormal = Vector3.Cross(e.start.normal, e.start.position - e.end.position);
                edgeNormal.Normalize();
                Vector3 projectionNormal = projection / projection.Length();
                float result = Vector3.Dot(edgeNormal, projectionNormal);
                if (result == 1)
                {
                    properties = e.properties;
                }
            }
            return properties;
        }

        public int UpdateBehavior(GameTime gameTime)
        {
            if (currentBehavior == null)
                return 0;
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

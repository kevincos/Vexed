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
        public bool nextBehavior;
        public float boundingBoxTop = 0;
        public float boundingBoxLeft = 0;
        public float boundingBoxRight = 0;
        public float boundingBoxBottom = 0;

        public List<Edge> edges;
        public List<Behavior> behaviors;

        public List<Block> unfoldedBlocks;

        public Behavior currentBehavior = null;
        public String currentBehaviorId;
        public int currentTime = 0;
        public Color color;
        public String id;

        public Block()
        {
            unfoldedBlocks = new List<Block>();
            edges = new List<Edge>();
        }

        public Block(Block b)
        {
            unfoldedBlocks = new List<Block>();
            if (b.unfoldedBlocks != null)
            {
                foreach (Block oldUnfoldedBlock in b.unfoldedBlocks)
                    unfoldedBlocks.Add(new Block(oldUnfoldedBlock));
            }
            id = b.id;
            staticObject = b.staticObject;
            nextBehavior = b.nextBehavior;
            color = b.color;
            currentTime = b.currentTime;
            currentBehaviorId = b.currentBehaviorId;
            if(b.currentBehavior != null)
                currentBehaviorId = b.currentBehavior.id;
            edges = new List<Edge>();
            behaviors = new List<Behavior>();
            foreach (Edge e in b.edges)
            {
                edges.Add(new Edge(e));
            }
            if (b.behaviors != null)
            {
                foreach (Behavior behavior in b.behaviors)
                {
                    behaviors.Add(new Behavior(behavior));
                }
            }
        }

        public Block(VexedLib.Block xmlBlock)
        {
            unfoldedBlocks = new List<Block>();
            edges = new List<Edge>();
            behaviors = new List<Behavior>();
            color = xmlBlock.color;
            id = xmlBlock.IDString;
        }

        public void UpdateUnfoldedBlocks(Room r, Vector3 normal, Vector3 up)
        {
            List<Vertex> points = new List<Vertex>();
            List<EdgeProperties> edgeTypes = new List<EdgeProperties>();
            unfoldedBlocks.Clear();
            for (int i = 0; i < this.edges.Count; i++)
            {
                Edge e = this.edges[i];
                points.Add(e.start);
                edgeTypes.Add(e.properties);
                if (e.start.normal != e.end.normal && (e.start.normal != normal && e.end.normal != normal))
                {
                    Vector3 fullEdge = e.end.position - e.start.position;
                    Vector3 currentComponent = Vector3.Dot(e.end.normal, fullEdge) * e.end.normal;
                    Vector3 nextComponent = Vector3.Dot(e.start.normal, fullEdge) * e.start.normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(e.end.normal, e.start.normal), fullEdge) * Vector3.Cross(e.end.normal, e.start.normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = e.start.position + currentComponent + currentPercent * constantComponent;
                    points.Add(new Vertex(midPoint, e.start.normal, e.start.velocity, e.start.direction));
                    edgeTypes.Add(e.properties);
                    points.Add(new Vertex(midPoint, e.end.normal, e.end.velocity, e.end.direction));
                    edgeTypes.Add(this.edges[(i + 1) % this.edges.Count].properties);

                }
            }
            if (points.Count == 4)
            {
                unfoldedBlocks.Add(new Block(points, edgeTypes, r, normal, up));
            }
            else
            {
                Vector3 n1 = points[0].normal;
                List<Vertex> vList1 = new List<Vertex>();
                List<Vertex> vList2 = new List<Vertex>();
                for (int i = 0; i < 8; i++)
                {
                    if (points[i].normal == n1)
                        vList1.Add(points[i]);
                    else
                        vList2.Add(points[i]);
                }
                unfoldedBlocks.Add(new Block(vList1, edgeTypes, r, normal, up));
                unfoldedBlocks.Add(new Block(vList2, edgeTypes, r, normal, up));
            }

            foreach (Block b in unfoldedBlocks)
                b.UpdateBoundingBox(Engine.player.up, Engine.player.right);
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
                    else
                    {
                        e.start.velocity = Vector3.Zero;
                        e.end.velocity = Vector3.Zero;
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

        public void SetBehavior(Behavior b)
        {
            currentBehavior = b;
            
            currentTime = 0;
            nextBehavior = false;

            foreach (Edge e in edges)
            {
                if (currentBehavior.duration != 0)
                {
                    e.start.velocity = currentBehavior.destination / currentBehavior.duration;
                    e.end.velocity = currentBehavior.destination / currentBehavior.duration;
                }
                else
                {
                    e.start.velocity = Vector3.Zero;
                    e.end.velocity = Vector3.Zero;
                }
            }
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

        public List<VertexPositionColorNormalTexture> baseTriangleList;

        public Color GetCurrentColor(Room currentRoom)
        {
            Color powerUpColor = color;
            if (currentRoom.maxOrbs != 0)
            {
                powerUpColor.R = (Byte)(40 + currentRoom.currentOrbs * (color.R - 40) / currentRoom.maxOrbs);
                powerUpColor.G = (Byte)(40 + currentRoom.currentOrbs * (color.G - 40) / currentRoom.maxOrbs);
                powerUpColor.B = (Byte)(40 + currentRoom.currentOrbs * (color.B - 40) / currentRoom.maxOrbs);
            }
            return powerUpColor;            
        }

        public void UpdateVertexData(Room currentRoom)
        {
            
            if (baseTriangleList == null || staticObject == false)
            {
                baseTriangleList = new List<VertexPositionColorNormalTexture>();
            
                List<Vertex> vList = new List<Vertex>();
                vList.Add(edges[0].start);
                vList.Add(edges[1].start);
                vList.Add(edges[2].start);
                vList.Add(edges[3].start);
                Color powerUpColor = GetCurrentColor(currentRoom);
                currentRoom.AddBlockToTriangleList2(vList, powerUpColor, .5f, baseTriangleList);
            }
            if (currentRoom.refreshVertices == true)
            {
                List<VertexPositionColorNormalTexture> newColors = new List<VertexPositionColorNormalTexture>();
                Color powerUpColor = GetCurrentColor(currentRoom);

                for (int i = 0; i < baseTriangleList.Count; i++)
                {
                    newColors.Add(new VertexPositionColorNormalTexture(baseTriangleList[i].Position, powerUpColor, baseTriangleList[i].Normal, baseTriangleList[i].TextureCoordinates));
                }
                baseTriangleList.Clear();
                baseTriangleList = newColors;
                Engine.reDraw = true;
            }
        }

        public void Draw(Room currentRoom)
        {
            UpdateVertexData(currentRoom);
            if (staticObject == true && Engine.player.currentRoom != currentRoom)
            {
                for (int i = 0; i < baseTriangleList.Count; i++)
                {
                    Engine.staticBlockVertexArray[Engine.staticBlockVertexArrayCount + i] = baseTriangleList[i];
                }
                Engine.staticBlockVertexArrayCount += baseTriangleList.Count;
            }
            else
            {
                for (int i = 0; i < baseTriangleList.Count; i++)
                {
                    Engine.dynamicBlockVertexArray[Engine.dynamicBlockVertexArrayCount + i] = baseTriangleList[i];
                }
                Engine.dynamicBlockVertexArrayCount += baseTriangleList.Count;
            }
        }

    
    }
}

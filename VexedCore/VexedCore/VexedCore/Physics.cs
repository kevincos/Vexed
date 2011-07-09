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
    class Physics
    {
        public static List<Block> BlockUnfold(Room r, Vector3 normal, Vector3 up)
        {
            List<Block> unfoldedBlockList = new List<Block>();
            
            foreach (Block b in r.blocks)
            {
                List<Vertex> points = new List<Vertex>();
                foreach (Edge e in b.edges)
                {
                    points.Add(e.start);
                    if (e.start.normal != e.end.normal)                        
                    {
                        Vector3 fullEdge = e.end.position - e.start.position;
                        Vector3 currentComponent = Vector3.Dot(e.end.normal, fullEdge) * e.end.normal;
                        Vector3 nextComponent = Vector3.Dot(e.start.normal, fullEdge) * e.start.normal;
                        Vector3 constantComponent = Vector3.Dot(Vector3.Cross(e.end.normal, e.start.normal), fullEdge) * Vector3.Cross(e.end.normal, e.start.normal);
                        float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                        Vector3 midPoint = e.start.position + currentComponent + currentPercent * constantComponent;
                        points.Add(new Vertex(midPoint, e.start.normal, e.start.velocity, e.start.direction));
                        points.Add(new Vertex(midPoint, e.end.normal, e.end.velocity, e.end.direction));                        
                    }
                }
                if (points.Count == 4)
                {
                    unfoldedBlockList.Add(new Block(points, r, normal, up));
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
                    unfoldedBlockList.Add(new Block(vList1, r, normal, up));
                    unfoldedBlockList.Add(new Block(vList2, r, normal, up));
                }
            }
            return unfoldedBlockList;
        }

        public static void DebugDraw(Room r, Vector3 normal, Vector3 up)
        {
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<Block> debugList = Physics.BlockUnfold(r, normal, up);
            foreach (Block b in debugList)
            {
                List<Vertex> vList = new List<Vertex>();
                vList.Add(b.edges[0].start);
                vList.Add(b.edges[1].start);
                vList.Add(b.edges[2].start);
                vList.Add(b.edges[3].start);
                r.AddBlockToTriangleList(vList, Color.Yellow, 0f, triangleList);
                 
                //Draw projection Normals
                for (int i = 0; i < 4; i++)
                {
                    triangleList.Add(new VertexPositionColorNormal(b.edges[i].start.position, Color.Red, normal));
                    triangleList.Add(new VertexPositionColorNormal(b.edges[i].end.position, Color.Red, normal));
                    Vector3 projectionNormal = -Vector3.Cross(normal, b.edges[i].end.position - b.edges[i].start.position);
                    projectionNormal.Normalize();
                    triangleList.Add(new VertexPositionColorNormal(projectionNormal + .5f * (b.edges[i].end.position + b.edges[i].start.position), Color.Red, normal));
                }
            }
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleList.ToArray(), 0, triangleList.Count / 3, VertexPositionColorNormal.VertexDeclaration);
        }

        public static void CollisionCheck(Room r, Player p, GameTime gameTime)
        {
            List<Block> unfoldedBlocks = Physics.BlockUnfold(r, p.center.normal, p.center.direction);
            List<Vector3> playerVertexList = new List<Vector3>();
            Vector3 up = p.center.direction;
            Vector3 right = Vector3.Cross(up, p.center.normal);


            playerVertexList.Add(p.center.position + .5f * up + .5f * right);            
            playerVertexList.Add(p.center.position + .5f * up - .5f * right);
            playerVertexList.Add(p.center.position - .5f * up - .5f * right);            
            playerVertexList.Add(p.center.position - .5f * up + .5f * right);

            List<Vector3> contributions = new List<Vector3>();
            Vector3 totalPlayerProjection = Vector3.Zero;
            int totalPlayerContributions = 0;

            foreach (Block b in unfoldedBlocks)
            {
                // if block intesects with rectVertexList
                List<Vector3> blockVertexList = new List<Vector3>();
                foreach (Edge e in b.edges)
                {
                    if(e.start.position != e.end.position)
                        blockVertexList.Add(e.start.position);
                }                
                
                Vector3 minProjectionNormal = Vector3.Zero;
                float minProjectionValue = 0f;
                bool collision_free = false;
                bool second_pass = false;

                // Block -> Player
                for(int i = 0; i < blockVertexList.Count(); i++) // Iterate over block edges
                {
                    Vector3 projectionNormal = -Vector3.Cross(p.center.normal, blockVertexList[(i+1)%blockVertexList.Count()] - blockVertexList[i]);
                    projectionNormal.Normalize();
                    float separatorValue = Vector3.Dot(projectionNormal, blockVertexList[i]);

                    
                    // Iterate over player points
                    float maxPointDistance = 0;
                    for(int j =0 ;j < playerVertexList.Count(); j++)
                    {
                        float pointValue = Vector3.Dot(projectionNormal, playerVertexList[j]);
                        float delta = separatorValue - pointValue;
                        if (j == 0 || delta > maxPointDistance)
                            maxPointDistance = delta;                        
                    }
                    if (maxPointDistance > 0)
                    {
                        if (minProjectionNormal == Vector3.Zero || (minProjectionNormal != Vector3.Zero && maxPointDistance < minProjectionValue))
                        {
                            minProjectionNormal = projectionNormal;
                            minProjectionValue = maxPointDistance;
                        }
                    }
                    else
                    {
                        minProjectionNormal = Vector3.Zero;
                        minProjectionValue = 0;
                        collision_free = true;
                        break;
                    }

                    /*
                    // Iterate over player points
                    bool possible_collision = false;
                    for(int j =0 ;j < playerVertexList.Count(); j++)
                    {
                        float pointValue = Vector3.Dot(projectionNormal, playerVertexList[j]);
                        float delta = separatorValue - pointValue;
                        if(delta > 0)
                        {
                            possible_collision = true;
                            if(minProjectionNormal == Vector3.Zero || (minProjectionNormal != Vector3.Zero && delta < minProjectionValue))
                            {
                                minProjectionNormal = projectionNormal;
                                minProjectionValue = delta;
                            }
                        }
                    }
                    // If no points cross the separator, no collision is taking place. Reset projectionNormal
                    if (possible_collision == false)
                    {
                        minProjectionNormal = Vector3.Zero;
                        minProjectionValue = 0;
                        collision_free = true;
                        break;
                    }*/
                }
                               

                if (collision_free == false)
                {
                    totalPlayerProjection += minProjectionNormal * minProjectionValue;
                    totalPlayerContributions++;
                    contributions.Add(minProjectionNormal * minProjectionValue);

                }
            }

            if (totalPlayerContributions > 0)
            {
                totalPlayerProjection = totalPlayerProjection / totalPlayerContributions;

                Vector3 projectionDirection = totalPlayerProjection / totalPlayerProjection.Length();

                float badVelocityComponent = Vector3.Dot(projectionDirection, p.center.velocity);
                if(badVelocityComponent < 0)
                    p.center.velocity -= badVelocityComponent * projectionDirection;
                
                p.center.position += totalPlayerProjection;// / totalPlayerContributions;
            }



            foreach (JumpPad j in r.jumpPads)
            {
                if ((j.position.position - p.center.position).Length() < .5f)
                    j.active = true;
                else
                    j.active = false;
            }

            foreach (Bridge b in r.bridges)
            {
                if ((b.position.position - p.center.position).Length() < .5f)
                    b.active = true;
                else
                    b.active = false;
            }

        }

    }
}

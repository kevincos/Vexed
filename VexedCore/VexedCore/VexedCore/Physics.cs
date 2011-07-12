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
                    //if (e.start.normal != e.end.normal)                        
                    if(e.start.normal != e.end.normal && (e.start.normal != normal && e.end.normal != normal))
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
                    /*triangleList.Add(new VertexPositionColorNormal(b.edges[i].start.position, Color.Red, normal));
                    triangleList.Add(new VertexPositionColorNormal(b.edges[i].end.position, Color.Red, normal));
                    Vector3 projectionNormal = -Vector3.Cross(normal, b.edges[i].end.position - b.edges[i].start.position);
                    projectionNormal.Normalize();
                    triangleList.Add(new VertexPositionColorNormal(projectionNormal + .5f * (b.edges[i].end.position + b.edges[i].start.position), Color.Red, normal));
                     */                    
                    Vector3 velocityNormal = b.edges[i].start.velocity;                    
                    velocityNormal.Normalize();
                    Vector3 sideNormal = Vector3.Cross(velocityNormal, normal);
                    triangleList.Add(new VertexPositionColorNormal(b.edges[i].start.position + .2f*sideNormal, Color.Red, normal));
                    triangleList.Add(new VertexPositionColorNormal(b.edges[i].start.position - .2f *sideNormal, Color.Red, normal));
                    triangleList.Add(new VertexPositionColorNormal(b.edges[i].start.position + velocityNormal, Color.Red, normal));
                }
                
            }
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleList.ToArray(), 0, triangleList.Count / 3, VertexPositionColorNormal.VertexDeclaration);
        }

        public static Vector3 Collide(List<Vector3> itemVertexList, List<Vector3> blockVertexList, Vector3 normal)
        {
            Vector3 minProjectionNormal = Vector3.Zero;
            float minProjectionValue = 0;
            bool collision_free = false;

            for (int i = 0; i < blockVertexList.Count(); i++) // Iterate over block edges
            {
                Vector3 projectionNormal = -Vector3.Cross(normal, blockVertexList[(i + 1) % blockVertexList.Count()] - blockVertexList[i]);
                projectionNormal.Normalize();
                float separatorValue = Vector3.Dot(projectionNormal, blockVertexList[i]);

                // Iterate over player points
                float maxPointDistance = 0;
                for (int j = 0; j < itemVertexList.Count(); j++)
                {
                    float pointValue = Vector3.Dot(projectionNormal, itemVertexList[j]);
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
            }

            // Player -> Block
            if (collision_free == false)
            {
                for (int i = 0; i < itemVertexList.Count(); i++) // Iterate over block edges
                {
                    Vector3 projectionNormal = -Vector3.Cross(normal, itemVertexList[(i + 1) % itemVertexList.Count()] - itemVertexList[i]);
                    projectionNormal.Normalize();
                    float separatorValue = Vector3.Dot(projectionNormal, itemVertexList[i]);

                    // Iterate over player points
                    float maxPointDistance = 0;
                    for (int j = 0; j < blockVertexList.Count(); j++)
                    {
                        float pointValue = Vector3.Dot(projectionNormal, blockVertexList[j]);
                        float delta = separatorValue - pointValue;
                        if (j == 0 || delta > maxPointDistance)
                            maxPointDistance = delta;
                    }
                    if (maxPointDistance > 0)
                    {
                        if (minProjectionNormal == Vector3.Zero || (minProjectionNormal != Vector3.Zero && maxPointDistance < minProjectionValue))
                        {
                            minProjectionNormal = -projectionNormal;
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
                }
            }
            return minProjectionNormal * minProjectionValue;
        }

        /* Test for collision between player and blocks, and adjust player's position and velocity accordingly.
         * There's some tricky order of operations here that is explained inline*/
        public static void CollisionCheck(Room r, Player p, GameTime gameTime)
        {
            // Variables used for entire collision test method.
            List<Block> unfoldedBlocks = Physics.BlockUnfold(r, p.center.normal, p.center.direction);
            List<Vector3> playerVertexList = new List<Vector3>();
            List<Vector3> playerGroundBox = new List<Vector3>();
            List<Vector3> playerLeftBox = new List<Vector3>();
            List<Vector3> playerRightBox = new List<Vector3>();
            Vector3 up = p.center.direction;
            Vector3 right = Vector3.Cross(up, p.center.normal);

            /* Stores the result of friction calculations, to be applied at the END of the function. Must be done at the end
             * because during the algorithm, it isn't fully determined what the player's state is, and friction is only applied
             * in certain cases. Specifically, it isn't known if the player has landed on the ground until the algorithm completes.
             */
             
            Vector3 frictionAdjustment = Vector3.Zero;            
            
            /* Perform the basic algorithm several times, each time taking only the most powerful contribution. I'm assuming for now
             * that it is EXTREMELY unlikely for the player to have meaningful collisions simultaneously with > 3 blocks.
             * The purpose of this is that sometimes two collisions will be detected, but only one needs to be resolved. For example,
             * if two blocks are adjacent to one another such that their edges form a single, straight edge, we only need (and want)
             * to use the projection from one of them*/
            for (int attempt = 0; attempt < 2; attempt++)
            {
                List<Vector3> projectionList = new List<Vector3>();
                List<Vector3> relVelList = new List<Vector3>();

                playerVertexList = new List<Vector3>();
                playerVertexList.Add(p.center.position + .5f * up + .5f * right);
                playerVertexList.Add(p.center.position + .5f * up - .5f * right);
                playerVertexList.Add(p.center.position - .5f * up - .5f * right);
                playerVertexList.Add(p.center.position - .5f * up + .5f * right);

                foreach (Block b in unfoldedBlocks)
                {
                    // if block intesects with rectVertexList
                    List<Vector3> blockVertexList = new List<Vector3>();
                    foreach (Edge e in b.edges)
                    {
                        if (e.start.position != e.end.position)
                            blockVertexList.Add(e.start.position);
                    }

                    // Actual collision detection between two polygons happens here.
                    Vector3 projection = Collide(playerVertexList, blockVertexList, p.center.normal);

                    if (projection.Length() > 0f)
                    {
                        // If a collision is found, save the necessary data and continue.
                        projectionList.Add(projection);
                        relVelList.Add(b.edges[0].start.velocity);   
                    }
                }

                // Compute the most powerful collision and resolve it.
                Vector3 maxProjection = Vector3.Zero;
                Vector3 relVel = Vector3.Zero;
                for (int i = 0; i < projectionList.Count(); i++)
                {
                    if (projectionList[i].Length() > maxProjection.Length())
                    {
                        maxProjection = projectionList[i];
                        relVel = relVelList[i];
                    }
                }
                if (maxProjection != Vector3.Zero)
                {
                    Vector3 projectionDirection = maxProjection / maxProjection.Length();

                    float badVelocityComponent = Vector3.Dot(projectionDirection, p.center.velocity - relVel);
                    
                    //if (badVelocityComponent < -0.01f)
                    if (badVelocityComponent < -0.0f)
                        p.center.velocity -= badVelocityComponent * projectionDirection;

                    float projectionUpComponent = Vector3.Dot(projectionDirection, p.center.direction);
                    if (projectionUpComponent > 0)
                    {
                        Vector3 frictionDirection = Vector3.Cross(Vector3.Dot(projectionDirection, p.center.direction) * p.center.direction, p.center.normal);

                        float frictionVelocityComponent = Vector3.Dot(frictionDirection, p.center.velocity - relVel);

                        if (Math.Abs(frictionVelocityComponent) < .02f)
                        {
                            frictionAdjustment -= frictionVelocityComponent * frictionDirection;
                        }
                        else if (frictionVelocityComponent > 0)
                        {
                            frictionAdjustment -= .02f * frictionDirection;
                        }
                        else
                        {
                            frictionAdjustment += .02f * frictionDirection;
                        }
                    }

                    p.center.position += maxProjection;
                }
                else
                    break;
            }

            // Now that player position is stabilized, use the special rects to detect if it is grounded
            // or prepped for a wall jump.

            playerGroundBox.Add(p.center.position + .5f * right);
            playerGroundBox.Add(p.center.position - .5f * right);
            playerGroundBox.Add(p.center.position - .6f * up - .5f * right);
            playerGroundBox.Add(p.center.position - .6f * up + .5f * right);
            playerLeftBox.Add(p.center.position);
            playerLeftBox.Add(p.center.position - .6f * right);
            playerLeftBox.Add(p.center.position - .4f * up - .6f * right);
            playerLeftBox.Add(p.center.position - .4f * up);
            playerRightBox.Add(p.center.position + .6f * right);
            playerRightBox.Add(p.center.position);
            playerRightBox.Add(p.center.position - .4f * up);
            playerRightBox.Add(p.center.position - .4f * up + .6f * right);

            p.leftWall = false;
            p.rightWall = false;

            p.platformVelocity = Vector3.Zero;
            foreach (Block b in unfoldedBlocks)
            {
                // if block intesects with rectVertexList
                List<Vector3> blockVertexList = new List<Vector3>();
                foreach (Edge e in b.edges)
                {
                    if (e.start.position != e.end.position)
                        blockVertexList.Add(e.start.position);
                }

                Vector3 groundProjection = Collide(playerGroundBox, blockVertexList, p.center.normal);
                Vector3 leftProjection = Collide(playerLeftBox, blockVertexList, p.center.normal);
                Vector3 rightProjection = Collide(playerRightBox, blockVertexList, p.center.normal);


                if (Vector3.Dot(groundProjection, up) > 0)
                {
                    p.grounded = true;
                    p.platformVelocity = b.edges[0].start.velocity;
                }
                if (Vector3.Dot(leftProjection, right) > 0)
                {
                    p.leftWall = true;
                }
                if (Vector3.Dot(rightProjection, -right) != 0)
                {
                    p.rightWall = true;
                }
            }

            // Now that we know if the player is grounded or not, we can use the walking property to determine whether or
            // not we should apply friction.
            if (!p.walking)
                p.center.velocity += frictionAdjustment;

            p.center.Update(r, 0);

            // Test and activate doodads.
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

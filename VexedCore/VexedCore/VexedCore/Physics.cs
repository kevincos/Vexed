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
        public static Room BlockUnfold(Room r, Vector3 normal, Vector3 up)
        {
            Vector3 right = Vector3.Cross(up, normal);
            Room unfoldedRoom = new Room();
            foreach (Doodad d in r.doodads)
            {
                Doodad unfoldedDoodad = new Doodad(d, r, normal, up);
                unfoldedRoom.doodads.Add(unfoldedDoodad);
            }
            foreach (Block b in r.blocks)
            {
                List<Vertex> points = new List<Vertex>();
                List<EdgeProperties> edgeTypes = new List<EdgeProperties>();
                
                for(int i = 0; i < b.edges.Count; i++)
                {
                    Edge e = b.edges[i];
                    points.Add(e.start);
                    edgeTypes.Add(e.properties);
                    if(e.start.normal != e.end.normal && (e.start.normal != normal && e.end.normal != normal))
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
                        edgeTypes.Add(b.edges[(i + 1) % b.edges.Count].properties);
                     
                    }
                }
                if (points.Count == 4)
                {
                    unfoldedRoom.blocks.Add(new Block(points, edgeTypes, r, normal, up));
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
                    unfoldedRoom.blocks.Add(new Block(vList1, edgeTypes, r, normal, up));
                    unfoldedRoom.blocks.Add(new Block(vList2, edgeTypes, r, normal, up));
                }
            }
            foreach (Block b in unfoldedRoom.blocks)
            {
                b.UpdateBoundingBox(up, right);
            }
            foreach (Doodad d in unfoldedRoom.doodads)
            {
                d.UpdateBoundingBox(up, right);
            }
            return unfoldedRoom;
        }

        public static void DebugDraw(Room r, Vector3 normal, Vector3 up)
        {
            List<VertexPositionColorNormalTexture> triangleList = new List<VertexPositionColorNormalTexture>();
            List<Block> debugList = Physics.BlockUnfold(r, normal, up).blocks;
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
                    triangleList.Add(new VertexPositionColorNormalTexture(b.edges[i].start.position + .2f * sideNormal, Color.Red, normal, new Vector2(0,0)));
                    triangleList.Add(new VertexPositionColorNormalTexture(b.edges[i].start.position - .2f * sideNormal, Color.Red, normal, new Vector2(0, 0)));
                    triangleList.Add(new VertexPositionColorNormalTexture(b.edges[i].start.position + velocityNormal, Color.Red, normal, new Vector2(0, 0)));
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
            //return Vector3.Zero;
        }

        /* Test for collision between player and blocks, and adjust player's position and velocity accordingly.
         * There's some tricky order of operations here that is explained inline*/
        public static void CollisionCheck(Room r, Player p, GameTime gameTime)
        {
            // Variables used for entire collision test method.
            Room unfoldedRoom = Physics.BlockUnfold(r, p.center.normal, p.center.direction);
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
            
            #region player-block collision

            

            for (int attempt = 0; attempt < 2; attempt++)
            {
                playerVertexList = new List<Vector3>();
                playerVertexList.Add(p.center.position + p.playerHalfHeight * up + p.playerHalfWidth * right);
                playerVertexList.Add(p.center.position + p.playerHalfHeight * up - p.playerHalfWidth * right);
                playerVertexList.Add(p.center.position - p.playerHalfHeight * up - p.playerHalfWidth * right);
                playerVertexList.Add(p.center.position - p.playerHalfHeight * up + p.playerHalfWidth * right);
                p.UpdateBoundingBox();
            
                List<Vector3> projectionList = new List<Vector3>();
                List<Vector3> relVelList = new List<Vector3>();
                List<EdgeProperties> edgePropertiesList = new List<EdgeProperties>();

                foreach (Block b in unfoldedRoom.blocks)
                {
                    if (p.boundingBoxBottom > b.boundingBoxTop ||
                        p.boundingBoxTop < b.boundingBoxBottom ||
                        p.boundingBoxLeft > b.boundingBoxRight ||
                        p.boundingBoxRight < b.boundingBoxLeft)
                        continue;

                    #region blockCollision
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
                        EdgeProperties properties = new EdgeProperties();
                        properties.type = VexedLib.EdgeType.Normal;
                        foreach (Edge e in b.edges)
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
                        edgePropertiesList.Add(properties);
                    }
                    #endregion
                }

                foreach (Doodad b in unfoldedRoom.doodads)
                {
                    if (p.boundingBoxBottom > b.boundingBoxTop ||
                        p.boundingBoxTop < b.boundingBoxBottom ||
                        p.boundingBoxLeft > b.boundingBoxRight ||
                        p.boundingBoxRight < b.boundingBoxLeft)
                        continue;
                    #region doodadCollision
                    if (b.hasCollisionRect)
                    {
                        // if block intesects with rectVertexList
                        List<Vector3> brickVertexList = new List<Vector3>();

                        brickVertexList.Add(b.position.position + b.up + b.right);
                        brickVertexList.Add(b.position.position + b.up + b.left);
                        brickVertexList.Add(b.position.position + b.down + b.left);
                        brickVertexList.Add(b.position.position + b.down + b.right);


                        // Actual collision detection between two polygons happens here.
                        Vector3 projection = Collide(playerVertexList, brickVertexList, p.center.normal);

                        if (projection.Length() > 0f)
                        {
                            // If a collision is found, save the necessary data and continue.
                            projectionList.Add(projection);
                            relVelList.Add(b.position.velocity);
                            EdgeProperties properties = new EdgeProperties();
                            properties.type = VexedLib.EdgeType.Normal;
                            edgePropertiesList.Add(properties);
                        }
                    }
                    #endregion
                }

                // Compute the most powerful collision and resolve it.
                Vector3 maxProjection = Vector3.Zero;
                Vector3 relVel = Vector3.Zero;
                EdgeProperties edgeProperties = null;
                for (int i = 0; i < projectionList.Count(); i++)
                {
                    if (projectionList[i].Length() > maxProjection.Length())
                    {
                        maxProjection = projectionList[i];
                        relVel = relVelList[i];
                        edgeProperties = edgePropertiesList[i];
                    }
                }
                if (maxProjection != Vector3.Zero)
                {
                    Vector3 projectionDirection = maxProjection / maxProjection.Length();
                    //Vector3 frictionDirection = Vector3.Cross(Vector3.Dot(projectionDirection, p.center.direction) * p.center.direction, p.center.normal);
                    Vector3 frictionDirection = Vector3.Cross(projectionDirection, p.center.normal);

                    if(edgeProperties.type == VexedLib.EdgeType.ConveyorBelt)
                    {
                        relVel += .001f*edgeProperties.primaryValue * frictionDirection;
                    }

                    float badVelocityComponent = Vector3.Dot(projectionDirection, p.center.velocity - relVel);
                                        
                    if (badVelocityComponent < -0.0f)
                    {
                        if (edgeProperties.type == VexedLib.EdgeType.Bounce)
                        {
                            p.center.velocity -= badVelocityComponent * projectionDirection;
                            p.center.velocity += p.jumpSpeed * projectionDirection;
                        }
                        else
                        {
                            p.center.velocity -= badVelocityComponent * projectionDirection;
                        }
                    }

                    float projectionUpComponent = Vector3.Dot(projectionDirection, p.center.direction);
                    if (projectionUpComponent > 0)
                    {
                        
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
                        if (edgeProperties.type == VexedLib.EdgeType.Ice || edgeProperties.type == VexedLib.EdgeType.Bounce)
                            frictionAdjustment = Vector3.Zero;
                    }

                    p.center.position += maxProjection;

                    if (edgeProperties.type == VexedLib.EdgeType.Spikes || (edgeProperties.type == VexedLib.EdgeType.Electric && edgeProperties.primaryValue > 0))
                    {
                        p.dead = true;
                    }
                }
                else
                    break;
            }

            // Now that player position is stabilized, use the special rects to detect if it is grounded
            // or prepped for a wall jump.

            playerGroundBox.Add(p.center.position + p.playerHalfWidth * right);
            playerGroundBox.Add(p.center.position - p.playerHalfWidth * right);
            playerGroundBox.Add(p.center.position - (p.playerHalfHeight + .1f) * up - p.playerHalfWidth * right);
            playerGroundBox.Add(p.center.position - (p.playerHalfHeight + .1f) * up + p.playerHalfWidth * right);
            playerLeftBox.Add(p.center.position);
            playerLeftBox.Add(p.center.position - (p.playerHalfWidth + .1f) * right);
            playerLeftBox.Add(p.center.position - (p.playerHalfHeight - .1f) * up - (p.playerHalfWidth + .1f) * right);
            playerLeftBox.Add(p.center.position - (p.playerHalfHeight - .1f) * up);
            playerRightBox.Add(p.center.position + (p.playerHalfWidth + .1f) * right);
            playerRightBox.Add(p.center.position);
            playerRightBox.Add(p.center.position - (p.playerHalfHeight - .1f) * up);
            playerRightBox.Add(p.center.position - (p.playerHalfHeight - .1f) * up + (p.playerHalfWidth + .1f) * right);

            p.leftWall = false;
            p.rightWall = false;

            p.platformVelocity = Vector3.Zero;
            foreach (Block b in unfoldedRoom.blocks)
            {
                if (p.boundingBoxBottom > b.boundingBoxTop + 1 ||
                        p.boundingBoxTop < b.boundingBoxBottom - 1 ||
                        p.boundingBoxLeft > b.boundingBoxRight +1||
                        p.boundingBoxRight < b.boundingBoxLeft - 1)
                    continue;
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
                    EdgeProperties properties = new EdgeProperties();
                    foreach (Edge e in b.edges)
                    {
                        Vector3 edgeNormal = Vector3.Cross(e.start.normal, e.start.position - e.end.position);
                        edgeNormal.Normalize();
                        Vector3 projectionNormal = groundProjection / groundProjection.Length();
                        float result = Vector3.Dot(edgeNormal, projectionNormal);
                        if (result == 1)
                        {
                            properties = e.properties;
                        }
                    }
                    p.platformVelocity = b.edges[0].start.velocity;
                    if (properties.type == VexedLib.EdgeType.ConveyorBelt)
                    {
                        Vector3 lateralDirection = Vector3.Cross(groundProjection, p.center.normal);
                        lateralDirection.Normalize();
                        p.platformVelocity += .001f * properties.primaryValue * lateralDirection;
                    }
                }
                if (Vector3.Dot(leftProjection, right) > 0)
                {
                    p.leftWall = true;
                    EdgeProperties properties = new EdgeProperties();                    
                    foreach (Edge e in b.edges)
                    {
                        Vector3 edgeNormal = Vector3.Cross(e.start.normal, e.start.position - e.end.position);
                        edgeNormal.Normalize();
                        Vector3 projectionNormal = leftProjection / leftProjection.Length();
                        float result = Vector3.Dot(edgeNormal, projectionNormal);
                        if (result == 1)
                        {
                            properties = e.properties;
                        }
                    }
                    p.platformVelocity = b.edges[0].start.velocity;
                    if (properties.type == VexedLib.EdgeType.Magnet)
                    {
                        p.state = State.Spin;
                        p.spinUp = leftProjection / leftProjection.Length();
                    }
                }
                if (Vector3.Dot(rightProjection, -right) != 0)
                {
                    p.rightWall = true;
                    EdgeProperties properties = new EdgeProperties();
                    foreach (Edge e in b.edges)
                    {
                        Vector3 edgeNormal = Vector3.Cross(e.start.normal, e.start.position - e.end.position);
                        edgeNormal.Normalize();
                        Vector3 projectionNormal = rightProjection / rightProjection.Length();
                        float result = Vector3.Dot(edgeNormal, projectionNormal);
                        if (result == 1)
                        {
                            properties = e.properties;
                        }
                    }
                    p.platformVelocity = b.edges[0].start.velocity;
                    if (properties.type == VexedLib.EdgeType.Magnet)
                    {
                        p.state = State.Spin;
                        p.spinUp = rightProjection / rightProjection.Length();
                    }
                }
            }
            
            foreach (Doodad b in unfoldedRoom.doodads)
            {
                if (b.hasCollisionRect)
                {
                    if (p.boundingBoxBottom > b.boundingBoxTop ||
                        p.boundingBoxTop < b.boundingBoxBottom ||
                        p.boundingBoxLeft > b.boundingBoxRight ||
                        p.boundingBoxRight < b.boundingBoxLeft)
                        continue;
                    // if block intesects with rectVertexList
                    List<Vector3> brickVertexList = new List<Vector3>();

                    brickVertexList.Add(b.position.position + b.up + b.right);
                    brickVertexList.Add(b.position.position + b.up + b.left);
                    brickVertexList.Add(b.position.position + b.down + b.left);
                    brickVertexList.Add(b.position.position + b.down + b.right);

                    Vector3 groundProjection = Collide(playerGroundBox, brickVertexList, p.center.normal);
                    Vector3 leftProjection = Collide(playerLeftBox, brickVertexList, p.center.normal);
                    Vector3 rightProjection = Collide(playerRightBox, brickVertexList, p.center.normal);


                    if (Vector3.Dot(groundProjection, up) > 0)
                    {
                        p.grounded = true;
                        p.platformVelocity = b.position.velocity;
                    }
                    if (Vector3.Dot(leftProjection, right) > 0)
                    {
                        p.leftWall = true;
                        p.platformVelocity = b.position.velocity;
                    }
                    if (Vector3.Dot(rightProjection, -right) != 0)
                    {
                        p.rightWall = true;
                        p.platformVelocity = b.position.velocity;
                    }
                }
            }

            // Now that we know if the player is grounded or not, we can use the walking property to determine whether or
            // not we should apply friction.
            if (!p.walking)
                p.center.velocity += frictionAdjustment;

            p.center.Update(r, 0);
            #endregion
            
            
            
            #region doodad-activation
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

            foreach (Doodad d in r.doodads)
            {
                if (d.type == VexedLib.DoodadType.Checkpoint)
                {
                    if (d == p.respawnPoint)
                        d.active = true;
                    else
                        d.active = false;
                }
                if ((d.position.position - p.center.position).Length() < d.triggerDistance)
                {
                    if (d.type == VexedLib.DoodadType.PowerOrb)
                    {
                        if(d.active == true)
                        {
                            d.active = false;
                            p.orbsCollected++;
                        }
                    }
                    if (d.type == VexedLib.DoodadType.Checkpoint)
                    {
                        p.respawnPoint = d;
                        p.respawnCenter = new Vertex(d.position.position, p.center.normal, Vector3.Zero, p.center.direction);

                    }
                    if(d.targetDoodad != null)
                    {
                        if (d.targetDoodad.currentBehavior.id == d.expectedBehavior)
                        {
                            foreach (Behavior b in d.targetDoodad.behaviors)
                            {
                                if (b.id == d.targetBehavior)
                                {
                                    d.targetDoodad.SetBehavior(b);
                                }
                            }
                        }                        
                    }
                }
            }
            #endregion
            
            #region doodadCollisionDetection
            List<Vector3> doodadVertexList;
            foreach(Doodad d in unfoldedRoom.doodads)
            {
                if (d.freeMotion == true)
                {
                    frictionAdjustment = Vector3.Zero;
                    for (int attempt = 0; attempt < 2; attempt++)
                    {                        
                        List<Vector3> projectionList = new List<Vector3>();
                        List<Vector3> relVelList = new List<Vector3>();
                        List<EdgeProperties> edgePropertiesList = new List<EdgeProperties>();

                        doodadVertexList = new List<Vector3>();
                        doodadVertexList.Add(d.position.position + d.up + d.right);
                        doodadVertexList.Add(d.position.position + d.up + d.left);
                        doodadVertexList.Add(d.position.position + d.down + d.left);
                        doodadVertexList.Add(d.position.position + d.down + d.right);
                        d.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                        foreach (Block b in unfoldedRoom.blocks)
                        {                            
                            if (d.boundingBoxBottom > b.boundingBoxTop ||
                                d.boundingBoxTop < b.boundingBoxBottom ||
                                d.boundingBoxLeft > b.boundingBoxRight ||
                                d.boundingBoxRight < b.boundingBoxLeft)
                                continue;
                            #region blockCollision
                            // if block intesects with rectVertexList
                            List<Vector3> blockVertexList = new List<Vector3>();
                            foreach (Edge e in b.edges)
                            {
                                if (e.start.position != e.end.position)
                                    blockVertexList.Add(e.start.position);
                            }

                            // Actual collision detection between two polygons happens here.
                            Vector3 projection = Collide(doodadVertexList, blockVertexList, p.center.normal);

                            if (projection.Length() > 0f)
                            {
                                // If a collision is found, save the necessary data and continue.
                                projectionList.Add(projection);
                                relVelList.Add(b.edges[0].start.velocity);
                                EdgeProperties properties = new EdgeProperties();
                                properties.type = VexedLib.EdgeType.Normal;
                                foreach (Edge e in b.edges)
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
                                edgePropertiesList.Add(properties);
                            }
                            #endregion
                        }

                        foreach (Doodad b in unfoldedRoom.doodads)
                        {
                            b.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                                
                            #region doodadCollision
                            if (b.hasCollisionRect && b != d)
                            {
                                if (d.boundingBoxBottom > b.boundingBoxTop ||
                                    d.boundingBoxTop < b.boundingBoxBottom ||
                                    d.boundingBoxLeft > b.boundingBoxRight ||
                                    d.boundingBoxRight < b.boundingBoxLeft)
                                        continue;
                                // if block intesects with rectVertexList
                                List<Vector3> brickVertexList = new List<Vector3>();

                                brickVertexList.Add(b.position.position + b.up + b.right);
                                brickVertexList.Add(b.position.position + b.up + b.left);
                                brickVertexList.Add(b.position.position + b.down + b.left);
                                brickVertexList.Add(b.position.position + b.down + b.right);


                                // Actual collision detection between two polygons happens here.
                                Vector3 projection = Collide(doodadVertexList, brickVertexList, p.center.normal);

                                if (projection.Length() > 0f)
                                {
                                    // If a collision is found, save the necessary data and continue.
                                    projectionList.Add(projection);
                                    relVelList.Add(b.position.velocity);
                                    EdgeProperties properties = new EdgeProperties();
                                    properties.type = VexedLib.EdgeType.Normal;
                                    edgePropertiesList.Add(properties);
                                }
                            }
                            #endregion
                        }

                        // Compute the most powerful collision and resolve it.
                        Vector3 maxProjection = Vector3.Zero;
                        Vector3 relVel = Vector3.Zero;
                        EdgeProperties edgeProperties = null;
                        for (int i = 0; i < projectionList.Count(); i++)
                        {
                            if (projectionList[i].Length() > maxProjection.Length())
                            {
                                maxProjection = projectionList[i];
                                relVel = relVelList[i];
                                edgeProperties = edgePropertiesList[i];
                            }
                        }
                        if (maxProjection != Vector3.Zero)
                        {

                            Vector3 projectionDirection = maxProjection / maxProjection.Length();
                            Vector3 frictionDirection = Vector3.Cross(projectionDirection, d.position.normal);                            
                            if (edgeProperties.type == VexedLib.EdgeType.ConveyorBelt)
                            {
                                relVel += .001f * edgeProperties.primaryValue * frictionDirection;
                            }

                            float badVelocityComponent = Vector3.Dot(projectionDirection, d.srcDoodad.position.velocity - relVel);
                            
                            if (badVelocityComponent < -0.0f)
                            {
                                if (edgeProperties.type == VexedLib.EdgeType.Bounce)
                                {
                                    d.srcDoodad.position.velocity -= badVelocityComponent * projectionDirection;
                                    d.srcDoodad.position.velocity += p.jumpSpeed * projectionDirection;
                                    d.position.velocity -= badVelocityComponent * projectionDirection;
                                    d.position.velocity += p.jumpSpeed * projectionDirection;
                                }
                                else
                                {
                                    d.srcDoodad.position.velocity -= badVelocityComponent * projectionDirection;
                                    d.position.velocity -= badVelocityComponent * projectionDirection;
                                }
                            }

                            float projectionUpComponent = Vector3.Dot(projectionDirection, p.center.direction);
                            if (projectionUpComponent > 0)
                            {

                                float frictionVelocityComponent = Vector3.Dot(frictionDirection, d.position.velocity - relVel);


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
                                if (edgeProperties.type == VexedLib.EdgeType.Ice || edgeProperties.type == VexedLib.EdgeType.Bounce)
                                    frictionAdjustment = Vector3.Zero;
                            }

                            d.srcDoodad.position.position += maxProjection;
                            d.position.position += maxProjection;
                            d.srcDoodad.position.velocity += frictionAdjustment;
                            d.position.velocity += frictionAdjustment;

                        }
                        else
                            break;
                    }
                }
            }
            #endregion
            
        }

    }
}

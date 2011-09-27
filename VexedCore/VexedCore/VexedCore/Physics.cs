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
    public class CollisionResult
    {
        public Vector3 projection;
        public Vector3 frictionAdjustment;
        public Vector3 velocityAdjustment;
        public Vector3 relVel;
        public EdgeProperties properties;
    }

    public class Physics
    {
        public static bool refresh = false;
        public static void BlockUnfold(Room r, Vector3 normal, Vector3 up)
        {
            if (Engine.player.center.normal != Engine.player.oldNormal || Engine.player.center.direction != Engine.player.oldUp)
            {
                refresh = true;
            }
            Vector3 right = Vector3.Cross(up, normal);
            foreach (Doodad d in r.doodads)
            {
                d.UpdateUnfoldedDoodad(r, normal, up);
                d.UpdateBoundingBox(Engine.player.up, Engine.player.right);
            }
            foreach (Monster m in r.monsters)
            {
                m.UpdateUnfoldedDoodad(r, normal, up);
                m.UpdateBoundingBox(Engine.player.up, Engine.player.right);
            }
            foreach (Projectile p in r.projectiles)
            {
                p.UpdateUnfoldedDoodad(r, normal, up);
                p.UpdateBoundingBox(Engine.player.up, Engine.player.right);
            }
            foreach (Block b in r.blocks)
            {
                if (b.staticObject == false || b.unfoldedBlocks.Count == 0 || refresh == true)
                {
                    b.UpdateUnfoldedBlocks(r, normal, up);
                    b.UpdateBoundingBox(Engine.player.up, Engine.player.right);
                }
                
            }
            refresh = false;
        }

        public static void DebugDraw(Room r, Vector3 normal, Vector3 up)
        {
            /*List<VertexPositionColorNormalTexture> triangleList = new List<VertexPositionColorNormalTexture>();
            List<Block> debugList = Physics.BlockUnfold(r, normal, up).blocks;
            foreach (Block b in debugList)
            {
                List<Vertex> vList = new List<Vertex>();
                vList.Add(b.edges[0].start);
                vList.Add(b.edges[1].start);
                vList.Add(b.edges[2].start);
                vList.Add(b.edges[3].start);
                r.AddBlockToTriangleList(vList, Color.Yellow, 0f, Room.plateTexCoords, triangleList);
                 
                //Draw projection Normals
                for (int i = 0; i < 4; i++)
                {
                    triangleList.Add(new VertexPositionColorNormal(b.edges[i].start.position, Color.Red, normal));
                    //triangleList.Add(new VertexPositionColorNormal(b.edges[i].end.position, Color.Red, normal));
                    //Vector3 projectionNormal = -Vector3.Cross(normal, b.edges[i].end.position - b.edges[i].start.position);
                    //projectionNormal.Normalize();
                    //triangleList.Add(new VertexPositionColorNormal(projectionNormal + .5f * (b.edges[i].end.position + b.edges[i].start.position), Color.Red, normal));
                                         
                    Vector3 velocityNormal = b.edges[i].start.velocity;                    
                    velocityNormal.Normalize();
                    Vector3 sideNormal = Vector3.Cross(velocityNormal, normal);
                    triangleList.Add(new VertexPositionColorNormalTexture(b.edges[i].start.position + .2f * sideNormal, Color.Red, normal, new Vector2(0,0)));
                    triangleList.Add(new VertexPositionColorNormalTexture(b.edges[i].start.position - .2f * sideNormal, Color.Red, normal, new Vector2(0, 0)));
                    triangleList.Add(new VertexPositionColorNormalTexture(b.edges[i].start.position + velocityNormal, Color.Red, normal, new Vector2(0, 0)));
                }
                
            }
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleList.ToArray(), 0, triangleList.Count / 3, VertexPositionColorNormal.VertexDeclaration);*/
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

        public static CollisionResult ResolveCollision(List<Vector3> projectionList, List<Vector3> relVelList, List<EdgeProperties> edgePropertiesList, Vector3 velocity, bool traction)
        {
            CollisionResult result = new CollisionResult();
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
            result.projection = maxProjection;
            result.properties = edgeProperties;
            result.relVel = relVel;
            if (maxProjection != Vector3.Zero)
            {
                Vector3 projectionDirection = maxProjection / maxProjection.Length();
                //Vector3 frictionDirection = Vector3.Cross(Vector3.Dot(projectionDirection, p.center.direction) * p.center.direction, p.center.normal);
                Vector3 frictionDirection = Vector3.Cross(projectionDirection, Engine.player.center.normal);

                if (edgeProperties.type == VexedLib.EdgeType.ConveyorBelt)
                {
                    relVel += .001f * edgeProperties.primaryValue * frictionDirection;
                }

                float badVelocityComponent = Vector3.Dot(projectionDirection, velocity - relVel);

                if (badVelocityComponent < -0.0f)
                {
                    if (edgeProperties.type == VexedLib.EdgeType.Bounce)
                    {
                        result.velocityAdjustment -= badVelocityComponent * projectionDirection;
                        result.velocityAdjustment += Engine.player.jumpSpeed * projectionDirection;
                    }
                    else
                    {
                        result.velocityAdjustment -= badVelocityComponent * projectionDirection;
                    }
                }

                float projectionUpComponent = Vector3.Dot(projectionDirection, Engine.player.center.direction);
                if (projectionUpComponent > 0)
                {                    
                    float frictionVelocityComponent = Vector3.Dot(frictionDirection, velocity - relVel);


                    if (Math.Abs(frictionVelocityComponent) < .02f)
                    {
                        result.frictionAdjustment -= frictionVelocityComponent * frictionDirection;
                    }
                    else if (frictionVelocityComponent > 0)
                    {
                        result.frictionAdjustment -= .02f * frictionDirection;
                    }
                    else
                    {
                        result.frictionAdjustment += .02f * frictionDirection;
                    }
                    if (edgeProperties.type == VexedLib.EdgeType.Ice && traction == false)
                        result.frictionAdjustment = Vector3.Zero;
                    if (edgeProperties.type == VexedLib.EdgeType.Bounce)
                        result.frictionAdjustment = Vector3.Zero;
                }
            }
            return result;    
        }

        public static bool PhaseTest()
        {
            bool phaseOK = true;
            float throughDistance = Math.Abs(Vector3.Dot(Engine.player.center.normal, Engine.player.currentRoom.size));
            Vector3 originalPosition = Engine.player.center.position;
            Engine.player.center.position = Engine.player.center.position - throughDistance * Engine.player.center.normal;
            Engine.player.center.normal = -Engine.player.center.normal;
            
            Engine.player.center.Update(Engine.player.currentRoom, 0);
            refresh = true;
            Physics.BlockUnfold(Engine.player.currentRoom, Engine.player.center.normal, Engine.player.center.direction);

            List<Vector3> playerVertexList = new List<Vector3>();
            Vector3 up = Engine.player.center.direction;
            Vector3 right = Vector3.Cross(up, Engine.player.center.normal);
            playerVertexList = Engine.player.GetCollisionRect();

            foreach (Block baseBlock in Engine.player.currentRoom.blocks)
            {
                foreach (Block b in baseBlock.unfoldedBlocks)
                {
                    if (Engine.player.CollisionFirstPass(b))
                        continue;

                    List<Vector3> blockVertexList = b.GetCollisionRect();
                    Vector3 projection = Collide(playerVertexList, blockVertexList, Engine.player.center.normal);
                    // If a collision is found, save the necessary data and continue.                        
                    if (projection.Length() > .3f)
                    {
                        phaseOK = false;
                    }
                }
            }
            Engine.player.center.normal = -Engine.player.center.normal;
            Engine.player.center.position = originalPosition;
            Engine.player.center.Update(Engine.player.currentRoom, 0);
            refresh = true;
            return phaseOK;
        }

        /* Test for collision between player and blocks, and adjust player's position and velocity accordingly.
         * There's some tricky order of operations here that is explained inline*/
        public static void CollisionCheck(Room r, Player p, GameTime gameTime)
        {
            // Variables used for entire collision test method.
            Physics.BlockUnfold(r, p.center.normal, p.center.direction);
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
                playerVertexList = p.GetCollisionRect();
                
                List<Vector3> projectionList = new List<Vector3>();
                List<Vector3> relVelList = new List<Vector3>();
                List<EdgeProperties> edgePropertiesList = new List<EdgeProperties>();

                foreach (Block baseBlock in r.blocks)
                {
                    foreach (Block b in baseBlock.unfoldedBlocks)
                    {
                        if (p.CollisionFirstPass(b))
                            continue;

                        List<Vector3> blockVertexList = b.GetCollisionRect();
                        Vector3 projection = Collide(playerVertexList, blockVertexList, p.center.normal);
                        // If a collision is found, save the necessary data and continue.                        
                        if (projection.Length() > 0f)
                        {
                            projectionList.Add(projection);
                            relVelList.Add(b.GetVelocity());
                            EdgeProperties eTemp = b.GetProperties(projection);
                            edgePropertiesList.Add(b.GetProperties(projection));
                        }
                    }
                }
                foreach (Doodad d in r.doodads)
                {
                    if (p.CollisionFirstPass(d))
                        continue;
                    if (d.type == VexedLib.DoodadType.BridgeGate)
                    {
                        List<Vector3> doodadVertexList = d.GetCollisionRect();
                        Vector3 projection = Collide(playerVertexList, doodadVertexList, p.center.normal);

                        d.ActivateDoodad(r, projection.Length() > 0f);                        
                    }
                    if (d.hasCollisionRect && d.type != VexedLib.DoodadType.PowerPlug)
                    {
                        List<Vector3> doodadVertexList = d.GetCollisionRect();
                        Vector3 projection = Collide(playerVertexList, doodadVertexList, p.center.normal);


                        if (projection.Length() > 0f)
                        {
                            projectionList.Add(projection);
                            relVelList.Add(d.position.velocity);
                            edgePropertiesList.Add(new EdgeProperties());
                        }
                    }                    
                }
                foreach (Monster m in r.monsters)
                {
                    if (p.CollisionFirstPass(m))
                        continue;
                    if (m.dead == true)
                        continue;
                    if (m.moveType == VexedLib.MovementType.SnakeBoss)
                    {
                        List<Vector3> monsterVertexList = m.GetCollisionRect();
                        Vector3 projection = Collide(playerVertexList, monsterVertexList, Engine.player.center.normal);
                        if (projection.Length() > 0f)
                        {
                            projectionList.Add(projection);
                            relVelList.Add(m.position.velocity);
                            edgePropertiesList.Add(new EdgeProperties());
                        }
                    }
                }

                // Compute the most powerful collision and resolve it.
                CollisionResult result = ResolveCollision(projectionList, relVelList, edgePropertiesList, p.center.velocity, p.HasTraction());

                if (result.properties != null && result.properties.type == VexedLib.EdgeType.Ice && Vector3.Dot(result.projection, p.center.direction) > 0)
                    p.sliding = true;
                else
                    p.sliding = false;
                p.center.velocity += result.velocityAdjustment;
                frictionAdjustment += result.frictionAdjustment;
                if(result.projection != Vector3.Zero)
                {
                    p.center.position += result.projection;

                    if (result.properties.type == VexedLib.EdgeType.Spikes || (result.properties.type == VexedLib.EdgeType.Electric && result.properties.primaryValue > 0)
                        || (result.properties.type == VexedLib.EdgeType.Fire && result.properties.primaryValue > 0))
                    {
                        p.Damage(result.projection);
                    }
                    if (result.projection.Length() > p.playerHalfWidth)
                    {
                        p.Damage(result.projection);
                    }
                }
                else
                    break;
            }


            List<Vector3> pVertexList = p.GetCollisionRect();
            foreach (Projectile s in r.projectiles)
            {
                if (s.active == true && s.exploding == false)
                {
                    if (s.srcMonster == null && (s.type == ProjectileType.Missile || s.type == ProjectileType.Bomb))
                        continue;
                    s.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                    if (p.CollisionFirstPass(s) == false)
                    {
                        List<Vector3> projectileVertexList = s.GetCollisionRect();
                        Vector3 projection = Collide(pVertexList, projectileVertexList, Engine.player.center.normal);
                        if (projection != Vector3.Zero)
                        {
                            p.Damage(projection);
                            s.exploding = true;
                            s.position.velocity = Vector3.Zero;
                        }
                    }
                }
            }
            foreach (Monster m in r.monsters)
            {
                if (m.dead == true)
                    continue;
                m.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                if (p.CollisionFirstPass(m) == false)
                {
                    if (m.moveType != VexedLib.MovementType.SnakeBoss)
                    {
                        Vector3 distance = (p.center.position - m.unfoldedPosition.position);
                        if (distance.Length() < (p.playerHalfHeight + m.halfHeight))
                        {
                            Vector3 projection = Vector3.Normalize(distance) * ((p.playerHalfHeight + m.halfHeight) - distance.Length());
                            p.Damage(projection);
                        }
                    }
                }
            }
            foreach (Doodad d in r.doodads)
            {
                if (d.type == VexedLib.DoodadType.Beam)
                {
                    d.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                    if (p.CollisionFirstPass(d) == false)
                    {
                        List<Vector3> doodadVertexList = d.GetCollisionRect();
                        Vector3 projection = Collide(pVertexList, doodadVertexList, Engine.player.center.normal);
                        if (projection != Vector3.Zero)
                        {
                            p.Damage(projection);
                        }
                    }
                }                
            }

            // Now that player position is stabilized, use the special rects to detect if it is grounded
            // or prepped for a wall jump.

            #region special-blocks for ground and walljump detection
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
            foreach (Block baseBlock in r.blocks)
            {
                foreach (Block b in baseBlock.unfoldedBlocks)
                {
                    if (p.boundingBoxBottom > b.boundingBoxTop + 1 ||
                            p.boundingBoxTop < b.boundingBoxBottom - 1 ||
                            p.boundingBoxLeft > b.boundingBoxRight + 1 ||
                            p.boundingBoxRight < b.boundingBoxLeft - 1)
                        continue;
                    // if block intesects with rectVertexList
                    List<Vector3> blockVertexList = b.GetCollisionRect();

                    Vector3 groundProjection = Collide(playerGroundBox, blockVertexList, p.center.normal);
                    Vector3 leftProjection = Collide(playerLeftBox, blockVertexList, p.center.normal);
                    Vector3 rightProjection = Collide(playerRightBox, blockVertexList, p.center.normal);


                    //if (Vector3.Dot(groundProjection, up) > 0)
                    //if (groundProjection != Vector3.Zero)
                    if (groundProjection.Length() > .001f)
                    {
                        p.grounded = true;
                        EdgeProperties properties = b.GetProperties(groundProjection);
                        p.platformVelocity = b.GetVelocity();
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
                        EdgeProperties properties = b.GetProperties(leftProjection);
                        p.platformVelocity = b.GetVelocity();
                        if (properties.type == VexedLib.EdgeType.Magnet)
                        {
                            p.Spin(leftProjection / leftProjection.Length());
                        }
                    }
                    if (Vector3.Dot(rightProjection, -right) != 0)
                    {
                        p.rightWall = true;
                        EdgeProperties properties = b.GetProperties(rightProjection);
                        p.platformVelocity = b.GetVelocity();
                        if (properties.type == VexedLib.EdgeType.Magnet)
                        {
                            p.Spin(rightProjection / rightProjection.Length());
                        }
                    }
                }
            }
            
            foreach (Doodad b in r.doodads)
            {
                if (b.hasCollisionRect && b.type != VexedLib.DoodadType.PowerPlug)
                {
                    b.UpdateBoundingBox(p.center.direction, p.right);
                    if (p.boundingBoxBottom > b.boundingBoxTop +2f ||
                        p.boundingBoxTop < b.boundingBoxBottom-2f ||
                        p.boundingBoxLeft > b.boundingBoxRight+2f ||
                        p.boundingBoxRight < b.boundingBoxLeft-2f)
                        continue;
                    // if block intesects with rectVertexList
                    List<Vector3> brickVertexList = b.GetCollisionRect();

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

            foreach (Monster m in r.monsters)
            {
                if (p.CollisionFirstPass(m))
                    continue;
                if (m.dead == true)
                    continue;
                if (m.moveType == VexedLib.MovementType.SnakeBoss)
                {
                    m.UpdateBoundingBox(p.center.direction, p.right);
                    if (p.boundingBoxBottom > m.boundingBoxTop + 2f ||
                        p.boundingBoxTop < m.boundingBoxBottom - 2f ||
                        p.boundingBoxLeft > m.boundingBoxRight + 2f ||
                        p.boundingBoxRight < m.boundingBoxLeft - 2f)
                        continue;
                    // if block intesects with rectVertexList
                    List<Vector3> brickVertexList = m.GetCollisionRect();

                    Vector3 groundProjection = Collide(playerGroundBox, brickVertexList, p.center.normal);
                    Vector3 leftProjection = Collide(playerLeftBox, brickVertexList, p.center.normal);
                    Vector3 rightProjection = Collide(playerRightBox, brickVertexList, p.center.normal);

                    if (Vector3.Dot(groundProjection, up) > 0)
                    {
                        p.grounded = true;
                        p.platformVelocity = m.position.velocity;
                    }
                    if (Vector3.Dot(leftProjection, right) > 0)
                    {
                        p.leftWall = true;
                        p.platformVelocity = m.position.velocity;
                    }
                    if (Vector3.Dot(rightProjection, -right) != 0)
                    {
                        p.rightWall = true;
                        p.platformVelocity = m.position.velocity;
                    }
                }
            }
            #endregion
            
            // Now that we know if the player is grounded or not, we can use the walking property to determine whether or
            // not we should apply friction.
            if (!p.walking)
                p.center.velocity += frictionAdjustment;
            
            #endregion                        
            
            #region doodad-activation
            // Test and activate doodads.

            foreach (Doodad d in r.doodads)
            {
                if (d.type == VexedLib.DoodadType.TriggerPoint)
                {
                    if (d.id.Contains("Rock2Trigger") && d.ActivationRange(p))
                        RockBoss.triggered = true;
                }
                if (d.type == VexedLib.DoodadType.Checkpoint)
                {
                    d.ActivateDoodad(r, d == p.respawnPoint);
                }
                if (d.type == VexedLib.DoodadType.Vortex || d.type == VexedLib.DoodadType.WarpStation || d.type == VexedLib.DoodadType.JumpPad || d.type == VexedLib.DoodadType.ItemBlock || d.type == VexedLib.DoodadType.JumpStation || d.type == VexedLib.DoodadType.PowerStation || d.type == VexedLib.DoodadType.SwitchStation || d.type == VexedLib.DoodadType.ItemStation || d.type == VexedLib.DoodadType.UpgradeStation || d.type == VexedLib.DoodadType.SaveStation || d.type == VexedLib.DoodadType.LoadStation || d.type == VexedLib.DoodadType.MenuStation)
                {
                    d.ActivateDoodad(r, d.ActivationRange(p));
                }
                if (d.type == VexedLib.DoodadType.NPC_OldMan)
                {
                    d.ActivateDoodad(r, d.ActivationRange(p));
                }
                if( (d.position.position - p.center.position).Length() < 3f*d.triggerDistance)
                {
                    if (d.type == VexedLib.DoodadType.PowerOrb)
                    {
                        d.tracking = true;
                    }
                }
                if ((d.position.position - p.center.position).Length() < d.triggerDistance)
                {
                    if (d.type == VexedLib.DoodadType.PowerOrb)
                    {
                        if(d.active == true)
                        {
                            d.ActivateDoodad(r, false);
                            r.currentOrbs++;
                            //Engine.reDraw = true;
                            r.refreshVertices = true;
                            p.orbsCollected++;
                            SoundFX.CollectOrb();
                        }
                    }
                    if (d.type == VexedLib.DoodadType.Checkpoint)
                    {
                        if (p.respawnPoint != d)
                        {
                            LevelLoader.QuickSave();
                            p.respawnPoint = d;
                            p.respawnPlayer = new Player();
                            p.respawnPlayer.currentRoom = p.currentRoom;
                            p.respawnPlayer.center = new Vertex(d.position.position, p.center.normal, Vector3.Zero, p.center.direction);
                        }

                    }
                    if(d.type == VexedLib.DoodadType.WallSwitch)
                    {
                        d.Activate();                        
                    }
                }
            }
            #endregion
            
            #region doodadCollisionDetection
            
            foreach(Doodad d in r.doodads)
            {
                if (d.freeMotion == true)
                {
                    frictionAdjustment = Vector3.Zero;
                    for (int attempt = 0; attempt < 2; attempt++)
                    {                        
                        List<Vector3> projectionList = new List<Vector3>();
                        List<Vector3> relVelList = new List<Vector3>();
                        List<EdgeProperties> edgePropertiesList = new List<EdgeProperties>();

                        List<Vector3> doodadVertexList = d.GetCollisionRect();
                        d.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                        foreach (Block baseBlock in r.blocks)
                        {
                            foreach (Block b in baseBlock.unfoldedBlocks)
                            {
                                if (d.CollisionFirstPass(b))
                                    continue;
                                List<Vector3> blockVertexList = b.GetCollisionRect();
                                Vector3 projection = Collide(doodadVertexList, blockVertexList, p.center.normal);

                                if (projection.Length() > 0f)
                                {
                                    projectionList.Add(projection);
                                    relVelList.Add(b.GetVelocity());
                                    edgePropertiesList.Add(b.GetProperties(projection));
                                }
                            }
                        }

                        foreach (Doodad b in r.doodads)
                        {
                            b.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                                
                            if (b.hasCollisionRect && b != d)
                            {
                                if (d.CollisionFirstPass(b))
                                        continue;
                                List<Vector3> brickVertexList = b.GetCollisionRect();
                                Vector3 projection = Collide(doodadVertexList, brickVertexList, p.center.normal);

                                if (projection.Length() > 0f)
                                {
                                    projectionList.Add(projection);
                                    relVelList.Add(b.position.velocity);
                                    edgePropertiesList.Add(new EdgeProperties());
                                }
                            }                            
                        }

                        CollisionResult result = ResolveCollision(projectionList, relVelList, edgePropertiesList, d.unfoldedPosition.velocity, false);
                        if(result.projection != Vector3.Zero)
                        {
                            d.AdjustVertex(result.projection, result.velocityAdjustment + result.frictionAdjustment, p.center.normal, p.center.direction);
                            d.position.Update(p.currentRoom, 0);
                            d.unfoldedPosition.position += result.projection;
                            d.unfoldedPosition.velocity += result.velocityAdjustment;
                            d.unfoldedPosition.velocity += result.frictionAdjustment;
                        }
                        else
                            break;
                    }
                        
                }
            }
            foreach (Doodad d in r.doodads)
            {
                if (d.freeMotion)
                {
                    foreach (Doodad b in r.doodads)
                    {
                        if ((d.position.position - b.position.position).Length() < b.triggerDistance)
                        {
                            if (b.type == VexedLib.DoodadType.WallSwitch)
                            {
                                b.Activate();
                            }
                            if (b.type == VexedLib.DoodadType.PlugSlot && d.type == VexedLib.DoodadType.PowerPlug)
                            {
                                d.active = true;
                                d.position.position = b.position.position;
                                d.SetTargetBehavior();
                            }
                        }
                    }
                }
            }

            #endregion

            #region monsters

            bool smashRockBarrier = false;

            foreach (Monster m in r.monsters)
            {
                if (m.moveType == VexedLib.MovementType.FaceBoss)
                    continue;
                if (m.dead == true)
                    continue;
                frictionAdjustment = Vector3.Zero;
                for (int attempt = 0; attempt < 2; attempt++)
                {

                    List<Vector3> projectionList = new List<Vector3>();
                    List<Vector3> relVelList = new List<Vector3>();
                    List<EdgeProperties> edgePropertiesList = new List<EdgeProperties>();

                    List<Vector3> monsterVertexList = m.GetCollisionRect();
                    m.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                    foreach (Block baseBlock in r.blocks)
                    {
                        foreach (Block b in baseBlock.unfoldedBlocks)
                        {
                            if (m.CollisionFirstPass(b))
                                continue;
                            List<Vector3> blockVertexList = b.GetCollisionRect();
                            Vector3 projection = Collide(monsterVertexList, blockVertexList, p.center.normal);

                            if (projection.Length() > 0f)
                            {
                                projectionList.Add(projection);
                                relVelList.Add(b.GetVelocity());
                                edgePropertiesList.Add(b.GetProperties(projection));
                            }
                        }
                    }

                    
                    foreach (Doodad b in r.doodads)
                    {
                        b.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));

                        if (b.hasCollisionRect && b.type != VexedLib.DoodadType.PowerPlug)
                        {
                            if (m.CollisionFirstPass(b))
                                continue;

                            if (m.moveType == VexedLib.MovementType.RockBoss && b.id.Contains("RockBarrier"))
                            {
                                smashRockBarrier = true;
                            }
                            if (m.moveType == VexedLib.MovementType.ChaseBoss && b.id.Contains("ChaseBarrier"))
                            {
                                smashRockBarrier = true;
                            }
                            if (m.moveType == VexedLib.MovementType.ChaseBoss && b.id.Contains("ChaseStop"))
                            {
                                ChaseBoss.studder = true;
                            }
                            if (m.moveType == VexedLib.MovementType.ChaseBoss && b.type == VexedLib.DoodadType.Brick)
                            {
                                b.ActivateDoodad(r, true);
                                continue;
                            }

                            List<Vector3> brickVertexList = b.GetCollisionRect();
                            Vector3 projection = Collide(monsterVertexList, brickVertexList, p.center.normal);

                            if (projection.Length() > 0f)
                            {
                                if (m.moveType == VexedLib.MovementType.RockBoss && b.type == VexedLib.DoodadType.Crate)
                                {
                                    if (m.rockBoss.rockHits > 0)
                                    {
                                        m.impactVector = projection;
                                        m.lastHitType = ProjectileType.Impact;
                                    }
                                    continue;
                                }
                                projectionList.Add(projection);
                                relVelList.Add(b.position.velocity);
                                edgePropertiesList.Add(new EdgeProperties());
                            }
                        }
                    }
                    
                    CollisionResult result = ResolveCollision(projectionList, relVelList, edgePropertiesList, m.unfoldedPosition.velocity, false);
                    if (result.projection != Vector3.Zero)
                    {
                        m.AdjustVertex(result.projection, result.velocityAdjustment, p.center.normal, p.center.direction);
                        m.position.Update(p.currentRoom, 0);
                        m.unfoldedPosition.position += result.projection;
                        m.unfoldedPosition.velocity += result.velocityAdjustment;
                        if (result.properties.type == VexedLib.EdgeType.Spikes)
                        {
                            m.impactVector = Monster.AdjustVector(result.projection, m.position.normal, Engine.player.center.normal, Engine.player.center.direction, true);
                            m.lastHitType = ProjectileType.Spikes;
                        }
                        if (result.projection.Length() > m.halfWidth)
                        {
                            m.impactVector = Monster.AdjustVector(result.projection, m.position.normal, Engine.player.center.normal, Engine.player.center.direction, true);
                            m.lastHitType = ProjectileType.Spikes;
                        }

                        if (Vector3.Dot(m.position.direction, Monster.AdjustVector(result.projection, m.position.normal, p.center.normal, p.center.direction, true)) <= 0)
                        {
                            m.AdjustVertex(Vector3.Zero, result.frictionAdjustment, p.center.normal, p.center.direction);                        
                            m.unfoldedPosition.velocity += result.frictionAdjustment;
                        }
                    }
                    else
                        break;
                }


                List<Vector3> mVertexList = m.GetCollisionRect();

                m.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));

                foreach (Monster m2 in r.monsters)
                {
                    if (m == m2 || m2.dead == true)
                        continue;
                    if (m.moveType == VexedLib.MovementType.SnakeBoss && m2.moveType == VexedLib.MovementType.SnakeBoss)
                        continue;
                    m2.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));

                    if (m.CollisionFirstPass(m2))
                        continue;
                    List<Vector3> brickVertexList = m2.GetCollisionRect();
                    Vector3 projection = Collide(mVertexList, brickVertexList, p.center.normal);

                    if (projection.Length() > 0f)
                    {
                        m.AdjustVertex(.3f*projection, Vector3.Zero, p.center.normal, p.center.direction);
                        m.position.Update(p.currentRoom, 0);
                        m.unfoldedPosition.position += .3f * projection;
                        m2.AdjustVertex(-.3f * projection, Vector3.Zero, p.center.normal, p.center.direction);
                        m2.position.Update(p.currentRoom, 0);
                        m2.unfoldedPosition.position -= .3f * projection;                        
                    }
                }

                foreach (Projectile s in r.projectiles)
                {
                    if (s.type == ProjectileType.Missile && s.srcMonster == null && (s.position.position - m.position.position).Length() < 7f)
                    {
                        s.SetTarget(m.position.position);
                    }
                    //if (s.active == true || m != s.srcMonster)
                    if (s.srcMonster != null && s.srcMonster.moveType == VexedLib.MovementType.SnakeBoss && m.moveType == VexedLib.MovementType.SnakeBoss)
                        continue;
                    if (m != s.srcMonster)
                    {
                        s.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                        if (m.CollisionFirstPass(s) == false)
                        {
                            List<Vector3> projectileVertexList = s.GetCollisionRect();
                            Vector3 projection = Collide(mVertexList, projectileVertexList, Engine.player.center.normal);
                            if (projection != Vector3.Zero)
                            {
                                s.exploding = true;
                                m.impactVector = Monster.AdjustVector(s.position.velocity, m.position.normal, Engine.player.center.normal, Engine.player.center.direction, true);
                                m.lastHitType = s.type;
                                s.position.velocity = Vector3.Zero;                                
                            }
                        }

                    }
                }

                List<Vector3> monsterGroundBox = m.GetGroundCollisionRect();
                List<Vector3> monsterForwardGroundBox = m.GetForwardGroundCollisionRect();
                List<Vector3> monsterForwardBox = m.GetForwardCollisionRect();

                m.groundProjection = Vector3.Zero;
                m.forwardGroundProjection = Vector3.Zero;
                m.forwardProjection = Vector3.Zero;
                foreach (Block baseBlock in r.blocks)
                {
                    foreach (Block b in baseBlock.unfoldedBlocks)
                    {
                        if (m.boundingBoxBottom > b.boundingBoxTop + 2 ||
                                m.boundingBoxTop < b.boundingBoxBottom - 2 ||
                                m.boundingBoxLeft > b.boundingBoxRight + 2 ||
                                m.boundingBoxRight < b.boundingBoxLeft - 2)
                            continue;
                        // if block intesects with rectVertexList
                        List<Vector3> blockVertexList = b.GetCollisionRect();

                        Vector3 groundProjection = Collide(monsterGroundBox, blockVertexList, p.center.normal);
                        Vector3 forwardGroundProjection = Collide(monsterForwardGroundBox, blockVertexList, p.center.normal);
                        Vector3 forwardProjection = Collide(monsterForwardBox, blockVertexList, p.center.normal);
                 
                        if (forwardGroundProjection != Vector3.Zero)
                            m.forwardGroundProjection = Monster.AdjustVector(forwardGroundProjection, m.position.normal, p.center.normal, p.center.direction, true);
                        if (forwardProjection != Vector3.Zero)
                            m.forwardProjection = Monster.AdjustVector(forwardProjection, m.position.normal, p.center.normal, p.center.direction, true);                        
                        if (groundProjection != Vector3.Zero)
                            m.groundProjection = Monster.AdjustVector(groundProjection, m.position.normal, p.center.normal, p.center.direction, true);
                    }
                }
                foreach (Monster m2 in r.monsters)
                {
                    if (m == m2 || m2.dead == true)
                        continue;
                    if (m.boundingBoxBottom > m2.boundingBoxTop + 2 ||
                            m.boundingBoxTop < m2.boundingBoxBottom - 2 ||
                            m.boundingBoxLeft > m2.boundingBoxRight + 2 ||
                            m.boundingBoxRight < m2.boundingBoxLeft - 2)
                        continue;
                    // if block intesects with rectVertexList
                    List<Vector3> blockVertexList = m2.GetCollisionRect();

                    Vector3 forwardProjection = Collide(monsterForwardBox, blockVertexList, p.center.normal);

                    if (forwardProjection != Vector3.Zero)
                    {
                        m.forwardProjection = Monster.AdjustVector(forwardProjection, m.position.normal, p.center.normal, p.center.direction, true);
                    }
                    
                }
            }

            if (smashRockBarrier == true)
            {
                foreach (Doodad d in r.doodads)
                {
                    if (d.id.Contains("RockBarrier") || d.id.Contains("ChaseBarrier"))
                    {
                        d.ActivateDoodad(r, true);
                    }
                }
            }


            #endregion




            #region projectile-collisions
            foreach (Projectile s in r.projectiles)
            {
                frictionAdjustment = Vector3.Zero;

                List<Vector3> projectionList = new List<Vector3>();
                List<Vector3> relVelList = new List<Vector3>();
                List<EdgeProperties> edgePropertiesList = new List<EdgeProperties>();

                List<Vector3> doodadVertexList = s.GetCollisionRect();
                s.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));
                foreach (Block baseBlock in r.blocks)
                {
                    foreach (Block b in baseBlock.unfoldedBlocks)
                    {
                        if (s.CollisionFirstPass(b))
                            continue;
                        List<Vector3> blockVertexList = b.GetCollisionRect();
                        Vector3 projection = Collide(doodadVertexList, blockVertexList, p.center.normal);
                        if (projection != Vector3.Zero)
                        {
                            //if (s.type == ProjectileType.Bomb)
                            //s.stopped = true;
                            //else
                            s.exploding = true;
                            s.position.velocity = Vector3.Zero;
                        }
                    }
                }

                foreach (Doodad b in r.doodads)
                {
                    b.UpdateBoundingBox(p.center.direction, Vector3.Cross(p.center.direction, p.center.normal));

                    if (b.hasCollisionRect || (b.type == VexedLib.DoodadType.LaserSwitch && s.type == ProjectileType.Laser && b.active == false))
                    {
                        if (s.CollisionFirstPass(b))
                            continue;
                        List<Vector3> brickVertexList = b.GetCollisionRect();
                        Vector3 projection = Collide(doodadVertexList, brickVertexList, p.center.normal);
                        if (projection != Vector3.Zero)
                        {
                            //if (s.type == ProjectileType.Bomb)A
                                //s.stopped = true;
                            //else
                                s.exploding = true;
                            s.position.velocity = Vector3.Zero;

                            if (b.type == VexedLib.DoodadType.LaserSwitch && s.type == ProjectileType.Laser)
                            {
                                b.ActivateDoodad(r, true);
                                b.SetTargetBehavior();
                            }
                            if ((s.type == ProjectileType.Bomb || s.type == ProjectileType.Missile) && s.exploding == true && b.type == VexedLib.DoodadType.Brick)
                            {
                                b.ActivateDoodad(r, true);
                            }
                        }
                    }
                }

            }
            #endregion

            p.center.Update(r, 0);
        }

    }
}

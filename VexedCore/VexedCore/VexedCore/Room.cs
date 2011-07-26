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
    public class Room
    {
        public static int innerBlockMode = 2;

        public Vector3 center;
        public Vector3 size;

        public static Texture2D blockTexture;

        public static List<Vector2> magnetSideTexCoords;
        public static List<Vector2> magnetSideEndTexCoords;
        public static List<Vector2> magnetTopTexCoords;
        public static List<Vector2> magnetTopEndTexCoords;

        public static List<Vector2> iceSideTexCoords;
        public static List<Vector2> iceSideEndTexCoords;
        public static List<Vector2> iceTopTexCoords;
        public static List<Vector2> iceTopEndTexCoords;

        public static List<Vector2> electricOnSideTexCoords;
        public static List<Vector2> electricOnSideEndTexCoords;
        public static List<Vector2> electricOnTopTexCoords;
        public static List<Vector2> electricOnTopEndTexCoords;

        public static List<Vector2> electricOffSideTexCoords;
        public static List<Vector2> electricOffSideEndTexCoords;
        public static List<Vector2> electricOffTopTexCoords;
        public static List<Vector2> electricOffTopEndTexCoords;

        public static List<Vector2> rubberSideTexCoords;
        public static List<Vector2> rubberSideEndTexCoords;
        public static List<Vector2> rubberTopTexCoords;
        public static List<Vector2> rubberTopEndTexCoords;

        public static List<Vector2> beltSideTexCoords;
        public static List<Vector2> beltSideEndTexCoords;
        public static List<Vector2> beltTopTexCoords;
        public static List<Vector2> beltTopEndTexCoords;

        public static List<Vector2> plateTexCoords;
        public static List<Vector2> blankTexCoords;
        public static float plateTexWidth = .25f;

        public static int texGridCount = 8;

        public static List<Vector2> LoadTexCoords(int x, int y)
        {
            float epsilon = .003f;
            float texWidth = 1f / texGridCount;
            List<Vector2> texCoords = new List<Vector2>();
            texCoords.Add(new Vector2((x + 1) * texWidth - epsilon, y * texWidth + epsilon));
            texCoords.Add(new Vector2(x * texWidth + epsilon, y * texWidth + epsilon));
            texCoords.Add(new Vector2(x * texWidth + epsilon, (y + 1) * texWidth - epsilon));
            texCoords.Add(new Vector2((x + 1) * texWidth - epsilon, (y + 1) * texWidth - epsilon));

            return texCoords;
        }

        public static void InitTexCoords()
        {
            plateTexCoords = new List<Vector2>();
            plateTexCoords.Add(new Vector2(plateTexWidth, 0));
            plateTexCoords.Add(new Vector2(0, 0));
            plateTexCoords.Add(new Vector2(0, plateTexWidth));
            plateTexCoords.Add(new Vector2(plateTexWidth, plateTexWidth));
            blankTexCoords = new List<Vector2>();
            blankTexCoords.Add(new Vector2(3*plateTexWidth/5, 2*plateTexWidth/5));
            blankTexCoords.Add(new Vector2(2 * plateTexWidth / 5, 2 * plateTexWidth / 5));
            blankTexCoords.Add(new Vector2(2 * plateTexWidth / 5, 3 * plateTexWidth / 5));
            blankTexCoords.Add(new Vector2(3 * plateTexWidth / 5, 3 * plateTexWidth / 5));

            magnetSideTexCoords = LoadTexCoords(3, 1);
            magnetTopTexCoords = LoadTexCoords(3, 0);
            magnetSideEndTexCoords = LoadTexCoords(2, 1);
            magnetTopEndTexCoords = LoadTexCoords(2, 0);

            iceSideTexCoords = LoadTexCoords(3, 3);
            iceTopTexCoords = LoadTexCoords(3, 2);
            iceSideEndTexCoords = LoadTexCoords(2, 3);
            iceTopEndTexCoords = LoadTexCoords(2, 2);

            electricOnSideTexCoords = LoadTexCoords(7, 1);
            electricOnTopTexCoords = LoadTexCoords(7, 0);
            electricOnSideEndTexCoords = LoadTexCoords(6, 1);
            electricOnTopEndTexCoords = LoadTexCoords(6, 0);

            electricOffSideTexCoords = LoadTexCoords(5, 1);
            electricOffTopTexCoords = LoadTexCoords(5, 0);
            electricOffSideEndTexCoords = LoadTexCoords(4, 1);
            electricOffTopEndTexCoords = LoadTexCoords(4, 0);

            rubberSideTexCoords = LoadTexCoords(5, 3);
            rubberTopTexCoords = LoadTexCoords(5, 2);
            rubberSideEndTexCoords = LoadTexCoords(4, 3);
            rubberTopEndTexCoords = LoadTexCoords(4, 2);

            beltSideTexCoords = LoadTexCoords(7, 3);
            beltTopTexCoords = LoadTexCoords(7, 2);
            beltSideEndTexCoords = LoadTexCoords(6, 3);
            beltTopEndTexCoords = LoadTexCoords(6, 2);
        }

        public List<Block> blocks;
        public List<JumpPad> jumpPads;
        public List<Bridge> bridges;
        public List<Doodad> doodads;
        public List<Monster> monsters;
        public List<Projectile> projectiles;


        public Room()
        {
            blocks = new List<Block>();
            jumpPads = new List<JumpPad>();
            bridges = new List<Bridge>();
            doodads = new List<Doodad>();
            monsters = new List<Monster>();
            projectiles = new List<Projectile>();
        }

        public Room(VexedLib.Room xmlRoom)
        {
            center = new Vector3(xmlRoom.centerX, xmlRoom.centerY, xmlRoom.centerZ);
            size = new Vector3(xmlRoom.sizeX, xmlRoom.sizeY, xmlRoom.sizeZ);
            blocks = new List<Block>();
            jumpPads = new List<JumpPad>();
            bridges = new List<Bridge>();
            doodads = new List<Doodad>();
            monsters = new List<Monster>();
            projectiles = new List<Projectile>();
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (Block b in blocks)
            {
                int blockUpdateTime = b.UpdateBehavior(gameTime);
                foreach (Edge e in b.edges)
                {
                    e.start.Update(this, blockUpdateTime);
                    e.end.Update(this, blockUpdateTime);
                    e.UpdateBehavior(gameTime);
                }                
            }

            if (this == Game1.player.currentRoom)
            {
                foreach (Doodad d in doodads)
                {
                    if (d.freeMotion)
                    {
                        d.position.Update(this, gameTime.ElapsedGameTime.Milliseconds);
                        Vector3 gravityDirection = Game1.player.center.direction;
                        if (d.position.normal == Game1.player.center.direction)
                        {
                            gravityDirection = -Game1.player.center.normal;
                        }
                        else if (d.position.normal == -Game1.player.center.direction)
                        {
                            gravityDirection = Game1.player.center.normal;
                        }

                        d.position.velocity -= Game1.player.gravityAcceleration * gravityDirection;

                        Vector3 up = d.position.direction;
                        Vector3 right = Vector3.Cross(d.position.direction, d.position.normal);

                        float upMagnitude = Vector3.Dot(up, d.position.velocity);
                        float rightMagnitude = Vector3.Dot(right, d.position.velocity);
                        float maxSpeed = Game1.player.maxVertSpeed / 2;
                        if (upMagnitude > maxSpeed)
                        {
                            d.position.velocity -= (upMagnitude - maxSpeed) * up;
                        }
                        if (upMagnitude < -maxSpeed)
                        {
                            d.position.velocity -= (maxSpeed + upMagnitude) * up;
                        }
                        if (rightMagnitude > maxSpeed)
                        {
                            d.position.velocity -= (rightMagnitude - maxSpeed) * right;
                        }
                        if (rightMagnitude < -maxSpeed)
                        {
                            d.position.velocity -= (maxSpeed + rightMagnitude) * right;
                        }

                    }
                    else
                    {
                        d.position.velocity = Vector3.Zero;
                    }
                    d.UpdateBehavior(gameTime);
                }
            }
        }

        public Vector3 AdjustedUp(Vector3 position, Vector3 up, Vector3 normal, float roundingThreshold)
        {
            Vector3 right = Vector3.Cross(up, normal);
            Vector3 relPosition = position - center;
            Vector3 returnUp = up;
            float upValue = Vector3.Dot(relPosition, up);
            float maxUpValue = Math.Abs(Vector3.Dot(size / 2, up));
            float rightValue = Vector3.Dot(relPosition, right);
            float maxRightValue = Math.Abs(Vector3.Dot(size / 2, right));
            if (upValue > maxUpValue - roundingThreshold)
            {
                float percentage = (roundingThreshold + maxUpValue - upValue) / (2*roundingThreshold);
                returnUp = percentage * up - (1-percentage) * normal;
            }
            if (upValue < roundingThreshold - maxUpValue)
            {
                float percentage = (roundingThreshold + upValue + maxUpValue) / (2*roundingThreshold);
                returnUp = percentage * up + (1-percentage) * normal;
            }
            returnUp.Normalize();
            return returnUp;
        }

        public Vector3 RaisedPosition(Vector3 position, float distanceModifier, float roundingThreshold)
        {
            Vector3 modifier = Vector3.Zero;
            Vector3 relPosition = position - center;
            if (relPosition.X > size.X / 2 - roundingThreshold)
            {
                modifier.X += distanceModifier * (relPosition.X - size.X / 2 + roundingThreshold)/roundingThreshold;
            }
            if (relPosition.X < -size.X / 2 + roundingThreshold)
            {
                modifier.X += distanceModifier * (relPosition.X + size.X / 2 - roundingThreshold) / roundingThreshold;
            }
            if (relPosition.Y > size.Y / 2 - roundingThreshold)
            {
                modifier.Y += distanceModifier * (relPosition.Y - size.Y / 2 + roundingThreshold) / roundingThreshold;
            }
            if (relPosition.Y < -size.Y / 2 + roundingThreshold)
            {
                modifier.Y += distanceModifier * (relPosition.Y + size.Y / 2 - roundingThreshold) / roundingThreshold;
            }
            if (relPosition.Z > size.Z / 2 - roundingThreshold)
            {
                modifier.Z += distanceModifier * (relPosition.Z - size.Z / 2 + roundingThreshold) / roundingThreshold;
            }
            if (relPosition.Z < -size.Z / 2 + roundingThreshold)
            {
                modifier.Z += distanceModifier * (relPosition.Z + size.Z / 2 - roundingThreshold) / roundingThreshold;
            }
            return position + modifier;
        }

        public VertexPositionColorNormal GenerateVertex(Vector3 position, Color color, Vector3 normal, float distanceModifier)
        {
            return new VertexPositionColorNormal(RaisedPosition(position, distanceModifier, 1f), color, normal);
        }
        
        public VertexPositionColorNormalTexture GenerateTexturedVertex(Vector3 position, Vector2 texCoord, Color color, Vector3 normal, float distanceModifier)
        {
            return new VertexPositionColorNormalTexture(RaisedPosition(position, distanceModifier, 1f), color, normal, texCoord);
        }

        public void AddStripToTriangleListHelper(Vertex start, Vertex end, float depth, EdgeProperties properties, List<VertexPositionColorNormalTexture> triangleList)
        {
            Vector3 edgeDir = end.position - start.position;
            Vector3 edgeNormal = Vector3.Cross(end.position - start.position, start.normal);
            edgeNormal.Normalize();
            edgeDir.Normalize();
            Color baseColor = Color.White;
            if(properties.type == VexedLib.EdgeType.Ice)
                baseColor = Color.White;
            if (properties.type == VexedLib.EdgeType.Magnet)
                baseColor = Color.Gray;
            if (properties.type == VexedLib.EdgeType.Bounce)
                baseColor = Color.Magenta;
            if (properties.type == VexedLib.EdgeType.Electric)
            {
                if (properties.primaryValue == 0)
                    baseColor = new Color(40, 40, 40);
                else
                    baseColor = Color.Yellow;
            }
            if (properties.type == VexedLib.EdgeType.ConveyorBelt)
                baseColor = Color.DarkGray;
            
            if (properties.type != VexedLib.EdgeType.Spikes)
            {
                float epsilon = .01f;

                //side
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position - .5f * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position - .5f * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(end.position - .5f * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));

                //top
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, depth + epsilon));
            }
            if (properties.type == VexedLib.EdgeType.Spikes)
            {
                int numSpikes = 2 * (int)(end.position - start.position).Length();
                float spikeHeight = .75f;
                float spikeWidth = (end.position - start.position).Length() / numSpikes;
                Color spikeColor = Color.LightGray;
                for (int i = 0; i < numSpikes; i++)
                {
                    Vector3 spikeStart = start.position + i * spikeWidth * edgeDir;
                    Vector3 spikeEnd = start.position + (i + 1) * spikeWidth * edgeDir;
                    Vector3 spikePoint = .5f * (spikeStart + spikeEnd) + spikeHeight * edgeNormal;

                    triangleList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, start.normal, depth));
                    triangleList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, start.normal, depth));
                    triangleList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, depth / 2));

                    triangleList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, -start.normal, 0));
                    triangleList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -start.normal, 0));
                    triangleList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, depth / 2));

                    triangleList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, edgeDir, depth));
                    triangleList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, edgeDir, 0));
                    triangleList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, depth / 2));

                    triangleList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -edgeDir, depth));
                    triangleList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -edgeDir, 0));
                    triangleList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, depth / 2));
                }
            
            }
            if (properties.type == VexedLib.EdgeType.ConveyorBelt)
            {
                int numSegments = (int)(end.position - start.position).Length();
                float segmentWidth = (end.position - start.position).Length() / numSegments;
                float arrowWidth = 0;
                if (properties.primaryValue > 0)
                    arrowWidth = -.5f;
                if (properties.primaryValue < 0)
                    arrowWidth = .5f;


                for (int i = 1; i < numSegments; i++)
                {
                    triangleList.Add(GenerateTexturedVertex(start.position + i * segmentWidth * edgeDir,blankTexCoords[1], Color.Yellow, start.normal, depth + .002f));
                    triangleList.Add(GenerateTexturedVertex(start.position + i * segmentWidth * edgeDir - .5f * edgeNormal,blankTexCoords[1], Color.Yellow, start.normal, depth + .002f));
                    triangleList.Add(GenerateTexturedVertex(start.position + arrowWidth * edgeDir + i * segmentWidth * edgeDir - .25f * edgeNormal,blankTexCoords[1], Color.Yellow, start.normal, depth + .002f));
                }
            }            
        }

        public void AddTopStrip(Vertex start, Vertex end, float depth, bool flip, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {           
            Color baseColor = Color.White;

            float epsilon = .01f;
            
            if (start.normal == end.normal)
            {                
                Vector3 edgeDir = end.position - start.position;
                Vector3 edgeNormal = Vector3.Cross(edgeDir, start.normal);
                edgeNormal.Normalize();
                edgeDir.Normalize();
                if(flip == true)
                    edgeNormal *= -1;            

                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[2], baseColor, edgeNormal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[3], baseColor, edgeNormal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[0], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[3], baseColor, edgeNormal, depth + epsilon));
            }
            else
            {
                Vector3 fullEdge = end.position - start.position;
                Vector3 currentComponent = Vector3.Dot(end.normal, fullEdge) * end.normal;
                Vector3 nextComponent = Vector3.Dot(start.normal, fullEdge) * start.normal;
                Vector3 constantComponent = Vector3.Dot(Vector3.Cross(end.normal, start.normal), fullEdge) * Vector3.Cross(end.normal, start.normal);
                float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                Vector3 midPoint = start.position + currentComponent + currentPercent * constantComponent;

                Vector3 edgeDir = midPoint - start.position;
                Vector3 edgeNormal = Vector3.Cross(midPoint - start.position, start.normal);
                edgeNormal.Normalize();
                edgeDir.Normalize();
                if (flip == true)
                    edgeNormal *= -1;    
                
                Vector2 midA = currentPercent * texCoords[3] + (1f-currentPercent) * texCoords[2];
                Vector2 midB = currentPercent * texCoords[0] + (1f-currentPercent) * texCoords[1];

                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[2], baseColor, edgeNormal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midA, baseColor, edgeNormal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midB, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midA, baseColor, edgeNormal, depth + epsilon));

                edgeDir = end.position - midPoint;
                edgeNormal = Vector3.Cross(end.position - midPoint, end.normal);
                edgeNormal.Normalize();
                edgeDir.Normalize();
                if (flip == true)
                    edgeNormal *= -1;    
                
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midA, baseColor, edgeNormal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midB, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[3], baseColor, edgeNormal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[0], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midB, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[3], baseColor, edgeNormal, depth + epsilon));
            
            }


        }

        public void AddStripToTriangleList2(Edge e, float depth, List<VertexPositionColorNormalTexture> triangleList)
        {
            float edgeLength = (e.end.position - e.start.position).Length();           

            if (e.start.normal != e.end.normal)
            {
                Vector3 fullEdge = e.end.position - e.start.position;
                Vector3 currentComponent = Vector3.Dot(e.end.normal, fullEdge) * e.end.normal;
                Vector3 nextComponent = Vector3.Dot(e.start.normal, fullEdge) * e.start.normal;
                Vector3 constantComponent = Vector3.Dot(Vector3.Cross(e.end.normal, e.start.normal), fullEdge) * Vector3.Cross(e.end.normal, e.start.normal);
                float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());
                Vector3 midPoint = e.start.position + currentComponent + currentPercent * constantComponent;
                edgeLength = (e.end.position - midPoint).Length() + (e.start.position - midPoint).Length();
            }

            int width = (int)(edgeLength+.5f);
            float blockWidth = edgeLength / width;


            List<Vertex> fullPointList = new List<Vertex>();
            for (int i = 0; i < width+1; i++)
            {
                Vector3 up = Vector3.Cross(e.start.normal, e.start.direction);
                fullPointList.Add(new Vertex(e.start, e.start.direction * i + up));
                fullPointList.Add(new Vertex(e.start, e.start.direction * i));
                fullPointList.Add(new Vertex(e.start, e.start.direction * i - up));
                
            }
            for (int i = 0; i < (width+1) * 3; i++)
            {
                fullPointList[i].Update(this, 0);
            }

            for (int i = 0; i < width; i++)
            {
                List<Vertex> subList = new List<Vertex>();
                Vector3 up = Vector3.Cross(fullPointList[(i * 3)].direction, fullPointList[i * 3].normal);
                if (i == width - 1)
                {
                    subList.Add(new Vertex(fullPointList[(i) * 3 + 1], .01f * up));
                    subList.Add(new Vertex(fullPointList[(i + 1) * 3 + 1], .01f * up));
                    subList.Add(fullPointList[(i + 1) * 3]);
                    subList.Add(fullPointList[i * 3]);
                }
                else
                {
                    subList.Add(new Vertex(fullPointList[(i+1) * 3 + 1], .01f*up));
                    subList.Add(new Vertex(fullPointList[(i) * 3 + 1], .01f*up));
                    subList.Add(fullPointList[(i) * 3]);
                    subList.Add(fullPointList[(i+1) * 3]);                    
                }

                if (e.properties.type == VexedLib.EdgeType.Ice)
                {
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, iceSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 3 + 1], fullPointList[i * 3 + 1], depth + .01f, true, iceTopEndTexCoords, triangleList);                        
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, iceSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, iceTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, iceSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, iceTopTexCoords, triangleList);
                    }
                }
                else if (e.properties.type == VexedLib.EdgeType.Bounce)
                {
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, rubberSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 3 + 1], fullPointList[i * 3 + 1], depth + .01f, true, rubberTopEndTexCoords, triangleList);
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, rubberSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, rubberTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, rubberSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, rubberTopTexCoords, triangleList);
                    }
                }
                else if (e.properties.type == VexedLib.EdgeType.ConveyorBelt)
                {
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, beltSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 3 + 1], fullPointList[i * 3 + 1], depth + .01f, true, beltTopEndTexCoords, triangleList);
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, beltSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, beltTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, beltSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, beltTopTexCoords, triangleList);
                    }
                }
                else if (e.properties.type == VexedLib.EdgeType.Electric && e.properties.primaryValue == 0)
                {
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, electricOffSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 3 + 1], fullPointList[i * 3 + 1], depth + .01f, true, electricOffTopEndTexCoords, triangleList);
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, electricOffSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, electricOffTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, electricOffSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, electricOffTopTexCoords, triangleList);
                    }
                }
                else if (e.properties.type == VexedLib.EdgeType.Electric && e.properties.primaryValue != 0)
                {
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, electricOnSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 3 + 1], fullPointList[i * 3 + 1], depth + .01f, true, electricOnTopEndTexCoords, triangleList);
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, electricOnSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, electricOnTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, electricOnSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, electricOnTopTexCoords, triangleList);
                    }
                }
                else
                {
                    if (i == width - 1 || i == 0)
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, magnetSideEndTexCoords, triangleList, true);
                    else
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, magnetSideTexCoords, triangleList, true);

                    AddTopStrip(fullPointList[i * 3 + 1], fullPointList[(i + 1) * 3 + 1], depth + .01f, false, magnetTopTexCoords, triangleList);
                }
            }

        }

        public void AddStripToTriangleList(Edge e, float depth, List<VertexPositionColorNormalTexture> triangleList)
        {
            if (e.start.normal != e.end.normal)
            {
                Vector3 fullEdge = e.end.position - e.start.position;
                Vector3 currentComponent = Vector3.Dot(e.end.normal, fullEdge) * e.end.normal;
                Vector3 nextComponent = Vector3.Dot(e.start.normal, fullEdge) * e.start.normal;
                Vector3 constantComponent = Vector3.Dot(Vector3.Cross(e.end.normal, e.start.normal), fullEdge) * Vector3.Cross(e.end.normal, e.start.normal);
                float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                Vector3 midPoint = e.start.position + currentComponent + currentPercent * constantComponent;

                AddStripToTriangleListHelper(e.start, new Vertex(midPoint, e.start.normal, e.start.velocity, e.start.direction), depth, e.properties, triangleList);
                AddStripToTriangleListHelper(new Vertex(midPoint, e.end.normal, e.end.velocity, e.end.direction), e.end, depth, e.properties, triangleList);
            }
            else
                AddStripToTriangleListHelper(e.start, e.end, depth, e.properties, triangleList);

        }

        public void AddSpikesToTriangleList(Edge e, float depth, List<VertexPositionColorNormalTexture> triangeList)
        {
            Vector3 edgeDir = e.end.position - e.start.position;
            int numSpikes = 2 * (int)edgeDir.Length();
            float spikeHeight = .75f;
            float spikeWidth = edgeDir.Length() / numSpikes;
            Vector3 edgeNormal = Vector3.Cross(e.end.position - e.start.position, e.start.normal);
            edgeNormal.Normalize();
            edgeDir.Normalize();
            Color spikeColor = Color.LightGray;

            if (e.start.velocity != Vector3.Zero)
            {
                AddStripToTriangleList(e, depth, triangeList);
            }
            else
            {
                for (int i = 0; i < numSpikes; i++)
                {
                    Vector3 spikeStart = e.start.position + i * spikeWidth * edgeDir;
                    Vector3 spikeEnd = e.start.position + (i + 1) * spikeWidth * edgeDir;
                    Vector3 spikePoint = .5f * (spikeStart + spikeEnd) + spikeHeight * edgeNormal;

                    triangeList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, e.start.normal, depth));
                    triangeList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, e.start.normal, depth));
                    triangeList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, depth / 2));

                    triangeList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, -e.start.normal, 0));
                    triangeList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -e.start.normal, 0));
                    triangeList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, depth / 2));

                    triangeList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, edgeDir, depth));
                    triangeList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, edgeDir, 0));
                    triangeList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, depth / 2));

                    triangeList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -edgeDir, depth));
                    triangeList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -edgeDir, 0));
                    triangeList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, depth / 2));

                    if (i < numSpikes - 1)
                    {
                        spikeStart = e.start.position + (.5f + i) * spikeWidth * edgeDir;
                        spikeEnd = e.start.position + (i + 1.5f) * spikeWidth * edgeDir;
                        spikePoint = .5f * (spikeStart + spikeEnd) + spikeHeight * edgeNormal;

                        triangeList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, e.start.normal, 0));
                        triangeList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, e.start.normal, 0));
                        triangeList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, -depth / 2));

                        triangeList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, -e.start.normal, -depth));
                        triangeList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -e.start.normal, -depth));
                        triangeList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, -depth / 2));

                        triangeList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, edgeDir, 0));
                        triangeList.Add(GenerateTexturedVertex(spikeStart,blankTexCoords[1], spikeColor, edgeDir, -depth));
                        triangeList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, -depth / 2));

                        triangeList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -edgeDir, 0));
                        triangeList.Add(GenerateTexturedVertex(spikeEnd,blankTexCoords[1], spikeColor, -edgeDir, -depth));
                        triangeList.Add(GenerateTexturedVertex(spikePoint,blankTexCoords[1], spikeColor, edgeNormal, -depth / 2));
                    }
                }
            }
        }


        public void AddBlockSidesToTriangleList(List<Vertex> vList, Color c, float depth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {
            for (int i = 0; i < 4; i++)
            {
                if (vList[i].normal != vList[(i + 1) % 4].normal)
                {
                    // corner edge case
                    Vector3 fullEdge = vList[(i + 1) % 4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i + 1) % 4].normal, fullEdge) * vList[(i + 1) % 4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    
                    Vector3 edgeNormal = Vector3.Cross(midPoint - vList[i].position, vList[i].normal);

                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[1], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[0], c, edgeNormal, depth));

                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[0], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[3], c, edgeNormal, -depth));

                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[1], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[0], c, edgeNormal, depth));

                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[0], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[3], c, edgeNormal, -depth));
                }
                else
                {
                    Vector3 edgeNormal = Vector3.Cross(vList[(i + 1) % 4].position - vList[i].position, vList[i].normal);
                    // straight edge case
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[1], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[0], c, edgeNormal, depth));

                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[0], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[3], c, edgeNormal, -depth));
                }
            }
        }



        public void AddBlockToTriangleList(List<Vertex> vList, Color c, float depth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();
            
            int jointVertexIndex = 0;
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                points.Add(vList[i]);
                pointsTexCoords.Add(texCoords[i]);
                count++;
                if (vList[i].normal != vList[(i+1)%4].normal)
                {
                    // corner edge case
                    Vector3 fullEdge = vList[(i+1)%4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i+1)%4].normal, fullEdge) * vList[(i+1)%4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i+1)%4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i+1)%4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    points.Add(new Vertex(midPoint, Vector3.Zero, Vector3.Zero, Vector3.Zero));
                    pointsTexCoords.Add((1 - currentPercent) * texCoords[i] + currentPercent * texCoords[(i + 1) % 4]);
                    jointVertexIndex = count;
                    count++;

                    Vector3 edgeNormal = Vector3.Cross(midPoint - vList[i].position, vList[i].normal);

                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[1], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[0], c, edgeNormal, depth));

                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[0], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[3], c, edgeNormal, -depth));

                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[1], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[0], c, edgeNormal, depth));

                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[0], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(midPoint, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[3], c, edgeNormal, -depth));
                }
                else
                {
                    Vector3 edgeNormal = Vector3.Cross(vList[(i+1)%4].position - vList[i].position, vList[i].normal);
                    // straight edge case
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[1], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[0], c, edgeNormal, depth));

                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[0], c, edgeNormal, depth));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, Room.blankTexCoords[2], c, edgeNormal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, Room.blankTexCoords[3], c, edgeNormal, -depth));
                }
            }

            if (points.Count == 4)
            {
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], c, vList[1].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], c, vList[3].normal, depth));

                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, -depth));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], c, vList[1].normal, -depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, -depth));
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, -depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, -depth));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], c, vList[3].normal, -depth));
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                    if (normal == Vector3.Zero)
                        normal = points[(jointVertexIndex + i + 1) % 6].normal;
                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], c, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], c, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], c, normal, depth));

                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], c, normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], c, normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], c, normal, -depth));
                }
            }

        }

        public void AddBlockFrontToTriangleList(List<Vertex> vList, Color c, float depth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList, bool outsideOnly)
        {
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();

            int jointVertexIndex = 0;
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                points.Add(vList[i]);
                pointsTexCoords.Add(texCoords[i]);
                count++;
                if (vList[i].normal != vList[(i + 1) % 4].normal)
                {
                    // corner edge case
                    Vector3 fullEdge = vList[(i + 1) % 4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i + 1) % 4].normal, fullEdge) * vList[(i + 1) % 4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    points.Add(new Vertex(midPoint, Vector3.Zero, Vector3.Zero, Vector3.Zero));
                    pointsTexCoords.Add((1 - currentPercent) * texCoords[i] + currentPercent * texCoords[(i + 1) % 4]);
                    jointVertexIndex = count;
                    count++;
                }                
            }

            if (points.Count == 4)
            {
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], c, vList[1].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], c, vList[3].normal, depth));

                if (outsideOnly == false)
                {
                    triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], c, vList[1].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], c, vList[3].normal, -depth));
                }
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                    if (normal == Vector3.Zero)
                        normal = points[(jointVertexIndex + i + 1) % 6].normal;
                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], c, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], c, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], c, normal, depth));

                    if (outsideOnly == false)
                    {
                        triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], c, normal, -depth));
                        triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], c, normal, -depth));
                        triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], c, normal, -depth));
                    }
                }
            }

        }


        public void AddBlockToTriangleList2(List<Vertex> vList, Color c, float depth, List<VertexPositionColorNormalTexture> triangleList)
        {
            AddBlockSidesToTriangleList(vList, c, depth, Room.blankTexCoords, triangleList);
            
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();
            
            List<Vertex> fullPointList = new List<Vertex>();
            List<Vector2> fullTexCoordsList = new List<Vector2>();

            //teal
            fullTexCoordsList.Add(new Vector2(0 * plateTexWidth, 1 * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(0 * plateTexWidth, .6f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.4f * plateTexWidth, .6f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.4f * plateTexWidth, 1f * plateTexWidth));
            
            //red
            fullTexCoordsList.Add(new Vector2(1 * plateTexWidth, 1 * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.6f * plateTexWidth, 1f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.6f * plateTexWidth, .6f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(1f * plateTexWidth, .6f * plateTexWidth));

            //green
            fullTexCoordsList.Add(new Vector2(1 * plateTexWidth, 0 * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(1f * plateTexWidth, .4f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.6f * plateTexWidth, .4f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.6f * plateTexWidth, 0 * plateTexWidth));
            
            //purple
            fullTexCoordsList.Add(new Vector2(0 * plateTexWidth, 0 * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.4f * plateTexWidth, 0f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.4f * plateTexWidth, .4f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(0 * plateTexWidth, .4f * plateTexWidth));

            for (int i = 0; i < 4; i++)
            {
                Vector3 incomingDirection = vList[(i+3)%4].direction;
                if (vList[(i + 3) % 4].normal != vList[i].normal)
                {
                    Vector3 fullEdge = vList[(i + 3) % 4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i + 3) % 4].normal, fullEdge) * vList[(i + 3) % 4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i + 3) % 4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i + 3) % 4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());
                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    incomingDirection = vList[i].position - midPoint;
                    incomingDirection.Normalize();
                }                                
                fullPointList.Add(new Vertex(vList[i], Vector3.Zero));
                fullPointList.Add(new Vertex(vList[i], -.5f * incomingDirection));
                fullPointList.Add(new Vertex(vList[i], .5f * vList[i].direction - .5f * incomingDirection));
                fullPointList.Add(new Vertex(vList[i], .5f * vList[i].direction));
            }
            for (int i = 0; i < 16; i++)
            {
                fullPointList[i].Update(this, 0);
            }


            for (int i = 0; i < 4; i++)
            {
                List<Vertex> subList = new List<Vertex>();
                List<Vector2> texCoordSubList = new List<Vector2>();
                subList.Add(fullPointList[i * 4 + 0]);
                subList.Add(fullPointList[i * 4 + 1]);
                subList.Add(fullPointList[i * 4 + 2]);
                subList.Add(fullPointList[i * 4 + 3]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 0]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 1]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 2]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 3]);
                AddBlockFrontToTriangleList(subList, c, depth, texCoordSubList, triangleList, false);
            }

            for (int i = 0; i < 4; i++)
            {
                List<Vertex> subList = new List<Vertex>();
                subList.Add(fullPointList[i * 4 + 3]);
                subList.Add(fullPointList[i * 4 + 2]);
                subList.Add(fullPointList[((i+1) * 4 + 2)%16]);
                subList.Add(fullPointList[((i+1) * 4 + 1)%16]);

                List<Vector2> texCoordSubList = new List<Vector2>();
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 3]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 2]);
                texCoordSubList.Add(fullTexCoordsList[((i + 1) * 4 + 2) % 16]);
                texCoordSubList.Add(fullTexCoordsList[((i+1) * 4 + 1)%16]);
                AddBlockFrontToTriangleList(subList, c, depth, texCoordSubList, triangleList, false);
            }

            List<Vertex> centerList = new List<Vertex>();
            centerList.Add(fullPointList[2]);
            centerList.Add(fullPointList[6]);
            centerList.Add(fullPointList[10]);
            centerList.Add(fullPointList[14]);
            List<Vector2> texCoordCenterList = new List<Vector2>();
            texCoordCenterList.Add(fullTexCoordsList[2]);
            texCoordCenterList.Add(fullTexCoordsList[6]);
            texCoordCenterList.Add(fullTexCoordsList[10]);
            texCoordCenterList.Add(fullTexCoordsList[14]);
            AddBlockFrontToTriangleList(centerList, c, depth, texCoordCenterList, triangleList, false);            
        }


        public void AddTextureToTriangleList(List<Vertex> vList, Color c, float depth, List<VertexPositionColorNormalTexture> triangleList, List<Vector2> inputTexCoords, bool flipHorizontal)
        {
            List<Vector2> texCoords = new List<Vector2>();
            if (flipHorizontal)
            {
                texCoords.Add(new Vector2(inputTexCoords[1].X, inputTexCoords[0].Y));
                texCoords.Add(new Vector2(inputTexCoords[0].X, inputTexCoords[1].Y));
                texCoords.Add(new Vector2(inputTexCoords[3].X, inputTexCoords[2].Y));
                texCoords.Add(new Vector2(inputTexCoords[2].X, inputTexCoords[3].Y));
            }
            else
            {
                texCoords.Add(new Vector2(inputTexCoords[0].X, inputTexCoords[0].Y));
                texCoords.Add(new Vector2(inputTexCoords[1].X, inputTexCoords[1].Y));
                texCoords.Add(new Vector2(inputTexCoords[2].X, inputTexCoords[2].Y));
                texCoords.Add(new Vector2(inputTexCoords[3].X, inputTexCoords[3].Y));
            }
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();
            int jointVertexIndex = 0;
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                points.Add(vList[i]);
                pointsTexCoords.Add(texCoords[i]);
                count++;
                if (vList[i].normal != vList[(i + 1) % 4].normal)
                {
                    // corner edge case
                    Vector3 fullEdge = vList[(i + 1) % 4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i + 1) % 4].normal, fullEdge) * vList[(i + 1) % 4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    points.Add(new Vertex(midPoint, Vector3.Zero, Vector3.Zero, Vector3.Zero));
                    pointsTexCoords.Add((1-currentPercent) * texCoords[i] + currentPercent * texCoords[(i + 1) % 4]);
                    jointVertexIndex = count;
                    count++;
                }
            }

            if (points.Count == 4)
            {
                triangleList.Add(GenerateTexturedVertex(vList[0].position,texCoords[0],c, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], c, vList[1].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, depth));

                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], c, vList[3].normal, depth));

            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                    if (normal == Vector3.Zero)
                        normal = points[(jointVertexIndex + i + 1) % 6].normal;
                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], c, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], c, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], c, normal, depth));
                }
            }

        }

        public void DrawMonsters()
        {
            foreach (Monster m in monsters)
            {
                m.Draw(this);
            }
        }

        public void DrawProjectiles()
        {
            foreach (Projectile p in projectiles)
            {
                p.Draw(this);
            }
        }

        public void UpdateMonsters(GameTime gameTime)
        {
            foreach (Monster m in monsters)
            {
                m.Update(gameTime);
            }
            for (int i = monsters.Count() - 1; i >= 0; i--)
            {
                if (monsters[i].dead == true)
                {
                    monsters.Remove(monsters[i]);
                }
            }
            foreach (Projectile p in projectiles)
            {
                p.Update(gameTime);
            }
            for(int i = projectiles.Count()-1; i >= 0; i--)
            {
                if (projectiles[i].exploded == true)
                {
                    projectiles.Remove(projectiles[i]);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            Color interiorColor = new Color(20, 20, 20);
            
            if (innerBlockMode == 2)
                interiorColor.A = 150;
            
            
            #region innerBlock
            Vector3 adjustedSize = new Vector3(size.X - .1f, size.Y - .1f, size.Z - .1f);
            if (innerBlockMode > 0)
            {
                List<VertexPositionColorNormalTexture> translucentTriangleList = new List<VertexPositionColorNormalTexture>();
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitX, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitX, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitX, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitX, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitY, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitY, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitY, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitY, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitZ, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, Vector3.UnitZ, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitZ, -.5f));

                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f,.5f), interiorColor, -Vector3.UnitZ, -.5f));
                for (int i = 0; i < translucentTriangleList.Count(); i+=6)
                {
                    TrasnparentSquare t = new TrasnparentSquare(translucentTriangleList[i], translucentTriangleList[i + 1], translucentTriangleList[i + 2], translucentTriangleList[i+3], translucentTriangleList[i + 4], translucentTriangleList[i + 5]);
                    Game1.staticTranslucentObjects.Add(t);
                }
            }
            #endregion
            

            #region Blocks
            foreach (Block b in blocks)
            {
                List<Vertex> vList = new List<Vertex>();
                vList.Add(b.edges[0].start);
                vList.Add(b.edges[1].start);
                vList.Add(b.edges[2].start);
                vList.Add(b.edges[3].start);

                if (b.staticObject == false)
                {
                    AddBlockToTriangleList2(vList, b.color, .5f, Game1.dynamicOpaqueObjects);
                    //AddBlockToTriangleList(vList, b.color, .5f, Room.plateTexCoords, Game1.dynamicOpaqueObjects);
                    foreach (Edge e in b.edges)
                    {
                        if (e.properties.type == VexedLib.EdgeType.Spikes)
                            AddSpikesToTriangleList(e, .5f, Game1.dynamicOpaqueObjects);
                        else if (e.properties.type != VexedLib.EdgeType.Normal)
                            AddStripToTriangleList2(e, .5f, Game1.dynamicDetailObjects);
                    }
                }
                else 
                {
                    //AddBlockToTriangleList(vList, b.color, .5f, Room.plateTexCoords, Game1.staticOpaqueObjects);
                    if (Game1.staticObjectsInitialized == false)
                        AddBlockToTriangleList2(vList, b.color, .5f, Game1.staticOpaqueObjects);

                    foreach (Edge e in b.edges)
                    {
                        if (e.properties.type == VexedLib.EdgeType.Spikes || e.properties.type == VexedLib.EdgeType.Electric || e.properties.type == VexedLib.EdgeType.ConveyorBelt)
                        {
                            if (e.properties.type == VexedLib.EdgeType.Spikes)
                                AddSpikesToTriangleList(e, .5f, Game1.dynamicOpaqueObjects);
                            else if (e.properties.type != VexedLib.EdgeType.Normal)
                                AddStripToTriangleList2(e, .5f, Game1.dynamicDetailObjects);
                        }
                        else if (Game1.staticObjectsInitialized == false)
                        {
                            if (e.properties.type == VexedLib.EdgeType.Spikes)
                                AddSpikesToTriangleList(e, .5f, Game1.staticOpaqueObjects);
                            else if (e.properties.type != VexedLib.EdgeType.Normal)
                                AddStripToTriangleList2(e, .5f, Game1.staticDetailObjects);
                        }
                    }
                }

                
                
                
            }
            #endregion

            #region Doodads
            if (Game1.staticObjectsInitialized == false)
            {
                foreach (JumpPad j in jumpPads)
                {
                    j.Draw(this, Game1.staticOpaqueObjects);
                }
                foreach (Bridge b in bridges)
                {
                    b.Draw(this, Game1.staticOpaqueObjects);
                }
            }
            foreach (Doodad d in doodads)
            {
                d.Draw(this, Game1.dynamicOpaqueObjects);
            }
            #endregion
        }

    }
}

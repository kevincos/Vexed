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

        public List<Block> blocks;
        public List<JumpPad> jumpPads;
        public List<Bridge> bridges;
        public List<Doodad> doodads;        
        
        public Room()
        {
            blocks = new List<Block>();
            jumpPads = new List<JumpPad>();
            bridges = new List<Bridge>();
            doodads = new List<Doodad>();            
        }

        public Room(VexedLib.Room xmlRoom)
        {
            center = new Vector3(xmlRoom.centerX, xmlRoom.centerY, xmlRoom.centerZ);
            size = new Vector3(xmlRoom.sizeX, xmlRoom.sizeY, xmlRoom.sizeZ);
            blocks = new List<Block>();
            jumpPads = new List<JumpPad>();
            bridges = new List<Bridge>();
            doodads = new List<Doodad>();            
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
            foreach (Doodad d in doodads)
            {
                d.UpdateBehavior(gameTime);
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

        public VertexPositionColorNormalTexture GenerateTexturedVertex(Vector3 position, Vector2 texCoord, Vector3 normal, float distanceModifier)
        {
            return new VertexPositionColorNormalTexture(RaisedPosition(position, distanceModifier, 1f), Color.White, normal, texCoord);
        }

        public void AddStripToTriangleListHelper(Vertex start, Vertex end, float depth, EdgeProperties properties, List<VertexPositionColorNormal> triangleList)
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
                triangleList.Add(GenerateVertex(start.position, baseColor, start.normal, depth + .001f));
                triangleList.Add(GenerateVertex(start.position - .5f * edgeNormal, baseColor, start.normal, depth + .001f));
                triangleList.Add(GenerateVertex(end.position, baseColor, start.normal, depth + .001f));

                triangleList.Add(GenerateVertex(end.position, baseColor, start.normal, depth + .001f));
                triangleList.Add(GenerateVertex(start.position - .5f * edgeNormal, baseColor, start.normal, depth + .001f));
                triangleList.Add(GenerateVertex(end.position - .5f * edgeNormal, baseColor, start.normal, depth + .001f));

                triangleList.Add(GenerateVertex(start.position + .001f * edgeNormal, baseColor, edgeNormal, depth));
                triangleList.Add(GenerateVertex(start.position + .001f * edgeNormal, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateVertex(end.position + .001f * edgeNormal, baseColor, edgeNormal, depth));

                triangleList.Add(GenerateVertex(end.position + .001f * edgeNormal, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateVertex(start.position + .001f * edgeNormal, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateVertex(end.position + .001f * edgeNormal, baseColor, edgeNormal, depth));
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

                    triangleList.Add(GenerateVertex(spikeStart, spikeColor, start.normal, depth));
                    triangleList.Add(GenerateVertex(spikeEnd, spikeColor, start.normal, depth));
                    triangleList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, depth / 2));

                    triangleList.Add(GenerateVertex(spikeStart, spikeColor, -start.normal, 0));
                    triangleList.Add(GenerateVertex(spikeEnd, spikeColor, -start.normal, 0));
                    triangleList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, depth / 2));

                    triangleList.Add(GenerateVertex(spikeStart, spikeColor, edgeDir, depth));
                    triangleList.Add(GenerateVertex(spikeStart, spikeColor, edgeDir, 0));
                    triangleList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, depth / 2));

                    triangleList.Add(GenerateVertex(spikeEnd, spikeColor, -edgeDir, depth));
                    triangleList.Add(GenerateVertex(spikeEnd, spikeColor, -edgeDir, 0));
                    triangleList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, depth / 2));
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
                    triangleList.Add(GenerateVertex(start.position + i * segmentWidth * edgeDir, Color.Yellow, start.normal, depth + .002f));
                    triangleList.Add(GenerateVertex(start.position + i * segmentWidth * edgeDir - .5f * edgeNormal, Color.Yellow, start.normal, depth + .002f));
                    triangleList.Add(GenerateVertex(start.position + arrowWidth * edgeDir + i * segmentWidth * edgeDir - .25f * edgeNormal, Color.Yellow, start.normal, depth + .002f));
                }
            }            
        }

        public void AddStripToTriangleList(Edge e, float depth, List<VertexPositionColorNormal> triangleList)
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

        public void AddSpikesToTriangleList(Edge e, float depth, List<VertexPositionColorNormal> triangeList)
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

                    triangeList.Add(GenerateVertex(spikeStart, spikeColor, e.start.normal, depth));
                    triangeList.Add(GenerateVertex(spikeEnd, spikeColor, e.start.normal, depth));
                    triangeList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, depth / 2));

                    triangeList.Add(GenerateVertex(spikeStart, spikeColor, -e.start.normal, 0));
                    triangeList.Add(GenerateVertex(spikeEnd, spikeColor, -e.start.normal, 0));
                    triangeList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, depth / 2));

                    triangeList.Add(GenerateVertex(spikeStart, spikeColor, edgeDir, depth));
                    triangeList.Add(GenerateVertex(spikeStart, spikeColor, edgeDir, 0));
                    triangeList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, depth / 2));

                    triangeList.Add(GenerateVertex(spikeEnd, spikeColor, -edgeDir, depth));
                    triangeList.Add(GenerateVertex(spikeEnd, spikeColor, -edgeDir, 0));
                    triangeList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, depth / 2));

                    if (i < numSpikes - 1)
                    {
                        spikeStart = e.start.position + (.5f + i) * spikeWidth * edgeDir;
                        spikeEnd = e.start.position + (i + 1.5f) * spikeWidth * edgeDir;
                        spikePoint = .5f * (spikeStart + spikeEnd) + spikeHeight * edgeNormal;

                        triangeList.Add(GenerateVertex(spikeStart, spikeColor, e.start.normal, 0));
                        triangeList.Add(GenerateVertex(spikeEnd, spikeColor, e.start.normal, 0));
                        triangeList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, -depth / 2));

                        triangeList.Add(GenerateVertex(spikeStart, spikeColor, -e.start.normal, -depth));
                        triangeList.Add(GenerateVertex(spikeEnd, spikeColor, -e.start.normal, -depth));
                        triangeList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, -depth / 2));

                        triangeList.Add(GenerateVertex(spikeStart, spikeColor, edgeDir, 0));
                        triangeList.Add(GenerateVertex(spikeStart, spikeColor, edgeDir, -depth));
                        triangeList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, -depth / 2));

                        triangeList.Add(GenerateVertex(spikeEnd, spikeColor, -edgeDir, 0));
                        triangeList.Add(GenerateVertex(spikeEnd, spikeColor, -edgeDir, -depth));
                        triangeList.Add(GenerateVertex(spikePoint, spikeColor, edgeNormal, -depth / 2));
                    }
                }
            }
        }

        public void AddBlockToTriangleList(List<Vertex> vList, Color c, float depth, List<VertexPositionColorNormal> triangleList)
        {
            List<Vertex> points = new List<Vertex>();
            int jointVertexIndex = 0;
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                points.Add(vList[i]);
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
                    jointVertexIndex = count;
                    count++;

                    Vector3 edgeNormal = Vector3.Cross(midPoint - vList[i].position, vList[i].normal);

                    triangleList.Add(GenerateVertex(vList[i].position, c, edgeNormal, depth));
                    triangleList.Add(GenerateVertex(vList[i].position, c, edgeNormal, -depth));
                    triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, depth));

                    triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, depth));
                    triangleList.Add(GenerateVertex(vList[i].position, c, edgeNormal, -depth));
                    triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, -depth));

                    triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, depth));
                    triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, -depth));
                    triangleList.Add(GenerateVertex(vList[(i+1)%4].position, c, edgeNormal, depth));

                    triangleList.Add(GenerateVertex(vList[(i+1)%4].position, c, edgeNormal, depth));
                    triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, -depth));
                    triangleList.Add(GenerateVertex(vList[(i+1)%4].position, c, edgeNormal, -depth));
                }
                else
                {
                    Vector3 edgeNormal = Vector3.Cross(vList[(i+1)%4].position - vList[i].position, vList[i].normal);
                    // straight edge case
                    triangleList.Add(GenerateVertex(vList[i].position, c, edgeNormal, depth));
                    triangleList.Add(GenerateVertex(vList[i].position, c, edgeNormal, -depth));
                    triangleList.Add(GenerateVertex(vList[(i+1)%4].position, c, edgeNormal, depth));

                    triangleList.Add(GenerateVertex(vList[(i+1)%4].position, c, edgeNormal, depth));
                    triangleList.Add(GenerateVertex(vList[i].position, c, edgeNormal, -depth));
                    triangleList.Add(GenerateVertex(vList[(i+1)%4].position, c, edgeNormal, -depth));
                }
            }

            if (points.Count == 4)
            {
                triangleList.Add(GenerateVertex(vList[0].position, c, vList[0].normal, depth));
                triangleList.Add(GenerateVertex(vList[1].position, c, vList[1].normal, depth));
                triangleList.Add(GenerateVertex(vList[2].position, c, vList[2].normal, depth));
                triangleList.Add(GenerateVertex(vList[0].position, c, vList[0].normal, depth));
                triangleList.Add(GenerateVertex(vList[2].position, c, vList[2].normal, depth));
                triangleList.Add(GenerateVertex(vList[3].position, c, vList[3].normal, depth));

                triangleList.Add(GenerateVertex(vList[0].position, c, vList[0].normal, -depth));
                triangleList.Add(GenerateVertex(vList[1].position, c, vList[1].normal, -depth));
                triangleList.Add(GenerateVertex(vList[2].position, c, vList[2].normal, -depth));
                triangleList.Add(GenerateVertex(vList[0].position, c, vList[0].normal, -depth));
                triangleList.Add(GenerateVertex(vList[2].position, c, vList[2].normal, -depth));
                triangleList.Add(GenerateVertex(vList[3].position, c, vList[3].normal, -depth));
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                    if (normal == Vector3.Zero)
                        normal = points[(jointVertexIndex + i + 1) % 6].normal;
                    triangleList.Add(GenerateVertex(points[jointVertexIndex].position, c, normal, depth));
                    triangleList.Add(GenerateVertex(points[(jointVertexIndex + i) % 6].position, c, normal, depth));
                    triangleList.Add(GenerateVertex(points[(jointVertexIndex + i + 1) % 6].position, c, normal, depth));

                    triangleList.Add(GenerateVertex(points[jointVertexIndex].position, c, normal, -depth));
                    triangleList.Add(GenerateVertex(points[(jointVertexIndex + i) % 6].position, c, normal, -depth));
                    triangleList.Add(GenerateVertex(points[(jointVertexIndex + i + 1) % 6].position, c, normal, -depth));
                }
            }

        }

        public void AddTextureToTriangleList(List<Vertex> vList, Color c, float depth, List<VertexPositionColorNormalTexture> triangleList, bool flipHorizontal)
        {
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();
            int jointVertexIndex = 0;
            int count = 0;

            List<Vector2> texCoords = new List<Vector2>();
            if (flipHorizontal == false)
            {
                texCoords.Add(new Vector2(.125f, 0));
                texCoords.Add(new Vector2(.875f, 0));
                texCoords.Add(new Vector2(.875f, 1));
                texCoords.Add(new Vector2(.125f, 1));
            }
            else
            {
                texCoords.Add(new Vector2(.875f, 0));
                texCoords.Add(new Vector2(.125f, 0));
                texCoords.Add(new Vector2(.125f, 1));
                texCoords.Add(new Vector2(.875f, 1));
            }

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
                triangleList.Add(GenerateTexturedVertex(vList[0].position,texCoords[0], vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], vList[1].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], vList[2].normal, depth));

                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], vList[3].normal, depth));

            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                    if (normal == Vector3.Zero)
                        normal = points[(jointVertexIndex + i + 1) % 6].normal;
                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], normal, depth));
                }
            }

        }


        public void Draw(GameTime gameTime)
        {            
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();

            

            Color interiorColor = new Color(20, 20, 20);
            
            if (innerBlockMode == 2)
                interiorColor.A = 150;
            
            
            #region innerBlock
            Vector3 adjustedSize = new Vector3(size.X - .1f, size.Y - .1f, size.Z - .1f);
            if (innerBlockMode > 0)
            {
                List<VertexPositionColorNormal> translucentTriangleList = new List<VertexPositionColorNormal>();
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, Vector3.UnitX, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, Vector3.UnitX, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitX, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitX, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, Vector3.UnitY, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, Vector3.UnitY, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitY, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitY, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitZ, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), interiorColor, Vector3.UnitZ, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));

                translucentTriangleList.Add(GenerateVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
                translucentTriangleList.Add(GenerateVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
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
                    AddBlockToTriangleList(vList, b.color, .5f, triangleList);
                }
                else if(Game1.staticObjectsInitialized == false)
                {
                    AddBlockToTriangleList(vList, b.color, .5f, Game1.staticOpaqueObjects);
                }

                foreach (Edge e in b.edges)
                {
                    if (e.properties.type == VexedLib.EdgeType.Spikes)
                        AddSpikesToTriangleList(e, .5f, triangleList);
                    else if (e.properties.type != VexedLib.EdgeType.Normal)
                        AddStripToTriangleList(e, .5f, triangleList);
                }
                
                
            }
            #endregion

            #region Doodads
            foreach (JumpPad j in jumpPads)
            {
                j.Draw(this, triangleList);
            }
            foreach (Bridge b in bridges)
            {
                b.Draw(this, triangleList);
            }
            foreach (Doodad b in doodads)
            {
                b.Draw(this, triangleList);
            }
            #endregion

            if (triangleList.Count > 0)
            {
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    triangleList.ToArray(), 0, triangleList.Count / 3, VertexPositionColorNormal.VertexDeclaration);
            }
        }
    }
}

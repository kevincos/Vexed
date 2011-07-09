﻿using System;
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
        public Vector3 center;
        public Vector3 size;
        

        public List<Block> blocks;
        public List<JumpPad> jumpPads;
        public List<Bridge> bridges;

        public Room(VexedLib.Room xmlRoom)
        {
            center = new Vector3(xmlRoom.centerX, xmlRoom.centerY, xmlRoom.centerZ);
            size = new Vector3(xmlRoom.sizeX, xmlRoom.sizeY, xmlRoom.sizeZ);
            blocks = new List<Block>();
            jumpPads = new List<JumpPad>();
            bridges = new List<Bridge>();
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

        public void Draw(GameTime gameTime)
        {            
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();

            

            Color interiorColor = new Color(40, 40, 40);
            
            #region innerBlock
            triangleList.Add(GenerateVertex(center + new Vector3(size.X/2, size.Y/2, size.Z/2), interiorColor, Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X/2, -size.Y/2, size.Z/2), interiorColor, Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X/2, size.Y/2, -size.Z/2), interiorColor, Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X/2, size.Y/2, -size.Z/2), interiorColor, Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X/2, -size.Y/2, size.Z/2), interiorColor, Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X/2, -size.Y/2, -size.Z/2), interiorColor, Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, size.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, -size.Y / 2, size.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, -size.Y / 2, size.Z / 2), interiorColor, -Vector3.UnitX, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, -size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitX, -.5f));

            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, size.Y / 2, -size.Z / 2), interiorColor, Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, size.Y / 2, -size.Z / 2), interiorColor, Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, -size.Z / 2), interiorColor, Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, -size.Y / 2, size.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, -size.Y / 2, size.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, -size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, -size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, -size.Y / 2, size.Z / 2), interiorColor, -Vector3.UnitY, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, -size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitY, -.5f));

            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, -size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, -size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, -size.Y / 2, size.Z / 2), interiorColor, Vector3.UnitZ, -.5f));

            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, -size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(size.X / 2, -size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
            triangleList.Add(GenerateVertex(center + new Vector3(-size.X / 2, -size.Y / 2, -size.Z / 2), interiorColor, -Vector3.UnitZ, -.5f));
            #endregion

            #region Blocks
            foreach (Block b in blocks)
            {
                List<Vertex> vList = new List<Vertex>();
                vList.Add(b.edges[0].start);
                vList.Add(b.edges[1].start);
                vList.Add(b.edges[2].start);
                vList.Add(b.edges[3].start);
                AddBlockToTriangleList(vList, b.color, .5f, triangleList);
            }
            #endregion

            #region Doodads
            foreach (JumpPad j in jumpPads)
            {
                Vector3 up = Vector3.UnitX;
                if (up == j.position.normal)
                    up = Vector3.UnitY;
                Vector3 right = Vector3.Cross(up, j.position.normal);
                List<Vertex> vList = new List<Vertex>();
                vList.Add(new Vertex(j.position, +.5f * up + .5f * right));
                vList.Add(new Vertex(j.position, +.5f * up - .5f * right));
                vList.Add(new Vertex(j.position, -.5f * up - .5f * right));
                vList.Add(new Vertex(j.position, -.5f * up + .5f * right));
                if(j.active == true)
                    AddBlockToTriangleList(vList, Color.Yellow, .1f, triangleList);
                else
                    AddBlockToTriangleList(vList, Color.HotPink, .1f, triangleList);
            }
            foreach (Bridge b in bridges)
            {
                Vector3 up = Vector3.UnitX;
                if (up == b.position.normal)
                    up = Vector3.UnitY;
                Vector3 right = Vector3.Cross(up, b.position.normal);
                List<Vertex> vList = new List<Vertex>();
                vList.Add(new Vertex(b.position, +.5f * up + .5f * right));
                vList.Add(new Vertex(b.position, +.5f * up - .5f * right));
                vList.Add(new Vertex(b.position, -.5f * up - .5f * right));
                vList.Add(new Vertex(b.position, -.5f * up + .5f * right));
                if (b.active == true)
                    AddBlockToTriangleList(vList, Color.Blue, .1f, triangleList);
                else
                    AddBlockToTriangleList(vList, Color.Green, .1f, triangleList);
            }
            #endregion

            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleList.ToArray(), 0, triangleList.Count / 3, VertexPositionColorNormal.VertexDeclaration);            
        }
    }
}

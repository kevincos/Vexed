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
        public Vector3 center;
        public Vector3 size;
        public BasicEffect effect = null;

        public List<Block> blocks;

        public Room(VexedLib.Room xmlRoom)
        {
            center = new Vector3(xmlRoom.centerX, xmlRoom.centerY, xmlRoom.centerZ);
            size = new Vector3(xmlRoom.sizeX, xmlRoom.sizeY, xmlRoom.sizeZ);
            blocks = new List<Block>();
        }

        public void UpdateVertex(Vertex v, int updateTime)
        {
            v.position += updateTime * v.velocity;
            Vector3 relative = v.position - center;
            float overFlow = 0;
            Vector3 oldNormal = v.normal;
            if (relative.X > size.X / 2)
            {
                v.normal = new Vector3(1, 0, 0);
                overFlow = relative.X - size.X / 2;
                v.position.X -= overFlow;
            }
            if (relative.X < -size.X / 2)
            {
                v.normal = new Vector3(-1, 0, 0);
                overFlow = Math.Abs(relative.X) - size.X / 2;
                v.position.X += overFlow;
            }
            if (relative.Y > size.Y / 2)
            {
                v.normal = new Vector3(0, 1, 0);
                overFlow = relative.Y - size.Y / 2;
                v.position.Y -= overFlow;                
            }
            if (relative.Y < -size.Y / 2)
            {
                v.normal = new Vector3(0, -1, 0);
                overFlow = Math.Abs(relative.Y) - size.Y / 2;
                v.position.Y += overFlow;
            }
            if (relative.Z > size.Z / 2)
            {
                v.normal = new Vector3(0, 0, 1);
                overFlow = relative.Z - size.Z / 2;
                v.position.Z -= overFlow;
            }
            if (relative.Z < -size.Z / 2)
            {
                v.normal = new Vector3(0, 0, -1);
                overFlow = Math.Abs(relative.Z) - size.Z / 2;
                v.position.Z += overFlow;
            }
            if (overFlow > 0)
            {                
                v.position -= overFlow * oldNormal;
                Vector3 oldVelocity = Vector3.Dot(v.normal, v.velocity) * v.normal;
                Vector3 newVelocity = -oldVelocity.Length() * oldNormal;
                v.velocity = v.velocity - oldVelocity + newVelocity;

                Vector3 oldDirection = Vector3.Dot(v.normal, v.direction) * v.normal;
                Vector3 newDirection = -oldDirection.Length() * oldNormal;
                v.direction = v.direction - oldDirection + newDirection;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Block b in blocks)
            {
                int blockUpdateTime = b.UpdateBehavior(gameTime);
                foreach (Edge e in b.edges)
                {
                    UpdateVertex(e.start, blockUpdateTime);
                    UpdateVertex(e.end, blockUpdateTime);
                }
            }
        }

        public VertexPositionColorNormal GenerateVertex(Vector3 position, Color color, Vector3 normal, float distanceModifier)
        {
            Vector3 modifier = Vector3.Zero;
            if (position.X == size.X / 2)
            {
                modifier.X += distanceModifier;
            }
            if (position.X == -size.X / 2)
            {
                modifier.X -= distanceModifier;
            }
            if (position.Y == size.Y / 2)
            {
                modifier.Y += distanceModifier;
            }
            if (position.Y == -size.Y / 2)
            {
                modifier.Y -= distanceModifier;
            }
            if (position.Z == size.Z / 2)
            {
                modifier.Z += distanceModifier;
            }
            if (position.Z == -size.Z / 2)
            {
                modifier.Z -= distanceModifier;
            }
            return new VertexPositionColorNormal(position + modifier, color, normal);
        }

        public void Draw(GameTime gameTime)
        {
            Vector3 currentTarget = Vector3.Zero;
            Vector3 currentCamera = new Vector3(30, 30, 30);
            Vector3 currentUp = new Vector3(0, 0, 1);
            float currentRotate = 0;
            float currentPitch = 0;

            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();


            Color interiorColor = new Color(40, 40, 40);
            // posX
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
            
            
            
            foreach (Block b in blocks)
            {
                Color c = b.color;
                List<Vertex> points = new List<Vertex>();
                int jointVertexIndex = 0;
                int count = 0;
                foreach (Edge e in b.edges)
                {
                    points.Add(e.start);
                    count++;
                    if (e.start.normal != e.end.normal)
                    {
                        // corner edge case
                        Vector3 fullEdge = e.end.position - e.start.position;
                        Vector3 currentComponent = Vector3.Dot(e.end.normal, fullEdge) * e.end.normal;
                        Vector3 nextComponent = Vector3.Dot(e.start.normal, fullEdge) * e.start.normal;
                        Vector3 constantComponent = Vector3.Dot(Vector3.Cross(e.end.normal, e.start.normal), fullEdge) * Vector3.Cross(e.end.normal, e.start.normal);
                        float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                        Vector3 midPoint = e.start.position + currentComponent + currentPercent * constantComponent;
                        points.Add(new Vertex(midPoint, Vector3.Zero, Vector3.Zero, Vector3.Zero));
                        jointVertexIndex = count;
                        count++;

                        Vector3 edgeNormal = Vector3.Cross(midPoint - e.start.position, e.start.normal);

                        triangleList.Add(GenerateVertex(e.start.position, c, edgeNormal, .5f));
                        triangleList.Add(GenerateVertex(e.start.position, c, edgeNormal, -.5f));
                        triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, .5f));

                        triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, .5f));
                        triangleList.Add(GenerateVertex(e.start.position, c, edgeNormal, -.5f));
                        triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, -.5f));

                        triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, .5f));
                        triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, -.5f));
                        triangleList.Add(GenerateVertex(e.end.position, c, edgeNormal, .5f));

                        triangleList.Add(GenerateVertex(e.end.position, c, edgeNormal, .5f));
                        triangleList.Add(GenerateVertex(midPoint, c, edgeNormal, -.5f));
                        triangleList.Add(GenerateVertex(e.end.position, c, edgeNormal, -.5f));
                    }
                    else
                    {
                        Vector3 edgeNormal = Vector3.Cross(e.end.position - e.start.position, e.start.normal);
                        // straight edge case
                        triangleList.Add(GenerateVertex(e.start.position, c, edgeNormal, .5f));
                        triangleList.Add(GenerateVertex(e.start.position, c, edgeNormal, -.5f));
                        triangleList.Add(GenerateVertex(e.end.position, c, edgeNormal, .5f));

                        triangleList.Add(GenerateVertex(e.end.position, c, edgeNormal, .5f));
                        triangleList.Add(GenerateVertex(e.start.position, c, edgeNormal, -.5f));
                        triangleList.Add(GenerateVertex(e.end.position, c, edgeNormal, -.5f));
                        /*if (c == Color.Blue)
                        {
                            triangleList.Add(new VertexPositionColorNormal(e.start.position, Color.Red, edgeNormal));
                            triangleList.Add(new VertexPositionColorNormal(e.end.position, Color.Red, edgeNormal));
                            triangleList.Add(new VertexPositionColorNormal(edgeNormal + .5f * (e.end.position + e.start.position), Color.Red, edgeNormal));
                        }*/
                    }
                }

                if (points.Count == 4)
                {
                    triangleList.Add(GenerateVertex(b.edges[0].start.position, c, b.edges[0].start.normal,.5f));
                    triangleList.Add(GenerateVertex(b.edges[1].start.position, c, b.edges[1].start.normal, .5f));
                    triangleList.Add(GenerateVertex(b.edges[2].start.position, c, b.edges[2].start.normal, .5f));
                    triangleList.Add(GenerateVertex(b.edges[0].start.position, c, b.edges[0].start.normal, .5f));
                    triangleList.Add(GenerateVertex(b.edges[2].start.position, c, b.edges[2].start.normal, .5f));
                    triangleList.Add(GenerateVertex(b.edges[3].start.position, c, b.edges[3].start.normal, .5f));

                    triangleList.Add(GenerateVertex(b.edges[0].start.position, c, b.edges[0].start.normal, -.5f));
                    triangleList.Add(GenerateVertex(b.edges[1].start.position, c, b.edges[1].start.normal, -.5f));
                    triangleList.Add(GenerateVertex(b.edges[2].start.position, c, b.edges[2].start.normal, -.5f));
                    triangleList.Add(GenerateVertex(b.edges[0].start.position, c, b.edges[0].start.normal, -.5f));
                    triangleList.Add(GenerateVertex(b.edges[2].start.position, c, b.edges[2].start.normal, -.5f));
                    triangleList.Add(GenerateVertex(b.edges[3].start.position, c, b.edges[3].start.normal, -.5f));

                }
                else
                {
                    for (int i = 1; i < 5; i++)
                    {
                        Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                        if(normal == Vector3.Zero)
                            normal = points[(jointVertexIndex + i+1) % 6].normal;
                        triangleList.Add(GenerateVertex(points[jointVertexIndex].position, c, normal, .5f));
                        triangleList.Add(GenerateVertex(points[(jointVertexIndex + i) % 6].position, c, normal, .5f));
                        triangleList.Add(GenerateVertex(points[(jointVertexIndex + i + 1) % 6].position, c, normal, .5f));

                        triangleList.Add(GenerateVertex(points[jointVertexIndex].position, c, normal, -.5f));
                        triangleList.Add(GenerateVertex(points[(jointVertexIndex + i) % 6].position, c, normal, -.5f));
                        triangleList.Add(GenerateVertex(points[(jointVertexIndex + i + 1) % 6].position, c, normal, -.5f));
                    }
                }
            }

            if (effect == null)
            {
                effect = new BasicEffect(Game1.graphicsDevice);
                effect.VertexColorEnabled = true;
            }
            
            Game1.graphicsDevice.Clear(Color.Black);
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //RenderTarget2D target = new RenderTarget2D(Game1.graphicsDevice, Game1.graphicsDevice.PresentationParameters.BackBufferWidth, Game1.graphicsDevice.PresentationParameters.BackBufferHeight);
            //Game1.graphicsDevice.SetRenderTarget(target);            

            // Set transform matrices.
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            //effect.World = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            effect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);

            effect.View = Matrix.CreateLookAt(currentCamera,
                                              currentTarget,
                                              currentUp);

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);
            effect.LightingEnabled = true;
            effect.Alpha = 1f;
            effect.SpecularPower = 0.1f;
            effect.AmbientLightColor = new Vector3(.7f, .7f, .7f);
            effect.DiffuseColor = new Vector3(1, 1, 1);
            effect.SpecularColor = new Vector3(0, 1f, 1f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-4, -1, -1));
            effect.DirectionalLight0.DiffuseColor = Color.Gray.ToVector3();
            effect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();            


            // Set renderstates.
            Game1.graphicsDevice.RasterizerState = RasterizerState.CullNone;            

            // Draw the triangle.
            effect.CurrentTechnique.Passes[0].Apply();            

            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleList.ToArray(), 0, triangleList.Count / 3, VertexPositionColorNormal.VertexDeclaration);            
        }

    }
}

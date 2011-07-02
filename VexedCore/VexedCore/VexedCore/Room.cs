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
            if (relative.X > size.X / 2)
            {
                Vector3 oldNormal = v.normal;
                v.normal = new Vector3(1, 0, 0);
                float overFlow = relative.X - size.X / 2;
                v.position.X -= overFlow;
                v.position -= overFlow * oldNormal;
                v.velocity = -v.velocity.Length() * oldNormal;
            }
            if (relative.X < -size.X / 2)
            {
                Vector3 oldNormal = v.normal;
                v.normal = new Vector3(-1, 0, 0);
                float overFlow = Math.Abs(relative.X) - size.X / 2;
                v.position.X += overFlow;
                v.position -= overFlow * oldNormal;
                v.velocity = -v.velocity.Length() * oldNormal;
            }
            if (relative.Y > size.Y / 2)
            {
                Vector3 oldNormal = v.normal;
                v.normal = new Vector3(0, 1, 0);
                float overFlow = relative.Y - size.Y / 2;
                v.position.Y -= overFlow;
                v.position -= overFlow * oldNormal;
                v.velocity = - v.velocity.Length() * oldNormal;
            }
            if(relative.Y < -size.Y / 2)
            {
                Vector3 oldNormal = v.normal;
                v.normal = new Vector3(0, -1, 0);
                float overFlow = Math.Abs(relative.Y) - size.Y / 2;
                v.position.Y += overFlow;
                v.position -= overFlow * oldNormal;
                v.velocity = -v.velocity.Length() * oldNormal;
            }
            if(relative.Z > size.Z / 2)
            {
                Vector3 oldNormal = v.normal;
                v.normal = new Vector3(0, 0, 1);
                float overFlow = relative.Z - size.Z / 2;
                v.position.Z -= overFlow;
                v.position -= overFlow * oldNormal;
                v.velocity = -v.velocity.Length() * oldNormal;
            }
            if (relative.Z < -size.Z / 2)
            {
                Vector3 oldNormal = v.normal;
                v.normal = new Vector3(0, 0, -1);
                float overFlow = Math.Abs(relative.Z) - size.Z / 2;
                v.position.Z += overFlow;
                v.position -= overFlow * oldNormal;
                v.velocity = -v.velocity.Length() * oldNormal;
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

        public void Draw(GameTime gameTime)
        {
            Vector3 currentTarget = Vector3.Zero;
            Vector3 currentCamera = new Vector3(30, 30, 30);
            Vector3 currentUp = new Vector3(0, 0, 1);
            float currentRotate = 0;
            float currentPitch = 0;
            List<VertexPositionColor> outlineList = new List<VertexPositionColor>();
            foreach (Block b in blocks)
            {
                foreach (Edge e in b.edges)
                {
                    if (e.start.normal == e.end.normal)
                    {
                        outlineList.Add(new VertexPositionColor(e.start.position, Color.Black));
                        outlineList.Add(new VertexPositionColor(e.end.position, Color.Black));
                    }
                    else
                    {
                        Vector3 velDir = e.start.velocity;
                        velDir.Normalize();
                        Vector3 midPoint = e.start.position + Vector3.Dot(velDir, e.end.position - e.start.position)*velDir;
                        outlineList.Add(new VertexPositionColor(e.start.position, Color.Black));
                        outlineList.Add(new VertexPositionColor(midPoint, Color.Black));
                        outlineList.Add(new VertexPositionColor(midPoint, Color.Black));
                        outlineList.Add(new VertexPositionColor(e.end.position, Color.Black));
                    }
                }
            }
            
            if(effect == null) effect = new BasicEffect(Game1.graphicsDevice);
            
            Game1.graphicsDevice.Clear(Color.Black);
            
            // Set transform matrices.
            float aspect = Game1.graphicsDevice.Viewport.AspectRatio;

            //effect.World = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            effect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);

            effect.View = Matrix.CreateLookAt(currentCamera,
                                              currentTarget,
                                              currentUp);

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);

            // Set renderstates.
            Game1.graphicsDevice.RasterizerState = RasterizerState.CullNone;

            // Draw the triangle.
            effect.CurrentTechnique.Passes[0].Apply();

            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                outlineList.ToArray(), 0, outlineList.Count / 2);
        }

    }
}

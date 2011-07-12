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
    public class Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 direction;
        public Vector3 velocity;

        public Vertex()
        {
            position = Vector3.Zero;
            normal = Vector3.Zero;
            direction = Vector3.Zero;
            velocity = Vector3.Zero;
        }

        public Vertex(Vertex v, Vector3 positionOffset)
        {
            position = v.position + positionOffset;
            normal = v.normal;
            direction = v.direction;
            velocity = v.velocity;
        }

        public Vertex(Vector3 pos, Vector3 n, Vector3 vel, Vector3 dir)
        {
            position = pos;
            normal = n;
            velocity = vel;
            direction = dir;
        }

        public Vertex Unfold(Room r, Vector3 n, Vector3 u)
        {
            Vector3 anchor = r.center + Math.Abs(Vector3.Dot(r.size / 2, n)) * n;
            Vertex v = new Vertex();
            if (normal == n)
            {
                v.normal = normal;
                v.position = position;
                v.velocity = velocity;
            }
            else if (Vector3.Dot(normal, n) == 0)
            {
                Vector3 badComponent = Vector3.Dot(n, position - anchor) * n;
                Vector3 badVelComponent = Vector3.Dot(n, velocity) * n;
                float badVelLength = Vector3.Dot(n, velocity);
                v.position = position - badComponent + badComponent.Length() * normal;
                v.velocity = velocity - badVelComponent - badVelLength * normal;
                v.normal = n;
            }
            else
            {
                Vector3 upAnchor = r.center + Math.Abs(Vector3.Dot(r.size / 2, u)) * u;
                Vector3 badComponent = Vector3.Dot(u, position - upAnchor) * u;
                Vector3 gapComponent = Vector3.Dot(r.size, n) * n;
                v.position = position - 2 * badComponent + gapComponent.Length() * n + gapComponent.Length() * u;
                v.normal = n;
            }
            return v;
        }

        public void Update(Room r, int updateTime)
        {
            position += updateTime * velocity;
            Vector3 relative = position - r.center;
            float overFlow = 0;
            Vector3 oldNormal = normal;
            if (relative.X > r.size.X / 2)
            {
                normal = new Vector3(1, 0, 0);
                overFlow = relative.X - r.size.X / 2;
                position.X -= overFlow;
            }
            if (relative.X < -r.size.X / 2)
            {
                normal = new Vector3(-1, 0, 0);
                overFlow = Math.Abs(relative.X) - r.size.X / 2;
                position.X += overFlow;
            }
            if (relative.Y > r.size.Y / 2)
            {
                normal = new Vector3(0, 1, 0);
                overFlow = relative.Y - r.size.Y / 2;
                position.Y -= overFlow;
            }
            if (relative.Y < -r.size.Y / 2)
            {
                normal = new Vector3(0, -1, 0);
                overFlow = Math.Abs(relative.Y) - r.size.Y / 2;
                position.Y += overFlow;
            }
            if (relative.Z > r.size.Z / 2)
            {
                normal = new Vector3(0, 0, 1);
                overFlow = relative.Z - r.size.Z / 2;
                position.Z -= overFlow;
            }
            if (relative.Z < -r.size.Z / 2)
            {
                normal = new Vector3(0, 0, -1);
                overFlow = Math.Abs(relative.Z) - r.size.Z / 2;
                position.Z += overFlow;
            }
            if (overFlow > 0)
            {
                position -= overFlow * oldNormal;
                Vector3 oldVelocity = Vector3.Dot(normal, velocity) * normal;
                Vector3 newVelocity = -oldVelocity.Length() * oldNormal;
                velocity = velocity - oldVelocity + newVelocity;

                float oldDirectionSign = Vector3.Dot(normal, direction);
                Vector3 oldDirection = oldDirectionSign * normal;
                Vector3 newDirection = -oldDirection.Length() * oldDirectionSign * oldNormal;
                
                direction = direction - oldDirection + newDirection;
            }
        }
    }
}

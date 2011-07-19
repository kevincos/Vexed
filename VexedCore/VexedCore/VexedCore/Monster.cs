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
    public class Monster
    {
        public static Texture2D monsterTexture;
        public Vertex position;
        public string firstWaypoint;
        public List<Vector3> waypoints;
        public int currentWaypointIndex = 0;
        public int wayPointDirection = 1;
        public bool waypointLoop = false;
        public VexedLib.AIType aiType;
        public VexedLib.MovementType moveType;
        public VexedLib.ArmorType armorType;
        public VexedLib.GunType gunType;

        public Monster srcMonster;

        public Monster(VexedLib.Monster xmlMonster, Vector3 normal)
        {
            this.position = new Vertex(xmlMonster.position, normal, Vector3.Zero, xmlMonster.up);
            firstWaypoint = xmlMonster.waypointId;
            waypoints = new List<Vector3>();
            aiType = xmlMonster.behavior;
            moveType = xmlMonster.movement;
            armorType = xmlMonster.armor;
            gunType = xmlMonster.weapon;
        }

        public Monster(Monster m, Room r, Vector3 n, Vector3 u)
        {
            position = m.position.Unfold(r,n,u);
            srcMonster = m;
        }

        public float acceleration
        {
            get
            {
                return .0005f;
            }
        }

        public float maxSpeed
        {
            get
            {
                return .008f;
            }
        }

        public Vector3 upUnit
        {
            get
            {
                return position.direction;
            }
        }
        public Vector3 rightUnit
        {
            get
            {
                return Vector3.Cross(position.direction, position.normal);
            }
        }
        public Vector3 right
        {
            get
            {
                return halfWidth * rightUnit;
            }
        }
        public Vector3 left
        {
            get
            {
                return -halfWidth * rightUnit;
            }
        }
        public Vector3 up
        {
            get
            {
                return halfHeight * upUnit;
            }
        }
        public Vector3 down
        {
            get
            {
                return -halfHeight * upUnit;
            }
        }

        public float halfWidth
        {
            get
            {
                return .5f;
            }
        }
        public float halfHeight
        {
            get
            {
                return .5f;
            }
        }
        public float depth
        {
            get
            {
                return .5f;
            }
        }

        public void Update(GameTime gameTime)
        {
            position.Update(Game1.player.currentRoom, gameTime.ElapsedGameTime.Milliseconds);
            Vector3 direction = Vector3.Zero;
            if (aiType == VexedLib.AIType.Waypoint)
            {
                Vector3 target = waypoints[currentWaypointIndex];
                direction = target - position.position;
                if (direction.Length() < .1f)
                {
                    currentWaypointIndex+=wayPointDirection;
                    if (waypointLoop)
                    {
                        if (currentWaypointIndex == waypoints.Count())
                        {
                            wayPointDirection = -wayPointDirection;
                            currentWaypointIndex += 2 * wayPointDirection;
                        }
                    }
                    else
                    {
                        currentWaypointIndex %= waypoints.Count();
                    }
                }
                else
                {
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                    if (direction.Length() > 1)
                        direction.Normalize();                    
                }                
            }
            if (aiType == VexedLib.AIType.Hunter)
            {
                Vector3 target = Game1.player.center.position;
                direction = target - position.position;
                if (position.normal == -Game1.player.center.normal)
                {
                    direction *= -1;
                }
                direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                if (direction.Length() > 1)
                    direction.Normalize();   
            }
            if (aiType == VexedLib.AIType.Wander)
            {
                direction = -rightUnit;
            }
            position.velocity += acceleration * direction;
            if (position.velocity.Length() > maxSpeed)
            {
                position.velocity.Normalize();
                position.velocity *= maxSpeed;
            }
        }

        public List<Vector3> GetCollisionRect()
        {
            List<Vector3> doodadVertexList = new List<Vector3>();

            doodadVertexList.Add(position.position + up + right);
            doodadVertexList.Add(position.position + up + left);
            doodadVertexList.Add(position.position + down + left);
            doodadVertexList.Add(position.position + down + right);

            return doodadVertexList;
        }

        public bool CollisionFirstPass(Block b)
        {
            return (boundingBoxBottom > b.boundingBoxTop ||
                        boundingBoxTop < b.boundingBoxBottom ||
                        boundingBoxLeft > b.boundingBoxRight ||
                        boundingBoxRight < b.boundingBoxLeft);
        }

        public bool CollisionFirstPass(Doodad d)
        {
            return (boundingBoxBottom > d.boundingBoxTop ||
                        boundingBoxTop < d.boundingBoxBottom ||
                        boundingBoxLeft > d.boundingBoxRight ||
                        boundingBoxRight < d.boundingBoxLeft);
        }


        public float boundingBoxTop;
        public float boundingBoxBottom;
        public float boundingBoxLeft;
        public float boundingBoxRight;

        public void UpdateBoundingBox(Vector3 playerUp, Vector3 playerRight)
        {
            float x1, x2, x3, x4 = 0;
            float y1, y2, y3, y4 = 0;
            x1 = Vector3.Dot(playerRight, position.position + up);
            x2 = Vector3.Dot(playerRight, position.position + down);
            x3 = Vector3.Dot(playerRight, position.position + left);
            x4 = Vector3.Dot(playerRight, position.position + right);
            y1 = Vector3.Dot(playerUp, position.position + up);
            y2 = Vector3.Dot(playerUp, position.position + down);
            y3 = Vector3.Dot(playerUp, position.position + left);
            y4 = Vector3.Dot(playerUp, position.position + right);
            boundingBoxLeft = x1;
            if (x2 < boundingBoxLeft)
                boundingBoxLeft = x2;
            if (x3 < boundingBoxLeft)
                boundingBoxLeft = x3;
            if (x4 < boundingBoxLeft)
                boundingBoxLeft = x4;
            boundingBoxRight = x1;
            if (x2 > boundingBoxRight)
                boundingBoxRight = x2;
            if (x3 > boundingBoxRight)
                boundingBoxRight = x3;
            if (x4 > boundingBoxRight)
                boundingBoxRight = x4;
            boundingBoxTop = y1;
            if (y2 > boundingBoxTop)
                boundingBoxTop = y2;
            if (y3 > boundingBoxTop)
                boundingBoxTop = y3;
            if (y4 > boundingBoxTop)
                boundingBoxTop = y4;
            boundingBoxBottom = y1;
            if (y2 < boundingBoxBottom)
                boundingBoxBottom = y2;
            if (y3 < boundingBoxBottom)
                boundingBoxBottom = y3;
            if (y4 < boundingBoxBottom)
                boundingBoxBottom = y4;
        }

        public void AdjustVertex(Vector3 pos, Vector3 vel, Vector3 normal, Vector3 playerUp)
        {
            Vector3 playerRight = Vector3.Cross(playerUp, normal);
            if (position.normal == normal)
            {
                position.position += pos;
                position.velocity += vel;
            }
            else if (position.normal == -normal)
            {
                float badVelComponent = Vector3.Dot(playerUp, vel);
                float badPosComponent = Vector3.Dot(playerUp, pos);
                position.position += pos - 2 * badPosComponent * playerUp;
                position.velocity += vel - 2 * badVelComponent * playerUp;
            }
            else if (position.normal == playerUp)
            {
                float badVelComponent = Vector3.Dot(playerUp, vel);
                float badPosComponent = Vector3.Dot(playerUp, pos);
                position.position += pos - badPosComponent * playerUp + badPosComponent * -normal;
                position.velocity += vel - badVelComponent * playerUp + badVelComponent * -normal;
            }
            else if (position.normal == -playerUp)
            {
                float badVelComponent = Vector3.Dot(playerUp, vel);
                float badPosComponent = Vector3.Dot(playerUp, pos);
                position.position += pos - badPosComponent * playerUp + badPosComponent * normal;
                position.velocity += vel - badVelComponent * playerUp + badVelComponent * normal;
            }
            else if (position.normal == playerRight)
            {
                float badVelComponent = Vector3.Dot(playerRight, vel);
                float badPosComponent = Vector3.Dot(playerRight, pos);
                position.position += pos - badPosComponent * playerRight + badPosComponent * -normal;
                position.velocity += vel - badVelComponent * playerRight + badVelComponent * -normal;
            }
            else if (position.normal == -playerRight)
            {
                float badVelComponent = Vector3.Dot(playerRight, vel);
                float badPosComponent = Vector3.Dot(playerRight, pos);
                position.position += pos - badPosComponent * playerRight + badPosComponent * normal;
                position.velocity += vel - badVelComponent * playerRight + badVelComponent * normal;
            }
            else
            {
                position.velocity = Vector3.Zero;
            }
        }

        public void Draw(Room r)
        {
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<VertexPositionColorNormalTexture> textureTriangleList = new List<VertexPositionColorNormalTexture>();
            List<Vertex> rectVertexList = new List<Vertex>();
    
            rectVertexList.Add(new Vertex(position.position, position.normal, up + right, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, up +left, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, down +left, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, down + right, position.direction));
            foreach (Vertex v in rectVertexList)
            {
                v.Update(Game1.player.currentRoom, 1);
            }
            List<Vector2> texCoords = new List<Vector2>();
            texCoords.Add(new Vector2(0, 0));
            texCoords.Add(new Vector2(1, 0));
            texCoords.Add(new Vector2(1, 1));
            texCoords.Add(new Vector2(0, 1));
            r.AddTextureToTriangleList(rectVertexList, Color.White, .3f, textureTriangleList, texCoords);


            VertexPositionColorNormalTexture[] triangleArray = textureTriangleList.ToArray();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
        }
    }
}

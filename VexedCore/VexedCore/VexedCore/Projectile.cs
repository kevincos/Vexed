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
    public enum ProjectileType
    {
        Plasma,
        Laser,
        Missile,
        Player
    }

    public class Projectile
    {
        public Vertex position;
        public Projectile srcProjectile;
        public ProjectileType type;


        public static Texture2D projectileTexture;

        public int lifeTime = 0;
        
        public bool exploded = false;

        public static List<Vector2> plasmaTexCoords;
        public static List<Vector2> missileTexCoords;
        public static List<Vector2> laserTexCoords;
        public static List<Vector2> blastTexCoords;

        public bool exploding = false;
        public int explodeTime = 0;
        

        public static int texGridCount = 8;

        public static List<Vector2> LoadTexCoords(int x, int y)
        {
            float texWidth = 1f / texGridCount;
            List<Vector2> texCoords = new List<Vector2>();
            texCoords.Add(new Vector2((x + 1) * texWidth, y * texWidth));
            texCoords.Add(new Vector2(x * texWidth, y * texWidth));
            texCoords.Add(new Vector2(x * texWidth, (y + 1) * texWidth));
            texCoords.Add(new Vector2((x + 1) * texWidth, (y + 1) * texWidth));

            return texCoords;
        }

        public int maxExplodeTime
        {
            get
            {
                if (type == ProjectileType.Player)
                    return 100;
                else if (type == ProjectileType.Plasma)
                    return 100;
                else if (type == ProjectileType.Missile)
                    return 2000;
                else if (type == ProjectileType.Laser)
                    return 50;
                else
                    return 0;
            }
        }

        public int maxLife
        {
            get
            {
                if (type == ProjectileType.Player)
                    return 1500;
                else if (type == ProjectileType.Plasma)
                    return 1500;
                else if (type == ProjectileType.Missile)
                    return 6000;
                else if (type == ProjectileType.Laser)
                    return 500;
                else
                    return 0;
            }
        }

        public float velocity
        {
            get
            {
                if (type == ProjectileType.Plasma)
                    return .008f;
                else if (type == ProjectileType.Player)
                    return .025f;
                else if (type == ProjectileType.Missile)
                    return .004f;
                else if (type == ProjectileType.Laser)
                    return .025f;
                else
                    return 0;
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
                if (type == ProjectileType.Missile || type == ProjectileType.Laser)
                {
                    return .3f;
                }
                if (exploding == true)
                {
                    return .2f + explodeTime * .2f / maxExplodeTime;
                }
                else
                {
                    return .2f;
                }
            }
        }
        public float halfHeight
        {
            get
            {
                if (type == ProjectileType.Laser)
                {
                    if (exploding == true)
                    {
                        return (maxExplodeTime - explodeTime) * 1.5f / maxExplodeTime;
                    }
                    else
                    {
                        return 1.5f;
                    }
                }
                if (type == ProjectileType.Missile)
                {
                    return .5f;
                }

                if (exploding == true)
                {
                    return .2f + explodeTime * .2f / maxExplodeTime;
                }
                else
                {
                    return .2f;
                }
            }
        }
        public float depth
        {
            get
            {
                return 0f;
            }
        }


        public static void InitTexCoords()
        {
            plasmaTexCoords = LoadTexCoords(0, 0);
            missileTexCoords = LoadTexCoords(0, 1);
            laserTexCoords = LoadTexCoords(0, 2);
            blastTexCoords = LoadTexCoords(1, 0);
        }

        public Projectile(ProjectileType type, Vector3 position, Vector3 velocity, Vector3 normal, Vector3 direction)
        {
            this.type = type;
            this.position = new Vertex(position, normal, Vector3.Zero, direction);
            Vector3 extraVelocity = direction;
            extraVelocity.Normalize();
            this.position.velocity += extraVelocity * this.velocity;
            
        }

        public Projectile(Projectile p, Room r, Vector3 n, Vector3 u)
        {
            position = p.position.Unfold(r,n,u);
            srcProjectile = p;
        }

        public void Update(GameTime gameTime)
        {
            if (exploding == true)
            {
                explodeTime += gameTime.ElapsedGameTime.Milliseconds;
                if (explodeTime > maxExplodeTime)
                    exploded = true;
            }
            lifeTime += gameTime.ElapsedGameTime.Milliseconds;
            if (lifeTime > maxLife)
            {
                exploding = true;
                position.velocity = Vector3.Zero;
            }
            if (type == ProjectileType.Missile)
            {
                Vector3 targetDirection = Game1.player.center.position - position.position;
                targetDirection = targetDirection - Vector3.Dot(targetDirection, position.normal) * position.normal;
                targetDirection.Normalize();
                
                position.velocity += .06f*this.velocity * targetDirection;
                if (position.velocity.Length() > this.velocity)
                {
                    position.velocity.Normalize();
                    position.velocity *= this.velocity;
                }

                position.direction = targetDirection/targetDirection.Length();
            }
            position.Update(Game1.player.currentRoom, gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(Room r)
        {
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<VertexPositionColorNormalTexture> textureTriangleList = new List<VertexPositionColorNormalTexture>();
            List<Vertex> rectVertexList = new List<Vertex>();

            rectVertexList.Add(new Vertex(position.position, position.normal, up + right, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, up + left, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, down + left, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, down + right, position.direction));

            foreach (Vertex v in rectVertexList)
            {
                v.Update(Game1.player.currentRoom, 1);
            }

            if (type == ProjectileType.Plasma || type == ProjectileType.Player)
            {
                if (exploding == true)
                {
                    r.AddTextureToTriangleList(rectVertexList, Color.GreenYellow, depth - .05f, textureTriangleList, blastTexCoords, true);
                }
                else
                {
                    r.AddTextureToTriangleList(rectVertexList, Color.GreenYellow, depth - .05f, textureTriangleList, plasmaTexCoords, true);
                }
            }
            if (type == ProjectileType.Missile)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - .05f, textureTriangleList, missileTexCoords, true);
            if (type == ProjectileType.Laser)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - .05f, textureTriangleList, laserTexCoords, true);

            VertexPositionColorNormalTexture[] triangleArray = textureTriangleList.ToArray();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
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


    }
}
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
using System.Xml.Serialization;

namespace VexedCore
{
    public class Monster
    {
        public static Texture2D monsterTexture;
        public static Texture2D monsterTextureDetail;

        public static List<Vector2> bodyTexCoords;
        public static List<Vector2> eyesTexCoords;
        public static List<Vector2> fullArmorTexCoords;
        public static List<Vector2> topArmorTexCoords;
        public static List<Vector2> frontArmorTexCoords;
        public static List<Vector2> treadsTexCoords;
        public static List<Vector2> spiderTexCoords;
        public static List<Vector2> legsTexCoords;
        public static List<Vector2> gunTexCoords;

        public static int texGridCount = 8;

        public Vertex spawnPosition;
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
        public Vector3 groundProjection;
        public Vector3 forwardGroundProjection;
        public bool jumping = false;
        public int jumpCooldown = 0;
        public int jumpTime = 0;
        public Vector3 spinUp;
        public int spinMaxTime = 400;
        public int spinTime = 0;
        public bool rightFacing = false;
        public bool rightMoving = false;
        [XmlIgnore]public Monster srcMonster;
        
        public int directionChangeCooldown = 0;        
        public float currentAngle = .5f;
        public float angleRotateSpeed = .05f;
        public int fireCooldown = 0;
        public string id;
        public bool dead;
        public Vector3 impactVector = Vector3.Zero;

        public int armorHP = 3;
        public int baseHP = 5;
        public ProjectileType lastHitType;

        public Vector3 gunLine = Vector3.Zero;
        public Vector3 gunNormal = Vector3.Zero;

        public Monster(Monster m)
        {
            spawnPosition = new Vertex(m.spawnPosition);
            position = new Vertex(m.position);
            firstWaypoint = m.firstWaypoint;
            waypoints = new List<Vector3>();
            foreach (Vector3 v in m.waypoints)
                waypoints.Add(v);
            currentWaypointIndex = m.currentWaypointIndex;
            wayPointDirection = m.wayPointDirection;
            waypointLoop = m.waypointLoop;
            aiType = m.aiType;
            moveType = m.moveType;
            armorType = m.armorType;
            gunType = m.gunType;
            groundProjection = m.groundProjection;
            forwardGroundProjection = m.forwardGroundProjection;
            jumping = m.jumping;
            jumpCooldown = m.jumpCooldown;
            spinUp = m.spinUp;
            spinMaxTime = m.spinMaxTime;
            spinTime = m.spinTime;
            rightFacing = m.rightFacing;
            rightMoving = m.rightMoving;
            directionChangeCooldown = m.directionChangeCooldown;
            currentAngle = m.currentAngle;
            angleRotateSpeed = m.angleRotateSpeed;
            fireCooldown = m.fireCooldown;
            id = m.id;
            dead = m.dead;
            impactVector = m.impactVector;
            gunLine = m.gunLine;
            gunNormal = m.gunNormal;
            armorHP = m.armorHP;
            baseHP = m.baseHP;
        }
        
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

        public static void InitTexCoords()
        {
            bodyTexCoords = LoadTexCoords(0, 0);
            fullArmorTexCoords = LoadTexCoords(1, 0);
            topArmorTexCoords = LoadTexCoords(2, 0);
            frontArmorTexCoords = LoadTexCoords(3, 0);
            eyesTexCoords = LoadTexCoords(4, 0);
            gunTexCoords = LoadTexCoords(5, 0);
            treadsTexCoords = LoadTexCoords(0, 2);
            spiderTexCoords = LoadTexCoords(0, 1);
            legsTexCoords = LoadTexCoords(0, 3);            
        }

        public Monster()
        {
        }

        public Monster(VexedLib.Monster xmlMonster, Vector3 normal)
        {
            this.spawnPosition = new Vertex(xmlMonster.position, normal, Vector3.Zero, xmlMonster.up);
            this.position = new Vertex(xmlMonster.position, normal, Vector3.Zero, xmlMonster.up);
            firstWaypoint = xmlMonster.waypointId;
            waypoints = new List<Vector3>();
            aiType = xmlMonster.behavior;
            moveType = xmlMonster.movement;
            armorType = xmlMonster.armor;
            gunType = xmlMonster.weapon;
            id = xmlMonster.IDString;
        }

        public Monster(Monster m, Room r, Vector3 n, Vector3 u)
        {
            position = m.position.Unfold(r,n,u);
            srcMonster = m;
            rightFacing = m.rightFacing;
            rightMoving = m.rightMoving;
        }

        public int fireTime
        {
            get
            {
                if(gunType == VexedLib.GunType.Blaster || gunType == VexedLib.GunType.Spread)
                    return 1000;
                if (gunType == VexedLib.GunType.Missile)
                    return 4000;
                if (gunType == VexedLib.GunType.Beam)
                    return 2000;
                if (gunType == VexedLib.GunType.Repeater)
                    return 200;
                return 0;
            }
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
                if (spinUp == Vector3.Zero)
                    return position.direction;
                else
                {
                    Vector3 tempUp = (spinUp * spinTime + position.direction * (spinMaxTime - spinTime)) / (1f * spinMaxTime);
                    tempUp.Normalize();
                    return tempUp;
                }
            }
        }
        public Vector3 rightUnit
        {
            get
            {
                if (spinUp == Vector3.Zero)
                    return Vector3.Cross(position.direction, position.normal);
                else
                {
                    Vector3 tempUp = Vector3.Cross((spinUp * spinTime + position.direction * (spinMaxTime - spinTime)) / (1f * spinMaxTime), position.normal);
                    tempUp.Normalize();
                    return tempUp;
                }
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
                return .6f;
            }
        }
        public float halfHeight
        {
            get
            {
                return .6f;
            }
        }
        public float depth
        {
            get
            {
                return 0f;
            }
        }

        public void ApplyDamage(bool armor, ProjectileType gunType)
        {
            if (armor == false)
            {
                baseHP--;
            }
            else if (gunType == ProjectileType.Laser)
            {
                armorHP--;
                if (armorHP == 0)
                {
                    armorType = VexedLib.ArmorType.None;
                }
            }
            if(baseHP == 0)
                dead = true;
        }

        public void Update(GameTime gameTime)
        {
            directionChangeCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (directionChangeCooldown < 0)
                directionChangeCooldown = 0;
            fireCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (fireCooldown < 0)
                fireCooldown = 0;


            if (impactVector != Vector3.Zero)
            {
                impactVector.Normalize();
                bool armorBlock = true;
                if(armorType == VexedLib.ArmorType.None)
                    armorBlock = false;
                else if (armorType == VexedLib.ArmorType.Top && Vector3.Dot(impactVector, position.direction) > .5f)
                    armorBlock = false;
                else if (rightFacing == true && armorType == VexedLib.ArmorType.Shield && Vector3.Dot(impactVector, rightUnit) > 0)
                    armorBlock = false;
                else if (rightFacing == false && armorType == VexedLib.ArmorType.Shield && Vector3.Dot(impactVector, -rightUnit) > 0)
                    armorBlock = false;

                ApplyDamage(armorBlock, lastHitType);

                impactVector = Vector3.Zero;                
            }


            position.Update(Engine.player.currentRoom, gameTime.ElapsedGameTime.Milliseconds);
            
            Vector3 direction = Vector3.Zero;
            if (aiType == VexedLib.AIType.Waypoint)
            {
                Vector3 target = waypoints[currentWaypointIndex];
                direction = target - position.position;
                if (direction.Length() < .5f)
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
                Vector3 target = Engine.player.center.position;
                direction = target - position.position;
                if (position.normal == -Engine.player.center.normal)
                {
                    direction *= -1;
                }
                if (moveType != VexedLib.MovementType.Jump && direction.Length() < 3.5f)
                {
                    directionChangeCooldown = 300;
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

            Vector3 aimTarget = Engine.player.center.position - position.position;
            aimTarget.Normalize();
            float cosTheta = Vector3.Dot(upUnit, aimTarget);
            float sinTheta = Vector3.Dot(rightUnit, aimTarget);
            float targetAngle = 0;
            if(sinTheta > 0)
                targetAngle = (float)Math.Acos(cosTheta);
            else
                targetAngle = (float)(2 * Math.PI) - (float)Math.Acos(cosTheta);
            float posGap = targetAngle - currentAngle;
            if (posGap < 0)
                posGap += (float)Math.PI * 2;
            if (posGap < .1f)
            {
            }
            else if (posGap < Math.PI) currentAngle += angleRotateSpeed;
            else if (posGap > Math.PI) currentAngle -= angleRotateSpeed;

            if (currentAngle > 2*Math.PI)
                currentAngle -= (float)Math.PI * 2;
            if (currentAngle < 0)
                currentAngle += (float)Math.PI * 2;

            if (moveType != VexedLib.MovementType.Hover)
            {
                if (currentAngle > Math.PI / 2 && currentAngle < Math.PI)
                    currentAngle = (float)(Math.PI / 2 - .05f);
                if (currentAngle > Math.PI && currentAngle < 3 * Math.PI / 2)
                    currentAngle = (float)(3 * Math.PI / 2 + .05f);
            }

            cosTheta = (float)Math.Cos(currentAngle);
            sinTheta = (float)Math.Sin(currentAngle);
            gunLine = right * sinTheta + up * cosTheta;
            gunNormal = Vector3.Cross(gunLine, position.normal);
            gunLine.Normalize();
            gunNormal.Normalize();

            if (fireCooldown == 0)
            {
                Vector3 projectileVelocity = gunLine;
                projectileVelocity.Normalize();
                
                fireCooldown = fireTime;
                if (gunType == VexedLib.GunType.Blaster || gunType == VexedLib.GunType.Repeater)
                {
                    Engine.player.currentRoom.projectiles.Add(new Projectile(this, ProjectileType.Plasma, position.position + gunLine, Vector3.Zero, position.normal, projectileVelocity));
                }
                if (gunType == VexedLib.GunType.Beam)
                {
                    Engine.player.currentRoom.projectiles.Add(new Projectile(this, ProjectileType.Laser, position.position + gunLine, Vector3.Zero, position.normal, projectileVelocity));
                }
                if (gunType == VexedLib.GunType.Missile)
                {
                    Engine.player.currentRoom.projectiles.Add(new Projectile(this, ProjectileType.Missile, position.position + gunLine, position.velocity, position.normal, projectileVelocity));
                }
                if (gunType == VexedLib.GunType.Spread)
                {
                    Engine.player.currentRoom.projectiles.Add(new Projectile(this, ProjectileType.Plasma, position.position + gunLine, Vector3.Zero, position.normal, projectileVelocity + .5f * gunNormal));
                    Engine.player.currentRoom.projectiles.Add(new Projectile(this, ProjectileType.Plasma, position.position + gunLine, Vector3.Zero, position.normal, projectileVelocity - .5f * gunNormal));
                    Engine.player.currentRoom.projectiles.Add(new Projectile(this, ProjectileType.Plasma, position.position + gunLine, Vector3.Zero, position.normal, projectileVelocity));
                }

            }

            if (moveType == VexedLib.MovementType.Tank)
            {

                if (groundProjection != Vector3.Zero)
                {
                    Vector3 groundDirection = groundProjection / groundProjection.Length();
                    if (Vector3.Dot(groundDirection, position.direction) == 1f)
                    {
                        direction -= Vector3.Dot(groundDirection, direction) * groundDirection;

                        position.velocity += acceleration * direction;
                    }
                }

                if (directionChangeCooldown == 0)
                {
                    if (Vector3.Dot(rightUnit, position.velocity) > 0)
                        rightFacing = true;
                    if (Vector3.Dot(rightUnit, position.velocity) < 0)
                        rightFacing = false;
                }
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightMoving = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightMoving = false;


                AdjustVertex(Vector3.Zero, -1.5f * acceleration * Engine.player.center.direction, Engine.player.center.normal, Engine.player.center.direction, false);
                if (forwardGroundProjection == Vector3.Zero && Vector3.Dot(groundProjection, Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false))>0f)
                {
                    position.velocity = Vector3.Zero;
                }
                if (position.velocity.Length() > maxSpeed)
                {
                    position.velocity.Normalize();
                    position.velocity *= maxSpeed;
                }
            }
            else if (moveType == VexedLib.MovementType.Spider)
            {                
                if (groundProjection != Vector3.Zero)
                {
                    position.velocity += acceleration * direction;
                    Vector3 groundDirection = groundProjection / groundProjection.Length();
                    position.velocity -= Vector3.Dot(groundDirection, position.velocity) * groundDirection;
                }
                else
                {
                    AdjustVertex(Vector3.Zero, -1.5f * acceleration * Engine.player.center.direction, Engine.player.center.normal, Engine.player.center.direction, false);
                }

                if (directionChangeCooldown == 0)
                {
                    if (Vector3.Dot(rightUnit, position.velocity) > 0)
                        rightFacing = true;
                    if (Vector3.Dot(rightUnit, position.velocity) < 0)
                        rightFacing = false;
                }
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightMoving = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightMoving = false;

                if (forwardGroundProjection == Vector3.Zero && groundProjection != Vector3.Zero)
                {
                    position.velocity = Vector3.Zero;
                }
                if (position.velocity.Length() > maxSpeed)
                {
                    position.velocity.Normalize();
                    position.velocity *= maxSpeed;
                }
            }
            else if (moveType == VexedLib.MovementType.Jump)
            {
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightFacing = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightFacing = false;
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightMoving = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightMoving = false;

                if (spinUp != Vector3.Zero)
                    groundProjection = Vector3.Zero;
                if (position.direction != Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false))
                {
                    spinUp = Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false);
                }
                if (spinUp != Vector3.Zero)
                {
                    spinTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (spinTime > spinMaxTime)
                    {
                        spinTime = 0;
                        position.direction = spinUp;
                        spinUp = Vector3.Zero;
                    }
                }

                if (groundProjection != Vector3.Zero)
                {
                    jumpCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                    if (jumpCooldown < 0) jumpCooldown = 0;

                    if (jumpTime > 10)
                    {
                        jumpTime = 0;
                        jumping = false;
                    }
                    if (jumping == true)
                        jumpTime += gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    AdjustVertex(Vector3.Zero, -1.5f * acceleration * Engine.player.center.direction, Engine.player.center.normal, Engine.player.center.direction, false);
                }
                if (groundProjection != Vector3.Zero && jumping == false)
                {
                    position.velocity = Vector3.Zero;
                }
                if (groundProjection != Vector3.Zero && jumpCooldown == 0)
                {
                    jumpCooldown = 500;
                    jumping = true;
                    position.velocity += 30 * acceleration * position.direction;
                    position.velocity += 30 * acceleration * direction;
                }

                if (jumping == false)
                {
                    if (position.velocity.Length() > maxSpeed)
                    {
                        position.velocity.Normalize();
                        position.velocity *= maxSpeed;
                    }
                }
                else
                {
                    if (position.velocity.Length() > 2*maxSpeed)
                    {
                        position.velocity.Normalize();
                        position.velocity *= 2*maxSpeed;
                    }

                }
            }
            else if (moveType == VexedLib.MovementType.Hover)
            {
                if (directionChangeCooldown == 0)
                {
                    if (Vector3.Dot(rightUnit, position.velocity) > 0)
                        rightFacing = true;
                    if (Vector3.Dot(rightUnit, position.velocity) < 0)
                        rightFacing = false;
                }
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightMoving = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightMoving = false;

                if (position.direction != Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false))
                {
                    spinUp = Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false);
                }
                if (spinUp != Vector3.Zero)
                {
                    spinTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (spinTime > spinMaxTime)
                    {
                        spinTime = 0;
                        position.direction = spinUp;
                        spinUp = Vector3.Zero;
                    }
                }

                position.velocity += acceleration * direction;
                if (position.velocity.Length() > maxSpeed)
                {
                    position.velocity.Normalize();
                    position.velocity *= maxSpeed;
                }
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

        public List<Vector3> GetGroundCollisionRect()
        {
            List<Vector3> doodadVertexList = new List<Vector3>();

            doodadVertexList.Add(position.position + right);
            doodadVertexList.Add(position.position + left);
            doodadVertexList.Add(position.position + 1.1f*down + left);
            doodadVertexList.Add(position.position + 1.1f*down + right);

            return doodadVertexList;
        }

        public List<Vector3> GetForwardGroundCollisionRect()
        {
            List<Vector3> doodadVertexList = new List<Vector3>();
            Vector3 forward = Vector3.Zero;
            
            
            if (rightMoving == false)
            {
                forward = -.75f * right;
            }
            else if (rightMoving == true)
            {
                forward = .75f * right;
            }
            forward+= Vector3.Dot(position.velocity, forward) * forward;

            doodadVertexList.Add(position.position + right/4f + forward);
            doodadVertexList.Add(position.position + left / 4f + forward);
            doodadVertexList.Add(position.position + 1.1f * down + left / 4f + forward);
            doodadVertexList.Add(position.position + 1.1f * down + right/4f + forward);

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

        public bool CollisionFirstPass(Projectile p)
        {
            return (boundingBoxBottom > p.boundingBoxTop ||
                        boundingBoxTop < p.boundingBoxBottom ||
                        boundingBoxLeft > p.boundingBoxRight ||
                        boundingBoxRight < p.boundingBoxLeft);
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
            AdjustVertex(pos, vel, normal, playerUp, true);
        }

        public void AdjustVertex(Vector3 pos, Vector3 vel, Vector3 normal, Vector3 playerUp, bool fold)
        {
            Vector3 playerRight = Vector3.Cross(playerUp, normal);
            if (position.normal == normal || (position.normal == -normal && fold==false))
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


        public static Vector3 AdjustVector(Vector3 input, Vector3 targetNormal, Vector3 playerNnormal, Vector3 playerUp, bool fold)
        {
            Vector3 playerRight = Vector3.Cross(playerUp, playerNnormal);
            if (targetNormal == playerNnormal || (targetNormal == -playerNnormal && fold == false))
            {
                return input;
            }
            else if (targetNormal == -playerNnormal)
            {
                float badVelComponent = Vector3.Dot(playerUp, input);
                return input - 2 * badVelComponent * playerUp;
            }
            else if (targetNormal == playerUp)
            {
                float badVelComponent = Vector3.Dot(playerUp, input);
                return input - badVelComponent * playerUp + badVelComponent * -playerNnormal;
            }
            else if (targetNormal == -playerUp)
            {
                float badVelComponent = Vector3.Dot(playerUp, input);
                return input - badVelComponent * playerUp + badVelComponent * playerNnormal;
            }
            else if (targetNormal == playerRight)
            {
                float badVelComponent = Vector3.Dot(playerRight, input);
                return input - badVelComponent * playerRight + badVelComponent * -playerNnormal;
            }
            else if (targetNormal == -playerRight)
            {
                float badVelComponent = Vector3.Dot(playerRight, input);
                return input - badVelComponent * playerRight + badVelComponent * playerNnormal;
            }
            return Vector3.Zero;
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

            List<Vertex> gunVertexList = new List<Vertex>();
            gunVertexList.Add(new Vertex(position.position, position.normal, .5f * gunNormal, position.direction));
            gunVertexList.Add(new Vertex(position.position, position.normal, gunLine + .5f * gunNormal, position.direction));
            gunVertexList.Add(new Vertex(position.position, position.normal, gunLine - .5f * gunNormal, position.direction));
            gunVertexList.Add(new Vertex(position.position, position.normal, -.5f*gunNormal, position.direction));
            
            foreach (Vertex v in rectVertexList)
            {
                v.Update(Engine.player.currentRoom, 1);
            }
            foreach (Vertex v in gunVertexList)
            {
                v.Update(Engine.player.currentRoom, 1);
            }

            if (moveType == VexedLib.MovementType.Tank)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - .1f, textureTriangleList, treadsTexCoords, rightFacing);
            else if (moveType == VexedLib.MovementType.Spider)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - .1f, textureTriangleList, spiderTexCoords, rightFacing);
            else if (moveType == VexedLib.MovementType.Jump)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth-.1f, textureTriangleList, legsTexCoords, rightFacing);

            r.AddTextureToTriangleList(gunVertexList, Color.White, depth - .05f, textureTriangleList, gunTexCoords, true);

            r.AddTextureToTriangleList(rectVertexList, Color.Blue, depth, textureTriangleList, bodyTexCoords, rightFacing);
            if (armorType == VexedLib.ArmorType.Full)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, fullArmorTexCoords, rightFacing);
            if (armorType == VexedLib.ArmorType.Top)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, topArmorTexCoords, rightFacing);
            if (armorType == VexedLib.ArmorType.Shield)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, frontArmorTexCoords, rightFacing);
            r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, eyesTexCoords, rightFacing);
            if(moveType == VexedLib.MovementType.Tank)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth+.1f, textureTriangleList, treadsTexCoords, rightFacing);
            else if (moveType == VexedLib.MovementType.Spider)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth+.1f, textureTriangleList, spiderTexCoords, rightFacing);
            else if (moveType == VexedLib.MovementType.Jump)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth+.1f, textureTriangleList, legsTexCoords, rightFacing);

            
            VertexPositionColorNormalTexture[] triangleArray = textureTriangleList.ToArray();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
        }
    }
}

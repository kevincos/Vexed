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
using System.Xml.Serialization;

namespace VexedCore
{
    public enum BaseType
    {
        None,
        Rock,
        Ice,
        Standard
    }

    public class GunEmplacement
    {
        public float currentAngle = .5f;
        public int fireCooldown = 0;
        public VexedLib.TrackType trackType;
        public Vector2 positionOffset;
        public Vertex position;
        public Vector3 gunLine = Vector3.Zero;
        public Vector3 gunNormal = Vector3.Zero;
        public VexedLib.GunType gunType;
        public float radius = .5f;
        public float baseRadius = .5f;
        public float depthOffset = 0f;
        public BaseType baseType = BaseType.None;
        

        public GunEmplacement(VexedLib.TrackType trackType, VexedLib.GunType gunType, Vector2 positionOffset, float radius, float depthOffset, BaseType baseType)
        {
            this.gunType = gunType;
            this.trackType = trackType;
            this.positionOffset = positionOffset;
            position = new Vertex();
            this.radius = radius;
            this.baseRadius = .75f * radius;
            this.depthOffset = depthOffset;
            this.baseType = baseType;
        }

        public GunEmplacement(GunEmplacement g)
        {
            trackType = g.trackType;
            currentAngle = g.currentAngle;
            fireCooldown = g.fireCooldown;
            //position = new Vertex(g.position);
            positionOffset = g.positionOffset;
            gunLine = g.gunLine;
            gunNormal = g.gunNormal;
            gunType = g.gunType;
            radius = g.radius;
            position = new Vertex();
            depthOffset = g.depthOffset;
            baseType = g.baseType;
            baseRadius = g.baseRadius;
        }

        public float angleRotateSpeed
        {
            get
            {
                if (trackType == VexedLib.TrackType.Slow)
                {
                    return .01f;
                }
                if (trackType == VexedLib.TrackType.Normal)
                {
                    return .03f;
                }
                if (trackType == VexedLib.TrackType.Fast)
                {
                    return .1f;
                }

                return 0f;
            }
        }

        public int fireTime
        {
            get
            {
                if (gunType == VexedLib.GunType.Blaster || gunType == VexedLib.GunType.Spread)
                    return 2000;
                if (gunType == VexedLib.GunType.Missile)
                    return 4000;
                if (gunType == VexedLib.GunType.Beam)
                    return 2000;
                if (gunType == VexedLib.GunType.Repeater)
                    return 200;
                return 0;
            }
        }


        public float weaponRange
        {
            get
            {
                if (gunType == VexedLib.GunType.Missile)
                    return 20f;
                if (gunType == VexedLib.GunType.Beam)
                    return 20f;
                return 15f;
            }
        }
        
        public void Upgate(GameTime gameTime, Monster srcMonster)
        {
            position = new Vertex(srcMonster.position);
            position.velocity = positionOffset.X * srcMonster.rightUnit - positionOffset.Y * srcMonster.upUnit;
            position.Update(Engine.player.currentRoom, 1);

            if ((Engine.player.center.position - position.position).Length() < weaponRange)
                fireCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (fireCooldown < 0)
                fireCooldown = 0;

            float cosTheta = 0f;
            float sinTheta = 0f;
            if (angleRotateSpeed != 0)
            {
                Vector3 aimTarget = Engine.player.center.position - position.position;
                aimTarget.Normalize();
                cosTheta = Vector3.Dot(srcMonster.upUnit, aimTarget);
                sinTheta = Vector3.Dot(srcMonster.rightUnit, aimTarget);
                float targetAngle = 0;
                if (sinTheta > 0)
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

                if (currentAngle > 2 * Math.PI)
                    currentAngle -= (float)Math.PI * 2;
                if (currentAngle < 0)
                    currentAngle += (float)Math.PI * 2;
                if (!(srcMonster.moveType == VexedLib.MovementType.Hover || srcMonster.moveType == VexedLib.MovementType.RockBoss || srcMonster.moveType == VexedLib.MovementType.SnakeBoss || srcMonster.moveType == VexedLib.MovementType.BattleBoss))
                {
                    if (currentAngle > Math.PI / 2 && currentAngle < Math.PI)
                        currentAngle = (float)(Math.PI / 2 - .05f);
                    if (currentAngle > Math.PI && currentAngle < 3 * Math.PI / 2)
                        currentAngle = (float)(3 * Math.PI / 2 + .05f);
                }
            }
            else if (trackType == VexedLib.TrackType.Up)
            {
                currentAngle = 0f;
            }
            else if (trackType == VexedLib.TrackType.UpLeft)
            {
                currentAngle = (float)(7 * Math.PI / 4);
            }
            else if (trackType == VexedLib.TrackType.UpRight)
            {
                currentAngle = (float)(Math.PI / 4);
            }
            else if (trackType == VexedLib.TrackType.Left)
            {
                currentAngle = (float)(3 * Math.PI / 2);
            }
            else if (trackType == VexedLib.TrackType.Right)
            {
                currentAngle = (float)(Math.PI / 2);
            }



            cosTheta = (float)Math.Cos(currentAngle);
            sinTheta = (float)Math.Sin(currentAngle);
            gunLine = srcMonster.right * sinTheta + srcMonster.up * cosTheta;
            gunNormal = Vector3.Cross(gunLine, position.normal);
            gunLine.Normalize();
            gunNormal.Normalize();

            if (srcMonster.moveType == VexedLib.MovementType.RockBoss && srcMonster.rockBoss.state != RockBossState.Fight1 && srcMonster.rockBoss.state != RockBossState.Fight2 && srcMonster.rockBoss.state != RockBossState.Fight3)
                return;

            if (fireCooldown == 0)
            {
                Vector3 projectileVelocity = gunLine;
                projectileVelocity.Normalize();

                fireCooldown = fireTime;
                if (gunType == VexedLib.GunType.Blaster || gunType == VexedLib.GunType.Repeater)
                {
                    Engine.player.currentRoom.projectiles.Add(new Projectile(srcMonster, ProjectileType.Plasma, position.position + radius * gunLine, Vector3.Zero, position.normal, projectileVelocity));
                }
                if (gunType == VexedLib.GunType.Beam)
                {
                    Engine.player.currentRoom.projectiles.Add(new Projectile(srcMonster, ProjectileType.Laser, position.position + radius * gunLine, Vector3.Zero, position.normal, projectileVelocity));
                }
                if (gunType == VexedLib.GunType.Missile)
                {
                    Engine.player.currentRoom.projectiles.Add(new Projectile(srcMonster, ProjectileType.Missile, position.position + radius * gunLine, position.velocity, position.normal, projectileVelocity));
                }
                if (gunType == VexedLib.GunType.Spread)
                {
                    Engine.player.currentRoom.projectiles.Add(new Projectile(srcMonster, ProjectileType.Plasma, position.position + radius * gunLine, Vector3.Zero, position.normal, projectileVelocity + .5f * gunNormal));
                    Engine.player.currentRoom.projectiles.Add(new Projectile(srcMonster, ProjectileType.Plasma, position.position + radius * gunLine, Vector3.Zero, position.normal, projectileVelocity - .5f * gunNormal));
                    Engine.player.currentRoom.projectiles.Add(new Projectile(srcMonster, ProjectileType.Plasma, position.position + radius * gunLine, Vector3.Zero, position.normal, projectileVelocity));
                }

            }
        }

    }
}
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
    public class BattleBoss
    {
        public Vector3 aimDirection;
        public int nextWaypointIndex = 0;
        public Vector3 nextWaypointTarget = Vector3.Zero;

        public int phase = 0;
        public bool dialogStarted = false;

        public void Update(int time, Monster srcMonster)
        {
            if (Engine.player.currentRoom.bossCleared == true)
                return;
            if (phase == 0 && srcMonster.state == MonsterState.Normal && dialogStarted == false)
            {
                dialogStarted = true;
                DialogBox.SetDialog("JetBoss1");
            }


            aimDirection = Engine.player.center.position - srcMonster.position.position;
            aimDirection.Normalize();

            if (srcMonster.baseHP == 0)
            {
                srcMonster.hasOrbs = true;
                if (phase == 0)
                {
                    srcMonster.guns[0].gunType = VL.GunType.Repeater;
                    DialogBox.SetDialog("JetBoss2");
                }
                if (phase == 1)
                {
                    srcMonster.guns[1].gunType = VL.GunType.Repeater;
                    srcMonster.guns[1].fireCooldown = 0;
                    srcMonster.guns[0].fireCooldown = 0;
                    DialogBox.SetDialog("JetBoss3");
                }
                if (phase == 2)
                {
                    DialogBox.SetDialog("JetBoss4");
                    Engine.player.currentRoom.bossCleared = true;
                }
                phase++;
                srcMonster.baseHP = srcMonster.startingBaseHP;
            }

            Vector3 direction = nextWaypointTarget - srcMonster.position.position;
            float distance = direction.Length();

            float playerdistance = (Engine.player.center.position - srcMonster.position.position).Length();
            if (playerdistance > 8f)
                srcMonster.speedType = VL.MonsterSpeed.Fast;
            else if (playerdistance > 3.5f)
                srcMonster.speedType = VL.MonsterSpeed.Medium;
            else
                srcMonster.speedType = VL.MonsterSpeed.Slow;

            if (nextWaypointIndex == 0 || Vector3.Dot(srcMonster.position.velocity, direction) < 0f)
            {
                if (nextWaypointTarget != Vector3.Zero)
                    srcMonster.position.position = nextWaypointTarget;
                srcMonster.position.velocity = Vector3.Zero;

                nextWaypointIndex++;
                if (nextWaypointIndex == 13) nextWaypointIndex = 1;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.speedType = VL.MonsterSpeed.Medium;
            }
    
            Vector3 targetUp = Vector3.Cross(srcMonster.position.normal, aimDirection);
            float tiltVelocity = .009f;
            if (playerdistance > 1)
                tiltVelocity = .013f;
            if (Vector3.Dot(targetUp, srcMonster.rightUnit) > .1f)
            {
                //tilt up
                srcMonster.position.direction += tiltVelocity * srcMonster.rightUnit;
                srcMonster.position.direction.Normalize();
            }
            else
            {
                //tilt down
                srcMonster.position.direction -= tiltVelocity * srcMonster.rightUnit;
                srcMonster.position.direction.Normalize();
            }
            

        }


        public Vector3 GetWaypointTarget()
        {

            String targetDoodadId = "BattleW" + nextWaypointIndex + "_";

            foreach (Doodad d in Engine.player.currentRoom.doodads)
            {
                if (d.id.Contains(targetDoodadId))
                {
                    return d.position.position;
                }
            }
            return Vector3.Zero;
        }
    }
}

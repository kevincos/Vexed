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
    public class JetBoss
    {
        public Vector3 aimDirection;
        public int nextWaypointIndex = 0;
        public Vector3 nextWaypointTarget = Vector3.Zero;
        public bool rightFacing = true;
        public int currentBehaviorType = 0;

        public void Update(int time, Monster srcMonster)
        {
            int behaviorType = (srcMonster.baseHP / 3) % 4;
            if (behaviorType != currentBehaviorType)
            {
                if (behaviorType == 0 || behaviorType == 2)
                {
                    rightFacing = true;
                }
                else
                {
                    rightFacing = false;
                }
                if (behaviorType == 0 || behaviorType ==2)
                {
                    srcMonster.spinUp = Vector3.Cross(srcMonster.position.normal, srcMonster.position.direction);
                }
                currentBehaviorType = behaviorType;
            }
            
            srcMonster.rightFacing = rightFacing;
            aimDirection = Engine.player.center.position - srcMonster.position.position;
            aimDirection.Normalize();

            Vector3 direction = nextWaypointTarget - srcMonster.position.position;
            float distance = direction.Length();

            if (nextWaypointIndex == 0 || Vector3.Dot(srcMonster.position.velocity, direction) < 0f)
            {
                if (nextWaypointTarget != Vector3.Zero)
                    srcMonster.position.position = nextWaypointTarget;
                srcMonster.position.velocity = Vector3.Zero;

                if (rightFacing)
                {
                    nextWaypointIndex++;
                    if (nextWaypointIndex >= 9) nextWaypointIndex = 1;
                }
                else
                {
                    nextWaypointIndex--;
                    if (nextWaypointIndex <= 0) nextWaypointIndex = 8;
                }
            
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.speedType = VL.MonsterSpeed.Medium;
            }


        }


        public Vector3 GetWaypointTarget()
        {

            String targetDoodadId = "JetW" + nextWaypointIndex + "_";

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

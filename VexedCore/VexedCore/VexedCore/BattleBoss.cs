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

        public void Update(int time, Monster srcMonster)
        {
            aimDirection = Engine.player.center.position - srcMonster.position.position;
            aimDirection.Normalize();

            Vector3 direction = nextWaypointTarget - srcMonster.position.position;
            float distance = direction.Length();

            if (nextWaypointIndex == 0 || Vector3.Dot(srcMonster.position.velocity, direction) < 0f)
            {
                if (nextWaypointTarget != Vector3.Zero)
                    srcMonster.position.position = nextWaypointTarget;
                srcMonster.position.velocity = Vector3.Zero;

                nextWaypointIndex++;
                if (nextWaypointIndex == 9) nextWaypointIndex = 1;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.speedType = VexedLib.MonsterSpeed.Medium;
            }
    
            Vector3 targetUp = Vector3.Cross(srcMonster.position.normal, aimDirection);
            if (Vector3.Dot(targetUp, srcMonster.rightUnit) > .1f)
            {
                //tilt up
                srcMonster.position.direction -= .013f * srcMonster.rightUnit;
                srcMonster.position.direction.Normalize();
            }
            else
            {
                //tilt down
                srcMonster.position.direction += .013f*srcMonster.rightUnit;
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

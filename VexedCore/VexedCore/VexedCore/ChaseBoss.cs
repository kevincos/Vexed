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
    public class ChaseBoss
    {
        public int nextWaypointIndex = 0;
        public Vector3 nextWaypointTarget = Vector3.Zero;
        public static bool studder = false;
        public int studderCooldown = 0;
        public int studderMaxCooldown = 300;

        public int dialogState = 0;

        public Vector3 GetWaypointTarget()
        {
            
            String targetDoodadId = "Chase"+nextWaypointIndex+"_";
            
            foreach (Doodad d in Engine.player.currentRoom.doodads)
            {
                if (d.id.Contains(targetDoodadId))
                {
                    return d.position.position;
                }
            }
            return Vector3.Zero;
        }

        public void Update(int time, Monster srcMonster)
        {
            foreach (Doodad d in Engine.player.currentRoom.doodads)
            {
                if (d.active == false && d.type == VexedLib.DoodadType.TriggerPoint)
                {
                    if (d.id.Contains("ChaseTalk"))
                    {
                        float distanceToTrigger = (d.position.position - srcMonster.position.position).Length();
                        if (distanceToTrigger < 2)
                        {
                            DialogBox.SetDialog(d.targetObject);
                            d.active = true;
                            dialogState++;
                        }
                    }
                }
            }
            if (dialogState == 2 && (Engine.player.primaryAbility.type == AbilityType.Blaster || Engine.player.secondaryAbility.type == AbilityType.Blaster))
            {
                DialogBox.SetDialog("ChaseBoss3");
                dialogState++;
            }
            if (dialogState < 9 && srcMonster.baseHP < srcMonster.startingBaseHP)
            {
                DialogBox.SetDialog("ChaseBoss4");
                dialogState = 9;
            }
            if (dialogState < 10 && srcMonster.dead == true)
            {
                DialogBox.SetDialog("ChaseBoss5");
                dialogState = 10;
            }
            float distance = (srcMonster.position.position - nextWaypointTarget).Length();
            if (studder == true)
            {
                studderCooldown = studderMaxCooldown;                
                studder = false;
            }
            if (studderCooldown > 0)
            {
                srcMonster.position.velocity = Vector3.Zero;
            }
            studderCooldown -= time;
            if (studderCooldown < 0)
                studderCooldown = 0;
            if (nextWaypointIndex == 0 || distance < .2f)
            {
                if(nextWaypointTarget != Vector3.Zero)
                    srcMonster.position.position = nextWaypointTarget;
                srcMonster.position.velocity = Vector3.Zero;
                nextWaypointIndex++;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.speedType = VexedLib.MonsterSpeed.Medium;
                if(nextWaypointIndex > 27)
                    srcMonster.speedType = VexedLib.MonsterSpeed.Fast;
            }
        }
    }
}

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
    public class ArmorBoss
    {
        public Vector3 aimDirection;
        public int nextWaypointIndex = 0;
        public Vector3 nextWaypointTarget = Vector3.Zero;

        public bool rotating = false;
        
        public static Random r;
        public int dialogState = 0;

        static ArmorBoss()
        {
            r = new Random();
        }

        public void Rotate(Monster srcMonster)
        {
            if (rotating == false)
            {
                rotating = true;
                int rotateDirection = r.Next(0, 2);

                if (rotateDirection == 0)
                {
                    aimDirection = srcMonster.position.direction;
                }
                if (rotateDirection == 1)
                {
                    aimDirection = -srcMonster.position.direction;
                }
                
            }
        }
        

        public void Update(int time, Monster srcMonster)
        {

            if (srcMonster.id.Contains("Basic"))
            {
                if (dialogState == 0 && Engine.player.state == State.Normal)
                {
                    DialogBox.SetDialog("ArmorBoss1");
                    dialogState++;
                }
                if (dialogState == 1 && srcMonster.baseHP < srcMonster.startingBaseHP)
                {
                    DialogBox.SetDialog("ArmorBoss2");
                    dialogState++;
                }
                if (dialogState == 2 && srcMonster.baseHP < 1 * srcMonster.startingBaseHP / 2)
                {
                    DialogBox.SetDialog("ArmorBoss3");
                    dialogState++;
                }
                if (dialogState < 10 && srcMonster.dead)
                {
                    DialogBox.SetDialog("ArmorBoss4");
                    dialogState = 10;
                }
            }
            if(srcMonster.id.Contains("AdvancedMain"))
            {
                if (dialogState == 0 && Engine.player.state == State.Normal)
                {
                    DialogBox.SetDialog("FinalArmor1");
                    dialogState++;
                }
            }
            if (srcMonster.id.Contains("Advanced"))
            {
                if (dialogState < 10 && srcMonster.dead == true)
                {
                    DialogBox.SetDialog("FinalArmorDead");
                    dialogState=10;
                }
            }
            if (srcMonster.id.Contains("MediumMain"))
            {
                if (dialogState == 0 && Engine.player.state == State.Normal)
                {
                    DialogBox.SetDialog("MedArmor1");
                    dialogState++;
                }
            }
            if (srcMonster.id.Contains("Medium"))
            {
                if (dialogState < 10 && srcMonster.dead == true)
                {
                    DialogBox.SetDialog("MedArmorDead");
                    dialogState = 10;
                }
            }

            if (srcMonster.id.Contains("Basic") && srcMonster.baseHP < 1*srcMonster.startingBaseHP / 2)
                srcMonster.guns[0].gunType = VL.GunType.Missile;

            if (aimDirection == Vector3.Zero)
                Rotate(srcMonster);

            Vector3 targetUp = Vector3.Cross(srcMonster.position.normal, aimDirection);

            float angleDiff = Vector3.Dot(targetUp, srcMonster.rightUnit);
            float angleChange = 0f;
            rotating = true;
            if (angleDiff > .3f)
                angleChange = .05f;
            else if (angleDiff < -.3f)
                angleChange = -.05f;
            else if (angleDiff > .1f)
                angleChange = .02f;
            else if (angleDiff < -.1f)
                angleChange = -.02f;
            else
            {
                rotating = false;
                angleChange = 0;
                srcMonster.position.direction = -targetUp;
            }

            if (angleChange != 0)
            {
                srcMonster.position.direction -= angleChange * srcMonster.rightUnit;
                srcMonster.position.direction.Normalize();
            }
            


        }

    }
}

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
    public class SnakeBoss
    {
        public bool waitingForPrev = false;
        public bool waitingForNext = false;
        public bool tail = false;
        public int nextWaypointIndex = 0;
        public Vector3 nextWaypointTarget = Vector3.Zero;
        public Monster nextSnakeLink;
        public int chainIndex = 0;
        public static int totalLife = 0;
        public bool deactivated = false;
        public static int dialogState = 0;
        public static bool gunDialog = false;

        public SnakeBoss()
        {
            
        }

        public static void Init()
        {
            totalLife = 0;
            dialogState = 0;
            gunDialog = false;
        }

        public bool waiting
        {
            get
            {
                return waitingForNext || waitingForPrev;
            }
        }
        

        public void InitializeLinks(Monster srcMonster, Room targetRoom)
        {
            SnakeBoss.totalLife++;
            if (srcMonster.id.Contains("T"))
                tail = true;
            String idNumber = srcMonster.id.Substring(srcMonster.id.IndexOf("S")+1, srcMonster.id.IndexOf("_") - srcMonster.id.IndexOf("S")-1);
            chainIndex = Convert.ToInt32(idNumber);
            int nextId = Convert.ToInt32(idNumber) - 1;
            foreach (Monster m in targetRoom.monsters)
            {
                if (m.id.Contains("S" + nextId + "_"))
                {
                    nextSnakeLink = m;
                }
            }
        }

        public Vector3 GetWaypointTarget()
        {

            String targetDoodadId = "SnakeW" + nextWaypointIndex + "_";

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
            if (SnakeBoss.totalLife == 0 && Engine.player.currentRoom.bossCleared == false)
            {
                if (dialogState < 9)
                {
                    dialogState = 9;
                    DialogBox.SetDialog("IceSnake5");
                }
                srcMonster.dead = true;
            }
            if (dialogState == 1 && srcMonster.baseHP == 2)
            {
                dialogState++;
                DialogBox.SetDialog("IceSnake2");
            }
            if (dialogState == 2 && srcMonster.baseHP == 1)
            {
                dialogState++;
                DialogBox.SetDialog("IceSnake4");
            }
            if (srcMonster.baseHP == 0 && deactivated == false)
            {
                deactivated = true;
                SnakeBoss.totalLife--;
            }
            if (chainIndex % 5 == 1)
            {
                if (srcMonster.guns.Count == 0)
                    srcMonster.guns.Add(new GunEmplacement(VL.TrackType.Fast, VL.GunType.None, Vector2.Zero, .7f * srcMonster.halfWidth, .01f, BaseType.Ice, srcMonster));
                else if (srcMonster.baseHP <= 1)
                {
                    if (gunDialog == false)
                    {
                        //DialogBox.SetDialog("IceSnake3");
                        gunDialog = true;                        
                    }
                    srcMonster.guns[0].gunType = VL.GunType.Blaster;
                }

            }
            Vector3 direction = nextWaypointTarget - srcMonster.position.position;
            float distance = direction.Length();

            if (SnakeBoss.totalLife == 0 && Engine.player.currentRoom.bossCleared == false)
            {
                Engine.player.currentRoom.bossCleared = true;
                foreach (Monster m in Engine.player.currentRoom.monsters)
                {                    
                    if (m.moveType == VL.MovementType.SnakeBoss)
                    {
                        m.dead = true;
                        m.state = MonsterState.Death;
                    }
                }
            }


            if (nextWaypointIndex == 0f || Vector3.Dot(srcMonster.position.velocity, direction) < 0f)
            {
                if (dialogState == 0 && Vector3.Dot(srcMonster.position.velocity, direction) < 0f)
                {
                    DialogBox.SetDialog("IceSnake1");
                    dialogState++;
                }

                if (nextWaypointTarget != Vector3.Zero)
                    srcMonster.position.position = nextWaypointTarget;
                srcMonster.position.velocity = Vector3.Zero;
                
                nextWaypointIndex++;
                if (nextWaypointIndex == 19) nextWaypointIndex = 1;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.speedType = VL.MonsterSpeed.Medium;
                if(nextSnakeLink != null)
                    waitingForNext = true;
                if(tail==false)
                    waitingForPrev = true;
                if(nextSnakeLink != null)
                    nextSnakeLink.snakeBoss.waitingForPrev = false;

            }

            if (waitingForNext)
            {
                float distanceToNextLink = (nextSnakeLink.position.position - srcMonster.position.position).Length();
                if (distanceToNextLink > 1.4f*srcMonster.halfWidth)
                    waitingForNext = false;                
            }
        }
    }
}

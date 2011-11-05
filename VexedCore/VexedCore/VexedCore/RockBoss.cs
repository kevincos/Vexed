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
    public enum RockBossState
    {
        Fight1,
        FleeA,
        FleeB,
        FleeC,
        Wait,
        Fight2,
        FleeC2,
        Phase,
        Wait2,
        Fight3,
        Snow_Fight1,
        Snow_Battle1,
        Snow_Flee1,
        Snow_Fight2,
        Snow_Battle2,
        Command_Fight1,
        Command_Battle1,
        Command_Flee1,
        Command_Fight2,
        Command_Battle2,
    }

    public class RockBoss
    {
        public static bool triggered = false;

        public int maxRockHits = 2;
        public int rockHits = 2;
        public int rockHitCooldown = 0;
        public int maxRockHitCooldown = 200;
        public RockBossState state = RockBossState.Snow_Fight1;
        public Vector3 nextWaypointTarget = Vector3.Zero;
        public bool phasing = false;
        public int dialogState = 0;

        public bool Impact()
        {
            bool result = false;
            if (rockHitCooldown == 0 && rockHits > 0)
            {
                rockHits--;
                result = true;
            }
            rockHitCooldown = maxRockHitCooldown;
            return result;
        }

        public Vector3 GetWaypointTarget()
        {
            String targetDoodadId = "";
            if (state == RockBossState.Snow_Flee1)
            {
                targetDoodadId = "SnowFlee";
            }
            if(state == RockBossState.Command_Flee1)
            {
                targetDoodadId = "CommandFlee";
            }
            if (state == RockBossState.FleeA)
            {
                targetDoodadId = "RockBossA";
            }
            if (state == RockBossState.FleeB)
            {
                targetDoodadId = "RockBossB";
            }
            if (state == RockBossState.FleeC)
            {
                targetDoodadId = "RockBossC";
            }
            if (state == RockBossState.FleeC2)
            {
                targetDoodadId = "RockBossC";
            }
            if (state == RockBossState.Phase)
            {
                targetDoodadId = "RockBossD";
            }
            if (targetDoodadId == "")
                return Vector3.Zero;
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
            if (srcMonster.id.Contains("CommandBoss") && state == RockBossState.Snow_Fight1)
            {
                state = RockBossState.Command_Fight1;
            }

            srcMonster.huntMinDistance = 4.5f;
            if (srcMonster.id.Contains("Snow"))
            {
                if (dialogState == 0 && rockHits != maxRockHits)
                {
                    DialogBox.SetDialog("SnowMan2");
                    dialogState++;
                }
                if (dialogState == 1 && state == RockBossState.Snow_Battle1)
                {
                    DialogBox.SetDialog("SnowMan3");
                    dialogState++;
                }
                if (dialogState == 2 && state == RockBossState.Snow_Fight2)
                {
                    DialogBox.SetDialog("SnowMan4");
                    dialogState++;
                }
                if (dialogState == 3 && state == RockBossState.Snow_Battle2)
                {
                    DialogBox.SetDialog("SnowMan5");
                    dialogState++;
                }
                if (dialogState == 4 && srcMonster.baseHP == 0)
                {
                    DialogBox.SetDialog("SnowMan6");
                    dialogState++;
                }
            }
            if (srcMonster.id.Contains("CommandBoss"))
            {
                if (dialogState == 0 && rockHits != maxRockHits)
                {
                    DialogBox.SetDialog("CommandBoss2");
                    dialogState++;
                }
                if (dialogState == 1 && state == RockBossState.Command_Battle1)
                {
                    DialogBox.SetDialog("CommandBoss3");
                    dialogState++;
                }
                if (dialogState == 2 && state == RockBossState.Command_Fight2)
                {
                    DialogBox.SetDialog("CommandBoss4");
                    dialogState++;
                }
                if (dialogState == 3 && srcMonster.baseHP == 0)
                {
                    DialogBox.SetDialog("CommandBoss5");
                    dialogState++;
                }
            }

            float distance = (srcMonster.position.position - nextWaypointTarget).Length();
            rockHitCooldown -= time;
            if (rockHitCooldown < 0) rockHitCooldown = 0;

            if (state == RockBossState.Fight1 && srcMonster.baseHP == 0)
            {                
                state = RockBossState.FleeA;
                
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.position.velocity = Vector3.Zero;
                
            }            
            else if (state == RockBossState.FleeA && distance < .5f)
            {
                srcMonster.speedType = VexedLib.MonsterSpeed.Medium;
                srcMonster.position.position = nextWaypointTarget;
                state = RockBossState.FleeB;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.position.velocity = Vector3.Zero;
                
                
            }
            else if (state == RockBossState.FleeB && distance < .5f)
            {
                srcMonster.position.position = nextWaypointTarget;
                state = RockBossState.FleeC;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.position.velocity = Vector3.Zero;
                
                
            }
            else if (state == RockBossState.FleeC && distance < .5f)
            {
                srcMonster.position.position = nextWaypointTarget;             
                state = RockBossState.Wait;
                triggered = false;
                nextWaypointTarget = Vector3.Zero;
                srcMonster.position.velocity = Vector3.Zero;
                srcMonster.baseHP = srcMonster.startingBaseHP;
                rockHits = maxRockHits;
                srcMonster.aiType = VexedLib.AIType.Stationary;
                
            }
            else if (state == RockBossState.Wait && triggered == true)
            {
                state = RockBossState.Fight2;
                triggered = false;
                nextWaypointTarget = Vector3.Zero;
                srcMonster.position.velocity = Vector3.Zero;
                srcMonster.baseHP = srcMonster.startingBaseHP;
                rockHits = maxRockHits;
                srcMonster.speedType = VexedLib.MonsterSpeed.Slow;
                srcMonster.aiType = VexedLib.AIType.Hunter;
                foreach (Doodad d in Engine.player.currentRoom.doodads)
                {
                    if (d.id.Contains("RockDoor"))
                    {
                        foreach (Behavior b in d.behaviors)
                        {
                            if (b.id.Contains("CLOSE"))
                                d.SetBehavior(b);
                        }
                    }
                        
                }
            }
            else if (state == RockBossState.Snow_Fight1 && rockHits == 0)
            {
                state = RockBossState.Snow_Battle1;
                srcMonster.speedType = VexedLib.MonsterSpeed.Slow;
                srcMonster.aiType = VexedLib.AIType.Hunter;                

            }
            else if (state == RockBossState.Snow_Fight2 && rockHits == 0)
            {
                state = RockBossState.Snow_Battle2;
                srcMonster.speedType = VexedLib.MonsterSpeed.Slow;
                srcMonster.aiType = VexedLib.AIType.Hunter;                

            }
            else if (state == RockBossState.Command_Fight1 && rockHits == 0)
            {
                state = RockBossState.Command_Battle1;
                srcMonster.speedType = VexedLib.MonsterSpeed.Slow;
                srcMonster.aiType = VexedLib.AIType.Hunter;

            }
            else if (state == RockBossState.Command_Fight2 && rockHits == 0)
            {
                state = RockBossState.Command_Battle2;
                srcMonster.speedType = VexedLib.MonsterSpeed.Slow;
                srcMonster.aiType = VexedLib.AIType.Hunter;

            }
            else if (state == RockBossState.Snow_Battle1 && srcMonster.baseHP == 0)
            {
                state = RockBossState.Snow_Flee1;
                srcMonster.speedType = VexedLib.MonsterSpeed.Medium;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.position.velocity = Vector3.Zero;
            }
            else if (state == RockBossState.Command_Battle1 && srcMonster.baseHP == 0)
            {
                state = RockBossState.Command_Flee1;
                srcMonster.speedType = VexedLib.MonsterSpeed.Medium;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.position.velocity = Vector3.Zero;
            }
            else if (state == RockBossState.Fight2 && srcMonster.baseHP == 0)
            {
                state = RockBossState.FleeC2;
                srcMonster.speedType = VexedLib.MonsterSpeed.Medium;
                nextWaypointTarget = GetWaypointTarget();
                srcMonster.position.velocity = Vector3.Zero;

            }  
            else if (state == RockBossState.FleeC2 && distance < 1f)
            {
                srcMonster.position.position = nextWaypointTarget;
                state = RockBossState.Phase;
                srcMonster.position.velocity = Vector3.Zero;
                phasing = true;
                nextWaypointTarget = GetWaypointTarget();
            }
            else if (state == RockBossState.Snow_Flee1 && distance < 1f)
            {
                state = RockBossState.Snow_Fight2;
                rockHits = maxRockHits;
                srcMonster.baseHP = srcMonster.startingBaseHP;
                srcMonster.position.position = nextWaypointTarget;
                srcMonster.position.velocity = Vector3.Zero;
                srcMonster.aiType = VexedLib.AIType.Stationary;

                nextWaypointTarget = GetWaypointTarget();
            }
            else if (state == RockBossState.Command_Flee1 && distance < 1f)
            {
                state = RockBossState.Command_Fight2;
                rockHits = maxRockHits;
                srcMonster.baseHP = srcMonster.startingBaseHP;
                srcMonster.position.position = nextWaypointTarget;
                srcMonster.position.velocity = Vector3.Zero;
                srcMonster.aiType = VexedLib.AIType.Stationary;

                nextWaypointTarget = GetWaypointTarget();
            }
            else if (state == RockBossState.Phase && distance < 1f)
            {
                phasing = false;
                srcMonster.position.normal = -srcMonster.position.normal;
                srcMonster.position.position = nextWaypointTarget;
                srcMonster.position.velocity = Vector3.Zero;
                state = RockBossState.Wait2;
                nextWaypointTarget = Vector3.Zero;
                triggered = false;
                srcMonster.aiType = VexedLib.AIType.Stationary;
            }
            else if (state == RockBossState.Wait2 && triggered == true)
            {
                state = RockBossState.Fight3;
                triggered = false;
                nextWaypointTarget = Vector3.Zero;
                srcMonster.position.velocity = Vector3.Zero;
                srcMonster.baseHP = srcMonster.startingBaseHP;
                srcMonster.huntMinDistance = 5f;
                rockHits = maxRockHits;
                srcMonster.speedType = VexedLib.MonsterSpeed.Slow;
                srcMonster.aiType = VexedLib.AIType.Hunter;
            }
        }
    }
}

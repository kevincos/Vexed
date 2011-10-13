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
    public enum ObjectiveType
    {
        DefeatBoss,
        Location,
        Upgrade
    }

    public class Objective
    {
        public ObjectiveType type;

        public List<String> locationTriggers;
        public List<String> targetBosses;
        public List<AbilityType> upgradeGoals;



        public String text;

        public Objective(ObjectiveType type, String text)
        {
            this.type = type;
            this.text = text;
            locationTriggers = new List<string>();
            targetBosses = new List<string>();
            upgradeGoals = new List<AbilityType>();
        }
    }

    class ObjectiveControl
    {
        public static int oscillate = 0;
        public static int maxOscillate = 1000;
        public static List<Objective> objectives;

        static ObjectiveControl()
        {
            objectives = new List<Objective>();

            Objective doubleJump = new Objective(ObjectiveType.Upgrade, "Locate Double Jump Upgrade");
            doubleJump.upgradeGoals.Add(AbilityType.DoubleJump);

            Objective blaster = new Objective(ObjectiveType.Upgrade, "Locate Blaster");
            blaster.upgradeGoals.Add(AbilityType.Blaster);

            Objective armorBoss = new Objective(ObjectiveType.DefeatBoss, "Defeat Armortron");
            armorBoss.targetBosses.Add("Basic_4161");

            Objective findHub = new Objective(ObjectiveType.Location, "Locate the Central Hub");
            findHub.locationTriggers.Add("HubEnter_11539");

            Objective findStorage = new Objective(ObjectiveType.Location, "Enter the Storage Sector");
            findStorage.locationTriggers.Add("StorageEnter_11541");

            Objective magnetBoots = new Objective(ObjectiveType.Location, "Investigate Gravitational Anomaly");
            magnetBoots.upgradeGoals.Add(AbilityType.Boots);

            Objective iceSnake = new Objective(ObjectiveType.DefeatBoss, "Defeat the Snow Boss");
            iceSnake.targetBosses.Add("SnowBoss_10379");

            Objective findEngine = new Objective(ObjectiveType.Location, "Locate the Engine Sector");
            findEngine.locationTriggers.Add("EngineEntrance_11728");

            objectives.Add(doubleJump);
            objectives.Add(blaster);
            objectives.Add(armorBoss);
            objectives.Add(findHub);
            objectives.Add(findStorage);
            objectives.Add(magnetBoots);
            objectives.Add(iceSnake);
            objectives.Add(findEngine);
        }

        public static List<Vertex> GetObjectiveLocations()
        {
            List<Vertex> targets = new List<Vertex>();
            foreach (AbilityType i in objectives[Engine.player.currentObjective].upgradeGoals)
            {
                foreach (Room r in Engine.roomList)
                {
                    foreach (Doodad d in r.doodads)
                    {
                        if (d.type == VexedLib.DoodadType.UpgradeStation && d.originalAbilityType == i)
                        {
                            targets.Add(d.position);
                        }
                    }
                }
            }
            foreach (String s in objectives[Engine.player.currentObjective].targetBosses)
            {
                foreach (Room r in Engine.roomList)
                {
                    foreach (Monster m in r.monsters)
                    {
                        if (m.id == s)
                        {
                            targets.Add(m.position);
                        }
                    }
                }
            }
            foreach (String s in objectives[Engine.player.currentObjective].locationTriggers)
            {
                foreach (Room r in Engine.roomList)
                {
                    foreach (Doodad d in r.doodads)
                    {
                        if (d.id == s)
                        {
                            targets.Add(d.position);
                        }
                    }
                }
            }
            return targets;
        }

        public static void UpdateObjectiveStatus(GameTime gameTime)
        {
            oscillate += gameTime.ElapsedGameTime.Milliseconds;
            if (oscillate > maxOscillate)
            {
                oscillate = 0;
            }
            List<Vector3> targets = new List<Vector3>();
            foreach (AbilityType i in objectives[Engine.player.currentObjective].upgradeGoals)
            {
                if (Engine.player.upgrades[(int)i] == true)
                {
                    objectives[Engine.player.currentObjective].upgradeGoals.Remove(i);
                    break;
                }
            }
            foreach (Room r in Engine.roomList)
            {
                foreach (Monster m in r.monsters)
                {
                    foreach (String s in objectives[Engine.player.currentObjective].targetBosses)
                    {            
                        if (m.id == s)
                        {
                            if(m.dead == true)
                            {
                                objectives[Engine.player.currentObjective].targetBosses.Remove(s);
                                break;
                            }
                        }
                    }
                }
            }

            foreach (Room r in Engine.roomList)
            {
                foreach (Doodad d in r.doodads)
                {
                    foreach (String s in objectives[Engine.player.currentObjective].locationTriggers)
                    {
                        if (d.id == s)
                        {
                            if ((Engine.player.center.position - d.position.position).Length() < 2f)
                            {
                                objectives[Engine.player.currentObjective].locationTriggers.Remove(s);
                                break;
                            }
                        }
                    }
                }
            }

            int objectivesRemaining = objectives[Engine.player.currentObjective].locationTriggers.Count
            + objectives[Engine.player.currentObjective].targetBosses.Count
            + objectives[Engine.player.currentObjective].upgradeGoals.Count;
            if (objectivesRemaining == 0)
            {
                Engine.player.currentObjective++;

            }
        }
    }
}
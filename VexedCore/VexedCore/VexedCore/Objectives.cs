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

        public String completionDialog = "ObjectiveComplete";
        public String startDialog = "NewObjective";



        public String text;
        public String shorttext = "SHORT";
        public String longtext = "LONG";

        public Objective(ObjectiveType type, String text, String shorttext)
        {
            this.type = type;
            this.text = text;
            locationTriggers = new List<string>();
            targetBosses = new List<string>();
            upgradeGoals = new List<AbilityType>();
            this.shorttext = shorttext;
            this.longtext = text;
        }
    }

    public class ObjectiveControl
    {
        public static int oscillate = 0;
        public static int maxOscillate = 1000;
        public static List<Objective> objectives;
        public static List<String> objectiveMessageQueue;
        public static int messageQueueTimer = 0;        

        public static void InitObjectiveControl()
        {
            objectives = new List<Objective>();
            objectiveMessageQueue = new List<string>();

            Objective doubleJump = new Objective(ObjectiveType.Upgrade, "Locate Double Jump Upgrade", "Double Jump");
            doubleJump.upgradeGoals.Add(AbilityType.DoubleJump);

            Objective raindropStation = new Objective(ObjectiveType.Upgrade, "Locate the Raindrop Station", "Raindrop Station");
            raindropStation.locationTriggers.Add("RaindropEnter");


            Objective blaster = new Objective(ObjectiveType.Upgrade, "Locate Blaster", "Blaster Gun");
            blaster.upgradeGoals.Add(AbilityType.Blaster);

            Objective armorBoss = new Objective(ObjectiveType.DefeatBoss, "Defeat Armortron", "Armortron");
            armorBoss.targetBosses.Add("Basic_4161");

            Objective findHub = new Objective(ObjectiveType.Location, "Locate the Central Hub", "Central Hub");
            findHub.locationTriggers.Add("HubEnter_11539");

            Objective findStorage = new Objective(ObjectiveType.Location, "Enter the Storage Sector", "Storage Sector");
            findStorage.locationTriggers.Add("StorageEnter_11541");

            Objective magnetBoots = new Objective(ObjectiveType.Location, "Investigate Gravitational Anomaly", "Magnet Boots");
            magnetBoots.upgradeGoals.Add(AbilityType.Boots);

            Objective primaryStorage = new Objective(ObjectiveType.Location, "Enter primary storage area", "Primary Storage");
            primaryStorage.locationTriggers.Add("PrimaryStorageGoal");

            Objective snowBoss = new Objective(ObjectiveType.DefeatBoss, "Defeat the Snow Boss", "Snow Man");
            snowBoss.targetBosses.Add("SnowBoss_10379");

            Objective findEngine = new Objective(ObjectiveType.Location, "Locate the Engine Sector", "Engine Sector");
            findEngine.locationTriggers.Add("EngineEntrance_11728");

            Objective thruster = new Objective(ObjectiveType.Location, "Find thruster pack.", "Thruster Pack");
            thruster.upgradeGoals.Add(AbilityType.Booster);

            Objective missile = new Objective(ObjectiveType.Location, "Acquire explosive weapons.", "Missile Launcher");
            missile.upgradeGoals.Add(AbilityType.Missile);

            Objective suit = new Objective(ObjectiveType.Location, "Upgrade your power suit.", "Suit Upgrades");
            suit.upgradeGoals.Add(AbilityType.PermanentBoots);
            suit.upgradeGoals.Add(AbilityType.PermanentWallJump);
            suit.upgradeGoals.Add(AbilityType.ImprovedJump);

            Objective command = new Objective(ObjectiveType.Location, "Enter Command Sector.", "Command Sector");
            command.locationTriggers.Add("CommandEntrance");

            Objective spinHook = new Objective(ObjectiveType.Location, "Locate Spin Hook.", "Spin Hook");
            spinHook.upgradeGoals.Add(AbilityType.SpinHook);

            Objective jetpack = new Objective(ObjectiveType.Location, "Locate Jet Pack.", "Jet Pack");
            jetpack.upgradeGoals.Add(AbilityType.JetPack);

            Objective yellowCodes = new Objective(ObjectiveType.Upgrade, "Obtain CPU Codes", "CPU Codes");
            yellowCodes.upgradeGoals.Add(AbilityType.PermanentYellowKey);

            Objective enterCPU = new Objective(ObjectiveType.Upgrade, "Locate CPU Access Chamber", "CPU Lock");
            enterCPU.locationTriggers.Add("EnterCPU");

            Objective armorCousins = new Objective(ObjectiveType.Upgrade, "Defeat Amrortron Counsins", "Armortron Counsins");
            armorCousins.upgradeGoals.Add(AbilityType.Phase);

            Objective access = new Objective(ObjectiveType.Location, "Obtain Core Access Codes", "Core Access");
            access.upgradeGoals.Add(AbilityType.PermanentRedKey);            
            access.upgradeGoals.Add(AbilityType.PermanentBlueKey);

            Objective core = new Objective(ObjectiveType.Location, "Access Reactor Core.", "Reactor Core");
            core.locationTriggers.Add("CoreEntrance");

            Objective final = new Objective(ObjectiveType.Location, "Defeat final boss.", "Final Boss");
            final.targetBosses.Add("FinalBoss");

            objectives.Add(doubleJump);
            objectives.Add(raindropStation);
            objectives.Add(blaster);
            objectives.Add(armorBoss);
            objectives.Add(findHub);
            objectives.Add(findStorage);
            objectives.Add(magnetBoots);
            objectives.Add(primaryStorage);
            objectives.Add(snowBoss);
            objectives.Add(findEngine);
            objectives.Add(thruster);
            objectives.Add(missile);
            objectives.Add(suit);
            objectives.Add(command);
            objectives.Add(spinHook);
            objectives.Add(jetpack);
            objectives.Add(yellowCodes);
            objectives.Add(enterCPU);
            objectives.Add(armorCousins);
            objectives.Add(access);
            objectives.Add(core);
            objectives.Add(final);
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
                        if (d.type == VL.DoodadType.UpgradeStation && d.originalAbilityType == i)
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

        public static void UpdateObjectiveStatus(int gameTime)
        {
            oscillate += gameTime;
            if (oscillate > maxOscillate)
            {
                oscillate = 0;
            }

            if(objectiveMessageQueue.Count > 0 && DialogBox.state == BoxState.None)
                messageQueueTimer += gameTime;
            if (messageQueueTimer > 2000)
                messageQueueTimer = 2000;
            if (messageQueueTimer == 2000 && objectiveMessageQueue.Count > 0)
            {
                String nextMessage = objectiveMessageQueue[0];
                objectiveMessageQueue.RemoveAt(0);
                DialogBox.SetCustomDialog(nextMessage);
                messageQueueTimer = 0;
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
                if (!Engine.player.currentRoom.id.Contains("Menu"))
                {
                    //objectiveMessageQueue.Add(objectives[Engine.player.currentObjective].completionDialog);         
                    objectiveMessageQueue.Add("Objective Complete: " + objectives[Engine.player.currentObjective].text);           
                }
                Engine.player.currentObjective++;
                if (!Engine.player.currentRoom.id.Contains("Menu"))
                {
                    if (Engine.player.currentObjective < objectives.Count)
                    {
                        //objectiveMessageQueue.Add(objectives[Engine.player.currentObjective].startDialog);
                        objectiveMessageQueue.Add("Objective Added: " + objectives[Engine.player.currentObjective].text);
                    }
                }

            }
        }
    }
}

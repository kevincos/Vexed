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
    public class SoundFX
    {
        public static SoundEffect soundSwoosh;
        public static SoundEffect soundBloop;
        public static SoundEffect soundBeep;
        public static SoundEffect soundClick;
        public static SoundEffect soundThud;
        public static SoundEffect footstep;
        public static SoundEffect laserShot;
        public static SoundEffect blasterShot;
        public static SoundEffect missileShot;
        public static SoundEffect jetPack;
        public static SoundEffectInstance jetPackInstance;
        public static SoundEffectInstance boosterInstance;
        public static SoundEffect missileExplosion;
        public static SoundEffect laserHit;
        public static SoundEffect doorOpen;
        public static SoundEffect doorClose;
        public static SoundEffect hookLaunch;
        public static SoundEffect hookLock;
        public static SoundEffect jump;
        public static SoundEffect phase;
        public static SoundEffect doubleJump;
        public static SoundEffect bootSpin;

        public static SoundEffect armorDink;
        public static SoundEffect armorBreak;
        public static SoundEffect monsterHit;
        public static SoundEffect playerHit;
        public static SoundEffect monsterDeath;
        public static SoundEffect bridge;
        public static SoundEffect heal;
        public static SoundEffect wallSwitch;
        public static SoundEffect wallSwitchClick;

        public static SoundEffect flameOn;
        public static SoundEffect electricOn;
        public static SoundEffect flameOff;
        public static SoundEffect electricOff;
        public static SoundEffect steam;

        public static SoundEffect hologramFade;
        public static SoundEffect hologramUse;

        public static SoundEffect platformMove;
        public static SoundEffect platformStop;

        public static SoundEffect dialogCharacter;
        public static SoundEffect dialogExtend;

        public static SoundEffect activateSwitchStation;
        public static SoundEffect activateLaserSwitch;
        public static SoundEffect activatePlug;
        public static SoundEffect lockedSwitch;
        public static SoundEffect equipItem;
        public static SoundEffect equipError;
        public static SoundEffect stationPowerUp;

        public static SoundEffect mapSelect;
        public static SoundEffect inventoryWhoosh;
        public static SoundEffect mapWhoosh;

        public static SoundEffect flameAmbient;
        public static SoundEffect zapAmbient;

        public static SoundEffect brickBreak;

        public static SoundEffect tunnel;

        public static void Init(ContentManager content)
        {
            activateSwitchStation = content.Load<SoundEffect>("Sounds\\PlasmaHollow");
            equipItem = content.Load<SoundEffect>("Sounds\\PlasmaHollow");
            stationPowerUp = content.Load<SoundEffect>("Sounds\\PlantBomb1");
            SoundFX.soundSwoosh = content.Load<SoundEffect>("Sounds\\swoosh");
            SoundFX.soundBloop = content.Load<SoundEffect>("Sounds\\PlasmaHollow");
            SoundFX.soundClick = content.Load<SoundEffect>("Sounds\\click");
            SoundFX.soundBeep = content.Load<SoundEffect>("Sounds\\beep");
            SoundFX.soundThud = content.Load<SoundEffect>("Sounds\\thud");
            SoundFX.footstep = content.Load<SoundEffect>("Sounds\\footstep");
            SoundFX.laserShot = content.Load<SoundEffect>("Sounds\\LaserHot");
            SoundFX.blasterShot = content.Load<SoundEffect>("Sounds\\LaserShortSoft");
            SoundFX.missileShot = content.Load<SoundEffect>("Sounds\\MissileShot");
            SoundFX.jetPack = content.Load<SoundEffect>("Sounds\\Infernal");
            SoundFX.doorOpen = content.Load<SoundEffect>("Sounds\\DoorOpen");
            SoundFX.doorClose = content.Load<SoundEffect>("Sounds\\DoorClose");
            SoundFX.missileExplosion = content.Load<SoundEffect>("Sounds\\CannonPowder1");
            SoundFX.laserHit = content.Load<SoundEffect>("Sounds\\LaserShortHard");
            SoundFX.hookLaunch = content.Load<SoundEffect>("Sounds\\ArrowDeep");
            SoundFX.hookLock = content.Load<SoundEffect>("Sounds\\Mechanismus2");
            SoundFX.jump = content.Load<SoundEffect>("Sounds\\Jump");
            SoundFX.phase = content.Load<SoundEffect>("Sounds\\Teleport3");
            SoundFX.bootSpin = content.Load<SoundEffect>("Sounds\\Throw2");
            SoundFX.doubleJump = content.Load<SoundEffect>("Sounds\\Artillery2");
            SoundFX.armorBreak = content.Load<SoundEffect>("Sounds\\ShatterSteel");
            SoundFX.armorDink = content.Load<SoundEffect>("Sounds\\Armour2");
            SoundFX.monsterDeath = content.Load<SoundEffect>("Sounds\\PainGroan1");
            SoundFX.monsterHit = content.Load<SoundEffect>("Sounds\\ZapBuzzy");
            SoundFX.playerHit = content.Load<SoundEffect>("Sounds\\ZapZing");
            SoundFX.tunnel = content.Load<SoundEffect>("Sounds\\Throw1");
            SoundFX.bridge = content.Load<SoundEffect>("Sounds\\Throw3");
            SoundFX.heal = content.Load<SoundEffect>("Sounds\\SineUp");
            SoundFX.activateSwitchStation = content.Load<SoundEffect>("Sounds\\Switch4");
            SoundFX.activatePlug = content.Load<SoundEffect>("Sounds\\Switch3");
            SoundFX.activateLaserSwitch = content.Load<SoundEffect>("Sounds\\Switch2");
            SoundFX.wallSwitch = content.Load<SoundEffect>("Sounds\\Switch1");
            SoundFX.lockedSwitch = content.Load<SoundEffect>("Sounds\\Panel3");
            SoundFX.equipItem = content.Load<SoundEffect>("Sounds\\Ammopack1");
            SoundFX.wallSwitchClick = content.Load<SoundEffect>("Sounds\\Click2");
            SoundFX.flameOn = content.Load<SoundEffect>("Sounds\\DragonPlain");
            SoundFX.flameOff = content.Load<SoundEffect>("Sounds\\swoosh");
            SoundFX.electricOn = content.Load<SoundEffect>("Sounds\\ZapTuned");
            SoundFX.electricOff = content.Load<SoundEffect>("Sounds\\ZapBuzzy");
            SoundFX.steam = content.Load<SoundEffect>("Sounds\\ZapBuzzy");
            SoundFX.equipError = content.Load<SoundEffect>("Sounds\\ZapBuzzy");
            SoundFX.platformMove = content.Load<SoundEffect>("Sounds\\SlidingDoor2");
            SoundFX.platformStop = content.Load<SoundEffect>("Sounds\\Click2");
            SoundFX.hologramFade = content.Load<SoundEffect>("Sounds\\Throw4");
            SoundFX.hologramUse = content.Load<SoundEffect>("Sounds\\Switch4");
            SoundFX.dialogExtend = content.Load<SoundEffect>("Sounds\\Throw4");
            SoundFX.dialogCharacter = content.Load<SoundEffect>("Sounds\\TinyZap2");

            SoundFX.mapSelect = content.Load<SoundEffect>("Sounds\\Switch3");
            SoundFX.inventoryWhoosh = content.Load<SoundEffect>("Sounds\\DoorOpen");
            SoundFX.mapWhoosh = content.Load<SoundEffect>("Sounds\\Throw2");
            SoundFX.brickBreak = content.Load<SoundEffect>("Sounds\\CannonPowder2");

            SoundFX.flameAmbient = content.Load<SoundEffect>("Sounds\\Infernal");
            SoundFX.zapAmbient = content.Load<SoundEffect>("Sounds\\BeamThin");

            jetPackInstance = jetPack.CreateInstance();
            boosterInstance = jetPack.CreateInstance();
        }

        public static float ComputeVolume(Vector3 source)
        {
            float distance = (source - Engine.player.center.position).Length();
            if (distance > 15f) return 0f;
            if (distance < 3f) 
                return 1f;
            return (1f - (distance-3) / 12);
        }

        

        public static void EquipError()
        {
            if (Engine.soundEffectsEnabled)
                equipError.Play();
        }


        public static void DialogCharacter()
        {
            if (Engine.soundEffectsEnabled)
                dialogCharacter.Play(.1f, 0f,0f);
        }

        public static void MapSelect()
        {
            if (Engine.soundEffectsEnabled)
                mapSelect.Play();
        }



        public static void InventoryWhoosh()
        {
            if (Engine.soundEffectsEnabled)
                inventoryWhoosh.Play();
        }

        public static void MapWhoosh()
        {
            if (Engine.soundEffectsEnabled)
                mapWhoosh.Play();
        }

        public static void DialogExtend()
        {
            if (Engine.soundEffectsEnabled)
                dialogExtend.Play();
        }

        public static void ArmorHit()
        {
            if (Engine.soundEffectsEnabled)
                armorDink.Play();            
        }

        public static void ArmorBreak()
        {
            if (Engine.soundEffectsEnabled)
                armorBreak.Play();
        }

        public static void PlayerHit()
        {
            if (Engine.soundEffectsEnabled)
                playerHit.Play();
        }



        public static void MonsterHit()
        {
            if (Engine.soundEffectsEnabled)
                monsterHit.Play();
        }

        public static void BridgeWarp()
        {
            if (Engine.soundEffectsEnabled)
                bridge.Play();
        }

        public static void HologramFade()
        {
            if (Engine.soundEffectsEnabled)
                hologramFade.Play(.2f,0f,0f);
        }

        public static void HologramUse()
        {
            if (Engine.soundEffectsEnabled)
                hologramUse.Play();
        }


        public static void WallSwitch()
        {
            if (Engine.soundEffectsEnabled)
                wallSwitch.Play();
        }

        public static void Steam()
        {
            if (Engine.soundEffectsEnabled)
                steam.Play();
        }

        public static void BrickBreak(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                brickBreak.Play(ComputeVolume(location), 0, 0);
        }

        public static void PlatformMove(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                platformMove.Play(ComputeVolume(location), 0, 0);
        }

        public static void PlatformStop(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                platformStop.Play(ComputeVolume(location), 0, 0);
        }

        public static void FlameOn(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                flameOn.Play(ComputeVolume(location),0,0);
        }
        public static void FlameOff(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                flameOff.Play(ComputeVolume(location), 0, 0);
        }
        public static void ElectricOn(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                electricOn.Play(ComputeVolume(location), 0, 0);
        }
        public static void ElectricOff(Vector3 location)
        {
            //if (Engine.soundEffectsEnabled)
            //electricOff.Play(ComputeVolume(location),0,0);
        }

        public static void WallSwitchOff(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                wallSwitchClick.Play(ComputeVolume(location), 0, 0);
        }

        public static void MonsterDeath()
        {
            if (Engine.soundEffectsEnabled)
                monsterDeath.Play();
        }

        public static void PlayMove()
        {
            if (Engine.soundEffectsEnabled)
                soundSwoosh.Play();            
        }

        public static void Land()
        {
            if (Engine.soundEffectsEnabled)
                soundThud.Play();
        }

        public static void StartJetPack()
        {
            if (Engine.soundEffectsEnabled)
            {
                jetPackInstance.Play();
            }
        }

        public static void EndJetPack()
        {
            if (Engine.soundEffectsEnabled)
            {
                jetPackInstance.Stop();
            }
        }


        public static void Heal()
        {
            if (Engine.soundEffectsEnabled)
                heal.Play();
        }

        public static void StationPowerUp()
        {
            if (Engine.soundEffectsEnabled)
                stationPowerUp.Play();
        }

        public static void ActivateSwitchStation()
        {
            if (Engine.soundEffectsEnabled)
                activateLaserSwitch.Play();
        }

        public static void ActivateLaserSwitch()
        {
            if (Engine.soundEffectsEnabled)
                activateSwitchStation.Play();
        }

        public static void ActivatePlug()
        {
            if (Engine.soundEffectsEnabled)
                activatePlug.Play();
        }

        public static void LockedSwitch()
        {
            if (Engine.soundEffectsEnabled)
                lockedSwitch.Play();
        }

        public static void EquipItem()
        {
            if (Engine.soundEffectsEnabled)
                equipItem.Play();
        }

        public static void StartBooster()
        {
            if (Engine.soundEffectsEnabled)
            {
                boosterInstance.Play();
            }
        }

        public static void EndBooster()
        {
            if (Engine.soundEffectsEnabled)
            {
                boosterInstance.Stop();
            }
        }

        public static void CollectOrb()
        {
            if (Engine.soundEffectsEnabled)
                soundBloop.Play();
        }

        public static void RoomJump()
        {
            if (Engine.soundEffectsEnabled)
                soundSwoosh.Play();
        }


        public static void JumpSound()
        {
            if (Engine.soundEffectsEnabled)
                jump.Play();
        }

        public static void RocketJump()
        {
            if (Engine.soundEffectsEnabled)
                doubleJump.Play();
        }

        public static void BootSpin()
        {
            if (Engine.soundEffectsEnabled)
                bootSpin.Play();
        }

        public static void Phase()
        {
            if (Engine.soundEffectsEnabled)
                phase.Play();
        }

        public static void FireLaser(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                laserShot.Play(ComputeVolume(location), 0, 0);
        }

        public static void LaunchHook()
        {
            if (Engine.soundEffectsEnabled)
                hookLaunch.Play();
        }

        public static void HookLock()
        {
            if (Engine.soundEffectsEnabled)
                hookLock.Play();
        }

        public static void TunnelWoosh()
        {
            if (Engine.soundEffectsEnabled)
                tunnel.Play();
        }

        public static void OpenDoor(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                doorOpen.Play(ComputeVolume(location), 0, 0);
        }

        public static void CloseDoor(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                doorClose.Play(ComputeVolume(location), 0, 0);
        }

        public static void FireBlaster(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                blasterShot.Play(ComputeVolume(location), 0, 0);
        }

        public static void FireMissile(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                missileShot.Play(ComputeVolume(location), 0, 0);
        }

        public static void MissileExplode(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                missileExplosion.Play(ComputeVolume(location), 0, 0);
        }

        public static void BlasterExplode(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                laserHit.Play(ComputeVolume(location), 0, 0);
        }

        public static void LaserExplode(Vector3 location)
        {
            if (Engine.soundEffectsEnabled)
                laserHit.Play(ComputeVolume(location), 0, 0);
        }

        public static void PlayScore()
        {
            if (Engine.soundEffectsEnabled)
                soundBloop.Play();
        }

        public static void PlayClick()
        {
            if (Engine.soundEffectsEnabled)
                soundClick.Play();
        }

        public static void PlayAlert()
        {
            if (Engine.soundEffectsEnabled)
                soundBeep.Play();
        }

        public static void PlayFootstep()
        {
            if (Engine.soundEffectsEnabled)
                footstep.Play();
        }
    }
   
}

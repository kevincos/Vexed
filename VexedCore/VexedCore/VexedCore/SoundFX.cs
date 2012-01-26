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

        public static SoundEffect tunnel;

        public static void Init(ContentManager content)
        {
            SoundFX.soundSwoosh = content.Load<SoundEffect>("Sounds\\swoosh");
            SoundFX.soundBloop = content.Load<SoundEffect>("Sounds\\bloop");
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

            jetPackInstance = jetPack.CreateInstance();
            boosterInstance = jetPack.CreateInstance();
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

        public static void FireLaser()
        {
            if (Engine.soundEffectsEnabled)
                laserShot.Play();
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

        public static void OpenDoor()
        {
            if (Engine.soundEffectsEnabled)
                doorOpen.Play();
        }

        public static void CloseDoor()
        {
            if (Engine.soundEffectsEnabled)
                doorClose.Play();
        }

        public static void FireBlaster()
        {
            if (Engine.soundEffectsEnabled)
                blasterShot.Play();
        }

        public static void FireMissile()
        {
            if (Engine.soundEffectsEnabled)
                missileShot.Play();
        }

        public static void MissileExplode()
        {
            if (Engine.soundEffectsEnabled)
                missileExplosion.Play();
        }

        public static void BlasterExplode()
        {
            if (Engine.soundEffectsEnabled)
                laserHit.Play();
        }

        public static void LaserExplode()
        {
            if (Engine.soundEffectsEnabled)
                laserHit.Play();
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

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

        public static void Init(ContentManager content)
        {
            SoundFX.soundSwoosh = content.Load<SoundEffect>("Sounds\\swoosh");
            SoundFX.soundBloop = content.Load<SoundEffect>("Sounds\\bloop");
            SoundFX.soundClick = content.Load<SoundEffect>("Sounds\\click");
            SoundFX.soundBeep = content.Load<SoundEffect>("Sounds\\beep");
            SoundFX.soundThud = content.Load<SoundEffect>("Sounds\\thud");
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
    }
   
}

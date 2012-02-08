using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace VexedCore
{
    public class AmbientSound
    {
        public SoundEffectInstance instance;

        public SoundEffect effect;
        public float volume = 0;

        public AmbientSound(SoundEffect effect)
        {
            this.effect = effect;
            
        }

        public static float ComputeVolume(Vector3 source)
        {
            float distance = (source - Engine.player.center.position).Length();
            if (distance > 12f) return 0f;
            if (distance < 1f)
                return 1f;
            return (.5f - (distance - 1f) / 11f);
        }

        public void Update(int gameTime, Vector3 location, bool on)
        {
            
            volume = ComputeVolume(location);
            if (Engine.soundEffectsEnabled && on && volume > .01f)
            {
                if (instance == null)
                    instance = effect.CreateInstance();                
                instance.Volume = volume;
                instance.Play();
            }
            else if(instance != null)
            {
                instance.Stop(true);
                instance.Dispose();
                instance = null;
            }
            
        }        
    }
}

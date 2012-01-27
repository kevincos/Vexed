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
    public class DepthControl
    {
        public static int depthCount = 80;
        public static int depthTimer = 3000;
        public static bool depthTrigger = false;
        public static int oscillation = 0;
        public static float oscillationDir = 1f;
        public static bool uiFade = true;

        public static int maxOscillation
        {
            get
            {
                return 5000;
            }
        }

        public static float maxOscillationSpeed
        {
            get
            {

                return 15- 14f * Engine.player.naturalShield.ammo / Engine.player.naturalShield.maxAmmo;
            }
        }

        public static int maxDepthTimer
        {
            get
            {
                return 3000;                
            }
        }

        public static float effectiveDepthTimer
        {
            get
            {
                float val = depthTimer + oscillation / 11;
                if (val > maxDepthTimer)
                {
                    val = maxDepthTimer;
                    oscillationDir = -1f;
                }
                return val;
            }            
        }

        public static float DepthFactor
        {
            get
            {
                float depthFactor = (1f * effectiveDepthTimer / maxDepthTimer);
                depthFactor = (float)Math.Pow(depthFactor, .25);
                return depthFactor;
            }
        }

        public static void Update(int gameTime)
        {
            
            oscillation += (int)(gameTime * oscillationDir);
            if (oscillation > 3*maxOscillation/4) oscillationDir -= .05f * maxOscillationSpeed;
            if (oscillation < 1*maxOscillation/4) oscillationDir += .05f * maxOscillationSpeed;
            if (oscillation < 0)
                oscillation = 0;
            if (oscillationDir > maxOscillationSpeed) oscillationDir = maxOscillationSpeed;
            if (oscillationDir < -maxOscillationSpeed) oscillationDir = -maxOscillationSpeed;
            if (Engine.state != EngineState.Active || depthTrigger == false)
            {
                oscillationDir = -1;
                
            }

            if ((depthTrigger == false) && Engine.state == EngineState.Active)
            {
                depthTimer += 3 * gameTime;
                if (depthTimer > maxDepthTimer)
                    depthTimer = maxDepthTimer;
            }
            if (depthTrigger == true || Engine.state != EngineState.Active)
            {
                int targetDepthValue = 2500 - 1900 * Engine.player.naturalShield.ammo / Engine.player.naturalShield.maxAmmo;
                if (depthTimer < targetDepthValue)
                {
                    depthTimer += 3 * gameTime;
                    if (depthTimer > targetDepthValue) depthTimer = targetDepthValue;
                }
                if (depthTimer > targetDepthValue)
                {
                    uiFade = false;
                    depthTimer -= gameTime;
                    if (depthTimer < targetDepthValue) depthTimer = targetDepthValue;
                }

            }
        }

        public static void Draw()
        {
            Engine.spriteBatch.Begin();

            Byte maxRed = (Byte)(100 * oscillation / maxOscillation);
            if (Engine.player.naturalShield.ammo != 0)
                maxRed = 0;
            Color transparentBlack = new Color(maxRed, 0, 0, maxRed);

            Engine.spriteBatch.Draw(PauseMenu.pauseBackground, new Rectangle(Game1.graphicsDevice.Viewport.X, Game1.graphicsDevice.Viewport.Y, Game1.graphicsDevice.Viewport.Width, Game1.graphicsDevice.Viewport.Height), transparentBlack);
            Engine.spriteBatch.End();
        }
    }
}

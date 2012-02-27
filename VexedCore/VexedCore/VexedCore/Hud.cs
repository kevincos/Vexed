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
    public enum MeterState
    {
        Normal,
        Drain,
        Expand
    }

    public class Hud
    {
        public static Texture2D hudBase;
        public static Texture2D hudBaseInfo;
        public static Texture2D healthmeter_top;
        public static Texture2D healthmeter_bottom;
        public static Texture2D healthmeter_mid;
        public static Texture2D healthmeter_slot;
        public static Texture2D healthmeter_color;
        public static Texture2D expansion_top;
        public static Texture2D expansion_bottom;
        public static Texture2D expansion_red;
        public static Texture2D expansion_back;
        public static Texture2D expansion_front;

        public static Texture2D fuelMeter;
        public static Texture2D fuelCover;

        public static int displayHP = 0;
        public static int displayHealthCubes = 0;
        public static int displayMaxHP = 3;
        public static int displayMaxHealthCubes = 4;
        public static int HPanimateTime = 0;
        public static int maxHPAnimateTime = 100;
        public static int cubeAnimateTime = 0;
        public static int maxCubeAnimateTime = 300;
        public static int drainAnimateTime = 0;
        public static int maxDrainAnimateTime = 1000;
        public static int expandAnimateTime = 0;
        public static int maxExpandAnimateTime = 200;
        public static bool hidden = true;

        public static int hiddenOffset = 1000;

        public static MeterState healthMeterState = MeterState.Normal;

        public static void ResetHud()
        {
            displayMaxHealthCubes = Engine.player.redOrbsGoal;
            displayHealthCubes = Engine.player.redOrbsCollected;
            displayHP = Engine.player.naturalShield.ammo;
            displayMaxHP = Engine.player.naturalShield.maxAmmo;
        }

        public static void Draw()
        {
            Engine.spriteBatch.Begin();
            int w = (int)(Game1.titleSafeRect.Width *.45f);
            int rightEdge = Game1.titleSafeRect.Right;
            int hudLeft = Game1.titleSafeRect.Right - w + hiddenOffset;
            int topEdge = Game1.titleSafeRect.Top;
            Engine.spriteBatch.Draw(hudBase, new Rectangle(hudLeft, topEdge, w, w), Color.White);
            Engine.spriteBatch.Draw(hudBaseInfo, new Rectangle(hudLeft, topEdge, w, w), Color.White);

            int iconSize = (int)(.14f * w);
            int iconXOffset = (int)(.45f * w);
            int iconYOffset = (int)(.15f * w);
            Engine.spriteBatch.Draw(Doodad.leftButton, new Rectangle(hudLeft + iconXOffset, topEdge + iconYOffset, iconSize, iconSize), Color.Blue);
            Engine.spriteBatch.Draw(Doodad.rightButton, new Rectangle(hudLeft + iconXOffset + iconSize, topEdge + iconYOffset, iconSize, iconSize), Color.Yellow);
            if (Engine.player.primaryAbility.type == AbilityType.JetPack)
            {
                Engine.spriteBatch.Draw(fuelMeter, new Rectangle(hudLeft + iconXOffset, topEdge + iconYOffset, iconSize, iconSize), Color.White);
                int coverSize = iconSize * Engine.player.primaryAbility.ammo / Engine.player.primaryAbility.maxAmmo;
                Engine.spriteBatch.Draw(fuelCover, new Rectangle(hudLeft + iconXOffset, topEdge + iconYOffset + iconSize - coverSize, iconSize, coverSize), Color.White);                
            }
            if (Engine.player.secondaryAbility.type == AbilityType.JetPack)
            {
                Engine.spriteBatch.Draw(fuelMeter, new Rectangle(hudLeft + iconXOffset + iconSize, topEdge + iconYOffset, iconSize, iconSize), Color.White);
                int coverSize = iconSize * Engine.player.secondaryAbility.ammo / Engine.player.secondaryAbility.maxAmmo;
                Engine.spriteBatch.Draw(fuelCover, new Rectangle(hudLeft + iconXOffset + iconSize, topEdge + iconYOffset + iconSize - coverSize, iconSize, coverSize), Color.White);
            }
            Engine.spriteBatch.Draw(Ability.GetDecal(Engine.player.primaryAbility.type), new Rectangle(hudLeft + iconXOffset, topEdge + iconYOffset, iconSize, iconSize), Color.White);            
            Engine.spriteBatch.Draw(Ability.GetDecal(Engine.player.secondaryAbility.type), new Rectangle(hudLeft + iconXOffset + iconSize, topEdge + iconYOffset, iconSize, iconSize), Color.White);
            


            int cubeSize = (int)(.13f * w);
            int cubeXOffset = (int)(.90 * w);
            int cubeYOffset = (int)(.075f * w);
            int cubeYInc = (int)(.09f * w);

            //int maxHealthCubes = Engine.player.redOrbsGoal;
            

            if (healthMeterState == MeterState.Normal)
            {
                Engine.spriteBatch.Draw(expansion_back, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize, cubeSize, cubeYInc * displayMaxHealthCubes), Color.White);
                Engine.spriteBatch.Draw(expansion_red, new Rectangle(hudLeft + cubeXOffset,
                    topEdge + cubeYOffset + cubeSize + (displayMaxHealthCubes - displayHealthCubes) * cubeYInc,
                    cubeSize,
                    cubeYInc * displayHealthCubes), Color.White);
                Engine.spriteBatch.Draw(expansion_red, new Rectangle(hudLeft + cubeXOffset,
                    topEdge + cubeYOffset + cubeSize + (displayMaxHealthCubes - displayHealthCubes) * cubeYInc - cubeAnimateTime * cubeYInc / maxCubeAnimateTime,
                    cubeSize,
                    cubeAnimateTime * cubeYInc / maxCubeAnimateTime), Color.White);

                for (int i = 0; i < displayMaxHealthCubes; i++)
                {
                    Engine.spriteBatch.Draw(expansion_front, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize + i * cubeYInc, cubeSize, cubeYInc), Color.White);
                }
                Engine.spriteBatch.Draw(expansion_bottom, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize + displayMaxHealthCubes * cubeYInc, cubeSize, cubeSize), Color.White);
            }
            else if (healthMeterState == MeterState.Drain)
            {
                Engine.spriteBatch.Draw(expansion_back, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize, cubeSize, cubeYInc * displayMaxHealthCubes), Color.White);
                Engine.spriteBatch.Draw(expansion_red, new Rectangle(hudLeft + cubeXOffset,
                    topEdge + cubeYOffset + cubeSize,
                    cubeSize,
                    cubeYInc * displayHealthCubes - cubeYInc * displayHealthCubes * drainAnimateTime / maxDrainAnimateTime), Color.White);


                for (int i = 0; i < displayMaxHealthCubes; i++)
                {
                    Engine.spriteBatch.Draw(expansion_front, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize + i * cubeYInc, cubeSize, cubeYInc), Color.White);
                }
                Engine.spriteBatch.Draw(expansion_bottom, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize + displayMaxHealthCubes * cubeYInc, cubeSize, cubeSize), Color.White);
            }
            else if (healthMeterState == MeterState.Expand)
            {
                int growOffset = cubeYInc * expandAnimateTime / maxExpandAnimateTime;
                Engine.spriteBatch.Draw(expansion_back, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize, cubeSize, cubeYInc * displayMaxHealthCubes + growOffset), Color.White);
                for (int i = -1; i < displayMaxHealthCubes; i++)
                {
                    Engine.spriteBatch.Draw(expansion_front, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize + i * cubeYInc + growOffset, cubeSize, cubeYInc), Color.White);
                }
                Engine.spriteBatch.Draw(expansion_bottom, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset + cubeSize + displayMaxHealthCubes * cubeYInc + growOffset, cubeSize, cubeSize), Color.White);
            }
            Engine.spriteBatch.Draw(expansion_top, new Rectangle(hudLeft + cubeXOffset, topEdge + cubeYOffset, cubeSize, cubeSize), Color.White);
            


            int healthSize = (int)(.15f * w);
            int healthXOffset = (int)(.76 * w);
            int healthYOffset = (int)(.075f * w);


            if (healthMeterState == MeterState.Normal)
            {
                for (int i = 0; i < displayMaxHP; i++)
                {
                    Color healthColor = Color.Yellow;
                    if (displayMaxHP - i > displayHP)
                    {
                        healthColor = Color.Red;
                    }

                    if (displayMaxHP - i == displayHP + 1 && displayHP < Engine.player.naturalShield.ammo)
                    {
                        float fade = 1f * HPanimateTime / maxHPAnimateTime;
                        healthColor.R = (Byte)(Color.Yellow.R * fade + Color.Red.R * (1 - fade));
                        healthColor.G = (Byte)(Color.Yellow.G * fade + Color.Red.G * (1 - fade));
                        healthColor.B = (Byte)(Color.Yellow.B * fade + Color.Red.B * (1 - fade));
                    }
                    if (displayMaxHP - i == displayHP && displayHP > Engine.player.naturalShield.ammo)
                    {
                        float fade = 1f * HPanimateTime / maxHPAnimateTime;
                        healthColor.R = (Byte)(Color.Red.R * fade + Color.Yellow.R * (1 - fade));
                        healthColor.G = (Byte)(Color.Red.G * fade + Color.Yellow.G * (1 - fade));
                        healthColor.B = (Byte)(Color.Red.B * fade + Color.Yellow.B * (1 - fade));
                    }                  

                    Engine.spriteBatch.Draw(healthmeter_mid, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize, healthSize, healthSize), Color.White);
                    Engine.spriteBatch.Draw(healthmeter_slot, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize, healthSize, healthSize), Color.White);
                    Engine.spriteBatch.Draw(healthmeter_color, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize, healthSize, healthSize), healthColor);
                }
                Engine.spriteBatch.Draw(healthmeter_bottom, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (1 + displayMaxHP) * healthSize, healthSize, healthSize), Color.White);                
            }
            else if (healthMeterState == MeterState.Expand)
            {
                for (int i = 0; i < displayMaxHP; i++)
                {
                    Color healthColor = Color.Yellow;
                    if (displayMaxHP - i > displayHP)
                    {
                        healthColor = Color.Red;
                    }

                    if (displayMaxHP - i == displayHP + 1 && displayHP < Engine.player.naturalShield.ammo)
                    {
                        float fade = 1f * HPanimateTime / maxHPAnimateTime;
                        healthColor.R = (Byte)(Color.Yellow.R * fade + Color.Red.R * (1 - fade));
                        healthColor.G = (Byte)(Color.Yellow.G * fade + Color.Red.G * (1 - fade));
                        healthColor.B = (Byte)(Color.Yellow.B * fade + Color.Red.B * (1 - fade));
                    }
                    if (displayMaxHP - i == displayHP && displayHP > Engine.player.naturalShield.ammo)
                    {
                        float fade = 1f * HPanimateTime / maxHPAnimateTime;
                        healthColor.R = (Byte)(Color.Red.R * fade + Color.Yellow.R * (1 - fade));
                        healthColor.G = (Byte)(Color.Red.G * fade + Color.Yellow.G * (1 - fade));
                        healthColor.B = (Byte)(Color.Red.B * fade + Color.Yellow.B * (1 - fade));
                    }     

                    Engine.spriteBatch.Draw(healthmeter_mid, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize, healthSize, healthSize), Color.White);
                    if (i == 0)
                    {
                        Engine.spriteBatch.Draw(healthmeter_slot, new Rectangle(hudLeft + healthXOffset + healthSize / 2 - healthSize * expandAnimateTime / maxExpandAnimateTime / 2,
                            topEdge + healthYOffset + (i + 1) * healthSize + healthSize /2 - healthSize * expandAnimateTime / maxExpandAnimateTime / 2, 
                            healthSize * expandAnimateTime / maxExpandAnimateTime, 
                            healthSize * expandAnimateTime / maxExpandAnimateTime), Color.White);
                        Engine.spriteBatch.Draw(healthmeter_color, new Rectangle(hudLeft + healthXOffset + healthSize / 2 - healthSize * expandAnimateTime / maxExpandAnimateTime / 2,
                            topEdge + healthYOffset + (i + 1) * healthSize + healthSize / 2 - healthSize * expandAnimateTime / maxExpandAnimateTime / 2,
                            healthSize * expandAnimateTime / maxExpandAnimateTime,
                            healthSize * expandAnimateTime / maxExpandAnimateTime), healthColor);                        
                    }
                    else
                    {
                        Engine.spriteBatch.Draw(healthmeter_slot, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize, healthSize, healthSize), Color.White);
                        Engine.spriteBatch.Draw(healthmeter_color, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize, healthSize, healthSize), healthColor);
                    }
                }
                Engine.spriteBatch.Draw(healthmeter_bottom, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (1 + displayMaxHP) * healthSize, healthSize, healthSize), Color.White);
            }
            else if (healthMeterState == MeterState.Drain)
            {
                int growOffset = healthSize * drainAnimateTime / maxDrainAnimateTime;
                Engine.spriteBatch.Draw(healthmeter_mid, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + growOffset, healthSize, healthSize), Color.White);
                for (int i = 0; i < displayMaxHP; i++)
                {
                    Color healthColor = Color.Yellow;
                    if (displayMaxHP - i > displayHP)
                        healthColor = Color.Red;

                    if (displayMaxHP - i == displayHP + 1 && displayHP < Engine.player.naturalShield.ammo)
                    {
                        float fade = 1f * HPanimateTime / maxHPAnimateTime;
                        healthColor.R = (Byte)(Color.Yellow.R * fade + Color.Red.R * (1 - fade));
                        healthColor.G = (Byte)(Color.Yellow.G * fade + Color.Red.G * (1 - fade));
                        healthColor.B = (Byte)(Color.Yellow.B * fade + Color.Red.B * (1 - fade));
                    }
                    if (displayMaxHP - i == displayHP && displayHP > Engine.player.naturalShield.ammo)
                    {
                        float fade = 1f * HPanimateTime / maxHPAnimateTime;
                        healthColor.R = (Byte)(Color.Red.R * fade + Color.Yellow.R * (1 - fade));
                        healthColor.G = (Byte)(Color.Red.G * fade + Color.Yellow.G * (1 - fade));
                        healthColor.B = (Byte)(Color.Red.B * fade + Color.Yellow.B * (1 - fade));
                    }     

                    Engine.spriteBatch.Draw(healthmeter_mid, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize + growOffset, healthSize, healthSize), Color.White);
                    Engine.spriteBatch.Draw(healthmeter_slot, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize + growOffset, healthSize, healthSize), Color.White);
                    Engine.spriteBatch.Draw(healthmeter_color, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (i + 1) * healthSize + growOffset, healthSize, healthSize), healthColor);
                }
                Engine.spriteBatch.Draw(healthmeter_bottom, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset + (1 + displayMaxHP) * healthSize + growOffset, healthSize, healthSize), Color.White); 

            }
            Engine.spriteBatch.Draw(healthmeter_top, new Rectangle(hudLeft + healthXOffset, topEdge + healthYOffset, healthSize, healthSize), Color.White);


            Engine.spriteBatch.End();
        }

        public static void Update(int gameTime)
        {


            if (hidden && hiddenOffset < 1000)
            {
                hiddenOffset += gameTime;
                if (hiddenOffset > 1000) hiddenOffset = 1000;
            }
            if (hidden == false && hiddenOffset > 0)
            {
                hiddenOffset -= gameTime;
                if (hiddenOffset < 0) hiddenOffset = 0;
            }

            if (healthMeterState == MeterState.Normal)
            {
                if (displayHP == Engine.player.naturalShield.ammo)
                    HPanimateTime = 0;
                else
                    HPanimateTime += gameTime;

                if (displayHP < Engine.player.naturalShield.ammo && HPanimateTime > maxHPAnimateTime)
                {
                    HPanimateTime = 0;
                    displayHP++;
                }
                if (displayHP > Engine.player.naturalShield.ammo && HPanimateTime > maxHPAnimateTime)
                {
                    HPanimateTime = 0;
                    displayHP--;
                }

                if (displayHealthCubes == Engine.player.redOrbsCollected)
                {
                    cubeAnimateTime = 0;
                }
                else
                {
                    cubeAnimateTime += gameTime;
                    if (cubeAnimateTime > maxCubeAnimateTime)
                    {
                        cubeAnimateTime = 0;
                        displayHealthCubes++;
                        if (displayHealthCubes == displayMaxHealthCubes)
                        {
                            healthMeterState = MeterState.Drain;
                        }                        
                    }
                }
            }
            else if (healthMeterState == MeterState.Drain)
            {
                drainAnimateTime += gameTime;
                if (drainAnimateTime > maxDrainAnimateTime)
                {
                    drainAnimateTime = 0;
                    healthMeterState = MeterState.Expand;
                    displayMaxHP = Engine.player.naturalShield.maxAmmo;
                }
            }
            else if (healthMeterState == MeterState.Expand)
            {
                expandAnimateTime += gameTime;
                if (expandAnimateTime > maxExpandAnimateTime)
                {
                    expandAnimateTime = 0;
                    healthMeterState = MeterState.Normal;
                    displayMaxHealthCubes = Engine.player.redOrbsGoal;
                    displayHealthCubes = Engine.player.redOrbsCollected;
                }
            }

            
            
        }
    }
}

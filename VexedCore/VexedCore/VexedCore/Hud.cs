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
    public class Hud
    {
        public static void Draw()
        {
            Engine.spriteBatch.Begin();
            if (Engine.controlType == ControlType.GamePad)
            {
                if (Engine.player.secondaryAbility.isPassive == false)
                    Ability.Draw(.825f, .02f, AbilityType.YButton);
                else
                    Ability.Draw(.825f, .02f, AbilityType.Passive);

                if (Engine.player.primaryAbility.isPassive == false)
                    Ability.Draw(.75f, .12f, AbilityType.XButton);
                else
                    Ability.Draw(.75f, .12f, AbilityType.Passive);
                Ability.Draw(.825f, .22f, AbilityType.AButton);
                Ability.Draw(.9f, .12f, AbilityType.BButton);

                Engine.player.secondaryAbility.Draw(.825f, .02f);
                Engine.player.primaryAbility.Draw(.75f, .12f);
                Ability.Draw(.825f, .22f, AbilityType.NormalJump, Engine.player.naturalShield.ammo, Engine.player.naturalShield.maxAmmo);
                Ability.Draw(.9f, .12f, AbilityType.Use);
            }
            if (Engine.controlType == ControlType.MouseAndKeyboard)
            {
                if (Engine.player.secondaryAbility.isPassive == false)
                    Ability.Draw(.825f, .06f, AbilityType.RightMouse, Color.Yellow, .1f);
                else
                    Ability.Draw(.825f, .06f, AbilityType.RightMouse);

                if (Engine.player.primaryAbility.isPassive == false)
                    Ability.Draw(.75f, .06f, AbilityType.LeftMouse, Color.Blue, .1f);
                else
                    Ability.Draw(.75f, .06f, AbilityType.LeftMouse);
                
                Engine.player.secondaryAbility.Draw(.825f, .06f);
                Engine.player.primaryAbility.Draw(.75f, .06f);

                for (int i = 0; i < Engine.player.naturalShield.maxAmmo; i++)
                {
                    Color healthColor = Color.Yellow;
                    if (Engine.player.naturalShield.maxAmmo - i > Engine.player.naturalShield.ammo)
                        healthColor = Color.Red;

                    Ability.Draw(.92f, .1f + i * .085f, AbilityType.YButton, healthColor, .05f);
                }
                for (int i = 0; i < 5; i++)
                {
                    Color healthColor = Color.White;
                    if (Engine.player.redOrbsCollected % 5 > i)
                        healthColor = Color.Red;

                    Ability.Draw(.965f, .1f + i * .05f, AbilityType.PowerCube, healthColor, .04f);
                }
                
            }

            if (Engine.controlType == ControlType.KeyboardOnly)
            {
                if (Engine.player.secondaryAbility.isPassive == false)
                {
                    if(Engine.player.secondaryAbility.type != AbilityType.Empty)
                        Ability.Draw(.85f, .06f, AbilityType.KeyGeneric, Color.Yellow, .1f);
                    else
                        Ability.Draw(.85f, .06f, AbilityType.ZKey, Color.Yellow, .1f);
                }
                else
                {
                    if (Engine.player.secondaryAbility.type != AbilityType.Empty)
                        Ability.Draw(.85f, .06f, AbilityType.KeyGeneric, Color.White, .1f);
                    else
                        Ability.Draw(.85f, .06f, AbilityType.ZKey, Color.White, .1f);
                }

                if (Engine.player.primaryAbility.isPassive == false)
                {
                    if(Engine.player.primaryAbility.type != AbilityType.Empty)
                        Ability.Draw(.75f, .06f, AbilityType.KeyGeneric, Color.Blue, .1f);
                    else
                        Ability.Draw(.75f, .06f, AbilityType.XKey, Color.Blue, .1f);
                }
                else
                {
                    if(Engine.player.primaryAbility.type != AbilityType.Empty)
                        Ability.Draw(.75f, .06f, AbilityType.KeyGeneric, Color.White, .1f);
                    else
                        Ability.Draw(.75f, .06f, AbilityType.XKey, Color.White, .1f);
                }

                Engine.player.secondaryAbility.Draw(.85f, .06f);
                Engine.player.primaryAbility.Draw(.75f, .06f);

                for (int i = 0; i < Engine.player.naturalShield.maxAmmo; i++)
                {
                    Color healthColor = Color.Yellow;
                    if (Engine.player.naturalShield.maxAmmo - i > Engine.player.naturalShield.ammo)
                        healthColor = Color.Red;

                    Ability.Draw(.9f, .2f + i * .085f, AbilityType.YButton, healthColor, .05f);
                }

            }
            Engine.spriteBatch.End();
        }
    }
}

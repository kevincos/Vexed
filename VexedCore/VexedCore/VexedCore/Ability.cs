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
    public enum AbilityType
    {
        Empty = 31,
        Blaster = 19,
        Missile = 16,
        Laser = 17,
        Bomb = 18,
        WallJump = 0,
        DoubleJump = 1,
        JetPack = 9,
        Boots = 10,
        Booster = 8,
        Shield = 4,
        RedKey = 11,
        BlueKey = 13,
        YellowKey = 12,
        NormalJump = 2,
        Use = 3,
        BButton = 24,
        XButton = 25,
        AButton = 26,
        YButton = 27,
        Passive = 28,
        PermanentBoots = 5,
        PermanentWallJump = 6,
        SpinHook = 7,
        PermanentYellowKey = 14,
        PermanentBlueKey = 15,
        PermanentRedKey = 20,
        Phase = 21,
        ImprovedJump = 22,
        Ultima = 23

    }

    public class Ability
    {
        public AbilityType type;

        public int ammo = 0;        
        public int cooldown = 0;
        public int duration = 0;

        public bool isPassive
        {
            get
            {
                return type == AbilityType.WallJump || type == AbilityType.DoubleJump || type == AbilityType.Shield || type == AbilityType.Boots || type == AbilityType.BlueKey || type == AbilityType.RedKey || type == AbilityType.YellowKey;
            }
        }

        public bool isItem
        {
            get
            {
                return type == AbilityType.WallJump || type == AbilityType.DoubleJump || type == AbilityType.Boots || type == AbilityType.JetPack || type == AbilityType.Blaster || type == AbilityType.Laser || type == AbilityType.Missile || type == AbilityType.Booster || type == AbilityType.Phase || type == AbilityType.SpinHook;
            }
        }

        public bool isUpgrade
        {
            get
            {
                return type == AbilityType.PermanentBlueKey || type == AbilityType.PermanentBoots || type == AbilityType.PermanentRedKey || type == AbilityType.PermanentWallJump || type == AbilityType.PermanentYellowKey || type == AbilityType.ImprovedJump;
            }
        }

        public bool isGun
        {
            get
            {
                return type == AbilityType.Blaster || type == AbilityType.Laser || type == AbilityType.Missile || type == AbilityType.Bomb || type == AbilityType.SpinHook;
            }
        }

        public bool isBoots
        {
            get
            {
                return type == AbilityType.DoubleJump || type == AbilityType.Boots || type == AbilityType.WallJump;
            }
        }

        public int maxAmmo
        {
            get
            {
                if (type == AbilityType.Shield)
                    return 2000;
                if(type == AbilityType.JetPack)
                    return 2000;
                return 0;
            }
        }

        public int maxDuration
        {
            get
            {
                return 1000;
            }
        }

        public int maxCooldown
        {
            get
            {
                if (type == AbilityType.Booster)
                    return 700;
                if (type == AbilityType.JetPack)
                    return 0;
                if (type == AbilityType.SpinHook)
                    return 100;
                return 200;
            }
        }

        public static Texture2D ability_textures;
        public static List<List<Vector2>> texCoordList;

        public static int texGridCount = 8;

        public static List<Vector2> LoadTexCoords(int x, int y)
        {
            float texWidth = 1f / texGridCount;
            List<Vector2> texCoords = new List<Vector2>();
            texCoords.Add(new Vector2((x + 1) * texWidth, y * texWidth));
            texCoords.Add(new Vector2(x * texWidth, y * texWidth));
            texCoords.Add(new Vector2(x * texWidth, (y + 1) * texWidth));
            texCoords.Add(new Vector2((x + 1) * texWidth, (y + 1) * texWidth));

            return texCoords;
        }
        public static void InitTexCoords()
        {
            texCoordList = new List<List<Vector2>>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    texCoordList.Add(LoadTexCoords(x, y));
                }
            }
        }

        public Ability()
        {
            type = AbilityType.Empty;
        }

        public Ability(AbilityType type)
        {
            this.type = type;
            if (type == AbilityType.JetPack)
            {
                ammo = maxAmmo;
            }
            if (type == AbilityType.Shield)
            {
                ammo = maxAmmo;
            }
        }

        public Ability(Ability a)
        {
            ammo = a.ammo;
            cooldown = a.cooldown;
            type = a.type;
            if (type == AbilityType.JetPack)
            {
                ammo = maxAmmo;
            }
        }

        public void AddAmmo(int value)
        {
            ammo += value;
            if (ammo > maxAmmo)
                ammo = maxAmmo;
        }

        public void DepleteAmmo(int value)
        {
            ammo -= value;
            if (ammo < 0)
                ammo = 0;
        }

        public void Update(GameTime gameTime)
        {
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown < 0) cooldown = 0;

            if (type == AbilityType.DoubleJump)
            {
                if (Engine.player.grounded == true)
                {
                    ammo = 1;
                }
            }
            if (type == AbilityType.JetPack)
            {
                if (Engine.player.grounded == true)
                {
                    ammo += gameTime.ElapsedGameTime.Milliseconds;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                }
            }

        }

        public void Do(GameTime gameTime)
        {
            if (cooldown == 0)
            {
                cooldown = maxCooldown;
                Vector3 shootDirection;
                if (Engine.player.grounded == false && Engine.player.leftWall == true)
                    shootDirection = Engine.player.right / Engine.player.right.Length();
                else if (Engine.player.grounded == false && Engine.player.rightWall == true)
                    shootDirection = -Engine.player.right / Engine.player.right.Length();
                else if (Engine.player.faceDirection >= 0)
                    shootDirection = Engine.player.right / Engine.player.right.Length();
                else
                    shootDirection = -Engine.player.right / Engine.player.right.Length();

                if (type == AbilityType.SpinHook)
                     Engine.player.SpinHook();
                if (type == AbilityType.Blaster)
                    Engine.player.currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Player, Engine.player.center.position, Engine.player.center.velocity, Engine.player.center.normal, shootDirection));
                if (type == AbilityType.Missile)
                    Engine.player.currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Missile, Engine.player.center.position + .5f * shootDirection, Engine.player.center.velocity, Engine.player.center.normal, shootDirection));
                if (type == AbilityType.Bomb)
                    Engine.player.currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Bomb, Engine.player.center.position + .5f * shootDirection, Engine.player.center.velocity, Engine.player.center.normal, shootDirection));
                if (type == AbilityType.Laser)
                    Engine.player.currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Laser, Engine.player.center.position, Engine.player.center.velocity, Engine.player.center.normal, shootDirection));

                if (type == AbilityType.Laser || type == AbilityType.Blaster || type == AbilityType.Missile || type == AbilityType.Bomb)
                    Engine.player.lastFireTime = 0;

                if (type == AbilityType.Phase)
                {
                    Engine.player.AttemptPhase();
                }
                if (type == AbilityType.Booster)
                {
                    Engine.player.Boost();               
                }
                if (type == AbilityType.JetPack)
                {
                    if (ammo > 0)
                    {
                        Engine.player.jetPacking = true;
                        Engine.player.jetPackThrust = true;
                        Engine.player.center.velocity += .002f * Engine.player.up;
                    }
                    ammo -= gameTime.ElapsedGameTime.Milliseconds;
                    if (ammo < 0) ammo = 0;
                }
            }
        }

        public void Draw(float xPercent, float yPercent)
        {
            Draw(xPercent, yPercent, type, ammo, maxAmmo);
        }

        public static void Draw(float xPercent, float yPercent, AbilityType type)
        {
            Draw(xPercent, yPercent, type, 0, 0);
        }

        public static void Draw(float xPercent, float yPercent, AbilityType type, int ammo, int maxAmmo)
        {
            float iconSize = .1f;
            float barWidth = .05f;
            float barHeight = .08f;
            int spriteX = (int)(type) % 8;
            int spriteY = (int)(type) / 8;
            
            Engine.spriteBatch.Begin();
            Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + xPercent * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)(iconSize * Game1.titleSafeRect.Width), (int)(iconSize * Game1.titleSafeRect.Width)), new Rectangle(128*spriteX, 128*spriteY, 128, 128), Color.White);
            if (type == AbilityType.JetPack)
            {
                Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + (xPercent + .06) * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)(barWidth * Game1.titleSafeRect.Width), (int)(barHeight * Game1.titleSafeRect.Width)), new Rectangle(128 * 5, 128 * 3, 128, 128), Color.White);
                Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + (xPercent + .06) * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height + (barHeight * Game1.titleSafeRect.Width - (int)((1f * ammo / maxAmmo) * barHeight * Game1.titleSafeRect.Width))), (int)(barWidth * Game1.titleSafeRect.Width), (int)((1f * ammo / maxAmmo) * barHeight * Game1.titleSafeRect.Width)), new Rectangle(128 * 6, 128 * 3, 128, 128), Color.White);
                
            }
            if (type == AbilityType.Shield || (type == AbilityType.NormalJump && maxAmmo != 0))
            {
                Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + (xPercent + .06) * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)(barWidth * Game1.titleSafeRect.Width), (int)(barHeight * Game1.titleSafeRect.Width)), new Rectangle(128 * 5, 128 * 3, 128, 128), Color.White);
                Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + (xPercent + .06) * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height + (barHeight * Game1.titleSafeRect.Width - (int)((1f * ammo / maxAmmo) * barHeight * Game1.titleSafeRect.Width))), (int)(barWidth * Game1.titleSafeRect.Width), (int)((1f * ammo / maxAmmo) * barHeight * Game1.titleSafeRect.Width)), new Rectangle(128 * 6, 128 * 3, 128, 128), Color.White);

            }
            Engine.spriteBatch.End();
        }

        public String FriendlyName()
        {
            if (type == AbilityType.DoubleJump)
                return "Double Jump";
            if (type == AbilityType.WallJump)
                return "Wall Jump";
            if (type == AbilityType.Blaster)
                return "Blaster Gun";
            if (type == AbilityType.Laser)
                return "Laser Cannon";
            if (type == AbilityType.Missile)
                return "Missile Launcher";
            if (type == AbilityType.Boots)
                return "Gravity Boots";
            if (type == AbilityType.Booster)
                return "Rocket Booster";
            if (type == AbilityType.JetPack)
                return "Jet Pack";
            if (type == AbilityType.SpinHook)
                return "Spin Hook";
            if (type == AbilityType.Phase)
                return "Phase Belt";
            if (type == AbilityType.RedKey)
                return "Red Access Card";
            if (type == AbilityType.BlueKey)
                return "Blue Access Card";
            if (type == AbilityType.YellowKey)
                return "Yellow Access Card";
            if (type == AbilityType.PermanentRedKey)
                return "Red Security Codes";
            if (type == AbilityType.PermanentBlueKey)
                return "Blue Security Codes";
            if (type == AbilityType.PermanentYellowKey)
                return "Yellow Security Codes";
            if (type == AbilityType.PermanentBoots)
                return "Gravity Module";
            if (type == AbilityType.PermanentWallJump)
                return "Wall Jump Module";
            if (type == AbilityType.ImprovedJump)
                return "Advanced Jump";
            return "BUG";
        }

        public String Description()
        {
            if (type == AbilityType.DoubleJump)
                return "Press the Jump button while in mid air\nto get a second jump.";
            if (type == AbilityType.WallJump)
                return "Press the Jump button while sliding on\na wall to jump off the wall.";
            if (type == AbilityType.Blaster)
                return "Short range energy weapon.";
            if (type == AbilityType.Laser)
                return "Long range energy weapon. Can be used\nto power up certain switches.";
            if (type == AbilityType.Missile)
                return "Explosive homing weapon. Can destroy\nenemy armor and certain blocks.";
            if (type == AbilityType.Boots)
                return "Allows you to walk on special gravity\nwalls.";
            if (type == AbilityType.Booster)
                return "Gives you a powerful horizontal turbo\nboost.";
            if (type == AbilityType.JetPack)
                return "Allows you to fly for a short period\noftime.";
            if (type == AbilityType.SpinHook)
                return "Fire at hook targets to shift gravity.";
            if (type == AbilityType.Phase)
                return "Allows you to teleport through to the\nfar side of the room.";
            if (type == AbilityType.RedKey)
                return "Allows you to activate Red switches.";
            if (type == AbilityType.BlueKey)
                return "Allows yout to activate Blue switches.";
            if (type == AbilityType.YellowKey)
                return "Allows you to activate Yellow switches.";
            if (type == AbilityType.PermanentRedKey)
                return "Allows you to activate Red switches\nwithout an access card.";
            if (type == AbilityType.PermanentBlueKey)
                return "Allows you to activate Blue switches\nwithout an access card.";
            if (type == AbilityType.PermanentYellowKey)
                return "Allows you to activate Yellow switches\nwithout an access card.";
            if (type == AbilityType.PermanentBoots)
                return "Allows you to walk on gravity strips\nwithout using an inventory slot.";
            if (type == AbilityType.PermanentWallJump)
                return "Allows you to perform wall jumps without\nusing an inventory slot.";
            if (type == AbilityType.ImprovedJump)
                return "Increases maximum jump height.";
            return "BUG";
        }
    }

    
}

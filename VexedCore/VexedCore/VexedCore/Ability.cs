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
        Empty = 29,
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
        PermanentShield = 7,
        PermanentYellowKey = 14,
        PermanentBlueKey = 15,
        PermanentRedKey = 20,
        DualWield = 21,
        ImprovedJump = 22,
        Ultima = 23

    }

    public class Ability
    {
        public AbilityType type;

        public int ammo = 0;        
        public int cooldown = 0;
        public int duration = 0;

        public bool isGun
        {
            get
            {
                return type == AbilityType.Blaster || type == AbilityType.Laser || type == AbilityType.Missile || type == AbilityType.Bomb;
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
                    return 600;
                if (type == AbilityType.JetPack)
                    return 0;
                return 400;
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
    }

    
}

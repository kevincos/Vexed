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
        Ultima = 23,
        LeftMouse = 46,
        RightMouse = 47,
        EKey= 48,
        ZKey = 49,
        CKey = 50,
        XKey = 51,
        MKey = 52,
        KeyGeneric = 53,
        CtrlKey = 54,
        Health = 55,
        PowerCube = 37
    }

    public class Ability
    {
        public AbilityType type;

        public int ammo = 0;
        public int _maxAmmo = -1;
        public int cooldown = 0;
        public int duration = 0;

        public bool isPassive
        {
            get
            {
                return type == AbilityType.WallJump || type == AbilityType.DoubleJump || type == AbilityType.Shield || type == AbilityType.Boots || type == AbilityType.BlueKey || type == AbilityType.RedKey || type == AbilityType.YellowKey;
            }
        }

        public bool isJump
        {
            get
            {
                return type == AbilityType.WallJump || type == AbilityType.DoubleJump;
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
                if (_maxAmmo == -1)
                {
                    if (type == AbilityType.Shield)
                        _maxAmmo = 3;
                    else if (type == AbilityType.JetPack)
                        _maxAmmo = 2000;
                    else
                        _maxAmmo = 0;
                }
                return _maxAmmo;                
            }
            set
            {
                _maxAmmo = value;
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

        //public static Texture2D ability_textures;
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
            _maxAmmo = a._maxAmmo;
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

        public void Update(int gameTime)
        {
            cooldown -= gameTime;
            if (cooldown < 0) cooldown = 0;

            if (type == AbilityType.DoubleJump)
            {
                if (Engine.player.grounded == true && Engine.player.flashTime == 0)
                {
                    if(ammo == 0)
                        ammo = 1;
                }
            }
            if (type == AbilityType.JetPack)
            {
                if (Engine.player.grounded == true && Engine.player.flashTime == 0)
                {
                    ammo += gameTime;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                }
            }

        }

        public void Do(int gameTime)
        {
            if (cooldown == 0)
            {
                if (Engine.player.faceDirection == 0)
                    Engine.player.faceDirection = 1;
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

                Vector3 relVelocity = Engine.player.center.velocity - Vector3.Dot(Engine.player.platformVelocity,Engine.player.right) * Engine.player.right;
                if (type == AbilityType.SpinHook)
                {                    
                    Engine.player.SpinHook();
                }
                if (type == AbilityType.Blaster)
                {
                    SoundFX.FireBlaster(Engine.player.center.position);
                    Engine.player.currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Player, Engine.player.center.position, relVelocity, Engine.player.center.normal, shootDirection));
                }
                if (type == AbilityType.Missile)
                {
                    SoundFX.FireMissile(Engine.player.center.position);
                    Engine.player.currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Missile, Engine.player.center.position + .5f * shootDirection, relVelocity, Engine.player.center.normal, shootDirection));
                }
                if (type == AbilityType.Bomb)
                    Engine.player.currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Bomb, Engine.player.center.position + .5f * shootDirection, relVelocity, Engine.player.center.normal, shootDirection));
                if (type == AbilityType.Laser)
                {
                    SoundFX.FireLaser(Engine.player.center.position);
                    Engine.player.currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Laser, Engine.player.center.position, relVelocity, Engine.player.center.normal, shootDirection));
                }

                if (type == AbilityType.Laser || type == AbilityType.Blaster || type == AbilityType.Missile || type == AbilityType.Bomb)
                    Engine.player.lastFireTime = 0;

                if (type == AbilityType.Phase)
                {
                    Engine.player.AttemptPhase();
                }
                if (type == AbilityType.Booster)
                {
                    SoundFX.StartBooster();
                    Engine.player.Boost();               
                }
                if (type == AbilityType.JetPack)
                {
                    if (ammo > 0)
                    {
                        Engine.player.jetPacking = true;
                        Engine.player.jetPackThrust = true;
                        Engine.player.center.velocity += .002f * Engine.player.up;
                        SoundFX.StartJetPack();
                    }
                    else
                        SoundFX.EndJetPack();
                    ammo -= gameTime;
                    if (ammo < 0) ammo = 0;
                }
            }
        }

        public static Texture2D GetDecal(AbilityType abilityType)
        {
            if (abilityType == AbilityType.RedKey)
                return Doodad.decalTextures[(int)Decal.RedKey];
            if (abilityType == AbilityType.BlueKey)
                return Doodad.decalTextures[(int)Decal.BlueKey];
            if (abilityType == AbilityType.YellowKey)
                return Doodad.decalTextures[(int)Decal.YellowKey];
            if (abilityType == AbilityType.Laser)
                return Doodad.decalTextures[(int)Decal.Laser];
            if (abilityType == AbilityType.Blaster)
                return Doodad.decalTextures[(int)Decal.Blaster];
            if (abilityType == AbilityType.Boots)
                return Doodad.decalTextures[(int)Decal.Boots];
            if (abilityType == AbilityType.JetPack)
                return Doodad.decalTextures[(int)Decal.JetPack];
            if (abilityType == AbilityType.Phase)
                return Doodad.decalTextures[(int)Decal.Phase];
            if (abilityType == AbilityType.Booster)
                return Doodad.decalTextures[(int)Decal.Booster];
            if (abilityType == AbilityType.DoubleJump)
                return Doodad.decalTextures[(int)Decal.DoubleJump];
            if (abilityType == AbilityType.WallJump)
                return Doodad.decalTextures[(int)Decal.WallJump];
            if (abilityType == AbilityType.SpinHook)
                return Doodad.decalTextures[(int)Decal.HookTarget];
            if (abilityType == AbilityType.Missile)
                return Doodad.decalTextures[(int)Decal.Missile];
            if (abilityType == AbilityType.PermanentBoots)
                return Doodad.decalTextures[(int)Decal.PermanantBoots];
            if (abilityType == AbilityType.PermanentWallJump)
                return Doodad.decalTextures[(int)Decal.PermanantWallJump];
            if (abilityType == AbilityType.ImprovedJump)
                return Doodad.decalTextures[(int)Decal.ImprovedJump];
            if (abilityType == AbilityType.PermanentRedKey)
                return Doodad.decalTextures[(int)Decal.RedCodes];
            if (abilityType == AbilityType.PermanentBlueKey)
                return Doodad.decalTextures[(int)Decal.BlueCodes];
            if (abilityType == AbilityType.PermanentYellowKey)
                return Doodad.decalTextures[(int)Decal.YellowCodes];
            return Doodad.decalTextures[(int)Decal.Empty];
        }

        /*
        public void Draw(float xPercent, float yPercent)
        {
            Draw(xPercent, yPercent, type, ammo, maxAmmo, Color.White, .1f);
        }

        public static void Draw(float xPercent, float yPercent, AbilityType type)
        {
            Draw(xPercent, yPercent, type, 0, 0, Color.White, .1f);
        }

        public static void Draw(float xPercent, float yPercent, AbilityType type, float size)
        {
            Draw(xPercent, yPercent, type, 0, 0, Color.White, size);
        }

        public static void Draw(float xPercent, float yPercent, AbilityType type, Color c, float size)
        {
            Draw(xPercent, yPercent, type, 0, 0, c, size);
        }

        public static void Draw(float xPercent, float yPercent, AbilityType type, int ammo, int maxAmmo)
        {
            Draw(xPercent, yPercent, type, ammo, maxAmmo, Color.White, .1f);
        }

        
        public static void Draw(float xPercent, float yPercent, AbilityType type, int ammo, int maxAmmo, Color c, float iconSize)
        {
            float barWidth = .05f;
            float barHeight = .08f;
            int spriteX = (int)(type) % 8;
            int spriteY = (int)(type) / 8;

            if (DepthControl.uiFade == true)
            {
                c.R = (Byte)(c.R - c.R * DepthControl.depthTimer / DepthControl.maxDepthTimer);
                c.G = (Byte)(c.G - c.G * DepthControl.depthTimer / DepthControl.maxDepthTimer);
                c.B = (Byte)(c.B - c.B * DepthControl.depthTimer / DepthControl.maxDepthTimer);
                c.A = (Byte)(c.A - c.A * DepthControl.depthTimer / DepthControl.maxDepthTimer);
            }
            
            
            Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + xPercent * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)(iconSize * Game1.titleSafeRect.Width), (int)(iconSize * Game1.titleSafeRect.Width)), new Rectangle(128*spriteX, 128*spriteY, 128, 128), c);
            if (type == AbilityType.JetPack)
            {
                Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + (xPercent + .06) * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)(barWidth * Game1.titleSafeRect.Width), (int)(barHeight * Game1.titleSafeRect.Width)), new Rectangle(128 * 5, 128 * 3, 128, 128), c);
                Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + (xPercent + .06) * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height + (barHeight * Game1.titleSafeRect.Width - (int)((1f * ammo / maxAmmo) * barHeight * Game1.titleSafeRect.Width))), (int)(barWidth * Game1.titleSafeRect.Width), (int)((1f * ammo / maxAmmo) * barHeight * Game1.titleSafeRect.Width)), new Rectangle(128 * 6, 128 * 3, 128, 128), c);
                
            }
            if (type == AbilityType.Shield || (type == AbilityType.NormalJump && maxAmmo != 0))
            {
                Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + (xPercent + .06) * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)(barWidth * Game1.titleSafeRect.Width), (int)(barHeight * Game1.titleSafeRect.Width)), new Rectangle(128 * 5, 128 * 3, 128, 128), c);
                Engine.spriteBatch.Draw(ability_textures, new Rectangle((int)(Game1.titleSafeRect.Left + (xPercent + .06) * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height + (barHeight * Game1.titleSafeRect.Width - (int)((1f * ammo / maxAmmo) * barHeight * Game1.titleSafeRect.Width))), (int)(barWidth * Game1.titleSafeRect.Width), (int)((1f * ammo / maxAmmo) * barHeight * Game1.titleSafeRect.Width)), new Rectangle(128 * 6, 128 * 3, 128, 128), c);

            }            
        }*/

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
            if (type == AbilityType.Empty)
                return "Empty";
            return "BUG";
        }

        public String GetDialogId()
        {
            if (type == AbilityType.DoubleJump)
                return "YouGotDoubleJump";
            if (type == AbilityType.Blaster)
                return "YouGotBlaster";
            if (type == AbilityType.WallJump)
                return "YouGotWallJump";
            if (type == AbilityType.Boots)
                return "YouGotGravityBoots";
            if (type == AbilityType.Laser)
                return "YouGotLaserCannon";            
            if (type == AbilityType.Booster)
                return "YouGotThruster";
            if (type == AbilityType.Missile)
                return "YouGotMissile";
            if (type == AbilityType.PermanentBoots)
                return "YouGotGravityModule";
            if (type == AbilityType.PermanentWallJump)
                return "YouGotWallJumpModule";
            if (type == AbilityType.ImprovedJump)
                return "YouGotImprovedJumpModule";
            if (type == AbilityType.SpinHook)
                return "YouGotSpinHook";
            if (type == AbilityType.JetPack)
                return "YouGotJetPack";
            if (type == AbilityType.Phase)
                return "YouGotPhaseBelt";
            if (type == AbilityType.PermanentRedKey)
                return "YouGotRedKey";
            if (type == AbilityType.PermanentYellowKey)
                return "YouGotYellowKey";
            if (type == AbilityType.PermanentBlueKey)
                return "YouGotBlueKey";
            return "OOPS";
        }

        public String Description()
        {
            if (type == AbilityType.Empty)
                return "Locate and equip an item to this slot.";
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

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
using System.Xml.Serialization;

namespace VexedCore
{
    public enum FaceState
    {
        Armored,
        Exploding,
        Angry,
        Rebuilding
    }

    

    public class FaceBoss
    {        
        int armorExplodeTime = 0;
        int armorExplodeMaxTime = 500;
        int timer = 0;
        int timerMax = 2000;

        int damageCooldown = 0;
        int damageCooldownMax = 500;

        public int dialogState = 0;

        public FaceState state = FaceState.Exploding;

        public bool PhaseTest()
        {
            float distance = (Engine.player.center.position - Engine.player.currentRoom.center).Length();
            return distance > 11.3f;
        }

        public void Update(int time, Monster srcMonster)
        {
            if (dialogState == 0 && Engine.player.state == State.Normal && state == FaceState.Rebuilding)
            {
                DialogBox.SetDialog("FinalBoss1");
                dialogState++;
            }
            if (dialogState == 1 && srcMonster.baseHP != srcMonster.startingBaseHP && damageCooldown == 0)
            {
                DialogBox.SetDialog("FinalBoss2");
                dialogState++;
            }
            if (dialogState == 2 && srcMonster.baseHP ==0)
            {
                DialogBox.SetDialog("FinalBoss3");
                dialogState++;
            }

            damageCooldown -= time;
            if (damageCooldown < 0)
                damageCooldown = 0;
            if (state == FaceState.Exploding)
            {
                armorExplodeTime += time;
                if (armorExplodeTime > armorExplodeMaxTime)
                {
                    armorExplodeTime = armorExplodeMaxTime;
                    state = FaceState.Angry;
                }
            }
            else if(state == FaceState.Rebuilding)
            {
                armorExplodeTime -= time;
                if (armorExplodeTime < 0)
                {
                    armorExplodeTime = 0;
                    state = FaceState.Armored;
                }
            }
            else if (state == FaceState.Angry)
            {
                timer -= time;
                if (dialogState < 3)
                {
                    if (timer < 0)
                    {
                        timer = timerMax;
                        state = FaceState.Rebuilding;
                        foreach (Monster m in Engine.player.currentRoom.monsters)
                        {
                            if (m.id.Contains("Guardian") && m.state == MonsterState.Death)
                            {
                                m.baseHP = m.startingBaseHP;
                                m.dead = false;
                                m.state = MonsterState.Spawn;
                                m.spawnTime = 0;
                                m.deathTime = Monster.maxDeathTime;
                            }
                        }
                    }
                }

                if(damageCooldown == 0 && (Engine.player.state == State.Phase || Engine.player.state == State.PhaseFail) && (Engine.player.jumpPosition - Engine.player.currentRoom.center).Length() < 6f)
                {
                    srcMonster.impactVector = new Vector3(1, 0, 0);
                    srcMonster.lastHitType = ProjectileType.Plasma;
                    damageCooldown = damageCooldownMax;
                }

            }
            else if (state == FaceState.Armored)
            {
                bool explode = true;
                if (srcMonster.startingBaseHP != 0)
                {

                    foreach (Monster m in Engine.player.currentRoom.monsters)
                    {
                        if (m.dead == false && m.id.Contains("Guardian"))
                            explode = false;
                    }
                    if (explode == true)
                    {
                        state = FaceState.Exploding;
                    }
                }                
            }

        }

        public void DrawNormalEyes(Room currentRoom)
        {
            Vector3 center = currentRoom.center;
            Vector3 outDirection = Engine.player.center.position - center;
            Vector3 up = Engine.player.cameraUp;
            outDirection.Normalize();
            up.Normalize();
            Vector3 right = Vector3.Cross(up, outDirection);
            VertexPositionColorNormalTexture[] eyeVertices = new VertexPositionColorNormalTexture[12];
            eyeVertices[0] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 2f * right, Color.White, outDirection, Room.plateTexCoords[2]);
            eyeVertices[1] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 1f * right + .2f * up, Color.White, outDirection, Room.plateTexCoords[3]);
            eyeVertices[2] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 2f * right + up, Color.White, outDirection, Room.plateTexCoords[1]);
            eyeVertices[3] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 1f * right + .2f * up, Color.White, outDirection, Room.plateTexCoords[3]);
            eyeVertices[4] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 2f * right + up, Color.White, outDirection, Room.plateTexCoords[1]);
            eyeVertices[5] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 1f * right + up, Color.White, outDirection, Room.plateTexCoords[0]);

            eyeVertices[6] = new VertexPositionColorNormalTexture(center + outDirection * 6 - 2 * right, Color.White, outDirection, Room.plateTexCoords[2]);
            eyeVertices[7] = new VertexPositionColorNormalTexture(center + outDirection * 6 - 1 * right + .2f * up, Color.White, outDirection, Room.plateTexCoords[3]);
            eyeVertices[8] = new VertexPositionColorNormalTexture(center + outDirection * 6 - 2f * right + up, Color.White, outDirection, Room.plateTexCoords[1]);
            eyeVertices[9] = eyeVertices[7];
            eyeVertices[10] = eyeVertices[8];
            eyeVertices[11] = new VertexPositionColorNormalTexture(center + outDirection * 6 - 1f * right + up, Color.White, outDirection, Room.plateTexCoords[0]);

            Engine.playerTextureEffect.Texture = Monster.monsterTextures[(int)MonsterTextureId.FaceNormalEye];
            Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                eyeVertices, 0, eyeVertices.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);

        }

        public void DrawAngryEyes(Room currentRoom)
        {
            Vector3 center = currentRoom.center;
            Vector3 outDirection = Engine.player.center.position - center;
            Vector3 up = Engine.player.cameraUp;
            outDirection.Normalize();
            up.Normalize();
            Vector3 right = Vector3.Cross(up, outDirection);
            VertexPositionColorNormalTexture[] eyeVertices = new VertexPositionColorNormalTexture[6];
            eyeVertices[0] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 2f * right, Color.White, outDirection, Room.plateTexCoords[2]);
            eyeVertices[1] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 1f * right + .2f * up, Color.White, outDirection, Room.plateTexCoords[3]);
            eyeVertices[2] = new VertexPositionColorNormalTexture(center + outDirection * 6 + 2f * right + up, Color.White, outDirection, Room.plateTexCoords[1]);

            eyeVertices[3] = new VertexPositionColorNormalTexture(center + outDirection * 6 - 2 * right, Color.White, outDirection, Room.plateTexCoords[2]);
            eyeVertices[4] = new VertexPositionColorNormalTexture(center + outDirection * 6 - 1 * right + .2f * up, Color.White, outDirection, Room.plateTexCoords[3]);
            eyeVertices[5] = new VertexPositionColorNormalTexture(center + outDirection * 6 - 2f * right + up, Color.White, outDirection, Room.plateTexCoords[1]);

            Engine.playerTextureEffect.Texture = Monster.monsterTextures[(int)MonsterTextureId.FaceAngryEye];
            Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                eyeVertices, 0, eyeVertices.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            
        }

        public void DrawSphere(Room currentRoom, bool armor)
        {
            int numThetaSides = 10;
            int numPhiSides = 8;
            float horizontalRadius = 5f;
            float verticalRadius = 5f;
            VertexPositionColorNormalTexture[] vertices = new VertexPositionColorNormalTexture[numThetaSides * 6 * (numPhiSides + 1)]; 
            
            int index = 0; 
            for (int i = 0; i < numThetaSides; ++i) 
            { 
                float theta1 = ((float)i / (float)numThetaSides) * MathHelper.ToRadians(360f); 
                float theta2 = ((float)(i + 1) / (float)numThetaSides) * MathHelper.ToRadians(360f); 
                for (int j = -1; j < numPhiSides; ++j) 
                { 
                    float phi_a = ((float)j / (float)numPhiSides) * MathHelper.ToRadians(180f); 
                    float x1_a = (float)(Math.Sin(phi_a) * Math.Cos(theta1)) * horizontalRadius;
                    float z1_a = (float)(Math.Sin(phi_a) * Math.Sin(theta1)) * horizontalRadius;
                    float x2_a = (float)(Math.Sin(phi_a) * Math.Cos(theta2)) * horizontalRadius;
                    float z2_a = (float)(Math.Sin(phi_a) * Math.Sin(theta2)) * horizontalRadius;
                    float y_a = (float)Math.Cos(phi_a) * verticalRadius;
                    float phi_b = ((float)(j+1) / (float)numPhiSides) * MathHelper.ToRadians(180f);
                    float x1_b = (float)(Math.Sin(phi_b) * Math.Cos(theta1)) * horizontalRadius;
                    float z1_b = (float)(Math.Sin(phi_b) * Math.Sin(theta1)) * horizontalRadius;
                    float x2_b = (float)(Math.Sin(phi_b) * Math.Cos(theta2)) * horizontalRadius;
                    float z2_b = (float)(Math.Sin(phi_b) * Math.Sin(theta2)) * horizontalRadius;
                    float y_b = (float)Math.Cos(phi_b) * verticalRadius; 
                    
                    Vector3 position1 = currentRoom.center + new Vector3(x1_a, y_a, z1_a);
                    Vector3 position2 = currentRoom.center + new Vector3(x2_a, y_a, z2_a);
                    Vector3 position3 = currentRoom.center + new Vector3(x1_b, y_b, z1_b);
                    Vector3 position4 = currentRoom.center + new Vector3(x2_b, y_b, z2_b);
                    Vector3 offset = (position1 + position2 + position3 + position4)/4 - currentRoom.center;
                    offset.Normalize();
                    offset *= 20f * armorExplodeTime / armorExplodeMaxTime;

                    if (armor == true)
                    {                        
                        vertices[index] = new VertexPositionColorNormalTexture(position1 + offset, Color.White, position1 - currentRoom.center, Room.plateTexCoords[0]);
                        vertices[index + 1] = new VertexPositionColorNormalTexture(position2 + offset, Color.White, position2 - currentRoom.center, Room.plateTexCoords[1]);
                        vertices[index + 2] = new VertexPositionColorNormalTexture(position3 + offset, Color.White, position3 - currentRoom.center, Room.plateTexCoords[3]);
                        vertices[index + 3] = new VertexPositionColorNormalTexture(position2 + offset, Color.White, position2 - currentRoom.center, Room.plateTexCoords[1]);
                        vertices[index + 4] = new VertexPositionColorNormalTexture(position3 + offset, Color.White, position3 - currentRoom.center, Room.plateTexCoords[3]);
                        vertices[index + 5] = new VertexPositionColorNormalTexture(position4 + offset, Color.White, position4 - currentRoom.center, Room.plateTexCoords[2]);
                    }
                    else
                    {
                        Color bodyColor = Color.Red;
                        bodyColor.G = (Byte)(damageCooldown * 255 / damageCooldownMax);

                        vertices[index] = new VertexPositionColorNormalTexture(position1, bodyColor, position1 - currentRoom.center, Room.plateTexCoords[0]);
                        vertices[index + 1] = new VertexPositionColorNormalTexture(position2, bodyColor, position2 - currentRoom.center, Room.plateTexCoords[1]);
                        vertices[index + 2] = new VertexPositionColorNormalTexture(position3, bodyColor, position3 - currentRoom.center, Room.plateTexCoords[3]);
                        vertices[index + 3] = new VertexPositionColorNormalTexture(position2, bodyColor, position2 - currentRoom.center, Room.plateTexCoords[1]);
                        vertices[index + 4] = new VertexPositionColorNormalTexture(position3, bodyColor, position3 - currentRoom.center, Room.plateTexCoords[3]);
                        vertices[index + 5] = new VertexPositionColorNormalTexture(position4, bodyColor, position4 - currentRoom.center, Room.plateTexCoords[2]);
                    }

                    index+=6;
                } 
            }
            if (armor == true)
            {
                Engine.playerTextureEffect.Texture = Monster.monsterTextures[(int)MonsterTextureId.FacePlate];
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                Engine.playerTextureEffect.Texture = Monster.monsterTextures[(int)MonsterTextureId.FaceWhite];
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            }
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
               vertices, 0, vertices.Count()/3, VertexPositionColorNormalTexture.VertexDeclaration);
        }

        public void Render(Room currentRoom)
        {
            DrawSphere(currentRoom,false);
            DrawSphere(currentRoom, true);
            if(state == FaceState.Armored)
                DrawNormalEyes(currentRoom);
            else
                DrawAngryEyes(currentRoom);
        }
    }
}

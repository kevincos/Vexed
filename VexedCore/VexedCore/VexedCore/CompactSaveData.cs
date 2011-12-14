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

    public class Sr // Sector
    {
        public int cbo; //current blue orbs
        public int cro; //current red orbs 
        public int co; //current orbs

        public Sr()
        {
        }

        public Sr(Sector s)
        {
            cbo = s.currentBlueOrbs;
            cro = s.currentRedOrbs;
            co = s.currentOrbs;            
        }
    }

    public class Rm // room
    {
        public int cbo; //current blue orbs
        public int cro; //current red orbs 
        public int co; //current orbs
        public bool bc; //boss cleared
        public bool e; //explored

        public Rm()
        {
        }

        public Rm(Room r)
        {
            cbo = r.currentBlueOrbs;
            cro = r.currentRedOrbs;
            co = r.currentOrbs;
            bc = r.bossCleared;
            e = r.explored;
        }
    }

    public class Dd // doodad
    {
        public string cbi; //current behavior id
        public int or; //orbs remaining
        public bool to; //toggle on
        public bool au; //already used
        public float st; //state transition
        public bool ac; //active

        public Dd()
        {
        }

        public Dd(Doodad d)
        {
            cbi = d.currentBehaviorId;
            or = d.orbsRemaining;
            to = d.toggleOn;
            au = d.alreadyUsed;
            st = d.stateTransition;
            ac = d.active;
        }
    }

    public class Mo // Monster
    {
        public bool ho; // has orbs

        public Mo()
        {
        }

        public Mo(Monster m)
        {
            ho = m.hasOrbs;
        }
    }

    public class Bl // Block
    {
        public string cbi; //current behavior id
        public Ed e1; //edge 1
        public Ed e2; //edge 2
        public Ed e3; //edge 3
        public Ed e4; //edge 4

        public Bl()
        {
        }

        public Bl(Block b)
        {
            cbi = b.currentBehaviorId;
            e1 = new Ed(b.edges[0]);
            e2 = new Ed(b.edges[1]);
            e3 = new Ed(b.edges[2]);
            e4 = new Ed(b.edges[3]);
        }
    }

    public class Ed // Edge
    {
        public string cbi; //current behavior id
        public Vertex vs; //start vertex
        public Vertex ve; //end vertex
        public int pv; //primary value;
        public int sv; //secondary value;
        public bool to; // toggle on;

        public Ed()
        {
        }

        public Ed(Edge e)
        {
            cbi = e.currentBehaviorId;
            vs = new Vertex(e.start);
            ve = new Vertex(e.end);
            pv = e.properties.primaryValue;
            sv = e.properties.secondaryValue;
            to = e.toggleOn;
        }
    }



    public class CompactSaveData
    {
        public List<Rm> rmLst;
        public List<Dd> ddLst;
        public List<Mo> moLst;
        public List<Bl> blLst;
        public List<Sr> srLst;
        public Player player;

        public CompactSaveData()
        {
        }

        public CompactSaveData(SaveData data)
        {
            rmLst = new List<Rm>();
            ddLst = new List<Dd>();
            moLst = new List<Mo>();
            blLst = new List<Bl>();
            srLst = new List<Sr>();
            foreach (Room r in data.roomList)
            {
                rmLst.Add(new Rm(r));

                foreach (Doodad d in r.doodads)
                {
                    ddLst.Add(new Dd(d));
                }

                foreach (Monster m in r.monsters)
                {
                    moLst.Add(new Mo(m));                    
                }

                foreach (Block b in r.blocks)
                {
                    if(b.staticObject == false)
                        blLst.Add( new Bl(b));                    
                }
            }
            foreach (Sector s in data.sectorList)
            {
                srLst.Add(new Sr(s));
            }
            player = new Player(data.player);
        }

        public void LoadCompactSaveData()
        {
            int currentDoodad = 0;
            int currentMonster = 0;
            int currentBlock = 0;
            for (int i = 0; i < Engine.roomList.Count; i++)
            {
                Engine.roomList[i].Load(rmLst[i]);
                foreach (Monster m in Engine.roomList[i].monsters)
                {
                    m.Load(moLst[currentMonster]);
                    currentMonster++;
                }
                foreach (Doodad d in Engine.roomList[i].doodads)
                {                    
                    d.Load(ddLst[currentDoodad]);
                    currentDoodad++;
                }
                foreach (Block b in Engine.roomList[i].blocks)
                {
                    if (b.staticObject == false)
                    {
                        b.Load(blLst[currentBlock]);
                        currentBlock++;
                    }
                }
            }
            Engine.player = new Player(player);
        }
    }
}

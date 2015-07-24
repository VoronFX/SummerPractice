using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CatapultGame
{
    public class Obstacle : Unit { }

    public class Unit
    {
        public int MaxHitpoints;
        public int MovementSpeed;
        // public Race Race;
        public int Defense;
        public int Attack;
        public float Range;
        public int Damage;
        public int MaxAmount;
        public int InitiativeMod;
        // public bool Magic;
        
      

        public Unit(int maxHitpoints, int defense, int attack, int damage, int movSpeed, int maxAmount, float range)
        {
            MaxHitpoints = maxHitpoints;
            Defense = defense;
            Attack = attack;
            Range = range;
            Damage = damage;

            MovementSpeed = movSpeed;
            MaxAmount = maxAmount;
            
        }
        public Unit()
        {
            MaxAmount = 0;
            MaxHitpoints = 0;
            Defense = 0;
            Attack = 0;
            Range = 0;
            Damage = 0;

        }

    }
}

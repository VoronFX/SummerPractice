using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CatapultGame
{
    public class Squad 
    {
        float TimeUpdate = 0.0001F;
        float TimeDraw = 0.0001F;
        public readonly Unit Unit;
        private int amount;
        public int DamageLeft;
        public Point position;
        public int start;
        Rectangle CurrentFrame;

        public enum ActionType { None, Move, Attack, MoveAndAttack, AttackAndMove, TakingDamage }

        public struct Action
        {
            public Squad Target;
            public Point[] Path;
            public int Damage;
            public ActionType Type;
        }

        public Action CurrentAction;

        private bool alive;
        private bool takingDMG=false;
        private bool attaking=false;

        public bool Alive
        {
            get { return alive; }
            private set { alive = value; }
        }

        public void Draw(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= TimeDraw)
            { 
                switch (CurrentAction.Type)
                {
                    case ActionType.None:
                        break;
                    case ActionType.Move:
                        break;
                    case ActionType.Attack:
                        attaking = false;
                        break;
                    case ActionType.MoveAndAttack:
                        break;
                    case ActionType.AttackAndMove:
                        break;
                    case ActionType.TakingDamage:

                        takingDMG = false;
                        //CurrentAction = new Action() { Type = ActionType.None };

                        break;
                    default:
                        break;
                }

            }
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= TimeUpdate)
            {
                switch (CurrentAction.Type)
                {
                    case ActionType.None:
                        break;
                    case ActionType.Move:
                        if(start == 999)
                        {
                            start =CurrentAction.Path.Length-1;
                        }
                        Point b = CurrentAction.Path[start - 1];
                        Point a = position;//CurrentAction.Path[start];
                        Vector2 delta = new Vector2(b.X - a.X, b.Y - a.Y);
                        Vector2.Normalize(delta);
                        position = new Point(position.X + (int)delta.X, position.Y + (int)delta.Y);
                        if (position==CurrentAction.Path[start-1])
                        {
                            start--;
                        }
                        if (start <= 0) ;
                        CurrentAction = new Action() { Type = ActionType.None };
                        break;
                    case ActionType.Attack:
                        if (CurrentAction.Target.CurrentAction.Type!=ActionType.TakingDamage)
                        Attack(CurrentAction.Target);
                        
                        if(!attaking)
                            CurrentAction = new Action() { Type = ActionType.None };
                        break;
                    case ActionType.MoveAndAttack:
                        if (start == 999)
                        {
                            start = CurrentAction.Path.Length - 1;
                        }
                        Point b1 = CurrentAction.Path[start - 1];
                        Point a1 = position;//CurrentAction.Path[start];
                        Vector2 delta1 = new Vector2(b1.X - a1.X, b1.Y - a1.Y);
                        Vector2.Normalize(delta1);
                        position = new Point(position.X + (int)delta1.X, position.Y + (int)delta1.Y);
                        if (position == CurrentAction.Path[start - 1])
                        {
                            start--;
                        }
                        if (start <= 0)
                        {
                            CurrentAction = new Action() { Type = ActionType.Attack, Target = CurrentAction.Target };
                            start = 999;
                        }

                        break;
                    case ActionType.AttackAndMove:
                        if (CurrentAction.Target.CurrentAction.Type != ActionType.TakingDamage)
                            Attack(CurrentAction.Target);

                        if (!attaking)
                            CurrentAction = new Action() { Type = ActionType.Move ,Path = CurrentAction.Path};
                        break;
                    case ActionType.TakingDamage:
                        if (!takingDMG) CurrentAction = new Action() { Type = ActionType.None };
                        break;
                    default:
                        break;
                }
            }
        }



        public int Amount
        {
            get { return amount; }
            set
            {
                if (value < 1)
                {
                    alive = false;
                    amount = 0;

                }
                else

                    amount = value;
            }
        }

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public Squad(Unit unit)
        {
            Unit = unit;
            amount = unit.MaxAmount;
            DamageLeft = 0;
            Position = new Point();
            start = 999;
            alive = true;
        }

        public void Attack(Squad target)
        {
            attaking = true;
            int Damage = 0;
            for (int i = 0; i < Amount; i++)
            {
                if ((1 + Random.Next(20) + Unit.Attack) >= target.Unit.Defense)
                    Damage += Unit.Damage;
            }

            target.TakeDamage(Damage);
        }

        public void TakeDamage(int damage)
        {
            takingDMG = true;
            CurrentAction = new Action() {Type = ActionType.TakingDamage,Damage =damage  };
            damage += DamageLeft;
            Amount -= (damage / Unit.MaxHitpoints);
            DamageLeft = damage % Unit.MaxHitpoints;


        }
     
        public object Clone()
        {
            return new Squad(Unit) { DamageLeft = 0, Amount = amount, Position = position };
        }
    }
}

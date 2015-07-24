using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CatapultGame
{


    using Maneuver = Action<BattleData, Squad>;
    using Step = KeyValuePair<Point, float>;
    public class Strategy
    {
        public static Strategy Offensive = new Offensive();
        public static Strategy Deffensive = new Deffensive();

        public readonly Maneuver[] Maneuvers;

        protected Strategy()
        {
            Maneuvers = new Maneuver[2];
        }
        protected static int NearestToPoint(Point p1, Squad[] army)
        {
            int Temp = -1;
            double minDistance = Double.MaxValue;
            for (int i = 0; i < army.Length; i++)
            {

                double distance = DistanceAndPath.DistanceTo(p1, army[i].Position);
                if ((distance < minDistance) && army[i].Alive)
                {

                    minDistance = distance;
                    Temp = i;
                }
            }
            return Temp;
        }
        /// <summary>
        /// Chose Nearest target to all army squads
        /// </summary>
        /// <param name="army"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        protected static int NearestToAll(Squad[] army, Squad[] targets)
        {
            int Temp = -1;
            double[] distances = new double[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                for (int j = 0; j < army.Length; j++)
                {
                    distances[i] += DistanceAndPath.DistanceTo(army[j].Position, targets[i].Position);
                }
            }

            double min = double.MaxValue;
            for (int i = 0; i < distances.Length; i++)
            {
                if ((distances[i] < min) && targets[i].Alive)
                {
                    min = distances[i];
                    Temp = i;
                }
            }

            return Temp;
        }

        protected static void AttackAndMove(Squad attacker, Squad target, Step[] path, BattleData bd)
        {
            attacker.CurrentAction.Type = Squad.ActionType.Attack;
            attacker.CurrentAction.Target = target;

            int dmg = target.Amount;
            attacker.Attack(target);

            attacker.CurrentAction.Damage = dmg - target.Amount;

            Move(attacker, path, bd);

            if (attacker.CurrentAction.Type == Squad.ActionType.Move)
                attacker.CurrentAction.Type = Squad.ActionType.AttackAndMove;

        }

        protected static void MoveAndAttack(Squad attacker, Squad target, Step[] path, BattleData bd)
        {
            attacker.CurrentAction.Type = Squad.ActionType.None;

            if (Move(attacker, path, bd))
            {
                attacker.CurrentAction.Type = attacker.CurrentAction.Type == Squad.ActionType.Move ?
                    Squad.ActionType.AttackAndMove : Squad.ActionType.Attack;
                attacker.CurrentAction.Target = target;

                int dmg = target.Amount;
                attacker.Attack(target);

                attacker.CurrentAction.Damage = dmg - target.Amount;
            }

        }

        protected static bool Move(Squad mover, Step[] path, BattleData bd)
        {
            if (path == null)
                return false;
            int length = path.Length;
            if (length < 1)
                return true;
            double movement = mover.Unit.MovementSpeed;
            Point temp = path[0].Key;

            for (int k = length - 1; k > 0; k--)
            {
                if (path[k].Value > movement)
                {
                    temp = path[k + 1].Key;

                    mover.CurrentAction.Type = Squad.ActionType.Move;
                    mover.CurrentAction.Path = path.Select(x => x.Key).SkipWhile(x => x != temp).TakeWhile(x => x != mover.Position).Concat(new Point[] { mover
                    .Position}).ToArray();

                    bd.Relocate(mover.Position, temp);
                    mover.Position = temp;
                    return false;
                }
            }

            mover.CurrentAction.Type = Squad.ActionType.Move;
            mover.CurrentAction.Path = path.Select(x => x.Key).SkipWhile(x => x != temp).TakeWhile(x => x != mover.Position).Concat(new Point[] { mover
                    .Position}).ToArray();

            bd.Relocate(mover.Position, temp);
            mover.Position = temp;
            return true;
        }

        protected static Point GetSafeFrom(Point victum, Point enemy)
        {
            Point temp = new Point(victum.X - enemy.X, victum.Y - enemy.Y);
            List<Point> SafePlaces = new List<Point>();
            if (temp.X <= 0)
                SafePlaces.Add(new Point(0, victum.Y));
            if (temp.X >= 0)
                SafePlaces.Add(new Point(99, victum.Y));
            if (temp.Y <= 0)
                SafePlaces.Add(new Point(victum.X, 0));
            if (temp.Y >= 0)
                SafePlaces.Add(new Point(victum.X, 99));

            double max = double.MinValue;
            foreach (var item in SafePlaces)
            {
                double dist = DistanceAndPath.DistanceTo(victum, item);
                if (dist > max)
                {
                    max = dist;
                    temp = item;
                }
            }

            return temp;
        }

    }
}


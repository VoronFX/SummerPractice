using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CatapultGame
{



    using Step = KeyValuePair<Point, float>;
    public class Deffensive : Strategy
    {
        static void Ambysh(BattleData battleData, Squad current)
        {
            if (battleData.EnemyArmy.Length < 1)
                return;

                int TargetIndex = Strategy.NearestToPoint(current.Position, battleData.EnemyArmy);
                if (TargetIndex < 0)
                    return;
                Step[] Path = DistanceAndPath.PathTo(
                    battleData,
                    current.Position,
                    battleData.EnemyArmy[TargetIndex].Position,
                    current.Unit.Range);
                if (Path != null)
                    Strategy.MoveAndAttack(current, battleData.EnemyArmy[TargetIndex], Path, battleData);
            
        }

        static void HitAndRun(BattleData battleData, Squad current)
        {
            if (battleData.EnemyArmy.Length < 1)
                return;

            int TargetIndex = Strategy.NearestToPoint(current.Position, battleData.EnemyArmy);
            if (TargetIndex < 0)
                return;
            Step[] Path = DistanceAndPath.PathTo(
                battleData,
                current.Position,
                battleData.EnemyArmy[TargetIndex].Position,
                current.Unit.Range);
            if (Path != null)
                if (Path.Length == 0)
                {
                    Point SafePoint = GetSafeFrom(current.Position, battleData.EnemyArmy[TargetIndex].Position);

                    Path = DistanceAndPath.PathTo(
                    battleData,
                    current.Position,
                    SafePoint,
                    0);

                    Strategy.AttackAndMove(current, battleData.EnemyArmy[TargetIndex], Path, battleData);
                }
                else
                    Strategy.MoveAndAttack(current, battleData.EnemyArmy[TargetIndex], Path, battleData);

        }


        public Deffensive()
        {
            Maneuvers[0] = Ambysh;
            Maneuvers[1] = HitAndRun;
        }
    }
}


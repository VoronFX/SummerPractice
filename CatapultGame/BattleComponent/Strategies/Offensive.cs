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
    public class Offensive : Strategy
    {
        static void Surround(BattleData battleData, Squad current)
        {
            if (battleData.EnemyArmy.Length < 1)
                return;

                int TargetIndex = Strategy.NearestToAll(battleData.AllyArmy, battleData.EnemyArmy);
                if (TargetIndex < 0)
                    return;
                Step[] Path = DistanceAndPath.PathTo(
                    battleData,
                    current.Position,
                    battleData.EnemyArmy[TargetIndex].Position,
                    current.Unit.Range);

                Strategy.MoveAndAttack(current, battleData.EnemyArmy[TargetIndex], Path, battleData);       

        }
        static void Rush(BattleData battleData, Squad current)
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

                Strategy.MoveAndAttack(current, battleData.EnemyArmy[TargetIndex], Path, battleData);

        }
        public Offensive()
        {

            Maneuvers[0] = Surround;
            Maneuvers[1] = Rush;
        }
    }
}

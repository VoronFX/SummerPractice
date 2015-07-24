using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GoblinsGame
{
    class AI : Player
    {
        Random random;

        public AI(Game game)
            : base(game)
        {
        }

        public AI(Game game, SpriteBatch screenSpriteBatch)
            : base(game, screenSpriteBatch)
        {
            // TODO: Initialize catapult
            Catapult = new Catapult(game, screenSpriteBatch,
                "Textures/Catapults/Red/redIdle/redIdle", new Vector2(600, 332),
                SpriteEffects.FlipHorizontally, true, false);

        }

        public override void Initialize()
        {
            // TODO: Initialize randomizer
            random = new Random();

            Catapult.Initialize();

            // TODO: Initialize guide projectile

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Check if it is time to take a shot
            if (Catapult.CurrentState == CatapultState.Aiming
                && !Catapult.AnimationRunning)
            {
                // Fire at a random strength and angle
                float shotVelocity =
                    random.Next((int)MinShotVelocity, (int)MaxShotVelocity);
                float shotAngle = MinShotAngle +
                    (float)random.NextDouble() * (MaxShotAngle - MinShotAngle);

                Catapult.ShotStrength = (shotVelocity / MaxShotVelocity);
                Catapult.ShotVelocity = shotVelocity;
                Catapult.ShotAngle = shotAngle;
            }
            base.Update(gameTime);
        }

    }
}

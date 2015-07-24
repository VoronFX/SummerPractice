using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GoblinsGame
{
    public enum WeaponType
    {
        Normal,
        Split
    }

    internal class Player : DrawableGameComponent
    {
        protected TheGame curGame;
        protected SpriteBatch spriteBatch;

        // Constants used for calculating shot strength
        public const float MinShotVelocity = 200f;
        public const float MaxShotVelocity = 665f;
        public const float MinShotAngle = 0; // 0 degrees
        public const float MaxShotAngle = 1.3962634f; // 80 degrees

        // Public variables used by Gameplay class
        public Catapult Catapult { get; set; }
        public int Score { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public WeaponType Weapon { get; set; }

        public Player Enemy
        {
            set
            {
                Catapult.Enemy = value;
                Catapult.Self = this;
            }
        }

        public bool IsActive { get; set; }

        public Player(Game game) : base(game)
        {
            curGame = (TheGame)game;
        }

        public Player(Game game, SpriteBatch screenSpriteBatch) : this(game)
        {
            spriteBatch = screenSpriteBatch;
        }

        public override void Initialize()
        {
            Score = 0;

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw related catapults
            Catapult.Draw(gameTime);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            // Update catapult related to the player
            Catapult.Update(gameTime);
            base.Update(gameTime);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GoblinsGame
{
    public enum ProjectileState
    {
        InFlight,
        HitGround,
        Destroyed
    }

    class Projectile : DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        protected Game curGame;

        // List of currently active projectiles. This allows projectiles
        // to spawn other projectiles.
        protected List<Projectile> activeProjectiles;

        // Textures for projectile
        string textureName;

        // Position and speed of projectile
        protected Vector2 projectileInitialVelocity = Vector2.Zero;
        protected Vector2 projectileRotationPosition = Vector2.Zero;

        protected float Gravity;
        protected float flightTime;
        protected float projectileRotation;
        protected float hitOffset;
        protected bool isRightPlayer;
        protected float gravity;

        public virtual float Wind { get; set; }

        Vector2 projectileStartPosition;
        public Vector2 ProjectileStartPosition
        {
            get
            {
                return projectileStartPosition;
            }
            set
            {
                projectileStartPosition = value;
            }
        }

        // Gets the position where the projectile hit the ground.
        // Only valid after a hit occurs.
        public Vector2 ProjectileHitPosition { get; private set; }

        Vector2 currentVelocity = Vector2.Zero;
        public Vector2 CurrentVelocity
        {
            get
            {
                return currentVelocity;
            }
        }

        Vector2 projectilePosition = Vector2.Zero;
        public Vector2 ProjectilePosition
        {
            get
            {
                return projectilePosition;
            }
            set
            {
                projectilePosition = value;
            }
        }

        Texture2D projectileTexture;
        public Texture2D ProjectileTexture
        {
            get
            {
                return projectileTexture;
            }
            set
            {
                projectileTexture = value;
            }
        }

        public ProjectileState State { get; private set; }

        /// <summary>
        /// Used to mark whether or not the projectile's hit was handled.
        /// </summary>
        public bool HitHandled { get; set; }

        public Projectile(Game game) : base(game)
        {
            curGame = game;
        }

        public Projectile(Game game, SpriteBatch screenSpriteBatch,
            List<Projectile> activeProjectiles, string textureName,
            Vector2 startPosition, float groundHitOffset, bool isRightPlayer,
            float gravity)
            : this(game)
        {
            spriteBatch = screenSpriteBatch;
            this.activeProjectiles = activeProjectiles;
            projectileStartPosition = startPosition;
            this.textureName = textureName;
            this.isRightPlayer = isRightPlayer;
            hitOffset = groundHitOffset;
            this.gravity = gravity;
        }

        public override void Initialize()
        {
            // Load a projectile texture
            projectileTexture = curGame.Content.Load<Texture2D>(textureName);
        }

        public void UpdateProjectileFlightData(float elapsedSeconds, float wind,
    float gravity)
        {
            flightTime += elapsedSeconds;

            // Calculate new projectile position using standard
            // formulas, taking the wind as a force.
            int direction = isRightPlayer ? -1 : 1;

            float previousXPosition = projectilePosition.X;
            float previousYPosition = projectilePosition.Y;

            projectilePosition.X = projectileStartPosition.X +
                (direction * projectileInitialVelocity.X * flightTime) +
                0.5f * (8 * wind * (float)Math.Pow(flightTime, 2));

            currentVelocity.X = projectileInitialVelocity.X + 8 * wind
                                * flightTime;

            projectilePosition.Y = projectileStartPosition.Y -
                (projectileInitialVelocity.Y * flightTime) +
                0.5f * (gravity * (float)Math.Pow(flightTime, 2));

            currentVelocity.Y = projectileInitialVelocity.Y - gravity
                                * flightTime;

            // Calculate the projectile rotation
            projectileRotation +=
                MathHelper.ToRadians(projectileInitialVelocity.X * 0.5f);

            // Check if projectile hit the ground or even passed it 
            // (could happen during normal calculation)
            if (projectilePosition.Y >= 332 + hitOffset)
            {
                projectilePosition.X = previousXPosition;
                projectilePosition.Y = previousYPosition;

                ProjectileHitPosition = new Vector2(previousXPosition, 332);

                State = ProjectileState.HitGround;
            }
        }

        public void Fire(float velocityX, float velocityY)
        {
            // Set initial projectile velocity
            projectilePosition = projectileStartPosition;
            projectileInitialVelocity.X = velocityX;
            projectileInitialVelocity.Y = velocityY;
            currentVelocity.X = velocityX;
            currentVelocity.Y = velocityY;
            // Reset calculation variables
            flightTime = 0;
            State = ProjectileState.InFlight;
            HitHandled = false;
        }

    }
}

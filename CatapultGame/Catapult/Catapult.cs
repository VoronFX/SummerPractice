using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Xml.Linq;

namespace GoblinsGame
{
    public enum CatapultState
    {
        Idle,
        Aiming,
        Firing,
        ProjectileFlying,
        ProjectileHit,
        Hit,
        Reset,
        Stalling,
        ProjectilesFalling,
        HitDamage,
        HitKill
    }

    enum HitCheckResult
    {
        Nothing,
        SelfCatapult,
        EnemyCatapult,
        SelfCrate,
        EnemyCrate
    }

    public class Catapult : DrawableGameComponent
    {
        TheGame curGame = null;

        SpriteBatch spriteBatch;
        Texture2D idleTexture;
        string idleTextureName;

        bool isLeftSide;
        bool isHuman;

        // TODO: Additional catapult fields 

        SpriteEffects spriteEffects;

        Vector2 catapultPosition;
        public Vector2 Position
        {
            get
            {
                return catapultPosition;
            }
        }

        CatapultState currentState;
        public CatapultState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        Random random;

        const int winScore = 5;

        public bool AnimationRunning { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        // In some cases, the game needs to start second animation while first
        // animation is still running;
        // this variable defines at which frame the second animation should start
        Dictionary<string, int> splitFrames;

        // TODO: Animations collection

        // Projectile
        Projectile projectile;

        // Game constants
        public const float gravity = 500f;

        // State of the catapult during its last update
        CatapultState lastUpdateState = CatapultState.Idle;

        // Used to stall animations
        int stallUpdateCycles;
        // Projectiles
        const int MaxActiveProjectiles = 3;

        Projectile normalProjectile;
        Projectile splitProjectile;

        List<Projectile> activeProjectiles; // Projectiles which are active
        List<Projectile> activeProjectilesCopy; // Copy of the above list
        List<Projectile> destroyedProjectiles; // Projectiles which are active

        float wind;
        public float Wind
        {
            set
            {
                wind = value;
            }
            get
            {
                return wind;
            }
        }

        Player enemy;
        internal Player Enemy
        {
            set
            {
                enemy = value;
            }
        }

        Player self;
        internal Player Self
        {
            set
            {
                self = value;
            }
        }

        // Describes how powerful the current shot being fired is. The more 
        // powerful the shot, the further it goes. 0 is the weakest, 1 is the 
        // strongest.
        public float ShotStrength { get; set; }

        public float ShotVelocity { get; set; }

        // <summary>
        /// The angle at which projectiles are fired, in radians.
        /// </summary>
        public float ShotAngle { get; set; }

        public Vector2 ProjectileStartPosition { get; private set; }

        public int GroundHitOffset
        {
            get
            {
                // TODO: Update GroundHitOffset
                return 0;
            }
        }

        // Used to determine whether the game is over
        public bool GameOver { get; set; }

        public Catapult(Game game)
            : base(game)
        {
            curGame = (TheGame)game;
        }

        public Catapult(Game game, SpriteBatch screenSpriteBatch,
            string IdleTexture, Vector2 CatapultPosition,
            SpriteEffects SpriteEffect, bool isLeftSide, bool isHuman)
            : this(game)
        {
            idleTextureName = IdleTexture;
            catapultPosition = CatapultPosition;
            spriteEffects = SpriteEffect;
            spriteBatch = screenSpriteBatch;
            this.isLeftSide = isLeftSide;
            this.isHuman = isHuman;

            if (isLeftSide)
                ProjectileStartPosition = new Vector2(630, 340);
            else
                ProjectileStartPosition = new Vector2(175, 340);

            splitFrames = new Dictionary<string, int>();
            // TODO: Instantiate animations
        }


        public override void Initialize()
        {
            // Define initial state of the catapult
            IsActive = true;
            AnimationRunning = false;
            currentState = CatapultState.Idle;
            stallUpdateCycles = 0;

            // Load multiple animations form XML definition
            XDocument doc = XDocument.Load(
                "Content/Textures/Catapults/AnimationsDef.xml");
            XName name = XName.Get("Definition");
            var definitions = doc.Document.Descendants(name);

            // Loop over all definitions in XML
            foreach (var animationDefinition in definitions)
            {
                bool? toLoad = null;
                bool val;
                if (bool.TryParse(animationDefinition.Attribute("IsAI").Value,
                                  out val))
                    toLoad = val;

                // Check if the animation definition need to be loaded 
                // for current catapult
                if (toLoad == isLeftSide || null == toLoad)
                {
                    // Get a name of the animation
                    string animatonAlias =
                        animationDefinition.Attribute("Alias").Value;
                    Texture2D texture =
                        curGame.Content.Load<Texture2D>(
                            animationDefinition.Attribute("SheetName").Value);

                    // Get the frame size (width & height)
                    Point frameSize = new Point();
                    frameSize.X = int.Parse(
                        animationDefinition.Attribute("FrameWidth").Value);
                    frameSize.Y = int.Parse(
                        animationDefinition.Attribute("FrameHeight").Value);

                    // Get the frames sheet dimensions
                    Point sheetSize = new Point();
                    sheetSize.X = int.Parse(
                        animationDefinition.Attribute("SheetColumns").Value);
                    sheetSize.Y = int.Parse(
                        animationDefinition.Attribute("SheetRows").Value);

                    // If definition has a "SplitFrame" - means that other 
                    // animation should start here - load it
                    if (null != animationDefinition.Attribute("SplitFrame"))
                        splitFrames.Add(animatonAlias,
                            int.Parse(
                              animationDefinition.Attribute("SplitFrame").Value));

                    // Defing animation speed
                    TimeSpan frameInterval = TimeSpan.FromSeconds((float)1 /
                        int.Parse(animationDefinition.Attribute("Speed").Value));

                    // TODO: Construct animations

                    // If definition has an offset defined - means that it should 
                    // be rendered relatively to some element/other animation - 
                    // load it
                    if (null != animationDefinition.Attribute("OffsetX") &&
                      null != animationDefinition.Attribute("OffsetY"))
                    {
                        // TODO: Set animation offset             
                    }
                    // TODO: Add animation alias
                }
            }

            // Load the textures
            idleTexture = curGame.Content.Load<Texture2D>(idleTextureName);

            activeProjectiles = new List<Projectile>(MaxActiveProjectiles);
            activeProjectilesCopy = new List<Projectile>(MaxActiveProjectiles);
            destroyedProjectiles = new List<Projectile>(MaxActiveProjectiles);

            // TODO: Update normalProjectile instantiation 
            normalProjectile = new Projectile(curGame, spriteBatch,
                activeProjectiles, "Textures/Ammo/rock_ammo",
                ProjectileStartPosition, 0, isLeftSide, gravity);
            normalProjectile.Initialize();

            // TODO: Add split projectile 

            // TODO: Add SupplyCrate 

            // Initialize randomizer
            random = new Random();

            base.Initialize();
        }


        public override void Draw(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            //Using the last update state makes sure we do
            // not draw before updating animations properly
            switch (lastUpdateState)
            {
                case CatapultState.ProjectilesFalling:
                case CatapultState.Idle:
                case CatapultState.Reset:
                    DrawIdleCatapult();
                    break;
                case CatapultState.Aiming:
                case CatapultState.Stalling:
                    //TODO: Handle stalling                   
                    break;
                case CatapultState.Firing:
                    //TODO: Handle firing                                       
                    break;
                case CatapultState.HitDamage:
                    // Draw the catapult
                    DrawIdleCatapult();
                    break;
                case CatapultState.HitKill:
                    //TODO: Handle hit kill                                    
                    break;
                default:
                    break;
            }

            // Draw projectiles
            foreach (var projectile in activeProjectiles)
            {
                projectile.Draw(gameTime);
            }

            //TODO: Draw supply crate

            base.Draw(gameTime);
        }


        public override void Update(GameTime gameTime)
        {
            bool startStall;
            CatapultState postUpdateStateChange = 0;

            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            // The catapult is inactive, so there is nothing to update
            if (!IsActive)
            {
                base.Update(gameTime);
                return;
            }

            switch (currentState)
            {
                case CatapultState.Idle:
                    // Nothing to do
                    break;
                case CatapultState.Aiming:
                    if (lastUpdateState != CatapultState.Aiming)
                    {
                        // TODO: Play rope stretch

                        AnimationRunning = true;
                        if (isLeftSide == true && !isHuman)
                        {
                            // TODO: Play aim animation
                            stallUpdateCycles = 20;
                            startStall = false;
                        }
                    }

                    // Progress Aiming "animation"
                    if (isHuman)
                    {
                        // TODO: Update aim
                    }
                    else
                    {
                        // TODO: Update aim
                        // TODO: Aim reach shot strength
                        currentState = (true) ?
                            CatapultState.Stalling : CatapultState.Aiming;
                    }
                    break;
                case CatapultState.Stalling:
                    if (stallUpdateCycles-- <= 0)
                    {
                        // We've finished stalling, fire the projectile
                        Fire(ShotVelocity, ShotAngle);
                        postUpdateStateChange = CatapultState.Firing;
                    }
                    break;
                case CatapultState.Firing:
                    // Progress Fire animation
                    if (lastUpdateState != CatapultState.Firing)
                    {
                        // TODO: Play firing sounds
                    }

                    // TODO: Play firing animation

                    // TODO: Fire at the appropriate animation frame
                    postUpdateStateChange = currentState |
                        CatapultState.ProjectileFlying;
                    Fire(ShotVelocity, ShotAngle);
                    break;
                case CatapultState.ProjectilesFalling:
                    // End turn if all projectiles have been destroyed
                    // TODO: Handle projectiles falling
                    break;
                case CatapultState.HitDamage:
                    // TODO: Handle hit damage
                    break;
                case CatapultState.HitKill:
                    // Progress hit animation
                    // TODO: Handle hit kill
                    break;
                case CatapultState.Reset:
                    AnimationRunning = false;
                    break;
                default:
                    break;
            }

            lastUpdateState = currentState;
            if (postUpdateStateChange != 0)
            {
                currentState = postUpdateStateChange;
            }

            // TODO: Update active projectiles

            base.Update(gameTime);
        }

        /// <summary>
        /// Performs all logic necessary when a projectile hits the ground.
        /// </summary>
        /// <param name="projectile"></param>
        private void HandleProjectileHit(Projectile projectile)
        {
            projectile.HitHandled = true;

            switch (CheckHit(projectile))
            {
                case HitCheckResult.SelfCrate:
                // Ignore self crate hits
                case HitCheckResult.Nothing:
                    PerformNothingHit(projectile);
                    break;
                case HitCheckResult.SelfCatapult:
                    if ((CurrentState == CatapultState.HitKill) ||
                        (CurrentState == CatapultState.HitDamage))
                    {
                        // TODO: Play smoke animation 
                    }
                    break;
                case HitCheckResult.EnemyCatapult:
                    if ((enemy.Catapult.CurrentState == CatapultState.HitKill) ||
                        (enemy.Catapult.CurrentState == CatapultState.HitDamage))
                    {
                        // TODO: Play smoke animation
                    }
                    else
                    {
                        PerformNothingHit(projectile);
                    }
                    break;
                case HitCheckResult.EnemyCrate:
                    // TODO: Handle EnemyCrate hit
                    break;
                default:
                    throw new InvalidOperationException("Hit invalid entity");
            }

            // TODO: Play hit animation
        }

        /// <summary>
        /// Check what a projectile hit. The possibilities are:
        /// Nothing hit, Hit enemy, Hit self, hit own/enemy's crate.
        /// </summary>
        /// <param name="projectile">The projectile for which to 
        /// perform the check.</param>
        /// <returns>A result inidicating what, if anything, was hit</returns>
        private HitCheckResult CheckHit(Projectile projectile)
        {
            HitCheckResult hitRes = HitCheckResult.Nothing;

            // Build a sphere around a projectile
            Vector3 center = new Vector3(projectile.ProjectilePosition, 0);
            BoundingSphere sphere = new BoundingSphere(center,
                Math.Max(projectile.ProjectileTexture.Width / 2,
                projectile.ProjectileTexture.Height / 2));

            // Check Self-Hit - create a bounding box around self
            Vector3 min = new Vector3(catapultPosition, 0);
            Vector3 max = new Vector3(catapultPosition +
                new Vector2(0, 0), 0);
            // TODO: Support fire size
            BoundingBox selfBox = new BoundingBox(min, min);

            // Check enemy - create a bounding box around the enemy
            min = new Vector3(enemy.Catapult.Position, 0);
            max = new Vector3(enemy.Catapult.Position +
                      new Vector2(0, 0), 0);
            // TODO: Support fire size
            BoundingBox enemyBox = new BoundingBox(min, min);

            // Check self-crate - Create bounding box around own crate
            // TODO: Check self crate

            // Check enemy-crate - Create bounding box around enemy crate
            // TODO: Check enemy crate

            // Check self hit
            if (sphere.Intersects(selfBox)
                && currentState != CatapultState.HitKill)
            {
                // TODO: Play explosion sound
                // Launch hit animation sequence on self
                UpdateHealth(self, sphere, selfBox);
                if (self.Health <= 0)
                {
                    Hit(true);
                    enemy.Score++;
                }

                hitRes = HitCheckResult.SelfCatapult;
            }
            // Check if enemy was hit
            else if (sphere.Intersects(enemyBox)
                && enemy.Catapult.CurrentState != CatapultState.HitKill
                && enemy.Catapult.CurrentState != CatapultState.Reset)
            {
                // TODO: Play explosion sound
                // Launch enemy hit animaton
                UpdateHealth(enemy, sphere, enemyBox);
                if (enemy.Health <= 0)
                {
                    enemy.Catapult.Hit(true);
                    self.Score++;
                }

                hitRes = HitCheckResult.EnemyCatapult;
                currentState = CatapultState.Reset;
            }
            // Check if own crate was hit
            // TODO: Handle crate checking

            return hitRes;
        }

        /// <summary>
        /// Updates the health status of the player based on hit area
        /// </summary>
        /// <param name="enemy"></param>
        private void UpdateHealth(Player player, BoundingSphere projectile, BoundingBox catapult)
        {
            bool isHit = false;

            float midPoint = (catapult.Max.X - catapult.Min.X) / 2;
            BoundingBox catapultCenter = new BoundingBox(
                new Vector3(catapult.Min.X + midPoint - projectile.Radius,
                    projectile.Center.Y - projectile.Radius, 0),
                new Vector3(catapult.Min.X + midPoint + projectile.Radius,
                    projectile.Center.Y + projectile.Radius, 0));

            BoundingBox catapultLeft = new BoundingBox(
                new Vector3(catapult.Min.X, projectile.Center.Y -
                    projectile.Radius, 0),
                new Vector3(catapult.Min.X + midPoint - projectile.Radius,
                    projectile.Center.Y + projectile.Radius, 0));

            BoundingBox catapultRight = new BoundingBox(
                new Vector3(catapult.Min.X + midPoint + projectile.Radius,
                    projectile.Center.Y - projectile.Radius, 0),
                new Vector3(catapult.Max.X, projectile.Center.Y +
                    projectile.Radius, 0));

            if (projectile.Intersects(catapultCenter))
            {
                player.Health -= 75;
                isHit = true;
            }
            else if (projectile.Intersects(catapultLeft))
            {
                player.Health -= isLeftSide ? 50 : 25;
                isHit = true;
            }

            else if (projectile.Intersects(catapultRight))
            {
                player.Health -= isLeftSide ? 25 : 50;
                isHit = true;
            }

            if (isHit)
            {
                player.Catapult.Hit(false);

                // Catapult hit - start longer vibration on any catapult hit 
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(
                        "Windows.Phone.Devices.Notification.VibrationDevice"))
                {
                    Windows.Phone.Devices.Notification
                        .VibrationDevice.GetDefault().Vibrate(
                            TimeSpan.FromMilliseconds(250));
                }
            }
        }

        private void PerformNothingHit(Projectile projectile)
        {
            if (Windows.Foundation.Metadata.ApiInformation
                   .IsTypePresent(
                       "Windows.Phone.Devices.Notification.VibrationDevice"))
            {
                Windows.Phone.Devices.Notification
                    .VibrationDevice.GetDefault()
                        .Vibrate(TimeSpan.FromMilliseconds(100));
                //VibrateController.Default.Start(
                //    TimeSpan.FromMilliseconds(100));
            }
            // Play hit sound only on a missed hit,
            // a direct hit will trigger the explosion sound
            // TODO: Handle miss sounds and animation
        }

        /// <summary>
        /// Start Hit sequence on catapult - could be executed on self or from enemy in case of hit
        /// </summary>
        public void Hit(bool isKilled)
        {
            AnimationRunning = true;
            // TODO: Handle hit animations

            if (isKilled)
                currentState = CatapultState.HitKill;
            else
                currentState = CatapultState.HitDamage;

            self.Weapon = WeaponType.Normal;
        }

        public void Fire(float velocity, float angle)
        {
            Projectile firedProjectile = null;

            switch (self.Weapon)
            {
                case WeaponType.Normal:
                    firedProjectile = normalProjectile;
                    break;
                case WeaponType.Split:
                    firedProjectile = splitProjectile;
                    break;
                default:
                    throw new
                        InvalidOperationException("Firing invalid ammunition");
            }

            // Fire the projectile
            firedProjectile.ProjectilePosition =
                firedProjectile.ProjectileStartPosition;
            firedProjectile.Fire(
                velocity * (float)Math.Cos(angle),
                velocity * (float)Math.Sin(angle));
            firedProjectile.Wind = wind;
            activeProjectiles.Add(firedProjectile);
        }

        private void DrawIdleCatapult()
        {
            spriteBatch.Draw(idleTexture, catapultPosition, null, Color.White,
                0.0f, Vector2.Zero, 1.0f,
                spriteEffects, 0);
        }

    }
}

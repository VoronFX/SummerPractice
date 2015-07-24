using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace GoblinsGame
{
    class GameplayScreenBattle : GameScreen
    {
        // Texture Members
        Texture2D foregroundTexture;

        SpriteFont hudFont;

    


        // Gameplay members

        Random random;
        const int minWind = 0;
        const int maxWind = 10;

        // Helper members
        bool isDragging;
        private bool gameOver;

        public void LoadAssets()
        {
            // Load textures
            foregroundTexture =
                Load<Texture2D>("Textures/Backgrounds/gameplay_screen");


            // Load font
            hudFont = Load<SpriteFont>("Fonts/HUDFont");



            // TODO: Define intial HUD positions and Initialize human & AI players





        }

        public override void LoadContent()
        {
            LoadAssets();
            // TODO: Start the game
            Start();

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ScreenManager.SpriteBatch.Begin();

            // Render all parts of the screen
            DrawBackground();
            // DrawComputer(gameTime);
            // TODO: Draw players
          

            DrawHud();

            ScreenManager.SpriteBatch.End();
        }

        private void DrawBackground()
        {
            // Clear the background
            ScreenManager.Game.GraphicsDevice.Clear(Color.White);

            // Draw the Sky

            // Draw the Castle, trees, and foreground 
            ScreenManager.SpriteBatch.Draw(foregroundTexture,
                Vector2.Zero, Color.White);
        }

        void Start()
        {
            // Set initial wind direction

        }

        // A simple helper to draw shadowed text.
        void DrawString(SpriteFont font, string text, Vector2 position, Color color)
        {
            ScreenManager.SpriteBatch.DrawString(font, text,
                new Vector2(position.X + 1, position.Y + 1), Color.Black);
            ScreenManager.SpriteBatch.DrawString(font, text, position, color);
        }

        // A simple helper to draw shadowed text.
        void DrawString(SpriteFont font, string text, Vector2 position, Color color, float fontScale)
        {
            ScreenManager.SpriteBatch.DrawString(font, text,
                new Vector2(position.X + 1, position.Y + 1),
                Color.Black, 0, new Vector2(0, font.LineSpacing / 2),
                fontScale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.DrawString(font, text, position,
                color, 0, new Vector2(0, font.LineSpacing / 2),
                fontScale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw the HUD, which consists of the score elements and the GAME OVER tag.
        /// </summary>
        void DrawHud()
        {
            // Draw Player Hud





        }


      

        public GameplayScreenBattle()
        {
            EnabledGestures = GestureType.FreeDrag |
                GestureType.DragComplete |
                GestureType.Tap;

            random = new Random();
            

        }

        /// <summary>
        /// Runs one frame of update for the game.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;



            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }



        /// <summary>
        /// Input helper method provided by GameScreen.  Packages up the various /// input values for ease of use.
        /// </summary>
        /// <param name="input">The state of the gamepads</param>
        public override void HandleInput(InputState input)
        {
            PlayerIndex player;

            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsNewKeyPress(Keys.F12, out player))
            {
                if (!Windows.UI.ViewManagement.ApplicationView
                        .GetForCurrentView().IsFullScreen)
                    Windows.UI.ViewManagement.ApplicationView
                        .GetForCurrentView().TryEnterFullScreenMode();
                else
                    Windows.UI.ViewManagement.ApplicationView
                        .GetForCurrentView().ExitFullScreenMode();
            }

            if (gameOver)
            {
                if (input.IsPauseGame())
                {
                    FinishCurrentGame();
                }

                if (input.IsNewKeyPress(Keys.Space, out player)
                    || input.IsNewKeyPress(Keys.Enter, out player))
                {
                    FinishCurrentGame();
                }

                if (input.IsNewGamePadButtonPress(Buttons.A, out player)
                    || input.IsNewGamePadButtonPress(Buttons.Start, out player))
                {
                    FinishCurrentGame();
                }

                if (input.IsNewMouseButtonPress(MouseButtons.LeftButton,
                                                out player))
                {
                    FinishCurrentGame();
                }

                foreach (GestureSample gestureSample in input.Gestures)
                {
                    if (gestureSample.GestureType == GestureType.Tap)
                    {
                        FinishCurrentGame();
                    }
                }

                return;
            }


        }

        private void FinishCurrentGame()
        {
            ExitScreen();
        }

        private void PauseCurrentGame()
        {
            // TODO: Pause the game
        }
    }
}

       

    
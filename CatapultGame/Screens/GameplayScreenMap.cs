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
using CatapultGame.Screens;
using System.IO;
using Newtonsoft.Json;
using CatapultGame.Players;

namespace GoblinsGame
{
    class GameplayScreenMap : GameScreen
    {
        // Texture Members
        //Texture2D foregroundTexture;
        //Texture2D cloud1Texture;
        //Texture2D cloud2Texture;
        //Texture2D mountainTexture;
        //Texture2D skyTexture;
        //Texture2D hudBackgroundTexture;
        //Texture2D ammoTypeNormalTexture;
        //Texture2D ammoTypeSplitTexture;
        //Texture2D windArrowTexture;
        //Texture2D defeatTexture;
        //Texture2D victoryTexture;
        //Texture2D blankTexture;
        Texture2D grassTexture;
        Texture2D waterTexture;
        Texture2D mountTexture;
        Texture2D heroTexture;
        Ground[] grounds = new Ground[3];

        List<List<Ground>> map = new List<List<Ground>>();
        Rectangle rect;
        int levelLength = 128;
        int blockLength = 50;
        Hero player;
        //SpriteFont hudFont;
        //List<Ground> grounds;  


        // Rendering members
        //Vector2 cloud1Position;
        //Vector2 cloud2Position;

        //Vector2 playerOneHUDPosition;
        //Vector2 playerTwoHUDPosition;
        //Vector2 windArrowPosition;
        //Vector2 playerOneHealthBarPosition;
        //Vector2 playerTwoHealthBarPosition;
        //Vector2 healthBarFullSize;

        // Gameplay members
        //Human playerOne;
        //Player playerTwo;
        //Vector2 wind;
        //bool changeTurn;
        //bool isFirstPlayerTurn;
        //bool isTwoHumanPlayers;
        //bool gameOver;
        //Random random;
        //const int minWind = 0;
        //const int maxWind = 10;

        // Helper members
        //bool isDragging;

        //public object File { get; private set; }

        //public void LoadAssets()
        //{
        //    // Load textures
        //    foregroundTexture =
        //        Load<Texture2D>("Textures/Backgrounds/gameplay_screen");
        //    cloud1Texture = Load<Texture2D>("Textures/Backgrounds/cloud1");
        //    cloud2Texture = Load<Texture2D>("Textures/Backgrounds/cloud2");
        //    mountainTexture = Load<Texture2D>("Textures/Backgrounds/mountain");
        //    skyTexture = Load<Texture2D>("Textures/Backgrounds/sky");
        //    defeatTexture = Load<Texture2D>("Textures/Backgrounds/defeat");
        //    victoryTexture = Load<Texture2D>("Textures/Backgrounds/victory");
        //    hudBackgroundTexture = Load<Texture2D>("Textures/HUD/hudBackground");
        //    windArrowTexture = Load<Texture2D>("Textures/HUD/windArrow");
        //    ammoTypeNormalTexture = Load<Texture2D>("Textures/HUD/ammoTypeNormal");
        //    ammoTypeSplitTexture = Load<Texture2D>("Textures/HUD/ammoTypeSplit");
        //    blankTexture = Load<Texture2D>("Textures/Backgrounds/blank");

        //    // Load font
        //    hudFont = Load<SpriteFont>("Fonts/HUDFont");

        //    // Define initial cloud position
        //    cloud1Position = new Vector2(224 - cloud1Texture.Width, 32);
        //    cloud2Position = new Vector2(64, 90);

        //    // TODO: Define intial HUD positions and Initialize human & AI players
        //    // Define initial HUD positions
        //    playerOneHUDPosition = new Vector2(7, 7);
        //    playerTwoHUDPosition = new Vector2(613, 7);
        //    windArrowPosition = new Vector2(345, 46);
        //    Vector2 healthBarOffset = new Vector2(25, 82);
        //    playerOneHealthBarPosition = playerOneHUDPosition + healthBarOffset;
        //    playerTwoHealthBarPosition = playerTwoHUDPosition + healthBarOffset;
        //    healthBarFullSize = new Vector2(130, 20);

        //    // Initialize human & AI players
        //    playerOne = new Human(ScreenManager.Game, ScreenManager.SpriteBatch,
        //        PlayerSide.Left);
        //    playerOne.Initialize();
        //    playerOne.Name = "Player" + (isTwoHumanPlayers ? " 1" : "");

        //    if (isTwoHumanPlayers)
        //    {
        //        playerTwo = new Human(ScreenManager.Game, ScreenManager.SpriteBatch,
        //            PlayerSide.Right);
        //        playerTwo.Initialize();
        //        playerTwo.Name = "Player 2";
        //    }
        //    else
        //    {
        //        playerTwo = new AI(ScreenManager.Game, ScreenManager.SpriteBatch);
        //        playerTwo.Initialize();
        //        playerTwo.Name = "AI";
        //    }


        //    // TODO: Add enemies
        //    playerOne.Enemy = playerTwo;
        //    playerTwo.Enemy = playerOne;

        //}


        public GameplayScreenMap()
        {
        }
        void CreateLevel()
        {
            grounds[0] = new Ground(grassTexture, 0);
            grounds[1] = new Ground(waterTexture, 1);
            grounds[2] = new Ground(mountTexture, 2);
            Random rand = new Random();
            for(int i=0; i<levelLength; i++)
            {
                map.Add(new List<Ground>());
                for (int j = 0; j < levelLength; j++)
                {
                    map[i].Add(grounds[rand.Next(3)]);
                }
            }
            //for(int i = 0; i< levelLength; i++)
            //{
            //    map[i].Add(grounds[rand.Next(3)]);
            //}
        }


        public override void LoadContent()
        {
            //LoadAssets();
            // TODO: Start the game

            //Start();
            grassTexture = Load<Texture2D>("grass");
            mountTexture = Load<Texture2D>("mount");
            waterTexture = Load<Texture2D>("water");
            heroTexture = Load<Texture2D>("hero");

            rect = new Rectangle(0, 0, blockLength, blockLength);
            player = new Hero(heroTexture);
            CreateLevel();
            base.LoadContent();
        }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            MouseState state = Mouse.GetState();
            if(state.LeftButton == ButtonState.Pressed)

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        public override void Draw(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ScreenManager.SpriteBatch.Begin();

            int x = 0, y = 0;
            for (int i = 0; i < levelLength; i++)
            {             
                for (int j = 0; j < levelLength; j++)
                {
                    x = j * blockLength;
                    rect.Location = new Point(x, y);
                    ScreenManager.SpriteBatch.Draw(map[i][j].Texture, rect, Color.White);
                }
                x = 0;
                y = i * blockLength;
            }
            ScreenManager.SpriteBatch.Draw(player.Texture, new Rectangle(0,0,blockLength,blockLength),Color.White);
            // Render all parts of the screen
            //DrawBackground();
            //// DrawComputer(gameTime);
            //// TODO: Draw players
            //DrawPlayerTwo(gameTime);
            //DrawPlayerOne(gameTime);
            //DrawHud();



            ScreenManager.SpriteBatch.End();
        }



        //private void DrawBackground()
        //{
        //    // Clear the background
        //    ScreenManager.Game.GraphicsDevice.Clear(Color.White);

        //    // Draw the Sky
        //    ScreenManager.SpriteBatch.Draw(skyTexture, Vector2.Zero, Color.White);

        //    // Draw Cloud #1
        //    ScreenManager.SpriteBatch.Draw(cloud1Texture,
        //        cloud1Position, Color.White);

        //    // Draw the Mountain
        //    ScreenManager.SpriteBatch.Draw(mountainTexture,
        //        Vector2.Zero, Color.White);

        //    // Draw Cloud #2
        //    ScreenManager.SpriteBatch.Draw(cloud2Texture,
        //        cloud2Position, Color.White);

        //    // Draw the Castle, trees, and foreground 
        //    ScreenManager.SpriteBatch.Draw(foregroundTexture,
        //        Vector2.Zero, Color.White);
        //}

        //void Start()
        //{
        //    // Set initialf wind direction
        //    wind = Vector2.Zero;
        //    isFirstPlayerTurn = false;
        //    changeTurn = true;
        //    // TODO: Reset catapult state
        //    playerTwo.Catapult.CurrentState = CatapultState.Reset;
        //}

        //// A simple helper to draw shadowed text.
        //void DrawString(SpriteFont font, string text, Vector2 position, Color color)
        //{
        //    ScreenManager.SpriteBatch.DrawString(font, text,
        //        new Vector2(position.X + 1, position.Y + 1), Color.Black);
        //    ScreenManager.SpriteBatch.DrawString(font, text, position, color);
        //}

        //// A simple helper to draw shadowed text.
        //void DrawString(SpriteFont font, string text, Vector2 position, Color color, float fontScale)
        //{
        //    ScreenManager.SpriteBatch.DrawString(font, text,
        //        new Vector2(position.X + 1, position.Y + 1),
        //        Color.Black, 0, new Vector2(0, font.LineSpacing / 2),
        //        fontScale, SpriteEffects.None, 0);
        //    ScreenManager.SpriteBatch.DrawString(font, text, position,
        //        color, 0, new Vector2(0, font.LineSpacing / 2),
        //        fontScale, SpriteEffects.None, 0);
        //}

        ///// <summary>
        ///// Draw the HUD, which consists of the score elements and the GAME OVER tag.
        ///// </summary>
        //void DrawHud()
        //{
        //    // Draw Player Hud
        //    ScreenManager.SpriteBatch.Draw(hudBackgroundTexture,
        //        playerOneHUDPosition, Color.White);
        //    ScreenManager.SpriteBatch.Draw(GetWeaponTexture(playerOne),
        //        playerOneHUDPosition + new Vector2(33, 35), Color.White);
        //    DrawString(hudFont, playerOne.Score.ToString(),
        //        playerOneHUDPosition + new Vector2(123, 35), Color.White);
        //    DrawString(hudFont, playerOne.Name,
        //        playerOneHUDPosition + new Vector2(40, 1), Color.Blue);

        //    Rectangle rect = new Rectangle((int)playerOneHealthBarPosition.X,
        //                            (int)playerOneHealthBarPosition.Y,
        //                            (int)healthBarFullSize.X * playerOne.Health / 100,
        //                            (int)healthBarFullSize.Y);
        //    Rectangle underRect = new Rectangle(rect.X, rect.Y, rect.Width + 1,
        //                                        rect.Height + 1);
        //    ScreenManager.SpriteBatch.Draw(blankTexture, underRect, Color.Black);
        //    ScreenManager.SpriteBatch.Draw(blankTexture, rect, Color.Blue);


        //    // Draw Computer Hud
        //    ScreenManager.SpriteBatch.Draw(hudBackgroundTexture,
        //        playerTwoHUDPosition, Color.White);
        //    ScreenManager.SpriteBatch.Draw(GetWeaponTexture(playerTwo),
        //        playerTwoHUDPosition + new Vector2(33, 35), Color.White);
        //    DrawString(hudFont, playerTwo.Score.ToString(),
        //        playerTwoHUDPosition + new Vector2(123, 35), Color.White);
        //    DrawString(hudFont, playerTwo.Name,
        //        playerTwoHUDPosition + new Vector2(40, 1), Color.Red);

        //    rect = new Rectangle((int)playerTwoHealthBarPosition.X,
        //                         (int)playerTwoHealthBarPosition.Y,
        //                         (int)healthBarFullSize.X * playerTwo.Health / 100,
        //                         (int)healthBarFullSize.Y);
        //    underRect = new Rectangle(rect.X, rect.Y, rect.Width + 1,
        //                              rect.Height + 1);
        //    ScreenManager.SpriteBatch.Draw(blankTexture, underRect, Color.Black);
        //    ScreenManager.SpriteBatch.Draw(blankTexture, rect, Color.Red);

        //    // Draw Wind direction
        //    string text = "WIND";
        //    Vector2 size = hudFont.MeasureString(text);
        //    Vector2 windarrowScale = new Vector2(wind.Y / 10, 1);
        //    ScreenManager.SpriteBatch.Draw(windArrowTexture,
        //                                  windArrowPosition, null, Color.White, 0,
        //                                  Vector2.Zero, windarrowScale,
        //                                  wind.X > 0 ? SpriteEffects.None
        //                                             : SpriteEffects.FlipHorizontally,
        //                                  0);

        //    DrawString(hudFont, text,
        //               windArrowPosition - new Vector2(0, size.Y), Color.Black);
        //    if (wind.Y == 0)
        //    {
        //        text = "NONE";
        //        DrawString(hudFont, text, windArrowPosition, Color.Black);
        //    }

        //    // If first player turn
        //    if (isFirstPlayerTurn)
        //    {
        //        // Prepare first player prompt message
        //        text = !isDragging ? (isTwoHumanPlayers ? "Player 1, " : "")
        //                                  + "Drag Anywhere to Fire"
        //                           : "Release to Fire!";
        //        size = hudFont.MeasureString(text);
        //    }
        //    else
        //    {
        //        // Prepare second player prompt message
        //        if (isTwoHumanPlayers)
        //            text = !isDragging ? "Player 2, Drag Anywhere to Fire!"
        //                               : "Release to Fire!";
        //        else
        //            text = "I'll get you yet!";

        //        size = hudFont.MeasureString(text);
        //    }

        //    DrawString(hudFont, text,
        //        new Vector2(
        //            ScreenManager.GraphicsDevice.Viewport.Width / 2 - size.X / 2,
        //            ScreenManager.GraphicsDevice.Viewport.Height - size.Y),
        //            Color.Green);

        //}

        ///// <summary>
        ///// Returns the texture appropriate for the player's current weapon
        ///// </summary>
        ///// <param name="player">The player for which to get the texture</param>
        ///// <returns>Ammo texture to draw in the HUD</returns>
        //private Texture2D GetWeaponTexture(Player player)
        //{
        //    switch (player.Weapon)
        //    {
        //        case WeaponType.Normal:
        //            return ammoTypeNormalTexture;
        //        case WeaponType.Split:
        //            return ammoTypeSplitTexture;
        //        default:
        //            throw new ArgumentException("Player has invalid weapon type",
        //                "player");
        //    }
        //}

        //void DrawPlayerOne(GameTime gameTime)
        //{
        //    if (!gameOver)
        //        playerOne.Draw(gameTime);
        //}

        //void DrawPlayerTwo(GameTime gameTime)
        //{
        //    if (!gameOver)
        //        playerTwo.Draw(gameTime);
        //}

        //public GameplayScreenMap(bool twoHumans)
        //{
        //    EnabledGestures = GestureType.FreeDrag |
        //        GestureType.DragComplete |
        //        GestureType.Tap;

        //    random = new Random();

        //    isTwoHumanPlayers = twoHumans;
        //}

        ///// <summary>
        ///// Runs one frame of update for the game.
        ///// </summary>
        ///// <param name="gameTime">Provides a snapshot of timing values.</param>
        //public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        //{
        //    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        //    // Check it one of the players reached 5 and stop the game
        //    if ((playerOne.Catapult.GameOver || playerTwo.Catapult.GameOver) &&
        //        (gameOver == false))
        //    {
        //        gameOver = true;

        //        if (playerOne.Score > playerTwo.Score)
        //        {
        //            // TODO: Play win sound 
        //        }
        //        else
        //        {
        //            // TODO: Play lose sound
        //        }

        //        return;
        //    }

        //    // If Reset flag raised and both catapults are not animating - 
        //    // active catapult finished the cycle, new turn!
        //    if ((playerOne.Catapult.CurrentState == CatapultState.Reset ||
        //        playerTwo.Catapult.CurrentState == CatapultState.Reset) &&
        //        !(playerOne.Catapult.AnimationRunning ||
        //        playerTwo.Catapult.AnimationRunning))
        //    {
        //        changeTurn = true;

        //        if (playerOne.IsActive == true)
        //        //Last turn was a left player turn?
        //        {
        //            playerOne.IsActive = false;
        //            playerTwo.IsActive = true;
        //            isFirstPlayerTurn = false;
        //            playerOne.Catapult.CurrentState = CatapultState.Idle;
        //            if (!isTwoHumanPlayers)
        //                playerTwo.Catapult.CurrentState = CatapultState.Aiming;
        //            else
        //                playerTwo.Catapult.CurrentState = CatapultState.Idle;
        //        }
        //        else //It was an right player turn
        //        {
        //            playerOne.IsActive = true;
        //            playerTwo.IsActive = false;
        //            isFirstPlayerTurn = true;
        //            playerTwo.Catapult.CurrentState = CatapultState.Idle;
        //            playerOne.Catapult.CurrentState = CatapultState.Idle;
        //        }
        //    }

        //    if (changeTurn)
        //    {
        //        // Update wind
        //        wind = new Vector2(random.Next(-1, 2),
        //            random.Next(minWind, maxWind + 1));

        //        // Set new wind value to the players and 
        //        playerOne.Catapult.Wind = playerTwo.Catapult.Wind =
        //            wind.X > 0 ? wind.Y : -wind.Y;
        //        changeTurn = false;
        //    }

        //    // Update the players
        //    playerOne.Update(gameTime);
        //    playerTwo.Update(gameTime);

        //    // Updates the clouds position
        //    UpdateClouds(elapsed);

        //    base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        //}

        //private void UpdateClouds(float elapsedTime)
        //{
        //    // Move the clouds according to the wind
        //    int windDirection = wind.X > 0 ? 1 : -1;

        //    cloud1Position += new Vector2(24.0f, 0.0f) * elapsedTime *
        //        windDirection * wind.Y;
        //    if (cloud1Position.X > ScreenManager.GraphicsDevice.Viewport.Width)
        //        cloud1Position.X = -cloud1Texture.Width * 2.0f;
        //    else if (cloud1Position.X < -cloud1Texture.Width * 2.0f)
        //        cloud1Position.X = ScreenManager.GraphicsDevice.Viewport.Width;

        //    cloud2Position += new Vector2(16.0f, 0.0f) * elapsedTime *
        //        windDirection * wind.Y;
        //    if (cloud2Position.X > ScreenManager.GraphicsDevice.Viewport.Width)
        //        cloud2Position.X = -cloud2Texture.Width * 2.0f;
        //    else if (cloud2Position.X < -cloud2Texture.Width * 2.0f)
        //        cloud2Position.X = ScreenManager.GraphicsDevice.Viewport.Width;
        //}

        ///// <summary>
        ///// Input helper method provided by GameScreen.  Packages up the various /// input values for ease of use.
        ///// </summary>
        ///// <param name="input">The state of the gamepads</param>
        //public override void HandleInput(InputState input)
        //{
        //    PlayerIndex player;

        //    if (input == null)
        //        throw new ArgumentNullException("input");

        //    if (input.IsNewKeyPress(Keys.F12, out player))
        //    {
        //        if (!Windows.UI.ViewManagement.ApplicationView
        //                .GetForCurrentView().IsFullScreen)
        //            Windows.UI.ViewManagement.ApplicationView
        //                .GetForCurrentView().TryEnterFullScreenMode();
        //        else
        //            Windows.UI.ViewManagement.ApplicationView
        //                .GetForCurrentView().ExitFullScreenMode();
        //    }

        //    if (gameOver)
        //    {
        //        if (input.IsPauseGame())
        //        {
        //            FinishCurrentGame();
        //        }

        //        if (input.IsNewKeyPress(Keys.Space, out player)
        //            || input.IsNewKeyPress(Keys.Enter, out player))
        //        {
        //            FinishCurrentGame();
        //        }

        //        if (input.IsNewGamePadButtonPress(Buttons.A, out player)
        //            || input.IsNewGamePadButtonPress(Buttons.Start, out player))
        //        {
        //            FinishCurrentGame();
        //        }

        //        if (input.IsNewMouseButtonPress(MouseButtons.LeftButton,
        //                                        out player))
        //        {
        //            FinishCurrentGame();
        //        }

        //        foreach (GestureSample gestureSample in input.Gestures)
        //        {
        //            if (gestureSample.GestureType == GestureType.Tap)
        //            {
        //                FinishCurrentGame();
        //            }
        //        }

        //        return;
        //    }

        //    if (input.IsPauseGame())
        //    {
        //        PauseCurrentGame();
        //    }
        //    else if (isFirstPlayerTurn &&
        //        (playerOne.Catapult.CurrentState == CatapultState.Idle ||
        //            playerOne.Catapult.CurrentState == CatapultState.Aiming))
        //    {
        //        //Read keyboard input
        //        playerOne.HandleKeybordInput(input.CurrentKeyboardState);

        //        //Read gamepad input
        //        playerOne.HandleGamePadInput(input.CurrentGamePadState);

        //        //Read mouse input
        //        playerOne.HandleMouseInput(input.CurrentMouseState,
        //                                   input.LastMouseState);

        //        if (input.Gestures.Count > 0)
        //        {
        //            // Read all available gestures
        //            foreach (GestureSample gestureSample in input.Gestures)
        //            {
        //                if (gestureSample.GestureType == GestureType.FreeDrag)
        //                    isDragging = true;
        //                else if (gestureSample.GestureType
        //                         == GestureType.DragComplete)
        //                    isDragging = false;

        //                playerOne.HandleInput(gestureSample);
        //            }
        //        }
        //    }
        //    else if (isTwoHumanPlayers && !isFirstPlayerTurn &&
        //        (playerTwo.Catapult.CurrentState == CatapultState.Idle ||
        //            playerTwo.Catapult.CurrentState == CatapultState.Aiming))
        //    {
        //        //Read keyboard input
        //        (playerTwo as Human)
        //            .HandleKeybordInput(input.CurrentKeyboardState);

        //        //Read gamepad input
        //        (playerTwo as Human)
        //            .HandleGamePadInput(input.CurrentGamePadState);

        //        //Read mouse input
        //        (playerTwo as Human).HandleMouseInput(input.CurrentMouseState,
        //                                              input.LastMouseState);

        //        if (input.Gestures.Count > 0)
        //        {
        //            // Read all available gestures
        //            foreach (GestureSample gestureSample in input.Gestures)
        //            {
        //                if (gestureSample.GestureType == GestureType.FreeDrag)
        //                    isDragging = true;
        //                else if (gestureSample.GestureType
        //                         == GestureType.DragComplete)
        //                    isDragging = false;

        //                (playerTwo as Human).HandleInput(gestureSample);
        //            }
        //        }
        //    }
        //}

        //private void FinishCurrentGame()
        //{
        //    ExitScreen();
        //}

        //private void PauseCurrentGame()
        //{
        //    // TODO: Pause the game
        //}

    }
}

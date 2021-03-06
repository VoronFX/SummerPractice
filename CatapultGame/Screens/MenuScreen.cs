#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        // the number of pixels to pad above and below menu entries for touch input
        const int menuEntryPadding = 25;

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            // menus generally only need Tap for menu selection
            EnabledGestures = GestureType.Tap;

            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry.
        /// </summary>
        protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            // the hit bounds are the entire width of the screen, and the height of the entry
            // with some additional padding above and below.
            return new Rectangle(
                0,
                (int)entry.Position.Y - menuEntryPadding,
                ScreenManager.GraphicsDevice.Viewport.Width,
                entry.GetHeight(this) + (menuEntryPadding * 2));
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            //Handle GamePad input
            HandleGamePadInput(input);

            //Handle keyboard keypress
            HandleKeyboardInput(input);

            //Handle mouse input
            HandleMouseInput(input);

            //Handle touch
            HandleTouchInput(input);
        }

        public override void HandleTouchInput(InputState input)
        {
            if (input.Gestures.Count > 0)
            {
                // look for any taps that occurred and select any entries that were tapped
                foreach (GestureSample gesture in input.Gestures)
                {
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        Vector2 tapLocation = Vector2.Zero;
                        // convert the position to a Point that we can test against a Rectangle
                        if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                        {
							// TODO: Use screen scaler
                            //tapLocation = new Vector2(gesture.Position.X / (ScreenManager.Game as GoblinsGame.TheGame).ScreenScale, gesture.Position.Y / (ScreenManager.Game as GoblinsGame.TheGame).ScreenScale);
                        }
                        else
                        {
							// TODO: Use screen scaler
                            //tapLocation = new Vector2(gesture.Position.X * ScreenManager.Scaler.Scale.X / (ScreenManager.Game as GoblinsGame.TheGame).ScreenScale, gesture.Position.Y * ScreenManager.Scaler.Scale.Y / (ScreenManager.Game as GoblinsGame.TheGame).ScreenScale);
                        }

                        CheckMenuUnderPoint(tapLocation.ToPoint());
                    }
                }
            }
        }

        public override void HandleMouseInput(InputState input)
        {
            PlayerIndex player;
            //check for any mouse clicks and select any entries that were clicked
            if (input.IsNewMouseButtonPress(MouseButtons.LeftButton, out player))
            {
				// TODO: Check mouse location
                //Vector2 clickLocation = new Vector2(input.CurrentMouseState.Position.X * ScreenManager.Scaler.Scale.X / (ScreenManager.Game as GoblinsGame.TheGame).ScreenScale, input.CurrentMouseState.Position.Y * ScreenManager.Scaler.Scale.Y / (ScreenManager.Game as GoblinsGame.TheGame).ScreenScale);
                //CheckMenuUnderPoint(clickLocation.ToPoint());
            }
        }

        public override void HandleKeyboardInput(InputState input)
        {
            PlayerIndex player;
            if (input.IsNewKeyPress(Keys.Space, out player) || input.IsNewKeyPress(Keys.Enter, out player))
            {
                OnSelectEntry(selectedEntry, PlayerIndex.One);
            }
            else if (input.IsNewKeyPress(Keys.Escape, out player))
            {
                OnCancel(player);
            }
            else if (input.IsNewKeyPress(Keys.Up, out player))
            {
                if (selectedEntry > 0)
                    selectedEntry--;
            }
            else if (input.IsNewKeyPress(Keys.Down, out player))
            {
                if (selectedEntry < menuEntries.Count - 1)
                    selectedEntry++;
            }
            else if (input.IsNewKeyPress(Keys.F12, out player))
            {
                if (!Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsFullScreen)
                    Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                else
                    Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().ExitFullScreenMode();
            }
        }

        public override void HandleGamePadInput(InputState input)
        {
            if (input.GamePadWasConnected)
            {
                PlayerIndex player;
                if (input.IsNewGamePadButtonPress(Buttons.Back, out player) || input.IsNewGamePadButtonPress(Buttons.B, out player))
                {
                    OnCancel(player);
                }
                else if (input.IsNewGamePadButtonPress(Buttons.Start, out player) || input.IsNewGamePadButtonPress(Buttons.A, out player))
                {
                    OnSelectEntry(selectedEntry, PlayerIndex.One);
                }
                else if (input.IsNewGamePadButtonPress(Buttons.LeftThumbstickUp, out player) || input.IsNewGamePadButtonPress(Buttons.DPadUp, out player))
                {
                    if (selectedEntry > 0)
                        selectedEntry--;
                }
                else if (input.IsNewGamePadButtonPress(Buttons.LeftThumbstickDown, out player) || input.IsNewGamePadButtonPress(Buttons.DPadDown, out player))
                {
                    if (selectedEntry < menuEntries.Count - 1)
                        selectedEntry++;
                }
            }
        }

        private void CheckMenuUnderPoint(Point clickLocation)
        {
            //clickLocation -= new Point(ScreenManager.GraphicsDevice.Viewport.X, ScreenManager.GraphicsDevice.Viewport.Y);

            // iterate the entries to see if any were tapped
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                if (GetMenuEntryHitBounds(menuEntry).Contains(clickLocation))
                {
                    // select the entry. since gestures are only available on Windows Phone,
                    // we can safely pass PlayerIndex.One to all entries since there is only
                    // one player on Windows Phone.
                    OnSelectEntry(i, PlayerIndex.One);
                }
            }
        }



        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, ScreenManager.GraphicsDevice.Viewport.Height / 2);

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                // each entry is to be centered horizontally
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry plus our padding
                position.Y += menuEntry.GetHeight(this) + (menuEntryPadding * 2);
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            //if (!Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                spriteBatch.Begin();
            //else
            //    spriteBatch.Begin(transformMatrix: ScreenManager.Camera.GetViewTransformationMatrix());

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                //menuEntry.Position *= ScreenManager.ScalingHelper.Ratio;

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        #endregion
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace RoomGenerator
{
    public class Rooms : Game
    {
        public Rooms()
            : base()
        {
            Utility.Instance.Graphics = new GraphicsDeviceManager(this);

            Utility.Instance.Graphics.PreferredBackBufferWidth = Utility.Instance.ScreenWidth;
            Utility.Instance.Graphics.PreferredBackBufferHeight = Utility.Instance.ScreenHeight;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            //need to set the game before doing anything else
            GameStateManager.Instance.SetGame(this);

            //add each required game state
            GameStateManager.Instance.AddGameState(new TitleMenuState());
            GameStateManager.Instance.AddGameState(new DungeonExploringState());
            GameStateManager.Instance.AddGameState(new EndOfDungeonState());
            GameStateManager.Instance.AddGameState(new PauseGameState());
            GameStateManager.Instance.AddGameState(new NextLevelState());
            GameStateManager.Instance.AddGameState(new PlayerDeathState());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Utility.Instance.SB = new SpriteBatch(GraphicsDevice);

            //standard asset that can be easily tinted for different variations
            Utility.Instance.WhiteSquare = Content.Load<Texture2D>("whiteSquare");

            //standard font sheet with which to draw text
            //TODO: incorporate different fonts with different sizes
            Utility.Instance.SF = Content.Load<SpriteFont>("spritefont");
        }

        protected override void UnloadContent() { Content.Unload(); }

        protected override void Update(GameTime gameTime)
        {
            if (Utility.Instance.QuitGame)
            {
                Exit();
            }

            Utility.Instance.CurrentKeyboardState = Keyboard.GetState();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //only executes code if it has not been previously initialised
            GameStateManager.Instance.InitialiseCurrentState();

            GameStateManager.Instance.UpdateCurrentState(dt);

            Utility.Instance.PreviousKeyboardState = Utility.Instance.CurrentKeyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GameStateManager.Instance.DrawCurrentState();

            base.Draw(gameTime);
        }
    }
}

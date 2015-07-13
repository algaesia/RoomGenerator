using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //when the user pauses the game
    public class PauseGameState : GameState
    {
        public PauseGameState()
            : base("Pause menu")
        {
            m_BackgroundColour = Color.Honeydew;

            //show options for returning to the game or quiting to the main menu
            menuOptions.AddMenuOption(new ReturnToGameOption(new Vector2(100, 100), Color.Black));
            menuOptions.AddMenuOption(new QuitToMenu(new Vector2(200, 100), Color.Black));
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Update(float dt)
        {
            if (Utility.Instance.IsKeyPressed(Keys.Escape))
            {
                //can return to game using menu or key press
                GameStateManager.Instance.ChangeGameState("Dungeon exploring");
            }

            base.Update(dt);
        }

        public override void Draw()
        {
            //draw background colour
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle(0, 0, Utility.Instance.ScreenWidth, Utility.Instance.ScreenHeight), m_BackgroundColour);
            Utility.Instance.SB.DrawString(Utility.Instance.SF, "PAUSE SCREEN", new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.5f), Color.Black);

            base.Draw();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public class TitleMenuState : GameState
    {
        public TitleMenuState()
            : base("Title menu")
        {
            m_BackgroundColour = Color.Red;

            //show options for starting the game or quitting
            menuOptions.AddMenuOption(new PlayGame(new Vector2(100, 100), Color.Black));
            menuOptions.AddMenuOption(new QuitGame(new Vector2(200, 100), Color.Black));
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Draw()
        {
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle(0, 0, Utility.Instance.ScreenWidth, Utility.Instance.ScreenHeight), m_BackgroundColour);
            Utility.Instance.SB.DrawString(Utility.Instance.SF, "TITLE SCREEN", new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.5f), Color.White);

            menuOptions.Draw();
        }
    }
}

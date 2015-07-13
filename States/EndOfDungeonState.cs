using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //TODO: figure out what to display when player reaches the dungeon exit
    public class EndOfDungeonState : GameState
    {
        public EndOfDungeonState()
            : base("End of dungeon")
        {
            m_BackgroundColour = Color.RoyalBlue;
            menuOptions.AddMenuOption(new QuitToMenu(new Vector2(100, 100), Color.Black));
            menuOptions.AddMenuOption(new PlayGame(new Vector2(200, 100), Color.Black));
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Draw()
        {
            Utility.Instance.SB.DrawString(Utility.Instance.SF, "You have reached the end of the dungeon.", new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.5f), Color.Black);

            base.Draw();
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public class PlayerDeathState : GameState
    {
        public PlayerDeathState()
            : base("Player death")
        {
            m_BackgroundColour = Color.LightBlue;

            menuOptions.AddMenuOption(new QuitToMenu(new Vector2(100, 100), Color.Black));
            menuOptions.AddMenuOption(new PlayGame(new Vector2(200, 100), Color.Black));
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Draw()
        {
            //TODO: Need some kind of messaging system to see which enemy attacked the player
            Utility.Instance.SB.DrawString(Utility.Instance.SF, "You have died.\nYou were killed by: ", new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.5f), Color.Black);

            base.Draw();
        }
    }
}

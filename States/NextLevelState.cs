using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //transition between levels
    //TODO: figure out what to display for transition
    public class NextLevelState : GameState
    {
        public NextLevelState()
            : base("Next level")
        {
            m_BackgroundColour = Color.Black;

            //add a menu option to begin the next level
            menuOptions.AddMenuOption(new NextLevel(new Vector2(100, 100), Color.Black));
        }

        public override void Initialise()
        {
            if (!initialised)
            {
                menuOptions.ResetHighlighter();
            }

            base.Initialise();
        }

        public override void Draw()
        {
            //fill background with background colour
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle(0, 0, Utility.Instance.ScreenWidth, Utility.Instance.ScreenHeight), m_BackgroundColour);

            //draw text to represent what the current screen is
            Utility.Instance.SB.DrawString(Utility.Instance.SF, "NEXT LEVEL SCREEN", new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.5f), Color.White);

            base.Draw();
        }
    }
}

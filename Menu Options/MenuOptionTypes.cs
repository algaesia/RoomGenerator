using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //implementation of each type of menu option
    //simply implements what happens when the user
    //selects the particular option in the menu

    public class ReturnToGameOption : MenuOption
    {
        public ReturnToGameOption(Vector2 a_Pos, Color a_TextColour)
            : base("Return to game", a_Pos, a_TextColour)
        {

        }

        public override void Use()
        {
            GameStateManager.Instance.ChangeGameState("Dungeon exploring");
        }
    }

    public class QuitToMenu : MenuOption
    {
        public QuitToMenu(Vector2 a_Pos, Color a_TextColour)
            : base("Quit to menu", a_Pos, a_TextColour)
        {

        }

        public override void Use()
        {
            GameStateManager.Instance.ChangeGameState("Title menu", true);
        }
    }

    public class QuitGame : MenuOption
    {
        public QuitGame(Vector2 a_Pos, Color a_TextColour)
            : base("Quit game", a_Pos, a_TextColour)
        {

        }

        public override void Use()
        {
            Utility.Instance.QuitGame = true;
        }
    }

    public class PlayGame : MenuOption
    {
        public PlayGame(Vector2 a_Pos, Color a_TextColour)
            : base("Play game", a_Pos, a_TextColour)
        {

        }

        public override void Use()
        {
            GameStateManager.Instance.ChangeGameState("Dungeon exploring", true);
        }
    }

    public class NextLevel : MenuOption
    {
        public NextLevel(Vector2 a_Pos, Color a_TextColour)
            : base("Go to next level", a_Pos, a_TextColour)
        {

        }

        public override void Use()
        {
            GameStateManager.Instance.ChangeGameState("Dungeon exploring");
            Dungeon.GenerateNewDungeon();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //each state has a menu manager
    //holds collection of menus that the player
    //can choose from
    //can also choose from vertical or horizontal menu
    //these will determine which set of keys 
    //will do the movement through the menu
    public class StateMenuManager
    {
        List<MenuOption> menuOptions = new List<MenuOption>();
        int currentMenuHighlight = 0;
        bool horizontalMenu = true;

        public StateMenuManager(bool horizontalMenu = true)
        {
            this.horizontalMenu = horizontalMenu;
        }

        public void Update()
        {
            if (Utility.Instance.IsKeyPressed(Keys.Enter))
            {
                menuOptions[currentMenuHighlight].Use();
            }

            if (horizontalMenu)
            {
                if (Utility.Instance.IsKeyPressed(Keys.Left))
                {
                    currentMenuHighlight--;
                }

                if (Utility.Instance.IsKeyPressed(Keys.Right))
                {
                    currentMenuHighlight++;
                }
            }
            else
            {
                if (Utility.Instance.IsKeyPressed(Keys.Up))
                {
                    currentMenuHighlight--;
                }

                if (Utility.Instance.IsKeyPressed(Keys.Down))
                {
                    currentMenuHighlight++;
                }
            }

            if (currentMenuHighlight < 0)
            {
                currentMenuHighlight = menuOptions.Count - 1;
            }

            if (currentMenuHighlight >= menuOptions.Count)
            {
                currentMenuHighlight = 0;
            }
        }

        public void Draw()
        {
            //show current selection
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)menuOptions[currentMenuHighlight].Position.X, (int)menuOptions[currentMenuHighlight].Position.Y, 50, 32), Color.DarkGreen);

            //draw each option contained in the menu for the current state
            foreach (MenuOption n in menuOptions)
            {
                n.Draw();
            }
        }

        public void AddMenuOption(MenuOption a_Option)
        {
            menuOptions.Add(a_Option);
        }

        public void ResetHighlighter()
        {
            currentMenuHighlight = 0;
        }

        public void ResetOptions()
        {
            menuOptions.Clear();
        }
    }
}

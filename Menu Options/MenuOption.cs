﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //represents a highlight-able option
    //shown on screen, that the user move to and use
    public abstract class MenuOption
    {
        protected string name;
        protected Vector2 pos;
        protected Color colour;
        protected OnScreenText textShown;

        public MenuOption(string a_Name, Vector2 a_Pos, Color a_TextColour)
        {
            name = a_Name;
            pos = a_Pos;
            colour = a_TextColour;
            textShown = new OnScreenText(name, a_Pos, a_TextColour, Color.White, Utility.Instance.LabelsFont);
        }

        //each class that inherits will need to implement what
        //happens when the player chooses this option
        public abstract void Use();

        public void Draw()
        {
            textShown.Draw();
        }

        //returns width and height of name in that particular font
        public Vector2 LengthOfString
        {
            get { return textShown.LengthOfString; }
        }

        public Vector2 Position
        {
            get { return pos; }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //draws twice. 
    //first in normal position
    //second in offset position
    public class OnScreenText
    {
        private string text;
        private Vector2 m_Position;
        private Vector2 m_Offset;
        private Color m_TextColour;
        private Color m_ShadowColour;
        private SpriteFont m_FontType;
        private float fadeTimer = 0;

        public OnScreenText(string textToBeDisplayed, Vector2 a_Position, Color a_TextColour, Color a_BackgroundColour, SpriteFont a_FontType)
        {
            text = textToBeDisplayed;
            m_Position = a_Position;
            m_TextColour = a_TextColour;
            m_ShadowColour = a_BackgroundColour;
            m_FontType = a_FontType;
            m_Offset = new Vector2(1, 0);
            CompleteFade = 5.0f;
            Displayed = true;
            StartFade = false;
        }

        public void Update(float dt)
        {
            if (Displayed)
            {
                if (StartFade)
                {
                    fadeTimer += dt;
                }
            }
        }

        public void Draw()
        {
            if (Displayed)
            {
                Utility.Instance.SB.DrawString(m_FontType, text, m_Position + m_Offset, m_ShadowColour * (1 - (fadeTimer / CompleteFade)));
                Utility.Instance.SB.DrawString(m_FontType, text, m_Position, m_TextColour * (1 - (fadeTimer / CompleteFade)));
            }
        }

        public Vector2 LengthOfString
        {
            get { return m_FontType.MeasureString(text); }
        }

        public void ChangeText(string a_NewText)
        {
            text = a_NewText;
        }

        public void ChangeShadowOffset(Vector2 a_Offset)
        {
            m_Offset = a_Offset;
        }

        public void ChangePosition(Vector2 a_NewPosition)
        {
            m_Position = a_NewPosition;
        }

        public float CompleteFade
        {
            get;
            set;
        }

        public bool Displayed
        {
            get;
            set;
        }

        public bool StartFade
        {
            get;
            set;
        }
    }
}

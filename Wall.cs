using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public class Wall
    {
        Vector2 m_Position;
        Vector2 m_Dimensions;
        Vector2 m_Velocity;

        Rectangle m_Rect;
        Color m_Colour;

        public Wall(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
        {
            m_Position = a_Position;
            m_Velocity = Vector2.Zero;
            m_Dimensions = a_Dimensions;

            m_Rect = new Rectangle();
            m_Colour = a_Colour;
        }

        public void Update(float deltaTime)
        {

        }

        public void Draw()
        {
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, GetRect, m_Colour);
        }

        public Rectangle GetRect
        {
            get 
            {
                m_Rect.X = (int)m_Position.X;
                m_Rect.Y = (int)m_Position.Y;
                m_Rect.Width = (int)m_Dimensions.X;
                m_Rect.Height = (int)m_Dimensions.Y;
                return m_Rect;
            }
        }
    }
}

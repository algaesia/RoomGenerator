using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public enum ItemType
    {
        NO_TYPE = -1,
        PERSEUSHELM, //helm of darkness
        TARNHELM, //helm of invisibility
        SVRIVATSA, //shield of vishnu
        PRIWEN, //shield of king arthur
        HARVESTEROFSOULS, //death's scythe
        CHENTU, //horse whip
        HEAVENSWILL, //sword of vietnamese king
        CALEDFWLCH, //similar to excalibur
        ARONDIGHT, //sword of lancelot
        MYSTELTAINN, //sword of Prainn
        GUNGNIR, //spear of odin
        SPEAROFLONGINUS,
        MJOLNIR, //hammr of thor
        VAJRA,
        MEGINGJORD, //belt of thor
        LOTUSTREE, 
        ORICHALCUM, //indestructible metal
        NUM_ITEMS,
    }

    //not abstract so that base class can be created
    //with specialised class to be determined later
    public class Item
    {
        //type will determine what will happen when item is used
        protected ItemType m_Type;
        protected Vector2 m_Position;
        protected Vector2 m_StartPos;
        protected Vector2 m_Dimensions;

        //each item will have different texture representing it on screen
        protected Texture2D m_Texture;
        public bool isAlive = true;

        public Item(Vector2 a_Position, Vector2 a_Dimensions, Texture2D a_Texture, ItemType a_Type)
        {
            m_Texture = a_Texture;
            m_Position = a_Position;
            m_StartPos = m_Position;
            m_Dimensions = a_Dimensions;
            m_Type = a_Type;
        }

        public void Draw()
        {
            if (isAlive)
            {
                Utility.Instance.SB.Draw(m_Texture, GetRect, Color.White);
#if DEBUG
                Utility.Instance.SB.DrawString(Utility.Instance.SF, m_Type.ToString(), m_Position, Color.White);
                Utility.Instance.SB.DrawString(Utility.Instance.SF, m_Type.ToString(), new Vector2(m_Position.X - 1, m_Position.Y), Color.Black);
#endif
            }
        }

        public Rectangle GetRect
        {
            get { return new Rectangle((int)m_Position.X, (int)m_Position.Y, (int)m_Dimensions.X, (int)m_Dimensions.Y); }
        }

        public bool Intersects(Rectangle a_Rect)
        {
            return GetRect.Intersects(a_Rect);
        }

        public ItemType GetItemType
        {
            get { return m_Type; }
        }

        public void MoveOffScreen()
        {
            m_Position = new Vector2(5000, 5000);
        }

        public void ResetPosition()
        {
            m_Position = m_StartPos;
        }

        //class level random type generation
        public static ItemType RandomType()
        {
            return (ItemType)(Utility.Instance.cRand.NextDouble() * (int)(ItemType.NUM_ITEMS));
        }

        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        //every item can be used, 
        //specified by the inherited class
        public virtual void Use()
        {

        }
    }
}

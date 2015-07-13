using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public sealed class Utility
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Texture2D whiteSquare;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        CryptoRandom cryptoRand = new CryptoRandom();
        bool quitGame = false;

        int m_ScreenWidth = 1280;
        int m_ScreenHeight = 720;

        static Utility instance = new Utility();

        public static Utility Instance
        {
            get { return instance; }
        }

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        public SpriteBatch SB
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        public SpriteFont SF
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        public KeyboardState CurrentKeyboardState
        {
            get { return currentKeyboardState; }
            set { currentKeyboardState = value; }
        }

        public KeyboardState PreviousKeyboardState
        {
            get { return previousKeyboardState; }
            set { previousKeyboardState = value; }
        }

        public CryptoRandom cRand
        {
            get { return cryptoRand; }
        }

        public bool QuitGame
        {
            get { return quitGame; }
            set { quitGame = value; }
        }

        public int ScreenWidth
        {
            get { return m_ScreenWidth; }
        }

        public int ScreenHeight
        {
            get { return m_ScreenHeight; }
        }

        public Texture2D WhiteSquare
        {
            get { return whiteSquare; }
            set { whiteSquare = value; }
        }

        public bool IsKeyPressed(Keys a_Key)
        {
            return (currentKeyboardState.IsKeyDown(a_Key) && currentKeyboardState != previousKeyboardState);
        }

        public bool IsKeyHeld(Keys a_Key)
        {
            return (currentKeyboardState.IsKeyDown(a_Key));
        }
    }
}

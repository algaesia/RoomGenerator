using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //shell class for a game state
    //initialise, update, draw
    //anything that inherits 
    public class GameState
    {
        protected ContentManager m_Content;
        protected Color m_BackgroundColour;
        protected bool initialised = false;
        protected string m_Name = "Default";
        protected StateMenuManager menuOptions = new StateMenuManager();

        public GameState(string a_Name)
        {
            m_Name = a_Name;
            m_Content = GameStateManager.Instance.Content;
        }

        public virtual void Initialise()
        {
            if (!initialised)
            {
                menuOptions.ResetHighlighter();
            }

            initialised = true;
        }

        public virtual void Update(float dt)
        {
            menuOptions.Update();
        }

        public virtual void Draw()
        {
            menuOptions.Draw();
        }

        public string Name
        {
            get { return m_Name; }
        }

        public void Reinitialise()
        {
            initialised = false;
        }
    }

    //singleton class, can't be inherited from
    public sealed class GameStateManager
    {
        int currentGameState = 0;
        static Game m_Game;
        static GameStateManager instance = new GameStateManager();
        public static GameStateManager Instance
        {
            get { return instance; }
        }

        List<GameState> m_GameStates = new List<GameState>();

        public ContentManager Content
        {
            get { return m_Game.Content; }
        }

        public GraphicsDevice GfxDevice
        {
            get { return m_Game.GraphicsDevice; }
        }

        //need to set game before doing anything else
        public void SetGame(Game a_Game)
        {
            m_Game = a_Game;
        }

        public void AddGameState(GameState a_GameState)
        {
            m_GameStates.Add(a_GameState);
        }

        public void RemoveGameState(string a_Name)
        {
            int removedState = -1;
            for (int i = 0; i < m_GameStates.Count; ++i)
            {
                if (m_GameStates[i].Name == a_Name)
                {
                    removedState = i;
                    break;
                }
            }

            if (!m_GameStates.Contains(m_GameStates[removedState]))
                return;

            m_GameStates.Remove(m_GameStates[removedState]);
        }

        //game state can be reinitialised if needed. will execute the initialise function again
        //when entering the game state
        public void ChangeGameState(string a_Name, bool reinitialise = false)
        {
            for (int i = 0; i < m_GameStates.Count; ++i)
            {
                if (m_GameStates[i].Name == a_Name)
                {
                    currentGameState = i;
                    if (reinitialise)
                    {
                        m_GameStates[currentGameState].Reinitialise();
                    }
                    return;
                }
            }
        }

        #region Main functions called from Game1
        public void InitialiseCurrentState()
        {
            m_GameStates[currentGameState].Initialise();
        }

        public void UpdateCurrentState(float dt)
        {
            m_GameStates[currentGameState].Update(dt);
        }

        public void DrawCurrentState()
        {
            Utility.Instance.SB.Begin();
            m_GameStates[currentGameState].Draw();
            Utility.Instance.SB.End();
        }
        #endregion
    }
}

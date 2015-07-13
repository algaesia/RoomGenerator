using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public class DungeonExploringState : GameState
    {
        Dungeon mainDungeon;

        public DungeonExploringState()
            : base("Dungeon exploring")
        {
            
        }

        public override void Initialise()
        {
            if (!initialised)
            {
                mainDungeon = new Dungeon(10, 10, 10);
            }

            base.Initialise();
        }

        public override void Update(float dt)
        {
            if (mainDungeon != null)
            {
                mainDungeon.Update(dt);
            }

            if (Utility.Instance.IsKeyPressed(Keys.Escape))
            {
                //reinitialise when entering pause menu
                GameStateManager.Instance.ChangeGameState("Pause menu", true);
            }

            //change to player death state if the player is no longer alive
            if (!Dungeon.GetPlayer.Alive)
            {
                mainDungeon.ResetDungeon(true);
                GameStateManager.Instance.ChangeGameState("Player death");
            }
        }

        public override void Draw()
        {
            if (mainDungeon != null)
            {
                mainDungeon.Draw();
            }
        }
    }
}

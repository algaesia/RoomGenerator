using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public enum PreRoomType
    {
        NO_ROOM = -1,
        ENTRANCE, //0
        EXIT,
        NEXTLEVEL, //2
        EMPTY, //3
        RIDDLE,
        BOSS,
        TRAP,
        TREASURE,
        MONSTER,
        VISTA,
        MAX_TYPES,
    }

    public enum Direction
    {
        NO_DIRECTION = -1,
        NORTH,
        SOUTH,
        EAST,
        WEST,
        MAX_DIRS,
    }

    public class Door
    {
        public Rectangle drawnBox;
        private Rectangle collider;
        public bool collidable;
        public static int width = 50;
        public static int height = 50;

        public Door(int x, int y)
        {
            drawnBox = new Rectangle(x, y, width, height);
            collider = new Rectangle();
            collidable = false;
        }

        public Rectangle Collider
        {
            get
            {
                collider.X = drawnBox.X + width / 2;
                collider.Y = drawnBox.Y + height / 2;
                collider.Width = drawnBox.Width;
                collider.Height = drawnBox.Height;
                return collider;
            }
        }
    }

    public class PreRoom
    {
        protected PreRoomType roomType;
        protected string name;
        protected int[] connections;
        protected int x, y;
        protected Color colour;
        protected bool explored = false;
        protected bool initialised = false;
        protected bool defeated = true;
        protected List<Door> doors = new List<Door>();
        protected List<WallChunk> wallChunks = new List<WallChunk>();
        protected List<Enemy> enemies = new List<Enemy>();
        protected TextBox textToDisplay = new TextBox("EMPTY TEXT BOX");

        protected float fadeTimer = 0;
        protected float startFade = 2.5f;

#if DEBUG
        protected OnScreenText roomNameText;
#endif

        public PreRoom(int a_X, int a_Y)
        {
            x = a_X;
            y = a_Y;

            ChangeRoomType(PreRoomType.NO_ROOM);
            connections = new int[4];
            for (int i = 0; i < 4; ++i)
                connections[i] = -1;

            //north door
            doors.Add(new Door(Utility.Instance.ScreenWidth / 2, 0));

            //south door
            doors.Add(new Door(Utility.Instance.ScreenWidth / 2, Utility.Instance.ScreenHeight - 50));

            //east door
            doors.Add(new Door(Utility.Instance.ScreenWidth - 50, Utility.Instance.ScreenHeight / 2));

            //west door
            doors.Add(new Door(0, Utility.Instance.ScreenHeight / 2));

            wallChunks.Add(new WallChunk(new Vector2(0), Direction.EAST, 40));
            wallChunks.Add(new WallChunk(new Vector2(0), Direction.SOUTH, 40));
            wallChunks.Add(new WallChunk(new Vector2(0, Utility.Instance.ScreenHeight - 32), Direction.EAST, 40));
            wallChunks.Add(new WallChunk(new Vector2(Utility.Instance.ScreenWidth - 32, 0), Direction.SOUTH, 40));

#if DEBUG
            roomNameText = new OnScreenText("EMPTY NAME", Vector2.Zero, Color.DodgerBlue, Color.White, Utility.Instance.HeadingsFont);
#endif
        }

        public int NumConnections
        {
            get 
            {
                int counter = 0;
                for (int i = 0; i < 4; ++i)
                {
                    if (connections[i] != -1)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public bool IsFullyConnected
        {
            //need to account for corners and edges
            get 
            {
                //corners
                if (ID == 0 ||
                    ID == Dungeon.DungeonWidth - 1 ||
                    ID == Dungeon.DungeonWidth * Dungeon.DungeonHeight - 1 ||
                    ID == Dungeon.DungeonWidth * (Dungeon.DungeonHeight - 1))
                {
                    return NumConnections == 2;
                }

                //edges
                if (ID < Dungeon.DungeonWidth ||
                    ID >= Dungeon.DungeonWidth * Dungeon.DungeonHeight - Dungeon.DungeonWidth ||
                    ID % Dungeon.DungeonWidth == Dungeon.DungeonWidth - 1 ||
                    ID % Dungeon.DungeonWidth == 0)
                {
                    return NumConnections == 3;
                }

                return NumConnections == 4; 
            }
        }

        public Direction GetRandomValidDirection()
        {
            int counter = 0;
            int maxCounter = 25;

            while (counter < maxCounter)
            {
                Direction randDir = (Direction)(Utility.Instance.cRand.NextDouble() * (int)Direction.MAX_DIRS);
                switch (randDir)
                {
                    case Direction.NORTH:
                        //need to account for top edge can't link north
                        if (ID < Dungeon.DungeonWidth)
                            break;

                        if (connections[(int)Direction.NORTH] == -1)
                        {
                            return Direction.NORTH;
                        }
                        break;
                    case Direction.SOUTH:
                        //need to account for bottom edge, can't link south
                        if (ID >= Dungeon.DungeonWidth * Dungeon.DungeonHeight - Dungeon.DungeonWidth)
                            break;

                        if (connections[(int)Direction.SOUTH] == -1)
                        {
                            return Direction.SOUTH;
                        }
                        break;
                    case Direction.EAST:
                        //need to account for right edge, can't link east
                        if (ID % Dungeon.DungeonWidth == Dungeon.DungeonWidth - 1)
                            break;

                        if (connections[(int)Direction.EAST] == -1)
                        {
                            return Direction.EAST;
                        }
                        break;
                    case Direction.WEST:
                        //need to account for left edge, can't link west
                        if (ID % Dungeon.DungeonWidth == 0)
                            break;

                        if (connections[(int)Direction.WEST] == -1)
                        {
                            return Direction.WEST;
                        }
                        break;
                }
                counter++;
            }
            return Direction.NO_DIRECTION;
        }

        public Direction GetRandomLinkedRoom()
        {
            int counter = 0;
            int maxCounter = 25;

            while (counter < maxCounter)
            {
                //need to check if there is a connection
                //              AND
                //that there is a room there
                Direction randDir = (Direction)(Utility.Instance.cRand.NextDouble() * (int)Direction.MAX_DIRS);
                switch (randDir)
                {
                    case Direction.NORTH:
                        //need to account for top edge can't link north
                        if (ID < Dungeon.DungeonWidth)
                            break;

                        if (connections[(int)Direction.NORTH] != -1)
                        {
                            return Direction.NORTH;
                        }
                        break;
                    case Direction.SOUTH:
                        //need to account for bottom edge, can't link south
                        if (ID >= Dungeon.DungeonWidth * Dungeon.DungeonHeight - Dungeon.DungeonWidth)
                            break;

                        if (connections[(int)Direction.SOUTH] != -1)
                        {
                            return Direction.SOUTH;
                        }
                        break;
                    case Direction.EAST:
                        //need to account for right edge, can't link east
                        if (ID % Dungeon.DungeonWidth == Dungeon.DungeonWidth - 1)
                            break;

                        if (connections[(int)Direction.EAST] != -1)
                        {
                            return Direction.EAST;
                        }
                        break;
                    case Direction.WEST:
                        //need to account for left edge, can't link west
                        if (ID % Dungeon.DungeonWidth == 0)
                            break;

                        if (connections[(int)Direction.WEST] != -1)
                        {
                            return Direction.WEST;
                        }
                        break;
                }
                counter++;
            }
            return Direction.NO_DIRECTION;
        }

        public int ID
        {
            get { return x + y * Dungeon.DungeonHeight; }
        }

        public void ChangeRoomType(PreRoomType a_Type)
        {
            roomType = a_Type;
        }

        public PreRoomType GetRoomType
        {
            get { return roomType; }
        }

        public string Name
        {
            get { return name; }
        }

        public virtual void Initialise()
        {
            Console.WriteLine("Type of room: " + GetRoomType.ToString() + " is initialised.");
            initialised = true;
        }

        public virtual void Update(float dt)
        {
            //update the timer for text on screen
            fadeTimer += dt;
            if (fadeTimer > startFade)
            {
                fadeTimer = 0;
                textToDisplay.StartFade = true;
            }

            textToDisplay.Update(dt);

            foreach (Enemy n in enemies)
            {
                n.Update(dt);
            }

            Rectangle temp = Dungeon.GetPlayer.GetRect;
            temp.X -= temp.Width / 2;
            temp.Y -= temp.Height / 2;

            //player projectile vs enemy + enemy projectile vs player
            foreach (Projectile projectile in ProjectileManager.Instance.Projectiles)
            {
                foreach (Enemy enemy in enemies)
                {
                    Rectangle tempEnemy = enemy.GetRect;
                    tempEnemy.X -= tempEnemy.Width / 2;
                    tempEnemy.Y -= tempEnemy.Height / 2;

                    if (projectile.GetDrawnRect.Intersects(tempEnemy) &&
                        projectile.IsPlayerProjectile)
                    {
                        projectile.IsAlive = false;
#if DEBUG
                        projectile.SetColour(Color.Yellow);
#endif
                        enemy.Health -= projectile.Damage;
                    }
                }

                if (!projectile.IsPlayerProjectile &&
                    projectile.GetDrawnRect.Intersects(temp))
                {
                    projectile.IsAlive = false;
#if DEBUG
                    projectile.SetColour(Color.Red);
#endif
                    Dungeon.GetPlayer.Health -= projectile.Damage;
                }
            }

            //enemy vs. player
            foreach (Enemy enemy in enemies)
            {
                Rectangle tempEnemyRect = enemy.GetRect;
                tempEnemyRect.X -= tempEnemyRect.Width / 2;
                tempEnemyRect.Y -= tempEnemyRect.Height / 2;

                if (tempEnemyRect.Intersects(temp))
                {
                    Dungeon.GetPlayer.Health -= enemy.AttackValue;
                    enemy.Health -= Dungeon.GetPlayer.AttackValue;
                    Dungeon.GetPlayer.PushBack(dt);
                    enemy.PushBack(dt);
                }
            }

            //doesn't work...
            //enemy vs. enemy
            foreach (Enemy n in enemies)
            {
                foreach (Enemy j in enemies)
                {
                    if (n == j)
                    {
                        continue;
                    }

                    Rectangle tempNRect = n.GetRect;
                    tempNRect.X -= tempNRect.Width / 2;
                    tempNRect.Y -= tempNRect.Height / 2;

                    Rectangle tempJRect = j.GetRect;
                    tempJRect.X -= tempJRect.Width / 2;
                    tempJRect.Y -= tempJRect.Height / 2;

                    //middle of texture moved to top left vs middle of texture moved to top left
                    if (tempNRect.Intersects(tempJRect))
                    {
                        n.PushBack(dt);
                        j.PushBack(dt);
                    }
                }
            }

            #region Collision checks
            #region Walls
            foreach (WallChunk n in wallChunks)
            {
                n.Update(dt);
                if (n.Intersects(temp))
                {
                    Dungeon.GetPlayer.PushBack(dt);
                }

                foreach (Enemy j in enemies)
                {
                    if (n.Intersects(j.GetRect))
                    {
                        j.PushBack(dt);
                    }
                }
            }
            #endregion

            #endregion
        }

        public virtual void Draw(Rectangle size)
        {
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, size, colour);

#if DEBUG
            Vector2 textDimensions = Utility.Instance.HeadingsFont.MeasureString(name);
            roomNameText.ChangeText(Name);
            roomNameText.ChangePosition(new Vector2(Utility.Instance.ScreenWidth * 0.5f - textDimensions.X * 0.5f, Utility.Instance.ScreenHeight * 0.5f - textDimensions.Y * 0.5f));
            roomNameText.Draw();
#endif
            foreach (Enemy n in enemies)
            {
                n.Draw();
            }

            foreach (WallChunk n in wallChunks)
            {
                n.Draw();
            }
        }

        //doors are drawn in the dungeon,
        //so text box must be drawn in dungeon
        //if drawn here, doors drawn over text box
        public void DrawTextBox()
        {
            textToDisplay.Draw();
        }

        public void StartTextFade()
        {
            textToDisplay.StartFade = true;
        }

        public int GetConnection(Direction a_Dir)
        {
            return connections[(int)a_Dir];
        }

        public void SetConnection(Direction a_Dir, int a_Value)
        {
            connections[(int)a_Dir] = a_Value;
        }

        public Door GetDoor(Direction a_Dir)
        {
            return doors[(int)a_Dir];
        }

        public bool CheckDoorCollidable(Direction a_Dir)
        {
            return doors[(int)a_Dir].collidable;
        }

        public void MakeDoorCollidable(Direction a_Dir)
        {
            doors[(int)a_Dir].collidable = true;
        }

        public Rectangle GetDoorDrawn(Direction a_Dir)
        {
            return doors[(int)a_Dir].drawnBox;
        }

        public Rectangle GetDoorCollider(Direction a_Dir)
        {
            return doors[(int)a_Dir].Collider;
        }

        public bool Explored
        {
            get { return explored; }
            set { explored = value; }
        }

        public bool Defeated
        {
            get { return defeated; }
            set { defeated = value; }
        }

        public int X
        {
            get { return x; }
        }

        public int Y
        {
            get { return y; }
        }

        public int[] GetConnections
        {
            get { return connections; }
        }
    }
}

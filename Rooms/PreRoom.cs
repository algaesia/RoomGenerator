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
        public Rectangle collisionBox;
        public bool collidable;
        public static int width = 50;
        public static int height = 50;

        public Door(int x, int y)
        {
            collisionBox = new Rectangle(x, y, width, height);
            collidable = false;
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
            foreach (WallChunk n in wallChunks)
            {
                n.Update(dt);
                if (n.Intersects(Dungeon.GetPlayer.GetRect))
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
        }

        public virtual void Draw(Rectangle size)
        {
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, size, colour);

#if DEBUG
            Utility.Instance.SB.DrawString(Utility.Instance.SF, Name, new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.5f), Color.White);
            Utility.Instance.SB.DrawString(Utility.Instance.SF, Name, new Vector2((Utility.Instance.ScreenWidth - 1) * 0.5f, Utility.Instance.ScreenHeight * 0.5f), Color.Black);
#endif

            foreach (WallChunk n in wallChunks)
            {
                n.Draw();
            }
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

        public Rectangle GetDoorCollider(Direction a_Dir)
        {
            return doors[(int)a_Dir].collisionBox;
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

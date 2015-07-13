using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public class WallChunk
    {
        Vector2 m_Position;
        Vector2 m_WallDimensions = new Vector2(32);
        Direction m_Direction;

        List<Wall> walls = new List<Wall>();

        public WallChunk(Vector2 a_Position, Direction a_Direction, int wallLength)
        {
            m_Position = a_Position;
            m_Direction = a_Direction;
            for (int i = 0; i < wallLength; ++i)
            {
                switch (a_Direction)
                {
                    case Direction.NORTH:
                        AddWall(new Wall(m_Position + new Vector2(0, -i * m_WallDimensions.Y), m_WallDimensions, Color.Red));
                        break;
                    case Direction.SOUTH:
                        AddWall(new Wall(m_Position + new Vector2(0, i * m_WallDimensions.Y), m_WallDimensions, Color.Red));
                        break;
                    case Direction.EAST:
                        AddWall(new Wall(m_Position + new Vector2(i * m_WallDimensions.X, 0), m_WallDimensions, Color.Red));
                        break;
                    case Direction.WEST:
                        AddWall(new Wall(m_Position + new Vector2(-i * m_WallDimensions.X, 0), m_WallDimensions, Color.Red));
                        break;
                }
            }
        }

        public void Update(float deltaTime)
        {
            foreach (Wall n in walls)
            {
                n.Update(deltaTime);
            }
        }

        public void Draw()
        {
            foreach (Wall n in walls)
            {
                n.Draw();
            }
        }

        void AddWall(Wall a_NewWall)
        {
            walls.Add(a_NewWall);
        }

        public bool Intersects(Rectangle a_Rect)
        {
            foreach (Wall n in walls)
            {
                if (n.GetRect.Intersects(a_Rect))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

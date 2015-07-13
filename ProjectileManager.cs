using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //simple class that moves in a particular direction
    //when it is alive. when it stops moving it is dead
    public class Projectile
    {
        Vector2 m_Position;
        Vector2 m_StartPosition;
        Vector2 m_Velocity;
        Vector2 m_Dimensions;

        bool m_IsAlive;
        bool m_IsPlayerProjectile;

        float m_MovementSpeed;
        float m_VelocityDecayRate;
        float m_MaxVelocity;
        float m_MaxDistFromStart;

        Texture2D m_Texture;
        Color m_Colour;

        public Projectile(Vector2 a_Position, Vector2 a_Velocity, Vector2 a_Dimensions, Color a_Colour, int a_Damage, bool playerProjectile)
        {
            Damage = a_Damage;
            m_IsPlayerProjectile = playerProjectile;
            m_MaxDistFromStart = 500;
            m_IsAlive = true;
            m_Position = a_Position;
            m_StartPosition = m_Position;
            m_Dimensions = a_Dimensions;
            m_Colour = a_Colour;
            m_Texture = Utility.Instance.WhiteSquare;
            m_MovementSpeed = 175;
            m_VelocityDecayRate = (float)(0.9f + Utility.Instance.cRand.NextDouble() * (0.99f - 0.9f));
            m_MaxVelocity = 35;
            m_Velocity = a_Velocity;
        }

        public void Update(float dt)
        {
            if (m_IsAlive)
            {
                if (m_Velocity.Length() < 0.01f)
                {
                    IsAlive = false;
                }
                
                if (m_Velocity.Length() != 0)
                {
                    float mag = m_Velocity.Length();
                    m_Velocity = Vector2.Normalize(m_Velocity) * Math.Min(mag, m_MaxVelocity) * m_VelocityDecayRate;
                }

                m_Position += m_Velocity * m_MovementSpeed * dt;

                CheckDistFromStart();
            }
        }

        public void Draw()
        {
            if (m_IsAlive)
            {
                Utility.Instance.SB.Draw(m_Texture, GetRect, m_Colour);
            }
        }

        public void CheckDistFromStart()
        {
            if (Vector2.Distance(m_Position, m_StartPosition) > m_MaxDistFromStart)
            {
                m_IsAlive = false;
            }
        }

        public Rectangle GetRect
        {
            get { return new Rectangle((int)m_Position.X, (int)m_Position.Y, (int)m_Dimensions.X, (int)m_Dimensions.Y); }
        }

        public int Damage
        {
            get;
            set;
        }

        public void ResetPosition()
        {
            m_Position = m_StartPosition;
        }

        //will also change start position
        public void SetPosition(Vector2 a_NewPosition)
        {
            m_Position = a_NewPosition;
            m_StartPosition = m_Position;
        }

        public void SetVelocity(Vector2 a_NewVelocity)
        {
            m_Velocity = a_NewVelocity;
        }

        public void SetColour(Color a_Colour)
        {
            m_Colour = a_Colour;
        }

        public void SetPlayerProjectile(bool a_PlayerProjectile)
        {
            m_IsPlayerProjectile = a_PlayerProjectile;
        }

        public bool IsAlive
        {
            get { return m_IsAlive; }
            set { m_IsAlive = value; }
        }

        public bool IsPlayerProjectile
        {
            get { return m_IsPlayerProjectile; }
        }
    }

    //singleton class that exists everywhere
    //handles all projectiles in the game
    public sealed class ProjectileManager
    {
        static readonly ProjectileManager instance = new ProjectileManager();

        public static ProjectileManager Instance
        {
            get { return instance; }
        }

        List<Projectile> projectiles = new List<Projectile>();

        ProjectileManager()
        {

        }

        public void Shoot(Vector2 a_Position, Vector2 a_Velocity, Vector2 a_Dimensions, Color a_Colour, int a_Damage, bool playerProjectile)
        {
            //if t is null, then there are no projectiles free, so create one and add it to the list
            Projectile t = GetUnusedProjectile();
            if (t == null)
            {
                projectiles.Add(new Projectile(a_Position, a_Velocity, a_Dimensions, a_Colour, a_Damage, playerProjectile));
            }
            //otherwise get a free one, copy over the new values and set it at the right position
            else
            {
                t.Damage = a_Damage;
                t.SetPlayerProjectile(playerProjectile);
                t.SetPosition(a_Position);
                t.SetVelocity(a_Velocity);
                t.SetColour(a_Colour);
                t.IsAlive = true;
            }
        }

        public void ClearBullets()
        {
            projectiles.Clear();
        }

        public void Update(float dt)
        {
            foreach (Projectile n in projectiles)
            {
                n.Update(dt);
            }

            //dead bullets are moved offscreen
            MoveDeadBullets();
        }

        public void Draw()
        {
            foreach (Projectile n in projectiles)
            {
                n.Draw();
            }
        }

        public List<Projectile> Projectiles
        {
            get { return projectiles; }
        }

        //returns null if there are no dead projectiles
        Projectile GetUnusedProjectile()
        {
            Projectile t = null;

            foreach (Projectile n in projectiles)
            {
                if (!n.IsAlive)
                {
                    t = n;
                    break;
                }
            }

            return t;
        }

        void MoveDeadBullets()
        {
            foreach (Projectile n in projectiles)
            {
                if (!n.IsAlive)
                {
                    n.SetPosition(new Vector2(-1000));
                }
            }
        }
    }
}

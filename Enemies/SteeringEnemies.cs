using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //chase
    public class Pursuer : Enemy
    {
        Pursue chaseBehaviour;
        int tooClose = 80;

        public Pursuer(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
            : base(a_Position, a_Dimensions, a_Colour)
        {
            chaseBehaviour = new Pursue(this);
        }

        public override void Update(float dt)
        {
            if (m_IsAlive)
            {
                int distToPlayer = (int)Math.Floor(Vector2.Distance(m_Position, Dungeon.GetPlayer.Position));

                //dist to player greater than 10, then chase
                if (distToPlayer > tooClose)
                {
                    m_Velocity += chaseBehaviour.GetForce();
                }
            }

            base.Update(dt);
        }
    }

    //sit and shoot
    public class Shooter : Enemy
    {
        float m_MinBiasToPlayer = 0.1f;
        float m_MaxBiasToPlayer = 1;
        float m_BiasToPlayer;

        public Shooter(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
            : base(a_Position, a_Dimensions, a_Colour)
        {
            m_ShootReady = (float)(m_MinShootReady + Utility.Instance.cRand.NextDouble() * (m_MaxShootReady - m_MinShootReady));
            m_BiasToPlayer = m_MinBiasToPlayer + (float)(Utility.Instance.cRand.NextDouble() * (m_MaxBiasToPlayer - m_MinBiasToPlayer));
        }

        public override void Update(float dt)
        {
            if (m_IsAlive)
            {
                m_ShootTimer += dt;

                Shoot();

                CheckBoundaries();

                if (m_CurrentHealth <= 0)
                {
                    MoveOffScreen();
                    m_IsAlive = false;
                }
            }
        }

        public override void Shoot()
        {
            if (Dungeon.GetPlayer != null)
            {
                if (m_ShootTimer > m_ShootReady)
                {
                    m_ShootTimer = 0;
                    Vector2 dirToPlayer = Dungeon.GetPlayer.Position - m_Position;
                    dirToPlayer.Normalize();
                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);
                }
            }

            base.Shoot();
        }
    }

    //run towards different section of room
    public class Avoider : Enemy
    {
        Pursue pursueBehaviour;
        int tooClose = 250;
        int chooseNewTarget = 100;

        Vector2[] quadrants = new Vector2[4];

        public Avoider(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
            : base(a_Position, a_Dimensions, a_Colour)            
        {
            pursueBehaviour = new Pursue(this);
            GenerateQuadrants();
        }

        public override void Update(float dt)
        {
            if (m_IsAlive)
            {
                int distToPlayer = (int)Math.Floor(Vector2.Distance(m_Position, Dungeon.GetPlayer.Position));

                if (distToPlayer < chooseNewTarget)
                {
                    //pursueBehaviour.SetTarget()
                }

                //player is too close, so run
                if (distToPlayer < tooClose)
                {
                    m_Velocity += pursueBehaviour.GetForce();
                }
            }

            base.Update(dt);
        }

        public void SetTarget(Vector2 a_Position)
        {
            pursueBehaviour.SetTarget(a_Position);
        }

        public void ResetTarget()
        {
            pursueBehaviour.ResetTarget();
        }

        public void GenerateQuadrants()
        {
            //top left
            //min 0 max screen width * 0.5
            quadrants[0].X = (float)(Utility.Instance.cRand.NextDouble() * Utility.Instance.ScreenWidth * 0.5);

            //min 0 max screen height * 0.5
            quadrants[0].Y = (float)(Utility.Instance.cRand.NextDouble() * Utility.Instance.ScreenHeight * 0.5);

            //top right
            //min screen width * 0.5 max screen width
            quadrants[1].X = (float)(Utility.Instance.ScreenWidth * 0.5 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth - Utility.Instance.ScreenWidth * 0.5));

            //min 0 max screen height * 0.5
            quadrants[1].Y = (float)(Utility.Instance.cRand.NextDouble() * Utility.Instance.ScreenHeight * 0.5);

            //bottom left
            //min 0 max screen width * 0.5
            quadrants[2].X = (float)(Utility.Instance.cRand.NextDouble() * Utility.Instance.ScreenWidth * 0.5);

            //min screen height * 0.5 max screen height
            quadrants[2].Y = (float)(Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight - (Utility.Instance.ScreenHeight * 0.5)));

            //bottom right
            //min screen width * 0.5 max screen width
            quadrants[3].X = (float)(Utility.Instance.ScreenWidth * 0.5 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth - Utility.Instance.ScreenWidth * 0.5));

            //min screen height * 0.5 max screen height
            quadrants[3].Y = (float)(Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight - (Utility.Instance.ScreenHeight * 0.5)));
        }
    }

    //chase and shoot
    public class PursuingShooter : Pursuer
    {
        float m_MinBiasToPlayer = 0.1f;
        float m_MaxBiasToPlayer = 1;
        float m_BiasToPlayer;

        public PursuingShooter(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
            : base(a_Position, a_Dimensions, a_Colour)
        {
            m_BiasToPlayer = m_MinBiasToPlayer + (float)(Utility.Instance.cRand.NextDouble() * (m_MaxBiasToPlayer - m_MinBiasToPlayer));
        }

        public override void Update(float dt)
        {
            if (m_IsAlive)
            {
                m_ShootTimer += dt;

                Shoot();
            }

            base.Update(dt);
        }

        public override void Shoot()
        {
            if (Dungeon.GetPlayer != null)
            {
                if (m_ShootTimer > m_ShootReady)
                {
                    m_ShootTimer = 0;
                    Vector2 dirToPlayer = Dungeon.GetPlayer.Position - m_Position;
                    dirToPlayer.Normalize();
                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);
                }
            }

            base.Shoot();
        }
    }

    //run away and shoot
    public class EvadingShooter : Avoider
    {
        float m_MinBiasToPlayer = 0.1f;
        float m_MaxBiasToPlayer = 1;
        float m_BiasToPlayer;

        public EvadingShooter(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
            : base(a_Position, a_Dimensions, a_Colour)
        {
            m_BiasToPlayer = m_MinBiasToPlayer + (float)(Utility.Instance.cRand.NextDouble() * (m_MaxBiasToPlayer - m_MinBiasToPlayer));
        }

        public override void Update(float dt)
        {
            if (m_IsAlive)
            {
                m_ShootTimer += dt;

                Shoot();
            }

            base.Update(dt);
        }

        public override void Shoot()
        {
            if (Dungeon.GetPlayer != null)
            {
                if (m_ShootTimer > m_ShootReady)
                {
                    m_ShootTimer = 0;
                    Vector2 dirToPlayer = Dungeon.GetPlayer.Position - m_Position;
                    dirToPlayer.Normalize();
                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * m_BiasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);
                }
            }

            base.Shoot();
        }
    }
}

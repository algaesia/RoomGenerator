using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        int tooClose = 40;
        Texture2D tooCloseRadius;

        public Pursuer(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
            : base(a_Position, a_Dimensions, a_Colour)
        {
            chaseBehaviour = new Pursue(this);
            tooCloseRadius = Utility.Instance.CreateCircle(tooClose, Color.Black);
        }

        public override void Update(float dt)
        {
            if (m_IsAlive)
            {
                chaseBehaviour.Target = Dungeon.GetPlayer.Position;
                int distToPlayer = (int)Math.Floor(Vector2.Distance(m_Position, Dungeon.GetPlayer.Position));

                //dist to player greater than 10, then chase
                if (distToPlayer > tooClose)
                {
                    m_Velocity += chaseBehaviour.GetForce();
                }
            }

            base.Update(dt);
        }

        public override void Draw()
        {
            base.Draw();

#if DEBUG
            Utility.Instance.SB.Draw(tooCloseRadius, m_Position - new Vector2(tooClose - GetRect.Width * 0.5f, tooClose - GetRect.Height * 0.5f), Color.Black);
#endif
        }
    }

    //sit and shoot
    public class Shooter : Enemy
    {
        float m_MinBiasToPlayer = 0.1f;
        float m_MaxBiasToPlayer = 1;
        float m_BiasToPlayer;

        int minBulletsToShoot = 3;
        int maxBulletsToShoot = 7;

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

        public override void Draw()
        {
            base.Draw();

#if DEBUG
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)m_Position.X, (int)m_Position.Y - 1, (int)((m_ShootTimer / m_ShootReady) * m_Dimensions.X), 10), Color.PaleVioletRed);
#endif
        }

        private void Shoot(Vector2 direction, int numOfBullets)
        {
            for (int i = 0; i < numOfBullets; ++i)
            {
                ProjectileManager.Instance.Shoot(m_Position + GetForward * m_Dimensions.X + new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1), (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)),
                                                 GetForward + new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1), (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) + direction,
                                                 new Vector2(10),
                                                 Color.DarkBlue,
                                                 m_AttackValue,
                                                 false);
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
                    if (dirToPlayer.Length() > 0)
                    {
                        dirToPlayer.Normalize();
                    }
                    else
                    {
                        dirToPlayer = new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1), (float)(Utility.Instance.cRand.NextDouble() * 2 - 1));
                    }

                    int numBulletsToShoot = minBulletsToShoot + (int)(Utility.Instance.cRand.NextDouble() * (maxBulletsToShoot - minBulletsToShoot));

                    Shoot(dirToPlayer, numBulletsToShoot);
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
        int chooseNewTarget = 50;
        bool hasTarget = false;

        Texture2D tooCloseRadius;
        Texture2D chooseNewTargetRadius;

        Vector2[] quadrants = new Vector2[4];

        public Avoider(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
            : base(a_Position, a_Dimensions, a_Colour)
        {
            pursueBehaviour = new Pursue(this);
            GenerateQuadrants();

            tooCloseRadius = Utility.Instance.CreateCircle(tooClose, Color.White);
            chooseNewTargetRadius = Utility.Instance.CreateCircle(chooseNewTarget, Color.Black);
            int randQuad = (int)(Utility.Instance.cRand.NextDouble() * 4);
            pursueBehaviour.Target = quadrants[randQuad];
        }

        public override void Update(float dt)
        {
            if (m_IsAlive)
            {
                int distToPlayer = (int)Math.Floor(Vector2.Distance(m_Position, Dungeon.GetPlayer.Position));
                int distToTarget = (int)Math.Floor(Vector2.Distance(m_Position, pursueBehaviour.Target));

                if ((distToPlayer < tooClose || distToTarget < chooseNewTarget) && !hasTarget)
                {
                    int randQuad = (int)(Utility.Instance.cRand.NextDouble() * 4);
                    pursueBehaviour.Target = quadrants[randQuad];
                    hasTarget = true;
                }

                if (distToTarget < chooseNewTarget || m_Velocity.Length() < 0.1f)
                {
                    GenerateQuadrants();
                    hasTarget = false;
                }

                m_Velocity += pursueBehaviour.GetForce();
            }

            base.Update(dt);
        }

        public override void Draw()
        {
            base.Draw();

#if DEBUG
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, pursueBehaviour.Target, Color.White);
            Utility.Instance.SB.Draw(tooCloseRadius, m_Position - new Vector2(tooClose - GetRect.Width * 0.5f, tooClose - GetRect.Height * 0.5f), Color.White);
            Utility.Instance.SB.Draw(chooseNewTargetRadius, m_Position - new Vector2(chooseNewTarget - GetRect.Width * 0.5f, chooseNewTarget - GetRect.Height * 0.5f), Color.Black);
#endif
        }

        public void SetTarget(Vector2 a_Position)
        {
            pursueBehaviour.Target = a_Position;
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

        int minBulletsToShoot = 3;
        int maxBulletsToShoot = 7;

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

        public override void Draw()
        {
            base.Draw();

#if DEBUG
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)m_Position.X, (int)m_Position.Y - 1, (int)((m_ShootTimer / m_ShootReady) * m_Dimensions.X), 10), Color.PaleVioletRed);
#endif
        }

        private void Shoot(Vector2 direction, int numOfBullets)
        {
            for (int i = 0; i < numOfBullets; ++i)
            {
                ProjectileManager.Instance.Shoot(m_Position + GetForward * m_Dimensions.X + new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1), (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)),
                                                         GetForward +
                                                         new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                     (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                         direction,
                                                         new Vector2(10),
                                                         Color.DarkBlue,
                                                         m_AttackValue,
                                                         false);
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
                    if (dirToPlayer.Length() > 0)
                    {
                        dirToPlayer.Normalize();
                    }
                    else
                    {
                        dirToPlayer = new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1), (float)(Utility.Instance.cRand.NextDouble() * 2 - 1));
                    }

                    int numBulletsToShoot = minBulletsToShoot + (int)(Utility.Instance.cRand.NextDouble() * (maxBulletsToShoot - minBulletsToShoot));

                    Shoot(dirToPlayer, numBulletsToShoot);
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

        int minBulletsToShoot = 3;
        int maxBulletsToShoot = 7;

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

        public override void Draw()
        {
            base.Draw();

#if DEBUG
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)m_Position.X, (int)m_Position.Y - 1, (int)((m_ShootTimer / m_ShootReady) * m_Dimensions.X), 10), Color.PaleVioletRed);
#endif
        }

        private void Shoot(Vector2 direction, int numOfBullets)
        {
            for (int i = 0; i < numOfBullets; ++i)
            {
                ProjectileManager.Instance.Shoot(m_Position + GetForward * m_Dimensions.X + new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1), (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)),
                                                         GetForward +
                                                         new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                     (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                         direction,
                                                         new Vector2(10),
                                                         Color.DarkBlue,
                                                         m_AttackValue,
                                                         false);
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
                    if (dirToPlayer.Length() > 0)
                    {
                        dirToPlayer.Normalize();
                    }
                    else
                    {
                        dirToPlayer = new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1), (float)(Utility.Instance.cRand.NextDouble() * 2 - 1));
                    }

                    int numBulletsToShoot = minBulletsToShoot + (int)(Utility.Instance.cRand.NextDouble() * (maxBulletsToShoot - minBulletsToShoot));

                    Shoot(dirToPlayer * m_BiasToPlayer, numBulletsToShoot);
                }
            }

            base.Shoot();
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //TODO: make boss enemy move
    //base boss enemy - behaviours added on top in inherited classes
    //all bosses shoot
    //increased movement speed
    //increased health
    //decreased velocity decay
    //increased max velocity
    //increased attack speed
    //origin = middle of texture
    public class BossEnemy : Enemy
    {
        public BossEnemy(Vector2 a_Position, Color a_Colour)
            : base(a_Position, new Vector2(35), a_Colour)
        {
            m_MaxMovementSpeed = 27;

            //attack speed randomised for each boss spawned
            m_MinShootReady = 0.3f;
            m_MaxShootReady = 2;
            m_ShootReady = (float)(m_MinShootReady + Utility.Instance.cRand.NextDouble() * (m_MaxShootReady - m_MinShootReady));
            m_MaxAttackSpeed = 1 / m_MinShootReady;

            m_MaxHealth = 30;
            m_CurrentMaxHealth = 14;
            m_CurrentHealth = m_CurrentMaxHealth;

            int minMovementSpeed = 30;
            int maxMovementSpeed = 50;
            m_MovementSpeed = (float)(minMovementSpeed + Utility.Instance.cRand.NextDouble() * (maxMovementSpeed - minMovementSpeed));

            float minVeloDecay = 0.8f;
            float maxVeloDecay = 0.99f;
            m_VelocityDecayRate = (float)(minVeloDecay + Utility.Instance.cRand.NextDouble() * (maxVeloDecay - minVeloDecay));

            float minVelo = 35;
            float maxVelo = 55;
            m_MaxVelocity = (float)(minVelo + Utility.Instance.cRand.NextDouble() * (maxVelo - minVelo));
        }

        public override void Update(float dt)
        {
            //every boss shoots
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
            Utility.Instance.SB.DrawString(Utility.Instance.LabelsFont, m_CurrentHealth.ToString(), m_Position, Color.White);
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)(m_Position.X), (int)(m_Position.Y), 5, 5), Color.White);
            //Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)(m_Position.X + m_Dimensions.X * 0.5f), (int)(m_Position.Y + m_Dimensions.Y * 0.5f), 5, 5), Color.White);
            //Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)(m_Position.X - m_Dimensions.X * 0.5f), (int)(m_Position.Y - m_Dimensions.Y * 0.5f), 5, 5), Color.White);
#endif
        }

        public override void Shoot()
        {
            //only shoot if player exists
            if (Dungeon.GetPlayer != null)
            {
                if (m_ShootTimer > m_ShootReady)
                {
                    m_ShootTimer = 0;
                    float minBias = 0.1f;
                    float maxBias = 1;
                    float biasToPlayer = minBias + (float)(Utility.Instance.cRand.NextDouble() * (maxBias - minBias));

                    //shoot at the player, with a bias
                    Vector2 dirToPlayer = Dungeon.GetPlayer.Position - m_Position;
                    dirToPlayer.Normalize();

                    //shoot 4 bullets
                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * biasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * biasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * biasToPlayer,
                                                     new Vector2(10),
                                                     Color.DarkBlue,
                                                     m_AttackValue,
                                                     false);

                    ProjectileManager.Instance.Shoot(m_Position + new Vector2(m_Dimensions.X * 0.5f, m_Dimensions.Y * 0.5f) + new Vector2((float)(Utility.Instance.cRand.NextDouble()), (float)(Utility.Instance.cRand.NextDouble())),
                                                     m_Velocity +
                                                     new Vector2((float)(Utility.Instance.cRand.NextDouble() * 2 - 1),
                                                                 (float)(Utility.Instance.cRand.NextDouble() * 2 - 1)) +
                                                     dirToPlayer * biasToPlayer,
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

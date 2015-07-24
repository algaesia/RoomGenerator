using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //simple template for the enemy class
    //only adds velocity to position
    public class Enemy
    {
        protected Vector2 m_Position;
        protected Vector2 m_Dimensions;
        protected Vector2 m_Velocity;
        protected Vector2 m_StartPos;

        protected float m_MovementSpeed;
        protected float m_MaxMovementSpeed = 22;

        protected float m_VelocityDecayRate;
        protected float m_MaxVelocity;

        protected float m_ShootTimer = 0;
        protected float m_MinShootReady = 0.5f;
        protected float m_MaxShootReady = 2.5f;
        protected float m_MaxAttackSpeed = 0.5f;
        protected float m_ShootReady = 1;

        protected Texture2D m_Texture;
        protected Color m_Colour;

        protected int m_MaxHealth = 25;
        protected int m_CurrentHealth;
        protected int m_CurrentMaxHealth;

        protected int m_AttackValue = 1;
        protected int m_MaxAttackValue = 10;

        protected bool m_IsAlive = true;

        protected float m_BulletSize = 14;
        protected float m_MaxBulletSize = 32;

        protected float m_Rotation = 0;

        protected Rectangle m_DrawnRect;

#if DEBUG
        OnScreenText velocityText;
        OnScreenText rotationText;
#endif

        public Enemy(Vector2 a_Position, Vector2 a_Dimensions, Color a_Colour)
        {
            m_CurrentMaxHealth = 3;
            m_CurrentHealth = m_CurrentMaxHealth;

            int textureChosen = (int)(Utility.Instance.cRand.NextDouble() * 8) + 1;
            string textureName = "enemy" + textureChosen.ToString();
            m_Texture = Utility.Instance.LoadTexture(textureName);

            m_Dimensions = a_Dimensions;
            m_Colour = Color.White;
            m_Position = a_Position;
            m_StartPos = m_Position;
            m_Velocity = Vector2.Zero;

            m_DrawnRect = new Rectangle((int)m_Position.X, (int)m_Position.Y, (int)m_Dimensions.X, (int)m_Dimensions.Y);

            int minMovementSpeed = 30;
            int maxMovementSpeed = 60;
            m_MovementSpeed = (float)(minMovementSpeed + Utility.Instance.cRand.NextDouble() * (maxMovementSpeed - minMovementSpeed));

            float minVeloDecay = 0.7f;
            float maxVeloDecay = 0.9f;
            m_VelocityDecayRate = (float)(minVeloDecay + Utility.Instance.cRand.NextDouble() * (maxVeloDecay - minVeloDecay));

            float minVelo = 35;
            float maxVelo = 50;
            m_MaxVelocity = (float)(minVelo + Utility.Instance.cRand.NextDouble() * (maxVelo - minVelo));

#if DEBUG
            velocityText = new OnScreenText("EMPTY TEXT", m_Position, Color.Black, Color.White, Utility.Instance.LabelsFont);
            rotationText = new OnScreenText("EMPTY TEXT", m_Position, Color.Black, Color.White, Utility.Instance.LabelsFont);
#endif
        }

        //zeroes out velocity if it gets too small
        //restricts velocity if it gets too high
        //adds velocity to position
        //check boundaries
        //check health
        public virtual void Update(float dt)
        {
            if (m_IsAlive)
            {
                if (m_Velocity.Length() < 0.01f)
                {
                    m_Velocity = Vector2.Zero;
                }

                if (m_Velocity.Length() != 0)
                {
                    float mag = m_Velocity.Length();
                    m_Velocity += GetForward;
                    m_Velocity = Vector2.Normalize(m_Velocity) * Math.Min(mag, m_MaxVelocity) * m_VelocityDecayRate;
                }

                m_Position += m_Velocity * m_MovementSpeed * dt;

                //when an enemy dies, it gets moved offscreen, ready to be reused
                if (m_CurrentHealth <= 0)
                {
                    MoveOffScreen();
                    m_IsAlive = false;
                }

                if (CheckBoundaries())
                {
                    PushBack(dt);
                }
            }
        }

        public virtual void Draw()
        {
            if (m_IsAlive)
            {
                Utility.Instance.SB.Draw(m_Texture, GetRect, null, m_Colour, m_Rotation, new Vector2(m_Texture.Width * 0.5f, m_Texture.Height * 0.5f), SpriteEffects.None, 0);
#if DEBUG
                velocityText.ChangePosition(m_Position);
                velocityText.ChangeText(m_Velocity.ToString());
                velocityText.Draw();

                rotationText.ChangePosition(m_Position + new Vector2(0, 35));
                rotationText.ChangeText(m_Rotation.ToString());
                rotationText.Draw();

                Utility.Instance.DrawLine(m_Position, m_Position + m_Velocity * m_MovementSpeed, Color.Yellow, 1);
#endif
            }
        }

        //virtual function for shooting
        //can be changed to suit enemies that shoot
        public virtual void Shoot()
        {

        }

        public bool CheckBoundaries()
        {
            if (m_Position.X - m_Dimensions.X * 0.5f < Door.width ||
                m_Position.Y - m_Dimensions.Y * 0.5f < Door.height ||
                m_Position.X + m_Dimensions.X * 0.5f > Utility.Instance.ScreenWidth - Door.width||
                m_Position.Y + m_Dimensions.Y * 0.5f > Utility.Instance.ScreenHeight - Door.height)
            {
                return true;
            }
            return false;
        }

        public void PushBack(float dt)
        {
            int pushbackFactor = 50;
            m_Velocity += -GetForward * pushbackFactor * m_MovementSpeed * dt;
        }

        public Vector2 GetForward
        {
            get
            {
                float rotS = (float)(Math.Sin(m_Rotation));
                float rotC = (float)(Math.Cos(m_Rotation));

                float dirX = 0;
                float dirY = -1;

                float forwardX = dirX * rotC - dirY * rotS;
                float forwardY = dirX * rotS + dirY * rotC;

                return new Vector2(forwardX, forwardY);
            }
        }

        public void FaceTarget(Vector2 a_Target)
        {
            Vector2 newFacing = a_Target - m_Position;
            newFacing.Normalize();

            m_Rotation = (float)(Math.Atan2(newFacing.X, -newFacing.Y));
        }

        public int Health
        {
            get { return m_CurrentHealth; }
            set 
            {
                m_CurrentHealth = value;
                if (m_CurrentHealth <= 0)
                {
                    m_CurrentHealth = 0;
                }

                if (m_CurrentHealth > m_CurrentMaxHealth)
                {
                    m_CurrentHealth = m_CurrentMaxHealth;
                }
            }
        }

        public Rectangle GetRect
        {
            get 
            {
                m_DrawnRect.X = (int)(m_Position.X);
                m_DrawnRect.Y = (int)(m_Position.Y);
                m_DrawnRect.Width = (int)m_Dimensions.X;
                m_DrawnRect.Height = (int)m_Dimensions.Y;
                return m_DrawnRect;
            }
        }

        public bool Intersects(Rectangle a_OtherRect)
        {
            Rectangle collision = new Rectangle(GetRect.X + (int)(m_Dimensions.X * 0.5f),
                                                GetRect.Y + (int)(m_Dimensions.Y * 0.5f),
                                                GetRect.Width,
                                                GetRect.Height);
            return collision.Intersects(a_OtherRect);
        }

        public void ResetPosition()
        {
            m_Position = m_StartPos;
        }

        public Vector2 Position
        {
            get { return m_Position; }
        }

        public Vector2 Velocity
        {
            get { return m_Velocity; }
        }

        public float MaxVelocity
        {
            get { return m_MaxVelocity; }
        }

        public void MoveOffScreen()
        {
            m_Position.X = -1000;
            m_Position.Y = -1000;
        }

        public bool Alive
        {
            get { return m_IsAlive; }
        }

        public int AttackValue
        {
            get { return m_AttackValue; }
        }

        public void IncreaseAttackSpeed()
        {
            m_ShootReady -= 0.25f;
            if (m_ShootReady < m_MaxAttackSpeed)
            {
                m_ShootReady = m_MaxAttackSpeed;
            }
        }

        public void DecreaseAttackSpeed()
        {
            m_ShootReady += 0.25f;
        }

        public void IncreaseMovementSpeed()
        {
            m_MovementSpeed++;
            if (m_MovementSpeed > m_MaxMovementSpeed)
            {
                m_MovementSpeed = m_MaxMovementSpeed;
            }
        }

        public void DecreaseMovementSpeed()
        {
            m_MovementSpeed--;
            if (m_MovementSpeed < 15)
            {
                m_MovementSpeed = 15;
            }
        }

        public void IncreaseMaxHealth()
        {
            m_CurrentMaxHealth++;
            if (m_CurrentMaxHealth >= m_MaxHealth)
            {
                m_CurrentMaxHealth = m_MaxHealth;
            }

            //if at full health and max health is increased
            //current health is increased with the change
            if (m_CurrentHealth == m_CurrentMaxHealth)
            {
                m_CurrentHealth = m_CurrentMaxHealth;
            }
        }

        public void DecreaseMaxHealth()
        {
            m_CurrentMaxHealth--;
            if (m_CurrentMaxHealth < 1)
            {
                m_CurrentMaxHealth = 1;
            }

            //make sure that current health and current max health are in line with each other
            if (m_CurrentHealth > m_CurrentMaxHealth)
            {
                m_CurrentHealth = m_CurrentMaxHealth;
            }
        }

        public void IncreaseBulletSize()
        {
            m_BulletSize += 1.5f;
            if (m_BulletSize >= m_MaxBulletSize)
            {
                m_BulletSize = m_MaxBulletSize;
            }
        }

        public void DecreaseBulletSize()
        {
            m_BulletSize -= 1.5f;
            if (m_BulletSize <= 0)
            {
                m_BulletSize = 1;
            }
        }

        public void IncreaseAttackDamage()
        {
            m_AttackValue++;
            if (m_AttackValue >= m_MaxAttackValue)
            {
                m_AttackValue = m_MaxAttackValue;
            }
        }

        public void DecreaseAttackDamage()
        {
            m_AttackValue--;
            if (m_AttackValue < 1)
            {
                m_AttackValue = 1;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public class Player
    {
        Item currentItem;

        //TODO: show items used upon death
        List<Item> usedItems = new List<Item>();

        Vector2 m_Position;
        Vector2 m_StartPosition;
        Vector2 m_Velocity;
        Vector2 m_Dimensions;

        //initial movement speed, value that is reset to when Reset function is called
        float m_StartingMovementSpeed = 15;

        //saved movement speed, value that is reset to when player defeats a trap room
        float m_SavedMovementSpeed = 15;

        //actual movement speed of the player
        float m_MovementSpeed = 15;
        
        //check to make sure buffs don't go past this value
        float m_MaxMovementSpeed = 25;

        //multiplied with the velocity
        //represents how quickly the velocity
        //decreases over time
        float m_VelocityDecayRate;
        float m_MaxVelocity;

        //attack speed is calculated as
        //1 / shoot ready
        float m_MaxAttackSpeed = 0.25f;
        float m_SavedAttackSpeed = 1.5f;
        float m_StartingAttackSpeed = 1.0f;

        float m_ShootTimer = 1.0f;
        float m_ShootReady = 1.0f;

        Texture2D m_Texture;
        Color m_Colour;

        int m_AttackValue = 1;
        int m_SavedAttackValue = 1;
        int m_StartingAttackValue = 1;
        int m_MaxAttackValue = 10;

        int m_MaxHealth = 25;
        int m_CurrentMaxHealth = 12;
        int m_CurrentHealth = 12;
        int m_SavedMaxHealth = 12;
        int m_StartingMaxHealth = 12;

        float m_BulletSize = 10;
        float m_MaxBulletSize = 28;
        float m_SavedBulletSize = 10;
        float m_StartingBulletSize = 10;

        bool m_IsAlive = true;

        //TODO: assign value to attacker that killed player
        Enemy attacker = null;

        public Player(Vector2 a_Position, Vector2 a_Dimensions)
        {
            m_Texture = Utility.Instance.WhiteSquare;
            m_Colour = Color.Black;
            m_Position = a_Position;
            m_StartPosition = m_Position;
            m_Velocity = Vector2.Zero;
            m_Dimensions = a_Dimensions;
            m_VelocityDecayRate = 0.85f;
            m_MaxVelocity = 25;
        }

        public void Update(float dt)
        {
            if (m_IsAlive)
            {
                m_ShootTimer += dt;
                
                //so that it doesn't exceed the max value
                if (m_ShootTimer >= m_ShootReady)
                {
                    m_ShootTimer = m_ShootReady;
                }

                //remove unnecessary accuracy
                if (m_Velocity.Length() < 0.05f)
                {
                    m_Velocity = Vector2.Zero;
                }
                
                //no keys pressed, so velocity can start decreasing
                if (!(Utility.Instance.IsKeyHeld(Keys.W) ||
                      Utility.Instance.IsKeyHeld(Keys.S) ||
                      Utility.Instance.IsKeyHeld(Keys.A) ||
                      Utility.Instance.IsKeyHeld(Keys.D)))
                {
                    m_Velocity *= m_VelocityDecayRate;
                }

                //keys for movement
                if (Utility.Instance.IsKeyHeld(Keys.W))
                {
                    m_Velocity.Y += -1;
                }

                if (Utility.Instance.IsKeyHeld(Keys.S))
                {
                    m_Velocity.Y += 1;
                }

                if (Utility.Instance.IsKeyHeld(Keys.A))
                {
                    m_Velocity.X += -1;
                }

                if (Utility.Instance.IsKeyHeld(Keys.D))
                {
                    m_Velocity.X += 1;
                }

                //use current item
                if (Utility.Instance.IsKeyPressed(Keys.E))
                {
                    if (currentItem != null)
                    {
                        currentItem.Use();
                        currentItem = null;
                        usedItems.Add(currentItem);
                    }
                }

                //shooting using arrow keys
                if (m_ShootTimer >= m_ShootReady)
                {
                    if (Utility.Instance.IsKeyHeld(Keys.Left))
                    {
                        ProjectileManager.Instance.Shoot(m_Position, m_Velocity * 0.1f + new Vector2(-1, 0) * 5, new Vector2(m_BulletSize), Color.Black, m_AttackValue, true);
                        m_ShootTimer = 0;
                    }
                    else if (Utility.Instance.IsKeyHeld(Keys.Right))
                    {
                        ProjectileManager.Instance.Shoot(m_Position, m_Velocity * 0.1f + new Vector2(1, 0) * 5, new Vector2(m_BulletSize), Color.Black, m_AttackValue, true);
                        m_ShootTimer = 0;
                    }
                    else if (Utility.Instance.IsKeyHeld(Keys.Up))
                    {
                        ProjectileManager.Instance.Shoot(m_Position, m_Velocity * 0.1f + new Vector2(0, -1) * 5, new Vector2(m_BulletSize), Color.Black, m_AttackValue, true);
                        m_ShootTimer = 0;
                    }
                    else if (Utility.Instance.IsKeyHeld(Keys.Down))
                    {
                        ProjectileManager.Instance.Shoot(m_Position, m_Velocity * 0.1f + new Vector2(0, 1) * 5, new Vector2(m_BulletSize), Color.Black, m_AttackValue, true);
                        m_ShootTimer = 0;
                    }
                }

                //make sure that velocity doesnt exceed max velocity
                if (m_Velocity.Length() > 0)
                {
                    float mag = m_Velocity.Length();

                    m_Velocity = Vector2.Normalize(m_Velocity) * Math.Min(mag, m_MaxVelocity);
                }

                m_Position += m_Velocity * m_MovementSpeed * dt;

                if (m_CurrentHealth <= 0)
                {
                    m_IsAlive = false;
                }
            }
        }

        public void Draw()
        {
            if (m_IsAlive)
            {
                if (m_Texture != null)
                {
                    Utility.Instance.SB.Draw(m_Texture, GetRect, m_Colour);
                }

                //show player name of current item
                Utility.Instance.SB.DrawString(Utility.Instance.SF, "Current item: ", new Vector2((Utility.Instance.ScreenWidth - 1) * 0.75f, 0), Color.SteelBlue);
                Utility.Instance.SB.DrawString(Utility.Instance.SF, "Current item: ", new Vector2(Utility.Instance.ScreenWidth * 0.75f, 0), Color.Black);

                //draw something else if player doesn't have an item
                if (currentItem != null)
                {
                    Utility.Instance.SB.DrawString(Utility.Instance.SF, currentItem.GetItemType.ToString(), new Vector2((Utility.Instance.ScreenWidth - 1) * 0.75f, 10), Color.SteelBlue);
                    Utility.Instance.SB.DrawString(Utility.Instance.SF, currentItem.GetItemType.ToString(), new Vector2((Utility.Instance.ScreenWidth) * 0.75f, 10), Color.Black);
                }
                else
                {
                    Utility.Instance.SB.DrawString(Utility.Instance.SF, "None", new Vector2((Utility.Instance.ScreenWidth - 1) * 0.75f, 10), Color.SteelBlue);
                    Utility.Instance.SB.DrawString(Utility.Instance.SF, "None", new Vector2((Utility.Instance.ScreenWidth) * 0.75f, 10), Color.Black);
                }

                //health bar, bottom of screen
                Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle(10, Utility.Instance.ScreenHeight - 10, (int)((m_CurrentHealth / (float)m_CurrentMaxHealth) * (Utility.Instance.ScreenWidth - 50)), 25), Color.Plum);

                //y offset for the rectangles
                float rectYOffset = 0.05f;

                //y offset for the text
                float textYOffset = 0.01f;

#if DEBUG
                //draw debug info only in debug mode
                DrawStat("Max health stat", (float)m_CurrentMaxHealth, (float)m_MaxHealth, Utility.Instance.ScreenHeight * textYOffset, Utility.Instance.ScreenHeight * rectYOffset, Color.HotPink, Color.Gray);
                DrawStat("Attack speed stat", AttackSpeedCalc(m_ShootReady), (AttackSpeedCalc(m_MaxAttackSpeed)), Utility.Instance.ScreenHeight * (textYOffset + 0.1f), Utility.Instance.ScreenHeight * (rectYOffset + 0.1f), Color.HotPink, Color.Gray);
                DrawStat("Movement speed stat", (float)m_MovementSpeed, (float)m_MaxMovementSpeed, Utility.Instance.ScreenHeight * (textYOffset + 0.2f), Utility.Instance.ScreenHeight * (rectYOffset + 0.2f), Color.HotPink, Color.Gray);
                DrawStat("Bullet size stat", (float)m_BulletSize, (float)m_MaxBulletSize, Utility.Instance.ScreenHeight * (textYOffset + 0.3f), Utility.Instance.ScreenHeight * (rectYOffset + 0.3f), Color.HotPink, Color.Gray);
                DrawStat("Attack damage stat", (float)m_AttackValue, (float)m_MaxAttackValue, Utility.Instance.ScreenHeight * (textYOffset + 0.4f), Utility.Instance.ScreenHeight * (rectYOffset + 0.4f), Color.HotPink, Color.Gray);
                DrawStat("Current health", m_CurrentHealth, m_CurrentMaxHealth, Utility.Instance.ScreenHeight * (textYOffset + 0.5f), Utility.Instance.ScreenHeight * (rectYOffset + 0.5f), Color.HotPink, Color.Gray);

                DrawShootTimer(450, 10);
#endif
            }
        }

        void DrawStat(string a_Name, float currentValue, float maxValue, float textYPos, float rectYPos, Color a_TextColour, Color a_RectangleColour)
        {
            //spacing between rectangles
            int overallInterRectangularSpace = 20;

            //more spacing between rectangles
            float interRectangularSpace = 1.5f;

            //starting point for rectangles and text on the left hand side
            int leftSideOffset = 125;
            int numRects = 5;

            Utility.Instance.SB.DrawString(Utility.Instance.SF, a_Name, new Vector2(leftSideOffset, (int)(textYPos)), a_TextColour);

            //calculate how many rects to draw based on current value vs max value by number of rects specified
            int currentNumRects = (int)Math.Ceiling((currentValue / (float)maxValue) * numRects);
            for (int i = 0; i < currentNumRects; ++i)
            {
                Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)(overallInterRectangularSpace * (i * interRectangularSpace) + leftSideOffset), (int)(rectYPos), 10, 25), a_RectangleColour);
            }

            Utility.Instance.SB.DrawString(Utility.Instance.SF, currentValue.ToString() + " of max: " + maxValue.ToString(), new Vector2(leftSideOffset + currentNumRects * 25, rectYPos), a_TextColour);
        }

        void DrawShootTimer(int xPos, int yPos)
        {
            Utility.Instance.SB.DrawString(Utility.Instance.SF, "Can shoot when full: ", new Vector2(xPos - 100, yPos), Color.HotPink);
            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle(xPos, yPos, (int)((m_ShootTimer / m_ShootReady) * 50), 20), Color.Gray);
        }

        public Rectangle GetRect
        {
            get { return new Rectangle((int)m_Position.X, (int)m_Position.Y, (int)m_Dimensions.X, (int)m_Dimensions.Y); }
        }

        public bool Intersects(Rectangle a_Other)
        {
            return GetRect.Intersects(a_Other);
        }

        public void CheckBoundaries()
        {
            //take into account the dimensions of the door to the next room
            //boundaries along the walls follow that
            if (m_Position.X < Door.width - (int)(Door.width * 0.5f))
            {
                m_Position.X = Door.width - (int)(Door.width * 0.5f);
            }

            if (m_Position.Y < Door.height - (int)(Door.height * 0.5f))
            {
                m_Position.Y = Door.height - (int)(Door.height * 0.5f);
            }

            if (m_Position.X + m_Dimensions.X > Utility.Instance.ScreenWidth - (Door.width - (int)(Door.width * 0.5f)))
            {
                m_Position.X = Utility.Instance.ScreenWidth - m_Dimensions.X - ((Door.width - (int)(Door.width * 0.5f)));
            }

            if (m_Position.Y + m_Dimensions.Y > Utility.Instance.ScreenHeight - (Door.height - (int)(Door.height * 0.5f)))
            {
                m_Position.Y = Utility.Instance.ScreenHeight - m_Dimensions.Y - (Door.height - (int)(Door.height * 0.5f));
            }
        }

        //function is used when the player collides with enemy or wall
        public void PushBack(float dt)
        {
            float pushBackFactor = 10;
            m_Velocity += -m_Velocity * pushBackFactor * m_MovementSpeed * dt;
        }

        public Vector2 Velocity
        {
            get { return m_Velocity; }
        }

        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public int Health
        {
            get { return m_CurrentHealth; }
            set
            {
                m_CurrentHealth = value;

                //make sure health is within the correct limits when assigned
                if (m_CurrentHealth <= 0)
                {
                    m_CurrentHealth = 0;
                }

                if (m_CurrentHealth >= m_CurrentMaxHealth)
                {
                    m_CurrentHealth = m_CurrentMaxHealth;
                }
            }
        }

        public bool Alive
        {
            get
            {
                return m_IsAlive;
            }
        }

        public void SetAttacker(Enemy a_Attacker)
        {
            attacker = a_Attacker;
        }

        void ResetAlive()
        {
            m_IsAlive = true;
        }

        public void ResetPosition()
        {
            m_Position = m_StartPosition;
            m_Velocity = Vector2.Zero;
        }

        public void Reset()
        {
            ResetPosition();
            ResetAlive();
            ResetItem();
            ResetStats();
        }

        //used when player finishes a trap room
        public void ResetStats()
        {
            m_ShootReady = m_StartingAttackSpeed;
            m_AttackValue = m_StartingAttackValue;
            m_CurrentMaxHealth = m_StartingMaxHealth;
            m_MovementSpeed = m_StartingMovementSpeed;
            m_BulletSize = m_StartingBulletSize;
        }

        public void AddItem(Item a_Item)
        {
            currentItem = a_Item;
        }

        void ResetItem()
        {
            currentItem = null;
        }

        public Item CurrentItem
        {
            get { return currentItem; }
        }

        public int AttackValue
        {
            get { return m_AttackValue; }
        }

        #region Functions to modify stats of the player - used by items
        public void IncreaseAttackSpeed()
        {
            m_ShootReady -= 0.1f;
            if (m_ShootReady < m_MaxAttackSpeed)
            {
                m_ShootReady = m_MaxAttackSpeed;
            }
        }

        public void DecreaseAttackSpeed()
        {
            m_ShootReady += 0.1f;
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

            //haven't lost health, and max health increased,
            //so increase current health to match the new max
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
            if (m_AttackValue <= 0)
            {
                m_AttackValue = 1;
            }
        }

        public float AttackSpeedCalc(float timeToShoot)
        {
            return (float)(1 / timeToShoot);
        }
        #endregion

        #region Save/Reset stats before/after entering trap room
        public void SaveStats()
        {
            m_SavedAttackSpeed = m_ShootReady;
            m_SavedAttackValue = m_AttackValue;
            m_SavedMaxHealth = m_CurrentMaxHealth;
            m_SavedMovementSpeed = m_MovementSpeed;
            m_SavedBulletSize = m_BulletSize;
        }

        public void ResetStatsToSaved()
        {
            m_ShootReady = m_SavedAttackSpeed;
            m_AttackValue = m_SavedAttackValue;
            m_CurrentMaxHealth = m_SavedMaxHealth;
            m_MovementSpeed = m_SavedMovementSpeed;
            m_BulletSize = m_SavedBulletSize;
        }
        #endregion
    }
}

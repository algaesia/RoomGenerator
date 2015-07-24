using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //can apply to both player or enemy
    public enum TrapType
    {
        NONE,
        INCR_MOVEMENT_SPEED,
        DECR_MOVEMENT_SPEED,
        INCR_ATTACK_SPEED,
        DECR_ATTACK_SPEED,
        INCR_ATTACK_DAMAGE,
        DECR_ATTACK_DAMAGE,
        INCR_BULLET_SIZE,
        DECR_BULLET_SIZE,
        INCR_NUM_ENEMIES,
        DECR_NUM_ENEMIES,
        INCR_HEALTH,
        DECR_HEALTH,
        MAX_TRAPS,
    }

    public class EmptyRoom : PreRoom
    {
        public EmptyRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            roomType = PreRoomType.EMPTY;
            int i = 0;
            foreach (int n in a_Connections)
            {
                connections[i] = n;
                i++;
            }

            name = "Empty";
            colour = Color.White;

            textToDisplay.ChangeDisplayedText("What a boring empty room.");
        }

        public override void Initialise()
        {
            if (!initialised)
            {
                
            }

            base.Initialise();
        }
    }

    //modify connections to refer elsewhere
    public class RiddleRoom : PreRoom
    {
        public RiddleRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            roomType = PreRoomType.RIDDLE;

            if (a_Connections[3] != -1)
            {
                connections[0] = a_Connections[3];
            }

            if (a_Connections[2] != -1)
            {
                connections[1] = a_Connections[2];
            }

            if (a_Connections[1] != -1)
            {
                connections[2] = a_Connections[1];
            }

            if (a_Connections[0] != -1)
            {
                connections[3] = a_Connections[0];
            }

            name = "Riddle";
            colour = Color.LightBlue;

            textToDisplay.ChangeDisplayedText("The rooms are connected weird here.");
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void Draw(Rectangle size)
        {
            base.Draw(size);
        }
    }

    public class BossRoom : PreRoom
    {
        BossEnemy boss;

        public BossRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            roomType = PreRoomType.BOSS;
            int i = 0;
            foreach (int n in a_Connections)
            {
                connections[i] = n;
                i++;
            }

            name = "Boss";
            colour = Color.Red;

            textToDisplay.ChangeDisplayedText("Big bad boss room.");
        }

        public override void Initialise()
        {
            if (!initialised)
            {
                Vector2 randPos = new Vector2((float)(Utility.Instance.ScreenWidth * 0.25f + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth * 0.75f - Utility.Instance.ScreenWidth * 0.25f)),
                                             (float)(Utility.Instance.ScreenHeight * 0.25f + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight * 0.75f - Utility.Instance.ScreenHeight * 0.25f)));
                boss = new BossEnemy(randPos, Color.Black);

                defeated = false;
            }

            base.Initialise();
        }

        public override void Update(float dt)
        {
            if (!boss.Alive)
            {
                defeated = true;
            }

            Rectangle tempBossRect = boss.GetRect;
            tempBossRect.X -= tempBossRect.Width / 2;
            tempBossRect.Y -= tempBossRect.Height / 2;

            Rectangle tempPlayerRect = Dungeon.GetPlayer.GetRect;
            tempPlayerRect.X -= tempPlayerRect.Width / 2;
            tempPlayerRect.Y -= tempPlayerRect.Height / 2;

            foreach (Projectile n in ProjectileManager.Instance.Projectiles)
            {
                //player bullet attacking boss - works
                //top left v middle of texture moved to top left
                if (n.GetDrawnRect.Intersects(tempBossRect) && n.IsPlayerProjectile)
                {
                    boss.Health -= n.Damage;
#if DEBUG
                    n.SetColour(Color.Yellow);
#endif
                    n.IsAlive = false;
                }

                //boss bullet attacking player - working
                //top left v middle of texture moved to top left
                if (n.GetDrawnRect.Intersects(tempPlayerRect) && !n.IsPlayerProjectile)
                {
                    Dungeon.GetPlayer.Health -= boss.AttackValue;
#if DEBUG
                    n.SetColour(Color.Black);
#endif
                    n.IsAlive = false;
                }
            }

            //middle of texture moved to top left vs middle of texture moved to top left
            if (tempPlayerRect.Intersects(tempBossRect))
            {
                boss.Health -= Dungeon.GetPlayer.AttackValue;
                Dungeon.GetPlayer.Health -= boss.AttackValue;
                Dungeon.GetPlayer.PushBack(dt);
                boss.PushBack(dt);
            }

            boss.Update(dt);

            base.Update(dt);
        }

        public override void Draw(Rectangle size)
        {
            //base = background, minimap etc
            //drawn first
            base.Draw(size);

            boss.Draw();
        }
    }

    //monsters spawn, but player has decreased stats
    //eg reduced movement speed or attack speed
    //random num of monsters
    public class TrapRoom : PreRoom
    {
        int maxMonsters = 5;
        TrapType roomTrapType;
#if DEBUG
        bool enemyBuffed = false;
        OnScreenText buffText;
#endif

        public TrapRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            defeated = false;
            roomType = PreRoomType.TRAP;
            int count = 0;
            foreach (int n in a_Connections)
            {
                connections[count] = n;
                count++;
            }

            name = "Trap";
            colour = Color.Blue;

#if DEBUG
            buffText = new OnScreenText("EMPTY TEXT", Vector2.Zero, Color.Black, Color.White, Utility.Instance.LabelsFont);
#endif
            textToDisplay.ChangeDisplayedText("It's a trap!");
        }

        public override void Initialise()
        {
            if (!initialised)
            {
                roomTrapType = (TrapType)(Utility.Instance.cRand.NextDouble() * (int)TrapType.MAX_TRAPS);
                Console.WriteLine("Trap type: " + roomTrapType.ToString());

                if (!defeated)
                {
                    double enemyBuffChance = Utility.Instance.cRand.NextDouble();
                    int numMonsters = (int)(Utility.Instance.cRand.NextDouble() * maxMonsters);
                    if (roomTrapType == TrapType.INCR_NUM_ENEMIES)
                    {
                        numMonsters *= 2;
                    }
                    else if (roomTrapType == TrapType.DECR_NUM_ENEMIES)
                    {
                        numMonsters = numMonsters == 0 ? 1 : numMonsters / 2;
                    }

                    if (numMonsters == 0)
                    {
                        numMonsters = 1;
                    }

                    for (int i = 0; i < numMonsters; ++i)
                    {
                        //increased values
                        Enemy t = null;
                        double typeChance = Utility.Instance.cRand.NextDouble();

                        //pursuer
                        if (typeChance >= 0 && typeChance < 0.2)
                        {
                            t = new Pursuer(new Vector2((float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth - 500)), (float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight - 500))), new Vector2(32), Color.Yellow);
                        }
                        else if (typeChance >= 0.2 && typeChance < 0.4)
                        {
                            t = new PursuingShooter(new Vector2((float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth - 500)), (float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight - 500))), new Vector2(32), Color.Yellow);
                        }
                        else if (typeChance >= 0.4 && typeChance < 0.6)
                        {
                            t = new Avoider(new Vector2((float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth - 500)), (float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight - 500))), new Vector2(32), Color.Yellow);
                        }
                        else if (typeChance >= 0.6 && typeChance < 0.8)
                        {
                            t = new EvadingShooter(new Vector2((float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth - 500)), (float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight - 500))), new Vector2(32), Color.Yellow);
                        }
                        else if (typeChance >= 0.8 && typeChance < 1)
                        {
                            t = new Shooter(new Vector2((float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth - 500)), (float)(250 + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight - 500))), new Vector2(32), Color.Yellow);
                        }

                        if (enemyBuffChance > 0.5)
                        {
#if DEBUG
                            enemyBuffed = true;
#endif
                            Console.WriteLine("ENEMY GETS BUFFED");
                            switch (roomTrapType)
                            {
                                case TrapType.INCR_ATTACK_DAMAGE:
                                    t.IncreaseAttackDamage();
                                    t.IncreaseAttackDamage();
                                    t.IncreaseAttackDamage();
                                    break;
                                case TrapType.INCR_ATTACK_SPEED:
                                    t.IncreaseAttackSpeed();
                                    t.IncreaseAttackSpeed();
                                    t.IncreaseAttackSpeed();
                                    break;
                                case TrapType.INCR_BULLET_SIZE:
                                    t.IncreaseBulletSize();
                                    t.IncreaseBulletSize();
                                    t.IncreaseBulletSize();
                                    break;
                                case TrapType.INCR_MOVEMENT_SPEED:
                                    t.IncreaseMovementSpeed();
                                    t.IncreaseMovementSpeed();
                                    t.IncreaseMovementSpeed();
                                    break;
                                case TrapType.INCR_HEALTH:
                                    t.IncreaseMaxHealth();
                                    t.IncreaseMaxHealth();
                                    break;
                                default: break;
                            }
                        }

                        Console.WriteLine("Adding: " + t.GetType().ToString());
                        enemies.Add(t);
                    }

                    //player gets buffed instead
                    if (enemyBuffChance <= 0.5)
                    {
#if DEBUG
                        enemyBuffed = false;
#endif
                        Console.WriteLine("PLAYER GETS BUFFED");
                        Dungeon.GetPlayer.SaveStats();
                        switch (roomTrapType)
                        {
                            case TrapType.INCR_ATTACK_DAMAGE:
                                Dungeon.GetPlayer.IncreaseAttackDamage();
                                Dungeon.GetPlayer.IncreaseAttackDamage();
                                Dungeon.GetPlayer.IncreaseAttackDamage();
                                break;
                            case TrapType.INCR_ATTACK_SPEED:
                                Dungeon.GetPlayer.IncreaseAttackSpeed();
                                Dungeon.GetPlayer.IncreaseAttackSpeed();
                                Dungeon.GetPlayer.IncreaseAttackSpeed();
                                break;
                            case TrapType.INCR_BULLET_SIZE:
                                Dungeon.GetPlayer.IncreaseBulletSize();
                                Dungeon.GetPlayer.IncreaseBulletSize();
                                Dungeon.GetPlayer.IncreaseBulletSize();
                                break;
                            case TrapType.INCR_MOVEMENT_SPEED:
                                Dungeon.GetPlayer.IncreaseMovementSpeed();
                                Dungeon.GetPlayer.IncreaseMovementSpeed();
                                Dungeon.GetPlayer.IncreaseMovementSpeed();
                                break;
                            case TrapType.INCR_HEALTH:
                                Dungeon.GetPlayer.IncreaseMaxHealth();
                                Dungeon.GetPlayer.IncreaseMaxHealth();
                                break;
                            default: break;
                        }
                    }
                }
            }

            base.Initialise();
        }

        public override void Update(float dt)
        {
            if (enemies.Count == 0)
            {
                defeated = true;
            }

            for (int i = 0; i < enemies.Count; ++i)
            {
                if (!enemies[i].Alive)
                {
                    enemies.Remove(enemies[i]);
                }
            }

            base.Update(dt);
        }

        public override void Draw(Rectangle size)
        {
            base.Draw(size);
#if DEBUG
            if (!defeated)
            {
                if (enemyBuffed)
                {
                    Vector2 textDimensions = Utility.Instance.LabelsFont.MeasureString("Enemy is buffed with: " + roomTrapType.ToString());
                    buffText.ChangeText("Enemy is buffed with: " + roomTrapType.ToString());
                    buffText.ChangePosition(new Vector2(Utility.Instance.ScreenWidth * 0.5f - textDimensions.X * 0.5f, Utility.Instance.ScreenHeight * 0.25f - textDimensions.Y * 0.25f));
                    buffText.Draw();
                }
                else
                {
                    Vector2 textDimensions = Utility.Instance.LabelsFont.MeasureString("Player is buffed with: " + roomTrapType.ToString());
                    buffText.ChangeText("Player is buffed with: " + roomTrapType.ToString());
                    buffText.ChangePosition(new Vector2(Utility.Instance.ScreenWidth * 0.5f - textDimensions.X * 0.5f, Utility.Instance.ScreenHeight * 0.25f - textDimensions.Y * 0.25f));
                    buffText.Draw();
                }
            }
#endif
            foreach (Enemy n in enemies)
            {
                n.Draw();
            }
        }
    }

    public class TreasureRoom : PreRoom
    {
        Item randItem;

        public TreasureRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            roomType = PreRoomType.TREASURE;
            int counter = 0;
            foreach (int n in a_Connections)
            {
                connections[counter] = n;
                counter++;
            }

            name = "Treasure";
            colour = Color.OrangeRed;

            Vector2 randPos = new Vector2((float)(Utility.Instance.ScreenWidth * 0.25f + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth * 0.75f - Utility.Instance.ScreenWidth * 0.25f)),
                                          (float)(Utility.Instance.ScreenHeight * 0.25f + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight * 0.75f - Utility.Instance.ScreenHeight * 0.25f)));
            ItemType randType = Item.RandomType();
            switch (randType)
            {
                case ItemType.ARONDIGHT:
                    randItem = new Arondight(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.CALEDFWLCH:
                    randItem = new Caledfwlch(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.CHENTU:
                    randItem = new Chentu(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.GUNGNIR:
                    randItem = new Gungnir(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.HARVESTEROFSOULS:
                    randItem = new HarvesterOfSouls(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.HEAVENSWILL:
                    randItem = new HeavensWill(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.LOTUSTREE:
                    randItem = new LotusTree(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.MEGINGJORD:
                    randItem = new Megingjord(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.MJOLNIR:
                    randItem = new Mjolnir(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.MYSTELTAINN:
                    randItem = new Mysteltainn(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.PERSEUSHELM:
                    randItem = new PerseusHelm(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.ORICHALCUM:
                    randItem = new Orichalcum(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.PRIWEN:
                    randItem = new Priwen(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.SPEAROFLONGINUS:
                    randItem = new SpearOfLonginus(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.SVRIVATSA:
                    randItem = new Svrivatsa(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.TARNHELM:
                    randItem = new TarnHelm(randPos, Utility.Instance.WhiteSquare);
                    break;
                case ItemType.VAJRA:
                    randItem = new Vajra(randPos, Utility.Instance.WhiteSquare);
                    break;
            }

            textToDisplay.ChangeDisplayedText("Sweet, sweet treasure.");
        }

        public override void Update(float dt)
        {
            if (randItem.Intersects(Dungeon.GetPlayer.GetRect))
            {
                Item rejectItem = null;

                //player already has an item
                if (Dungeon.GetPlayer.CurrentItem != null)
                {
                    //save out current item
                    rejectItem = Dungeon.GetPlayer.CurrentItem;

                    //add item on floor to player
                    Dungeon.GetPlayer.AddItem(randItem);

                    rejectItem.Position = randItem.Position;
                    rejectItem.isAlive = true;

                    //set item on floor to reject item
                    randItem = rejectItem;
                }
                else
                {
                    randItem.isAlive = false;
                    Dungeon.GetPlayer.AddItem(randItem);
                }
            }

            if (!randItem.isAlive)
            {
                randItem.MoveOffScreen();
            }

            base.Update(dt);
        }

        public override void Draw(Rectangle size)
        {
            base.Draw(size);

            randItem.Draw();
        }
    }

    public class MonsterRoom : PreRoom
    {
        int maxEnemies = 5;
        OnScreenText enemiesLeft;

        public MonsterRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            enemiesLeft = new OnScreenText("Enemies left: ", Vector2.Zero, Color.Black, Color.White, Utility.Instance.LabelsFont);
            defeated = false;
            roomType = PreRoomType.MONSTER;
            int counter = 0;
            foreach (int n in a_Connections)
            {
                connections[counter] = n;
                counter++;
            }

            name = "Monster";
            colour = Color.Red;

            int numEnemies = 1 + (int)(Utility.Instance.cRand.NextDouble() * (maxEnemies - 1));
            for (int i = 0; i < numEnemies; ++i)
            {
                Vector2 randPos = new Vector2((float)(Utility.Instance.ScreenWidth * 0.25f + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenWidth * 0.75f - Utility.Instance.ScreenWidth * 0.25f)),
                                              (float)(Utility.Instance.ScreenHeight * 0.25f + Utility.Instance.cRand.NextDouble() * (Utility.Instance.ScreenHeight * 0.75f - Utility.Instance.ScreenHeight * 0.25f)));
                float dimensions = 20;
                Enemy t = null;

                double typeChance = Utility.Instance.cRand.NextDouble();

                if (typeChance >= 0 && typeChance < 0.2)
                {
                    t = new Pursuer(randPos, new Vector2(dimensions), Color.DarkGreen);
                }
                else if (typeChance >= 0.2 && typeChance < 0.4)
                {
                    t = new PursuingShooter(randPos, new Vector2(dimensions), Color.DeepSkyBlue);
                }
                else if (typeChance >= 0.4 && typeChance < 0.6)
                {
                    t = new Avoider(randPos, new Vector2(dimensions), Color.HotPink);
                }
                else if (typeChance >= 0.6 && typeChance < 0.8)
                {
                    t = new EvadingShooter(randPos, new Vector2(dimensions), Color.LightYellow);
                }
                else
                {
                    t = new Shooter(randPos, new Vector2(dimensions), Color.SpringGreen);
                }

                if (t != null)
                {
                    Console.WriteLine(t.GetType().ToString());
                    enemies.Add(t);
                }
            }

            textToDisplay.ChangeDisplayedText("Kill everything first, then you can leave.");
        }

        public override void Update(float dt)
        {
            if (enemies.Count == 0)
            {
                defeated = true;
            }

            //remove dead enemies
            for (int i = 0; i < enemies.Count; ++i)
            {
                if (!enemies[i].Alive)
                {
                    enemies.Remove(enemies[i]);
                }
            }

            enemiesLeft.Update(dt);

            base.Update(dt);
        }

        public override void Draw(Rectangle size)
        {
            base.Draw(size);

            if (!defeated)
            {
                enemiesLeft.ChangePosition(new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.15f));
                enemiesLeft.ChangeText("Enemies left: " + enemies.Count.ToString());
                enemiesLeft.Draw();
            }
        }
    }

    public class VistaRoom : PreRoom
    {
        public VistaRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            roomType = PreRoomType.VISTA;
            int i = 0;
            foreach (int n in a_Connections)
            {
                connections[i] = n;
                i++;
            }

            name = "Vista";
            colour = Color.Brown;

            textToDisplay.ChangeDisplayedText("Soon to be implemented vistas.");
        }
    }

    public class EntranceRoom : PreRoom
    {
        public EntranceRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            roomType = PreRoomType.ENTRANCE;
            int i = 0;
            foreach (int n in a_Connections)
            {
                connections[i] = n;
                i++;
            }

            name = "Entrance";
            colour = Color.Gold;

            textToDisplay.ChangeDisplayedText("The beginning of a great adventure.");
        }
    }

    public class ExitRoom : PreRoom
    {
        Door exitDoor;

        public ExitRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            roomType = PreRoomType.EXIT;
            int i = 0;
            foreach (int n in a_Connections)
            {
                connections[i] = n;
                i++;
            }

            name = "Exit";
            colour = Color.BurlyWood;

            exitDoor = new Door((int)(Utility.Instance.ScreenWidth * 0.35f), (int)(Utility.Instance.ScreenHeight * 0.25f));

            textToDisplay.ChangeDisplayedText("The end of a great adventure.");
        }

        public override void Update(float dt)
        {
            if (Dungeon.GetPlayer.Intersects(exitDoor.Collider))
            {
                //go back to the main menu
                GameStateManager.Instance.ChangeGameState("End of dungeon");
                return;
            }

            base.Update(dt);
        }

        public override void Draw(Rectangle size)
        {
            base.Draw(size);

            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, exitDoor.drawnBox, Color.DarkOrchid);
        }
    }

    public class NextLevelRoom : PreRoom
    {
        Door nextLevelDoor;

        public NextLevelRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            roomType = PreRoomType.NEXTLEVEL;
            int i = 0;
            foreach (int n in a_Connections)
            {
                connections[i] = n;
                i++;
            }

            name = "Next Level";
            colour = Color.DarkRed;

            nextLevelDoor = new Door((int)(Utility.Instance.ScreenWidth * 0.35f), (int)(Utility.Instance.ScreenHeight * 0.25f));

            textToDisplay.ChangeDisplayedText("The adventure continues!");
        }

        public override void Update(float dt)
        {
            if (Dungeon.GetPlayer.Intersects(nextLevelDoor.Collider))
            {
                GameStateManager.Instance.ChangeGameState("Next level");
                Dungeon.GetPlayer.ResetPosition();
            }

            base.Update(dt);
        }

        public override void Draw(Rectangle size)
        {
            base.Draw(size);

            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, nextLevelDoor.drawnBox, Color.Coral);
        }
    }

    public class NoRoom : PreRoom
    {
        public NoRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
            defeated = false;
            roomType = PreRoomType.NO_ROOM;
            int i = 0;
            foreach (int n in a_Connections)
            {
                connections[i] = n;
                i++;
            }

            name = "No Room";
            colour = Color.Black;
        }
    }
}

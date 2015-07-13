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

            foreach (Projectile n in ProjectileManager.Instance.Projectiles)
            {
                //player bullet attacking boss
                if (n.GetRect.Intersects(boss.GetRect) && n.IsPlayerProjectile)
                {
                    boss.Health -= n.Damage;
                    n.IsAlive = false;
                }

                //boss bullet attacking player
                if (n.GetRect.Intersects(Dungeon.GetPlayer.GetRect) && !n.IsPlayerProjectile)
                {
                    Dungeon.GetPlayer.Health -= boss.AttackValue;
                    n.IsAlive = false;
                }
            }

            if (Dungeon.GetPlayer.GetRect.Intersects(boss.GetRect))
            {
                boss.Health -= Dungeon.GetPlayer.AttackValue;
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
        bool enemyBuffed = false;

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
                            enemyBuffed = true;
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
                        enemyBuffed = false;
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

            foreach (Enemy n in enemies)
            {
                n.Update(dt);
            }

            foreach (Projectile projectile in ProjectileManager.Instance.Projectiles)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.GetRect.Intersects(projectile.GetRect) &&
                        projectile.IsPlayerProjectile)
                    {
                        projectile.IsAlive = false;
                        enemy.Health -= projectile.Damage;
                    }
                }

                if (!projectile.IsPlayerProjectile && 
                    Dungeon.GetPlayer.Intersects(projectile.GetRect))
                {
                    projectile.IsAlive = false;
                    Dungeon.GetPlayer.Health -= projectile.Damage;
                }
            }

            foreach (Enemy enemy in enemies)
            {
                if (enemy.GetRect.Intersects(Dungeon.GetPlayer.GetRect))
                {
                    Dungeon.GetPlayer.Health -= enemy.AttackValue;
                    enemy.Health -= Dungeon.GetPlayer.AttackValue;
                    Dungeon.GetPlayer.PushBack(dt);
                    enemy.PushBack(dt);
                }
            }

            foreach (Enemy n in enemies)
            {
                foreach (Enemy j in enemies)
                {
                    if (n == j)
                    {
                        continue;
                    }

                    if (n.GetRect.Intersects(j.GetRect))
                    {
                        n.PushBack(dt);
                        j.PushBack(dt);
                    }
                }
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
                    Utility.Instance.SB.DrawString(Utility.Instance.SF, "Enemy is buffed with: " + roomTrapType.ToString(), new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.1f), Color.Gainsboro);
                }
                else
                {
                    Utility.Instance.SB.DrawString(Utility.Instance.SF, "Player is buffed with: " + roomTrapType.ToString(), new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight * 0.1f), Color.Gainsboro);
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
                    randItem = new Arondight(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.CALEDFWLCH:
                    randItem = new Caledfwlch(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.CHENTU:
                    randItem = new Chentu(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.GUNGNIR:
                    randItem = new Gungnir(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.HARVESTEROFSOULS:
                    randItem = new HarvesterOfSouls(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.HEAVENSWILL:
                    randItem = new HeavensWill(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.LOTUSTREE:
                    randItem = new LotusTree(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.MEGINGJORD:
                    randItem = new Megingjord(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.MJOLNIR:
                    randItem = new Mjolnir(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.MYSTELTAINN:
                    randItem = new Mysteltainn(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.PERSEUSHELM:
                    randItem = new PerseusHelm(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.ORICHALCUM:
                    randItem = new Orichalcum(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.PRIWEN:
                    randItem = new Priwen(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.SPEAROFLONGINUS:
                    randItem = new SpearOfLonginus(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.SVRIVATSA:
                    randItem = new Svrivatsa(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.TARNHELM:
                    randItem = new TarnHelm(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
                case ItemType.VAJRA:
                    randItem = new Vajra(randPos, new Vector2(25), Utility.Instance.WhiteSquare);
                    break;
            }
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

        public MonsterRoom(int a_X, int a_Y, int[] a_Connections)
            : base(a_X, a_Y)
        {
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

                Console.WriteLine("Type chance is: " + typeChance);

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
                    Console.WriteLine("Adding: " + t.GetType().ToString());
                    enemies.Add(t);
                }
            }
        }

        public override void Update(float dt)
        {
            if (enemies.Count == 0)
            {
                defeated = true;
            }

            foreach (Enemy n in enemies)
            {
                n.Update(dt);
            }

            foreach (Projectile projectile in ProjectileManager.Instance.Projectiles)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.GetRect.Intersects(projectile.GetRect) && 
                        projectile.IsPlayerProjectile)
                    {
                        projectile.IsAlive = false;
                        enemy.Health -= projectile.Damage;
                    }
                }

                if (!projectile.IsPlayerProjectile &&
                    Dungeon.GetPlayer.Intersects(projectile.GetRect))
                {
                    projectile.IsAlive = false;
                    Dungeon.GetPlayer.Health -= projectile.Damage;
                }
            }

            foreach (Enemy enemy in enemies)
            {
                if (enemy.GetRect.Intersects(Dungeon.GetPlayer.GetRect))
                {
                    Dungeon.GetPlayer.Health -= enemy.AttackValue;
                    enemy.Health -= Dungeon.GetPlayer.AttackValue;
                    Dungeon.GetPlayer.PushBack(dt);
                    enemy.PushBack(dt);
                }
            }

            foreach (Enemy n in enemies)
            {
                foreach (Enemy j in enemies)
                {
                    if (n == j)
                    {
                        continue;
                    }

                    if (n.GetRect.Intersects(j.GetRect))
                    {
                        n.PushBack(dt);
                        j.PushBack(dt);
                    }
                }
            }

            //remove dead enemies
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

            foreach (Enemy n in enemies)
            {
                n.Draw();
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
        }

        public override void Update(float dt)
        {
            if (Dungeon.GetPlayer.Intersects(exitDoor.collisionBox))
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

            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, exitDoor.collisionBox, Color.DarkOrchid);
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
        }

        public override void Update(float dt)
        {
            if (Dungeon.GetPlayer.Intersects(nextLevelDoor.collisionBox))
            {
                GameStateManager.Instance.ChangeGameState("Next level");
                Dungeon.GetPlayer.ResetPosition();
            }

            base.Update(dt);
        }

        public override void Draw(Rectangle size)
        {
            base.Draw(size);

            Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, nextLevelDoor.collisionBox, Color.Coral);
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

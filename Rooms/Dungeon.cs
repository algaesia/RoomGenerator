using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //sealed prevents anything from inheriting from it
    public class Dungeon
    {
        //used to loosely restrict how many rooms are generated
        //checked in ResetDungeon
        const int m_MinRooms = 20;
        const int m_MaxRooms = 40;

        //used as the starting point for offsets when drawing
        //the mini map
        //used in DrawMinimap
        Vector2 m_StartPos;

        //where all the room data is stored
        List<PreRoom> m_PreRooms = new List<PreRoom>();

        //used as an indicator for where 
        //the player currently is
        //only the current room is updated and drawn
        PreRoom m_CurrentRoom;

        //all rooms have the same width and height
        int m_RoomWidth;
        int m_RoomHeight;

        //static required for boundary checks in PreRoom class
        static int m_DungeonHeight;
        static int m_DungeonWidth;

        //one player per dungeon
        //static required for access in other classes
        //mainly collision and targeting by enemies
        static Player player;

        //static required for NextLevel class, signals to generate new dungeon, without resetting player stats
        static bool generateNextLevel = false;

        //count for how many filled rooms (GetRoomType != NO_ROOM)
        //incremented in DeterminePreRoomTypes
        int numFilledRooms = 0;

        //player marker on the minimap
        OnScreenText playerMarker;

        //dungeon width * dungeon height = total number of rooms
        //room width and height are the same
        public Dungeon(int dungeonWidth, int dungeonHeight, int roomSize)
        {
            m_StartPos = Vector2.Zero;

            m_DungeonWidth = dungeonWidth;
            m_DungeonHeight = dungeonHeight;

            m_RoomWidth = m_RoomHeight = roomSize;

            //player starts in the middle of the screen
            player = new Player(new Vector2(Utility.Instance.ScreenWidth / 2, Utility.Instance.ScreenHeight / 2), new Vector2(32));

            //reset the dungeon completely - including player stats and items
            ResetDungeon(true);

            playerMarker = new OnScreenText("H", Vector2.Zero, Color.Black, Color.White, Utility.Instance.SF);
        }

        //update player, projectiles and current room
        //if the current room is defeated, then player can
        //move into the next rooms
        //if the next level needs to be generated, then do so
        //without resetting the player
        public void Update(float dt)
        {
            if (generateNextLevel)
            {
                //pass in false, don't reset player items and stats
                ResetDungeon(false);
                generateNextLevel = false;
            }

            player.Update(dt);
            ProjectileManager.Instance.Update(dt);

            if (Utility.Instance.IsKeyPressed(Keys.R))
            {
                ResetDungeon();
            }

            if (Utility.Instance.IsKeyPressed(Keys.T))
            {
                UncoverRooms();
            }

            m_CurrentRoom.Update(dt);

            if (m_CurrentRoom.Defeated)
            {
                //defeat trap room, then stats go back to normal
                if (m_CurrentRoom.GetRoomType == PreRoomType.TRAP)
                {
                    //saving of stats is done when player enters the trap room
                    player.ResetStatsToSaved();
                }

                #region Door collision checks
                if (m_CurrentRoom.CheckDoorCollidable(Direction.NORTH) && player.Intersects(m_CurrentRoom.GetDoorCollider(Direction.NORTH)))
                {
                    //check that connection is valid
                    int northOfCurrent = m_CurrentRoom.GetConnection(Direction.NORTH);
                    if (northOfCurrent != -1)
                    {
                        //get the next room
                        PreRoom nextRoom = m_PreRooms[northOfCurrent];

                        //make sure it is a valid room
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            //move player to just above south door
                            player.Position = new Vector2(Utility.Instance.ScreenWidth * 0.5f, Utility.Instance.ScreenHeight - Door.height * 2.5f);

                            m_CurrentRoom = nextRoom;
                            m_PreRooms[northOfCurrent].Explored = true;
                            m_CurrentRoom.Initialise();
                            ProjectileManager.Instance.ClearBullets();
                        }
                    }
                }

                if (m_CurrentRoom.CheckDoorCollidable(Direction.SOUTH) && player.Intersects(m_CurrentRoom.GetDoorCollider(Direction.SOUTH)))
                {
                    int southOfCurrent = m_CurrentRoom.GetConnection(Direction.SOUTH);
                    if (southOfCurrent != -1)
                    {
                        PreRoom nextRoom = m_PreRooms[southOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            player.Position = new Vector2(Utility.Instance.ScreenWidth * 0.5f, Door.height * 1.5f);

                            m_CurrentRoom = nextRoom;
                            m_PreRooms[southOfCurrent].Explored = true;
                            m_CurrentRoom.Initialise();
                            ProjectileManager.Instance.ClearBullets();
                        }
                    }
                }

                if (m_CurrentRoom.CheckDoorCollidable(Direction.EAST) && player.Intersects(m_CurrentRoom.GetDoorCollider(Direction.EAST)))
                {
                    int eastOfCurrent = m_CurrentRoom.GetConnection(Direction.EAST);
                    if (eastOfCurrent != -1)
                    {
                        //east
                        PreRoom nextRoom = m_PreRooms[eastOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            //move player to just near west door
                            player.Position = new Vector2(Door.width * 1.5f, Utility.Instance.ScreenHeight * 0.5f);

                            m_CurrentRoom = nextRoom;
                            m_PreRooms[eastOfCurrent].Explored = true;
                            m_CurrentRoom.Initialise();
                            ProjectileManager.Instance.ClearBullets();
                        }
                    }
                }

                if (m_CurrentRoom.CheckDoorCollidable(Direction.WEST) && player.Intersects(m_CurrentRoom.GetDoorCollider(Direction.WEST)))
                {
                    int westOfCurrent = m_CurrentRoom.GetConnection(Direction.WEST);
                    if (westOfCurrent != -1)
                    {
                        //west
                        PreRoom nextRoom = m_PreRooms[westOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            //move player to just near east door
                            player.Position = new Vector2(Utility.Instance.ScreenWidth - Door.width * 2.5f, Utility.Instance.ScreenHeight * 0.5f);

                            m_CurrentRoom = nextRoom;
                            m_PreRooms[westOfCurrent].Explored = true;
                            m_CurrentRoom.Initialise();
                            ProjectileManager.Instance.ClearBullets();
                        }
                    }
                }
                #endregion
            }
        }

        //draw the current room, doors and minimap
        public void Draw()
        {
            //fill the screen with the current room
            m_CurrentRoom.Draw(new Rectangle(0, 0, Utility.Instance.ScreenWidth, Utility.Instance.ScreenHeight));

            DrawDoors();
            m_CurrentRoom.DrawTextBox();
            DrawMiniMap();

            player.Draw();
            ProjectileManager.Instance.Draw();
        }

        //check current room and four adj rooms, making sure
        //that current and each adj room are 
        //not both treasure rooms
        void RemoveAdjacentTreasureRooms()
        {
            for (int i = 0; i < m_PreRooms.Count; ++i)
            {
                PreRoom currentRoom = m_PreRooms[i];

                if (currentRoom.GetRoomType != PreRoomType.NO_ROOM)
                {
                    //check connection is valid
                    int northOfCurrent = currentRoom.GetConnection(Direction.NORTH);
                    if (northOfCurrent != -1)
                    {
                        //get next room
                        PreRoom nextRoom = m_PreRooms[northOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            //north of current and current are treasure rooms
                            if (nextRoom.GetRoomType == PreRoomType.TREASURE && currentRoom.GetRoomType == PreRoomType.TREASURE)
                            {
                                //get a random type, excluding treasure, exit, next level and boss
                                PreRoomType t = RandomRoomTypeExcluding(new HashSet<int>() { (int)PreRoomType.TREASURE, (int)PreRoomType.EXIT, (int)PreRoomType.NEXTLEVEL, (int)PreRoomType.BOSS });
                                nextRoom.ChangeRoomType(t);
                            }
                        }
                    }

                    int southOfCurrent = currentRoom.GetConnection(Direction.SOUTH);
                    if (southOfCurrent != -1)
                    {
                        PreRoom nextRoom = m_PreRooms[southOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            if (nextRoom.GetRoomType == PreRoomType.TREASURE && currentRoom.GetRoomType == PreRoomType.TREASURE)
                            {
                                PreRoomType t = RandomRoomTypeExcluding(new HashSet<int>() { (int)PreRoomType.TREASURE, (int)PreRoomType.EXIT, (int)PreRoomType.NEXTLEVEL, (int)PreRoomType.BOSS });
                                nextRoom.ChangeRoomType(t);
                            }
                        }
                    }

                    int eastOfCurrent = currentRoom.GetConnection(Direction.EAST);
                    if (eastOfCurrent != -1)
                    {
                        PreRoom nextRoom = m_PreRooms[eastOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            if (nextRoom.GetRoomType == PreRoomType.TREASURE && currentRoom.GetRoomType == PreRoomType.TREASURE)
                            {
                                PreRoomType t = RandomRoomTypeExcluding(new HashSet<int>() { (int)PreRoomType.TREASURE, (int)PreRoomType.EXIT, (int)PreRoomType.NEXTLEVEL, (int)PreRoomType.BOSS });
                                nextRoom.ChangeRoomType(t);
                            }
                        }
                    }

                    int westOfCurrent = currentRoom.GetConnection(Direction.WEST);
                    if (westOfCurrent != -1)
                    {
                        PreRoom nextRoom = m_PreRooms[westOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            if (nextRoom.GetRoomType == PreRoomType.TREASURE && currentRoom.GetRoomType == PreRoomType.TREASURE)
                            {
                                PreRoomType t = RandomRoomTypeExcluding(new HashSet<int>() { (int)PreRoomType.TREASURE, (int)PreRoomType.EXIT, (int)PreRoomType.NEXTLEVEL, (int)PreRoomType.BOSS });
                                nextRoom.ChangeRoomType(t);
                            }
                        }
                    }
                }
            }
        }

        void RemoveMultipleEntrances()
        {
            int numOfEntrances = 0;
            foreach (PreRoom n in m_PreRooms)
            {
                if (n.GetRoomType == PreRoomType.ENTRANCE)
                {
                    numOfEntrances++;
                }
            }

            foreach (PreRoom n in m_PreRooms)
            {
                if (numOfEntrances == 1)
                {
                    break;
                }

                if (n.GetRoomType == PreRoomType.ENTRANCE)
                {
                    numOfEntrances--;
                    PreRoomType randType = RandomRoomTypeExcluding(new HashSet<int> { (int)PreRoomType.TREASURE, (int)PreRoomType.EXIT, (int)PreRoomType.NEXTLEVEL, (int)PreRoomType.BOSS, (int)PreRoomType.ENTRANCE });
                    n.ChangeRoomType(randType);
                }
            }
        }

        //room traversal often creates dead ends
        //where rooms are adj but not connected
        void LinkAdjacentRooms()
        {
            for (int j = 0; j < m_DungeonHeight; ++j)
            {
                for (int i = 0; i < m_DungeonWidth; ++i)
                {
                    PreRoom currentRoom = m_PreRooms[i + j * m_DungeonHeight];
                    
                    //check if room exists in m_PreRooms
                    //check if a connection exists in currentRoom
                    if (currentRoom.GetRoomType != PreRoomType.NO_ROOM)
                    {
                        //checks look at the position of the current room in the dungeon eg
                        //can only connect north if not on the north boundary
                        //otherwise it adj room variables will be set to null
                        PreRoom northOfCurrent = i + j * m_DungeonHeight < m_DungeonWidth ? null : m_PreRooms[i + (j - 1) * m_DungeonHeight];
                        PreRoom southofCurrent = i + j * m_DungeonHeight >= m_DungeonWidth * m_DungeonHeight - m_DungeonWidth ? null : m_PreRooms[i + (j + 1) * m_DungeonHeight];
                        PreRoom eastOfCurrent = (i + j * m_DungeonHeight) % m_DungeonWidth == m_DungeonWidth - 1 ? null : m_PreRooms[i + 1 + j * m_DungeonHeight];
                        PreRoom westOfCurrent = (i + j * m_DungeonHeight) % m_DungeonWidth == 0 ? null : m_PreRooms[i - 1 + j * m_DungeonHeight];

                            //adj room is not null, 
                        if (northOfCurrent != null &&

                            //current room's connection to adj room exists,
                            currentRoom.GetConnection(Direction.NORTH) == -1 &&

                            //adj room is valid
                            northOfCurrent.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            //connect both ways

                            //current -> adj 
                            currentRoom.SetConnection(Direction.NORTH, northOfCurrent.ID);

                            //adj -> current
                            northOfCurrent.SetConnection(Direction.SOUTH, currentRoom.ID);
                        }

                        if (southofCurrent != null && 
                            currentRoom.GetConnection(Direction.SOUTH) == -1 && 
                            southofCurrent.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            currentRoom.SetConnection(Direction.SOUTH, southofCurrent.ID);
                            southofCurrent.SetConnection(Direction.NORTH, currentRoom.ID);
                        }

                        if (eastOfCurrent != null && 
                            currentRoom.GetConnection(Direction.EAST) == -1 
                            && eastOfCurrent.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            currentRoom.SetConnection(Direction.EAST, eastOfCurrent.ID);
                            eastOfCurrent.SetConnection(Direction.WEST, currentRoom.ID);
                        }

                        if (westOfCurrent != null && 
                            currentRoom.GetConnection(Direction.WEST) == -1 && 
                            westOfCurrent.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            currentRoom.SetConnection(Direction.WEST, westOfCurrent.ID);
                            westOfCurrent.SetConnection(Direction.EAST, currentRoom.ID);
                        }
                    }
                }
            }
        }

        //Find a random room that is not NO_ROOM
        PreRoom RandomFilledRoom()
        {
            int counter = 0;
            int maxCounter = 100;

            while (counter < maxCounter)
            {
                counter++;

                int randX = (int)(Utility.Instance.cRand.NextDouble() * m_DungeonWidth);
                int randY = (int)(Utility.Instance.cRand.NextDouble() * m_DungeonHeight);

                PreRoom t = m_PreRooms[(int)(randX + randY * m_DungeonHeight)];

                if (t.GetRoomType != PreRoomType.NO_ROOM)
                {
                    return t;
                }
            }

            return null;
        }

        //create rooms and add them to the list of prerooms
        public void CreatePreRooms()
        {
            for (int i = 0; i < m_DungeonWidth; ++i)
            {
                for (int j = 0; j < m_DungeonHeight; ++j)
                {
                    m_PreRooms.Add(new PreRoom(j, i));
                }
            }
        }

        //check doors of current room, if there is something
        //on the other side, then make that direction door
        //of the current room collidable - only checks doors
        //in the list of doors, nothing else
        void MakeDoorsCollidable()
        {
            foreach (PreRoom n in m_PreRooms)
            {
                //only check if its a valid room
                if (n.GetRoomType != PreRoomType.NO_ROOM)
                {
                    //get id of room north of current, used as index into pre rooms list
                    int northOfCurrent = n.GetConnection(Direction.NORTH);

                    //check that it is valid
                    if (northOfCurrent != -1)
                    {
                        //get next room
                        PreRoom nextRoom = m_PreRooms[northOfCurrent];

                        //check that there exists a room to go into
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            //set collidable bool to be true
                            n.MakeDoorCollidable(Direction.NORTH);
                        }
                    }

                    int southOfCurrent = n.GetConnection(Direction.SOUTH);
                    if (southOfCurrent != -1)
                    {
                        PreRoom nextRoom = m_PreRooms[southOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            n.MakeDoorCollidable(Direction.SOUTH);
                        }
                    }

                    int eastOfCurrent = n.GetConnection(Direction.EAST);
                    if (eastOfCurrent != -1)
                    {
                        PreRoom nextRoom = m_PreRooms[eastOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            n.MakeDoorCollidable(Direction.EAST);
                        }
                    }

                    int westOfCurrent = n.GetConnection(Direction.WEST);
                    if (westOfCurrent != -1)
                    {
                        PreRoom nextRoom = m_PreRooms[westOfCurrent];
                        if (nextRoom.GetRoomType != PreRoomType.NO_ROOM)
                        {
                            n.MakeDoorCollidable(Direction.WEST);
                        }
                    }
                }
            }
        }

        //parameter checks for resetting the player
        public void ResetDungeon(bool reset = true)
        {
            if (reset)
            {
                player.Reset();
                ProjectileManager.Instance.ClearBullets();
            }

            //keep going if starting room is null
            //and there aren't enough rooms
            int counter = 0;
            int maxCounter = 100;

            while (counter < maxCounter)
            {
                //reset num filled rooms.
                //gets filled in when ProcedurallyFilledDungeon is called
                numFilledRooms = 0;
                counter++;

                //clear list, start from beginning
                m_PreRooms.Clear();

                //create rooms, add them to list
                CreatePreRooms();

                //traversal rooms
                //random start, random direction per step
                ProcedurallyFilledDungeon();

                RemoveAdjacentTreasureRooms();
                RemoveMultipleEntrances();

                //create specific classes for each room type
                SpecialisePreRooms();

                //make sure adj rooms are connected properly
                LinkAdjacentRooms();

                //make sure player can move from one room to the next
                MakeDoorsCollidable();

                //set the current room to be the starting room
                m_CurrentRoom = StartRoom;

                if (BossRoom != null && StartRoom != null && EndRoom != null && numFilledRooms > m_MinRooms && numFilledRooms < m_MaxRooms)
                {
                    m_CurrentRoom.Explored = true;
                    ProjectileManager.Instance.ClearBullets();
                    Console.WriteLine(counter.ToString() + " tries for success.");
                    break;
                }
            }
        }

        //create new list, add specific rooms,
        //copy new list to current list
        void SpecialisePreRooms()
        {
            List<PreRoom> local = new List<PreRoom>();

            for (int i = 0; i < m_PreRooms.Count; ++i)
            {
                PreRoom current = m_PreRooms[i];
                switch (current.GetRoomType)
                {
                    case PreRoomType.BOSS:
                        //constructor takes in x, y and the connections of the room, so that they are maintained
                        BossRoom tempBossRoom = new BossRoom(current.X, current.Y, current.GetConnections);
                        tempBossRoom.Explored = current.Explored;
                        local.Add(tempBossRoom);
                        
                        break;
                    case PreRoomType.EMPTY:
                        EmptyRoom tempEmptyRoom = new EmptyRoom(current.X, current.Y, current.GetConnections);
                        tempEmptyRoom.Explored = current.Explored;
                        local.Add(tempEmptyRoom);

                        break;
                    case PreRoomType.ENTRANCE:
                        EntranceRoom tempEntrance = new EntranceRoom(current.X, current.Y, current.GetConnections);
                        tempEntrance.Explored = current.Explored;
                        local.Add(tempEntrance);

                        break;
                    case PreRoomType.EXIT:
                        ExitRoom tempExit = new ExitRoom(current.X, current.Y, current.GetConnections);
                        tempExit.Explored = current.Explored;
                        local.Add(tempExit);

                        break;
                    case PreRoomType.MONSTER:
                        MonsterRoom tempMonster = new MonsterRoom(current.X, current.Y, current.GetConnections);
                        tempMonster.Explored = current.Explored;
                        local.Add(tempMonster);

                        break;
                    case PreRoomType.NEXTLEVEL:
                        NextLevelRoom tempNext = new NextLevelRoom(current.X, current.Y, current.GetConnections);
                        tempNext.Explored = current.Explored;
                        local.Add(tempNext);

                        break;
                    case PreRoomType.RIDDLE:
                        RiddleRoom tempRiddle = new RiddleRoom(current.X, current.Y, current.GetConnections);
                        tempRiddle.Explored = current.Explored;
                        local.Add(tempRiddle);

                        break;
                    case PreRoomType.TRAP:
                        TrapRoom tempTrap = new TrapRoom(current.X, current.Y, current.GetConnections);
                        tempTrap.Explored = current.Explored;
                        local.Add(tempTrap);

                        break;
                    case PreRoomType.TREASURE:
                        TreasureRoom tempTreasure = new TreasureRoom(current.X, current.Y, current.GetConnections);
                        tempTreasure.Explored = current.Explored;
                        local.Add(tempTreasure);

                        break;
                    case PreRoomType.VISTA:
                        VistaRoom tempVista = new VistaRoom(current.X, current.Y, current.GetConnections);
                        tempVista.Explored = current.Explored;
                        local.Add(tempVista);
                        break;

                    case PreRoomType.NO_ROOM:
                        NoRoom tempNoRoom = new NoRoom(current.X, current.Y, current.GetConnections);
                        tempNoRoom.Explored = current.Explored;
                        local.Add(tempNoRoom);
                        break;
                }
            }
            m_PreRooms = local;
        }

        //room traversal
        //choose random point as start
        //if start room is fully connected, stop
        //choose random dir
        //if current room is fully connected, stop
        //switch on the direction
        //set connection both ways
        //assign next room to current room
        //loop
        void EstablishLinksBetweenPreRooms()
        {
            int startingX = (int)(Utility.Instance.cRand.NextDouble() * m_DungeonWidth);
            int startingY = (int)(Utility.Instance.cRand.NextDouble() * m_DungeonHeight);

            PreRoom startRoom = m_PreRooms[startingX + m_DungeonHeight * startingY];
            startRoom.ChangeRoomType(PreRoomType.ENTRANCE);

            PreRoom currentRoom = null;

            int firstPassCounter = 0;
            int maxFirstPassCounter = 100;

            int secondPassCounter = 0;
            int maxSecondPassCounter = 250;

            while (firstPassCounter < maxFirstPassCounter)
            {
                if (startRoom.IsFullyConnected)
                    return;

                currentRoom = startRoom;

                secondPassCounter = 0;

                firstPassCounter++;

                while (secondPassCounter < maxSecondPassCounter)
                {
                    if (currentRoom.IsFullyConnected)
                        return;

                    Direction randDir = currentRoom.GetRandomValidDirection();

                    if (randDir == Direction.NO_DIRECTION)
                        return;

                    PreRoom nextRoom = null;

                    switch (randDir)
                    {
                        case Direction.NORTH:
                            nextRoom = m_PreRooms[currentRoom.ID - m_DungeonHeight];

                            nextRoom.SetConnection(Direction.SOUTH, currentRoom.ID);
                            break;
                        case Direction.SOUTH:
                            nextRoom = m_PreRooms[currentRoom.ID + m_DungeonHeight];

                            nextRoom.SetConnection(Direction.NORTH, currentRoom.ID);
                            break;
                        case Direction.EAST:
                            nextRoom = m_PreRooms[currentRoom.ID + 1];

                            nextRoom.SetConnection(Direction.WEST, currentRoom.ID);
                            break;
                        case Direction.WEST:
                            nextRoom = m_PreRooms[currentRoom.ID - 1];

                            nextRoom.SetConnection(Direction.EAST, currentRoom.ID);
                            break;
                    }

                    currentRoom.SetConnection(randDir, nextRoom.ID);

                    currentRoom = nextRoom;

                    secondPassCounter++;
                }
            }
        }

        //loop through all rooms, as long as there are connections
        //and the current room is valid, there is a chance for the 
        //room type to be something else
        //room type chosen based on num of connections
        void DeterminePreRoomTypes()
        {
            foreach (PreRoom preroom in m_PreRooms)
            {
                if (preroom.NumConnections > 0 && preroom.GetRoomType == PreRoomType.NO_ROOM)
                {
                    numFilledRooms++;

                    if (preroom.GetRoomType == PreRoomType.ENTRANCE)
                    {
                        continue;
                    }

                    if (preroom.NumConnections == 1)
                    {
                        double n = Utility.Instance.cRand.NextDouble();

                        if (n < 0.1)
                            preroom.ChangeRoomType(PreRoomType.TREASURE);
                        else
                            preroom.ChangeRoomType(PreRoomType.EMPTY);
                    }
                    else if (preroom.NumConnections == 2)
                    {
                        double n = Utility.Instance.cRand.NextDouble();

                        if (n < 0.25)
                            preroom.ChangeRoomType(PreRoomType.VISTA);
                        else if (n >= 0.25 && n < 0.5)
                            preroom.ChangeRoomType(PreRoomType.TREASURE);
                        else
                            preroom.ChangeRoomType(PreRoomType.TRAP);
                    }
                    else if (preroom.NumConnections == 3)
                    {
                        double n = Utility.Instance.cRand.NextDouble();

                        if (n < 0.25)
                            preroom.ChangeRoomType(PreRoomType.TRAP);
                        else
                            preroom.ChangeRoomType(PreRoomType.EMPTY);
                    }
                    else if (preroom.NumConnections == 4)
                    {
                        double n = Utility.Instance.cRand.NextDouble();

                        if (n < 0.5)
                            preroom.ChangeRoomType(PreRoomType.MONSTER);
                        else if (n >= 0.5 && n < 0.75)
                            preroom.ChangeRoomType(PreRoomType.EMPTY);
                        else
                            preroom.ChangeRoomType(PreRoomType.TRAP);
                    }
                }
            }

            while (BossRoom == null)
            {
                PreRoom bossRoom = RandomFilledRoom();

                if (bossRoom != null)
                {
                    if (bossRoom.GetRoomType != PreRoomType.ENTRANCE)
                    {
                        bossRoom.ChangeRoomType(PreRoomType.BOSS);
                    }
                }
            }

            while (EndRoom == null)
            {
                PreRoom exitRoom = RandomFilledRoom();

                if (exitRoom != null)
                {
                    if (exitRoom.GetRoomType != PreRoomType.ENTRANCE || exitRoom.GetRoomType != PreRoomType.BOSS)
                    {
                        double roomTypeChance = Utility.Instance.cRand.NextDouble();
                        if (roomTypeChance > 0.5)
                        {
                            exitRoom.ChangeRoomType(PreRoomType.EXIT);
                        }
                        else
                        {
                            exitRoom.ChangeRoomType(PreRoomType.NEXTLEVEL);
                        }
                    }
                }
            }
        }

        //set up links between rooms
        //determine what type of rooms are present
        void ProcedurallyFilledDungeon()
        {
            EstablishLinksBetweenPreRooms();
            DeterminePreRoomTypes();
        }

        //using LINQ, exclude specific types from enum, then generate random room type
        PreRoomType RandomRoomTypeExcluding(HashSet<int> excluded)
        {
            //make range of numbers from 0 to max types in preroom types, not including the ones to be excluded
            var range = Enumerable.Range(0, (int)PreRoomType.MAX_TYPES).Where(i => !excluded.Contains(i));
            int randIndex = (int)(Utility.Instance.cRand.NextDouble() * ((int)PreRoomType.MAX_TYPES - excluded.Count));
            return (PreRoomType)(range.ElementAt(randIndex));
        }

        //draw each room of dungeon, if its not explored and GetRoomType == NO_ROOM
        //then it will be filled in as black, otherwise it is an explored and valid room
        //displayed in white
        //also draw marker for where the player currently is
        public void DrawMiniMap(PreRoomType typeToHighlight = PreRoomType.NO_ROOM)
        {
            foreach (PreRoom n in m_PreRooms)
            {
                if (typeToHighlight != PreRoomType.NO_ROOM)
                {
                    if (n.GetRoomType == typeToHighlight)
                    {
                        Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)m_StartPos.X + n.X * m_RoomWidth, (int)m_StartPos.Y + n.Y * m_RoomHeight, m_RoomWidth, m_RoomHeight), Color.Red);
                        continue;
                    }
                }

                //specialised room
                if (n.GetRoomType != PreRoomType.NO_ROOM)
                {
                    //explored room
                    if (n.Explored)
                    {
                        Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)m_StartPos.X + n.X * m_RoomWidth, (int)m_StartPos.Y + n.Y * m_RoomHeight, m_RoomWidth, m_RoomHeight), Color.White);
                    }
                    //not explored room
                    else
                    {
                        Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)m_StartPos.X + n.X * m_RoomWidth, (int)m_StartPos.Y + n.Y * m_RoomHeight, m_RoomWidth, m_RoomHeight), Color.Black);
                    }
                }
                //non-specialised room
                else
                {
                    Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, new Rectangle((int)m_StartPos.X + n.X * m_RoomWidth, (int)m_StartPos.Y + n.Y * m_RoomHeight, m_RoomWidth, m_RoomHeight), Color.Black);
                }
            }

            playerMarker.ChangePosition(new Vector2(m_StartPos.X + m_CurrentRoom.X * (m_RoomWidth) - 0.5f, m_StartPos.Y + m_CurrentRoom.Y * (m_RoomHeight)));
            playerMarker.Draw();
        }

        //only draw doors if there is a valid connection 
        //going from current room to the next room
        //next connection valid
        //next room type is valid
        public void DrawDoors()
        {
            //check validity of connection
            if (m_CurrentRoom.GetConnection(Direction.NORTH) != -1)
            {
                //check validity of room in list
                int northOfCurrent = m_PreRooms[m_CurrentRoom.GetConnection(Direction.NORTH)].ID;

                //check validity of room type
                if (m_PreRooms[northOfCurrent].GetRoomType != PreRoomType.NO_ROOM)
                {
                    Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, m_CurrentRoom.GetDoorDrawn(Direction.NORTH), Color.RoyalBlue);
                }
            }

            if (m_CurrentRoom.GetConnection(Direction.SOUTH) != -1)
            {
                int southOfCurrent = m_PreRooms[m_CurrentRoom.GetConnection(Direction.SOUTH)].ID;
                if (m_PreRooms[southOfCurrent].GetRoomType != PreRoomType.NO_ROOM)
                {
                    Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, m_CurrentRoom.GetDoorDrawn(Direction.SOUTH), Color.RoyalBlue);
                }
            }

            if (m_CurrentRoom.GetConnection(Direction.EAST) != -1)
            {
                int eastOfCurrent = m_PreRooms[m_CurrentRoom.GetConnection(Direction.EAST)].ID;
                if (m_PreRooms[eastOfCurrent].GetRoomType != PreRoomType.NO_ROOM)
                {
                    Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, m_CurrentRoom.GetDoorDrawn(Direction.EAST), Color.RoyalBlue);
                }
            }

            if (m_CurrentRoom.GetConnection(Direction.WEST) != -1)
            {
                int westOfCurrent = m_PreRooms[m_CurrentRoom.GetConnection(Direction.WEST)].ID;
                if (m_PreRooms[westOfCurrent].GetRoomType != PreRoomType.NO_ROOM)
                {
                    Utility.Instance.SB.Draw(Utility.Instance.WhiteSquare, m_CurrentRoom.GetDoorDrawn(Direction.WEST), Color.RoyalBlue);
                }
            }
        }

        //set explored to true
        void UncoverRooms()
        {
            foreach (PreRoom n in m_PreRooms)
            {
                if (n.GetRoomType != PreRoomType.NO_ROOM)
                {
                    n.Explored = true;
                }
            }
        }

        //set explored to false
        void CoverRooms()
        {
            foreach (PreRoom n in m_PreRooms)
            {
                if (n == m_CurrentRoom)
                {
                    continue;
                }

                if (n.GetRoomType != PreRoomType.NO_ROOM)
                {
                    n.Explored = false;
                }
            }
        }

        public int RoomWidth
        {
            get { return m_RoomWidth; }
        }

        public int RoomHeight
        {
            get { return m_RoomHeight; }
        }

        public static int DungeonWidth
        {
            get { return m_DungeonWidth; }
        }

        public static int DungeonHeight
        {
            get { return m_DungeonHeight; }
        }

        public List<PreRoom> PreRooms
        {
            get { return m_PreRooms; }
        }

        //loop through and find room with GetRoomType == ENTRANCE
        public PreRoom StartRoom
        {
            get
            {
                foreach (PreRoom n in m_PreRooms)
                {
                    if (n.GetRoomType == PreRoomType.ENTRANCE)
                    {
                        Console.WriteLine("Found start room at: " + n.X.ToString() + " " + n.Y.ToString() + '\n');
                        return n;
                    }
                }
                return null;
            }
        }

        //loop through and find room with GetRoomType == NEXTLEVEL or EXIT
        public PreRoom EndRoom
        {
            get
            {
                foreach (PreRoom n in m_PreRooms)
                {
                    if (n.GetRoomType == PreRoomType.EXIT || n.GetRoomType == PreRoomType.NEXTLEVEL)
                    {
                        Console.WriteLine("Found end room at: " + n.X.ToString() + " " + n.Y.ToString() + '\n');
                        return n;
                    }
                }
                return null;
            }
        }

        //loop through and find room with GetRoomType == BOSS
        public PreRoom BossRoom
        {
            get
            {
                foreach (PreRoom n in m_PreRooms)
                {
                    if (n.GetRoomType == PreRoomType.BOSS)
                    {
                        Console.WriteLine("Found boss room at: " + n.X.ToString() + " " + n.Y.ToString() + '\n');
                        return n;
                    }
                }
                return null;
            }
        }

        public static Player GetPlayer
        {
            get { return player; }
        }

        public static void GenerateNewDungeon()
        {
            generateNextLevel = true;
        }
    }
}

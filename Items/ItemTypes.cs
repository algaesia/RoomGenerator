using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    //implementation for each type of item
    //implements the inherited use function from item parent class
    //each class simply implements the use function, 
    //depending on what the item type is

    public class PerseusHelm : Item
    {
        public PerseusHelm(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.PERSEUSHELM)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseAttackSpeed();
            base.Use();
        }
    }

    public class TarnHelm : Item
    {
        public TarnHelm(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.TARNHELM)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.DecreaseMovementSpeed();
            Dungeon.GetPlayer.IncreaseMaxHealth();
            base.Use();
        }
    }

    public class Svrivatsa : Item
    {
        public Svrivatsa(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.SVRIVATSA)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseAttackSpeed();
            Dungeon.GetPlayer.DecreaseMaxHealth();
            base.Use();
        }
    }

    public class Priwen : Item
    {
        public Priwen(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.PRIWEN)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseMovementSpeed();
            Dungeon.GetPlayer.IncreaseAttackSpeed();

            base.Use();
        }
    }

    public class HarvesterOfSouls : Item
    {
        public HarvesterOfSouls(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.HARVESTEROFSOULS)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseBulletSize();
            Dungeon.GetPlayer.DecreaseAttackSpeed();
            base.Use();
        }
    }

    public class Chentu : Item
    {
        public Chentu(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.CHENTU)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseAttackDamage();
            base.Use();
        }
    }

    public class HeavensWill : Item
    {
        public HeavensWill(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.HEAVENSWILL)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseAttackDamage();
            Dungeon.GetPlayer.IncreaseAttackDamage();
            Dungeon.GetPlayer.DecreaseBulletSize();
            Dungeon.GetPlayer.DecreaseBulletSize();
            base.Use();
        }
    }

    public class Caledfwlch : Item
    {
        public Caledfwlch(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.CALEDFWLCH)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseMovementSpeed();
            Dungeon.GetPlayer.IncreaseMovementSpeed();
            Dungeon.GetPlayer.DecreaseBulletSize();
            Dungeon.GetPlayer.DecreaseMovementSpeed();
            Dungeon.GetPlayer.DecreaseMaxHealth();
            base.Use();
        }
    }

    public class Arondight : Item
    {
        public Arondight(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.ARONDIGHT)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseAttackSpeed();
            Dungeon.GetPlayer.IncreaseAttackDamage();
            Dungeon.GetPlayer.DecreaseMaxHealth();

            base.Use();
        }
    }

    public class Mysteltainn : Item
    {
        public Mysteltainn(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.MYSTELTAINN)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseMovementSpeed();
            Dungeon.GetPlayer.IncreaseAttackSpeed();
            Dungeon.GetPlayer.DecreaseMaxHealth();

            base.Use();
        }
    }

    public class Gungnir : Item
    {
        public Gungnir(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.GUNGNIR)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseBulletSize();
            Dungeon.GetPlayer.IncreaseBulletSize();
            Dungeon.GetPlayer.DecreaseMovementSpeed();

            base.Use();
        }
    }

    public class SpearOfLonginus : Item
    {
        public SpearOfLonginus(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.SPEAROFLONGINUS)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseBulletSize();
            Dungeon.GetPlayer.IncreaseBulletSize();
            Dungeon.GetPlayer.DecreaseAttackSpeed();

            base.Use();
        }
    }

    public class Mjolnir : Item
    {
        public Mjolnir(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.MJOLNIR)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseBulletSize();
            Dungeon.GetPlayer.IncreaseAttackDamage();
            Dungeon.GetPlayer.DecreaseMovementSpeed();
            Dungeon.GetPlayer.DecreaseMovementSpeed();

            base.Use();
        }
    }

    public class Vajra : Item
    {
        public Vajra(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.VAJRA)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseAttackDamage();

            base.Use();
        }
    }

    public class Megingjord : Item
    {
        public Megingjord(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.MEGINGJORD)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseAttackDamage();
            Dungeon.GetPlayer.IncreaseAttackSpeed();
            Dungeon.GetPlayer.DecreaseMovementSpeed();
            Dungeon.GetPlayer.DecreaseMovementSpeed();
            base.Use();
        }
    }

    public class LotusTree : Item
    {
        public LotusTree(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.LOTUSTREE)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseMaxHealth();
            Dungeon.GetPlayer.IncreaseMaxHealth();

            base.Use();
        }
    }

    public class Orichalcum : Item
    {
        public Orichalcum(Vector2 a_Position, Texture2D a_Texture)
            : base(a_Position, a_Texture, ItemType.ORICHALCUM)
        {
            m_BaseColour *= 0.5f;
            m_ActualColour = m_BaseColour;
            m_ActualColour.A = 1;
        }

        public override void Use()
        {
            Dungeon.GetPlayer.IncreaseMaxHealth();
            Dungeon.GetPlayer.DecreaseBulletSize();

            base.Use();
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomGenerator
{
    public enum BehaviourType
    {
        SEEK,
        FLEE,
        WANDER,
        PURSUE,
        EVADE,
        ARRIVAL,
    };

    //shell class for a type of steering behaviour
    public abstract class SteeringBehaviour
    {
        protected Enemy m_Owner;
        protected BehaviourType type;

        public abstract Vector2 GetForce();
    }

    //calculates evade, based on player's position and velocity
    public class Evade : SteeringBehaviour
    {
        public Evade(Enemy owner)
        {
            type = BehaviourType.EVADE;
            m_Owner = owner;
        }

        public override Vector2 GetForce()
        {
            if (!Dungeon.GetPlayer.Alive)
            {
                return Vector2.Zero;
            }

            m_Owner.FaceTarget(Dungeon.GetPlayer.Position + Dungeon.GetPlayer.Velocity);

            Vector2 force = Vector2.Normalize((Dungeon.GetPlayer.Position + Dungeon.GetPlayer.Velocity) - m_Owner.Position);
            return (force - m_Owner.Velocity);
        }
    }

    //calculates pursue based on player's position and velocity - effectively opposite of evade
    public class Pursue : SteeringBehaviour
    {
        private Vector2 target = Vector2.Zero;

        public Pursue(Enemy owner)
        {
            type = BehaviourType.PURSUE;
            m_Owner = owner;
        }

        public override Vector2 GetForce()
        {
            if (target.Length() == 0)
            {
                return Vector2.Zero;
            }

            m_Owner.FaceTarget(target);

            Vector2 force = Vector2.Normalize(target - m_Owner.Position);
            return (force - m_Owner.Velocity);
        }

        public Vector2 Target
        {
            get
            {
                return target;
            }
            set
            {
                target.X = value.X;
                target.Y = value.Y;
            }
        }

        public void ResetTarget()
        {
            target.X = 0;
            target.Y = 0;
        }
    }

    public class Wander : SteeringBehaviour
    {
        public Wander(Enemy Owner)
        {
            m_Owner = Owner;
            type = BehaviourType.WANDER;
        }

        public override Vector2 GetForce()
        {
            return Vector2.Zero;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using Microsoft.Xna.Framework;

namespace PlagueEngine.ArtificialIntelligence
{
    /*Te argsy lecą, jak najbliższy wróg wejdzie w zakres widzenia postaci*/
    class EnemyNoticed : EventArgs
    {
        public AbstractLivingBeing ClosestNoticedEnemy;
        public EnemyNoticed(AbstractLivingBeing ClosestNoticedEnemy)
        {
            this.ClosestNoticedEnemy = ClosestNoticedEnemy;
        }
        public override string ToString()
        {
            return "Noticed enemy " + ClosestNoticedEnemy.ToString();
        }
    }

    /* Event rozkazu ataku */
    class AttackOrderEvent : EventArgs
    {
        public AbstractLivingBeing EnemyToAttack;
        public AttackOrderEvent(AbstractLivingBeing EnemyToAttack)
        {
            this.EnemyToAttack = EnemyToAttack;
        }
        public override string ToString()
        {
            return "ATTACKING ENEMY: " + EnemyToAttack.ToString();
        }
    }

    /* Te argsy lecą, jak wróg zostanie ubity */
    class EnemyKilled : EventArgs
    {
        public AbstractLivingBeing DeadEnemy;
        public EnemyKilled(AbstractLivingBeing DeadEnemy)
        {
            this.DeadEnemy = DeadEnemy;
        }
        public override string ToString()
        {
            return "ENEMY: " + DeadEnemy.ToString() + " KILLED!";
        }
    }

    /* Te argsy lecą, jak wróg wyjdzie poza zasięg widzenia */
    class EnemyOutOfSight : EventArgs
    {

    }

    class SoundAt : EventArgs
    {
        public Vector3 position;

        public SoundAt(Vector3 position)
        {
            this.position = position;
        }

        public override string ToString()
        {
            return "Sound at " + position.ToString();
        }
    }

    class TakeDamage : EventArgs
    {
        public double amount;
        public AbstractLivingBeing attacker;

        public TakeDamage(double amount, AbstractLivingBeing attacker)
        {
            this.amount = amount;
            this.attacker = attacker;
        }

        public override string ToString()
        {
            return "HP: " + amount + " attacker: " + attacker == null ? "NULL" : attacker.ToString();
        }
    }

}


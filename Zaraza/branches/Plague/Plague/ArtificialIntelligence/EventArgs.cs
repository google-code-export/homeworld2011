using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.LowLevelGameFlow.GameObjects;

namespace PlagueEngine.ArtificialIntelligence
{
    /*Te argsy lecą, jak najbliższy wróg wejdzie w zakres widzenia postaci*/
    class EnemyNoticed : EventArgs
    {
        public LivingBeing ClosestNoticedEnemy;
        public EnemyNoticed(LivingBeing ClosestNoticedEnemy)
        {
            this.ClosestNoticedEnemy = ClosestNoticedEnemy;
        }
        public override string ToString()
        {
            return "Noticed enemy " + ClosestNoticedEnemy.ToString();
        }
    }

    /*DEPRECATED Te argsy lecą, jak najbliższy wróg wejdzie w zasięg strzału.*/
    class EnemyInRange : EventArgs
    {

    }

    /*DEPRECATED Te argsy lecą jak najbliższy wróg będzie poza zasięgiem.*/
    class EnemyOutOfRange : EventArgs
    {

    }

    /* Te argsy lecą, jak wróg zostanie ubity */
    class EnemyKilled : EventArgs
    {
        public LivingBeing DeadEnemy;
        public EnemyKilled(LivingBeing DeadEnemy)
        {
            this.DeadEnemy = DeadEnemy;
        }
        public override string ToString()
        {
            return "ENEMY: "+ DeadEnemy.ToString() + " KILLED!";
        }
    }

    /* Te argsy lecą, jak wróg wyjdzie poza zasięg widzenia */
    class EnemyOutOfSight : EventArgs
    {

    }

    class TakeDamage : EventArgs
    {
        public double amount;
        public LivingBeing attacker;

        public TakeDamage(double amount, LivingBeing attacker)
        {
            this.amount   = amount;
            this.attacker = attacker;
        }

        public override string ToString()
        {
            return "HP: " + amount + " attacker: " + attacker==null?"NULL":attacker.ToString();
        }
    }

}


using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.Physics;
using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.EventsSystem;
using PlagueEngine.LowLevelGameFlow.GameObjects;
using PlagueEngine.ArtificialIntelligence.Controllers;


namespace PlagueEngine.ArtificialIntelligence.Controllers
{
    enum MobType { ZOMBIE, MUTATED };

    class MobController : AbstractAIController
    {
        //TODO: change temporary constructor
        bool CanBleed;

        public MobType Type = MobType.ZOMBIE;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <param name="rotationSpeed"></param>
        /// <param name="movingSpeed"></param>
        /// <param name="distance"></param>
        /// <param name="angle"></param>
        /// <param name="MaxHP"></param>
        /// <param name="HP"></param>
        /// <param name="AnimationBinding"></param>
        public MobController(AbstractLivingBeing person, float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle,
                         uint MaxHP,
                         uint HP,
                         Dictionary<Action, string>   AnimationMapping )
            : base(person, MaxHP, HP, rotationSpeed, movingSpeed, distance, angle, AnimationMapping)
        {
            ai.registerController(this);
            //niech defaultowo nie krwawią, najwyżej zmieni się na ostatnią chwilę i już.
            CanBleed = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        override public void OnEvent(EventsSystem.EventsSender sender, System.EventArgs e)
        {
            if (e.GetType().Equals(typeof(TakeDamage)))
            {
#if DEBUG
                Diagnostics.PushLog(LoggingLevel.INFO, "Mob Hit, calculating dmg");
#endif
                #region Take Damage
                TakeDamage evt = e as TakeDamage;

                if (evt.causesBleeding && CanBleed)
                {
                    IsBleeding = true;
                }

                if (HP <= evt.amount)
                {
                    EnemyKilled args = new EnemyKilled(controlledObject);
                    SendEvent(args, Priority.Normal, AbstractAIController.ai);
                    ai.Kill(controlledObject);
                }
                else
                {
                    HP -= (uint)evt.amount;
                    if (evt.attacker != null && AttackTarget == null)
                    {
                        AttackTarget = evt.attacker;
                        Action = PlagueEngine.ArtificialIntelligence.Controllers.Action.ENGAGE;
                    }
                }

                #endregion
            }
            else
            {
                base.OnEvent(sender, e);
            }
        }


        public override void bleed()
        {
            if (CanBleed)
            {
                base.bleed();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime">Różnica czasu między Update'ami</param>
        public override void Update(System.TimeSpan deltaTime)
        {
            switch (Action)
            {
                case Action.PLAY_DEAD:
                    return;
                default:
                    base.Update(deltaTime);
                    return;
            }
        }
    }
}

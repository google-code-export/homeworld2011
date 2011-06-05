
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
    class MobController : AbstractAIController
    {
        private GameObjectInstance currentObject;
        //TODO: change temporary constructor
        public MobController(AbstractLivingBeing person, float rotationSpeed,
                         float movingSpeed,
                         float distance,
                         float angle,
                         uint MaxHP,
                         uint HP)
            : base(person, MaxHP, HP)
        {
            RotationSpeed   = rotationSpeed;
            MovingSpeed     = movingSpeed;
            Distance        = distance;
            AnglePrecision  = angle;
            animationBinding = new Dictionary<Action, string>();
            animationBinding.Add(Action.IDLE, "Idle");
            animationBinding.Add(Action.MOVE, "Run");
            animationBinding.Add(Action.ENGAGE, "Run");
            animationBinding.Add(Action.ATTACK, "Attack03");
            ai.registerController(this);
        }



        override public void OnEvent(EventsSystem.EventsSender sender, System.EventArgs e)
        {
           base.OnEvent(sender, e);
        }



        public override void Update(System.TimeSpan deltaTime)
        {
            switch (action)
            {
                default:
                    base.Update(deltaTime);
                    return;
            }
        }
    }
}

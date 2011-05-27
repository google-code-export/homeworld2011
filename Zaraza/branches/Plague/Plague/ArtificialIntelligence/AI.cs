using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.EventsSystem;
using PlagueEngine.ArtificialIntelligence.Controllers;

namespace PlagueEngine.ArtificialIntelligence
{
    class AI : IEventsReceiver
    {
        private AIFactory controllersFactory;

        List<MercenaryController> GoodGuys;
        List<Controller>          BadGuys;

        public AI()
        {
            this.GoodGuys = new List<MercenaryController>();
            this.BadGuys  = new List<Controller>();
        }

        public void Initialize()
        {
        }
        public  bool IsDisposed()
        {
            return true;
        }
        public  void OnEvent(EventsSender sender, EventArgs e)
        {
        }
        public  void Dispose()
        {
        }

        internal void addController(Controllers.Controller controller)
        {
            if (controller.GetType() == typeof(MercenaryController))
            {
                GoodGuys.Add(controller as MercenaryController);
            }
            else
            {
                BadGuys.Add(controller);
            }
        }
    }
}

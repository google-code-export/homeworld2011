using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.EventsSystem;

namespace PlagueEngine.ArtificialIntelligence
{
    class AI : IEventsReceiver
    {
        private AIFactory controllersFactory;

        public AI()
        {
            controllersFactory = new AIFactory(this);
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
    }
}

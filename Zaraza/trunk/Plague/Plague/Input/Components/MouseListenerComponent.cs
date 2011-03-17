using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

using PlagueEngine.LowLevelGameFlow;
using PlagueEngine.Input;

namespace PlagueEngine.Input.Components
{
    enum MouseKeyAction
    {
        LeftClick,RightClick,MiddleClick
    }

    enum MouseMoveAction
    {
        Move,Scroll
    }

    class MouseListenerComponent : GameObjectComponent
    {
        
        private Input input = null;
        private bool active = true;

        public MouseListenerComponent(GameObjectInstance gameObject,Input input,bool active):base(gameObject)
        {
            this.input=input;
            this.active=active;
        }

        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
            }
        }

        public void subscribeKeys(onMouseKey oNmouseKey,params MouseKeyAction[] mouseKeyAction)
        {
            foreach (MouseKeyAction mouseKey in mouseKeyAction)
            {
                input.SubscribeMouseKeyListener(this, mouseKey, oNmouseKey);
            }
        }

        public void subscribeMouseMove(onMouseMove onMouseMove, params MouseMoveAction[] mouseMoveAction)
        {
            foreach(MouseMoveAction mouseAction in mouseMoveAction)
            {
                input.SubscribeMouseMoveListener(this,mouseAction,onMouseMove);
            }
        }

       
        public override void ReleaseMe()
        {
            input.ReleaseMouseListenerComponent(this);
        }

    }
}

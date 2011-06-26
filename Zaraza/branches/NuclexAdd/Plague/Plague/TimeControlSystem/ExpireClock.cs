using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.TimeControlSystem
{
    class ExpireClock
    {
        private TimeSpan _expireTime;
        private DateTime _expireDate;
        public ExpireClock(TimeSpan time)
        {
            _expireTime = time;
            _expireDate = DateTime.Now;
        }
        public bool isExpired(){
            if(_expireDate<=DateTime.Now){
                _expireDate = DateTime.Now + _expireTime;
                return true;
            }
            return false;
        }
        static public  ExpireClock FromSeconds(double time)
        {
            return new ExpireClock(TimeSpan.FromSeconds(time));
        } 
        static public ExpireClock FromMinutes(double time)
        {
            return new ExpireClock(TimeSpan.FromMinutes(time));
        } 
    }
}

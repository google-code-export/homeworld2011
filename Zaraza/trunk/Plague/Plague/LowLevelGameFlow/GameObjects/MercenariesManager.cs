using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlagueEngine.EventsSystem;


/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// MercenariesManager
    /********************************************************************************/
    class MercenariesManager : GameObjectInstance
    {

        /****************************************************************************/
        /// Fields        
        /****************************************************************************/
        private List<Mercenary> SelectedMercenaries = null;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(List<Mercenary> mercenaries)
        {
            SelectedMercenaries = mercenaries;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// ReleaseComponents
        /****************************************************************************/
        public override void ReleaseComponents()
        {            
        }
        /****************************************************************************/


        /****************************************************************************/
        /// On Event
        /****************************************************************************/
        public override void OnEvent(EventsSender sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(SelectedObjectEvent)))
            {
                Mercenary m = ((SelectedObjectEvent)e).gameObject as Mercenary;

                if (m != null)
                {
                    foreach (Mercenary merc in SelectedMercenaries) merc.Marker.Enabled = false;
                    SelectedMercenaries.Clear();
                    
                    SelectedMercenaries.Add(m);
                    m.Marker.Enabled = true;
                }
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// MercenariesManager Data
    /********************************************************************************/
    [Serializable]
    public class MercenariesManagerData : GameObjectInstanceData
    {
        public List<uint> SelectedMercenaries { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/
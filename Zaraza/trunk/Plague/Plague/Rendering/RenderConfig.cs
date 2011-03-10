using System;
using System.Collections.Generic;
using System.Text;


/************************************************************************************/
/// PlagueEngine.Rendering
/************************************************************************************/
namespace PlagueEngine.Rendering
{

    /********************************************************************************/
    /// RenderConfig
    /// <summary>
    /// Klasa zawierajaca podstawowe ustawienia urządzenia renderującego. 
    /// Przeznaczona do serializacji.
    /// </summary>
    /********************************************************************************/
    public class RenderConfig
    {   

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public int  Width;        
        public int  Height;       
        public bool FullScreen;   
        public bool Multisampling;
        public bool VSync;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor
        /****************************************************************************/
        public RenderConfig(int width,int height,bool fullScreen,bool multisampling,bool vSync) 
        {
            Width           = width;
            Height          = height;
            FullScreen      = fullScreen;
            Multisampling   = multisampling;
            VSync           = vSync;
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/
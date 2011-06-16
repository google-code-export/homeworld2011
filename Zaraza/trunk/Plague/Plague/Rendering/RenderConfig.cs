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
        public float Brightness;
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (1)
        /****************************************************************************/
        public RenderConfig()
        {
            Width           = 0;
            Height          = 0;
            FullScreen      = false;
            Multisampling   = false;
            VSync           = false;
            Brightness = 1.0f;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (2)
        /****************************************************************************/
        public RenderConfig(int width,int height,bool fullScreen,bool multisampling,bool vSync,float brightness) 
        {
            Width           = width;
            Height          = height;
            FullScreen      = fullScreen;
            Multisampling   = multisampling;
            VSync           = vSync;
            Brightness = brightness;
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/
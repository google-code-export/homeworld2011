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
        public float Contrast;
        public bool SSAO;
        public float BloomIntensity;
        public float BaseIntensity;
        public float BloomSaturation;
        public float BaseSaturation;
        public float BloomThreshold;
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
            Brightness      = 0.0f;
            Contrast        = 1.0f;
            SSAO            = false;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Constructor (2)
        /****************************************************************************/
        public RenderConfig(int width, int height, bool fullScreen, bool multisampling, bool vSync, float brightness, float contrast,bool ssao,
                                     float bloomIntensity,
         float baseIntensity,
         float bloomSaturation,
         float baseSaturation,
         float bloomThreshold) 
        {
            Width           = width;
            Height          = height;
            FullScreen      = fullScreen;
            Multisampling   = multisampling;
            VSync           = vSync;
            Brightness      = brightness;
            Contrast        = contrast;
            SSAO            = ssao;

            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
            BloomThreshold = bloomThreshold;
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/
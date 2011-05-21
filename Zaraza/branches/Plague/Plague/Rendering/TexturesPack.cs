using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

/********************************************************************************/
/// PlagueEngine.Rendering
/********************************************************************************/
namespace PlagueEngine.Rendering
{

    /****************************************************************************/
    /// TexturesPack
    /****************************************************************************/
    class TexturesPack
    {

        /************************************************************************/
        /// Fields
        /************************************************************************/
        private Texture2D diffuse    = null;
        private Texture2D specular   = null;
        private Texture2D normals    = null;
        /************************************************************************/


        /************************************************************************/
        /// Constructor
        /************************************************************************/
        public TexturesPack(Texture2D diffuse, Texture2D specular, Texture2D normals)
        {
            this.diffuse  = diffuse;
            this.specular = specular;
            this.normals  = normals;           
        }
        /************************************************************************/


        /************************************************************************/
        /// Equals
        /************************************************************************/
        public bool Equals(String[] textures)
        {
            if (diffuse == null)
            {
                if (!String.IsNullOrEmpty(textures[0])) return false;
            }
            else if (!diffuse.Name.Equals(textures[0])) return false;

            if (specular == null)
            {
               if(!String.IsNullOrEmpty(textures[1])) return false;
            }
            else if(!specular.Name.Equals(textures[1])) return false;

            if (normals == null)
            {
                if (!String.IsNullOrEmpty(textures[2])) return false;
            }
            else if (!normals.Name.Equals(textures[2])) return false;
            
            return true;
        }
        /************************************************************************/


        /************************************************************************/
        /// Properties
        /************************************************************************/
        public Texture2D Diffuse  { get { return diffuse ; } }
        public Texture2D Specular { get { return specular; } }
        public Texture2D Normals  { get { return normals ; } }
        /************************************************************************/
        
    }
    /****************************************************************************/

}
/********************************************************************************/
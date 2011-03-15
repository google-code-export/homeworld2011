using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow
{

    /********************************************************************************/
    /// Game Object Definition
    /********************************************************************************/
    public class GameObjectDefinition : IXmlSerializable
    {

        /****************************************************************************/
        /// Fields
        /****************************************************************************/
        public String Name                           = String.Empty;
        public Dictionary<String, object> Properties = new Dictionary<String, object>();
        /****************************************************************************/


        /****************************************************************************/
        /// Get Schema()
        /****************************************************************************/
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Read Xml
        /****************************************************************************/
        public void ReadXml(System.Xml.XmlReader reader)
        {
            Properties.Clear();

            Name = reader.ReadElementString();
            
            int count = int.Parse(reader.GetAttribute("Count"));
            reader.ReadStartElement();
            
            object value;
            Type type;
            String key;
            for (int i = 0; i < count; i++)
            {
                key     = reader.LocalName;
                type    = Type.GetType(reader.GetAttribute("Type"));
                value   = Convert.ChangeType(reader.ReadElementString(), type);
                Properties.Add(key, value);
            }
            reader.ReadEndElement();
            reader.ReadEndElement();
        }
        /****************************************************************************/


        /****************************************************************************/
        /// WriteXml
        /****************************************************************************/
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Name", Name);
            writer.WriteStartElement("Properties");
            writer.WriteAttributeString("Count", Properties.Keys.Count.ToString());
            foreach (String key in Properties.Keys)
            {
                writer.WriteStartElement(key);
                writer.WriteAttributeString("Type", Properties[key].GetType().ToString());
                writer.WriteValue(Properties[key]);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        /****************************************************************************/

    }
    /********************************************************************************/

}
/************************************************************************************/
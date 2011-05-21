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
        public String GameObjectClass                = string.Empty;
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
            GameObjectClass = reader.ReadElementString();

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
            writer.WriteElementString("GameObjectClass", GameObjectClass);
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


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public GameObjectDefinitionData GetData()
        {
            GameObjectDefinitionData data = new GameObjectDefinitionData();

            data.Name = Name;
            data.GameObjectClass = GameObjectClass;

            StringObjectPair sopair;
            foreach (KeyValuePair<String, Object> pair in Properties)
            {
                sopair = new StringObjectPair();
                sopair.String = pair.Key;
                sopair.Object = pair.Value;
                data.Properties.Add(sopair);
            }

            return data;
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Set
        /****************************************************************************/
        public void Set(GameObjectDefinitionData data)
        {
            Name = data.Name;
            GameObjectClass = data.GameObjectClass;

            Properties.Clear();

            foreach (StringObjectPair pair in data.Properties)
            {
                Properties.Add(pair.String, pair.Object);
            }
        }
        /****************************************************************************/

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Game Object Definition
    /********************************************************************************/
    [Serializable]
    public class GameObjectDefinitionData
    {
        public String                 Name            = String.Empty;
        public String                 GameObjectClass = String.Empty;
        public List<StringObjectPair> Properties      = new List<StringObjectPair>();
    }
    /********************************************************************************/


    /********************************************************************************/
    /// StringObjectPair
    /********************************************************************************/
    [Serializable]
    public class StringObjectPair
    {
        public String String = String.Empty;
        public Object Object = null;
    }
    /********************************************************************************/

}
/************************************************************************************/
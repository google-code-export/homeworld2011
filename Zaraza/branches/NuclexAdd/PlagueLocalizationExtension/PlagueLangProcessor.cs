using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = System.Xml.XmlDocument;
using TOutput = PlagueLocalizationExtension.LangFile;
using System.Xml;
	

namespace PlagueLocalizationExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "PlagueLangProcessor")]
    public class PlagueLangProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            TOutput output = new LangFile();
 
            //We traverse the XML file as usual and store certain elements into the
            //output object;
            if(input.GetElementsByTagName("XNABabylonStringFile").Count > 0)
            {
                foreach (XmlNode node in input.GetElementsByTagName("XNABabylonStringFile")[0].ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "version":
                            Double.TryParse(node.InnerText, out output.version);
                            break;
                        case "revisionDate":
                            DateTime.TryParse(node.InnerText, out output.revision);
                            break;
                        case "author":
                            output.author = node.InnerText;
                            break;
                        case "language":
                            output.language = node.InnerText;
                            break;
 
                        case "package":
                            //Loop trough all the keys and store the key id and translated value into a dictionary
                            Dictionary<string, string> package = new Dictionary<string, string>();
                            foreach (XmlNode key in node.ChildNodes)
                            {
                                output.keys.Add(key.Attributes["id"].InnerText, key.Attributes["val"].InnerText);
                            }
                            break;
                    }
                }
            }
 
            return output;
        }
    }
}
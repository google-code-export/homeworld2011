using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

using TImport = System.Xml.XmlDocument;


namespace PlagueLocalizationExtension
{
    [ContentImporter(".bab", DisplayName = "Plague .lang importer", DefaultProcessor = "PlagueLangProcessor")]
    public class TextImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            XmlDocument stringFile = new XmlDocument();
            try
            {
                stringFile.Load(filename);
            }
            catch (Exception e)
            {
                context.Logger.LogImportantMessage("The file "
                    + filename + " is not valid: " + e.Message);
                throw e;
            }
            return stringFile;
        }
    }
}

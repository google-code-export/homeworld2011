using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PlagueLocalizationExtension
{
    public class LangContent : ContentManager
    {
         public LangContent(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
            packages = new Dictionary<string , Dictionary<string, string>>();
            Language = DEFAULTLANGUAGE;
        }
 
        /// <summary>
        /// Load strings using &quot;PackageName.Key&quot;, load other files normally.
        /// </summary>
        public override T Load<T>(string assetName)
        {
 
            if (typeof(T) == typeof(String))
            {
                string[] packageKey = assetName.Split('.');
                if (packageKey.Length > 1 && !packageKey[0].Contains(' '))
                {
                    return (T)(object)GetString(packageKey[0], packageKey[1]);
                }
                else
                {
                    return (T)(object) (assetName);
                }
            }
            else
            {
                return base.Load<T>(assetName);
            }
        }
 
        /// <summary>
        /// Given a package and a key finds the corresponding string in the
        /// language currently set in .Language
        /// </summary>
        public string GetString(string package, string key)
        {
            return GetString(package, key, language);
        }
 
        /// <summary>
        /// Given a package and a key finds the corresponding string in the
        /// language requested
        /// </summary>
        public string GetString(string package, string key, string packageLanguage)
        {
            package = package.ToLower();
            key = key.ToLower();
            packageLanguage = ProperString(packageLanguage);
 
            if (packages.Keys.Contains(package))
            {
                if ((packages[package]).Keys.Contains(key))
                {
                    return (packages[package])[key];
                }
                else
                {
                    return "No key " + key;
                }
            }
 
            try
            {
                LangFile bab = base.Load<LangFile>(LangDir + "/" + packageLanguage + "/" + package);
                packages.Add(package, bab.keys);
            }
            catch (ContentLoadException )
            {
                //Now try to load the content using the default language
                //but only if the current language isn't the default language
                //else we would get stuck in infinite exception recursion.
                if (!packageLanguage.Equals(DEFAULTLANGUAGE))
                {
                    return GetString(package, key, DEFAULTLANGUAGE);
                }
            }
 
            if (packages.Keys.Contains(package))
            {
                if ((packages[package]).Keys.Contains(key))
                {
                    return (packages[package])[key];
                }
                else
                {
                    return "No key: " + key + "in package:" + package;
                }
            }
 
            //The string searched for couldnt be found in the
            //translated file or the default file.
            return String.Empty;
 
        }
 
        /// <summary>
        /// Converts LaNGuAGe to Language
        /// </summary>
        private string ProperString(string value)
        {
            if (value.Length > 0)
            {
                value = value.ToLower();
                value = value.ToUpper()[0] + value.Substring(1); ;
            }
            return value;
        }
 
        private Dictionary<string, Dictionary<string, string>> packages;
 
        private string language;
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                //Set the new language and clear the translation dictionary
                if (language == null || !language.Equals(ProperString(value)))
                {
                    language = ProperString(value);
                    packages.Clear();
                }
            }
        }

        public string LangDir { get; set; }
        //Change this variable to the default language your content files are in
        //This value will be used as a fallback for untranslated content
        private static string DEFAULTLANGUAGE = "English";
    }
}

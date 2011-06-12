using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

/************************************************************************************/
/// PlagueEngine.LowLevelGameFlow.GameObjects
/************************************************************************************/
namespace PlagueEngine.LowLevelGameFlow.GameObjects
{

    /********************************************************************************/
    /// Ammunition
    /********************************************************************************/
    class Ammunition : GameObjectInstance
    {

        /****************************************************************************/
        /// Properties
        /****************************************************************************/
        public Dictionary<uint, AmmunitionInfo>                          AmmunitionData        { get; private set; }
        public Dictionary<uint, Dictionary<uint, AmmunitionVersionInfo>> AmmunitionVersionData { get; private set; }
        public Dictionary<String, uint>                                  NameToID              { get; private set; }
        
        private uint lastID = 0;
        /****************************************************************************/


        /****************************************************************************/
        /// Init
        /****************************************************************************/
        public void Init(List<AmmunitionInfo> ammunitionData)
        {
            if (ammunitionData == null) return;

            AmmunitionData        = new Dictionary<uint, AmmunitionInfo>();
            AmmunitionVersionData = new Dictionary<uint, Dictionary<uint, AmmunitionVersionInfo>>();
            NameToID              = new Dictionary<String, uint>();

            foreach (AmmunitionInfo info in ammunitionData)
            {
                if (!NameToID.ContainsKey(info.Name))
                {
                    NameToID.Add(info.Name, ++lastID);
                    AmmunitionData.Add(lastID, info);
                    AmmunitionVersionData.Add(lastID, new Dictionary<uint, AmmunitionVersionInfo>());
                }

                foreach(AmmunitionVersionInfo version in info.Versions)
                {
                    if (!AmmunitionVersionData[NameToID[info.Name]].ContainsKey(version.Version))
                    {
                        AmmunitionVersionData[NameToID[info.Name]].Add(version.Version, version);
                    }
                }
            }
        }
        /****************************************************************************/


        /****************************************************************************/
        /// Get Data
        /****************************************************************************/
        public override GameObjectInstanceData GetData()
        {
            AmmunitionData data = new AmmunitionData();
            GetData(data);

            data.AmmunitionInfo = new List<AmmunitionInfo>();

            if (AmmunitionData == null) return data;

            foreach (var ammunitionInfo in AmmunitionData.Values)
            {
                data.AmmunitionInfo.Add(ammunitionInfo);
            }

            return data;
        }
        /****************************************************************************/        

    }
    /********************************************************************************/


    /********************************************************************************/
    /// Ammunition Info
    /********************************************************************************/
    [Serializable]
    class AmmunitionInfo
    {
        public AmmunitionInfo()
        {
            Versions = new List<AmmunitionVersionInfo>();
        }

        [CategoryAttribute("Identification")]
        public String Name  { get; set; }
        [CategoryAttribute("Identification"),
        DescriptionAttribute("1 - Pistol, 2 - Intermediate, 3 - Rifle, 4 - Shotgun, 5 - Large")]
        public uint Genre   { get; set; }
        
        [CategoryAttribute("Versions")]
        public List<AmmunitionVersionInfo> Versions { get; set; }

        [CategoryAttribute("P++")]
        public bool PPP { get; set; }

        [CategoryAttribute("Description")]
        public String Description { get; set; } 
    }
    /********************************************************************************/


    /********************************************************************************/
    /// Ammunition Version Info
    /********************************************************************************/
    [Serializable]
    class AmmunitionVersionInfo
    {
        [CategoryAttribute("Identification"),
        DescriptionAttribute("0 - FMJ, 1 - JSP, 2 - JHP, 3 - FMJS, 4 - JHPS, 5 - TMJS, 6 - AP, 7 - API")]
        public uint Version { get; set; }

        [CategoryAttribute("Parameters")]
        public float Damage         { get; set; }
        [CategoryAttribute("Parameters")]
        public float Accuracy       { get; set; }
        [CategoryAttribute("Parameters")]
        public float Range          { get; set; }
        [CategoryAttribute("Parameters")]
        public float Penetration    { get; set; }
        [CategoryAttribute("Parameters")]
        public float Recoil         { get; set; }
        [CategoryAttribute("Parameters")]
        public float StoppingPower  { get; set; }
        
        public static String VersionToString(uint version)
        {
            switch (version)
            {
                case 0: return "FMJ";
                case 1: return "JSP";
                case 2: return "JHP";
                case 3: return "FMJS";
                case 4: return "JHPS";
                case 5: return "TFMJ";
                case 6: return "AP";
                case 7: return "API";
                default: return String.Empty;
            }
        }
    }
    /********************************************************************************/


    /********************************************************************************/
    /// AmmunitionData
    /********************************************************************************/
    [Serializable]
    class AmmunitionData : GameObjectInstanceData
    {
        public AmmunitionData()
        {
            Type = typeof(Ammunition);            
        }

        [CategoryAttribute("Parameters")]
        public List<AmmunitionInfo> AmmunitionInfo { get; set; }
    }
    /********************************************************************************/

}
/************************************************************************************/
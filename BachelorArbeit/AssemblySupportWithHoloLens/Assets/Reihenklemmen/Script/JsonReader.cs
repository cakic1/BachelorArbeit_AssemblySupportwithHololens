
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class JsonReader : MonoBehaviour
{

    [Serializable]
    public class MaterialofModel
    {
        public int matNo { get; set; }
        public string GoName { get; set; }
        public string MatName { get; set; }
        public string UnityTransform { get; set; } //GameObject
        public string UnityMat { get; set; } // Material
        public string MatInfo { get; set; }

    }
    [Serializable]
    public class MaterialReihenfolge
    {
        public int matno { get; set; }
        public int matRheinfolge { get; set; }

        public bool redbridgeInfo_FrontRow { get; set; }
    }
    [Serializable]
    public class Model
    {
        public string ModelName { get; set; }

        public IList<MaterialofModel> matofmodel { get; set; }
        public IList<MaterialReihenfolge> matreihenfolge { get; set; }
    }

    [Serializable]
    public class AllModel
    {
        public IList<Model> Imodel { get; set; }
    }

    // Use this for initialization
    public static AllModel JsonFileRead()
    {

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        string pathFilename = "Assets/Resources/AssemblyModelJson";
        string pathFoldername = "Assets/Resources/";
        
        if (!Directory.Exists(pathFoldername)) // Hier ist für HoloLens, weil die HoloLens diesen Pfad "Assets/Resources/" nicht finden kann  
        {
            if (!Directory.Exists(Application.persistentDataPath + "/" + pathFoldername))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + pathFoldername);
                
            }
            pathFilename = Application.persistentDataPath + "/" + pathFilename + ".json"; // Für HoloLens müssen wir in der File-Pfad ".json" auch geben. 
                                                                                        //Sonst kann die HoloLens diese Json-Datei nicht lesen oder finden.
        }

        string jsontext = File.ReadAllText(pathFilename);

        AllModel alm = JsonConvert.DeserializeObject<AllModel>(jsontext);
        return alm;       
    }
}

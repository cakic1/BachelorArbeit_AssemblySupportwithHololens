using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrangeJsonData
{

    public class ArrangeClass
    {
        public int matNo { get; set; }
        public string GoName { get; set; }
        public string MatName { get; set; }
        public string UnityTransform { get; set; } //GameObject
        public string UnityMat { get; set; } // Material
        public string MatInfo { get; set; }
        public int matRheinfolge { get; set; }
        public bool redbridgeInfo_FrontRow { get; set; }
        public string ModelName { get; set; }

    }

    public List<ArrangeClass> LarrangeClass;

    /// <summary>
    /// Hier werden die Daten, die aus der Json-Datei eingelesen wurden, in der Reihenfolge geordnet.
    /// </summary>
    /// <param name="imodelNumber">aufgerufene Modellnummer</param>
    public ArrangeJsonData(int imodelNumber)
    {
        ArrangeClass arrangeClass;
        JsonReader.AllModel allmodel = JsonReader.JsonFileRead();
        LarrangeClass = new List<ArrangeClass>();
        
        var item = allmodel.Imodel[imodelNumber];

            foreach (var reihe in item.matreihenfolge.OrderBy(o => o.matRheinfolge).ToList())
            {
                foreach (var matmod in item.matofmodel)
                {
                    if (matmod.matNo == reihe.matno)
                    {
                        arrangeClass = new ArrangeClass();
                        arrangeClass.matNo = matmod.matNo;
                        arrangeClass.GoName = matmod.GoName;
                        arrangeClass.MatName = matmod.MatName;
                        arrangeClass.UnityTransform = matmod.UnityTransform;
                        arrangeClass.UnityMat = matmod.UnityMat;
                        arrangeClass.MatInfo = matmod.MatInfo;
                        arrangeClass.matRheinfolge = reihe.matRheinfolge;
                        arrangeClass.redbridgeInfo_FrontRow = reihe.redbridgeInfo_FrontRow;
                        arrangeClass.ModelName = item.ModelName;
                        LarrangeClass.Add(arrangeClass);
                    }
                }
            }
    }


    /// <summary>
    /// Das gibt die Total Stückzahl der Teile zurück für die Virtuelle-InfoDisplay
    /// </summary>
    /// <returns></returns>
    public int ModelTotalCount()
    {
        JsonReader.AllModel allmodel = JsonReader.JsonFileRead();

        int modeltotalcount = allmodel.Imodel.Count();

        return modeltotalcount;
    }

}
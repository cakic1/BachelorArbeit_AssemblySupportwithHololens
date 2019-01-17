using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using Vuforia;

public class PartClone : MonoBehaviour
{
    //Prefabs
    public GameObject RailPrefabClone;
    public GameObject BlueTerminalPrefabClone;
    public GameObject GrayTerminalPrefabClone;
    public GameObject RelaisPrefabClone;
    public GameObject RedBridgePrefabClone;
    public Transform ImageTarget;

    //NextModel
    public Transform BlueTerminalNextModel;
    public Transform GrayTerminalNextModel;
    public Transform RelaisNextModel;
    public Transform RedBridgeNextModel;

    public GameObject Arrow_Relais; // grüne Pfeile für Image Erkennung
    public GameObject Arrow_BlueTerminal;// grüne Pfeile für Image Erkennung
    public GameObject Arrow_GrayTerminal; //grüne Pfeile für Image Erkennung
    public GameObject Arrow_RedBridge; //grüne Pfeile für Image Erkennung
       
    //Materials
    public Material ShaderMaterial_Rail;
    public Material ShaderMaterial_BlueTerminal;
    public Material ShaderMaterial_GrayTerminal;
    public Material ShaderMaterial_Relais;
    public Material ShaderMaterial_RedBridge;
    private List<Material> _mats = new List<Material>(); // List für Materials (Farbe und Beleuchtungen von den Modellteielen)

    public Text InfoText;

    GameObject _sampleObject;

    List<GameObject> allObjects;
    List<GameObject> ImageTargetArrowList;

    float generalXAxisValue = 0.0f;

    // Breite der Prefabs
    const float BlueXaxisValue = 0.0208f;
    const float GrayXaxisValue = 0.0208f;
    const float RelaisXaxisValue = 0.0706f;
    const float RedbridgeXaxisValue = 0.057f;

    public float speed = 20f;
    private float _color_a = 0.11f;

    private Transform _activRotateNetxModel;

    ArrangeJsonData ajd;

    private int _assemblyModelNumber = 0;

    // Use this for initialization
    public void Start()
    {
        FirstFillOfList(0);
    }

    void FirstFillOfList(int AssemblyModelNumber)
    {
        ajd = new ArrangeJsonData(AssemblyModelNumber); // Hier wird die ArrangeJsonData-Klasse aufgerufen, um Daten zu holen.

        TextDisplay(ajd);

        ajd.LarrangeClass.RemoveAt(0); //Rail wurde gelöst. 

        _mats.Add(ShaderMaterial_Rail);
        _mats.Add(ShaderMaterial_BlueTerminal);
        _mats.Add(ShaderMaterial_GrayTerminal);
        _mats.Add(ShaderMaterial_Relais);
        _mats.Add(ShaderMaterial_RedBridge);

        allObjects = new List<GameObject>();


        ImageTargetArrowList = new List<GameObject>();
        ImageTargetArrowList.Add(Arrow_BlueTerminal);
        ImageTargetArrowList.Add(Arrow_GrayTerminal);
        ImageTargetArrowList.Add(Arrow_Relais);
        ImageTargetArrowList.Add(Arrow_RedBridge);


        NextModelOnOff(ajd.LarrangeClass[allObjects.Count].MatName + "NEXTMODEL");
    }


    /// <summary>
    /// Die Informationen von den Modellen werden hier auf der virtuelle Display gezeigt.
    /// </summary>
    /// <param name="arrangejsondata"></param>
    void TextDisplay(ArrangeJsonData arrangejsondata)
    {
        string fullText;

        string modelanzahlText = "Model " + (_assemblyModelNumber + 1) + "/" + arrangejsondata.ModelTotalCount() + Environment.NewLine + Environment.NewLine;

        string modelName = "Model Name: " + Environment.NewLine + arrangejsondata.LarrangeClass[0].ModelName + Environment.NewLine + Environment.NewLine;

        string materialAnzahl = "Material Anzahl: " + arrangejsondata.LarrangeClass.Count + Environment.NewLine + Environment.NewLine;

        string materialAnzahl_Detail = "";

        var materialname = from name in arrangejsondata.LarrangeClass
                           orderby name.matRheinfolge
                           select name.MatName;

        foreach (var value in materialname.Distinct())
        {
            int matcount = materialname.Count(name => name.StartsWith(value));

            materialAnzahl_Detail = materialAnzahl_Detail + matcount + " Stück " + value + Environment.NewLine;
        }

        fullText = modelanzahlText + modelName + materialAnzahl + materialAnzahl_Detail;

        InfoText.text = fullText;        
    }

 
    private void Update()
    {
        if (_activRotateNetxModel != null)
        {
            _activRotateNetxModel.gameObject.SetActive(true);
            _activRotateNetxModel.GetChild(0).Rotate(Vector3.left, speed * Time.deltaTime);
            _activRotateNetxModel.GetChild(1).Rotate(Vector3.up, speed * Time.deltaTime);
        }


        // Hier ist nur am Laptop ohne Sprachbefehlen zu steuern.
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            VoiceCommandNext();
        }
        else if (Input.GetKeyDown(KeyCode.P) == true)
        {
            VoiceCommandPrevious();
        }
        else if (Input.GetKeyDown(KeyCode.R) == true)
        {
            VoiceCommandReset();
        }
        else if (Input.GetKeyDown(KeyCode.C) == true)
        {
            VoiceCommandCompletely();
        }
        else if (Input.GetKeyDown(KeyCode.W) == true)
        {
            VoiceCommandNewAssembly();
        }
    }

    /// <summary>
    /// Hier ist für die NEXTPART. Nächste Teil wird aktiviert und vorherige Teil deaktiviert.
    /// </summary>
    /// <param name="materialname"></param>
    public void NextModelOnOff(string materialname)
    {
        if (_activRotateNetxModel != null)
        {
            _activRotateNetxModel.gameObject.SetActive(false);
        }

        switch (materialname.ToUpper())
        {
            case "BLUETERMINALNEXTMODEL":

                _activRotateNetxModel = BlueTerminalNextModel;

                break;
            case "GRAYTERMINALNEXTMODEL":
                _activRotateNetxModel = GrayTerminalNextModel;
                break;
            case "RELAISNEXTMODEL":
                _activRotateNetxModel = RelaisNextModel;
                break;
            case "REDBRIDGENEXTMODEL":
                _activRotateNetxModel = RedBridgeNextModel;
                break;

            default:

                _activRotateNetxModel.gameObject.SetActive(false);
                _activRotateNetxModel = null;
                break;
        }

        if (_activRotateNetxModel != null)
        {
            _activRotateNetxModel.gameObject.SetActive(true);
            _activRotateNetxModel.GetChild(0).Rotate(Vector3.left, speed * Time.deltaTime);
            _activRotateNetxModel.GetChild(1).Rotate(Vector3.up, speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Nachdem Image Scanning wird die Grüne Pfeile für die Box aktiviert, welche von gescannten Box gehört.
    /// </summary>
    /// <param name="ArrowName"></param>
    void ImageTargetControl(string ArrowName)
    {
        foreach (var item in ImageTargetArrowList)
        {
            if (item.name.ToUpper().Contains(ArrowName.ToUpper()))
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Hier wird bestimmt, welche Teil muss geklont werden.
    /// </summary>
    /// <returns></returns>
    public bool ModelBuild()
    {
        bool result = false;

        int tmpallobjectCounter = allObjects.Count + 2;

        foreach (var item in ajd.LarrangeClass)
        {
            if (item.matRheinfolge == tmpallobjectCounter)

            {
                //mit dem Materialnamen, welchen aus der JSON-Datei gelesen wurde, wird Gameobjekt geklont.
                switch (item.MatName.ToUpper())
                {
                    case "RAIL":

                        break;
                    case "BLUETERMINAL":

                        ObjectClone(BlueTerminalPrefabClone, BlueXaxisValue, item.MatName + item.matRheinfolge.ToString());

                        result = true;
                        break;
                    case "GRAYTERMINAL":


                        ObjectClone(GrayTerminalPrefabClone, GrayXaxisValue, item.MatName + item.matRheinfolge.ToString());
                        result = true;
                        break;
                    case "RELAIS":

                        ObjectClone(RelaisPrefabClone, RelaisXaxisValue, item.MatName + item.matRheinfolge.ToString());
                        
                        result = true;
                        break;
                    case "REDBRIDGE":
                        if (item.redbridgeInfo_FrontRow)
                            ObjectClone(RedBridgePrefabClone, RedbridgeXaxisValue / 3, item.MatName + item.matRheinfolge.ToString(), 0.0196f);
                        else
                            ObjectClone(RedBridgePrefabClone, RedbridgeXaxisValue / 3, item.MatName + item.matRheinfolge.ToString());
                        result = true;
                        break;
                    default:
                        Debug.Log(item.MatName.ToUpper()+" ist nicht erkannt.");
                        result = false;
                        break;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Hier werden die Prefabs geklont.
    /// </summary>
    /// <param name="Prefabs">Original Objekt um sich zu klonen</param>
    /// <param name="ObjectWidth">breite des Objekts</param>
    /// <param name="NewgoName">Name der neu geklontes Objekt</param>
    /// <param name="ObjectPosZ"> Das ist nur für die rote Steckenbrücken (RedBridge)</param>
    void ObjectClone(GameObject Prefabs, float ObjectWidth, string NewgoName, float ObjectPosZ = 0.0f)
    {
        generalXAxisValue += ObjectWidth;// _sampleObject.transform.position.x + ObjectWidth;

        _sampleObject = Instantiate(Prefabs, transform.localPosition, Quaternion.identity, ImageTarget.gameObject.transform);

        _sampleObject.transform.localPosition = new Vector3(generalXAxisValue, 0f, ObjectPosZ);//new Vector3(0f, 0f, 0f);
        _sampleObject.transform.localRotation = Quaternion.identity;
        
        _sampleObject.gameObject.SetActive(true);

        _sampleObject.name = NewgoName;
        allObjects.Add(_sampleObject);
    }
    /// <summary>
    /// Hier ist für den Sprachbefehl (NEXT), um den nächsten Teil aufzurufen.
    /// </summary>
    public void VoiceCommandNext()
    {

        if (allObjects.Count < ajd.LarrangeClass.Count)
        {
            if (ModelBuild())
            {
                if (ajd.LarrangeClass.Count > allObjects.Count)
                {
                    NextModelOnOff(ajd.LarrangeClass[allObjects.Count].MatName + "NEXTMODEL");
                }
                else
                {
                    NextModelOnOff("");
                }

                ImageTargetControl(ajd.LarrangeClass[allObjects.Count - 1].MatName);

                AnimatiorActivation(false);
            }
        }
        else
        {
            AnimatiorActivation(true);
        }
    }
    
    /// <summary>
    ///  Hier ist für den Sprachbefehl (PREVIOUS), um den vorherigen Teil aufzurufen.
    /// </summary>
    public void VoiceCommandPrevious()
    {
        if (allObjects.Count > 0)
        {
            string gameobjName = allObjects[allObjects.Count - 1].name.ToUpper();

            if (gameobjName.Contains("TERMINAL"))
            {
                generalXAxisValue -= BlueXaxisValue;
            }
            else if (gameobjName.Contains("RELAIS"))
            {
                generalXAxisValue -= RelaisXaxisValue;

            }
            else if (gameobjName.Contains("REDBRIDGE"))
            {
                generalXAxisValue -= RedbridgeXaxisValue / 3;
            }

            Destroy(allObjects[allObjects.Count - 1]);
            allObjects.RemoveAt(allObjects.Count - 1);


            NextModelOnOff(ajd.LarrangeClass[allObjects.Count].MatName + "NEXTMODEL");

            ImageTargetControl(ajd.LarrangeClass[allObjects.Count - 1].MatName);

            AnimatiorActivation(false);
        }

        else
        {
            NextModelOnOff(ajd.LarrangeClass[allObjects.Count].MatName + "NEXTMODEL"); // Das ist nur für erste NEXTPART Objekt.
            generalXAxisValue = 0.0f;

            allObjects = new List<GameObject>();

            _sampleObject = new GameObject();
        }
    }

    /// <summary>
    /// Hier ist für den Sprachbefehl (COMPLETELY), um das ganzes Model zuzeigen.
    /// </summary>
    public void VoiceCommandCompletely()
    {
        VoiceCommandReset();

        foreach (var item in ajd.LarrangeClass)
        {
            ModelBuild();
        }
        NextModelOnOff("");
    }

    /// <summary>
    /// Hier ist für den Sprachbefehl (RESET), um das ganzes Model wegzunehmen.
    /// </summary>
    public void VoiceCommandReset()
    {
        for (int i = allObjects.Count - 1; i >= 0; i--)
        {
            Destroy(allObjects[i]);
            allObjects.RemoveAt(i);
        }

        _sampleObject = new GameObject();

        generalXAxisValue = 0.0f;
        ImageTargetControl("xxxx");
        NextModelOnOff(ajd.LarrangeClass[0].MatName + "NEXTMODEL");
    }

    /// <summary>
    /// Hier wird mit der Sprachbefehl NewAssembly aufgeruft, um neues Model zuholen.
    /// </summary>
    public void VoiceCommandNewAssembly() 
    {

        if (_assemblyModelNumber < (ajd.ModelTotalCount() - 1))
        {
            _assemblyModelNumber += 1;

            for (int i = allObjects.Count - 1; i >= 0; i--)
            {
                Destroy(allObjects[i]);
                allObjects.RemoveAt(i);
            }
            for (int i = _mats.Count - 1; i >= 0; i--)
            {
                _mats.RemoveAt(i);
            }

            generalXAxisValue = 0.0f;
            _sampleObject = new GameObject();

            FirstFillOfList(_assemblyModelNumber);
        }
        else
        {
            Debug.Log("Es gibt keine andere Assembly!");
        }
    }
    /// <summary>
    /// Hier wird mit der Sprachbefehl LastAssembly aufgeruft, um letztes Model zuholen.
    /// </summary>
    public void VoiceCommandLastAssembly()
    {
        if (_assemblyModelNumber > 0)
        {
            _assemblyModelNumber -= 1;

            for (int i = allObjects.Count - 1; i >= 0; i--)
            {
                Destroy(allObjects[i]);
                allObjects.RemoveAt(i);
            }
            for (int i = _mats.Count - 1; i >= 0; i--)
            {
                _mats.RemoveAt(i);
            }

            generalXAxisValue = 0.0f;
            _sampleObject = new GameObject();

            FirstFillOfList(_assemblyModelNumber);
        }
        else
        {
            Debug.Log("Es gibt keine andere Assembly!");
        }
    }

    /// <summary>
    /// Hier ist für den Sprachbefehl (DARK), um das ganzes Models noch sichtbarer zumachen.
    /// </summary>
    public void VoiceCommandDark()
    {
        if (_color_a <= 0.97f)
            _color_a = _color_a + 0.03f;
        ColorLighting(_color_a);
    }

    /// <summary>
    /// Hier ist für den Sprachbefehl (DARK), um das ganzes Models noch transparenter zumachen.
    /// </summary>
    public void VoiceCommandLight()
    {
        if (_color_a >= 0.03f)
            _color_a = _color_a - 0.03f;
        ColorLighting(_color_a);
    }

    private void ColorLighting(float colorValue)
    {
        foreach (var item in _mats)
        {
            item.color = new Color(item.color.r, item.color.g, item.color.b, colorValue);
        }
    }

    void AnimatiorActivation(bool Stop)
    {
        Animator anim_Inactiv = allObjects[allObjects.Count - 1].transform.GetComponentInChildren<Animator>();
        Animator anim_activ = allObjects[allObjects.Count - 1].transform.GetComponentInChildren<Animator>();

        if ((allObjects.Count > 1))
        {
            anim_Inactiv = allObjects[allObjects.Count - 2].transform.GetComponentInChildren<Animator>();
        }

        if (allObjects.Count != 1)
        {
            anim_Inactiv.SetBool("Start", false);
        }
        anim_activ.SetBool("Start", true);

        if (Stop)
        {
            anim_activ.SetBool("Start", false);
            anim_Inactiv.SetBool("Start", false);
        }
    }
}

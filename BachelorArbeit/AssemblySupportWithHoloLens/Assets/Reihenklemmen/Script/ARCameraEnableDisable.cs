using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ARCameraEnableDisable : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    /// <summary>
    /// AR-Camera von Vuforia wird aktiviert.
    /// Das geht mit dem Sprachbefehl "START"
    /// </summary>
    public void ARCameraStart()
    {
        CameraDevice.Instance.Init();

        CameraDevice.Instance.Start();

    }

    /// <summary>
    /// AR-Camera von Vuforia wird deaktiviert.
    /// Das geht mit dem Sprachbefehl "STOP"
    /// </summary>
    public void ARCameraStop()
    {
        CameraDevice.Instance.Stop();

        // wenn man das auch verwenden würde, kommen die virtuelle Objekten, die unter Image-Target stehen, nicht auf den realen Welt     
        //CameraDevice.Instance.Deinit();   
    }
}

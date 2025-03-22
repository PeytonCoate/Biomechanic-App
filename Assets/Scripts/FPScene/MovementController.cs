using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;

public class MovementController : MonoBehaviour
{
    [SerializeField] private GameObject mainModel;

    private GameObject TLL_HipBox, TLL_KneeBox, TLL_AnkleBox;
    private GameObject TRL_HipBox, TRL_KneeBox, TRL_AnkleBox;

    private GameObject TLL_Hip, TLL_Knee, TLL_Ankle;
    private GameObject TRL_Hip, TRL_Knee, TRL_Ankle;

    private Renderer[] targetRendererList = new Renderer[6];

    /* //obsolete
    private List<Vector3> TRL_HipData = new List<Vector3>();
    private List<Vector3> TRL_KneeData = new List<Vector3>();
    private List<Vector3> TRL_AnkleData = new List<Vector3>();

    private List<Vector3> TLL_HipData = new List<Vector3>();
    private List<Vector3> TLL_KneeData = new List<Vector3>();
    private List<Vector3> TLL_AnkleData = new List<Vector3>();


    private Vector3 TRL_Offset = new Vector3(-30, 0, 0);
    private Vector3 TLL_Offset = new Vector3(30, 0, 0);


    private int frameIndex = 0;
    private float updateRate = .01f;
    */

    private float yellowThreshold = 11f; // Distance where box turns yellow
    private float redThreshold = 13f; // Distance where box turns red

    void Start()
    {
        Transform[] joints = mainModel.GetComponentsInChildren<Transform>();
        int j = 0;
        foreach(Transform joint in joints)
        {
            Renderer targetRenderer = joint.GetComponent<Renderer>();
            
            if (targetRenderer != null && joint.childCount > 0)
            {
                targetRendererList[j] = targetRenderer;
                j++;
                Transform jointSphereTransform = joint.transform.GetChild(0);
                jointSphereTransform.position = targetRenderer.bounds.center;

            }
        }

        TLL_HipBox = joints[1].gameObject;
        TLL_KneeBox = joints[5].gameObject;
        TLL_AnkleBox = joints[9].gameObject;

        TRL_HipBox = joints[3].gameObject;
        TRL_KneeBox = joints[7].gameObject;
        TRL_AnkleBox = joints[11].gameObject;

        TLL_Hip = TLL_HipBox.transform.GetChild(0).gameObject;
        TLL_Knee = TLL_KneeBox.transform.GetChild(0).gameObject;
        TLL_Ankle = TLL_AnkleBox.transform.GetChild(0).gameObject;

        TRL_Hip = TRL_HipBox.transform.GetChild(0).gameObject;
        TRL_Knee = TRL_KneeBox.transform.GetChild(0).gameObject;
        TRL_Ankle = TRL_AnkleBox.transform.GetChild(0).gameObject;

        //StartCoroutine(UpdatePositions());
    }

    public void loadJoints()
    {
        TLL_Hip.GetComponent<MeshRenderer>().enabled = true;
        TLL_Knee.GetComponent<MeshRenderer>().enabled = true;
        TLL_Ankle.GetComponent<MeshRenderer>().enabled = true;

        TRL_Hip.GetComponent<MeshRenderer>().enabled = true;
        TRL_Knee.GetComponent<MeshRenderer>().enabled = true;
        TRL_Ankle.GetComponent<MeshRenderer>().enabled = true;
    }

    public void unloadJoints()
    {
        TLL_Hip.GetComponent<MeshRenderer>().enabled = false;
        TLL_Knee.GetComponent<MeshRenderer>().enabled = false;
        TLL_Ankle.GetComponent<MeshRenderer>().enabled = false;

        TRL_Hip.GetComponent<MeshRenderer>().enabled = false;
        TRL_Knee.GetComponent<MeshRenderer>().enabled = false;
        TRL_Ankle.GetComponent<MeshRenderer>().enabled = false;
    }


    //list data likely no longer needed because the position of the joints will be determined by the position of the progress bar.
    public void LoadCSVData(string path, int progressBarValue, string[] lines)  /*ref List<Vector3> hipData, ref List<Vector3> kneeData, ref List<Vector3> ankleData, bool flipX*/
    {
        //string path = Path.Combine(Application.streamingAssetsPath, fileName); not neccesary as the file path is now passed in from the FileLoader.
        
        if (!File.Exists(path))
        {
            Debug.LogError("File not found: " + path);
            return;
        }

        string currentLine = lines[progressBarValue];

        BuildPositionList(currentLine);

    }

    public void LoadReadInData(string readInLine)
    {
        BuildPositionList(readInLine);
    }

    private void BuildPositionList(string currentLine)
    {
        string[] values = currentLine.Split(',');


        if (values.Length < 19)
        {
            Debug.LogWarning("not enough columns in file row.");
            return;
        }

        //string time = values[0].Trim(); //use if time is ever neccesary

        float[] positionList = new float[values.Length - 1];

        for (int i = 1; i < values.Length; i++)
        {
            positionList[i - 1] = float.Parse(values[i]);
        }
        Vector3[] vectorList = new Vector3[6];
        int j = 0;
        for (int i = 0; i < positionList.Length; i += 3)
        {
            vectorList[j] = new Vector3(positionList[i], positionList[i + 1], positionList[i + 2]);
            j++;
        }

        UpdatePositions(vectorList);
    }

    private void UpdatePositions(Vector3[] vectorList)
    {

        // Move spheres based on CSV while applying offsets

        TLL_Hip.transform.position = vectorList[4] + targetRendererList[0].bounds.center; //THESE TWO TARGET RENDERERS ARE TEMPORARILY SWAPPED DUE TO DATA LABELING BEING WRONG
        TLL_Knee.transform.position = vectorList[2] + targetRendererList[2].bounds.center;//THESE TWO TARGET RENDERERS ARE TEMPORARILY SWAPPED DUE TO DATA LABELING BEING WRONG
        TLL_Ankle.transform.position = vectorList[0] + targetRendererList[4].bounds.center;
        


        TRL_Hip.transform.position = vectorList[1] + targetRendererList[1].bounds.center;
        TRL_Knee.transform.position = vectorList[3] + targetRendererList[3].bounds.center; ;
        TRL_Ankle.transform.position = vectorList[5] + targetRendererList[5].bounds.center;



        // Check and update box colors based on deviation

        /*
        UpdateBoxColor(TRL_HipBox, TRL_Hip.transform.position, targetRendererList[0].bounds.center);
        UpdateBoxColor(TRL_KneeBox, TRL_Knee.transform.position, targetRendererList[2].bounds.center);
        UpdateBoxColor(TRL_AnkleBox, TRL_Ankle.transform.position, targetRendererList[4].bounds.center);

        UpdateBoxColor(TLL_HipBox, TLL_Hip.transform.position, targetRendererList[1].bounds.center);
        UpdateBoxColor(TLL_KneeBox, TLL_Knee.transform.position, targetRendererList[3].bounds.center);
        UpdateBoxColor(TLL_AnkleBox, TLL_Ankle.transform.position, targetRendererList[5].bounds.center);
        */
        //yield return new WaitForSeconds(updateRate);

    }

    private void UpdateBoxColor(GameObject box, Vector3 spherePos, Vector3 boxCenter)
    {
        float distance = Vector3.Distance(spherePos, boxCenter);
        Renderer renderer = box.GetComponent<Renderer>();

        if (distance > redThreshold)
            renderer.material.color = new Color(1f, 0f, 0f, 0.3f); // Transparent Red
        else if (distance > yellowThreshold)
            renderer.material.color = new Color(1f, 1f, 0f, 0.3f); // Transparent Yellow
        else
            renderer.material.color = new Color(0f, 1f, 0f, 0.3f); // Transparent Green
    }

}

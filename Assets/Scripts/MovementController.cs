using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MovementController : MonoBehaviour
{
    public GameObject TRL_Hip, TRL_Knee, TRL_Ankle;
    public GameObject TLL_Hip, TLL_Knee, TLL_Ankle;

    public GameObject TRL_HipBox, TRL_KneeBox, TRL_AnkleBox;
    public GameObject TLL_HipBox, TLL_KneeBox, TLL_AnkleBox;

    private List<Vector3> TRL_HipData = new List<Vector3>();
    private List<Vector3> TRL_KneeData = new List<Vector3>();
    private List<Vector3> TRL_AnkleData = new List<Vector3>();

    private List<Vector3> TLL_HipData = new List<Vector3>();
    private List<Vector3> TLL_KneeData = new List<Vector3>();
    private List<Vector3> TLL_AnkleData = new List<Vector3>();

    private Vector3 TRL_Offset = new Vector3(-30, 0, 0);
    private Vector3 TLL_Offset = new Vector3(30, 0, 0);

    private int frameIndex = 0;
    private float updateRate = 0.05f;

    private float yellowThreshold = 11f; // Distance where box turns yellow
    private float redThreshold = 13f; // Distance where box turns red

    void Start()
    {
        LoadCSVData("TRL.csv", ref TRL_HipData, ref TRL_KneeData, ref TRL_AnkleData, false);
        LoadCSVData("TLL.csv", ref TLL_HipData, ref TLL_KneeData, ref TLL_AnkleData, true);

        StartCoroutine(UpdatePositions());
    }

    void LoadCSVData(string fileName, ref List<Vector3> hipData, ref List<Vector3> kneeData, ref List<Vector3> ankleData, bool flipX)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        
        if (!File.Exists(path))
        {
            Debug.LogError("File not found: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        foreach (string line in lines)
        {
            string[] values = line.Split(',');

            if (values.Length < 4) continue;

            string label = values[0].Trim();
            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);

            if (flipX) x *= -1;

            Vector3 position = new Vector3(x, y, z);

            if (label == "Hip") hipData.Add(position);
            else if (label == "Kne") kneeData.Add(position);
            else if (label == "Ank") ankleData.Add(position);
        }
    }

    IEnumerator UpdatePositions()
    {
        while (true)
        {
            if (TRL_HipData.Count > frameIndex)
            {
                // Move spheres based on CSV while applying offsets
                TRL_Hip.transform.position = TRL_HipData[frameIndex] + TRL_Offset + new Vector3(0, 60, 0);
                TRL_Knee.transform.position = TRL_KneeData[frameIndex] + TRL_Offset + new Vector3(0, 30, 0);
                TRL_Ankle.transform.position = TRL_AnkleData[frameIndex] + TRL_Offset;

                TLL_Hip.transform.position = TLL_HipData[frameIndex] + TLL_Offset + new Vector3(0, 60, 0);
                TLL_Knee.transform.position = TLL_KneeData[frameIndex] + TLL_Offset + new Vector3(0, 30, 0);
                TLL_Ankle.transform.position = TLL_AnkleData[frameIndex] + TLL_Offset;

                // Check and update box colors based on deviation
                UpdateBoxColor(TRL_HipBox, TRL_Hip.transform.position, TRL_Offset + new Vector3(0, 60, 0));
                UpdateBoxColor(TRL_KneeBox, TRL_Knee.transform.position, TRL_Offset + new Vector3(0, 30, 0));
                UpdateBoxColor(TRL_AnkleBox, TRL_Ankle.transform.position, TRL_Offset);

                UpdateBoxColor(TLL_HipBox, TLL_Hip.transform.position, TLL_Offset + new Vector3(0, 60, 0));
                UpdateBoxColor(TLL_KneeBox, TLL_Knee.transform.position, TLL_Offset + new Vector3(0, 30, 0));
                UpdateBoxColor(TLL_AnkleBox, TLL_Ankle.transform.position, TLL_Offset);
            }

            frameIndex = (frameIndex + 1) % Mathf.Min(TRL_HipData.Count, TLL_HipData.Count);
            yield return new WaitForSeconds(updateRate);
        }
    }

    void UpdateBoxColor(GameObject box, Vector3 spherePos, Vector3 boxCenter)
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

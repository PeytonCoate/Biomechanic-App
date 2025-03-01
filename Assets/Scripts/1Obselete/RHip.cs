using System.Collections.Generic;
using UnityEngine;

public class HipMove : MonoBehaviour
{
    public string data_05; // Name of the CSV file (without the extension)
    private List<Vector3> positions; // List to hold positions read from the CSV
    public float speed = 2.5f; // Speed of movement

    void Start()
    {
        positions = new List<Vector3>();
        ReadCSV();
    }

    void ReadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(data_05);
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Resources folder");
            return;
        }

        string[] csvLines = csvFile.text.Split('\n');

        for (int i = 0; i < csvLines.Length; i++) // Process each line
        {
            string[] values = csvLines[i].Split(',');

            // Check if the line starts with "Hip"
            if (values.Length == 4 && values[0].Trim() == "RHip" && 
                float.TryParse(values[1], out float x) && 
                float.TryParse(values[2], out float y) && 
                float.TryParse(values[3], out float z))
            {
                Vector3 position = new Vector3(x, y, z);
                positions.Add(position);
            }
        }
    }

    void Update()
    {
        // Example: Move the sphere to each position over time
        if (positions.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, positions[0], Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, positions[0]) < 0.1f)
            {
                positions.RemoveAt(0);
            }
        }
    }
}

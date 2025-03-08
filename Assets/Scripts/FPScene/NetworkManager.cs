using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TreeEditor;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string url = "http://18.223.209.117:8080/patients";
        Get(url, (string error) => {
            Debug.Log("Error: " + error);
        }, (string text) =>
        {
            List<Patient> patients = JsonConvert.DeserializeObject<List<Patient>>(text);
            //Debug.Log("Received: " + text);

            foreach (var patient in patients)
            {
                Debug.Log(patient.fld_p_name);
            }
        });
    }

    private void Get(string url, Action<string> onError, Action<string> onSuccess)
    {
        StartCoroutine(GetCoroutine(url, onError, onSuccess));
    }

    private IEnumerator GetCoroutine(string url, Action<string> onError, Action<string> onSuccess)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                onError(webRequest.error);
            }
            else
            {
                onSuccess(webRequest.downloadHandler.text);
            }
        }
    }
}

[System.Serializable]
public class Patient
{
    public string fld_p_id_pk;
    public string fld_p_name;
    public int fld_p_number;
    public int fld_p_age;
    public string fld_p_dob;
    public string fld_t_id_fk;
}
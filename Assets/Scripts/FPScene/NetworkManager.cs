using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TreeEditor;
using System.Text;
using UnityEngine.UI;
using System.Net;
using UnityEditor.PackageManager.Requests;
using TMPro;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private GameObject userProfileButtonTemplate;
    [SerializeField] private Transform profiles;
    private readonly string serverUrl = "http://18.223.209.117:8080";
    // Start is called before the first frame update
    void Start()
    {

    }

    public void TryLogin(){
        string inputEmail = GameObject.Find("UsernameInput").GetComponent<TMP_InputField>().text;
        string inputPass = GameObject.Find("PasswordInput").GetComponent<TMP_InputField>().text;
        var dataToPost = new PostData() { email = inputEmail, pass = inputPass }; // email = "RAWR@gmail.com", pass = "rawr123" 
        StartCoroutine(PostLoginCoroutine((string error) =>
        {
            Debug.Log("Error: " + error);
            GameObject.Find("ErrorText").GetComponent<TMP_Text>().text = "Error: " + error;
        }, (string text) =>
        {
            JsonUtility.FromJson<PostResult>(text);
            Debug.Log(text);
            LoadUserProfiles();

            GameObject.Find("ErrorText").GetComponent<TMP_Text>().text = "";
            GameObject.Find("Canvas").GetComponent<MenuController>().PopPage();
        }, dataToPost));
    }

    private void LoadUserProfiles()
    {
        Get(serverUrl + "/patients", (string error) => {
            Debug.Log("Error: " + error);
        }, (string text) =>
        {
            List<Patient> patients = JsonConvert.DeserializeObject<List<Patient>>(text);
            //Debug.Log("Received: " + text);

            foreach (var patient in patients)
            {
                Debug.Log(patient.fld_p_name);
                GameObject profileButton = Instantiate(userProfileButtonTemplate, profiles, false) as GameObject;
                TMP_Text[] textComponents = profileButton.GetComponentsInChildren<TMP_Text>();
                textComponents[0].text = patient.fld_p_name;
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

    private IEnumerator PostLoginCoroutine(Action<string> onError, Action<string> onSuccess, PostData dataToPost)
    {
        
        var postRequest = CreateRequest(serverUrl + "/users/login", RequestType.POST, dataToPost);
        yield return postRequest.SendWebRequest();
        if (postRequest.result == UnityWebRequest.Result.ConnectionError || postRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            onError(postRequest.error);
        }
        else
        {
            //var deserializedPostData = JsonUtility.FromJson<PostResult>(postRequest.downloadHandler.text);
            onSuccess(postRequest.downloadHandler.text);
        }
        
    }


    private UnityWebRequest CreateRequest(string path, RequestType type = RequestType.GET, object data = null)
    {
        var request = new UnityWebRequest(path, type.ToString());

        if (data != null)
        {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }
}

public enum RequestType
{
    GET = 0,
    POST = 1,
    PUT = 2,
    DELETE = 3,
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

[Serializable]
public class PostData
{
    public string email;
    public string pass;
}

public class PostResult
{
    public string success { get; set; }
}
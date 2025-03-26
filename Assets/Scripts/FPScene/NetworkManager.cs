using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;


public class NetworkManager : MonoBehaviour
{
    [SerializeField] private GameObject userProfileButtonTemplate;
    [SerializeField] private Transform profiles;

    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject logoutButton;

    private readonly string serverUrl = "http://18.223.209.117:8080";

    private TokenResponse tokenResponse;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Attempts to log into the server.
    /// </summary>
    public void TryLogin() {
        string inputEmail = GameObject.Find("UsernameInput").GetComponent<TMP_InputField>().text;
        string inputPass = GameObject.Find("PasswordInput").GetComponent<TMP_InputField>().text;
        var dataToPost = new UserLoginData() { email = inputEmail, pass = inputPass }; // email = "RAWR@gmail.com", pass = "rawr123" 

        var postRequest = CreateRequest(serverUrl + "/users/login", RequestType.POST, dataToPost);
        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log("Error: " + error);
            GameObject.Find("ErrorText").GetComponent<TMP_Text>().text = "Error: " + error;
            ResetInputText();
        }, (string text, Dictionary<string, string> responseHeaders) => //if successful, loads user profiles into settings
        {
            tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(text);

            string accessToken = tokenResponse.accessToken;
            Debug.Log(accessToken);

            if (responseHeaders.TryGetValue("set-cookie", out string cookieHeader))
            {
                string refreshToken = ExtractRefreshToken(cookieHeader);
                Debug.Log("Refresh Token Extracted: " + refreshToken);
                PlayerPrefs.SetString("refreshToken", refreshToken); // Store it for future use
            }
            else
            {
                Debug.LogWarning("Refresh token cookie not found");
            }


            LoadUserProfiles();
            ResetInputText();
            GameObject.Find("ErrorText").GetComponent<TMP_Text>().text = "";
            loginButton.SetActive(false);
            logoutButton.SetActive(true);
            GameObject.Find("Canvas").GetComponent<MenuController>().PopPage();
        }, postRequest));
    }

    private string ExtractRefreshToken(string cookieHeader)
    {
        string[] cookies = cookieHeader.Split(';');
        foreach (string cookie in cookies)
        {
            if (cookie.Trim().StartsWith("refreshToken="))
            {
                return cookie.Split('=')[1].Trim();
            }
        }
        return null;
    }

    /// <summary>
    /// Attempts to log into the server. Requires the access token so the server is able to delete it.
    /// </summary>
    public void TryLogout()
    {
        if (tokenResponse == null)
        {
            return;
        }

        TokenData deleteData = new TokenData();
        deleteData.token = tokenResponse.accessToken;

        var postRequest = CreateRequest(serverUrl + "/users/logout", RequestType.DELETE, deleteData);

        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log("Error: " + error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {

            JsonUtility.FromJson<PostResult>(text);
            Debug.Log(text);

            ResetInputText();
            logoutButton.SetActive(false);
            loginButton.gameObject.SetActive(true);
        }, postRequest));
    }

    //Resets the input fields after logging in.
    private void ResetInputText()
    {
        GameObject.Find("UsernameInput").GetComponent<TMP_InputField>().text = string.Empty;
        GameObject.Find("PasswordInput").GetComponent<TMP_InputField>().text = string.Empty;
    }

    /// <summary>
    /// Verifies that the access token is valid and not expired.
    /// </summary>
    private void VerifyAccessToken()
    {
        if (tokenResponse == null)
        {
            return;
        }

        TokenData expiryCheckData = new TokenData();
        expiryCheckData.token = tokenResponse.refreshToken;

        var postRequest = CreateRequest(serverUrl + "/users/login/test", RequestType.POST, expiryCheckData);

        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log("Error: " + error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {

            JsonUtility.FromJson<PostResult>(text);
            Debug.Log(text);

        }, postRequest));
    }

    /// <summary>
    /// Loads all of the user profiles that the logged in user has control over
    /// TODO: update the URL to correct access; /patients shows all test patients, whereas the user should only see the patients they have control over.
    /// </summary>
    private void LoadUserProfiles()
    {
        GetPatients(serverUrl + "/patients", (string error) => {
            Debug.Log("Error: " + error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {

            List<Patient> patients = JsonConvert.DeserializeObject<List<Patient>>(text); //TODO: load the files from patient[0] into the recordings cloud tab.
            //Debug.Log("Received: " + text);

            foreach (var patient in patients)
            {
                //Debug.Log(patient.fld_p_name);
                GameObject profileButton = Instantiate(userProfileButtonTemplate, profiles, false) as GameObject;
                TMP_Text[] textComponents = profileButton.GetComponentsInChildren<TMP_Text>();
                textComponents[0].text = patient.fld_p_name;

                Button buttonComponent = profileButton.GetComponent<Button>();

                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(VerifyAccessToken);
                    buttonComponent.onClick.AddListener(() => GetPatientRecordings(patient.fld_p_number));
                }
            }
        });
    }

    /// <summary>
    /// Gets all patients that the user has control over. might need adjustment when LoadUSerProfiles() is changed.
    /// </summary>
    private void GetPatients(string url, Action<string> onError, Action<string, Dictionary<string, string>> onSuccess)
    {
        var webRequest = UnityWebRequest.Get(url);
        StartCoroutine(GeneralRequestCoroutine(onError, onSuccess, webRequest));
    }

    /// <summary>
    /// Gets a JSON list of all of the selected patients recordings.
    /// </summary>
    /// <param name="p_number">Taken from LoadUserProfiles().</param>
    private void GetPatientRecordings(int p_number){

        var patientId = new { fld_p_number = p_number };
        var postRequest = CreateRequest(serverUrl + "/patient/recordings/:id", RequestType.GET, patientId);
        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log("Error: " + error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {

            JsonUtility.FromJson<PostResult>(text);
            Debug.Log(text);

        }, postRequest));
    }



    //ENUMERATOR

    /// <summary>
    /// A general IEnumerator used to return request results.
    /// </summary>
    /// <param name="onError">Used to return and handle errors.</param>
    /// <param name="onSuccess">Used to return and handle successful calls.</param>
    /// <param name="request">The unitywebrequest being sent. see CreateRequest() for more details.</param>
    /// <returns></returns>
    private IEnumerator GeneralRequestCoroutine(Action<string> onError, Action<string, Dictionary<string, string>> onSuccess, UnityWebRequest request)
    {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            onError(request.error);
        }
        else
        {
            Dictionary<string, string> headers = request.GetResponseHeaders();
            onSuccess(request.downloadHandler.text, headers);
        }
    }

    //WEB REQUEST

    /// <summary>
    /// Used to generate a Unitywebrequest.
    /// </summary>
    /// <param name="path">The path the request will use.</param>
    /// <param name="type">An enum determining the type of request. currently can be GET, POST, PUT, or DELETE.</param>
    /// <param name="data">an object representing data set to null by default. used to pass body JSON objects.</param>
    /// <returns>A unity web request used as a parameter for GeneralRequestCoroutine</returns>
    private UnityWebRequest CreateRequest(string path, RequestType type = RequestType.GET, object data = null)
    {
        var request = new UnityWebRequest(path, type.ToString());

        if (data != null)
        {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            Debug.Log(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }
}

//request type enum
public enum RequestType
{
    GET = 0,
    POST = 1,
    PUT = 2,
    DELETE = 3,
}

[System.Serializable]

//Patient class.
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
//user login data.
public class UserLoginData
{
    public string email;
    public string pass;
}

/// <summary>
/// used to store token resposes.
/// </summary>
public class TokenResponse
{
    public string accessToken;
    public string refreshToken;
}

/// <summary>
/// used to send a single token.
/// </summary>
public class TokenData
{
    public string token;
}

//unused i think
public class PostResult
{
    public string success { get; set; }
}
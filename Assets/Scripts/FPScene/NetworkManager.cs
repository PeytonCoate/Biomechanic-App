using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
//using static System.Net.Mime.MediaTypeNames;


public class NetworkManager : MonoBehaviour
{
    [SerializeField] private GameObject userProfileButtonTemplate;
    [SerializeField] private Transform profiles;

    [SerializeField] private GameObject RecordingButtonCloudTemplate;
    [SerializeField] private Transform RecordingItems;

    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject logoutButton;

    [SerializeField] private Toggle CloudRecordingsToggle;
    [SerializeField] private Toggle DesktopRecordingsToggle;

    [SerializeField] private FileManager fileManager;

    private int sortType = 0;

    private readonly string serverUrl = "http://18.188.25.75:8080";

    private string mainPath;

    private TokenResponse tokenResponse;

    // Start is called before the first frame update
    void Awake()
    {

    }

    private void TryRegister()
    {
        string inputEmail = "RAWR@gmail.com";
        string inputPass = "rawr123";
        var dataToPost = new UserRegisterData() { email = inputEmail, pass = inputPass, provider = false, name = "RAWR123" }; // email = "RAWR@gmail.com", pass = "rawr123" 

        var postRequest = CreateRequest(serverUrl + "/users/register", RequestType.POST, dataToPost);
        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log("Error: " + error);
        }, (string text, Dictionary<string, string> responseHeaders) => 
        {
            Debug.Log(text);
        }, postRequest));
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

            if (responseHeaders.TryGetValue("set-cookie", out string cookieHeader))
            {
                tokenResponse.refreshToken = cookieHeader;
                //SaveCookieSecurely(cookieHeader); //TODO: securely save cookie
            }
            else
            {
                Debug.LogWarning("Refresh token cookie not found");
            }



            if (CloudRecordingsToggle.isOn)
            {
                LoadUserExercises();
            }
            else if(DesktopRecordingsToggle.isOn)
            {
                CloudRecordingsToggle.isOn = true;
            }


            ResetInputText();
            GameObject.Find("ErrorText").GetComponent<TMP_Text>().text = "";
            loginButton.SetActive(false);
            logoutButton.SetActive(true);
            GameObject.Find("Canvas").GetComponent<MenuController>().PopPage();
        }, postRequest));

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

            tokenResponse =  null;
            fileManager.UnloadSelectedFolderFiles();

            ResetInputText();
            logoutButton.SetActive(false);
            loginButton.gameObject.SetActive(true);
        }, postRequest));
    }

    /// <summary>
    /// Verifies that the access token is valid and not expired.
    /// </summary>
    private void CheckRefreshToken()
    {
        if (tokenResponse == null)
        {
            return;
        }

        TokenData RefreshToken = new TokenData();
        RefreshToken.token = ExtractRefreshToken(tokenResponse.refreshToken);

        var postRequest = CreateRequest(serverUrl + "/users/check", RequestType.GET, RefreshToken);

        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log("Error: " + error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {

            Debug.Log(text);

        }, postRequest));
    }


    private bool AuthenticateAccessToken()
    {
        if (tokenResponse == null) { return false; }

        bool authenticated = false;

        TokenData tokenToAuthenticate = new TokenData();
        tokenToAuthenticate.token = tokenResponse.accessToken;

        var postRequest = CreateRequest(serverUrl + "/users/login/test", RequestType.POST, tokenToAuthenticate);

        postRequest.SetRequestHeader("authorization", $"Bearer {tokenResponse.accessToken}");

        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log(error);
            if (postRequest.responseCode == 403)
            {
                RegenerateAcessToken();
            }
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {
            authenticated = true;
        }, postRequest));

        return authenticated;
    }

    /// <summary>
    /// Regenerates an access token if it has expired.
    /// </summary>
    private void RegenerateAcessToken()
    {
        if(tokenResponse == null) { return; }

        TokenData tokenToRegenerate = new TokenData();
        tokenToRegenerate.token = tokenResponse.accessToken;

        var postRequest = CreateRequest(serverUrl + "/users/token", RequestType.POST, tokenToRegenerate);

        postRequest.SetRequestHeader("Cookie", tokenResponse.refreshToken);

        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log("Error: " + error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {

            tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(text);
            string accessToken = tokenResponse.accessToken;
        }, postRequest));
    }

    public void LoadUserExercises()
    {
        if (tokenResponse == null)
        {
            Debug.Log("user not logged in!");
            return;
        }
        AuthenticateAccessToken();

        var GetRequest = CreateRequest(serverUrl + "/exercises/list", RequestType.GET);

        GetRequest.SetRequestHeader("authorization", $"Bearer {tokenResponse.accessToken}");

        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log(error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {
            string json = text;
            List<ExerciseItem> exercises = JsonConvert.DeserializeObject<List<ExerciseItem>>(json);

            //for changed sort type in sorting dropdown;
            if (sortType == 1)
            {
                exercises = exercises.OrderBy(f => f.date).ToList();
            }
            else if (sortType == 2)
            {
                exercises = exercises.OrderBy(f => f.key).ToList();
            }
            else
            {
                exercises = exercises.OrderBy(f => f.key).ToList();
            }

            foreach (var exercise in exercises)
            {
                GameObject recordingButton = Instantiate(RecordingButtonCloudTemplate, RecordingItems, false) as GameObject;
                TMP_Text[] textComponents = recordingButton.GetComponentsInChildren<TMP_Text>();
                textComponents[0].text = exercise.key;
                textComponents[1].text = exercise.date;
                textComponents[2].text = exercise.exercise;

                Button buttonComponent = recordingButton.GetComponent<Button>();

                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => AuthenticateAccessToken());
                    //buttonComponent.onClick.AddListener(() => GetPatientRecordings(patient.fld_p_number));
                }

                Button uploadButton = recordingButton.transform.Find("UploadButton").GetComponent<Button>();

                if (uploadButton != null)
                {
                    uploadButton.onClick.AddListener(() => TryDownload(exercise.exercise, exercise.key));
                }

            }
            GameObject buttonControllerObject = GameObject.Find("ButtonController");//buttoncontroller 
            SetScroll setScroll = buttonControllerObject.GetComponent<SetScroll>();
            setScroll.SetScrollArea();
        }, GetRequest));
    }

    public void TryDownload(string key, string name)
    {
        if (tokenResponse == null)
        {
            Debug.Log("user not logged in!");
            return;
        }
        AuthenticateAccessToken();

        var GetRequest = CreateRequest(serverUrl + $"/exercise/{key}", RequestType.GET);

        GetRequest.SetRequestHeader("authorization", $"Bearer {tokenResponse.accessToken}");

        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log(error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {
            // Save to file

            Debug.Log(mainPath);
            string filePath = Path.Combine(mainPath, $"{name}.csv");
            File.WriteAllBytes(filePath, GetRequest.downloadHandler.data);

            Debug.Log("File downloaded to: " + filePath);
        }, GetRequest));

    }

    public void TryUpload(byte[] fileData, string fileName)
    {
        if (tokenResponse == null)
        {
            Debug.Log("user not logged in!");
            return;
        }
        AuthenticateAccessToken();

        UserExerciseData exerciseData = new UserExerciseData();
        exerciseData.exercisename = fileName;

        WWWForm form = new WWWForm();
        form.AddBinaryData("exercise", fileData, fileName, "text/csv");
        form.AddField("userData", JsonUtility.ToJson(exerciseData));


        var postRequest = UnityWebRequest.Post(serverUrl + "/uploadcsv", form);


        postRequest.SetRequestHeader("authorization", $"Bearer {tokenResponse.accessToken}");
        //postRequest.SetRequestHeader("Content-Type", "multipart/form-data");

        StartCoroutine(GeneralRequestCoroutine((string error) =>
        {
            Debug.Log(error);
        }, (string text, Dictionary<string, string> responseHeaders) =>
        {
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
                    buttonComponent.onClick.AddListener(() => AuthenticateAccessToken());
                    //buttonComponent.onClick.AddListener(() => GetPatientRecordings(patient.fld_p_number));
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
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }


    //NON WEB

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

    //Resets the input fields after logging in.
    private void ResetInputText()
    {
        GameObject.Find("UsernameInput").GetComponent<TMP_InputField>().text = string.Empty;
        GameObject.Find("PasswordInput").GetComponent<TMP_InputField>().text = string.Empty;
    }

    public void changeSortType(int newSortType)
    {
        sortType = newSortType;
    }

    public void changeDefaultPath(string path)
    {
        mainPath = path;
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

public class UserRegisterData
{
    public string email;
    public string pass;
    public bool provider;
    public string name;
}


[Serializable]
//user login data.
public class UserLoginData
{
    public string email;
    public string pass;
}

public class UserExerciseData
{
    public string exercisename;
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

public class CookieData
{
    public string cookie;
}

//unused i think
public class PostResult
{
    public string success { get; set; }
}
public class ExerciseItem
{
    public string key;
    public string date;
    public string exercise;
}
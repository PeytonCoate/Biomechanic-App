using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DTSceneSwap : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadDTScene() {
        SceneManager.LoadSceneAsync("DigitalTwin");
    }
}

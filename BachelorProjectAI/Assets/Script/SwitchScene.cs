using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public string newSceneName;


    public void ChangeScene()
    {
        SceneManager.LoadScene(newSceneName);
    }

}

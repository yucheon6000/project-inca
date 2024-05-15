using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        Invoke(nameof(_LoadGameScene), 3);
    }

    private void _LoadGameScene()
    {
        SceneManager.LoadScene("Game Scene", LoadSceneMode.Additive); ;
    }
}

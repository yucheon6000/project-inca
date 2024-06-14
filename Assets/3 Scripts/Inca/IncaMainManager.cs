using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inca
{
    public class IncaMainManager : IncaManager
    {
        [SerializeField]
        private string detectedWorldSceneName;

        private void Awake()
        {
            AddDetectedWorldScene();
        }

        private void AddDetectedWorldScene()
        {
            SceneManager.LoadScene(detectedWorldSceneName, LoadSceneMode.Additive);
        }
    }
}

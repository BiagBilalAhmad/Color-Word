using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
   public void GameScene()
    {
        SceneManager.LoadScene(1);
    }

    public void HardGameScene()
    {
        SceneManager.LoadScene(2);
    }
}

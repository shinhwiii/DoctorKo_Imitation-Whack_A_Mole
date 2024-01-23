using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickEvent : MonoBehaviour
{
    public void SceneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_ANDROID
                Application.Quit();
        #elif UNITY_IOS
                Application.Quit();
        #endif
    }
}

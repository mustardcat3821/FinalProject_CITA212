using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(() => LoadSceneByName("Title Screen"));
    }

    private void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(() => LoadSceneByName("Title Screen"));
    }
}


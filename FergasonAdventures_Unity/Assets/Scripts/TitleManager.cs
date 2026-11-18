using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(() => LoadSceneByName("Level 1"));
    }

    private void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(() => LoadSceneByName("Level 1"));
    }
}


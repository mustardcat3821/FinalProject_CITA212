using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 1f;

    bool isLoading = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isLoading) { return; }

        isLoading = true;
        StartCoroutine(LoadNextLevel());
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(levelLoadDelay);

        string currentSceneName = SceneManager.GetActiveScene().name;
        string nextSceneName = "";

        if (currentSceneName.StartsWith("Level "))
        {
            string levelText = currentSceneName.Substring(6);

            if (int.TryParse(levelText, out int levelNumber))
            {
                nextSceneName = "Level " + (levelNumber + 1);
            }
        }

        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ScenePersist scenePersist = FindFirstObjectByType<ScenePersist>();

            if (scenePersist != null)
            {
                scenePersist.ResetScenePersist();
            }

            SceneManager.LoadScene("End Screen");
        }
    }
}
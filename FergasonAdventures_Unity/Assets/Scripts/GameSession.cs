using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] int playerScore = 0;
    [SerializeField] AudioClip gameSessionMusic;

    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;

    AudioSource musicSource;

    void Awake()
    {
        int gameSessionCount = FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length;

        if (gameSessionCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        livesText.text = "Lives: " + playerLives.ToString();
        scoreText.text = "Score: " + playerScore.ToString();

        musicSource = GetComponent<AudioSource>();
        musicSource.clip = gameSessionMusic;
        musicSource.Play();
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives >= 1)
        {
            TakeLife();
        }
        else
        {
            musicSource.Stop();
            StartCoroutine(WaitAndResetSession(2f));
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        playerScore += pointsToAdd;
        scoreText.text = "Score: " + playerScore.ToString();
    }

    private void TakeLife()
    {
        playerLives = playerLives - 1;
        musicSource.Stop();
        StartCoroutine(WaitAndReloadScene(2f));
        livesText.text = "Lives: " + playerLives.ToString();
    }

    private void ResetGameSession()
    {
        musicSource.Stop();
        FindFirstObjectByType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    IEnumerator WaitAndReloadScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        musicSource.Play();
    }

    IEnumerator WaitAndResetSession(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ResetGameSession();
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutSceneController : MonoBehaviour
{
    [SerializeField] public SceneField nextScene;

    [Header("Fade effect")]
    public Image fadeImage;
    public float fadeDuration = 2.5f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ExecuteMoveNextScene()
    {
        StartCoroutine(MoveNextSceneCoroutine());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInCoroutine());
    }

    public IEnumerator MoveNextSceneCoroutine()
    {
        yield return StartCoroutine(FadeOutCoroutine());

        SceneManager.LoadScene(nextScene);
    }

    public IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;

        Color fadeColor = fadeImage.color;
        
        while (fadeColor.a > 0.01f)
        {
            elapsedTime += Time.deltaTime;

            fadeColor.a = Mathf.Max(1f - elapsedTime / fadeDuration, 0f);
            fadeImage.color = fadeColor;
            
            // Debug.Log($"alpha : {fadeColor.a}, elapsedTime : {elapsedTime} sec");

            yield return null;
        }

        fadeColor.a = 0f;
        fadeImage.color = fadeColor;

        yield break;
    }

    public IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;

        Color fadeColor = fadeImage.color;

        while (fadeColor.a < 0.95f)
        {
            elapsedTime += Time.deltaTime;

            fadeColor.a = Mathf.Clamp(elapsedTime / fadeDuration, 0f, 1f);
            fadeImage.color = fadeColor;

            yield return null;
        }

        fadeColor.a = 1f;
        fadeImage.color = fadeColor;

        yield break;
    }



}

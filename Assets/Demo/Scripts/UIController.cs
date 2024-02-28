using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIController : MonoBehaviour
{
    [Header("Input")]
    public InputActionData menuEvent;
    public InputActionData cancelEvent;

    [Header("Settings")]
    public Image background;
    public float backgroundBlur = 25f;
    public float fadeTime = 0.1f;

    GameCore m_gameCore;
    Coroutine m_fadeRoutine;
    
    
    public void InjectDependencies(GameCore gameCore)
        => m_gameCore = gameCore;

    public void Init()
    {
        m_gameCore.OnSwitchGamePause += OnSwitchGamePause;

        InputObserverTool.Bind(ref menuEvent);
        InputObserverTool.Bind(ref cancelEvent);

        background.material = Instantiate(background.material);
        
        background.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (m_gameCore.IsPaused && (menuEvent.IsStarted || cancelEvent.IsStarted))
        {
            m_gameCore.SwitchGamePause();
        }
    }

    private void OnSwitchGamePause(bool isPaused)
    {
        gameObject.SetActive(true);
        if (m_fadeRoutine != null) StopCoroutine(m_fadeRoutine);
        m_fadeRoutine = StartCoroutine(IEFadeBackground(isPaused));
    }

    private IEnumerator IEFadeBackground(bool isPaused)
    {
        var mat = background.material;
        int radius_id = Shader.PropertyToID("_Radius");

        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / fadeTime)
        {
            float tv = isPaused ? t : (1f - t);
            mat.SetFloat(radius_id, tv * backgroundBlur);
            yield return null;
        }
        
        mat.SetFloat(radius_id, isPaused ? backgroundBlur : 0f);
        if (!isPaused) gameObject.SetActive(false);
        m_fadeRoutine = null;
    }
};

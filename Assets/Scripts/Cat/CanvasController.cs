using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] Canvas guiCanvas;
    [SerializeField] Canvas endScreenCanvas;
    [SerializeField] Canvas levelPickCanvas;
    
    [SerializeField] Button restartButton;
    [SerializeField] Button closeLevelsButton;
    [SerializeField] Animator endScreenAnimator;
    [SerializeField] Animator levelScreenAnimator;
    [SerializeField] Canvas fadeCanvas;
    [SerializeField] Animator fadeAnimator;
    [SerializeField] CharacterController2D cat;
    [SerializeField] TextMeshProUGUI prompt;
    void Start()
    {
        guiCanvas.gameObject.SetActive(true);
        levelPickCanvas.gameObject.SetActive(false);
        endScreenCanvas.gameObject.SetActive(false);
        restartButton.onClick.AddListener(restartLevel);
        closeLevelsButton.onClick.AddListener(closeMenu);
    }

    public IEnumerator start()
    {
        yield return new WaitForSeconds(1);
    }

    public void loadLevel(string levelName)
    {
        //Debug.Log(levelName);
        StartCoroutine(loadLevelWaiter(levelName));
        //SceneManager.LoadScene(levelName);
    }

    public IEnumerator loadLevelWaiter(string levelName)
    {
        fadeAnimator.SetTrigger("FadeIN");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(levelName);
    }

    public void restartLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //StartCoroutine(loadLevelWaiter(SceneManager.GetActiveScene().name));
        StartCoroutine(restartLevelWaiter());
    }

    public IEnumerator restartLevelWaiter()
    {
        fadeAnimator.SetTrigger("FadeIN");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void closeMenu()
    {
        guiCanvasActive();
    }

    public void levelCanvasActive()
    {
        guiCanvas.gameObject.SetActive(false);
        levelPickCanvas.gameObject.SetActive(true);
        endScreenCanvas.gameObject.SetActive(false);
        cat.SetInteracting(true);
        levelScreenAnimator.SetTrigger("open");
    }

    public void guiCanvasActive()
    {
        guiCanvas.gameObject.SetActive(true);
        levelPickCanvas.gameObject.SetActive(false);
        endScreenCanvas.gameObject.SetActive(false);
        cat.SetInteracting(false);
    }

    public void endScreenCanvasActive()
    {
        levelPickCanvas.gameObject.SetActive(false);
        guiCanvas.gameObject.SetActive(false);
        endScreenCanvas.gameObject.SetActive(true);
        endScreenAnimator.SetTrigger("open");
    }

    public void EnablePrompt(string ptext)
    {
        prompt.gameObject.SetActive(true);
        prompt.text = ptext;
    }
    public void DisablePrompt()
    {
        prompt.text = "";
        prompt.gameObject.SetActive(false);
    }

}

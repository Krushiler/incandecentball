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
    [SerializeField] Canvas levelFinishCanvas;
    
    [SerializeField] Button restartButton;
    [SerializeField] Button closeLevelsButton;
    [SerializeField] Button nextLevelButton;
    [SerializeField] Animator endScreenAnimator;
    [SerializeField] Animator levelScreenAnimator;
    [SerializeField] Animator finishLevelAnimator;
    [SerializeField] Canvas fadeCanvas;
    [SerializeField] Animator fadeAnimator;
    [SerializeField] CharacterController2D cat;
    [SerializeField] TextMeshProUGUI prompt;

    int levelNumber;
    bool isFinalLevel;
    string levelSection;

    void Start()
    {
        showCanvas(guiCanvas);
        restartButton.onClick.AddListener(restartLevel);
        closeLevelsButton.onClick.AddListener(closeMenu);
        
        levelNumber = cat.GetLevelNumber();
        isFinalLevel = cat.GetLevelIsFinal();
        levelSection = cat.GetLevelSection();

        nextLevelButton.onClick.AddListener(delegate
        {
            if (!isFinalLevel)
            {
                loadLevel(levelSection + (levelNumber+1).ToString());
            }
            else
            {
                loadLevel("Home");
            }
        });
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
        showCanvas(levelPickCanvas);
        cat.SetInteracting(true);
        levelScreenAnimator.SetTrigger("open");
    }

    public void guiCanvasActive()
    {
        showCanvas(guiCanvas);
        cat.SetInteracting(false);
    }

    public void endScreenCanvasActive()
    {
        showCanvas(endScreenCanvas);
        endScreenAnimator.SetTrigger("open");
    }

    public void levelFinishCanvasActive()
    {
        showCanvas(levelFinishCanvas);
        finishLevelAnimator.SetTrigger("open");

    }

    void showCanvas(Canvas can)
    {
        guiCanvas.gameObject.SetActive(true);
        levelPickCanvas.gameObject.SetActive(false);
        endScreenCanvas.gameObject.SetActive(false);
        levelFinishCanvas.gameObject.SetActive(false);
        can.gameObject.SetActive(true);
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

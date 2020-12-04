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
    [SerializeField] Button restartButton1;
    [SerializeField] Button closeLevelsButton;
    [SerializeField] Button nextLevelButton;
    [SerializeField] Button homeButton;
    [SerializeField] Button homeButton1;
    [SerializeField] Animator endScreenAnimator;
    [SerializeField] Animator levelScreenAnimator;
    [SerializeField] Animator finishLevelAnimator;
    [SerializeField] Canvas fadeCanvas;
    [SerializeField] Animator fadeAnimator;
    [SerializeField] CharacterController2D cat;
    [SerializeField] TextMeshProUGUI prompt;
    [SerializeField] Text timeText;
    [SerializeField] Text timeRecordText;
    

    int levelNumber;
    bool isFinalLevel;
    string levelSection;

    void Start()
    {
        showCanvas(guiCanvas);
        
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
        homeButton.onClick.AddListener(delegate
        {
            loadLevel("Home");
        });
        homeButton1.onClick.AddListener(delegate
        {
            loadLevel("Home");
        });
        restartButton.onClick.AddListener(delegate { restartLevel(); });
        restartButton1.onClick.AddListener(delegate { restartLevel(); });
    }

    public IEnumerator start()
    {
        yield return new WaitForSeconds(1);
    }

    public void setTime(float _time, float _timeRecord)
    {
        timeText.text = "Время: " + _time.ToString("0.00") + " сек";
        timeRecordText.text = "Рекорд: " + _timeRecord.ToString("0.00") + " сек";
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
        
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        guiCanvas.gameObject.SetActive(false);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq.Expressions;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] CanvasController canvasController;
    public GameObject levelHolder;
    public GameObject levelIconGreen;
    public GameObject levelIconRed;
    public GameObject levelIconGray;
    public GameObject thisCanvas;
    public int numberOfLevels = 50;
    public Vector2 iconSpacing;
    private Rect panelDimensions;
    private Rect iconDimensions;
    private int amountPerPage;
    private int currentLevelCount;

    int completedLevels = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("completedLevels"))
        {
            completedLevels = PlayerPrefs.GetInt("completedLevels");
        }
        panelDimensions = levelHolder.GetComponent<RectTransform>().rect;
        iconDimensions = levelIconGreen.GetComponent<RectTransform>().rect;
        int maxInARow = Mathf.FloorToInt((panelDimensions.width + iconSpacing.x) / (iconDimensions.width + iconSpacing.x));
        int maxInACol = Mathf.FloorToInt((panelDimensions.height + iconSpacing.y) / (iconDimensions.height + iconSpacing.y));
        amountPerPage = maxInARow * maxInACol; 
        int totalPages = Mathf.CeilToInt((float)numberOfLevels / amountPerPage);
        LoadPanels(totalPages);
    }
    void LoadPanels(int numberOfPanels)
    {
        GameObject panelClone = Instantiate(levelHolder) as GameObject;
        //PageSwiper swiper = levelHolder.AddComponent <PageSwiper>();
        PageSwiper swiper = PageSwiper.CreateComponent(levelHolder, panelDimensions.width);
        swiper.totalPages = numberOfPanels;

        for (int i = 1; i <= numberOfPanels; i++)
        {
            GameObject panel = Instantiate(panelClone) as GameObject;
            panel.transform.SetParent(thisCanvas.transform, false);
            panel.transform.SetParent(levelHolder.transform);
            panel.name = "Page-" + i;
            panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (i - 1), 0);
            SetUpGrid(panel);
            int numberOfIcons = i == numberOfPanels ? numberOfLevels - currentLevelCount : amountPerPage;
            LoadIcons(numberOfIcons, panel);
        }
        Destroy(panelClone);
    }
    void SetUpGrid(GameObject panel)
    {
        GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(iconDimensions.width, iconDimensions.height);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.spacing = iconSpacing;
    }

    int iconNum = 1;
    List<Button> btns = new List<Button>();

    void LoadIcons(int numberOfIcons, GameObject parentObject)
    {
        for (int i = 1; i <= numberOfIcons; i++)
        {
            currentLevelCount++;
            int x = iconNum;
            GameObject icon;
            if (completedLevels + 1 >= x)
            {
                if (completedLevels >= x)
                {
                    if (PlayerPrefs.GetInt("LevelCat" + x + "AllMoney") > 0)
                    {
                        icon = Instantiate(levelIconGreen) as GameObject;
                    }
                    else
                    {
                        icon = Instantiate(levelIconRed) as GameObject;
                    }
                }
                else
                {
                    icon = Instantiate(levelIconRed) as GameObject;
                }
            }
            else
            {
                icon = Instantiate(levelIconGray) as GameObject;
            }
            icon.transform.SetParent(thisCanvas.transform, false);
            icon.transform.SetParent(parentObject.transform);
            icon.name = "Level " + x;
            icon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(x.ToString());
            btns.Add(icon.GetComponent<Button>());
            if (completedLevels + 1 >= x)
            {
                btns[x - 1].onClick.AddListener(delegate
                {
                    canvasController.loadLevel("LevelCat" + x.ToString());
                });
            }
            iconNum++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
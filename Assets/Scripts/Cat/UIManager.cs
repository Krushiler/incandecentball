using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas catCanvas;
    [SerializeField] Text money;
    [SerializeField] RectTransform moneyImageTransform;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateMoney(int _money, int needMoney, string levelSection)
    {
        if (levelSection != "Home")
        {
            money.text = _money.ToString() + "/" + needMoney.ToString();
        }
        else
        {
            if (!PlayerPrefs.HasKey("Money"))
            {
                PlayerPrefs.SetInt("Money", 0);
            }
            money.text = PlayerPrefs.GetInt("Money").ToString();
            
        }
    }

    public Vector2 GetMoneyPos(Camera playerCamera)
    {
        Vector2 imgPos = (Vector2)playerCamera.ScreenToWorldPoint(moneyImageTransform.transform.position) - new Vector2(0.5f, 0.5f);
        return imgPos;
    }

}

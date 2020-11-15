using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropsUIController : MonoBehaviour {

    public Button btnBuy;
    public Button btnUse;
    public Button btnCancleUse;

    public GameObject objCommonCard;
    public GameObject objFindErrorCard;
    public GameObject objlimitTypeCard;

    public Text txtScore;

    // Use this for initialization
    void Start () {
        RefreshPanel();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RefreshPanel()
    {
        txtScore.text = "我的积分：" + GameManager.Instance.MyScore;

        //根据是否拥有道具卡显示按钮和卡片信息
        btnBuy.gameObject.SetActive(GameManager.Instance.MyCardType == ePropsCardType.none);
        btnUse.gameObject.SetActive(GameManager.Instance.MyCardType != ePropsCardType.none && !GameManager.Instance.bComCardUseState);
        btnCancleUse.gameObject.SetActive(GameManager.Instance.MyCardType == ePropsCardType.Common && GameManager.Instance.bComCardUseState);
        objCommonCard.SetActive(GameManager.Instance.MyCardType == ePropsCardType.Common);
        objFindErrorCard.SetActive(GameManager.Instance.MyCardType == ePropsCardType.FindError);
        objlimitTypeCard.SetActive(GameManager.Instance.MyCardType == ePropsCardType.LimitType);

        //如果当前没有道具卡并且积分大于30，才激活购买按钮
        btnBuy.interactable = GameManager.Instance.MyCardType == ePropsCardType.none && GameManager.Instance.MyScore >= 30;

        //万能卡颜色显示：如果是使用状态就显示绿色
        objCommonCard.GetComponent<Image>().color = GameManager.Instance.bComCardUseState ? Color.green : Color.white;
    }

    /// <summary>
    /// 购买按钮：消耗30积分购买道具卡，随机一个
    /// </summary>
    public void OnClickBtnBuy()
    {
        GameManager.Instance.BuyPropsCard();
    }

    /// <summary>
    /// 使用按钮：购买后使用道具
    /// </summary>
    public void OnClickBtnUse()
    {
        GameManager.Instance.UsePropsCard();
    }
}

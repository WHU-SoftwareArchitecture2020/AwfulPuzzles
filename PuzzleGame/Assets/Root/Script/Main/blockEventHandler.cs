using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 图片类型
/// </summary>
public enum eImageType
{
    none,
    game,   //显示在拼图游戏区的
    factory //显示在生产区的
}

[RequireComponent(typeof(Image))] //确保gameObject拥有Image组件
[RequireComponent(typeof(EventTrigger))] //确保gameObject拥有EventTrigger组件

/// <summary>
/// 拼图块事件处理器
/// </summary>
public class blockEventHandler : MonoBehaviour {
    /// <summary>
    /// 万能卡提示显示
    /// </summary>
    public GameObject objComCardTips;

    /// <summary>
    /// 默认游戏区的，这个值在预设或者UI上直接设置了
    /// </summary>
    public eImageType imageType = eImageType.game;

    /// <summary>
    /// 图片方块的图片信息
    /// </summary>
    private ImageInfo imageInfo;

    /// <summary>
    /// 拼图块的Image组件
    /// </summary>
	public Image image;

    /// <summary>
    /// 拼图块的EventTrigger组件
    /// </summary>
	EventTrigger eventTrigger;

	// Use this for initialization
	void Awake () {
        //获取Image组件
		image = gameObject.GetComponent<Image>();

        //获取EventTrigger组件
		eventTrigger = gameObject.GetComponent<EventTrigger>();

        //实例化Entry类型触发器
		EventTrigger.Entry entry = new EventTrigger.Entry();

        //设置监听事件类型为点击
		entry.eventID = EventTriggerType.PointerClick;

        //使用匿名函数为entry触发器添加监听回调函数OnPointerDownDelegate
		entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });

        //把entry触发器添加到 EventTrigger组件上，动态为该gameobject添加点击事件监听
        eventTrigger.triggers.Add(entry);
	}

    /// <summary>
    /// 拼图块的点击响应逻辑
    /// </summary>
    /// <param name="data"></param>
	void OnPointerDownDelegate(PointerEventData data) {
        //如果是万能卡使用状态中
        if (GameManager.Instance.bComCardUseState)
        {
            //点击的是拼图区的图
            if (imageType == eImageType.game)
            {
                int b = int.Parse(gameObject.name);
                GameManager.Instance.UserComCard(b);
            }
            else
            {
                //不允许在万能卡使用过程中点击其他区域的卡牌
                return;
            }
        }

        //to do onclick
        if (!GameManager.Instance.isHold) { //判断游戏管理器中 没有拼图块处于选中状态
            SetHoldState();
        }
		else {  //已有拼图块选中

            //取消之前选中拼图块的提示颜色
			GameManager.Instance.HoldingObject.GetComponent<Image>().color = Color.white;

			if (GameManager.Instance.HoldingObject == gameObject) {//判断已选中拼图块是不是自己
				Debug.Log("Putdown: Block" + gameObject.name);

			}
			else {  //已选中的拼图块不是自己
                //如果点击的都是拼图区域的
                if (imageType == eImageType.game && GameManager.Instance.CurHoldImageType == imageType)
                {
                    int a = int.Parse(GameManager.Instance.HoldingObject.name);
                    int b = int.Parse(gameObject.name);
                    Debug.Log("Switch Block: " + a + " and " + b);
                    GameManager.Instance.SwitchBlock(a, b);  //交换拼图块
                }
				else if (imageType == eImageType.factory && GameManager.Instance.CurHoldImageType == imageType)
                {
                    //如果点击的都是生产区的图片，那么换一张处于选中状态并返回
                    SetHoldState();
                    return;
                }
                else if (imageType == eImageType.game && GameManager.Instance.CurHoldImageType == eImageType.factory)
                {
                    //先点击的生产区域的图片，后点击拼图区的图片：需要将生产区的图片显示下来到拼图区
                    int b = int.Parse(gameObject.name);
                    ImageInfo factoryImageInfo = GameManager.Instance.HoldingObject.GetComponent<blockEventHandler>().GetImageInfo();
                    GameManager.Instance.FactoryToGame(factoryImageInfo, b);  //下放图块
                }
                else if (imageType == eImageType.factory && GameManager.Instance.CurHoldImageType == eImageType.game)
                {
                    //先点击的拼图区域的图片，后点击生产区的图片：不允许反向替换，重新设置选中兑现为生产区的，直接返回
                    SetHoldState();
                    return;
                }
            }

            SetNoHoldState();
        }
    }

    //设置选中状态的一些情况
    void SetHoldState()
    {
        Debug.Log("Pickup Block: " + gameObject.name);
        GameManager.Instance.isHold = true; //设置拼图选中标识为真
        GameManager.Instance.HoldingObject = gameObject;    //保存选中拼图块的引用
        GameManager.Instance.CurHoldImageType = imageType;
        if(image.sprite != null)
        {
            GameManager.Instance.SetBtnThrowInteractable(true);
        }
        image.color = Color.green;  //通过设置颜色表示拼图块被选中
    }

    void SetNoHoldState()
    {
        GameManager.Instance.isHold = false;    //取消选择拼图块标识
        GameManager.Instance.HoldingObject = null;  //设置选中块为空
        GameManager.Instance.CurHoldImageType = eImageType.none;
        GameManager.Instance.SetBtnThrowInteractable(false);
    }

    public void SetImageInfoAndShow(ImageInfo imageInfo)
    {
        gameObject.SetActive(true);
        this.imageInfo = imageInfo;
        objComCardTips.SetActive(imageInfo.isCommonCard);
        image.sprite = GameManager.Instance.GetSpriteByImageInfo(imageInfo);
    }

    public ImageInfo GetImageInfo()
    {
        return imageInfo;
    }

    public void InitImageInfo()
    {
        imageInfo = null;
        image.sprite = null;
        objComCardTips.SetActive(false);
    }
}

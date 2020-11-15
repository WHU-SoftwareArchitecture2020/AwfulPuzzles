using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyLevelEventHandler : MonoBehaviour {

    /// <summary>
    /// 本关卡是否解锁
    /// </summary>
    public bool IsUnloack;

    /// <summary>
    /// 锁图标
    /// </summary>
    public GameObject objLock;

    /// <summary>
    /// 最高得分显示
    /// </summary>
    public Text txtScore;

    /// <summary>
    /// UI的RectTransform
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// 难度确定按钮
    /// </summary>
    public Button btn_ok;

    /// <summary>
    /// 关卡名称
    /// </summary>
    public string levelName;

    /// <summary>
    /// 难度等级
    /// </summary>
    public int level = 0;

    /// <summary>
    /// 点击消息处理委托
    /// </summary>
    /// <param name="str"></param>
    public delegate void OnClickHandler(string str);

    /// <summary>
    /// 点击事件
    /// </summary>
    public static event OnClickHandler OnClickEvent;

    /// <summary>
    /// 难度UI的点击响应
    /// </summary>
    public void onClick()
    {
        if (!IsUnloack)
        {
            btn_ok.interactable = false;
            Debug.LogFormat("{0}还未解锁", levelName);
            return;
        }
        btn_ok.interactable = true;		//只有选择了难度等级才不禁用确认按钮

        if (OnClickEvent != null)
        {		//判读点击事件的委托(通俗讲就是分发)列表是否为空
            OnClickEvent(levelName);	//执行(可以理解为分发)点击事件(该事件下的委托函数都会被调用)
        }
    }

    /// <summary>
    /// 点击响应的处理逻辑
    /// </summary>
    /// <param name="str"></param>
    void ReceiveClickMessage(string str)
    {
        if (str == levelName)
        {	//通过levelName判断当前接受事件的对象是否为目标对象
            //当前对象为被点击对象时

            rectTransform.localScale = new Vector3(1.25f, 1.25f, 1);	//修改UI大小

            DataManager.Instance.Current_Level = level;		//设置数据管理器的当前难度等级
            Debug.Log("Current Level:" + DataManager.Instance.Current_Level);
        }
        else
        {
            //当前对象不为被点击对象时
            rectTransform.localScale = Vector3.one;		//复原UI大小
        }
    }

    void Start()
    {
        //获取 RectTransform 组件
        rectTransform = gameObject.GetComponent<RectTransform>();

        //注册点击事件处理函数 (就是把当前对象的点击响应的处理逻辑 加入点击事件的分发名单中)
        MyLevelEventHandler.OnClickEvent += ReceiveClickMessage;

        //刷新关卡显示
        RefreshLevelInfo();
    }

    /// <summary>
    /// 销毁对象时执行
    /// </summary>
    void OnDestroy()
    {
        //解除点击事件处理函数
        MyLevelEventHandler.OnClickEvent -= ReceiveClickMessage;
    }

    /// <summary>
    /// 刷新关卡显示
    /// </summary>
    void RefreshLevelInfo()
    {
        List<Mission> missionData = DataManager.Instance.LoadMissions();	//读取本地xml中的关卡数据
        Mission curData = missionData[level];
        IsUnloack = curData.UnLock;
        objLock.SetActive(!IsUnloack);
        txtScore.gameObject.SetActive(IsUnloack);
        if (IsUnloack)
        {
            txtScore.text = curData.Score.ToString();
        }
    }
}

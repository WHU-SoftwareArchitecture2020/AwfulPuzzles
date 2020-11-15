using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 游戏难度UI 的事件处理器
/// </summary>
public class LevelEventHandler : MonoBehaviour {

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
    public void onClick() {
        btn_ok.interactable = true;		//只有选择了难度等级才不禁用确认按钮

        if (OnClickEvent != null) {		//判读点击事件的委托(通俗讲就是分发)列表是否为空
            OnClickEvent(levelName);	//执行(可以理解为分发)点击事件(该事件下的委托函数都会被调用)
        }
    }

    /// <summary>
    /// 点击响应的处理逻辑
    /// </summary>
    /// <param name="str"></param>
    void ReceiveClickMessage(string str) {
        if (str == levelName) {	//通过levelName判断当前接受事件的对象是否为目标对象
            //当前对象为被点击对象时

            rectTransform.localScale = new Vector3(2, 2, 1);	//修改UI大小

            DataManager.Instance.Current_Level = level;		//设置数据管理器的当前难度等级
            Debug.Log("Current Level:" + DataManager.Instance.Current_Level);
        }
        else {
            //当前对象不为被点击对象时

            rectTransform.localScale = Vector3.one;		//复原UI大小
        }
    }

    void Start() {

        //获取 RectTransform 组件
        rectTransform = gameObject.GetComponent<RectTransform>();

		//注册点击事件处理函数 (就是把当前对象的点击响应的处理逻辑 加入点击事件的分发名单中)
        LevelEventHandler.OnClickEvent += ReceiveClickMessage;
    }

    /// <summary>
    /// 销毁对象时执行
    /// </summary>
    void OnDestroy() {
        //解除点击事件处理函数
        LevelEventHandler.OnClickEvent -= ReceiveClickMessage;

    }
}

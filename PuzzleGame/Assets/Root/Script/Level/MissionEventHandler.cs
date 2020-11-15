using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 关卡UI 的事件处理器
/// </summary>
public class MissionEventHandler : MonoBehaviour {

	/// <summary>
	/// 事件触发器 用于动态绑定事件
	/// </summary>
    private EventTrigger eventTrigger;

	/// <summary>
	/// 关卡UI的数据处理器
	/// </summary>
    private MissionDataHandler missionDataHandler;

	/// <summary>
	/// 关卡UI的RectTransform
	/// </summary>
	private RectTransform rectTransform;

	/// <summary>
	/// 关卡选择确认按钮
	/// </summary>
    public Button btn_ok;

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
    /// 点击响应的处理逻辑
    /// </summary>
    /// <param name="str"></param>
    void ReceiveClickMessage(string name) {
		if (name == gameObject.name) {	//通过name判断当前接受事件的对象是否为目标对象
			Debug.Log(gameObject.name);

			if (missionDataHandler.IsUnlock) {	//如果点击中的关卡已经解锁
				btn_ok.interactable = true;		//确认按钮设置为可用
				rectTransform.localScale = new Vector3(1.5f, 1.5f, 1);	//设置关卡UI的缩放

				DataManager.Instance.Current_Mission = missionDataHandler.ID;	//设置数据管理器的当前关卡
			}
			else {  //如果点击的关卡尚未解锁
				btn_ok.interactable = false;      //禁用确认按钮
			}
		}
		else {  //当前接受事件的对象不为目标对象
			rectTransform.localScale = Vector3.one; //重置关卡UI的缩放
		}
    }

    /// <summary>
    /// 关卡UI的点击响应
    /// </summary>
    /// <param name="data"></param>
    void OnPointerDownDelegate(PointerEventData data) {
        if (OnClickEvent != null) {   //如果点击事件的委托函数列表不为空
            OnClickEvent(gameObject.name);    //执行点击事件(该事件下的委托函数都会被调用)
        }
    }

	// Use this for initialization
	void Start () {
        //注册点击事件处理函数
        MissionEventHandler.OnClickEvent += ReceiveClickMessage;

        #region  动态添加点击事件监听

        //获取EventTrigger 事件触发器组件
        eventTrigger = gameObject.GetComponent<EventTrigger>();

        //实例化 Entry类型的触发器
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;  //设置触发器事件类型为点击

        // 使用 匿名函数 为触发器添加监听的回调函数-> OnPointerDownDelegate
        entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        //把entry触发器添加到 EventTrigger组件上，动态为该gameobject添加点击事件监听
        eventTrigger.triggers.Add(entry);  

        #endregion

        //获取关卡数据处理器
        missionDataHandler = gameObject.GetComponent<MissionDataHandler>();

        //获取关卡UI的RectTransform
		rectTransform = gameObject.GetComponent<RectTransform>();
	}
	
    /// <summary>
    /// 对象销毁时执行
    /// </summary>
    void OnDestroy() {
        //解除点击事件处理函数
        MissionEventHandler.OnClickEvent -= ReceiveClickMessage;
    }

}

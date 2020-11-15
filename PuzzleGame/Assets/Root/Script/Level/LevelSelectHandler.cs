using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏难度选择 处理器
/// </summary>
public class LevelSelectHandler : MonoBehaviour {

	/// <summary>
	/// 难度关卡选择面板的RectTransform
	/// </summary>
	private RectTransform rectTransform;

	/// <summary>
	/// 关卡UI 的集合
	/// </summary>
	private GameObject[] missionCollections;

	/// <summary>
	/// 返回难度选择
	/// </summary>
	public void onMissionBack() {
		t = 0f;		//重置计时器
		direction = 0;	//难度关卡选择面板的滑动方向 0-往右
		isMove = true;	//开始滑动

		DataManager.Instance.Current_Mission=0;	//清除当前选中的关卡为默认值0

		for (int i = 0; i < 10; i++) {	//重置这10个关卡UI的RectTransform的缩放
			var temp = missionCollections[i].GetComponent<RectTransform>();
			temp.localScale = Vector3.one;
		}

	}

	/// <summary>
	/// 当确认难度选择时执行
	/// </summary>
	public void onLevelSelected() {
		t = 0f;		//重置计时器	
		direction = 1;	//难度关卡选择面板的滑动方向		1-往左

		var missionData = DataManager.Instance.LoadMissions();	//读取本地xml中的关卡数据

		int current_level = DataManager.Instance.Current_Level;		//获取数据管理器的当前难度等级

		for (int i = 0; i < 10; i++) {	//设置这10个关卡UI的数据绑定
			var missionDataHandler = missionCollections[i].GetComponent<MissionDataHandler>();
			missionDataHandler.DataBind(missionData[10 * current_level+i]);
		}

		isMove = true;	//开始滑动
	}

	/// <summary>
	/// 滑动开始标识
	/// </summary>
	bool isMove = false;

	/// <summary>
	/// 滑动计时器
	/// </summary>
	float t = 0f;

	/// <summary>
	/// 滑动方向
	/// </summary>
	int direction = -1;

	/// <summary>
	/// 滑动目标向量 通过direction变量改变方向
	/// </summary>
	Vector3 targetPosition;

	void Update() {
		if (isMove) {	//如果滑动标识为真 开始滑动动画
			if (t < 1) {	//表示滑动动画持续1秒	综合效果表现为 1秒内向目标Position平滑移动

				//通过插值函数Lerp控制难度选择面板的位置，实现滑动效果
				rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, direction * targetPosition, t);

				t += Time.deltaTime;	//累加动画耗时
			}
			else {	//滑动动画到达1秒后
				isMove = false;		//停止滑动
			}
		}
	}

	void Start() {
		//获取难度关卡的RectTransform
		rectTransform = gameObject.GetComponent<RectTransform>();

		//初始化目标向量
		targetPosition = new Vector3(-1200, 0, 0);

		//保存10个关卡UI的引用到missionCollections数组中
		missionCollections = new GameObject[10];
		for (int i = 0; i < 10; i++) {
			string itemName = "MissionItem" + i.ToString();
			missionCollections[i] = GameObject.Find(itemName);	//根据名称查找GameObject
		}
	}
}

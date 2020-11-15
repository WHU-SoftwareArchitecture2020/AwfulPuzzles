using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 关卡UI的 数据处理器
/// </summary>
public class MissionDataHandler : MonoBehaviour {

	/// <summary>
	/// 关卡UI的未解锁图片
	/// </summary>
	public Sprite unlockImage;

	/// <summary>
	/// 关卡UI的解锁图片
	/// </summary>
	public Sprite lockImage;

	/// <summary>
	/// 关卡UI的Text组件 用于显示得分
	/// </summary>
	public Text score;

	/// <summary>
	/// 关卡UI的Image组件	用于显示解锁与未解锁的背景图片
	/// </summary>
	public Image image;

	/// <summary>
	/// 关卡UI的数据 ID
	/// </summary>
	public int ID;

	/// <summary>
	/// 关卡UI的数据 IsUnlock
	/// </summary>
	public bool IsUnlock = false;

	/// <summary>
	/// 关卡UI的数据 Score
	/// </summary>
	public int Score = 0;

	// Use this for initialization
	void Start () {
		//获取关卡UI的Image组件
		image = gameObject.GetComponent<Image>();
	}
	
	/// <summary>
	/// 关卡UI的数据绑定
	/// </summary>
	/// <param name="m">数据源</param>
	public void DataBind(Mission m) {
		ID = m.ID;	//设置ID
		IsUnlock = m.UnLock;	//设置是否解锁
		Score = m.Score;	//设置分数

		if (!IsUnlock) {	//如果关卡为未解锁
			image.sprite = lockImage;	//设置关卡UI的背景图为 锁
			score.enabled = false;	//不显示得分
		}
		else {
			image.sprite = unlockImage;	//设置关卡UI的背景图为 解锁
			score.enabled = true;	//显示得分

			switch (Score) {	//设置得分 0-3星
				case 1: score.text = "★☆☆"; break;		
				case 2: score.text = "★★☆"; break;
				case 3: score.text = "★★★"; break;
				default: score.text = "☆☆☆"; break;
			}
		}

	}
}

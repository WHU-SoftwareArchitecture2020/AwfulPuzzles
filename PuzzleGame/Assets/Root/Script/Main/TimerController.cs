using UnityEngine;
using System.Collections;

/// <summary>
/// 提示计时器控制器
/// </summary>
public class TimerController : MonoBehaviour {

    /// <summary>
    /// 计时条背景RectTransform
    /// </summary>
    public RectTransform backgroundRectTransform;

    /// <summary>
    /// 计时条RectTransform
    /// </summary>
    public RectTransform timerRectTransform;

    /// <summary>
    /// 计时条时长 10s
    /// </summary>
    public float time=10;

    /// <summary>
    /// 计时条缩放向量
    /// </summary>
    public Vector3 init_scale;

    void Start() {
        //设置缩放向量初始值
        init_scale = backgroundRectTransform.localScale;

        Init();//初始化
    }


    void Init() {
        //设置计时条UI的缩放
        timerRectTransform.localScale = init_scale;
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    public void StartTimerWork() {
        StartCoroutine(DoTimerWork());  //开启计时条UI显示协程

    }

    /// <summary>
    /// 计时条UI显示协程
    /// </summary>
    /// <returns></returns>
    IEnumerator DoTimerWork() {
        float t = time;     //设置计时累计时长
        while (t>0) {   //如果剩余时长大于0

            float temp_scaleX = t / time;   //计算缩放比例
            //Debug.Log(temp_scaleX);

            //设置计时条UI的缩放
            timerRectTransform.localScale=new Vector3(temp_scaleX,init_scale.y,init_scale.z);

            t -= Time.deltaTime;    //计算消耗时长

            yield return null; 
        }

        //循环结束即提示时长耗光，开始拼图游戏
        GameManager.Instance.StartGame();
    }
}

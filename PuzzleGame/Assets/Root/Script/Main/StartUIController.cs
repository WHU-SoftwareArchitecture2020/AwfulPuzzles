using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 开始UI控制器
/// </summary>
public class StartUIController : MonoBehaviour {

    /// <summary>
    /// 提示Image
    /// </summary>
    public GameObject tipImage;

    /// <summary>
    /// 提示计时器控制器
    /// </summary>
    public TimerController timerController;

    /// <summary>
    /// 设置提示图片
    /// </summary>
    /// <param name="sprite"></param>
    public void SetTipImage(Sprite sprite) {
        tipImage.GetComponent<Image>().sprite = sprite;
    }

    /// <summary>
    /// 开始提示
    /// </summary>
    public void StartTip() {
        tipImage.SetActive( true);
        timerController.StartTimerWork();
    }
}

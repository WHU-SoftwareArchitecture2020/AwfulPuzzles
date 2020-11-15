using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour {
    /// <summary>
    /// 步数计数文本
    /// </summary>
    public Text txtSecond;

    /// <summary>
    /// 没找到错误图片提示
    /// </summary>
    public GameObject objNotFuindTips;

    /// <summary>
    /// 统计秒数
    /// </summary>
    public int SecondCount { get; set; }

    //1秒节点
    private float timer1 = 0;

    //2秒节点
    private int timer2 = 0;

    /// <summary>
    /// 是否已经完成
    /// </summary>
    private bool isComplete = false;
    public bool IsComplete
    {
        set { isComplete = value; }
    }

    /// <summary>
    /// 产生的图片的父对象
    /// </summary>
    public Transform imagesParent;


    // Use this for initialization
    void Start () {
        HideFindTipsShow();
    }

    public void ShowNotFindTips()
    {
        objNotFuindTips.SetActive(true);
        Invoke("HideFindTipsShow", 2); 
    }

    void HideFindTipsShow()
    {
        objNotFuindTips.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (!isComplete)
        {
            //只允许显示8张，超过8张不生产了
            if (GameManager.Instance.GenerateImageCount >= 8 )
            {
                return;
            }

            timer1 += Time.deltaTime;
            if (timer1 >= 1)
            {
                SecondCount++;

                timer1 = 0;
                txtSecond.text = "已用时间：" + SecondCount.ToString();

                timer2++;
                if (timer2 >= 2)
                {
                    timer2 = 0;
                    //2秒生产一张图片
                    ImageInfo imageInfo = GameManager.Instance.GenerateImageInfo();
                    if (imageInfo == null)
                    {
                        Debug.LogError("2秒生产一张图片imageInfo == null");
                        return;
                    }
                    //找到没显示的第一个子物体显示
                    int activeCount = GameManager.Instance.GenerateImageCount;
                    Debug.Log("目前生产区显示的子物体数量为：" + activeCount);
                    Transform nextChild = imagesParent.GetChild(activeCount - 1);
                    nextChild.GetComponent<blockEventHandler>().SetImageInfoAndShow(imageInfo);
                }
            }
        }
    }
}

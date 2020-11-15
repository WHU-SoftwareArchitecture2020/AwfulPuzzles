using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 道具卡类型
/// </summary>
public enum ePropsCardType
{
    none,
    Common, //万能卡
    FindError,//找错卡
    LimitType //限定卡
}


/// <summary>
/// 图片方块的图片信息
/// </summary>
public class ImageInfo
{
    public int id = -1;//其在自己所在图集的id
    public bool isMission;//是否是任务图片
    public bool isCommonCard;//是否是万能卡
}

/// <summary>
/// 游戏管理器
/// </summary>
public class GameManager : MonoBehaviour {

	#region 暴露Instance便于引用
	private static GameManager _instance = null;
	public static GameManager Instance {
		get {
			return _instance;
		}
	}
	#endregion

	/// <summary>
	/// 游戏场景管理器
	/// </summary>
	public GameSceneManager gameSceneManager;

	/// <summary>
	/// 开始UI
	/// </summary>
	public GameObject StartUI;

	/// <summary>
	/// 主游戏UI
	/// </summary>
	public GameObject MainUI;
    private MainUIController mainUICtr;

	/// <summary>
	/// 结束UI
	/// </summary>
	public GameObject EndUI;

	/// <summary>
	/// 拼图块预制体
	/// </summary>
	public GameObject blockPrefab;

	/// <summary>
	/// 拼图GridLayoutGroup
	/// </summary>
	public GridLayoutGroup gamePanel;

	/// <summary>
	/// 拼图Panel Size
	/// </summary>
	public float gamePanelSize;

	/// <summary>
	/// 拼图区域的图片信息数组
	/// </summary>
	public ImageInfo[] CurImagesInfo;

	/// <summary>
	/// 拼图块UI集合
	/// </summary>
	public GameObject[] blockCollections;

    /// <summary>
    /// 丢弃按钮
    /// </summary>
    public Button btnThrow;

    /// <summary>
    /// 是否已选中拼图块
    /// </summary>
    public bool isHold { get; set; }

	/// <summary>
	/// 选中的拼图块引用
	/// </summary>
	public GameObject HoldingObject { get; set; }

    /// <summary>
    /// 目前生产区显示的图片数量，最多8张
    /// </summary>
    public int GenerateImageCount { get; set; }

    /// <summary>
    /// 记录不是本地碎片后的操作次数
    /// </summary>
    private int noLocalImageCount = 0;

    /// <summary>
    /// 记录4次生产中是否有过一次不是拼图区图片
    /// </summary>
    private bool isNotLoaclImage = false;

    /// <summary>
    /// 生产区信息列表
    /// </summary>
    private Dictionary<bool, List<ImageInfo>> generateImageInfo;
    
    /// <summary>
    /// 最近5张图片的信息：不允许5张之内出现同一张图片
    /// </summary>
    private ImageInfo[] lately5ImageInfo;

    /// <summary>
    /// 用于lately5ImageInfo里面的下标更新最新的数据
    /// </summary>
    private int latelyIndex = 0;

    /// <summary>
    /// 当前选中的方块的区域类型
    /// </summary>
    public eImageType CurHoldImageType { get; set; }

    /// <summary>
    /// 存储任务图片和混淆图片
    /// </summary>
    private Dictionary<bool, List<Sprite>> allSprite;

    /// <summary>
    /// 记录我的积分
    /// </summary>
    public int MyScore { get; set; }

    /// <summary>
    /// 用于本地记录积分的钥匙
    /// </summary>
    public const string MYSCORE = "MYSCORE_KEY";

    /// <summary>
    /// 万能卡图片
    /// </summary>
    Sprite sptComCard;

    private ImageInfo comCardImageInfo;

    /// <summary>
    /// 万能卡：是否处于使用状态，使用状态不允许其他操作，只能点击拼图区的位置进行使用
    /// </summary>
    public bool bComCardUseState { get; set; }

    /// <summary>
    /// 限定卡：是否处于限定状态，限定状态只允许生产任务图片
    /// </summary>
    public bool bLimitCardState { get; set; }

    /// <summary>
    /// 道具区域UI控制器
    /// </summary>
    public PropsUIController propsUIController;

    /// <summary>
    /// 记录我拥有的卡
    /// </summary>
    public ePropsCardType MyCardType { get; set; }

    /// <summary>
    /// 用于本地记录我拥有的卡类型，本次游戏没有使用下次可以用
    /// </summary>
    public const string MYCARDTYPE = "MYCARDTYPE_KEY";

    void ClearAllSprite()
    {
        if (allSprite != null)
        {
            if (allSprite[true] != null)
            {
                allSprite[false].Clear();
                allSprite[true] = null;
            }
            if (allSprite[false] != null)
            {
                allSprite[false].Clear();
                allSprite[false] = null;
            }
            allSprite.Clear();
        }
        allSprite = null;
    }

    void ClearGenerateImageInfo()
    {
        if (generateImageInfo != null)
        {
            if (generateImageInfo[true] != null)
            {
                generateImageInfo[false].Clear();
                generateImageInfo[true] = null;
            }
            if (generateImageInfo[false] != null)
            {
                generateImageInfo[false].Clear();
                generateImageInfo[false] = null;
            }
            generateImageInfo.Clear();
            generateImageInfo = null;
        }
    }

    private void Awake()
    {
        //需要早点读取
        MyScore = 500;// PlayerPrefs.GetInt(MYSCORE);//读取本地我的积分，默认值0
        MyCardType = (ePropsCardType)PlayerPrefs.GetInt(MYCARDTYPE);//读取本地我的道具卡，默认值0

        //初始化万能卡使用状态和限定卡限定状态
        bComCardUseState = false;
        bLimitCardState = false;

        //初始化万能卡信息
        comCardImageInfo = new ImageInfo();
        comCardImageInfo.id = 10000;
        comCardImageInfo.isMission = true;
        comCardImageInfo.isCommonCard = true;
    }

    // Use this for initialization
    void Start() {

		_instance = this;   //设置GameManager Instance引用
        mainUICtr = MainUI.GetComponent<MainUIController>();
        
        StartUI.SetActive(true);    //显示开始UI
		MainUI.SetActive(false);    //隐藏游戏主UI
		EndUI.SetActive(false);     //隐藏结束UI

		initTip();  //初始化提示图片
		initGame(); //初始化主游戏逻辑
	}

    private void Update()
    {

    }


    /// <summary>
    /// 初始化提示图片
    /// </summary>
    void initTip() {

        //设置关卡对应的拼图原图
        string spriteName = "Images/missions/mission_tip" + GetImageIndexByLevel();
		//加载图片
		var sprite = Resources.Load<Sprite>(spriteName);
		//设置提示图片
		StartUI.GetComponent<StartUIController>().SetTipImage(sprite);
        //万能卡
        sptComCard = Resources.Load<Sprite>("Images/missions/mission_tip23");
    }

    /// <summary>
    /// 根据关卡等级获取关卡图片的资源名称末尾序号
    /// </summary>
    /// <returns></returns>
    int GetImageIndexByLevel()
    {
        int index = 0;
        switch (DataManager.Instance.Current_Level)
        {
            case 0:
                index = 0;
                break;
            case 1:
                index = 6;
                break;
            case 2:
                index = 12;
                break;
            case 3:
                index = 13;
                break;
            case 4:
                index = 28;
                break;
            default:
                break;
        }
        return index;
    }

    /// <summary>
    /// 根据相应等级关卡所使用的图片的分割块数获取分割数
    /// </summary>
    /// <returns></returns>
    int GetSideCountByLevel()
    {
        int count = 0;
        switch (DataManager.Instance.Current_Level)
        {
            case 0:
            case 1:
                count = 4;
                break;
            case 2:
            case 3:
                count = 5;
                break;
            case 4:
                count = 6;
                break;
            default:
                break;
        }
        return count;
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame() {
		StartUI.SetActive(false);   //隐藏开始UI
		MainUI.SetActive(true);     //显示主游戏UI
	}

	/// <summary>
	/// 初始化主游戏逻辑
	/// </summary>
	void initGame() {
        //StepCount = 0;  //初始化步数统计

		int side = GetSideCountByLevel();        //计算拼图分割数
		float spacing = 5 * (2 + side - 1);        //计算拼图块总间隔

		//计算拼图块单块Size=（拼图面板边长-拼图块总间隔）/分割数
		float cellSize = (gamePanelSize - spacing) / side;

		gamePanel.cellSize = new Vector2(cellSize, cellSize);   //设置gamePanel的块Size，即拼图块的size
		gamePanel.constraintCount = side;   //设置gamePanel的换行步长为拼图分割数

		int length = side * side;   //计算总拼图块块数
		CurImagesInfo = new ImageInfo[length]; //创建总块数等长的int型数组，用于映射拼图块序列
        Debug.LogFormat("计算总拼图块块数 = {0}, CurImagesInfo == null is {1}", length, CurImagesInfo == null);
		blockCollections = new GameObject[length];  //创建总块数等长的GameObject数组,储存拼图块UI引用
		for (int i = 0; i < length; i++) {
            CurImagesInfo[i] = new ImageInfo();
            CurImagesInfo[i].id = -1; //赋值拼图映射：一开始不显示使用-1作为初始值
            CurImagesInfo[i].isMission = false; //

            //实例化拼图块预制体
            GameObject newBlock = Instantiate(blockPrefab, gamePanel.transform, false) as GameObject;
			newBlock.name = i.ToString();   //命名拼图块
			blockCollections[i] = newBlock; //储存拼图块UI对象引用
		}

        //加载拼图区和生产区的图片并存储
        GenerateAllImages();

        //CurrentIndex = RandArray(CurrentIndex); //打乱映射数组

        //一开始不显示了
        //根据映射数组匹配拼图块显示内容
        //MatchSprite(blockCollections, spriteBlocks, CurrentIndex);
    }

    /// <summary>
    /// 加载拼图区和生产区的图片并存储
    /// 组装生产区信息列表
    /// </summary>
    void GenerateAllImages()
    {
        lately5ImageInfo = new ImageInfo[5];
        latelyIndex = 0;

        ClearGenerateImageInfo();
        ClearAllSprite();
        allSprite = new Dictionary<bool, List<Sprite>>();
        generateImageInfo = new Dictionary<bool, List<ImageInfo>>();
        //关卡任务图集
        allSprite[true] = new List<Sprite>();
        generateImageInfo[true] = new List<ImageInfo>();
        string spriteName1 = "Images/missions/mission" + GetImageIndexByLevel();
        Sprite[] spriteBlocks1 = Resources.LoadAll<Sprite>(spriteName1);
        Debug.LogFormat("加载{0}获得{1}个图片", spriteName1, spriteBlocks1.Length);
        for (int i = 0; i < spriteBlocks1.Length; i++)
        {
            allSprite[true].Add(spriteBlocks1[i]);
            ImageInfo info = new ImageInfo();
            info.id = i;
            info.isMission = true;
            generateImageInfo[true].Add(info);
        }
        //混淆图集：在这里任意选一张了
        allSprite[false] = new List<Sprite>();
        generateImageInfo[false] = new List<ImageInfo>();
        string spriteName2 = "Images/missions/mission17";
        Sprite[] spriteBlocks2 = Resources.LoadAll<Sprite>(spriteName2);
        Debug.LogFormat("加载{0}获得{1}个图片", spriteName2, spriteBlocks2.Length);
        for (int i = 0; i < spriteBlocks2.Length; i++)
        {
            allSprite[false].Add(spriteBlocks2[i]);
            ImageInfo info = new ImageInfo();
            info.id = i;
            info.isMission = false;
            generateImageInfo[false].Add(info);
        }
    }

    /// <summary>
    /// 随机出一张图片信息：到生产区显示
    /// </summary>
    public ImageInfo GenerateImageInfo()
    {
        //只允许显示8张，超过8张不生产了
        if (GenerateImageCount < 8)
        {
            GenerateImageCount++;

            //如果是限定状态，只产生任务图片
            if (bLimitCardState)
            {
                return RandomMissionImage();
            }

            //从图片数组中取一张显示
            //限制规则：1.每4张碎片中至多一张非本地图的碎片，2.每抽5张最多1张已出现的碎片
            if (isNotLoaclImage) //如果随机过非任务图片，那么只能随机任务图片了
            {
                noLocalImageCount++;
                if (noLocalImageCount >= 4)
                {
                    noLocalImageCount = 0;
                    isNotLoaclImage = false;
                }
                return RandomMissionImage();
            }
            else
            {
                //如果随机到的是偶数就生产任务图片
                bool isMission = Random.Range(1, 10) % 2 == 0;
                if (isMission)
                {
                    return RandomMissionImage();
                }
                else
                {
                    isNotLoaclImage = true;
                    noLocalImageCount++;
                    return RandomNoMissionImage();
                }
            }
        }

        return null;
    }
    
    /// <summary>
    /// 随机任务图片
    /// </summary>
    /// <returns></returns>
    ImageInfo RandomMissionImage()
    {
        //每抽5张最多1张已出现的碎片
        List<ImageInfo> imageInfos = generateImageInfo[true];
        return RandomImage(imageInfos);
    }

    /// <summary>
    /// 随机非任务图片
    /// </summary>
    /// <returns></returns>
    ImageInfo RandomNoMissionImage()
    {
        //每抽5张最多1张已出现的碎片
        List<ImageInfo> imageInfos = generateImageInfo[false];
        return RandomImage(imageInfos);
    }

    ImageInfo RandomImage(List<ImageInfo> imageInfos)
    {
        ImageInfo info = null;
        bool isOK = false;
        while (!isOK)
        {
            int index = Random.Range(0, imageInfos.Count);
            for (int i = 0; i < lately5ImageInfo.Length; i++)
            {
                if (lately5ImageInfo[i] == null || imageInfos[index].id != lately5ImageInfo[i].id || imageInfos[index].isMission != lately5ImageInfo[i].isMission)
                {
                    info = new ImageInfo();
                    info.isMission = imageInfos[index].isMission;
                    info.id = imageInfos[index].id;

                    lately5ImageInfo[latelyIndex] = info;
                    latelyIndex++;
                    if (latelyIndex >= 5)
                    {
                        latelyIndex = 0;
                    }
                    isOK = true;
                }
            }
        }

        return info;
    }

    ///// <summary>
    ///// 打乱数组函数
    ///// </summary>
    ///// <param name="arr"></param>
    ///// <returns></returns>
    //int[] RandArray(int[] arr) {
    //	int[] newarr = new int[arr.Length];
    //	int k = arr.Length;
    //	for (int i = 0; i < arr.Length; i++) {
    //		int temp = Random.Range(0, k);
    //		newarr[i] = arr[temp];
    //		//arr[temp]后面的数向前移一位  
    //		for (int j = temp; j < arr.Length - 1; j++) {
    //			arr[j] = arr[j + 1];
    //		}
    //		k--;
    //	}
    //	return newarr;
    //}

    ///// <summary>
    ///// 匹配拼图块显示精灵
    ///// </summary>
    ///// <param name="blockCollections">拼图块GameObject集合</param>
    ///// <param name="spriteBlocks">图集</param>
    ///// <param name="indexs">映射数组</param>
    //void MatchSprite(GameObject[] blockCollections, Sprite[] spriteBlocks, int[] indexs) {
    //	int length = indexs.Length;
    //	for (int i = 0; i < length; i++) {
    //		var image = blockCollections[i].GetComponent<Image>();
    //		image.sprite = spriteBlocks[indexs[i]];
    //	}
    //}

    /// <summary>
    /// 交换拼图
    /// </summary>
    /// <param name="block1">拼图块1的序号</param>
    /// <param name="block2">拼图块2的序号</param>
    public void SwitchBlock(int block1, int block2) {
		#region 交换映射数据
		ImageInfo a = CurImagesInfo[block1];   //暂存拼图块1序号对应的映射数据
		CurImagesInfo[block1] = CurImagesInfo[block2];    //设置拼图块1序号对应的映射数据为块2对应的映射数据
		CurImagesInfo[block2] = a;   //把暂存的映射数据赋值给块2对应的映射数据
		Debug.Log("Switch Real Index: " + a + " And " + CurImagesInfo[block1]);
		#endregion

		#region 交换拼图块显示的图片精灵
		//var b = blockCollections[block1].GetComponent<Image>().sprite;
		//blockCollections[block1].GetComponent<Image>().sprite = blockCollections[block2].GetComponent<Image>().sprite;
		//blockCollections[block2].GetComponent<Image>().sprite = b;
        //同时设置信息和图片显示
        blockCollections[block1].GetComponent<blockEventHandler>().SetImageInfoAndShow(CurImagesInfo[block1]);
        blockCollections[block2].GetComponent<blockEventHandler>().SetImageInfoAndShow(CurImagesInfo[block2]);
        #endregion

        //StepCount++;    //累加步数
        //StepCounterText.text = "已用步数：" + StepCount; //更新步数统计显示文字

        if (CheckComplete()) {  //判断拼图是否完成
			GameComplete(); //执行拼图完成函数
		}
	}

    /// <summary>
    /// 生产区的图片放到拼图区
    /// </summary>
    /// <param name="info"></param>
    /// <param name="block"></param>
    public void FactoryToGame(ImageInfo info,int block)
    {
        GenerateImageCount--;
        ImageInfo old = CurImagesInfo[block];
        CurImagesInfo[block] = info;
        //blockCollections[block].GetComponent<Image>().sprite = GetSpriteByImageInfo(info);//仅仅这一句没有同步信息
        //同时设置信息和图片显示
        blockCollections[block].GetComponent<blockEventHandler>().SetImageInfoAndShow(info);

        blockEventHandler handler = HoldingObject.GetComponent<blockEventHandler>();
        if (old.id == -1)
        {
            //如果原本拼图区这一块没有图片，生产区域的图片还要隐藏并放到父物体最后面去
            handler.image.sprite = null;
            handler.transform.SetAsLastSibling();//隐藏的移到父物体最后面
            handler.gameObject.SetActive(false);
        }
        else
        {
            //如果拼图区原本有图片，将其放回架子上
            handler.SetImageInfoAndShow(old);
        }


        if (CheckComplete())
        {  //判断拼图是否完成
            GameComplete(); //执行拼图完成函数
        }
    }

    /// <summary>
    ///根据图片信息获取图片
    /// </summary>
    /// <returns></returns>
    public Sprite GetSpriteByImageInfo(ImageInfo info)
    {
        if (info == null)
        {
            Debug.LogError("根据图片信息获取图片传递的参数为空");
            return null;
        }
        Debug.LogFormat("根据图片信息获取图片id={0}, isMission={1}, isCommonCard = {2}", info.id, info.isMission, info.isCommonCard);
        if (info.isCommonCard)
        {
            return sptComCard;
        }
        else
        {
            List<Sprite> spts = allSprite[info.isMission];
            if (info.id == -1)
            {
                return null;
            }
            return spts[info.id];
        }
    }

	/// <summary>
	/// 检查拼图是否完成
	/// </summary>
	/// <returns></returns>
	public bool CheckComplete() {
		bool isComplete = true;
		var length = CurImagesInfo.Length;
		for (int i = 0; i < length; i++) {
			if ((i != CurImagesInfo[i].id || !CurImagesInfo[i].isMission) && !CurImagesInfo[i].isCommonCard) {   //根据映射数组元素是否有序连贯来判断是否完成拼图，且需要时任务图片，或者是万能卡
				isComplete = false;
				break;
			}
		}
		return isComplete;
	}

	/// <summary>
	/// 拼图完成函数
	/// </summary>
	void GameComplete() {
		MainUI.SetActive(false);    //隐藏游戏主UI
		EndUI.SetActive(true);  //显示结束UI

        //var level = DataManager.Instance.Current_Level; //读取当前关卡难度
        //int threeStart = (level + 4) * (level + 4);     //计算3星得分步数线
        //int twoStart = (level + 4) * (level + 4 + 2);   //计算2星得分步数线
        //int oneStart = (level + 4) * (level + 4 + 4);   //计算1星得分步数线
        //int start = 1;

        //if (StepCount <= threeStart) {  //根据步数设置得分
        //	start = 3;
        //}
        //else if (StepCount <= twoStart) {
        //	start = 2;
        //}
        //else {
        //	start = 1;
        //}

        //计算得分
        int score = 0;
        int SecondCount = mainUICtr.SecondCount;
        switch (DataManager.Instance.Current_Level)
        {
            case 0:
                if(SecondCount > 240)
                    score = 15;
                else if(SecondCount > 180)
                    score = 25;
                else if (SecondCount > 120)
                    score = 35;
                else if (SecondCount > 90)
                    score = 40;
                else
                    score = 50;
                break;
            case 1:
                if (SecondCount > 240)
                    score = 15;
                else if (SecondCount > 180)
                    score = 35;
                else if (SecondCount > 120)
                    score = 40;
                else if (SecondCount > 90)
                    score = 45;
                else
                    score = 60;
                break;
            case 2:
                if (SecondCount > 240)
                    score = 25;
                else if (SecondCount > 180)
                    score = 40;
                else if (SecondCount > 120)
                    score = 45;
                else if (SecondCount > 90)
                    score = 50;
                else
                    score = 70;
                break;
            case 3:
                if (SecondCount > 240)
                    score = 25;
                else if (SecondCount > 180)
                    score = 40;
                else if (SecondCount > 120)
                    score = 45;
                else if (SecondCount > 90)
                    score = 50;
                else
                    score = 70;
                break;
            case 4:
                if (SecondCount > 240)
                    score = 30;
                else if (SecondCount > 180)
                    score = 50;
                else if (SecondCount > 120)
                    score = 60;
                else if (SecondCount > 90)
                    score = 70;
                else
                    score = 100;
                break;
            default:
                break;
        }
        //本地记录我的积分
        MyScore += score;
        PlayerPrefs.SetInt(MYSCORE, MyScore);
        //通过数据管理器设置关卡完成，设置得分并解锁下一关卡
        DataManager.Instance.CompleteMission(DataManager.Instance.Current_Level, score);

        //设置结束UI的分数，步数显示
        //EndUI.GetComponent<EndUIController>().SetScore(StepCount, start);
        EndUI.GetComponent<EndUIController>().SetScore(SecondCount, score);
        mainUICtr.IsComplete = true;
    }

	/// <summary>
	/// 进入下一关卡
	/// </summary>
	public void NextMission() {
        //设置数据管理器当前关卡为下一关
        if (DataManager.Instance.Current_Level < 5)
        {
            DataManager.Instance.Current_Level++;
        }
        //跳转新的游戏场景
        gameSceneManager.GoMainScene();
	}

	/// <summary>
	/// 销毁时调用
	/// </summary>
	void OnDestroy() {
		_instance = null;
	}


    /// <summary>
    /// 点击丢弃一张图片
    /// </summary>
    public void OnClickBtnThrow()
    {
        if (isHold)
        {
            isHold = false;
            //先清理图片
            InitImage(HoldingObject);
            //根据图片类型处理：是生产区域的图片还要隐藏并放到父物体最后面去
            if (CurHoldImageType == eImageType.factory)
            {
                GenerateImageCount--;//丢弃的是生产区的，增加一个空位
                HoldingObject.transform.SetAsLastSibling();//隐藏的移到父物体最后面
                HoldingObject.SetActive(false);
            }
            else if (CurHoldImageType == eImageType.game)
            {
                int a = int.Parse(HoldingObject.name);
                CurImagesInfo[a].id = -1;
                CurImagesInfo[a].isMission = false;
                CurImagesInfo[a].isCommonCard = false;
                HoldingObject.GetComponent<blockEventHandler>().InitImageInfo();
            }
            HoldingObject = null;
        }
    }

    /// <summary>
    /// 初始化Image组件上的图片，显示空白
    /// </summary>
    /// <param name="image"></param>
    public void InitImage(Image image)
    {
        if (image)
        {
            image.color = Color.white;
            image.sprite = null;
        }
    }

    /// <summary>
    /// 初始化Image组件上的图片，显示空白
    /// </summary>
    /// <param name="ObjImage"></param>
    public void InitImage(GameObject ObjImage)
    {
        Image image = ObjImage.GetComponent<Image>();
        InitImage(image);
    }

    /// <summary>
    /// 设置是都启动丢弃按钮：选中了一个图片的时候才启用
    /// </summary>
    /// <param name="interactable"></param>
    public void SetBtnThrowInteractable(bool interactable)
    {
        btnThrow.interactable = interactable;
    }

    /// <summary>
    /// 购买道具
    /// </summary>
    public void BuyPropsCard()
    {
        int cardType = Random.Range(1, 4);//Random.Range针对整数随机，返回一个随机整数，在min（包含）和max（排除）之间，左闭右开原则（和float是不一样的）
        MyCardType = (ePropsCardType)cardType;
        //本地记录一波道具信息
        PlayerPrefs.SetInt(MYCARDTYPE, cardType);
        //减去消费积分并本地记录一波积分信息
        MyScore -= 30;
        PlayerPrefs.SetInt(MYSCORE, MyScore);
        propsUIController.RefreshPanel();
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    public void UsePropsCard()
    {
        switch (MyCardType)
        {
            case ePropsCardType.Common:
                //设置万能卡为使用状态后，设置万能卡为绿色显示
                bComCardUseState = true;
                break;
            case ePropsCardType.FindError:
                //直接查找错误的拼图显示为红色，没找到不算
                if (!FindErrorImage())
                {
                    mainUICtr.ShowNotFindTips();
                    Debug.Log("没有找到错误的拼图");
                }
                UseCardComplete();
                break;
            case ePropsCardType.LimitType:
                bLimitCardState = true;
                UseCardComplete();
                break;
            default:
                break;
        }

        propsUIController.RefreshPanel();
    }

    /// <summary>
    /// 使用道具卡完毕，重置道具信息
    /// </summary>
    private void UseCardComplete()
    {
        MyCardType = ePropsCardType.none;
        //重置本地记录道具信息
        PlayerPrefs.SetInt(MYCARDTYPE, 0);
    }

    /// <summary>
    /// 查找错误的拼图显示为红色
    /// </summary>
    private bool FindErrorImage()
    {
        //存储所有错误的地方，最后随机一个出来
        List<int> errList = new List<int>();
        var length = CurImagesInfo.Length;
        for (int i = 0; i < length; i++)
        {
            if ((i != CurImagesInfo[i].id || !CurImagesInfo[i].isMission) && !CurImagesInfo[i].isCommonCard && CurImagesInfo[i].id != -1)
            {   //id对不上或者不是任务图片，并且不是万能卡也不是白卡
                errList.Add(i);
            }
        }
        int errCount = errList.Count;
        if (errCount <= 0)
        {
            Debug.Log("没有找到错误的拼图");
            return false;
        }

        //随机一个并设置红色：变为红色的图片在后面操作后会变成正常颜色
        int index = Random.Range(0, errCount);
        int id = errList[index];
        blockCollections[id].GetComponent<Image>().color = Color.red;
        return true;
    }

    /// <summary>
    /// 正式使用万能卡
    /// </summary>
    public void UserComCard(int block)
    {
        CurImagesInfo[block] = comCardImageInfo;
        blockCollections[block].GetComponent<blockEventHandler>().SetImageInfoAndShow(comCardImageInfo);

        bComCardUseState = false;
        UseCardComplete();
        propsUIController.RefreshPanel();

        if (CheckComplete())
        {  //判断拼图是否完成
            GameComplete(); //执行拼图完成函数
        }
    }
}

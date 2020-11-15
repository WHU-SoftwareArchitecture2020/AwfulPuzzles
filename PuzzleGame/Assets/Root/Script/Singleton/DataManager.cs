using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// DataManager 数据管理器
/// 单例模式
/// </summary>
public class DataManager : MonoBehaviour {

    private static DataManager _instance = null;

	/// <summary>
	/// 暴露Instance属性便于单例调用
	/// </summary>
    public static DataManager Instance {
        get {
			if (_instance == null) {
				GameObject obj = new GameObject("DataManager");	//创建空对象
				_instance = obj.AddComponent<DataManager>();	//添加DataManager脚本组件
				DontDestroyOnLoad(obj);							//设置不在场景切换时销毁
			}
			return _instance;
		}
    }

    /// <summary>
    /// 当前难度
    /// </summary>
    public int Current_Level;	

    /// <summary>
    /// 当前关卡
    /// </summary>
    public int Current_Mission;


    /// <summary>
    /// 加载Xml文件 
    /// </summary>
    /// <returns>The levels.</returns>
	public List<Mission> LoadMissions() {
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        //如果本地存在配置文件则读取配置文件
        //否则在本地创建配置文件的副本
        //为了跨平台及可读可写，需要使用Application.persistentDataPath
		string filePath = Application.persistentDataPath + "/mymissions.xml";

		//检查是否存在配置文件
        if (!IOUntility.isFileExists(filePath)) { //不存在
			xmlDoc.LoadXml(((TextAsset)Resources.Load("mymissions")).text);	//加载Resources文件夹中的配置范本，拷贝其中内容
			IOUntility.CreateFile(filePath, xmlDoc.InnerXml);	//使用IOUntility类方法创建配置文件
        }
        else {	//存在
            xmlDoc.Load(filePath);	//加载配置文件
        }

        XmlElement root = xmlDoc.DocumentElement;	//读取配置文件中的元素
		XmlNodeList missionNodes = root.SelectNodes("/missions/mission");	//查找xml配置文件中的mission节点
        //初始化关卡列表
        List<Mission> missions = new List<Mission>();
        foreach (XmlElement xe in missionNodes) {	//遍历配置节点，把配置存在missons数组中
            Mission l = new Mission();
            l.ID = int.Parse(xe.GetAttribute("id"));	//读取关卡id
            l.Score = int.Parse(xe.GetAttribute("score"));	//读取关卡得分
            //使用unlock属性来标识当前关卡是否解锁
            if (xe.GetAttribute("unlock") == "1") {
                l.UnLock = true;
            }
            else {
                l.UnLock = false;
            }

            missions.Add(l);	//添加到数组
        }

        return missions;	//返回关卡信息数组
    }

	/// <summary>
	/// 设置关卡信息
	/// </summary>
	/// <param name="id"></param>
	/// <param name="unlock">是否解锁</param>
	/// <param name="score">分数</param>
	private void SetMission(int id, bool unlock,int score) {
		//创建Xml对象
		XmlDocument xmlDoc = new XmlDocument();
		//设置配置文件地址
		string filePath = Application.persistentDataPath + "/mymissions.xml";
		//加载文件
		xmlDoc.Load(filePath);
		//读取配置文件中的元素
		XmlElement root = xmlDoc.DocumentElement;
		//查找xml配置文件中的mission节点
		XmlNodeList missionNodes = root.SelectNodes("/missions/mission");
		foreach (XmlElement xe in missionNodes) {
			//根据id找到对应的关卡
			if (xe.GetAttribute("id") == id.ToString()) {
				//根据unlock重新为关卡赋值
				if (unlock) {
					xe.SetAttribute("unlock", "1");
				}
				else {
					xe.SetAttribute("unlock", "0");
				}

				xe.SetAttribute("score", score.ToString());	//设置分数
			}
		}

		//保存文件
		xmlDoc.Save(filePath);
	}

	/// <summary>
	/// 完成关卡
	/// </summary>
	/// <param name="id"></param>
	/// <param name="score">分数</param>
	/// <param name="unlockNext">是否解锁下一关</param>
	public void CompleteMission(int id, int score,bool unlockNext=true) {
		var missionsData = Instance.LoadMissions();		//读取关卡配置

		score = Mathf.Max(score,missionsData[id].Score);	//只保存关卡的最高得分
		SetMission(id,true,score);	//更新本关卡信息

		if (unlockNext && id < 5) {	//总共30关，防止越界
			SetMission(id + 1, true, missionsData[id+1].Score);	//解锁下一关卡信息
		}
	}

}

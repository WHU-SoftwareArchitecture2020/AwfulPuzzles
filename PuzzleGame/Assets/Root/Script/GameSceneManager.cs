using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏场景管理器
/// </summary>
public class GameSceneManager : MonoBehaviour {

	/// <summary>
	/// 开始场景
	/// </summary>
    public string menuSceneName;

	/// <summary>
	/// 难度关卡选择场景
	/// </summary>
    public string levelSceneName;

	/// <summary>
	/// 主游戏场景
	/// </summary>
    public string mainSceneName;

	/// <summary>
	/// 跳转开始场景
	/// </summary>
    public void GoStartScene() {
        SceneManager.LoadScene(menuSceneName);

    }

	/// <summary>
	/// 跳转游戏主场景
	/// </summary>
    public void GoMainScene() {
        SceneManager.LoadScene(mainSceneName);
    }


	/// <summary>
	/// 跳转难度关卡选择场景
	/// </summary>
    public void GoLevelScene() {
        SceneManager.LoadScene(levelSceneName);
    }


	/// <summary>
	/// 退出程序
	/// </summary>
    public void ExitGame() {
        //If we are running in a standalone build of the game
#if UNITY_STANDALONE
        //Quit the application
        Application.Quit();
#endif

        //If we are running in the editor
#if UNITY_EDITOR
        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

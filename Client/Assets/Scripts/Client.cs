using Game.Core.Manager;
using Game.Core.Util;
using UnityEngine;

/************************* 
* 作者： wenyueyun 
* 时间： 2018/4/18 9:11:54 
* 描述： 游戏启动 
*************************/
public class Client : MonoBehaviour
{

    /* 预编译模式
    * DEV                       --开发模式(程序开发使用)
    * STABLE                  --稳定模式(正式环境使用)
    * REFLECTION           --安卓热更
    * ILRUNTIME             --IOS热更
    */

    public static Client Instance = null;
    private bool isInit = false;
    private void Awake()
    {
        LogUtil.LogGreen(string.Format("Application.dataPath:{0}", Application.dataPath));
        LogUtil.LogGreen(string.Format("Application.persistentDataPath:{0}", Application.persistentDataPath));
        LogUtil.LogGreen(string.Format("Application.streamingAssetsPath:{0}", Application.streamingAssetsPath));
        LogUtil.LogGreen(PathUtil.CutFilePath(Application.dataPath, 2));
    }

    //启动更新程序
    private void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
       // DontDestoryManager.Instance.Initialize();

        Instance = this;
        isInit = false;
        //检测更新
        gameObject.AddComponent<Update>();
    }

    //开始游戏
    public void StartGame()
    {
        isInit = true;
        LogUtil.LogGreen("开始游戏");

        //Loading界面

        //初始化管理器
        Initialize();

        //加载配置表

        //进入游戏
    }

    
    public void Initialize()
    {
        //对象池
        PoolManager.Instance.Initialize();
        ////脚本管理
        //ScriptManager.Instance.Initialize();
        //资源管理
        AssetManager.Instance.Initialize();
        //配置表
        ConfigManager.Instance.Initialize();

    }

    public void UnInitialize()
    {

    }


    private void Update()
    {
        if (isInit)
        {
            AssetManager.Instance.Update();
            ScriptManager.Instance.Update();
        }
    }

    private void LateUpdate()
    {
        if (isInit)
        {
            ScriptManager.Instance.LateUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (isInit)
        {
            ScriptManager.Instance.FixedUpdate();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (isInit)
        {
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (isInit)
        {
        }
    }

    private void OnApplicationQuit()
    {
        if (isInit)
        {
            UnInitialize();
        }
    }
}

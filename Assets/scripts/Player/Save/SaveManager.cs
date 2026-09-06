using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public ResourceSystemHost resourceSystem;
    public Transform player;

    [Tooltip("拖场景里面CGControl物体")]
    public CGControl cgControl;

    private bool _isSaving = false; // 防止连续按Q重复保存

    private void Start()
    {
        LoadGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !_isSaving)
        {
            SaveGame();
        }
    }

    //保存游戏
    public void SaveGame()
    {
        _isSaving = true;

        PlayerSaveData data = new PlayerSaveData();
        //获取资源
        data.resources = resourceSystem.Manager.GetAllResource();
        //获取玩家位置
        data.playerPosition = new PlayerPosition();
        data.playerPosition.x = player.position.x;
        data.playerPosition.y = player.position.y;
        //暂时打印测试
        Debug.Log(
            "准备保存游戏，资源数量：" + data.resources.Count + ",玩家位置：(" + data.playerPosition.x + "," + data.playerPosition.y + ")");

        SaveNetwork saveNetwork = new SaveNetwork(resourceSystem.apiSettings);
        StartCoroutine(SaveCoroutineWrap(saveNetwork, data));
    }

    /// <summary>包装保存协程：网络保存完成后播放存档CG，结束解锁</summary>
    private IEnumerator SaveCoroutineWrap(SaveNetwork saveNetwork, PlayerSaveData data)
    {
        yield return saveNetwork.SaveGame(data);

        // =========网络保存完成，执行播放存档CG=========
        Debug.Log("云端存档保存完成，播放存档CG");
        if (cgControl != null)
        {
            cgControl.PlaySaveCg();
        }

        _isSaving = false; //保存流程结束，允许再次按Q存档
    }

    //读取游戏
    public void LoadGame()
    {
        SaveNetwork saveNetwork = new SaveNetwork(resourceSystem.apiSettings);
        StartCoroutine(saveNetwork.LoadGame(OnLoadGameSuccess));
    }

    private void OnLoadGameSuccess(PlayerSaveData data)
    {
        Debug.Log(
        "存档加载成功，资源数量：" + data.resources.Count + "，玩家位置：(" + data.playerPosition.x + ", " + data.playerPosition.y + ")");
        player.position = new Vector3(data.playerPosition.x, data.playerPosition.y, player.position.z);
        Debug.Log("玩家位置恢复完成：（" + player.position.x + "," + player.position.y + ")");
    }
}

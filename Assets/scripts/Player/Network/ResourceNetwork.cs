using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class ResourceNetwork
{
    private ApiSettings apiSettings;
    //背包管理器引用，网络拿到服务器数据后交给Manager更新背包
    public ResourceNetwork(ApiSettings apiSettings)
    { this.apiSettings = apiSettings; }
    //协程：POST请求，上传新增资源信息到后端
    public IEnumerator SendResource(ResourceData resource,ResourceManager manager, System.Action<bool> callback)
    {
        Debug.Log("当前资源仅保存到本地存档，不同步resources.json");
        callback?.Invoke(true);
        yield break;
    }
    //协程：GET请求，获取玩家全部背包资源
    public IEnumerator GetResource(ResourceManager manager, System.Action<bool> callback = null)
    {
        string fullUrl = apiSettings.baseUrl + "/resource/list";
        //快速创建GET请求
        UnityWebRequest request = UnityWebRequest.Get(fullUrl);
        yield return request.SendWebRequest();
        Debug.Log(request.result);
        //GET失败
        if (request.result != UnityWebRequest.Result.Success)
        {
            callback?.Invoke(false);
            yield break;
        }
        //GET成功
        //将后端返回的JSON反序列化为 ResourceListData 对象
        string json = request.downloadHandler.text;
        Debug.Log(
            "服务器返回：" + json
        );
        ResourceListData data = JsonUtility.FromJson<ResourceListData>(json);
        //把服务器下发的数据解析
        if (data == null || data.resources == null)
        {
            Debug.LogError("服务器资源解析失败");
            callback?.Invoke(false);
            yield break;
        }
        //更新客户端
        manager.LoadFromServer(data.resources);
        Debug.Log("客户端资源更新完成");
        callback?.Invoke(true);
    }
    public IEnumerator RemoveResource(ResourceData resource,ResourceManager manager,System.Action<bool> callback)
    {
        string fullUrl = apiSettings.baseUrl + "/resource/remove";
        string json = JsonUtility.ToJson(resource);
        UnityWebRequest request = new UnityWebRequest(fullUrl, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("资源扣除失败：" + request.error);
            callback?.Invoke(false);
            yield break;
        }
        bool getSuccess = false;
        yield return GetResource(manager, (success) => { getSuccess = success; });
        callback?.Invoke(getSuccess);
    }
}
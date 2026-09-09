using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TreasureChest : MonoBehaviour
{
    //宝箱奖励列表
    public List<ChestReward> rewards = new List<ChestReward>();
    //资源管理器
    public ResourceSystemHost resourceSystem;
    private ResourceManager manager;
    //世界状态管理器
    public WorldStateManager worldStateManager;
    //是否已经开启
    private bool opened = false;
    [Header("宝箱唯一ID")]
    public string chestId;
    //音效
    public AudioClip openChestClip;
    private AudioSource AudioSource;
    private IEnumerator Start()
    {
        if(resourceSystem!=null)
        {
            manager = resourceSystem.Manager;
        }
        if(worldStateManager!=null)
        {
            yield return new WaitUntil(() => worldStateManager.IsInitialized);
            if(worldStateManager.IsChestOpened(chestId))
            {
                opened = true;
                gameObject.SetActive(false);
                Debug.Log("宝箱已经打开过：" + chestId);
            }
        }
    }
    void Awake()
    {
        //获取自身AudioSource组件
        AudioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("碰到宝箱:" + other.name);
        if (other.CompareTag("Player"))
        {
            OpenChest();
        }
    }
    void OpenChest()
    {
        if (opened) return;
        opened = true;
        if (manager == null)
        {
            Debug.LogError("manager为空");
            return;
        }
        //播放音效
        if (openChestClip != null && AudioSource != null)
        {
            AudioSource.PlayOneShot(openChestClip);
        }
        int requestCount;
        int successCount;
        requestCount = rewards.Count;
        successCount = 0;
        foreach (var reward in rewards)
        {
            manager.RequestAddResource(
                reward.resourceID,
                reward.amount,
                success =>
                {
                    if (success)
                    {
                        successCount++;
                        if (successCount == requestCount)
                        {
                            Debug.Log("宝箱全部奖励确认成功");
                            if (worldStateManager != null)
                            {
                                worldStateManager.RegisterOpenedChest(chestId);
                            }
                            //所有奖励都成功后，一次性显示全部奖励
                            ResourceGetTip.Instance?.Show( rewards,manager);
                            Destroy( gameObject,openChestClip != null ? openChestClip.length : 0.1f
                            );
                        }
                    }
                }
            );
        }
    }
}


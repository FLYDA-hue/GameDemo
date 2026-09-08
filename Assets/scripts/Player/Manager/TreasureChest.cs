using System.Collections.Generic;
using UnityEngine;
public class TreasureChest : MonoBehaviour
{
    //宝箱奖励列表
    public List<ChestReward> rewards = new List<ChestReward>();
    //资源管理器
    public ResourceSystemHost resourceSystem;
    private ResourceManager manager;
    //是否已经开启
    private bool opened = false;
    //音效
    public AudioClip openChestClip;
    private AudioSource AudioSource;
    void Start()
    {
        if (resourceSystem != null)
        {
            manager = resourceSystem.Manager;
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


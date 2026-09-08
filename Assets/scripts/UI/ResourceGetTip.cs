using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ResourceGetTip : MonoBehaviour
{
    public static ResourceGetTip Instance;
    public GameObject tipPanel;
    public Transform content;
    public GameObject rewardTextPrefab;
    public float showDuration = 1.5f;
    private Coroutine hideCoroutine;
    private void Awake()
    {
        Instance = this;
        if (tipPanel != null)
        {
            tipPanel.SetActive(false);
        }
    }
    public void Show(List<ChestReward> rewards, ResourceManager manager)
    {
        if (tipPanel == null || content == null || rewardTextPrefab == null)
        {
            Debug.LogError("ResourceGetTip 未正确配置");
            return;
        }
        //清空上一次的奖励
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        //生成本次奖励
        foreach (ChestReward reward in rewards)
        {
            GameObject rewardText = Instantiate(
                rewardTextPrefab,
                content
            );
            TMP_Text text = rewardText.GetComponent<TMP_Text>();
            if (text != null)
            {
                string resourceName =
                    manager.GetResourceName(reward.resourceID);
                text.text =
                    resourceName + " ×" + reward.amount;
            }
        }
        tipPanel.SetActive(true);
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);
        tipPanel.SetActive(false);
        hideCoroutine = null;
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
public class SaveSelectUI : MonoBehaviour
{
    public ApiSettings apiSettings;
    [Header("三个存档槽的通关星星")]
    public TMP_Text slot1Star;
    public TMP_Text slot2Star;
    public TMP_Text slot3Star;
    private void Start()
    {
        StartCoroutine(LoadAllSlots());
    }
    public void SelectSlot(int slot)
    {
        SaveSlotManager.CurrentSlot = slot;
        Debug.Log("当前选择存档：" + slot);
        SceneManager.LoadScene("GameScene");
    }
    private IEnumerator LoadAllSlots()
    {
        yield return CheckSlot(1, slot1Star);
        yield return CheckSlot(2, slot2Star);
        yield return CheckSlot(3, slot3Star);
    }
    private IEnumerator CheckSlot(int slot, TMP_Text starText)
    {
        string url = apiSettings.baseUrl + "/save?slot=" + slot;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            PlayerSaveData data =JsonUtility.FromJson<PlayerSaveData>(request.downloadHandler.text);
            if (data != null && data.completed)
            {
                starText.text = "★";
                starText.gameObject.SetActive(true);
                Debug.Log("存档" + slot + "已经通关");
            }
            else
            {
                starText.gameObject.SetActive(false);
            }
        }
        else
        {
            // 没有存档或者请求失败，就不显示星星
            starText.gameObject.SetActive(false);
        }
    }
}
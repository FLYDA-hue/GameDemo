using UnityEngine;
public class SavePoint : MonoBehaviour
{
    public SaveManager saveManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (saveManager == null)
            {
                saveManager = FindObjectOfType<SaveManager>();
            }
            if (saveManager != null)
            {
                saveManager.SetCanSave(true);
                Debug.Log("玩家进入树屋，可以保存");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (saveManager == null)
            {
                saveManager = FindObjectOfType<SaveManager>();
            }
            if (saveManager != null)
            {
                saveManager.SetCanSave(false);
                Debug.Log("玩家离开树屋，不可以保存");
            }
        }
    }
}
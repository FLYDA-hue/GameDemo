using UnityEngine;
public class SavePoint : MonoBehaviour
{
    public SaveManager saveManager;
    private bool playerInside = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("进入存档点");
        }
    }
    private void OnTriggerExit2D(Collider2D other
        )
    {
        if(other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("离开存档点");
        }
    }
    private void Update()
    {
        if(playerInside&&Input.GetKeyDown(KeyCode.X))
        {
            Save();
        }
    }
    private void Save()
    {
        if(saveManager==null)
        {
            Debug.LogError("SaveManager为空");
            return;
        }
        saveManager.SaveGame();
        Debug.Log("存档请求已发送");
    }
}

using UnityEngine;
public class PopupPanel : MonoBehaviour
{
    public GameObject targetPanel;

    public void TogglePanel()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(!targetPanel.activeSelf);
        }
    }
}

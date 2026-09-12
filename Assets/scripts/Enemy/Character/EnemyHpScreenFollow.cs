using UnityEngine;
public class EnemyHpScreenFollow : MonoBehaviour
{
    public Transform targetEnemy;
    public Vector2 offset = new Vector2(0, 45);
    private Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
        Debug.Log($"跟随脚本Start，目标：{targetEnemy?.name}");
    }

    void LateUpdate()
    {
        if (targetEnemy == null || _mainCam == null)
        {
            return;
        }
        RectTransform canvasRt = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector2 worldPos = targetEnemy.position;
        // 把怪物世界坐标 → 转换到Canvas RectTransform内部坐标
        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRt,
            _mainCam.WorldToScreenPoint(worldPos),
            _mainCam,
            out uiPos
        );
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = uiPos + offset;

        Debug.Log($"怪物世界坐标:{worldPos} | UI局部坐标:{uiPos + offset}");
    }

}

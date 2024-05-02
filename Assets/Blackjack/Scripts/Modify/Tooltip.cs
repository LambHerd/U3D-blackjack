using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerExitHandler, IPointerMoveHandler,IPointerEnterHandler
{
    public GameObject wordPanel;
    public string wordTextTxt;

    GameObject wordText;

    public int index;
    public bool used = false;

    // 鼠标进入UI Panel时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        wordText = Instantiate(wordPanel,transform);
        wordText.GetComponent<SettipText>().Settip(wordTextTxt);
    }

    public void OnPointerMove(PointerEventData eventData)
    {

        // 设置Word Panel的位置
        wordText.transform.position = Input.mousePosition+new Vector3(90f,-100f,0);
        
    }

    // 鼠标离开UI Panel时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(wordText);

    }

    float moveDistance = 100f; // 移动的距离
    float moveSpeed = 100f; // 移动的速度
    float moveAcceleration = -50f; // 移动的加速度

    private bool isMoving = false; // 标志是否正在移动
    private Vector3 targetPosition; // 目标位置

    public void OnClickToMove()
    {
        if (!isMoving)
        {
            // 计算目标位置为当前位置向上移动一定距离
            targetPosition = transform.position + Vector3.up * moveDistance;
            isMoving = true;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // 计算向目标位置移动的方向和距离
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);

            // 计算加速度
            float acceleration = moveAcceleration * Time.deltaTime;

            // 根据速度和加速度更新位置
            transform.position += direction * Mathf.Min(moveSpeed * Time.deltaTime, distance);

            // 如果已经接近目标位置，停止移动
            if (distance <= 0.01f)
            {
                isMoving = false;
            }
        }
    }
}


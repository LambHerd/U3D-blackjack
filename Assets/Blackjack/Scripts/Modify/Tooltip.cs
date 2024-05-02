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

    // ������UI Panelʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        wordText = Instantiate(wordPanel,transform);
        wordText.GetComponent<SettipText>().Settip(wordTextTxt);
    }

    public void OnPointerMove(PointerEventData eventData)
    {

        // ����Word Panel��λ��
        wordText.transform.position = Input.mousePosition+new Vector3(90f,-100f,0);
        
    }

    // ����뿪UI Panelʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(wordText);

    }

    float moveDistance = 100f; // �ƶ��ľ���
    float moveSpeed = 100f; // �ƶ����ٶ�
    float moveAcceleration = -50f; // �ƶ��ļ��ٶ�

    private bool isMoving = false; // ��־�Ƿ������ƶ�
    private Vector3 targetPosition; // Ŀ��λ��

    public void OnClickToMove()
    {
        if (!isMoving)
        {
            // ����Ŀ��λ��Ϊ��ǰλ�������ƶ�һ������
            targetPosition = transform.position + Vector3.up * moveDistance;
            isMoving = true;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // ������Ŀ��λ���ƶ��ķ���;���
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);

            // ������ٶ�
            float acceleration = moveAcceleration * Time.deltaTime;

            // �����ٶȺͼ��ٶȸ���λ��
            transform.position += direction * Mathf.Min(moveSpeed * Time.deltaTime, distance);

            // ����Ѿ��ӽ�Ŀ��λ�ã�ֹͣ�ƶ�
            if (distance <= 0.01f)
            {
                isMoving = false;
            }
        }
    }
}


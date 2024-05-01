using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveToBet : MonoBehaviour
{
    Vector3 targetPosition; // 目标位置
    float initialSpeed = 1000f; // 初始速度
    float acceleration = -20f; // 加速度

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        Vector3 initialPosition = rectTransform.anchoredPosition3D;
        float distance = Vector3.Distance(initialPosition, targetPosition);
        float currentSpeed = initialSpeed;

        while (Vector3.Distance(rectTransform.anchoredPosition3D, targetPosition) > 0.1f)
        {
            float step = currentSpeed * Time.deltaTime;
            rectTransform.anchoredPosition3D = Vector3.MoveTowards(rectTransform.anchoredPosition3D, targetPosition, step);

            // 根据加速度调整速度
            currentSpeed += acceleration * Time.deltaTime;

            yield return null;
        }
    }

    public void StartBet(Vector3 pos)
    {
        targetPosition = pos;
        StartCoroutine(MoveToTarget());
    }
}

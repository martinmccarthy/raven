using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private float m_speedX = 10f;
    [SerializeField] private float m_speedY = 10f;

    private RectTransform rect;
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        // move on both x and y

        rect.anchoredPosition += new Vector2(m_speedX, m_speedY) * Time.deltaTime;

        if (rect.anchoredPosition.y > ((RectTransform)transform.parent).rect.height / 2)
        {
            Destroy(gameObject);
        }
    }
}

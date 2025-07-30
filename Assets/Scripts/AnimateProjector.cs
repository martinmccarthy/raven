using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnimateProjector : MonoBehaviour
{
    [SerializeField] private GameObject m_imageIcon;
    [SerializeField] private float m_spawnInterval = 1f;
    [SerializeField] private RectTransform m_spawnArea;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= m_spawnInterval)
        {
            SpawnIcon();
            timer = 0f;
        }
    }

    void SpawnIcon()
    {
        GameObject icon = Instantiate(m_imageIcon, m_spawnArea);
        RectTransform r = icon.GetComponent<RectTransform>();
        float rand = Random.Range(-m_spawnArea.rect.width / 2, m_spawnArea.rect.height / 2);
        r.anchoredPosition = new Vector2(rand, -m_spawnArea.rect.height / 2);
    }
}

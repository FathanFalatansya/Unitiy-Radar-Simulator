using UnityEngine;
using UnityEngine.UI;

public class RadarDotFade : MonoBehaviour
{
    public float fadeSpeed = 2f; // Kecepatan perubahan alpha

    private Image dotImage;
    private float targetAlpha = 0f; // Target alpha (transparansi)

    private void Awake()
    {
        dotImage = GetComponent<Image>();
    }

    private void Update()
    {
        // Mengubah alpha menuju target alpha
        dotImage.color = Color.Lerp(dotImage.color, new Color(dotImage.color.r, dotImage.color.g, dotImage.color.b, targetAlpha), fadeSpeed * Time.deltaTime);
    }

    // Fungsi untuk mengatur target alpha
    public void SetTargetAlpha(float alpha)
    {
        targetAlpha = alpha;
    }
}

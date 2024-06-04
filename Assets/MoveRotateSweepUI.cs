using UnityEngine;
using UnityEngine.UI;

public class MoveRotateSweepUI : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 50f; // Kecepatan rotasi gambar

    void Update()
    {
        // Rotasi gambar sepanjang sumbu Z
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }
}

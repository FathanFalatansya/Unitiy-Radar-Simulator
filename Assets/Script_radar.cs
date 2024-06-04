using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_radar : MonoBehaviour
{
    [SerializeField] private Transform pfRadarDot; // Prefab untuk titik radar
    [SerializeField] private LayerMask radarLayerMask; // Layer yang akan dideteksi oleh radar
    [SerializeField] private Transform radarCenter; // Objek yang akan dijadikan pusat radar
    [SerializeField] private Transform[] targets; // Array objek yang akan dilacak

    private Transform sweepTransform; // Transform garis radar
    private float rotationSpeed; // Kecepatan rotasi radar
    private float radarDistance; // Jarak maksimum deteksi radar
    private List<GameObject> detectedObjects = new List<GameObject>(); // Daftar objek yang terdeteksi

    private void Awake()
    {
        sweepTransform = transform.Find("Sweep");
        rotationSpeed = 180f;
        radarDistance = 150f;
    }

    private void Update()
    {
        RotateRadar();

        DetectObjects();
    }

    private void RotateRadar()
    {
        sweepTransform.eulerAngles -= new Vector3(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void DetectObjects()
    {
        // Buat raycast dari posisi radar sejauh radarDistance
        RaycastHit[] hits = Physics.RaycastAll(transform.position, sweepTransform.forward, radarDistance, radarLayerMask);

        foreach (RaycastHit hit in hits)
        {
            if (!detectedObjects.Contains(hit.collider.gameObject))
            {
                // Tambahkan objek yang terdeteksi ke dalam daftar
                detectedObjects.Add(hit.collider.gameObject);

                // Tampilkan titik merah pada posisi tabrakan
                GameObject radarDotObject = Instantiate(pfRadarDot.gameObject, hit.point, Quaternion.identity);
                radarDotObject.transform.SetParent(transform);

                // Beri warna merah pada titik radar
                radarDotObject.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    public void SetRadarCenter(Transform center)
    {
        // Set posisi radar ke posisi center
        transform.position = center.position;
    }

    public void ClearDetectedObjects()
    {
        // Hapus semua objek yang terdeteksi dari daftar
        detectedObjects.Clear();

        // Hapus semua titik radar dari tampilan
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void AdjustRotationSpeed(float speedChange)
    {
        // Sesuaikan kecepatan rotasi radar
        rotationSpeed += speedChange;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;

public class RadarSystem : MonoBehaviour
{
    [SerializeField]
    private Transform radarCenter;

    [SerializeField]
    private Transform sweepTransform;

    [SerializeField, Min(1)]
    private float range = 20f;

    [SerializeField]
    private float rotationSpeed = 300f;

    [SerializeField]
    private RectTransform radarUIContainer;

    [SerializeField]
    private GameObject radarDotPrefab;

    [SerializeField]
    private float fadeOutDelay = 1f;

    private List<Transform> detectedObjects = new List<Transform>();
    private Dictionary<Transform, GameObject> radarDots = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, Vector3> previousPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, float> previousTime = new Dictionary<Transform, float>();

    private MqttClient client;

    void Start()
    {
        // Inisialisasi MQTT Client
        client = new MqttClient("172.22.155.65", 1883, false, null, null, MqttSslProtocols.None);
        string clientId = System.Guid.NewGuid().ToString();
        client.Connect(clientId);

        Debug.Log("Connected to MQTT Broker");
    }

    void Update()
    {
        RotateSweep();
        DetectObjects();
        UpdateRadarDots();
    }

    void RotateSweep()
    {
        if (sweepTransform != null)
        {
            sweepTransform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
    }

    void DetectObjects()
    {
        detectedObjects.Clear();

        Collider[] hits = Physics.OverlapSphere(radarCenter.position, range);
        foreach (Collider hit in hits)
        {
            if (hit.transform != radarCenter && !hit.transform.IsChildOf(radarCenter))
            {
                detectedObjects.Add(hit.transform);
            }
        }
    }

    void UpdateRadarDots()
    {

        List<Transform> keys = new List<Transform>(radarDots.Keys);
        foreach (var key in keys)
        {
            if (!detectedObjects.Contains(key))
            {
                Destroy(radarDots[key]);
                radarDots.Remove(key);
                previousPositions.Remove(key);
                previousTime.Remove(key);
            }
        }

        foreach (var detectedObject in detectedObjects)
        {
            Vector2 radarPosition = CalculateRadarPosition(detectedObject.position);
            float angle = Vector3.Angle(sweepTransform.up, detectedObject.position - radarCenter.position);

            if (angle < 5f)
            {
                float distance = Vector3.Distance(radarCenter.position, detectedObject.position);
                float bearing = CalculateBearing(radarCenter.position, detectedObject.position);

                Vector3 previousPosition;
                float previousTimestamp;
                if (previousPositions.TryGetValue(detectedObject, out previousPosition) && previousTime.TryGetValue(detectedObject, out previousTimestamp))
                {
                    float timeDifference = Time.time - previousTimestamp;
                    float relativeSpeed = Vector3.Distance(detectedObject.position, previousPosition) / timeDifference;
                    float acceleration = relativeSpeed / timeDifference;
                    float speedInKnots = relativeSpeed * 1.94384f;

                    // Kirim data ke broker MQTT
                    string message = $"{distance},{bearing},{speedInKnots},{acceleration}";
                    client.Publish("radar_data", Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                    Debug.Log("Sent to MQTT: " + message);
                }

                previousPositions[detectedObject] = detectedObject.position;
                previousTime[detectedObject] = Time.time;

                if (!radarDots.ContainsKey(detectedObject))
                {
                    GameObject radarDot = Instantiate(radarDotPrefab, radarUIContainer);
                    radarDot.GetComponent<RectTransform>().anchoredPosition = radarPosition;
                    radarDots[detectedObject] = radarDot;
                    radarDot.GetComponent<RadarDotFade>().SetTargetAlpha(1f);
                }
                else
                {
                    radarDots[detectedObject].GetComponent<RectTransform>().anchoredPosition = radarPosition;
                    radarDots[detectedObject].GetComponent<RadarDotFade>().SetTargetAlpha(1f);
                }
            }
            else
            {
                if (radarDots.ContainsKey(detectedObject))
                {
                    radarDots[detectedObject].GetComponent<RadarDotFade>().SetTargetAlpha(0f);
                    Destroy(radarDots[detectedObject], fadeOutDelay);
                    radarDots.Remove(detectedObject);
                    previousPositions.Remove(detectedObject);
                    previousTime.Remove(detectedObject);
                }
            }
        }
    }

    Vector2 CalculateRadarPosition(Vector3 objectPosition)
    {
        Vector3 offset = objectPosition - radarCenter.position;
        float distance = offset.magnitude;
        if (distance > range) return Vector2.zero;

        Vector2 radarPosition = new Vector2(offset.x, offset.z) / range * (radarUIContainer.rect.width / 2f);
        return radarPosition;
    }

    float CalculateBearing(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = toPosition - fromPosition;
        float angle = Mathf.Atan2(direction.x, direction.z);
        return angle;
    }

    void OnDestroy()
    {
        if (client != null && client.IsConnected)
        {
            client.Disconnect();
        }
    }
}

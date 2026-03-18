using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightFlicker : MonoBehaviour
{
    public Light lanternLight;

    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;

    public float flickerSpeed = 0.1f;

    private float _randomOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _randomOffset = Random.Range(0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, _randomOffset);
        lanternLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}

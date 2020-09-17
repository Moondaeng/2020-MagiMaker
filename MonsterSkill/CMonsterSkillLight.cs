using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMonsterSkillLight : MonoBehaviour
{
    [System.Serializable]
    public struct RangeOfIntegers
    {
        public int Minimum;
        public int Maximum;
    }

    [System.Serializable]
    public struct RangeOfFloats
    {
        public float Minimum;
        public float Maximum;
    }
    
    [Tooltip("Random seed for movement, 0 for no movement.")]
    public float Seed = 100.0f;

    [Tooltip("Multiplier for light intensity.")]
    public float IntensityModifier = 2.0f;
    
    public RangeOfFloats IntensityMaxRange = new RangeOfFloats { Minimum = 0.0f, Maximum = 8.0f };

    private Light firePointLight;
    private float lightIntensity;
    private float seed;
    private CMonsterSkillBase BaseScript;
    private float baseY;

    private void Awake()
    {
        // find a point light
        firePointLight = gameObject.GetComponentInChildren<Light>();
        if (firePointLight != null)
        {
            // we have a point light, set the intensity to 0 so it can fade in nicely
            lightIntensity = firePointLight.intensity;
            firePointLight.intensity = 0.0f;
            baseY = firePointLight.gameObject.transform.position.y;
        }
        seed = UnityEngine.Random.value * Seed;
        BaseScript = gameObject.GetComponent<CMonsterSkillBase>();
    }

    private void Update()
    {
        if (firePointLight == null)
        {
            return;
        }

        if (seed != 0)
        {
            // we have a random movement seed, set up with random movement
            bool setIntensity = true;
            float intensityModifier2 = 1.0f;
            if (BaseScript != null)
            {
                if (BaseScript.Stopping)
                {
                    // don't randomize intensity during a stop, it looks bad
                    setIntensity = false;
                    firePointLight.intensity = Mathf.Lerp(firePointLight.intensity, 0.0f, BaseScript.StopPercent);
                }
                else if (BaseScript.Starting)
                {
                    intensityModifier2 = BaseScript.StartPercent;
                }
            }

            if (setIntensity)
            {
                float intensity = Mathf.Clamp(IntensityModifier * intensityModifier2 * Mathf.PerlinNoise(seed + Time.time, seed + 1 + Time.time),
                    IntensityMaxRange.Minimum, IntensityMaxRange.Maximum);
                firePointLight.intensity = intensity;
            }

            // random movement with perlin noise
            float x = Mathf.PerlinNoise(seed + 0 + Time.time * 2, seed + 1 + Time.time * 2) - 0.5f;
            float y = baseY + Mathf.PerlinNoise(seed + 2 + Time.time * 2, seed + 3 + Time.time * 2) - 0.5f;
            float z = Mathf.PerlinNoise(seed + 4 + Time.time * 2, seed + 5 + Time.time * 2) - 0.5f;
            firePointLight.gameObject.transform.localPosition = Vector3.up + new Vector3(x, y, z);
        }
        else if (BaseScript.Stopping)
        {
            // fade out
            firePointLight.intensity = Mathf.Lerp(firePointLight.intensity, 0.0f, BaseScript.StopPercent);
        }
        else if (BaseScript.Starting)
        {
            // fade in
            firePointLight.intensity = Mathf.Lerp(0.0f, lightIntensity, BaseScript.StartPercent);
        }
    }
}

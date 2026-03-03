using UnityEngine;

public class HeatmapPulse : MonoBehaviour
{
    [Header("Assign your heatmap material here")]
    public Material heatmapMaterial;

    [Header("Pulse Settings")]
    [Range(0f, 1f)] public float minAlpha = 0.2f;
    [Range(0f, 1f)] public float maxAlpha = 0.6f;
    public float pulseSpeed = 1.5f;

    [Header("Color Settings")]
    public Color coldColor = new Color(0f, 1f, 0.3f, 1f); // green-ish
    public Color hotColor  = new Color(1f, 0f, 0f, 1f);   // red

    [Header("Emission")]
    public bool enableEmission = true;
    public float emissionIntensity = 2.0f;

    Renderer _renderer;
    Material _runtimeMat;

    void Start()
    {
        _renderer = GetComponent<Renderer>();

        // 如果你没手动拖材质，就自动从 Renderer 上拿
        if (heatmapMaterial == null && _renderer != null)
            heatmapMaterial = _renderer.sharedMaterial;

        // 运行时复制一份材质，避免改到工程里原材质
        if (heatmapMaterial != null)
        {
            _runtimeMat = new Material(heatmapMaterial);
            _renderer.material = _runtimeMat;
        }
    }

    void Update()
    {
        if (_runtimeMat == null) return;

        // 0~1 的循环变化
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        // 颜色：绿 -> 红
        Color c = Color.Lerp(coldColor, hotColor, t);

        // 透明度：minAlpha -> maxAlpha
        float a = Mathf.Lerp(minAlpha, maxAlpha, t);
        c.a = a;

        // BaseMap（URP Unlit/Lit 通用）
        _runtimeMat.SetColor("_BaseColor", c);

        // Emission
        if (enableEmission)
        {
            _runtimeMat.EnableKeyword("_EMISSION");
            _runtimeMat.SetColor("_EmissionColor", c * emissionIntensity);
        }
        else
        {
            _runtimeMat.DisableKeyword("_EMISSION");
        }
    }
}

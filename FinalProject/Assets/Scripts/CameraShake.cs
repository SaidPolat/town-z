using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class CameraShake : MonoBehaviour
{
    PostProcessLayer layer;
    public TMP_Dropdown aaDropdown;
    public static int dropdownValue = 3;

    void Start()
    {
        layer = gameObject.GetComponent<PostProcessLayer>();
        aaDropdown.onValueChanged.AddListener(delegate { AADropdownChange(); });
    }

    void Update()
    {
        if (dropdownValue == 0)
            layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
        if (dropdownValue == 1)
            layer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        if (dropdownValue == 2)
            layer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
        if (dropdownValue == 3)
            layer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
    }

    public void AADropdownChange()
    {
        dropdownValue = aaDropdown.value;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        //transform.localPosition = originalPos;
        transform.localPosition = new Vector3(0f, 0f, 0f);
    }
}

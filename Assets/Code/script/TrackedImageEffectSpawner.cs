using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageEffectSpawner : MonoBehaviour
{
    public GameObject effectPrefab;   // 你要出现的特效 prefab
    public bool hideWhenNotTracking = true;

    ARTrackedImageManager _manager;
    readonly Dictionary<TrackableId, GameObject> _spawned = new();

    void Awake()
    {
        _manager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        _manager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        _manager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added)
            SpawnOrUpdate(img);

        foreach (var img in args.updated)
            SpawnOrUpdate(img);

        foreach (var img in args.removed)
            Remove(img);
    }

    void SpawnOrUpdate(ARTrackedImage img)
    {
        if (effectPrefab == null) return;

        if (!_spawned.TryGetValue(img.trackableId, out var go) || go == null)
        {
            go = Instantiate(effectPrefab, img.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            _spawned[img.trackableId] = go;
        }

        // 跟随图片
        go.transform.SetParent(img.transform, false);

        // 根据 tracking 状态显示/隐藏
        if (hideWhenNotTracking)
        {
            bool shouldShow = img.trackingState == TrackingState.Tracking;
            go.SetActive(shouldShow);
        }
        else
        {
            go.SetActive(true);
        }
    }

    void Remove(ARTrackedImage img)
    {
        if (_spawned.TryGetValue(img.trackableId, out var go) && go != null)
            Destroy(go);

        _spawned.Remove(img.trackableId);
    }
}
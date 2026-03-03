using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageSpawner : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject contentPrefab;

    private readonly Dictionary<string, GameObject> spawned = new();

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added)
            Upsert(img);

        foreach (var img in args.updated)
            Upsert(img);

        foreach (var img in args.removed)
        {
            var name = img.referenceImage.name;
            if (spawned.TryGetValue(name, out var go))
            {
                Destroy(go);
                spawned.Remove(name);
            }
        }
    }

    private void Upsert(ARTrackedImage img)
    {
        var name = img.referenceImage.name;

        if (!spawned.TryGetValue(name, out var go))
        {
            go = Instantiate(contentPrefab, img.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            spawned[name] = go;
        }

        go.SetActive(img.trackingState == TrackingState.Tracking);
    }
}
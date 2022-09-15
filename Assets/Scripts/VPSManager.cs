using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System;

public class VPSManager : MonoBehaviour
{
    [SerializeField]private AREarthManager em;
    [Serializable]
    public struct GeospatialObject
    {
        public GameObject objectPrefab;
        public EarthPosition earthPosition;
    }
    [Serializable]
    public struct EarthPosition
    {
        public double latitud;
        public double longitud;
        public double altitud;

    }
    [SerializeField]
    private List<GeospatialObject> geospatialObjects = new List<GeospatialObject>();
    [SerializeField]
    private ARAnchorManager aRAnchorManager;
    private void Start()
    {
        verifySupport();
    }
    private void verifySupport()
    {
        var res = em.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (res)
        {
            case FeatureSupported.Supported:
                Debug.Log("ready to use VPS");
                placeObjects();
                break;
            case FeatureSupported.Unknown:
                Debug.Log("Unknown...");
                Invoke("verifySupport", 3.0f);
                break;
            case FeatureSupported.Unsupported:
                Debug.Log("VPS not supported");
                break;
        }
    }
    private void placeObjects()
    {
        if (em.EarthTrackingState == TrackingState.Tracking)
        {
            var geospatialPose = em.CameraGeospatialPose;
            foreach(var obj in geospatialObjects)
            {
                var earthPosition = obj.earthPosition;
                var objAnchor = ARAnchorManagerExtensions.AddAnchor(aRAnchorManager,earthPosition.latitud, earthPosition.longitud, earthPosition.altitud,Quaternion.identity);
                Instantiate(obj.objectPrefab, objAnchor.transform.position,Quaternion.identity, objAnchor.transform);
            }
        }
        else if(em.EarthTrackingState == TrackingState.None)
        {
            Invoke("placeObjects", 5.0f);
        }
    }
}

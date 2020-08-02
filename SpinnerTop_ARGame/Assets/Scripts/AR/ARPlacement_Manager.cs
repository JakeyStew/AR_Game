using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement_Manager : MonoBehaviour
{
    ARRaycastManager _ARRaycastManager;
    static List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();
    
    public Camera _arCamera;
    public GameObject _battleAreanGameObject;

    private void Awake()
    {
        _ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 centerOfScreen = new Vector3(Screen.width /2, Screen.height / 2);
        Ray ray = _arCamera.ScreenPointToRay(centerOfScreen);
        if(_ARRaycastManager.Raycast(ray, _raycastHits, TrackableType.PlaneWithinPolygon))
        {
            //Intersection
            Pose hitPose = _raycastHits[0].pose;
            Vector3 positionToBePlaced = hitPose.position;
            _battleAreanGameObject.transform.position = positionToBePlaced;
        }
    }
}

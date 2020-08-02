using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARPlacement_Position_Controller : MonoBehaviour
{
    ARPlaneManager _ARPlaneManager;
    ARPlacement_Manager _ARPlacementManager;

    public GameObject _placeButton;
    public GameObject _adjustButton;

    public GameObject _searchForGameButton;
    public GameObject _scaleSlider;

    public TextMeshProUGUI _informPanel;

    private void Awake()
    {
        _ARPlaneManager = GetComponent<ARPlaneManager>();
        _ARPlacementManager = GetComponent<ARPlacement_Manager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _placeButton.SetActive(true);
        _scaleSlider.SetActive(true);
        _adjustButton.SetActive(false);
        _searchForGameButton.SetActive(false);

        _informPanel.text = "Move phone to detect planes and place the battle arena!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableARPlacementAndDetection()
    {
        _ARPlaneManager.enabled = false;
        _ARPlacementManager.enabled = false;

        SetAllPlanesActiveOrDeactive(false);
        _placeButton.SetActive(false);
        _scaleSlider.SetActive(false);
        _adjustButton.SetActive(true);
        _searchForGameButton.SetActive(true);

        _informPanel.text = "Now search for games to battle!";
    }

    public void EnableARPlacementAndDetection()
    {
        _ARPlaneManager.enabled = true;
        _ARPlacementManager.enabled = true;

        SetAllPlanesActiveOrDeactive(true);
        _placeButton.SetActive(true);
        _scaleSlider.SetActive(true);
        _adjustButton.SetActive(false);
        _searchForGameButton.SetActive(false);

        _informPanel.text = "Move phone to detect planes and place the battle arena!";
    }



    private void SetAllPlanesActiveOrDeactive(bool value)
    {
        foreach (var plane in _ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
}

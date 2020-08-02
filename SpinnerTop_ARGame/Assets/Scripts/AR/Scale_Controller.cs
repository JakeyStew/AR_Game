using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class Scale_Controller : MonoBehaviour
{
    ARSessionOrigin _ARSessionOrigin;
    public Slider _scaleSlider;

    private void Awake()
    {
        _ARSessionOrigin = GetComponent<ARSessionOrigin>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _scaleSlider.onValueChanged.AddListener(OnScaleSliderChanged);
    }

    private void OnScaleSliderChanged(float value)
    {
        if(_scaleSlider != null)
        {
            _ARSessionOrigin.transform.localScale = Vector3.one / value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

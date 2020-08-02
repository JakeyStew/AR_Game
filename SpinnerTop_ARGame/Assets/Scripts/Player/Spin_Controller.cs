using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin_Controller : MonoBehaviour
{
    [SerializeField]
    public float _spinSpeed = 3600;
    [SerializeField]
    private bool _doSpin = false;
    [SerializeField]
    private GameObject _playerGraphics;

    private Rigidbody _rb;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_doSpin)
        {
            _playerGraphics.transform.Rotate(new Vector3(0, _spinSpeed * Time.deltaTime, 0));
        }
    }
}

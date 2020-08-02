using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Controller : MonoBehaviour
{
    [SerializeField]
    public Joystick joystick;
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private float _maxVelocityChange = 4.0f;

    [SerializeField]
    private float _tiltAmount = 10f;

    private Vector3 _velocityVector = Vector3.zero;
    private Rigidbody _playerRb;
    void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float xMovementInput = joystick.Horizontal;
        float zMovementInput = joystick.Vertical;

        Vector3 movementHorizontal = transform.right * xMovementInput;
        Vector3 movementVertical = transform.forward * zMovementInput;

        Vector3 movementVelocity = (movementHorizontal + movementVertical).normalized * _speed;
        Move(movementVelocity);
        //Multiple horizontal by -1 to get the inverse of the horinzontal input
        transform.rotation = Quaternion.Euler(joystick.Vertical * _speed * _tiltAmount, 0, -1 * joystick.Horizontal * _speed * _tiltAmount);
    }

    private void Move(Vector3 movementVelocity)
    {
        _velocityVector = movementVelocity;
    }

    private void FixedUpdate()
    {
        if(_velocityVector != Vector3.zero)
        {
            Vector3 velocity = _playerRb.velocity;
            Vector3 velocityChange = (_velocityVector - velocity);

            //Restrict the amount of force you can apply
            velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);

            _playerRb.AddForce(velocityChange, ForceMode.Acceleration);
        }
    }

}

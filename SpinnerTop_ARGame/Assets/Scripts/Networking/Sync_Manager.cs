using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sync_Manager : MonoBehaviourPun, IPunObservable
{
    private Rigidbody _rb;
    private PhotonView _photonView;

    private Vector3 _networkPos;
    private Quaternion _networkRotation;

    public bool _syncVelocity = true;
    public bool _syncAngularVelocity = true;
    public bool _isTeleportEnabled = true;
    public float _teleportDistanceIfGreaterThan = 1.0f;

    private float _distance;
    private float _angle;


    private GameObject _battleArenaObject;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();

        _networkPos = new Vector3();
        _networkRotation = new Quaternion();

        _battleArenaObject = GameObject.Find("BattleArena");
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine)
        {
            _rb.position = Vector3.MoveTowards(_rb.position, _networkPos, _distance * (1.0f / PhotonNetwork.SerializationRate));
            _rb.rotation = Quaternion.RotateTowards(_rb.rotation, _networkRotation, _angle * (1.0f / PhotonNetwork.SerializationRate));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Then, photonView is mine and I am the one who controls this player
            //Should send position, velocity etc... data to the other players
            stream.SendNext(_rb.position - _battleArenaObject.transform.position);
            stream.SendNext(_rb.rotation);

            if(_syncVelocity == true)
            {
                stream.SendNext(_rb.velocity);
            }
            if (_syncAngularVelocity == true)
            {
                stream.SendNext(_rb.angularVelocity);
            }
        }
        else if(stream.IsReading)
        {
            //On my player gameobject that exists in remote players game
            _networkPos = (Vector3)stream.ReceiveNext();
            _networkRotation = (Quaternion)stream.ReceiveNext();

            if(_isTeleportEnabled)
            {
                if(Vector3.Distance(_rb.position, _networkPos) > _teleportDistanceIfGreaterThan)
                {
                    _rb.position = _networkPos;
                }
            }

            if(_syncVelocity || _syncAngularVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                if(_syncVelocity == true)
                {
                    _rb.velocity = (Vector3)stream.ReceiveNext();
                    _networkPos += _rb.velocity * lag;

                    _distance = Vector3.Distance(_rb.position, _networkPos);
                }

                if (_syncAngularVelocity == true)
                {
                    _rb.angularVelocity = (Vector3)stream.ReceiveNext()+_battleArenaObject.transform.position;
                    _networkRotation = Quaternion.Euler(_rb.angularVelocity * lag) * _networkRotation;
                    _angle = Quaternion.Angle(_rb.rotation, _networkRotation);
                }
            }
        }
    }
}

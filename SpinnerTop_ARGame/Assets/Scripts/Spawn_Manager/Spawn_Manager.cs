using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Spawn_Manager : MonoBehaviourPunCallbacks
{
    public enum RaiseEventCode
    {
        PlayerSpawnEventCode = 0
    }
    public GameObject[] _playerPrefabs;
    public Transform[] _spawnPositions;

    public GameObject _battleAreanaGameObject;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    #region PHOTON Callback Methods
    void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventCode.PlayerSpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 recievedPos = (Vector3)data[0];
            Quaternion recievedRotation = (Quaternion)data[1];
            int recievedPlayerSelecitonData = (int)data[3];

            GameObject playerGameObject = Instantiate(_playerPrefabs[recievedPlayerSelecitonData], recievedPos + _battleAreanaGameObject.transform.position, recievedRotation);
            PhotonView photonView = playerGameObject.GetComponent<PhotonView>();
            photonView.ViewID = (int)data[2];

        }
    }
    
    public override void OnJoinedRoom()
    {
        //if (PhotonNetwork.IsConnectedAndReady)
        //{
        //    object playerSelectionNumber;
        //    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
        //    {
        //        Debug.Log(("Player Number = " + (int)playerSelectionNumber));
        //        int randomSpanwPoint = Random.Range(0, _spawnPositions.Length - 1);
        //        Vector3 instantiatePos = _spawnPositions[randomSpanwPoint].position; 
        //        PhotonNetwork.Instantiate(_playerPrefabs[(int)playerSelectionNumber].name, instantiatePos, Quaternion.identity);
        //    }
        //}
        //base.OnJoinedRoom();
        SpawnPlayer();
    }
    #endregion

    #region Private Methods
    private void SpawnPlayer()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log(("Player Number = " + (int)playerSelectionNumber));
                int randomSpanwPoint = Random.Range(0, _spawnPositions.Length - 1);
                Vector3 instantiatePos = _spawnPositions[randomSpanwPoint].position;

                GameObject playerGameobject = Instantiate(_playerPrefabs[(int)playerSelectionNumber], instantiatePos, Quaternion.identity);
                PhotonView photonView = playerGameobject.GetComponent<PhotonView>();

                if (PhotonNetwork.AllocateViewID(photonView))
                {
                    object[] data = new object[]
                    {
                        playerGameobject.transform.position - _battleAreanaGameObject.transform.position,
                        playerGameobject.transform.rotation, 
                        photonView.ViewID, 
                        playerSelectionNumber
                    };

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.Others,
                        CachingOption = EventCaching.AddToRoomCache
                    };
                    SendOptions sendOptions = new SendOptions
                    {
                        Reliability = true
                    };

                    //Raise events!
                    PhotonNetwork.RaiseEvent((byte)RaiseEventCode.PlayerSpawnEventCode, data, raiseEventOptions, sendOptions);
                }
                else
                {
                    Destroy(playerGameobject);
                }
            }
        }
    }
    #endregion
}

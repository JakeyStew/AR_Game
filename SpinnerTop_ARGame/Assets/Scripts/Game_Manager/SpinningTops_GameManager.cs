using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class SpinningTops_GameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject _uiInformPanelgameObject;
    public TextMeshProUGUI _uiInformtext;
    public GameObject _searchForGameButton;
    public GameObject _adjustButton;
    public GameObject _raycastCenterImage;

    // Start is called before the first frame update
    void Start()
    {
        _uiInformPanelgameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region UI Callbacks Method
    public void JoinRandomRoom()
    {
        _uiInformtext.text = "Searching for Available Rooms...";
        PhotonNetwork.JoinRandomRoom();
        _searchForGameButton.SetActive(false);
    }

    public void OnQuitMatchButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Scene_Manager.Instance.LoadScene("Scene_Lobby");
        }
    }
    #endregion

    #region Photon Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
        _uiInformtext.text = message;
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        _adjustButton.SetActive(false);
        _raycastCenterImage.SetActive(false);
        //base.OnJoinedRoom();
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            _uiInformtext.text = "Joined to " + PhotonNetwork.CurrentRoom.Name + ". Waiting for Other Players...";
        }
        else
        {
            _uiInformtext.text = "Joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeaxtivateAfterSeconds(_uiInformPanelgameObject, 2.0f));
        }
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + ". \nPlayer count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        _uiInformtext.text = newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + ". \nPlayer count: " + PhotonNetwork.CurrentRoom.PlayerCount;
        StartCoroutine(DeaxtivateAfterSeconds(_uiInformPanelgameObject, 2.0f));
    }

    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        Scene_Manager.Instance.LoadScene("Scene_Lobby");
    }
    #endregion

    #region Private Methods
    private void CreateAndJoinRoom()
    {
        string RandomRoomName = "Room: " + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        //Creating room
        PhotonNetwork.CreateRoom(RandomRoomName, roomOptions);
    }

    IEnumerator DeaxtivateAfterSeconds(GameObject gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
    #endregion
}

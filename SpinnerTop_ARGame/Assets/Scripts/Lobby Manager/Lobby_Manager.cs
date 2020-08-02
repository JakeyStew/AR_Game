using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Lobby_Manager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField _playerNameInputField;
    public GameObject _uiLoginGameObject;

    [Header("Lobvby UI")]
    public GameObject _uiLobbyGameObject;
    public GameObject _ui3DGameObject;

    [Header("Connection Status UI")]
    public GameObject _uiConnecitonStatusGameObject;
    public Text _connecitonStatusText;
    public bool _showCOnnectionStatus = false;

    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            _uiLobbyGameObject.SetActive(true);
            _ui3DGameObject.SetActive(true);
            _uiConnecitonStatusGameObject.SetActive(false);
            _uiLoginGameObject.SetActive(false);
        }
        else
        {
            _uiLobbyGameObject.SetActive(false);
            _ui3DGameObject.SetActive(false);
            _uiConnecitonStatusGameObject.SetActive(false);
            _uiLoginGameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_showCOnnectionStatus)
        {
            _connecitonStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }
    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonCLicked()
    {
        string playerName = _playerNameInputField.text;
        if(!string.IsNullOrEmpty(playerName))
        {
            _uiLobbyGameObject.SetActive(false);
            _ui3DGameObject.SetActive(false);
            _uiLoginGameObject.SetActive(false);

            _showCOnnectionStatus = true;
            _uiConnecitonStatusGameObject.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;

                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player Name is Invalid or Empty!");
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        Scene_Manager.Instance.LoadScene("Scene_PlayerSelection");

    }
    #endregion

    #region PHOTON Callback Methods
    public override void OnConnected()
    {
        Debug.Log("We connected to the internet.");
        //base.OnConnected();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to the network.");
        //base.OnConnectedToMaster();
        _uiLobbyGameObject.SetActive(true);
        _ui3DGameObject.SetActive(true);
        _uiLoginGameObject.SetActive(false);
        _uiConnecitonStatusGameObject.SetActive(false);
    }

    #endregion
}

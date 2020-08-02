using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Player_Selection_Manager : MonoBehaviour
{
    public Transform _playerSwitcherTransform;
    public int _playerSelectionNumber;
    public GameObject[] _spinnerTopModels;

    [Header("UI")]
    public TextMeshProUGUI _playerModelText;
    public Button _nextButton;
    public Button _prevButton;

    public GameObject _uiSelection;
    public GameObject _uiAfterSelection;
    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        _uiSelection.SetActive(true);
        _uiAfterSelection.SetActive(false);
        _playerSelectionNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods
    public void NextPlayer()
    {
        _playerSelectionNumber += 1;
        if(_playerSelectionNumber >= +_spinnerTopModels.Length)
        {
            _playerSelectionNumber = 0;
        }
        _nextButton.enabled = false;
        _prevButton.enabled = false;
        StartCoroutine(Rotate(Vector3.up, _playerSwitcherTransform, 90, 1.0f));

        if(_playerSelectionNumber == 0 || _playerSelectionNumber == 1)
        {
            //This means the player type is Attack
            _playerModelText.text = "Attack";
        }
        else 
        {
            _playerModelText.text = "Defense";
        }
    }

    public void PreviousPLayer()
    {
        _playerSelectionNumber -= 1;
        if (_playerSelectionNumber < 0)
        {
            _playerSelectionNumber = _spinnerTopModels.Length - 1;
        }
        _nextButton.enabled = false;
        _prevButton.enabled = false;
        StartCoroutine(Rotate(Vector3.up, _playerSwitcherTransform, -90, 1.0f));

        if (_playerSelectionNumber == 2 || _playerSelectionNumber == 3)
        {
            //This means the player type is Attack
            _playerModelText.text = "Defense";
        }
        else
        {
            _playerModelText.text = "Attack";
        }
    }

    public void OnSelectButtonClicked()
    {
        _uiSelection.SetActive(false);
        _uiAfterSelection.SetActive(true);

        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, _playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void OnReSelectButtonClicked()
    {
        _uiSelection.SetActive(true);
        _uiAfterSelection.SetActive(false);
    }

    public void OnBattleButtonClicked()
    {
        Scene_Manager.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnBackButtonClicked()
    {
        Scene_Manager.Instance.LoadScene("Scene_Lobby");
    }
    #endregion

    #region Private Methods
    IEnumerator Rotate(Vector3 axis, Transform transformRotate, float angle, float duration = 1.0f)
    {
        Quaternion originalRotation = transformRotate.rotation;
        Quaternion finalRotation = transformRotate.rotation * Quaternion.Euler(axis * angle);

        float elapsedTime = 0.0f;
        while(elapsedTime < duration)
        {
            transformRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transformRotate.rotation = finalRotation;
        _nextButton.enabled = true;
        _prevButton.enabled = true;
    }
    #endregion
}

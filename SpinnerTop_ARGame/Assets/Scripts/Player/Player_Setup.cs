using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Player_Setup : MonoBehaviourPun
{
    public TextMeshProUGUI _playerNameText;
    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            //The player is local player
            transform.GetComponent<Movement_Controller>().enabled = true;
            transform.GetComponent<Movement_Controller>().joystick.gameObject.SetActive(true);
        }
        else
        {
            //The player is remote player
            transform.GetComponent<Movement_Controller>().enabled = false;
            transform.GetComponent<Movement_Controller>().joystick.gameObject.SetActive(false);
        }
        SetPlayerName();
    }

    private void SetPlayerName()
    {
        if (_playerNameText != null)
        {
            if (photonView.IsMine)
            {
                _playerNameText.text = "YOU";
                _playerNameText.color = Color.red;
            }
            else
            {
                _playerNameText.text = photonView.Owner.NickName;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Combat_Controller : MonoBehaviourPun
{
    public Spin_Controller spinnerScript;
    private Rigidbody _rb;

    public GameObject _ui3DGameobject;
    public GameObject _deathPanelUIPrefab;
    private GameObject _deathPanelUIGameObject;

    private float _startSpinSpeed;
    private float _currentSpinSpeed;
    public Image _spinSpeedBarImage;
    public TextMeshProUGUI _spinSpeedHealth;
    public float _commonDamageCoefficient = 0.04f;

    [Header("Player Type Damage Coefficient")]
    public float _doDamageCoefficientAttacker = 10f;
    public float _getDamageCoefficientAttacker = 1.2f;

    public float _doDamageCoefficientDefender = 0.75f;
    public float _getDamageCoefficientDefender = 0.2f;

    public bool _isAttacker;
    public bool _isDefender;

    private bool _isDead = false;

    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;

    // Start is called before the first frame update
    private void Awake()
    {
        _startSpinSpeed = spinnerScript._spinSpeed;
        _currentSpinSpeed = spinnerScript._spinSpeed;
        _spinSpeedBarImage.fillAmount = _currentSpinSpeed / _startSpinSpeed;
    }

    private void CheckPlayerType()
    {
        if(gameObject.name.Contains("Attacker"))
        {
            _isAttacker = true;
            _isDefender = false;
        } 
        else if (gameObject.name.Contains("Defender"))
        {
            _isAttacker = false;
            _isDefender = true;

            spinnerScript._spinSpeed = 4400;
            _startSpinSpeed = spinnerScript._spinSpeed;
            _currentSpinSpeed = spinnerScript._spinSpeed;

            _spinSpeedHealth.text = _currentSpinSpeed + "/" + _startSpinSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);

                //Instantiate Collision Effect ParticleSystem
                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    //De-activate Collision Effect Particle System after some seconds.
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));

                }
            }

            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if(mySpeed > otherPlayerSpeed)
            {
                float defaultDamageAmount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * _commonDamageCoefficient;
                if (_isAttacker)
                {
                    defaultDamageAmount *= _doDamageCoefficientAttacker;
                }
                else if (_isDefender)
                {
                    defaultDamageAmount *= _doDamageCoefficientDefender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    //Apply damage to the slower player
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, defaultDamageAmount);
                }
            }
        }
    }

    [PunRPC]
    public void DoDamage(float damageAmount)
    {
        if (!_isDead)
        {
            if (_isAttacker)
            {
                damageAmount *= _doDamageCoefficientAttacker;
                if(damageAmount > 1000)
                {
                    damageAmount = 400f;
                }
            }
            else if (_isDefender)
            {
                damageAmount *= _doDamageCoefficientDefender;
            }

            spinnerScript._spinSpeed -= damageAmount;
            _currentSpinSpeed = spinnerScript._spinSpeed;

            _spinSpeedBarImage.fillAmount = _currentSpinSpeed / _startSpinSpeed;
            _spinSpeedHealth.text = _currentSpinSpeed.ToString("F0") + "/" + _startSpinSpeed;

            if (_currentSpinSpeed < 100f)
            {
                //Die
                Die();
            }
        }
    }

    private void Die()
    {
        _isDead = true;
        GetComponent<Movement_Controller>().enabled = false;
        _rb.freezeRotation = false;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        spinnerScript._spinSpeed = 0.0f;
        _ui3DGameobject.SetActive(false);

        if(photonView.IsMine)
        {
            //Countdown for respawn
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        GameObject canvasGamObject = GameObject.Find("Canvas");
        if(_deathPanelUIGameObject == null)
        {
            _deathPanelUIGameObject = Instantiate(_deathPanelUIPrefab, canvasGamObject.transform);
        }
        else
        {
            _deathPanelUIGameObject.SetActive(true);
        }

        Text respawnText = _deathPanelUIGameObject.transform.Find("RespawnTimeText").GetComponent<Text>();
        float respawnTime = 8.0f;
        respawnText.text = respawnTime.ToString(".00");

        while(respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnText.text = respawnTime.ToString(".00");

            GetComponent<Movement_Controller>().enabled = false;
        }

        _deathPanelUIGameObject.SetActive(false);
        GetComponent<Movement_Controller>().enabled = true;
        photonView.RPC("ReBorn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ReBorn()
    {
        spinnerScript._spinSpeed = _startSpinSpeed;
        _currentSpinSpeed = spinnerScript._spinSpeed;

        _spinSpeedBarImage.fillAmount = _currentSpinSpeed / _startSpinSpeed;
        _spinSpeedHealth.text = _currentSpinSpeed + "/" + _startSpinSpeed;

        _rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        _ui3DGameobject.SetActive(true);
        _isDead = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerType();
        _rb = GetComponent<Rigidbody>();
        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peanut : MonoBehaviour
{

    public int _value;

    public GameObject _pickupEffect;

    AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindAnyObjectByType<GameManager>().AddPeanuts(_value);

            _audioManager.Play("Crunch");

            //Instantiate(_pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}

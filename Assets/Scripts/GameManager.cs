using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    AudioManager _audioManager;

    public int _currentPeanuts;
    public int _currentCrates;
    public TextMeshProUGUI _peanutsText;
    public TextMeshProUGUI _cratesText;

    public Image _peanutImage;
    public Image _crateImage;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioManager.Play("Music");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCrates(int cratesToAdd)
    {
        _currentCrates += cratesToAdd;
        _cratesText.text = _currentCrates.ToString();
    }

    public void AddPeanuts(int peanutsToAdd)
    {
        _audioManager.Play("Collect");

        _currentPeanuts += peanutsToAdd;
        _peanutsText.text = _currentPeanuts.ToString();

        // Add a scaling effect
        LeanTween.scale(_peanutsText.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f)
            .setEaseInOutQuad() // You can adjust the easing function
            .setOnComplete(() =>
            {
                // Scale back to the original size
                LeanTween.scale(_peanutsText.gameObject, Vector3.one, 0.2f)
                    .setEaseInOutQuad();
            });

        // Adjusted scaling factors for the image
        float initialImageScale = 0.1442f;
        float scaledUpFactor = 1.2f;
        float scaledDownFactor = 1.0f / scaledUpFactor;

        // Add a scaling effect to the image
        LeanTween.scale(_peanutImage.gameObject, new Vector3(initialImageScale * scaledUpFactor, initialImageScale * scaledUpFactor, initialImageScale * scaledUpFactor), 0.2f)
            .setEaseInOutQuad()
            .setOnComplete(() =>
            {
                // Scale back to the original size
                LeanTween.scale(_peanutImage.gameObject, new Vector3(initialImageScale, initialImageScale, initialImageScale), 0.2f)
                    .setEaseInOutQuad();
            });
    }
}

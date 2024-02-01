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
    public TextMeshProUGUI _timerText;

    public Image _peanutImage;
    public Image _crateImage;

    public GameObject _cratesUI;
    public GameObject _timerUI;
    public GameObject _peanutsUI;

    Animator _cratesAnimator;
    Animator _timerAnimator;
    Animator _peanutsAnimator;

    Coroutine _hideUITimerCoroutine;

    bool _peantusUIvisible = false;

    private bool _timerStarted = false;
    private bool _timerRunning = false;
    private float _startTime;
    private float _endTime;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _timerAnimator = _timerUI.GetComponent<Animator>();
        _peanutsAnimator = _peanutsUI.GetComponent<Animator>();
        _cratesAnimator = _cratesUI.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioManager.Play("Music");
    }

    // Update is called once per frame
    void Update()
    {
        if (_timerRunning)
        {
            float elapsed = Time.time - _startTime;
            int minutes = Mathf.FloorToInt(elapsed / 60); // Calculate minutes
            int seconds = Mathf.FloorToInt(elapsed % 60); // Calculate remaining seconds
            int milliseconds = Mathf.FloorToInt((elapsed * 1000) % 1000); // Calculate milliseconds

            // Display the time in minutes, seconds, and milliseconds
            _timerText.text = string.Format("{0}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        }
    }

    public void AddCrates(int cratesToAdd)
    {
        _currentCrates += cratesToAdd;
        _cratesText.text = _currentCrates + "/68";
    }

    public void AddPeanuts(int peanutsToAdd)
    {
        _audioManager.Play("Collect");

        _currentPeanuts += peanutsToAdd;
        _peanutsText.text = _currentPeanuts.ToString();

        if (!_peantusUIvisible)
        {
            _peanutsAnimator.SetTrigger("ShowUI");
            _peantusUIvisible = true;
        }

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
        float initialImageScale = 0.2f;
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

        ResetHideUITimer();
    }

    void ResetHideUITimer()
    {
        if (_hideUITimerCoroutine != null)
        {
            StopCoroutine(_hideUITimerCoroutine);
        }

        // Start the hide UI timer coroutine
        _hideUITimerCoroutine = StartCoroutine(HideUITimerCoroutine());
    }
    IEnumerator HideUITimerCoroutine()
    {
        yield return new WaitForSeconds(3f); // Adjust the duration as needed

        // Hide the UI after the timer expires
        _peanutsAnimator.SetTrigger("HideUI");

        _peantusUIvisible = false;
    }

    public void StopHideUITimer()
    {
        if (_hideUITimerCoroutine != null)
        {
            StopCoroutine(_hideUITimerCoroutine);
        }
    }

    public void StartTimer()
    {
        if (!_timerStarted)
        {
            _startTime = Time.time;
            _timerRunning = true;
            _timerStarted = true;

            _timerAnimator.SetTrigger("StartTimer");
        }
    }

    public void StopTimer()
    {
        _endTime = Time.time;
        _timerRunning = false;
        ShowElapsedTime();

        _cratesAnimator.SetTrigger("ShowUI");

        // Stop the hide UI timer when the game ends
        StopHideUITimer();
    }

    private void ShowElapsedTime()
    {
        float elapsed = Time.time - _startTime;
        int minutes = Mathf.FloorToInt(elapsed / 60); // Calculate minutes
        int seconds = Mathf.FloorToInt(elapsed % 60); // Calculate remaining seconds
        int milliseconds = Mathf.FloorToInt((elapsed * 1000) % 1000); // Calculate milliseconds

        // Display the time in minutes, seconds, and milliseconds
        _timerText.text = string.Format("{0}:{1:00}.{2:00}", minutes, seconds, milliseconds);


        _timerAnimator.SetTrigger("EndTimer");
    }
}

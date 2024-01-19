using System.Collections;
using UnityEngine;

public class Snake : MonoBehaviour
{

    // ref variables
    Animator _animator;
    public Transform _player;

    public int _damage = 1;

    bool _isAlert;
    bool _isInAttackRange;

    // variables to store optimized setter/getter parameter IDs
    int _isAlertHash;
    int _isAttackingHash;

    // getters and setters
    public int IsAlertHash { set { _isAlertHash = value; } }
    public int IsAttackingHash { set { _isAttackingHash = value; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // set the parameter hash references
        _isAlertHash = Animator.StringToHash("isAlert");
        _isAttackingHash = Animator.StringToHash("isAttacking");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isAlert == true)
        {
            Vector3 lookAt = _player.position;
            lookAt.y = transform.position.y;
            transform.LookAt(lookAt);
        }
    }

    // Called when the player enters the alert collider
    public void OnAlertTriggerEnter(Collider other)
    {
        _animator.SetBool(_isAlertHash, true);
        _isAlert = true;
    }

    // Called when the player exits the alert collider
    public void OnAlertTriggerExit(Collider other)
    {
        _animator.SetBool(_isAlertHash, false);
        _isAlert = false;
    }


    // Called when the player enters the attack collider
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInAttackRange = true;

            _animator.SetBool(_isAlertHash, false);
            _animator.SetBool(_isAttackingHash, true);

            StartCoroutine(HandleAttack(other));
        }
    }

    IEnumerator HandleAttack(Collider other)
    {
        yield return new WaitForSeconds(1.1f);

        Vector3 hitDirection = other.transform.position - transform.position;
        hitDirection = hitDirection.normalized;

        PlayerManager playerManager = other.GetComponent<PlayerManager>();

        if (_isInAttackRange == true)
        {
            playerManager.TakeDamage(_damage, hitDirection);

            yield return new WaitForSeconds(3);

            ResetAlertState();
        }
        else
        {
            yield break;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        _animator.SetBool(_isAlertHash, true);
        _animator.SetBool(_isAttackingHash, false);

        _isInAttackRange = false;
    }

    public void ResetAlertState()
    {
        if (_animator != null)
        {
            _animator.SetBool(_isAlertHash, false);
            _animator.SetBool(_isAttackingHash, false);
            _isAlert = false;
        }
    }

}

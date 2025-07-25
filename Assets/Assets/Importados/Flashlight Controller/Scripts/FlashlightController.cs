using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] AudioClip _onSFX;
    [SerializeField] AudioClip _offSFX;
    [SerializeField] KeyCode _toggleKey;

    private GameObject _cameraObject;
    private GameObject _lightSource;
    private AudioSource _audioSource;
    private Vector3 _offset;

    public bool IsOn { get; private set; }
    private readonly float _speed = 5f;

    public bool IsEnabled = true;

    private bool isPlayer = true;

    private void Awake()
    {
        _cameraObject = Camera.main.gameObject;
        _lightSource = transform.GetChild(0).gameObject;
        _audioSource = GetComponent<AudioSource>();
        
    }

    private void Start()
    {
        _lightSource.gameObject.SetActive(false);
        _offset = transform.position - _cameraObject.transform.position;
    }

    private void Update()
    {
        isPlayer = IsParentPlayer();

        gameObject.SetActive(isPlayer);

        if (!isPlayer) return;

        IsEnabled = true;

        transform.position = _cameraObject.transform.position + _offset;
        transform.rotation = Quaternion.Slerp(transform.rotation, _cameraObject.transform.rotation, _speed * Time.deltaTime);

        if (!IsEnabled)
        {
            _lightSource.gameObject.SetActive(false);
            IsOn = false;
            return;
        }

        if (Input.GetKeyDown(_toggleKey))
        {
            _audioSource.PlayOneShot(_onSFX);
        }

        if (Input.GetKeyUp(_toggleKey))
        {
            _audioSource.PlayOneShot(_offSFX);
            _lightSource.SetActive(!IsOn);
            IsOn = !IsOn;
        }
    }


    public void PlayFlashlightOffSFX()
    {
        _audioSource.PlayOneShot(_onSFX);
        _audioSource.PlayDelayed(2f);
        _audioSource.PlayOneShot(_offSFX);
    }

    private bool IsParentPlayer()
    {
        Transform current = transform;

        while (current.parent != null)
        {
            current = current.parent;

            if (current.CompareTag("Player"))
                return true;
        }

        return false;
    }
    public void SetIsPlayer(bool value)
    {
        IsEnabled = value;

        if (!value)
        {
            _lightSource.SetActive(false);
            IsOn = false;
        }
    }



}

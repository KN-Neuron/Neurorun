using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public PopupMenu _popupMenu;
    public LayerMask interactableLayer;
    public SpriteRenderer _aura;
    public float _auraScaleSpeed;

    [Header("CHARACTER_STATS")][SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    public int attention = 0;
    [SerializeField] private int _attentionTreshold = 40;
    [SerializeField] private bool isUsingBrainWaves = true;
    public InputAction _playerControls;

    [Header("Jumping")][SerializeField] private float _jumpTime = 0.5f;
    [SerializeField] private int _extraJumps = 2;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _jumpMultiplier = 2f;
    [SerializeField] CapsuleCollider2D groundCheckCollider;
    [SerializeField] private float _jumpSlowdownRate;
    [SerializeField] private float _coyoteTime;
    [SerializeField] private float _bufferedJumpTime;

    [Header("Movement")][SerializeField] private float _accelerationRate;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _decelerationRate;
    private float _currentSpeed;


    private bool _canCoyoteJump = true;
    private float _coyoteTimeCounter;
    private bool _isBufferedJumping;
    private float _bufferedJumpTimeCounter;
    public int _jumpCount = 0;
    private float _jumpTimeCounter;
    private bool _isJumping = false;
    private bool _jumpButtonIsHeld = false;
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool _isGrounded;
    Vector2 vecGravity ;


    private Rigidbody2D _rigidbody;
    private float _horizontalInput;
    private bool isFacingRight = true;
    public bool _canInteract = false;
    public bool _isInteracting = false;
    MindWave.TGCConnectionController tcgController;
    public string packet;
    
    int meditation = 0;
    int poorSignalLevel = 200;
    public Slider _sliderTreshold;
    public Slider _sliderAttention;
    public ConnectionManager _connectionState;
    public ParticleSystem _runningParticles;
    public ParticleSystem _landingParticles;

    public bool isOnMoving = false;

    Animator _animator;





    public void setPacket(string outPacket)
    {
        packet = outPacket;
        string pattern = "\"attention\":(\\d+)";
        string pattern2 = "\"meditation\":(\\d+)";
        string pattern3 = "\"poorSignalLevel\":(\\d+)";
        Regex regex = new Regex(pattern);
        Regex regex2 = new Regex(pattern2);
        Regex regex3 = new Regex(pattern3);
        MatchCollection matches1 = regex.Matches(packet);
        MatchCollection matches2 = regex2.Matches(packet);
        MatchCollection matches3 = regex3.Matches(packet);

            foreach (Match match in matches1)
            {
                attention = int.Parse(match.Groups[1].Value);
             
            }
        foreach (Match match in matches2)
        {
            meditation = int.Parse(match.Groups[1].Value);
           
        }

        foreach (Match match in matches3)
        {
            poorSignalLevel = int.Parse(match.Groups[1].Value);
            
        }
    
       /* Debug.Log("PoorSignalLevel: " + poorSignalLevel + "\n"+  " Attention: " + attention + " Meditation: " + meditation);*/
    }

    #region Input Controls

/*    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }*/

    #endregion


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        tcgController = GetComponent<MindWave.TGCConnectionController>();
        _attentionTreshold = PlayerPrefs.GetInt("deviceSensitivity", 40);
        UpdateAttentionTreshold();
        _connectionState = FindObjectOfType<ConnectionManager>();
        _animator = GetComponent<Animator>();
/*        Debug.Log("Attention Treshold: " + _attentionTreshold);*/
        
    }




    #region Updates


    private void doChecks()
    {
        if (isUsingBrainWaves)
        {
            checkAttention();
        }

        isGrounded();
        
        if (_isGrounded)
        {
            _jumpCount = 0;
            _coyoteTimeCounter = 0;
            if (_isBufferedJumping &&_bufferedJumpTimeCounter < _bufferedJumpTime)
            {
                startJump();
                _isBufferedJumping = false;
            }
            if (_jumpButtonIsHeld)
            {
                startJump();
            }
        }

        
        _aura.color = new Color(_aura.color.r, _aura.color.g, _aura.color.b, Mathf.Lerp(_aura.color.a, _currentAttention/100, _auraScaleSpeed * Time.deltaTime));
      
       
        Flip();
    }

    private bool isGrounded()
    {
        _isGrounded = Physics2D.OverlapCapsule(groundCheck.position, groundCheckCollider.size, CapsuleDirection2D.Horizontal, 0 , groundLayer);

        return _isGrounded;
      
    }
    
 

    void Update()
    {
        /*Debug.Log(_coyoteTimeCounter);*/


        _connectionState.SetConnectionState(poorSignalLevel, tcgController.IsConnected());

        if (_isBufferedJumping)
        {
            _bufferedJumpTimeCounter += Time.deltaTime;
         
        }
        doChecks();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down * 1f, 0.1f,interactableLayer);
        


        /*if (hit.collider != null && _horizontalInput == 0 && !_isJumping)
      
        {
            Debug.Log(hit.collider.name);
            transform.parent = hit.transform;
        }
        else
        {
            transform.parent = null;
        }*/
        

    

        UpdateAttention(attention);
        _sliderAttention.value = Mathf.Lerp(_sliderAttention.value, _currentAttention, 0.1f);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector2.down * 1f);
    }

    void FixedUpdate()
    {
        if (_rigidbody.velocity.y > 0 && _isJumping)
        {

            _jumpTimeCounter += Time.deltaTime;



            if (_jumpTimeCounter > _jumpTime)
            {
                _isJumping = false;
            }

            float t = _jumpTimeCounter / _jumpTime;
            float currentJumpM = _jumpMultiplier;
            
            if(t >0.5f)
            {
                currentJumpM = _jumpMultiplier * (1-t);
            }

            _rigidbody.velocity += vecGravity * _jumpMultiplier * Time.deltaTime;
        }

        if (_rigidbody.velocity.y < 0)
        {
             _coyoteTimeCounter += Time.deltaTime;
            _rigidbody.velocity -= vecGravity * _fallMultiplier * Time.deltaTime;
            
           
        }
        else
        {
            _animator.SetBool("Falling", false);
        }

        if (_rigidbody.velocity.y < -0.5f)
        {
            _animator.SetBool("Falling", true);
        }
        else
        {
            _animator.SetBool("Falling", false);
        }


        if (_horizontalInput != 0)
        {
            _animator.SetBool("Run", true);
            _currentSpeed += _accelerationRate * Time.deltaTime*_horizontalInput;
         
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, _decelerationRate * Time.deltaTime);
            _animator.SetBool("Run", false);
        }
       

        _currentSpeed = Mathf.Clamp(_currentSpeed, -_maxSpeed, _maxSpeed);
        _rigidbody.velocity = new Vector2(_currentSpeed, _rigidbody.velocity.y);
    }

    private void checkAttention()
    {
        if (attention >= _attentionTreshold)
        {
            _isInteracting = true;
            _isButtonHeld = true;
        }
        else
        {
            _isButtonHeld = false;
            _isInteracting = false;
        }
    }



    #endregion



    #region Movement


    public void Move(InputAction.CallbackContext context)
    {
        
        _horizontalInput = context.ReadValue<Vector2>().x;
        /*Debug.Log(_horizontalInput);*/
    }

    private void Flip()
    {
        if (isFacingRight && _horizontalInput < 0f)
        {

            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingRight = false;
        }
        else if (!isFacingRight && _horizontalInput > 0f)
        {

            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingRight = true;
        }
    }


    #endregion



    #region Jumping

/*Implement coyote time, jump buffering, switch to velocity based jumping*/

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && (_isGrounded || _jumpCount < _extraJumps || _coyoteTimeCounter< _coyoteTime))
        {
           
            startJump();
            _jumpCount++;
            _coyoteTimeCounter = _coyoteTime;
            _jumpButtonIsHeld = true;
        }
        else if (context.started && _rigidbody.velocity.y < 0)
        {
            _isBufferedJumping = true;
            _bufferedJumpTimeCounter = 0;
        }




        if (context.canceled)
        {
            _animator.SetBool("Jump", false);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * _jumpSlowdownRate);
            _isJumping = false;
            _jumpButtonIsHeld = false;
        }
    }

    private void startJump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
        _isJumping = true;
        _animator.SetBool("Jump",true);
    
        _jumpTimeCounter = 0;
 

    }

    #endregion

    #region Interact

    private bool _isButtonHeld = false;

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && _canInteract)
        {
            _isInteracting = true;
            _isButtonHeld = true;


        }

 

        if (context.canceled)
        {
                _isButtonHeld = false;
                _isInteracting = false;
        }
    }

    public void UpdateAttentionTreshold()
    {
        _attentionTreshold = PlayerPrefs.GetInt("deviceSensitivity", 40);
        _sliderTreshold.value = _attentionTreshold;
    }

    private float _currentAttention;

    public void UpdateAttention(float attention)
    {
        
        _currentAttention = attention;
    }

    private void Start()
    {
        tcgController.UpdateAttentionEvent += OnUpdateAttention;
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
    }

    private void OnUpdateAttention(int value)
    {
        attention = value;
        UpdateAttention(value);
        /*Debug.Log("Attention:" + attention);*/
    }


    #endregion

    public void PauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Pause");
            if (Time.timeScale == 1)
            {
                _popupMenu.GetComponent<PopupMenu>().showMenu();
                Time.timeScale = 0;
                
            }
            else
            {
                UpdateAttentionTreshold();
                Time.timeScale = 1;
                _popupMenu.hideMenu();
            }
        }
    }

    public void Connect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Reconnect");
            tcgController.StartConnection();
        }
    }




}

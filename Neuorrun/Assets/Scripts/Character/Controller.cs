using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [Header("CHARACTER_STATS")][SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    public InputAction _playerControls;


    private Rigidbody2D _rigidbody;
    private float _horizontalInput;
    private bool isFacingRight = true;
    public bool _canInteract = false;
    public bool _isInteracting = false;

    





    #region Input Controls

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    #endregion


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }


    #region Updates

       /* Implement ground checks*/ 
    private void doChecks()
    {
        Flip();
    }

    void Update()
    {
        doChecks();
    }

    void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(_horizontalInput * _movementSpeed, _rigidbody.velocity.y);
    }


    #endregion



    #region Movement

/*Implement slowing down*/
    public void Move(InputAction.CallbackContext context)
    {
        _horizontalInput = context.ReadValue<Vector2>().x;
    }

    private void Flip()
    {
        if (isFacingRight && _rigidbody.velocity.x < 0)
        {
            transform.Rotate(0f, 180f, 0f);
            isFacingRight = false;
        }
        else if (!isFacingRight && _rigidbody.velocity.x > 0)
        {
            transform.Rotate(0f, 180f, 0f);
            isFacingRight = true;
        }
    }


    #endregion



    #region Jumping

/*Implement coyote time, jump buffering*/

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
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

    #endregion




}

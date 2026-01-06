using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

namespace C042 {

    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour {

        private CharacterController _characterController;
        [SerializeField] private InputActionReference _inputMovementRef;
        private InputAction _inputMovement;

        [SerializeField] private InputActionReference _inputDisparRef;
        [SerializeField] private InputActionReference _inputSegonDisparRef;
        [SerializeField] private LayerMask _disparMask;

        [SerializeField] private float _speed = 3f;
        [SerializeField] private Transform _camara;

        private InputAction _inputLook;
        [SerializeField] private InputActionReference _inputLookRef;

        private InputAction _inputJump;
        [SerializeField] private InputActionReference _inputJumpRef;

        [SerializeField] private MissatgeGlobalVector3 _eventMoure;

        private float _mouseSensibility = 0.1f;
        private float _constantSensibility = 1f;
        private float mirarVertical = 0f;
        private float mirarHorizontal = 0f;

        [SerializeField] private float _gravity = -9.8f;
        [SerializeField] private float _jumpHeight = 1f;
        [SerializeField] private float _fallMultiplier = 2f;

        private Vector3 _velocity;


        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _inputMovement = _inputMovementRef.action;
            _inputLook = _inputLookRef.action;
            _inputJump = _inputJumpRef.action;

            _inputDisparRef.action.performed += OnDisparar;
            _inputSegonDisparRef.action.performed += OnSegonDispar;
            _inputJump.performed += OnSaltar;

        }

        private void OnEnable()
        {
            _inputMovement.Enable();
            _inputLook.Enable();
            _inputDisparRef.action.Enable();
            _inputJump.Enable();
        }

        private void OnDisable()
        {
            _inputMovement.Disable();
            _inputLook.Disable();
            _inputDisparRef.action.Disable();
            _inputJump.Disable();
        }

        private void OnSegonDispar (InputAction.CallbackContext context)
        {
            if (Physics.Raycast(_camara.position, _camara.forward, out RaycastHit hit, 20f, _disparMask))
            {
                Debug.Log("Aqui pasará algo.");
            }
        }

        private void OnDisparar (InputAction.CallbackContext context)
        {
            if(Physics.Raycast(_camara.position, _camara.forward, out RaycastHit hit, 20f, _disparMask))
            {
                Debug.DrawLine(_camara.position, hit.point, Color.red, 4f);
                _eventMoure.Raise(hit.point);
            } 
        }

        private void Update()
        {
            Vector2 movimentInput = _inputMovement.ReadValue<Vector2>();
            Vector3 desplacament = transform.right * movimentInput.x + transform.forward * movimentInput.y;
            _characterController.Move(desplacament * _speed * Time.deltaTime);

            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }else
            {
                _velocity.y += _gravity * _fallMultiplier * Time.deltaTime;
            }

            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);

            LookMouse();
        }

        private void LookMouse()
        {
            Vector2 delta = _inputLook.ReadValue<Vector2>();

            mirarVertical -= delta.y * _mouseSensibility * _constantSensibility;
            mirarVertical = Mathf.Clamp(mirarVertical, -75, 75);

            mirarHorizontal += delta.x * _mouseSensibility * _constantSensibility;

            _camara.localEulerAngles = new Vector3(mirarVertical, 0f, 0f);
            transform.localEulerAngles = new Vector3(0f, mirarHorizontal, 0f);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void OnSaltar(InputAction.CallbackContext ctx)
        {
            if (_characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
        }
    }
}

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

        
        

        [SerializeField] private float _speed = 3f;
        [SerializeField] private Camera _camara;
        [SerializeField] private Camera[] _camaras;
        private int _indexArrayCamaras = 0;
        [SerializeField] private InputActionReference _cam1Ref;
        [SerializeField] private InputActionReference _cam2Ref;
        [SerializeField] private InputActionReference _cam3Ref;
        private InputAction _cam1;
        private InputAction _cam2;
        private InputAction _cam3;

        private InputAction _inputLook;
        [SerializeField] private InputActionReference _inputLookRef;

        private InputAction _inputJump;
        [SerializeField] private InputActionReference _inputJumpRef;
        [SerializeField] private InputActionReference _inputDisparRef;
        [SerializeField] private LayerMask _disparMask;
        [SerializeField] private InputActionReference _inputSegonDisparRef;
        [SerializeField] private LayerMask _pikminMask;
        [SerializeField] private Material[] _materialsInPikmin;

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

            _cam1 = _cam1Ref.action;
            _cam2 = _cam2Ref.action;
            _cam3 = _cam3Ref.action;

            _cam1.performed += OnCam1Canvi;
            _cam2.performed += OnCam2Canvi;
            _cam3.performed += OnCam3Canvi;

            for (int i = 0; i < _camaras.Length; i++)
            { 
                _camaras[i].enabled = (i == _indexArrayCamaras);
            }
            _camara = _camaras[_indexArrayCamaras];
        }

        private void OnEnable()
        {
            _inputMovement.Enable();
            _inputLook.Enable();
            _inputDisparRef.action.Enable();
            _inputSegonDisparRef.action.Enable();
            _inputJump.Enable();
            _cam1.Enable();
            _cam2.Enable();
            _cam3.Enable();
        }

        private void OnDisable()
        {
            _inputMovement.Disable();
            _inputLook.Disable();
            _inputDisparRef.action.Disable();
            _inputSegonDisparRef.action.Disable();
            _inputJump.Disable();
            _cam1.Disable();
            _cam2.Disable();
            _cam3.Disable();
        }

        private void OnCam1Canvi(InputAction.CallbackContext context)
        {
            SetCamara(0);
        }

        private void OnCam2Canvi(InputAction.CallbackContext context)
        {
            SetCamara(1);
        }

        private void OnCam3Canvi(InputAction.CallbackContext context)
        {
            SetCamara(2);
        }

        private void SetCamara(int index)
        {
            if (index < 0 || index >= _camaras.Length) return;

            _indexArrayCamaras = index;
            for (int i = 0; i < _camaras.Length; i++)
            {
                _camaras[i].enabled = (i == _indexArrayCamaras);
                if (i == 2)
                {
                    Vector3 euler = _camaras[i].transform.eulerAngles;
                    euler.y = -129.306f;
                    _camaras[i].transform.eulerAngles = euler;
                }
            }
            _camara = _camaras[_indexArrayCamaras];
        }

        private void OnSegonDispar(InputAction.CallbackContext context)
        {
            Debug.Log("RIGHT CLICK ACTION PERFORMED");
            if (Physics.Raycast(_camara.transform.position, _camara.transform.forward, out RaycastHit hit, 20f, _pikminMask))
            {
                Debug.Log("l'hi he donat al " + hit.collider.name);
                GameObject pikminGO = hit.collider.gameObject;
                MeshRenderer renderer = pikminGO.GetComponentInChildren<MeshRenderer>();

                if (renderer != null)
                {
                    if (renderer.sharedMaterial.name.Contains("Neon_03"))
                    {
                        Debug.Log("HA ENTRADO EN EL IF DEL NOMBRE DEL RENDERER");
                        renderer.material = _materialsInPikmin[0];
                    }
                    else
                    {
                        renderer.material = _materialsInPikmin[1];
                    }
                } else
                {
                    Debug.Log("EL RENDERER ES NULL");
                }
            }
        }

        private void OnDisparar (InputAction.CallbackContext context)
        {
            if(Physics.Raycast(_camara.transform.position, _camara.transform.forward, out RaycastHit hit, 20f, _disparMask))
            {
                Debug.DrawLine(_camara.transform.position, hit.point, Color.red, 4f);
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

            _camara.transform.localEulerAngles = new Vector3(mirarVertical, 0f, 0f);
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

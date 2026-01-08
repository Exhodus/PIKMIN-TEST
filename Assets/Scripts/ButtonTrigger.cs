using System;           // Por Action<>
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _porta;
    private MeshRenderer _meshRenderer;
    private Material _material;
    public event Action<GameObject> OnObjectEnter;
    private Coroutine _bajarBoton;
    [SerializeField] private float _lowerDistance = 0.5f;
    [SerializeField] private float _speed = 1.5f;
    private bool _isPressed = false;
    private bool _isDown = false;

    public void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.sharedMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        MeshRenderer meshRendererGO = other.GetComponentInChildren<MeshRenderer>();

        if (meshRendererGO.sharedMaterial == _material)
        {
            Debug.Log("ENTRA EN EL IF DEL TRIGGER BUTTON");
            OnObjectEnter?.Invoke(other.gameObject);
            if (!_isPressed && !_isDown)
            {
                _isPressed = true;
                _bajarBoton = StartCoroutine(BajarBoton());
            }
        } else if (meshRendererGO == null) 
        {
                Debug.Log("NO ENTRA EN EL IF DEL TRIGGER BUTTON");
        } else 
        {
                Debug.Log("NO ES NULL PERO NO ENTRA AL IF");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        _isPressed = false;
    }

    private IEnumerator BajarBoton()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.down * _lowerDistance;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                _speed * Time.deltaTime
            );

            yield return null; //Si poso segons es torna boig. Segons internet aixó fa que s'esperi al següent frame
        }

        _isDown = true;
    }
}

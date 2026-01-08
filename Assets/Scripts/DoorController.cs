using System.Collections;
using UnityEngine;


public class DoorController : MonoBehaviour
{

    [SerializeField] private ButtonTrigger _buttonTrigger;
    private Coroutine _bajarPuerta;
    [SerializeField] private float _lowerDistance = 0.5f;
    [SerializeField] private float _speed = 1.5f;
    private bool _isLowered = false;

    private void OnEnable()
    {
        if( _buttonTrigger != null)
        {
            _buttonTrigger.OnObjectEnter += OpenDoor;
        }
    }

    private void OnDisable()
    {
        _buttonTrigger.OnObjectEnter -= OpenDoor;
    }

    private void OpenDoor(GameObject gameObject)
    {
        if (!_isLowered)
        {
            _bajarPuerta = StartCoroutine(Bajar());
        }
    }

    private IEnumerator Bajar()
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

        _isLowered = true;
    }
}

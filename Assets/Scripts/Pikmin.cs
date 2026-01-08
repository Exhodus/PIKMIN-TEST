using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Pikmin : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Vector3 _desti;
    private enum AgentEstats { NULL, Quiet, Pointed, Roam, Waiting}
    private AgentEstats _estatActual;
    [SerializeField] private int _roamMask;

    [SerializeField] private MissatgeGlobalVector3 _eventMoure;
    [SerializeField] private MissatgeGlobal _eventArribar;

    private int _zonaID = 0;

    private Coroutine _waitCoroutine;

    //https://docs.unity3d.com/ScriptReference/Transform-eulerAngles.html
    private float _rotationSpeed = 20f;
    private Vector3 _currentEulerAngles;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _roamMask = _agent.areaMask;
    }

    private void Start()
    {
        ChangeState(AgentEstats.Roam);
    }


    private void OnEnable()
    {
        _eventMoure.OnEvent += OnMoure;
        _eventArribar.OnEvent += OnRoam;
    }

    private void OnDisable()
    {
        _eventMoure.OnEvent -= OnMoure;
        _eventArribar.OnEvent -= OnRoam;
    }

    private void Update()
    {
        UpdateStateMachine();
    }

    private void UpdateStateMachine()
    {
        switch (_estatActual)
        {
            case AgentEstats.Pointed:
                Vector3 distancia = _desti - transform.position;
                distancia.y = 0;
                if (distancia.sqrMagnitude <= 0.2f)
                {

                    _eventArribar.Raise();
                    ChangeState(AgentEstats.Waiting);
                }
                break;
            case AgentEstats.Roam:
                distancia = _desti - transform.position;
                distancia.y = 0;
                
                if (distancia.sqrMagnitude <= 0.2f)
                {
                    ChangeState(AgentEstats.Roam);
                }
                break;
        }
    }

    private IEnumerator WaitTwoSecs(float waitTime)
    {
        Debug.Log("Tiempo de espera "+ waitTime);
        yield return new WaitForSeconds(waitTime);
        Debug.Log("CAMBIA A ROAM");
        ChangeState(AgentEstats.Roam);
    }

    private void ChangeState(AgentEstats estat)
    {
        ExitState();
        InitState(estat);
    }

    private void InitState(AgentEstats estat)
    {
       _estatActual = estat;
        switch (_estatActual)
        {
            case AgentEstats.Roam:

                //Aixó ho he tret de la documentació oficial de Unity
                //buscant per internet he trobat que la millor manera per fer el SamplePosition
                //era afegir un area al boltant del Pikmin per tal de que no caigués el punt sobre 
                //ell mateix, perque a vegades donaba alguns bugs i es quedaba quiet.
                /*
                 * Exemple de la documentació: (https://docs.unity3d.com/ScriptReference/Random-insideUnitSphere.html)
                 * public class Example : MonoBehaviour
                        {
                            void Start()
                            {
                                // Sets the position to be somewhere inside a sphere
                                // with radius 5 and the center at zero.

                                transform.position = Random.insideUnitSphere * 5;
                            }
                        }
                 */
                Vector3 puntRandom = UnityEngine.Random.insideUnitSphere * 2f;
                puntRandom.y = 0;
                Vector3 target = transform.position + puntRandom;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(target, out hit, 3f, _roamMask))
                {
                    
                    _desti = hit.position;
                    _desti.y = 0;
                    _agent.isStopped = false;
                    _agent.SetDestination(_desti);
                } 
                break;

            case AgentEstats.Pointed:
                _agent.isStopped = false;
                _agent.SetDestination(_desti);
                break;

            case AgentEstats.Quiet:
                _agent.isStopped = true;
                break;

            case AgentEstats.Waiting:
                _agent.isStopped = true;
                _waitCoroutine = StartCoroutine(WaitTwoSecs(2.0f));
                break;
        }
    }

    private void ExitState()
    {
        switch (_estatActual)
        {
            
            case AgentEstats.Pointed:
                _agent.isStopped = false;
                break;
            case AgentEstats.Quiet:
                _agent.isStopped = false;
                break;
            case AgentEstats.Roam:
                _agent.isStopped = true;
                break;
            case AgentEstats.Waiting:
                if (_waitCoroutine != null)
                {
                    StopCoroutine(_waitCoroutine);
                    _waitCoroutine = null;
                }
                Debug.Log("Corrutina Parada.");
                _agent.isStopped = false;
                break;

        }
    }

    private void OnMoure(Vector3 vector)
    {
        _desti = vector;
        ChangeState(AgentEstats.Pointed);
    }

    private void OnRoam()
    {
        
        ChangeState(AgentEstats.Roam);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HE ENTRADO EN TRIGGER");
    }

    public void SetZona(int zonaID)
    {
        /*
        _zonaID = zonaID;
        int areaIndex = zonaID;
        int mask = 1 << areaIndex;
        _agent.areaMask = mask;
        */
    }
}
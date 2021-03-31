using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Cowboy : MonoBehaviour
{
    public Animator anim;
    float forwardAmount;

    public NavMeshAgent agent;
    private NavMeshHit navHit;

    public Team Team => _team;
    [SerializeField] private Team _team;
    [SerializeField] private LayerMask _layerMask;

    private float _attackRange = 3f;
    //private float _rayDistance = 5.0f;
    //private float _stoppingDistance = 1.5f;
    private float wanderRange = 20f;
    private float checkRate;
    private float nextCheck;
    private Transform cowTransform;

    //private Vector3 _destination;
    //private Quaternion _desiredRotation;
    //private Vector3 _direction;
    private Vector3 wanderTarget;
    private NormalBoy _target;
    //private Cowboy enemyMaster; 
    private CowboyState _currentState;

    private bool isShooting;



    void Start()
    {
        checkRate = Random.Range(0.3f,0.4f);
        cowTransform = this.gameObject.transform;
    }

    private void Update()
    {
        if(Time.time < Time.deltaTime + checkRate)
        {
            nextCheck = Time.time + checkRate;
        }

        switch (_currentState)
        {
            case CowboyState.Wander:
                {
                    /*if (NeedsDestination())
                    {
                        GetDestination();
                    }*/

                    //transform.rotation = _desiredRotation;
                    var targetToAggro = CheckForAggro();
                    if (targetToAggro == null)
                    {
                        if (NeedsDestination(cowTransform.position, wanderRange, out wanderTarget))
                        {
                            agent.SetDestination(wanderTarget);
                            Debug.Log(wanderTarget);
                            anim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
                            forwardAmount = Vector3.forward.z;
                        }
                    }

                    //var rayColor = IsPathBlocked() ? Color.red : Color.green;
                    //Debug.DrawRay(transform.position, _direction * _rayDistance, rayColor);

                    /*while (IsPathBlocked())
                    {
                        //Debug.Log("Path Blocked");
                        GetDestination();
                    }*/

                    if (targetToAggro != null)
                    {
                        _target = targetToAggro.GetComponent<NormalBoy>();
                        _currentState = CowboyState.Chase;
                    }

                    break;
                }
            case CowboyState.Chase:
                {
                    if (_target == null)
                    {
                        _currentState = CowboyState.Wander;
                        return;
                    }

                    transform.LookAt(_target.transform);
                    isShooting = false;
                    agent.SetDestination(_target.transform.position);

                    if (Vector3.Distance(transform.position, _target.transform.position) < _attackRange)
                    {
                        _currentState = CowboyState.Attack;
                    }
                    break;
                }
            case CowboyState.Attack:
                {
                    if (_target != null)
                    {
                        isShooting = true;
                        anim.SetBool("isShooting", true);
                        Destroy(gameObject);
                        Debug.Log("attacked");
                    }

                    _currentState = CowboyState.Wander;
                    anim.SetBool("isShooting", false);
                    break;
                }
            case CowboyState.Dead:
                {
                    break;
                }
        }
    }

    //private bool IsPathBlocked()
    //{
        //Ray ray = new Ray(transform.position, _direction);
        //var hitSomething = Physics.RaycastAll(ray, _rayDistance, _layerMask);
        //return hitSomething.Any();
    //}

    //private void GetDestination()
    //{
        //Vector3 testPosition = (transform.position + (transform.forward * 2f)) +
                               //new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0f,
                                   //UnityEngine.Random.Range(-4.5f, 4.5f));

        //_destination = new Vector3(testPosition.x, 1f, testPosition.z);

        //_direction = Vector3.Normalize(_destination - transform.position);
        //_direction = new Vector3(_direction.x, 0f, _direction.z);
        //_desiredRotation = Quaternion.LookRotation(_direction);
    //}

    bool NeedsDestination(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * wanderRange;
        if(NavMesh.SamplePosition(randomPoint, out navHit,20f, NavMesh.AllAreas))
        {
            result = navHit.position;
            return true;
        }
        else
        {
            result = center;
            return false;
        }
        //if (_destination == Vector3.zero)
            //return true;

        //var distance = Vector3.Distance(transform.position, _destination);
        //if (distance <= _stoppingDistance)
        //{
            //return true;
        //}

        //return false;
    }



    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    private Transform CheckForAggro()
    {
        float aggroRadius = 5f;

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (var i = 0; i < 24; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, aggroRadius))
            {
                var drone = hit.collider.GetComponent<NormalBoy>();
                if (drone != null && drone.tag != "Cowboy")//drone.Team != gameObject.GetComponent<Cowboy>().Team)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    //Debug.Log("hit Normal");
                    return drone.transform;
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * aggroRadius, Color.white);
            }
            direction = stepAngle * direction;
        }

        return null;
    }
}

public enum Team
{
    Cowboy,
    NormalBoy
}

public enum CowboyState
{
    Wander,
    Chase,
    Attack,
    Dead
}


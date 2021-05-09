using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Cowboy : MonoBehaviour

{
    public Animator anim;
    public CheckAggroTest aggroScript;
    //float forwardAmount;

    public NavMeshAgent agent;
    //private NavMeshHit navHit;

    //public float _attackRange = 3f;
    //public float _foundRange = 15f;
    //private float _rayDistance = 5.0f;
    //private float _stoppingDistance = 1.5f;
    //public float wanderRange = 20f;
    //private float checkRate;
    //private float nextCheck;
    //private Transform cowTransform;

    //private Vector3 _destination;
    //private Quaternion _desiredRotation;
    //private Vector3 _direction;
    private NormalBoy _target; 
    private CowboyState _currentState;

    //private bool isShooting;



    private void Update()
    {
        switch (_currentState)
        {
            case CowboyState.Wait:
                {
                    var targetToAggro = gameObject.GetComponent<CheckAggroTest>().CheckForAggro();
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
                        _currentState = CowboyState.Wait;
                        return;
                    }

                    agent.SetDestination(_target.transform.position);


                    var targetToAggro = gameObject.GetComponent<CheckAggroTest>().CheckForAggro();
                    if (targetToAggro != null)
                    {
                        agent.SetDestination(_target.transform.position);
                        _target = targetToAggro.GetComponent<NormalBoy>();
                        _currentState = CowboyState.Attack;
                    }
                    break;
                }
            case CowboyState.Attack:
                {
                    RaycastHit hit;
                    Physics.Raycast(transform.position,transform.forward,out hit,20f);
                    NormalBoy target = hit.transform.GetComponent<NormalBoy>();
                    if(target != null)
                    {
                        Destroy(target);
                    }
                    Debug.Log("attacking");
                    //isShooting = true;
                    anim.SetBool("isShooting",true);
                    if(_target == null)
                    {
                        _currentState = CowboyState.Wait;
                    }
                    break;
                }
        }
    }

    /*private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, _direction);
        var hitSomething = Physics.RaycastAll(ray, _rayDistance, _layerMask);
        return hitSomething.Any();
    }

    private void GetDestination()
    {
        Vector3 testPosition = (transform.position + (transform.forward * 2f)) +
                               new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0f,
                                   UnityEngine.Random.Range(-4.5f, 4.5f));

        _destination = new Vector3(testPosition.x, 1f, testPosition.z);

        _direction = Vector3.Normalize(_destination - transform.position);
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        _desiredRotation = Quaternion.LookRotation(_direction);
    }

    private bool NeedsDestination()
    {
        if (_destination == Vector3.zero)
            return true;

        var distance = Vector3.Distance(transform.position, _destination);
        if (distance <= _stoppingDistance)
        {
            return true;
        }

        return false;
    }



    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    private Transform CheckForAggro()
    {
        float aggroRadius = 10f;

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (var i = 0; i < 24; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, aggroRadius))
            {
                var drone = hit.collider.GetComponent<NormalBoy>();
                if (drone != null && drone.tag != "Cowboy")
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
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
    }*/
}

public enum CowboyState
{
    Wander,
    Wait,
    Chase,
    Attack,
    Dead
}


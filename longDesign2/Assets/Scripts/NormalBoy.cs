using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class NormalBoy : MonoBehaviour
{
    public Animator anim;
    float forwardAmount;

    public NavMeshAgent agent;

    //public Team Team => _team;
    //[SerializeField] private Team _team;
    [SerializeField] private LayerMask _layerMask;

    private float _hideRange = 15f;
    private float _rayDistance = 10.0f;
    private float _stoppingDistance = 1.5f;

    private Vector3 _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;
    private Cowboy _target;
    private NormalState _currentState;


    private void Update()
    {
        switch (_currentState)
        {
            case NormalState.Wander:
                {
                    if (NeedsDestination())
                    {
                        GetDestination();
                    }

                    transform.rotation = _desiredRotation;

                    transform.Translate(Vector3.forward * Time.deltaTime * 2f);
                    anim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
                    forwardAmount = Vector3.forward.z;

                    var rayColor = IsPathBlocked() ? Color.red : Color.green;
                    Debug.DrawRay(transform.position, _direction * _rayDistance, rayColor);

                    while (IsPathBlocked())
                    {
                        GetDestination();
                    }

                    var targetToAggro = CheckForAggro();
                    if (targetToAggro != null)
                    {
                        _target = targetToAggro.GetComponent<Cowboy>();
                        _currentState = NormalState.Escape;
                    }

                    break;
                }
            case NormalState.Escape:
                {
                    if (_target == null)
                    {
                        _currentState = NormalState.Wander;
                        return;
                    }

                    Vector3 dirtoPlayer = transform.position - _target.transform.position;
                    Vector3 newPos = transform.position + dirtoPlayer;
                    transform.Translate(newPos);


                    if (Vector3.Distance(transform.position, _target.transform.position) > _hideRange)
                    {
                        _currentState = NormalState.Hide;
                    }
                    break;
                }
            case NormalState.Hide:
                {
                    Debug.Log("Hiding");

                    _currentState = NormalState.Wander;
                    break;
                }
        }
    }

    private bool IsPathBlocked()
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
                var drone = hit.collider.GetComponent<Cowboy>();
                if (drone != null && drone.tag != "NormalBoy")
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
    }
}

public enum NormalState
{
    Wander,
    Escape,
    Hide
}




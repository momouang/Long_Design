using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckAggroTest : MonoBehaviour
{
    //public Team Team => _team;
    [SerializeField] private Team _team;

    [SerializeField] private LayerMask _layerMask;

    public float _rayDistance = 10.0f;
    public float _stoppingDistance = 1.5f;

    public Vector3 _destination = Vector3.zero;
    public Quaternion _desiredRotation = Quaternion.identity;
    public Vector3 _direction;// = Vector3.zero;


    public bool IsPathBlocked()
    {
        
        Ray ray = new Ray(transform.position, _direction);
        var hitSomething = Physics.RaycastAll(ray, _rayDistance, _layerMask);
        //var hitSomething = Physics.RaycastAll(transform.position, transform.forward, _rayDistance,_layerMask);
        
        return hitSomething.Any();
    }

    public void GetDestination()
    {
        Vector3 testPosition = (transform.position + (transform.forward * 2f)) +
                               new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0f,
                                   UnityEngine.Random.Range(-4.5f, 4.5f));

        _destination = new Vector3(testPosition.x, 1f, testPosition.z);

        _direction = Vector3.Normalize(_destination - transform.position);
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        _desiredRotation = Quaternion.LookRotation(_direction);

    }

    public bool NeedsDestination()
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

    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector2.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector2.up);
    public Transform CheckForAggro()
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
                var drone = hit.collider.GetComponent<CheckAggroTest>();
                if (drone != null && drone._team != gameObject.GetComponent<CheckAggroTest>()._team)
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

    public enum Team
    {
        Arthur,
        NormalBoy,
        Cowboy
    }
}

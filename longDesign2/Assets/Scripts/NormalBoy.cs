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

    private float _hideRange = 5f;
   

    public CheckAggroTest aggroScript;
    //private Vector3 _destination;
    //private Quaternion _desiredRotation;
    //private Vector3 _direction;
    private Cowboy _target;
    private NormalState _currentState;
 


    private void Update()
    {
        switch (_currentState)
        {
            case NormalState.Wander:
                {
                    Debug.Log("N_Wander");
                    if (aggroScript.NeedsDestination())
                    {
                        aggroScript.GetDestination();
                    }

                    transform.rotation = aggroScript._desiredRotation;

                    transform.Translate(Vector3.forward * Time.deltaTime);
                    anim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
                    forwardAmount = Vector3.forward.z;                  

                    while (aggroScript.IsPathBlocked())
                    {                      
                        aggroScript.GetDestination();
                    }

                    var targetToAggro = gameObject.GetComponent<CheckAggroTest>().CheckForAggro();
                    if (targetToAggro != null)
                    {
                        _target = targetToAggro.GetComponent<Cowboy>();
                        _currentState = NormalState.Escape;
                    }

                    break;
                }
            case NormalState.Escape:
                {
                    Debug.Log("escaping");
                    if (_target == null)
                    {
                        _currentState = NormalState.Wander;
                        return;
                    }


                    float distance = Vector3.Distance(transform.position, _target.transform.position);

                    
                        Debug.Log("N_distance");
                        Vector3 dirtoPlayer = transform.position - _target.transform.position;
                        Vector3 newPos = (transform.position + dirtoPlayer);
                        if (agent.SetDestination(newPos) == false)
                        {
                            Debug.Log("N_SetDest Error: " + dirtoPlayer.ToString() + ":" + newPos.ToString());
                        }
                    
                    if (distance >= _hideRange)
                    { 
                        _currentState = NormalState.Hide;
                        //agent.Stop();
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


    //Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    //Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    /*private Transform CheckForAggro()
    {
        float aggroRadius = 10f;

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;                            #Gaming_Portfolio_of
        var direction = angle * Vector3.forward;                                   #Momo_Uang
        var pos = transform.position;                                              #2021
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
    }*/
}

public enum NormalState
{
    Wander,
    Escape,
    Hide
}




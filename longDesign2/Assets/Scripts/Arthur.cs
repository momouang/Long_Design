using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

static class gGlobal
{
    public static bool fShoot = false;
}

public class Arthur : MonoBehaviour
{

    private ArthurState _currentState;
    private Cowboy _target;

    public CheckAggroTest aggroScript;
    public GameObject arthurBody;

    public NavMeshAgent agent;
    public Animator anim;

    public float speed = 10f;
    public float time = 2.0f;
    public float timeDuration = 2.0f;

    public bool isShooting;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;

    // Update is called once per frame

    void Update()
    {
        //arthurBody.transform.position = gameObject.transform.position;
        switch (_currentState)
        { 
            case ArthurState.Wander:
                {
                    Debug.Log("arthurPOS : " + transform.position);
                    Debug.Log("A_Wander");
                    anim.Play("Arthur_walk");
                    if (aggroScript.NeedsDestination())
                    {
                        Debug.Log("needsDestination");
                        aggroScript.GetDestination();
                    }

                    Debug.Log("des:"+aggroScript._destination);
                    transform.rotation = aggroScript._desiredRotation;
                    transform.Translate(Vector3.forward * Time.deltaTime);

                    gGlobal.fShoot = false;
                    while (aggroScript.IsPathBlocked())
                    {
                        Debug.Log("pathBlocked");
                        aggroScript.GetDestination();
                    }

                    var targetToAggro = GetComponent<CheckAggroTest>().CheckForAggro();
                    if (targetToAggro != null)
                    {
                        _target = targetToAggro.GetComponent<Cowboy>();
                        _currentState = ArthurState.Chase;
                    }
                    break;
                }

            case ArthurState.Chase:
                {
                    Debug.Log("A_Chase");
                    if(_target == null)
                    {
                        _currentState = ArthurState.Wander;
                        return;
                    }
                    var targetToAggro = GetComponent<CheckAggroTest>().CheckForAggro();
                    if(targetToAggro != null)
                    {
                        Vector3 newArthurPos = new Vector3(_target.transform.position.x - 5f, _target.transform.position.y, _target.transform.position.z);
                        agent.SetDestination(newArthurPos);
                        //anim.Play("Arthur_takeGun");
                        _target = targetToAggro.GetComponent<Cowboy>();
                        _currentState = ArthurState.Attack;
                        gGlobal.fShoot = false;

                        Debug.Log("arthur : " + newArthurPos);
                        Debug.Log("target : " + _target.transform.position);
                    }

                    break;
                }

            case ArthurState.Attack:
                {
                    if (time >= 0)
                    {
                        time -= Time.deltaTime;
                        //Debug.Log(time);
                        return;
                    }

                    if (time <= 0 && _target != null)
                    {
                        anim.Play("Arthur_shoot");
                        Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
                        bulletPrefab.transform.LookAt(_target.transform.position);
                        time = timeDuration;
                    }

                    if(gGlobal.fShoot == true)
                    {
                        _currentState = ArthurState.Wander;
                    }


                    break;
                }

        }

    }


    public enum ArthurState
    {
        Wander,
        Chase,
        Attack
    }

}

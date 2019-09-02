using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject target;
    Vector3 offset;

    bool tragetIsDead;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<Submarine>().gameObject;
        offset = target.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = target.transform.position- offset;
        pos.y = transform.position.y;
        if (tragetIsDead)
        {
            pos.z = transform.position.z;
        }
        transform.position = pos;
    }

    public void SubmarineIsDead()
    {
        tragetIsDead = true;
    }
}

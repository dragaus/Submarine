using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineShow : MonoBehaviour
{
    public GameObject pointsManager;
    GameObject[] positionsToGo;

    int index = 0;

    float rotSpeed = 1.5f;
    float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        positionsToGo = new GameObject[pointsManager.transform.childCount];
        for (int i = 0; i < pointsManager.transform.childCount; i++)
        {
            positionsToGo[i] = pointsManager.transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = positionsToGo[index].transform.position - this.transform.position;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                Quaternion.LookRotation(direction),
                                                Time.deltaTime * rotSpeed);

        this.transform.Translate(0, 0, speed * Time.deltaTime);

        if (Vector3.Distance(positionsToGo[index].transform.position, transform.position) < 0.2f)
        {
            index++;
            if (index >= positionsToGo.Length)
            {
                index = 0;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    Rigidbody rigidBody;
    [SerializeField] float speedOfRotation = 20f;
    [SerializeField] float speedOfPropulsion = 20f;

    enum State { Alive, Dead, Trascending};
    State submarineState = State.Alive;

    bool hasMove;

    GameManager manager;

    bool gyroEnabled;
    Gyroscope gyroscope;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        gyroEnabled = EnableldGyro();

    }

    // Update is called once per frame
    void Update()
    {
        if (submarineState == State.Alive)
        {
            Propel();
            Rotate();
        }
    }

    bool EnableldGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyroscope = Input.gyro;
            gyroscope.enabled = true;
            return true;
        }

        return false;
    }

    //This is used to rotate the submarine according to its z axis
    void Rotate()
    {
        rigidBody.freezeRotation = true;

        var rotationForce = speedOfRotation * Time.deltaTime;

        if (gyroEnabled)
        {
            transform.Rotate(Vector3.forward * gyroscope.rotationRateUnbiased.z);
        }

        if (Input.GetKey(KeyCode.A))
        {
            FirstMove();
            transform.Rotate(Vector3.forward * rotationForce);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            FirstMove();
            transform.Rotate(-Vector3.forward * rotationForce);
        }


        rigidBody.freezeRotation = false;
    }

    //This is used to move the submarine foward in its oposition
    private void Propel()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            FirstMove();
            rigidBody.AddRelativeForce(Vector3.right * speedOfPropulsion);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            audioSource.Pause();
        }
    }

    //Its used to tell the game manager that the player just start the movement of the submarine
    void FirstMove()
    {
        if (!hasMove)
        {
            hasMove = true;
            manager.StartMovement();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (submarineState == State.Alive)
        {
            switch (collision.gameObject.tag)
            {
                case "Friendly":
                    break;
                default:
                    //This is used for the first touch with something that can kill the submarine
                    audioSource.clip = Resources.Load<AudioClip>("Audio/SFX/Submarine/Hit");
                    audioSource.loop = false;
                    audioSource.Play();
                    submarineState = State.Dead;
                    Camera.main.GetComponent<CameraFollow>().SubmarineIsDead();
                    rigidBody.useGravity = true;
                    manager.EndGame();
                    break;
            }
        }
        else if(submarineState == State.Dead)
        {
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider target)
    {
        if (submarineState == State.Alive)
        {
            switch (target.tag)
            {
                case "Finish":
                    //This is used when the player finish the level
                    manager.WinLevel(target.gameObject);
                    submarineState = State.Trascending;
                    audioSource.Stop();
                    break;
                case "Star":
                    //By convention there will be always 3 stars named Star + _ + Number of the star 
                    //thats how we know there will be a number to parse 
                    int starNumber = int.Parse(target.name.Split('_')[1]);
                    manager.SetStar(starNumber);
                    Destroy(target.gameObject);
                    break;
            }
        }
    }
}

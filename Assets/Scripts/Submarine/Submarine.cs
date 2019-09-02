using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Submarine : MonoBehaviour
{
    Rigidbody rigidBody;
    [SerializeField] float speedOfRotation = 20f;
    [SerializeField] float speedOfPropulsion = 20f;

    enum State { Alive, Dead, Trascending};
    State submaerineState = State.Alive;

    bool hasMove;

    GameManager manager;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (submaerineState == State.Alive)
        {
            Propel();
            Rotate();
        }
    }

    void Rotate()
    {
        rigidBody.freezeRotation = true;
        var rotationForce = speedOfRotation * Time.deltaTime;

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

    private void Propel()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            FirstMove();
            rigidBody.AddRelativeForce(Vector3.right * speedOfPropulsion);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            audioSource.Pause();
        }
    }

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
        if (submaerineState == State.Alive)
        {
            switch (collision.gameObject.tag)
            {
                case "Friendly":
                    break;
                case "Finish":
                    manager.NextScene();
                    break;
                default:
                    audioSource.clip = Resources.Load<AudioClip>("Audio/SFX/Submarine/Hit");
                    audioSource.loop = false;
                    audioSource.Play();
                    submaerineState = State.Dead;
                    Camera.main.GetComponent<CameraFollow>().SubmarineIsDead();
                    rigidBody.useGravity = true;
                    manager.EndGame();
                    break;
            }
        }
        else
        {
            audioSource.Play();
        }
    }
}

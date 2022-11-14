using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovPlayer : MonoBehaviour
{
    [Header("Vars for movement")]
    [Range(0f, 1f)]
    public float ComplexionBodySpeed = 1; //1 a 0.5;
    public float SpeedMovement = 10;
    public Transform SpritePlayer;
    [Space]


    public bool isPlayer1 = false; //Solo Para testeo

    Rigidbody rb;
    float InputY;
    float InputX;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        SpritePlayer.LookAt(new Vector3(0, Camera.main.transform.position.x, Camera.main.transform.position.z));
    }

    private void FixedUpdate()
    {
        if (isPlayer1)
        {
            InputY = Input.GetAxis("Vertical");
            InputX = Input.GetAxis("Horizontal");
        }
        else
        {           
            InputY = Input.GetAxis("Vertical2");
            InputX = Input.GetAxis("Horizontal2");

        }

        Vector3 movedirection = new Vector3(InputX, 0.0f, InputY);
        movedirection = transform.TransformDirection(movedirection);

        rb.MovePosition(transform.position + movedirection * ((SpeedMovement * Time.deltaTime) * ComplexionBodySpeed));
    }
}

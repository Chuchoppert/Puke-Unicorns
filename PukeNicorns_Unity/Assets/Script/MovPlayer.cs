using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovPlayer : MonoBehaviour
{
    [Header("Vars for movement")]
    [Range(0.2f, 1f)] public float ComplexionBodySpeed = 1; //1 a 0.2; velocidad de peso
    [SerializeField] float SpeedMovement = 10; //Velocidad Base
    private float InputY;
    private float InputX;
    [SerializeField] float RotationY;
    [SerializeField] GameObject Canvas_UI;
    [SerializeField] bool CanMove = true;


    [Space, Header("Dash vars")]
    [SerializeField] KeyCode Dash_Action;


    [Space, Header("Vars for invert movement")]
    [Range(-1, 1), SerializeField] float InvertMovementX = 1;
    [Range(-1, 1), SerializeField] float InvertMovementY = 1;

    [Space, Header("Extra Vars")]
    [SerializeField] CharacterController ControllerPlayer;
    [SerializeField] Rigidbody rb;

    [Space, SerializeField] bool isPlayer1 = false;



    void Update()
    {        
        ///Revisar que player es para recibir inputs
        if (isPlayer1)
        {
            InputY = Input.GetAxis("Vertical") * InvertMovementY;
            InputX = Input.GetAxis("Horizontal") * InvertMovementX;
        }
        else
        {
            InputY = Input.GetAxis("Vertical2") * InvertMovementY;
            InputX = Input.GetAxis("Horizontal2") * InvertMovementX;
        }
        
        //Proceso para conocer la rotacion que se debe tener
        CheckDirection();

        //Girar player 
        this.Canvas_UI.transform.rotation = Quaternion.Euler(0.0f, RotationY, 0.0f);
    }
    void FixedUpdate()
    {
        //Siempre que el jugador no este envenenado, mover
        if(CanMove)
        {
            Vector3 movedirection = new Vector3(InputX, 0.0f, InputY);//.normalized;
                                                                      //movedirection = transform.TransformDirection(movedirection);

            //rb.MovePosition(transform.position + movedirection * ((SpeedMovement * Time.deltaTime) * ComplexionBodySpeed));
            ControllerPlayer.Move(movedirection * ((SpeedMovement * Time.deltaTime) * ComplexionBodySpeed));
        }      
    }

    //Metodo para checar direction
    void CheckDirection()
    {
        //x: izquierda|derecha
        //y: arriba|abajo

        //Agregar los sprites o giros del sprite
        if (InputX == 0 && InputY == 0) //Dejar para evitar un bug de siempre mirar a 45° 
        {
            //RotationY = 0.0f;
        }
        else if (InputX == 0 && Mathf.Sign(InputY) != 0) //Casos para movimiento solo vertical
        {
            if (InputX == 0 && Mathf.Sign(InputY) == 1) //arriba
            {
                RotationY = 180.0f;
                GetComponent<GrabLaunchCake>().GiveDirToLaunch(new Vector3(0.0f, 0.0f, 1.0f));
            } 
            else if (InputX == 0 && Mathf.Sign(InputY) == -1) //abajo
            {
                RotationY = 0.0f;
                GetComponent<GrabLaunchCake>().GiveDirToLaunch(new Vector3(0.0f, 0.0f, -1.0f));
            }         
        }
        else if (Mathf.Sign(InputX) != 0 && InputY == 0)//Casos para movimiento solo horizontal
        {
            if (Mathf.Sign(InputX) == 1 && InputY == 0) //izquierda
            {
                RotationY = 0.0f;
                GetComponent<GrabLaunchCake>().GiveDirToLaunch(new Vector3(1.0f, 0.0f, 0.0f));
            }
            else if (Mathf.Sign(InputX) == -1 && InputY == 0) //derecha
            {
                RotationY = 180.0f;
                GetComponent<GrabLaunchCake>().GiveDirToLaunch(new Vector3(-1.0f, 0.0f, 0.0f));
            }
        }
        else                                            //Casos para diagonales
        {
            if (Mathf.Sign(InputX) == 1 && Mathf.Sign(InputY) == 1) //arriba-derecha
            {
                RotationY = -45.0f;
                GetComponent<GrabLaunchCake>().GiveDirToLaunch(new Vector3(1.0f, 0.0f, 1.0f));
            }
            else if (Mathf.Sign(InputX) == -1 && Mathf.Sign(InputY) == 1) //arriba-izquierda
            {
                RotationY = -135.0f;
                GetComponent<GrabLaunchCake>().GiveDirToLaunch(new Vector3(-1.0f, 0.0f, 1.0f));
            }

            else if (Mathf.Sign(InputX) == 1 && Mathf.Sign(InputY) == -1) //abajo-derecha
            {
                RotationY = 45.0f;
                GetComponent<GrabLaunchCake>().GiveDirToLaunch(new Vector3(1.0f, 0.0f, -1.0f));
            }
            else if (Mathf.Sign(InputX) == -1 && Mathf.Sign(InputY) == -1) //abajo-izquierda
            {
                RotationY = 135.0f;
                GetComponent<GrabLaunchCake>().GiveDirToLaunch(new Vector3(-1.0f, 0.0f, -1.0f));
            }
        }
            
    }

    public void PlayerCanMove(bool MovePlayer)
    {
        CanMove = MovePlayer;
    }
}

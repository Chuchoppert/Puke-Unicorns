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

    [Space, Header("Vars for effects from cake")]
    [Range(0, 5)] public int CakesEatsWithoutDigest = 0; //cantidad de pasteles sin terminar de digerir
    [SerializeField] int RangeMaxFromFlatToFat = 5; //Cantidad maxima de pasteles para llegar a la gordura maxima
    [SerializeField] float TimeToDigestCake = 3.0f; //tiempo en tardar en "digerir" el pastel
    public Slider Slider_Eat;
    private float timerPassed_Digest = 0;
    [Space]
    [SerializeField] float TimeWhilePoison = 3.0f; //Tiempo inhabilitado cuando come pastel malo
    [SerializeField] RawImage Player_RawImage;
    private float timerPassed_Poison = 0;

    [Space, Header("Grab and Launchs vars")]
    [SerializeField] float ForceToLauch = 5;
    public KeyCode GrabAndLaunch_Action;
    public bool havePlayerCake = false;
    public GameObject cakeTaked_GO;
    public GameObject UI_Direction;
    [SerializeField] Vector3 DirToLaunch; 
    [SerializeField] float RotationY;
    private bool ReadyToLaunch = false;

    [Space, Header("Dash vars")]
    [SerializeField] KeyCode Dash_Action;

    [Space, Header("Vars for points")]
    [SerializeField] int PointsPlayer = 0;
    [SerializeField] TextMeshProUGUI Points_TMP;
    [SerializeField] RawImage DashIcon_RawImage;

    [Space, Header("Vars for invert movement")]
    [Range(-1, 1), SerializeField] float InvertMovementX = 1;
    [Range(-1, 1), SerializeField] float InvertMovementY = 1;

    [Space, Header("Extra Vars")]
    [SerializeField] Transform SpritePlayer;
    [SerializeField] CharacterController ControllerPlayer;
    [SerializeField] Rigidbody rb;

    [Space, SerializeField] bool isPlayer1 = false;



    void Update()
    {        
        //////////////////////////////////
        SpritePlayer.LookAt(new Vector3(0, -Camera.main.transform.position.x, Camera.main.transform.position.z));

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
        /////////////////////////////////////
        
        //Proceso para diferir pasteles 
        if (CakesEatsWithoutDigest > 0)
        {
            //empezar timer
            timerPassed_Digest += Time.deltaTime;

            //Si timer es igual o mayor que el tiempo para digerir eliminar un valor
            if(timerPassed_Digest >= TimeToDigestCake)
            {
                Debug.Log(this.gameObject.name + ": " + CakesEatsWithoutDigest + " to digest");

                //Resetear timer y restar pasteles sin digerir
                timerPassed_Digest = 0;
                CakesEatsWithoutDigest -= 1;

                //Actualizar peso del player
                EffectWeightMovement();
            }
        }
        //Proceso player envenenado
        else if(CakesEatsWithoutDigest == -1)
        {
            //empezar timer
            timerPassed_Poison += Time.deltaTime;

            //Si timer es igual o mayor que el tiempo de envenamiento quitar envenenamiento y hacer "flaco"
            if (timerPassed_Poison >= TimeWhilePoison)
            {
                Debug.Log(this.gameObject.name + ": is now right!");

                //resetear timer y pasteles sin digerir
                timerPassed_Poison = 0;
                CakesEatsWithoutDigest = 0;

                //Test, cambiar por sprite normal
                if (isPlayer1)
                {
                    Player_RawImage.color = Color.white;
                }
                else
                {
                    Player_RawImage.color = Color.yellow;
                }

                //Actulizar peso del player
                EffectWeightMovement();
            }
        }
        /////////////////////////////////////
        //Proceso para conocer la rotacion que se debe tener
        CheckDirection();

        //Girar player 
        this.gameObject.transform.rotation = Quaternion.Euler(0.0f, RotationY, 0.0f);

        //Solo si el player tiene un pastel, preparar el lanzamiento y lanzar
        if (havePlayerCake)
        {
            //Una vez que el player tenga el pastel, debera dejar de tocar la tecla para preparar el lanzamiento
            if (Input.GetKeyUp(GrabAndLaunch_Action))
            {
                ReadyToLaunch = true;
            }

            //solo si el player vuelve a tomar la tecla y esta listo para larzar
            if (Input.GetKey(GrabAndLaunch_Action) && ReadyToLaunch)
            {
                //Desparentar el objeto
                cakeTaked_GO.transform.parent = null;

                //Activar fisicas de unity, empujar hacia la direccion reciente
                cakeTaked_GO.GetComponent<Rigidbody>().isKinematic = false;
                cakeTaked_GO.GetComponent<Rigidbody>().AddForce(DirToLaunch * ForceToLauch, ForceMode.Impulse);

                //Regresar el collider a la normalidad y activar colisiones
                cakeTaked_GO.GetComponent<BoxCollider>().size = Vector3.one;
                cakeTaked_GO.GetComponent<BoxCollider>().enabled = true;

                //Ocultar direccion de lanzamiento
                UI_Direction.SetActive(false);

                //Resetear valores para el proximo pastel
                cakeTaked_GO = null;
                havePlayerCake = false;
                ReadyToLaunch = false;
            }
        }
        /////////////////////////////////////
    }
    void FixedUpdate()
    {
        //Siempre que el jugador no este envenenado, mover
        if(CakesEatsWithoutDigest != -1)
        {
            Vector3 movedirection = new Vector3(InputX, 0.0f, InputY);//.normalized;
                                                                      //movedirection = transform.TransformDirection(movedirection);

            //rb.MovePosition(transform.position + movedirection * ((SpeedMovement * Time.deltaTime) * ComplexionBodySpeed));
            ControllerPlayer.Move(movedirection * ((SpeedMovement * Time.deltaTime) * ComplexionBodySpeed));
        }      
    }

    //Metodo para afectar al player desde el script pastel
    public void EffectCake(bool CakeWasGood, int AddPoints = 0, int SubstracPoints = 0)
    {
        //Si el pastel era bueno, actualizar pesos y puntuacion
        if (CakeWasGood == false)
        {
            //Resetear timer y sumar a los pasteles sin digerir
            timerPassed_Digest = 0.0f;
            CakesEatsWithoutDigest += 1;

            Debug.Log(this.gameObject.name + ": Eat good cake");

            //Actualizar peso del player
            EffectWeightMovement();

            //Actualizar puntos
            RefreshPointsAndUi(AddPoints);
        }
        else
        {
            Debug.Log(this.gameObject.name + ": Eat bad cake");

            //Test, cambiar por sprite envenenado
            Player_RawImage.color = Color.green;

            // -1 por que vomita pasteles sin digerir y para empezar el proceso de envenamiento
            CakesEatsWithoutDigest = -1;

            //Actualizar puntos
            RefreshPointsAndUi(SubstracPoints);
        }
    }

    //Metodo para actualizar pesos (velocidad)
    void EffectWeightMovement()
    {
        //Si no hay pasteles por digerir regresar la velocidad normal
        if(CakesEatsWithoutDigest == 0)
        {
            Debug.Log(this.gameObject.name + ": All cakes digested");

            ComplexionBodySpeed = 1.0f;
        }

        ///Si los pasteles por digerir son igual o menor que el rango max para hacer pesado al player, cambiar velocidad
        else if (CakesEatsWithoutDigest <= RangeMaxFromFlatToFat)
        {          
            //el rango de velocidad / el numero max para hacer pesado al player = proporcion de velocidad por cada unidad de pastel comida
            float ValueForSpeed = (1.0f - 0.2f) / RangeMaxFromFlatToFat;

            //Velocidad = rango minimo + (la proporcion de velocidad * (porciones de velocidad))
            ComplexionBodySpeed = 0.2f + (ValueForSpeed * (RangeMaxFromFlatToFat - CakesEatsWithoutDigest));
            //Ejemplo con rango maximo de 5, valueForSpeed = (0.8f)/5 = 0.16f
            //case 0 (5): 0.2 + (x * (5 - 0)); = 1.0f segun el rango maximo sera aprox 1.0f mejor se deja en un if
            //case 1 (4): 0.2 + (x * (5 - 1)); = 0.84f
            //case 2 (3): 0.2 + (x * (5 - 2)); = 0.68f
            //case 3 (2): 0.2 + (x * (5 - 3)); = 0.52f
            //case 4 (1): 0.2 + (x * (5 - 4)); = 0.36f
            //case 5 (1): 0.2 + (x * (5 - 5)); = 0.2f

            //segun el numero de sprites, agregar aqui el cambio de sprites de flaco a gordo y si hay sonido de gordura caminando agregar

            //Limitar pasteles a "digerir", dejando que solo tenga que reducir este limite de pasteles.
            if (CakesEatsWithoutDigest == RangeMaxFromFlatToFat) CakesEatsWithoutDigest = RangeMaxFromFlatToFat;
        }
    }

    //Metodo para actualizar el ui y el puntaje
    void RefreshPointsAndUi(int ValueEffect)
    {
        //suma o resta puntaje
        PointsPlayer += ValueEffect;

        //limitar el minimo de puntos a 0
        if(PointsPlayer < 0)
        {
            PointsPlayer = 0;
        }

        //actualizar texto de puntos.
        Points_TMP.text = $"Puntos: {PointsPlayer}";
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
            if (InputX == 0 && Mathf.Sign(InputY) == 1) //abajo
            {
                RotationY = 0.0f;
                DirToLaunch = new Vector3(0.0f, 0.0f, 1.0f);
            } 
            else if (InputX == 0 && Mathf.Sign(InputY) == -1) //arriba
            {
                RotationY = 180.0f;
                DirToLaunch = new Vector3(0.0f, 0.0f, -1.0f);
            }         
        }
        else if (Mathf.Sign(InputX) != 0 && InputY == 0)//Casos para movimiento solo horizontal
        {
            if (Mathf.Sign(InputX) == 1 && InputY == 0) //derecha
            {
                RotationY = 90.0f;
                DirToLaunch = new Vector3(1.0f, 0.0f, 0.0f);
            }
            else if (Mathf.Sign(InputX) == -1 && InputY == 0) //izquierda
            {
                RotationY = -90.0f;
                DirToLaunch = new Vector3(-1.0f, 0.0f, 0.0f);
            }
        }
        else                                            //Casos para diagonales
        {
            if (Mathf.Sign(InputX) == 1 && Mathf.Sign(InputY) == 1) //arriba-derecha
            {
                RotationY = 45.0f;
                DirToLaunch = new Vector3(1.0f, 0.0f, 1.0f);
            }
            else if (Mathf.Sign(InputX) == -1 && Mathf.Sign(InputY) == 1) //arriba-izquierda
            {
                RotationY = -45.0f;
                DirToLaunch = new Vector3(-1.0f, 0.0f, 1.0f);
            }

            else if (Mathf.Sign(InputX) == 1 && Mathf.Sign(InputY) == -1) //abajo-derecha
            {
                RotationY = 135.0f;
                DirToLaunch = new Vector3(1.0f, 0.0f, -1.0f);
            }
            else if (Mathf.Sign(InputX) == -1 && Mathf.Sign(InputY) == -1) //abajo-izquierda
            {
                RotationY = -135.0f;
                DirToLaunch = new Vector3(-1.0f, 0.0f, -1.0f);
            }
        }
            
    }
}

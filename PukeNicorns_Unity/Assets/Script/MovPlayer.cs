using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovPlayer : MonoBehaviour
{
    [Header("Vars for movement")]
    [Range(0.2f, 1f)] public float ComplexionBodySpeed = 1; //1 a 0.2; velocidad de peso
    [SerializeField] float SpeedMovement = 10; //Velocidad Base

    [Space, Header("Vars for effects from cake")]
    public float TimeWhileEatCake = 3.0f; //tiempo en tardar en comer pastel
    [SerializeField] float TimeToDigestCake = 3.0f; //tiempo para digerir pastel
    [SerializeField] float TimeWhilePoison = 3.0f; //Tiempo inhabilitado cuando come pastel malo
    [SerializeField, Range(0, 5)] int CakesEatsWithoutDigest = 0; //cantidad de pasteles sin terminar de digerir
    [SerializeField] int RangeMaxFromFlatToFat = 5; //Cantidad maxima de pasteles para llegar a la gordura maxima

    [Space, Header("Vars for invert movement")]
    [Range(-1, 1), SerializeField] float InvertMovementX = 1;
    [Range(-1, 1), SerializeField] float InvertMovementY = 1;

    [Space, Header("Limits movement")]
    [SerializeField] Vector2 HorizontalLimit;
    [SerializeField] Vector2 VerticalLimit;

    [Space, Header("Extra Vars")]
    [SerializeField] Transform SpritePlayer;
    [SerializeField] CharacterController ControllerPlayer;
    [SerializeField] Rigidbody rb;

    [Space, SerializeField] bool isPlayer1 = false;
    [SerializeField] float timerPassedForCakeEffect = 0;
    float InputY;
    float InputX;



    void Update()
    {
        SpritePlayer.LookAt(new Vector3(0, -Camera.main.transform.position.x, Camera.main.transform.position.z));

        if (isPlayer1)
        {
            InputY = Input.GetAxis("Vertical") * InvertMovementX;
            InputX = Input.GetAxis("Horizontal") * InvertMovementY;
        }
        else
        {
            InputY = Input.GetAxis("Vertical2") * InvertMovementX;
            InputX = Input.GetAxis("Horizontal2") * InvertMovementY;
        }

        if(CakesEatsWithoutDigest > 0)
        {
            timerPassedForCakeEffect += Time.deltaTime;
            if(timerPassedForCakeEffect >= TimeToDigestCake)
            {
                Debug.Log(this.gameObject.name + ": " + CakesEatsWithoutDigest + " to digest");
                timerPassedForCakeEffect = 0;
                CakesEatsWithoutDigest -= 1;

                EffectWeightMovement();
            }
        }
    }
    void FixedUpdate()
    {
        Vector3 movedirection = new Vector3(InputX, 0.0f, InputY);//.normalized;
        //movedirection = transform.TransformDirection(movedirection);

        //rb.MovePosition(transform.position + movedirection * ((SpeedMovement * Time.deltaTime) * ComplexionBodySpeed));
        ControllerPlayer.Move(movedirection * ((SpeedMovement * Time.deltaTime) * ComplexionBodySpeed));
    }

    public void EffectCake(bool CakeWasGood)
    {
        if (CakeWasGood == false) //Pastel bueno
        {
            timerPassedForCakeEffect = 0.0f;
            CakesEatsWithoutDigest += 1;
            Debug.Log(this.gameObject.name + ": Eat good cake");
            EffectWeightMovement();
        }
        else
        {
            Debug.Log(this.gameObject.name + ": Eat bad cake");
            ////Vomitar pasteles, restar puntos, restar pasteles sin digerir, regresar variable de velocidad del peso
        }
    }

    void EffectWeightMovement()
    {
        if(CakesEatsWithoutDigest == 0)
        {
            Debug.Log(this.gameObject.name + ": All cakes digested");
            ComplexionBodySpeed = 1.0f;
        }
        else if (CakesEatsWithoutDigest <= RangeMaxFromFlatToFat)
        {          
            float ValueForSpeed = (1.0f - 0.2f) / RangeMaxFromFlatToFat;
            ComplexionBodySpeed = 0.2f + (ValueForSpeed * (RangeMaxFromFlatToFat - CakesEatsWithoutDigest));

            //Ejemplo con rango maximo de 5, valueForSpeed = (0.8f)/5 = 0.16f
            //case 0 (5): 0.2 + (x * (5 - 0)); = 1.0f segun el rango maximo sera aprox 1.0f mejor se deja en un if
            //case 1 (4): 0.2 + (x * (5 - 1)); = 0.84f
            //case 2 (3): 0.2 + (x * (5 - 2)); = 0.68f
            //case 3 (2): 0.2 + (x * (5 - 3)); = 0.52f
            //case 4 (1): 0.2 + (x * (5 - 4)); = 0.36f
            //case 5 (1): 0.2 + (x * (5 - 5)); = 0.2f
        }
    }
}

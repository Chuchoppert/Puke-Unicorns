using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CakeEffectsPlayer : MonoBehaviour
{
    [Space, Header("Effects from good Cake")]
    [Range(0, 5)] public int CakesEatsWithoutDigest = 0; //cantidad de pasteles sin terminar de digerir
    [SerializeField] int RangeMaxFromFlatToFat = 5; //Cantidad maxima de pasteles para llegar a la gordura maxima
    [SerializeField] float TimeToDigestCake = 3.0f; //tiempo en tardar en "digerir" el pastel
    public Slider Slider_Eat;
    private float timerPassed_Digest = 0;

    [Space, Header("Poison Effect")]
    [SerializeField] float TimeWhilePoison = 3.0f; //Tiempo inhabilitado cuando come pastel malo
    private float timerPassed_Poison = 0;

    [Space, Header("Vars for points")]
    [SerializeField] int PointsPlayer = 0;
    [SerializeField] TextMeshProUGUI Points_TMP;

    [Space, Header("Extra Vars")]
    [SerializeField] TextureUnicornStatus unicornStatusTexture;
    [SerializeField] MovPlayer movPlayer;

    private void Update()
    {
        ////Proceso para diferir pasteles 
        if (CakesEatsWithoutDigest > 0)
        {
            DigestCakeEffect();
        }
        else if(CakesEatsWithoutDigest == -1)
        {
            PoisonEffect();
        }
    }

    //Metodo para diferir pasteles 
    void DigestCakeEffect()
    {
        //empezar timer
        timerPassed_Digest += Time.deltaTime;

        //Si timer es igual o mayor que el tiempo para digerir eliminar un valor
        if (timerPassed_Digest >= TimeToDigestCake)
        {
            Debug.Log(this.gameObject.name + ": " + CakesEatsWithoutDigest + " to digest");

            if (CakesEatsWithoutDigest <= RangeMaxFromFlatToFat / 2)
            {
                unicornStatusTexture.CheckTexturesUnicorn(Complex.Normal, StatusUnicorn.Idle);
            }

            //Resetear timer y restar pasteles sin digerir
            timerPassed_Digest = 0;
            CakesEatsWithoutDigest -= 1;

            //Actualizar peso del player
            EffectWeightMovement();
        }
    }

    void PoisonEffect()
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

            ///textura flaco-normal        
            unicornStatusTexture.ResetColorPlayer();
            unicornStatusTexture.CheckTexturesUnicorn(Complex.Slim, StatusUnicorn.Idle);

            movPlayer.PlayerCanMove(true);

            //Actulizar peso del player
            EffectWeightMovement();
        }
    }

    //Metodo para actualizar pesos (velocidad)
    void EffectWeightMovement()
    {
        //Si no hay pasteles por digerir regresar la velocidad normal
        if (CakesEatsWithoutDigest == 0)
        {
            Debug.Log(this.gameObject.name + ": All cakes digested");

            movPlayer.ComplexionBodySpeed = 1.0f;
        }

        ///Si los pasteles por digerir son igual o menor que el rango max para hacer pesado al player, cambiar velocidad
        else if (CakesEatsWithoutDigest <= RangeMaxFromFlatToFat)
        {
            //el rango de velocidad / el numero max para hacer pesado al player = proporcion de velocidad por cada unidad de pastel comida
            float ValueForSpeed = (1.0f - 0.2f) / RangeMaxFromFlatToFat;

            //Velocidad = rango minimo + (la proporcion de velocidad * (porciones de velocidad))
            movPlayer.ComplexionBodySpeed = 0.2f + (ValueForSpeed * (RangeMaxFromFlatToFat - CakesEatsWithoutDigest));
            //Ejemplo con rango maximo de 5, valueForSpeed = (0.8f)/5 = 0.16f
            //case 0 (5): 0.2 + (x * (5 - 0)); = 1.0f segun el rango maximo sera aprox 1.0f mejor se deja en un if
            //case 1 (4): 0.2 + (x * (5 - 1)); = 0.84f
            //case 2 (3): 0.2 + (x * (5 - 2)); = 0.68f
            //case 3 (2): 0.2 + (x * (5 - 3)); = 0.52f
            //case 4 (1): 0.2 + (x * (5 - 4)); = 0.36f
            //case 5 (1): 0.2 + (x * (5 - 5)); = 0.2f

            //segun el numero de sprites, agregar aqui el cambio de sprites de flaco a gordo y si hay sonido de gordura caminando agregar
        }
    }

    public void EffectCake(bool CakeWasBad, int AddPoints = 0, int SubstracPoints = 0)
    {
        //Si el pastel era bueno, actualizar pesos y puntuacion
        if (CakeWasBad == false)
        {
            //Resetear timer y sumar a los pasteles sin digerir
            timerPassed_Digest = 0.0f;

            //Limitar pasteles a "digerir", dejando que solo tenga que reducir este limite de pasteles.
            if (CakesEatsWithoutDigest < RangeMaxFromFlatToFat)
            {
                CakesEatsWithoutDigest += 1;
            }

            Debug.Log(this.gameObject.name + ": Eat good cake");

            //Si esta la textura flaco - cambiar a textura normal
            if(unicornStatusTexture.DimensionComplex == 0)
            {
                unicornStatusTexture.CheckTexturesUnicorn(Complex.Normal, StatusUnicorn.Idle);
            }

            //Si CakesEatsWithoutDigest esta a la mitad del rango - cambiar a textura gorda
            if(CakesEatsWithoutDigest > RangeMaxFromFlatToFat / 2)
            {
                unicornStatusTexture.CheckTexturesUnicorn(Complex.Fat, StatusUnicorn.Idle);
            }

            //Actualizar peso del player
            EffectWeightMovement();

            //Actualizar puntos
            RefreshPointsAndUi(AddPoints);
        }
        //Afectar solo si el player no esta envenenado
        else if (CakeWasBad && CakesEatsWithoutDigest != -1)
        {
            Debug.Log(this.gameObject.name + ": Eat bad cake");

            // -1 por que vomita pasteles sin digerir y para empezar el proceso de envenamiento
            CakesEatsWithoutDigest = -1;

            //cambiar por texturas envenenado
            unicornStatusTexture.CheckTexturesUnicorn(Complex.Same, StatusUnicorn.Vomit);
            unicornStatusTexture.ChangeColorPlayer(Color.green);

            //Actualizar puntos
            RefreshPointsAndUi(SubstracPoints);
        }
    }

    void RefreshPointsAndUi(int ValueEffect)
    {
        //suma o resta puntaje
        PointsPlayer += ValueEffect;

        //limitar el minimo de puntos a 0
        if (PointsPlayer < 0)
        {
            PointsPlayer = 0;
        }

        //actualizar texto de puntos.
        Points_TMP.text = $"Puntos: {PointsPlayer}";
    }
}
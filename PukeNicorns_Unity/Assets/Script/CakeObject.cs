using UnityEngine;

public class CakeObject : MonoBehaviour
{
    [SerializeField] bool isCakeSpoiled = false;
    [SerializeField] bool isGround = false;
    [Space]
    [SerializeField] float TimeToSpoil = 5;
    [SerializeField] float TimeToDestroy = 5;
    [SerializeField] float TimeToEatCake = 5;
    private float TimerCake;

    private float TimerGrabCake;
    private bool isCakeTaked = false;
    private Collider playerCollider;

    [Space]
    [SerializeField] int PointsToAdd;
    [SerializeField] int PointsToSubstrac;
    [Space]
    [Header("Testing")]
    [SerializeField] Material materialGood;

    //Solo para cambiar el estado del pastel
    private void OnCollisionEnter(Collision collision)
    {
        //Evitar que se mueva o sea activado por otros objetos mientras cae.
        if (collision.gameObject.CompareTag("Piso"))
        {
            //Evitar movimiento extra al pastel
            this.GetComponent<Rigidbody>().useGravity = false;
            this.GetComponent<Rigidbody>().isKinematic = true;

            //Cambiar la forma de colision
            this.GetComponent<BoxCollider>().isTrigger = true;
            this.GetComponent<BoxCollider>().size = new Vector3(1.5f, 2.0f, 1.5f);

            //Aqui cambiar de sprite "cayendo" a "desparramao", al igual que agregar sonido splash

            //Una vez caido, activara el proceso GetSpoiledEffectOrDestroy()
            this.isGround = true;
        }
    }

    //Solo deberia entrar aqui si el pastel ya cayo.
    private void OnTriggerStay(Collider other)
    {
        //Solo si un personaje lo toca y este haya caido hacer: recoger pastel, comer pastel o afectar al personaje
        if (other.CompareTag("Player") && this.isGround)
        {
            /////Solo casos en que el pastel no haya sido tomado.
            //Si el player toca la tecla de accion de tomar y lanzar entonces activara el proceso GrabOrEatCake()
            if (Input.GetKeyDown(other.GetComponent<MovPlayer>().GrabAndLaunch_Action) && isCakeTaked == false)
            {
                //Muestra el tiempo que tienes y que falta para tomar o comer el pastel
                other.GetComponent<MovPlayer>().Slider_Eat.maxValue = TimeToEatCake;

                //Guardar collider del player para los procesos de GrabOrEatCake()
                playerCollider = other;
            }
            //Si el player levanta la tecla, se interrumpira el proceso
            else if(Input.GetKeyUp(other.GetComponent<MovPlayer>().GrabAndLaunch_Action) && isCakeTaked == false)
            {
                //Quitar activador del proceso GrabOrEatCake() y reiniciar timer
                playerCollider = null;
                TimerGrabCake = 0.0f;
            }
            /////
            
            /////Este caso solo deberia pasar si es lanzado y toca al contrario
            //Solo caso en que el pastel haya sido tomado y esta malo
            else if (this.isCakeSpoiled && isCakeTaked)
            {
                //Si el nombre del pastel es el mismo del personaje que lo lanzo, no afectar.
                if(other.name != name)
                {
                    //Afectar solo si el player no esta envenenado
                    if(other.GetComponent<MovPlayer>().CakesEatsWithoutDigest != -1)
                    {
                        //Si es distinto entonces hacer el proceso EffectCake del script del player y restar puntos.
                        other.GetComponent<MovPlayer>().EffectCake(isCakeSpoiled, 0, PointsToSubstrac);
                    }

                    //Despues de afectar, destruir
                    Destroy(this.gameObject);
                }
            }
        }

    }

    ///Solo para interrumpir el proceso de tomar o comer pastel, si el player ya salio del area del pastel
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && this.isGround)
        {
            //Quitar activador del proceso GrabOrEatCake() y reiniciar timer
            TimerGrabCake = 0.0f;
            playerCollider = null;
        }
    }

    ///Revision de los procesos
    private void Update()
    {
        GetSpoiledEffectOrDestroy();

        if (playerCollider)
        {
            GrabOrEatCake(playerCollider);
        }
    }

    //Metodo para pudrir pastel y destruir
    void GetSpoiledEffectOrDestroy()
    {
        //Solo activar si el pastel toco el piso
        if (this.isGround)
        {
            //Comenzar timer
            this.TimerCake += Time.deltaTime;

            //Si el pastel esta bueno
            if (isCakeSpoiled == false)
            {
                //Y el timer es igual o mayor al tiempo para pudrir pastel, hacer el pastel malo
                if (this.TimerCake >= TimeToSpoil)
                {
                    Debug.Log("Cake was spoil");
                    this.isCakeSpoiled = true;

                    //Sonido player afectado por pastel malo

                    //Reiniciar timer
                    this.TimerCake = 0.0f;
                    
                    //Test, cambiar por sprite malo
                    this.GetComponent<MeshRenderer>().material = materialGood;
                }
            }
            //Pero si el pastel esta malo
            else
            {
                //Y ha pasado el tiempo (se agrega la otra condicion, para que no se elimine el pastel en las manos del player)
                if (this.TimerCake >= TimeToDestroy && isCakeTaked == false)
                {
                    Debug.Log("Cake wasted");

                    //Si habra sonido de pastel destruido, agregar aqui

                    //Destruir el pastel malo
                    Destroy(this.gameObject);
                }
            }

        }
    }

    //Metodo para tomar o comer pastel
    void GrabOrEatCake(Collider other)
    {
        //Solo hacer el contador mientras el pastel no hay sido tomado
        if (isCakeTaked == false)
        {
            this.TimerGrabCake += Time.deltaTime;
            other.GetComponent<MovPlayer>().Slider_Eat.value = this.TimerGrabCake;

            //Si hay sonido de player intentando tomar pastel, agregar aqui.
        }
        
        //Si el contador es igual o mayor al tiempo para tomar pastel entones tomar o comer
        if (this.TimerGrabCake >= TimeToEatCake)
        {
            //Tomar pastel solo si el player no tiene otro pastel y si el pastel es malo
            if (isCakeSpoiled && other.GetComponent<MovPlayer>().havePlayerCake == false)
            {
                Debug.Log("Cake was taked");

                //Sonido de pastel tomado


                //Preparar el script del player para el lanzamiento
                other.GetComponent<MovPlayer>().havePlayerCake = true;
                other.GetComponent<MovPlayer>().cakeTaked_GO = this.gameObject;
                other.GetComponent<MovPlayer>().UI_Direction.SetActive(true);

                //Cambiar nombre a este pastel para evitar que afecte al personaje lanzador
                name = other.name;

                //quitar posibilidad de colisiones mientras este en la mano del player y que siga al player
                GetComponent<BoxCollider>().enabled = false;
                this.gameObject.transform.SetParent(other.gameObject.transform, true);

                //Reset timer
                this.TimerGrabCake = 0.0f;

                isCakeTaked = true;
            }
            //De lo contrario solo comer pastel
            else
            {
                Debug.Log("Cake was eated");

                //Sonido pastel comido

                //Hacer proceso EffectCake, sumando puntos
                other.GetComponent<MovPlayer>().EffectCake(isCakeSpoiled, PointsToAdd);

                //Reiniciar, por si acaso se repite
                this.TimerGrabCake = 0.0f;

                //Destruir
                Destroy(this.gameObject);
            }

            //Reiniciar slider
            other.GetComponent<MovPlayer>().Slider_Eat.value = 0;
        }
    }
}

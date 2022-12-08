using UnityEngine;

public class GrabLaunchCake : MonoBehaviour
{
    [Space, Header("Basics vars for launch")]
    [SerializeField] float ForceToLauch = 5;
    public KeyCode GrabAndLaunch_Action;
    public bool havePlayerCake = false;

    [Space, Header("Extra Vars")]
    [SerializeField] GameObject cakeTaked_GO;
    [SerializeField] GameObject UI_Direction;
    [SerializeField] Vector3 DirToLaunch;
    //[SerializeField] float RotationY;
    private bool ReadyToLaunch = false;



    private void Update()
    {
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
                //Preparar pastel para lanzar
                cakeTaked_GO.GetComponent<CakeObject>().IconCakeLaunched(DirToLaunch, ForceToLauch);

                ////Girar pastel  
                //cakeTaked_GO.GetComponent<CakeObject>().UICanvas_Cake.transform.rotation = Quaternion.Euler(0.0f, RotationY, 0.0f);

                //Ocultar direccion de lanzamiento
                UI_Direction.SetActive(false);

                //Resetear valores para el proximo pastel
                cakeTaked_GO = null;
                havePlayerCake = false;
                ReadyToLaunch = false;
            }
        }
    }

    public void GiveDirToLaunch(Vector3 Dir)
    {
        DirToLaunch = Dir;
    }

    public void PrepareLaunch(bool HaveCake, GameObject CakeGO)
    {
        havePlayerCake = HaveCake;
        cakeTaked_GO = CakeGO;
        UI_Direction.SetActive(true);
    }
}

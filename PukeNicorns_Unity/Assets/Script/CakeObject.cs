using UnityEngine;

public class CakeObject : MonoBehaviour
{
    [SerializeField] bool isCakeSpoiled = false;
    [SerializeField] bool isGround = false;
    [SerializeField] float TimeToSpoil = 5;
    [SerializeField] float TimeToDestroy = 5;
    [SerializeField] float TimerCake;

    [Header("Testing")]
    [SerializeField] Material materialGood;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Piso"))
        {
            this.GetComponent<Rigidbody>().useGravity = false;

            this.GetComponent<BoxCollider>().isTrigger = true;
            this.GetComponent<BoxCollider>().size = new Vector3(1.5f, 2.0f, 1.5f);
            this.isGround = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && this.isGround)
        {
            other.GetComponent<MovPlayer>().EffectCake(isCakeSpoiled);
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (this.isGround)
        {
            this.TimerCake += Time.deltaTime;
            if (isCakeSpoiled == false)
            {
                if (this.TimerCake >= TimeToSpoil)
                {
                    Debug.Log("Cake was spoil");
                    this.isCakeSpoiled = true;
                    this.TimerCake = 0.0f;

                    this.GetComponent<MeshRenderer>().material = materialGood;
                }
            }
            else
            {
                if (this.TimerCake >= TimeToDestroy)
                {
                    Debug.Log("Cake wasted");
                    Destroy(this.gameObject);
                }
            }

        }
    }
}

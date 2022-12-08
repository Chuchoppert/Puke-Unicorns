using UnityEngine;
using UnityEngine.UI;

public class TextureUnicornStatus : MonoBehaviour
{
    [SerializeField] Color BasicColor;
    [SerializeField] Image Player_Image;
    [SerializeField] Sprite[] SlimSprites;
    [SerializeField] Sprite[] NormalSprites;
    [SerializeField] Sprite[] FatSprites;
    [Space]

    public int DimensionComplex;
    [SerializeField] int DimensionStatus;
    [SerializeField] Sprite[] TempSprite = null;

    private void Start()
    {
        CheckTexturesUnicorn(Complex.Normal, StatusUnicorn.Idle);
    }

    public void CheckTexturesUnicorn(Complex complex, StatusUnicorn status)
    {
        if(complex != Complex.Same)
        {
            if (complex == Complex.Slim)
            {
                DimensionComplex = 0;
                TempSprite = SlimSprites;
            }
            else if (complex == Complex.Normal)
            {
                DimensionComplex = 1;
                TempSprite = NormalSprites;
            }
            else if (complex == Complex.Fat)
            {
                DimensionComplex = 2;
                TempSprite = FatSprites;
            }
        }

        if(status != StatusUnicorn.Same)
        {
            if (status == StatusUnicorn.Idle)
            {
                DimensionStatus = 0;
    
            }
            else if (status == StatusUnicorn.Eat)
            {
                DimensionStatus = 1;
            }
            else if (status == StatusUnicorn.Vomit)
            {
                DimensionStatus = 2;
            }
        }

        Player_Image.sprite = TempSprite[DimensionStatus];
    }

    public void ChangeColorPlayer(Color color)
    {
        Player_Image.color = color;
    }

    public void ResetColorPlayer()
    {
        Player_Image.color = BasicColor;
    }
}


public enum Complex
{
    Slim,
    Normal,
    Fat,
    Same
}

public enum StatusUnicorn
{
    Idle,
    Eat,
    Vomit,
    Same
}
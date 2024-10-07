using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChanger : MonoBehaviour {

    public GameObject spriteObject;
    Maps maps;

    public Sprite spriteAlBasrah;
    public Sprite spriteBelaya;
    public Sprite spriteChora;
    public Sprite spriteFoolsRoad;
    public Sprite spriteOPFirstLight;
    public Sprite spriteGorodok;
    public Sprite spriteJensensRange;
    public Sprite spriteKohat;
    public Sprite spriteKokan;
    public Sprite spriteLogarValley;
    public Sprite spriteMestia;
    public Sprite spriteNarva;
    public Sprite spriteSumari;
    public Sprite spriteYehorivka;


    float scaleBasrah = 3200f;
    float scaleBelaya = 3910f;
    float scaleChora = 4063f;
    float scaleFoolsRoad = 1769f;
    float scaleOPFirstLight = 1201f; 
    float scaleGorodok = 4340f;
    float scaleJensensRange = 1510f; 
    float scaleKohat = 4015f;
    float scaleKokan = 2496f;
    float scaleLogarValley = 1763f;
    float scaleMestia = 2400f;
    float scaleNarva = 2800f;
    float scaleSumari = 1300f;     
    float scaleYehorivka = 4034f;



    // Use this for initialization
    void Start () {
        maps = this.GetComponent<Maps>();
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DropdownChanged(int i)
    {
        switch (i)
        {
            case 0:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteAlBasrah;
                maps.MapChanged(scaleBasrah,i);
                break;

            case 1:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteBelaya;
                maps.MapChanged(scaleBelaya, i);
                break;

            case 2:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteChora;
                maps.MapChanged(scaleChora,i);
                break;

            case 3:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteFoolsRoad;
                maps.MapChanged(scaleFoolsRoad,i);
                break;


            case 4:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteGorodok;
                maps.MapChanged(scaleGorodok,i);
                break;

            case 5:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteJensensRange;
                maps.MapChanged(scaleJensensRange,i);
                break;

            case 6:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteKohat;
                maps.MapChanged(scaleKohat,i);
                break;

            case 7:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteKokan;
                maps.MapChanged(scaleKokan,i);
                break;

            case 8:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteLogarValley;
                maps.MapChanged(scaleLogarValley,i);
                break;

            case 9:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteMestia;
                maps.MapChanged(scaleMestia,i);
                break;

            case 10:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteNarva;
                maps.MapChanged(scaleNarva,i);
                break;

            case 11:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteOPFirstLight;
                maps.MapChanged(scaleOPFirstLight,i);
                break;

            case 12:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteSumari;
                maps.MapChanged(scaleSumari,i);
                break;

            case 13:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteYehorivka;
                maps.MapChanged(scaleYehorivka,i);
                break;

            default:
                spriteObject.GetComponent<SpriteRenderer>().sprite = spriteAlBasrah;
                maps.MapChanged(scaleBasrah,i);
                break;
        }


    }
}

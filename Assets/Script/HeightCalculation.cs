using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightCalculation : MonoBehaviour {

    public Texture2D HeightmapBasrah;
    public Texture2D HeightmapBelaya;
    public Texture2D HeightmapChora;
    public Texture2D HeightmapFoolsRoad;
    public Texture2D HeightmapFirstLight;
    public Texture2D HeightmapGorodok;
    public Texture2D HeightmapKohat;
    public Texture2D HeightmapKokan;
    public Texture2D HeightmapLogar;
    public Texture2D HeightmapMestia;
    public Texture2D HeightmapNarva;
    public Texture2D HeightmapSumari;
    public Texture2D HeightmapYehorivka;

    Texture2D currentHeightmap;
    CalculationNew calcNew;


    float currentMin;
    float currentMax;

    //flaot values for the maps
    float BasrahMin = -15.6f;
    float BasrahMax = 20.7f; //highest point on map is 13.5 on 200 greyscale

	float BelayaMin = -307f;	//-30500
	float BelayaMax = -251f;

    float ChoraMin =  0f; //-780
    float ChoraMax = 42f; // -738
  
    float FoolsMin = -58f; //9f
    float FoolsMax = 15f; //63f

    float GoroMin = -60f; //-60f
    float GoroMax = -6f; // -6f

    float KohatMin = 9f;
    float KohatMax = 392f;

    float KokanMin = 0f; //-254f
    float KokanMax = 88f; //-166

    float LogarMin = 32; //3f
    float LogarMax = 88f; //13f

    float MestiaMin = -153f;
    float MestiaMax = 67f;
    
    float NarvaMin = -8f; //-4f
    float NarvaMax = 45f; //44.7

    float FirstLightMin = -7f;
    float FirstLightMax = 21.5f;

    float SumariMin = 0f; //-13.8f -13800
    float SumariMax = 114f; //-2.0f   -2000

    float YehorivkaMin = -20f; //-14f
    float YehorivkaMax = 118f; // 120f


	// Use this for initialization
	void Start () {
        calcNew = this.GetComponent<CalculationNew>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetCorrectHeightmap(int i)
    {
        currentHeightmap = HeightmapFirstLight;
        currentMin = FirstLightMin;
        currentMax = FirstLightMax;

        switch (i)
        {
            case 0:
                currentHeightmap = HeightmapBasrah;
                currentMin = BasrahMin;
                currentMax = BasrahMax;
                break;

            case 1:
				currentHeightmap = HeightmapBelaya; //belaya
                currentMin = BelayaMin;
                currentMax = BelayaMax;
                break;

            case 2:
                currentHeightmap = HeightmapChora;
                currentMin = ChoraMin;
                currentMax = ChoraMax;
                break;

            case 3:
                currentHeightmap = HeightmapFoolsRoad;
                currentMin = FoolsMin;
                currentMax = FoolsMax;
                break;

            case 4:
                currentHeightmap = HeightmapGorodok;
                currentMin = GoroMin;
                currentMax = GoroMax;
                break;

            case 5:
                currentHeightmap = null; //jensens
                currentMin = 0f;
                currentMax = 0f;
                break;

            case 6:
                currentHeightmap = HeightmapKohat;
                currentMin = KohatMin;
                currentMax = KohatMax;
                break;

            case 7:
                currentHeightmap = HeightmapKokan;
                currentMin = KokanMin;
                currentMax = KokanMax;
                break;

            case 8:
                currentHeightmap = HeightmapLogar;
                currentMin = LogarMin;
                currentMax = LogarMax;
                break;

            case 9:
                currentHeightmap = HeightmapMestia;
                currentMin = MestiaMin;
                currentMax = MestiaMax;
                break;

            case 10:
                currentHeightmap = HeightmapNarva;
                currentMin = NarvaMin;
                currentMax = NarvaMax;
                break;

            case 11:
                currentHeightmap = HeightmapFirstLight;
                currentMin = FirstLightMin;
                currentMax = FirstLightMax;
                break;

            case 12:
                currentHeightmap = HeightmapSumari;
                currentMin = SumariMin;
                currentMax = SumariMax;
                break;

            case 13:
                currentHeightmap = HeightmapYehorivka;
                currentMin = YehorivkaMin;
                currentMax = YehorivkaMax;
                break;

            default:
                currentHeightmap = HeightmapBasrah;
                currentMin = BasrahMin;
                currentMax = BasrahMax;
                break;
        }


    }


    public float GetHeightFromTexturePoint(Vector2 pos)
    {

        if (currentHeightmap == null)
            return 0f;
        //normalize targetPosition (from 40.94 to 4096 etc.)
        Vector2 tempPos = new Vector2(pos.x * 100f, pos.y * -100f);
        //convert to texture coordinate system (0/0 bottom left insteand of top left)
        tempPos.y = 4096f-tempPos.y;

        //grayscale in from 1 to 256 (maybe +1 to prevent 0 to 255)
        //256 / grayscale = factor
        //maxvalue - minvalue (clear offset) / factor = actual Height
        //add offset
        Color32 color = currentHeightmap.GetPixel((int)tempPos.x, (int)tempPos.y);
        float grayscale = (float)color.r;
    //    Debug.Log(grayscale);
        float factor = (currentMax - currentMin) / 255f;

        float actualHeight;
            actualHeight = (factor * color.r ) + currentMin;


        return actualHeight;
    }
    
    public float CalculateDistanceOffset(float mil, float originalDistance, float heightOffset)
    {
     //   Debug.Log("heightoffset " + heightOffset);
        float degrees = mil * 0.05625f;
        degrees = 90f - degrees;
      //  Debug.Log("deg " + degrees + " / mil " + mil);

        //transfer to radians because Mathf.Tan() only takes rad
        float rad = degrees * Mathf.PI / 180f;
        float distanceOffset = (Mathf.Tan(rad) )* heightOffset;
   //       Debug.Log("offset " + distanceOffset + "m");



        //do some Karm Calculations
        float combinedDistance = originalDistance + distanceOffset;
        // calculate new Mil 
        float newMil = calcNew.CalcMil(combinedDistance);
        //transform mil to degree
        float newDegrees = newMil * 0.05625f;
        //calculation the angle difference with more crazy karm calculations
        float angleDifference = (2 * newDegrees) * (distanceOffset / combinedDistance);
        //
        float finalDistance = (Mathf.Tan((degrees + (angleDifference/2)) * Mathf.PI / 180f) *heightOffset) ;


   //     Debug.Log("offset " + finalDistance + "m");

        return finalDistance;
    }
}

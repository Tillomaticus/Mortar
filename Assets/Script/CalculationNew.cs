using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CalculationNew : MonoBehaviour
{
    Maps maps;
    HeightCalculation heightCalc;

    public static string clipBoard {
        get { return GUIUtility.systemCopyBuffer; }
        set { GUIUtility.systemCopyBuffer = value; }
    }

    public GameUI gameUI;
    public TargetStore targetStore;
    Vector2 mortar = new Vector2(20.48f, -20.48f);
    Vector2 target = new Vector2(20.48f, -20.48f);
    float mortarHeight = 0f;
    float targetHeight = 0f;
    float distance;
    float angle;
    int mil = 0;
    float manualHeightoffset = 0f;

    float gridScale;
    float xValue;
    float yValue;
    float xGrid;
    float yGrid;
    float keypad;
    float subKeypadX;
    float subKeypadY;

    float heightOffset = 0f;
    string mortarInput;
    string targetInput;

	bool target1Active=true;

	public Wireframe wireframeTarget1;
	public Wireframe wireframeTarget2;

    // string inputString;

    public char[] letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    float[] meterPresets = new float[] { 1250f, 1200f, 1150f, 1100f, 1050f, 1000f, 950f, 900f, 850f, 800f, 750f, 700f, 650f, 600f, 550f, 500f, 450f, 400f, 350f, 300f, 250f, 200f, 150f, 100f, 50f };
    float[] averageMeterPerMil = new float[] { 2.36f, 1.4f, 1.02f, 0.84f, 0.74f, 0.68f, 0.64f, 0.58f, 0.56f, 0.54f, 0.5f, 0.5f, 0.48f, 0.48f, 0.46f, 0.44f, 0.44f, 0.44f, 0.44f, 0.44f, 0.42f, 0.42f, 0.42f, 0.42f, 0.42f };
    float[] milPresets = new float[] { 800f, 918f, 988f, 1039f, 1081f, 1118f, 1152f, 1183f, 1212f, 1240f, 1267f, 1292f, 1317f, 1341f, 1364f, 1387f, 1409f, 1431f, 1453f, 1475f, 1496f, 1517f, 1538f, 1558f, 1579f };

    private void Start()
    {
        gameUI = GetComponent<GameUI>();
        maps = GetComponent<Maps>();
        heightCalc = GetComponent<HeightCalculation>();
    }


    public void StartCalculation()
    {
        if(mortar != null && target != null)
            DoIt();
    }


    public void UpdateGridScale(float f)
    {
        gridScale = f;
    }

    public void SetMortarPosition(Vector3 newPos, float height)
    {
        mortar = newPos;
        mortarHeight = height;
    }

	public void SetTargetPosition(Vector3 newPos, float height, bool isTarget1)
    {
		target1Active = isTarget1;
        target = newPos;
        targetHeight = height;
    }

    public void SetManualHeightOffset(float f)
    {
        if (f == 0)
            manualHeightoffset = 0;
        else
            manualHeightoffset += f;

        manualHeightoffset= Mathf.Clamp(manualHeightoffset, -50f, 50f);
        gameUI.UpdateManualHeightOffset(manualHeightoffset);
        StartCalculation();
    }


    void DoIt()
    {
        distance = Vector2.Distance(mortar, target);
        distance = distance *gridScale;

        CalcDistance();
        CalcAngle();

		UpdateMil(target1Active);
		UpdateUI(target1Active);
    }


	//TO DO: FUNCTION WORKS BUT Clicks to do not always get recognized  
	public void CopyToClipboard()
	{
		if (angle >= 370f || mil <= -1f)
			return;
		
		string newClipboardString;
		newClipboardString ="Angle: " + angle.ToString ("F1") + "° " + "MIL: " + mil.ToString();   
		clipBoard = newClipboardString;
	}


    public void UpdateFromTargetStore(Vector2 newMortar, Vector2 newTarget, float newDistance, float newAngle, int newMil)
    {
		//keep mortar as currently set, if newStorage isnt set yet
		if(newMortar != new Vector2(20.48f, -20.48f))
        mortar = newMortar;
		
        target = newTarget;
        distance = newDistance;
        angle = newAngle;
        mil = newMil;

        //recalculate Height
         mortarHeight = heightCalc.GetHeightFromTexturePoint(mortar);
         targetHeight = heightCalc.GetHeightFromTexturePoint(target); 
	


        DoIt();
        maps.SetMortarMarkerByPosition(mortar);
		maps.SetMortarTargetByPosition(target,target1Active);
    }


    public void UpdateTargetStore()
    {
        targetStore.UpdateStorage(mortar, target, distance, angle, mil);
    }

    void ThrowUIError(GameUI.ErrorCode errorCode)
    {
        gameUI.ThrowError(errorCode, target1Active);
    }

	private void UpdateUI(bool isTarget1)
    {
        float heightDifference = heightOffset;
        gameUI.UpdateDistanceText((int)distance, heightDifference,isTarget1);
        gameUI.UpdateAngleText(angle,isTarget1);
    }
	private void UpdateMil(bool isTarget1)
    {
        if(mil > 1)
			gameUI.UpdateMilText(mil,isTarget1);
    }

    void CalcDistance()
    {    
        mil = -1;
        
        //calculate MIL for the first time to have an angle to calculate the height offset with
        //calculating MIL for a straight flat LINE
        mil = CalcMil(distance);

  //      Debug.Log("original mil " + mil);


        //change distance to "0" for height calc
       float distanceWithOffset=0;

        heightOffset = targetHeight - mortarHeight;

        //add the manual heightoffset from the gui
        heightOffset += manualHeightoffset;
        if (mil > 1)
        {
            distanceWithOffset = distance + heightCalc.CalculateDistanceOffset(mil, distance, heightOffset);
        }
        else return;
        
        
        
        //OBSOLETE BECAUSE INSINDE CALC MIL
        /*
        //too short for firing
        if (distanceWithOffset < 50f)
        {
            UpdateUI();
            ThrowUIError(GameUI.ErrorCode.tooShort);
            mil = -1;
            return;
        }
        */

        //too long for firing
        if (distanceWithOffset > 1250f)
        {
			UpdateUI(target1Active);
            ThrowUIError(GameUI.ErrorCode.tooLong);
            mil = -1;
            return;
        }


        mil = CalcMil(distanceWithOffset);
		ApplToWireframe (distanceWithOffset);


	}

	void ApplToWireframe(float distanceWithOffset)
	{

		//get adjusted distance - > scale it to mapsize / map distance
		//multiply it up vector from mortar pos
		float tempDistance = (DifferenceInDistanceForExtraMil (distanceWithOffset, 5) + distance) / gridScale;
		Vector2 overshoot = mortar + (Vector2.up * tempDistance);

		tempDistance =(DifferenceInDistanceForExtraMil (distanceWithOffset, -5) + distance) / gridScale;
		Vector2 undershoot = mortar + (Vector2.up * tempDistance);

		tempDistance =(DifferenceInDistanceForExtraMil (distanceWithOffset, 10) + distance) / gridScale;
		Vector2 heavyOvershoot = mortar + (Vector2.up * tempDistance);

		tempDistance =(DifferenceInDistanceForExtraMil (distanceWithOffset, -10) + distance) / gridScale;
		Vector2 heavyUndershoot = mortar + (Vector2.up * tempDistance);

		Vector2 center = mortar + (Vector2.up * (distance/gridScale));

		Vector2 tempVector;
		GameObject currentWirfeframeTarget;

		if (target1Active) 
		{
		//CENTER POINT
		tempVector = RotateAroundPoint(center, mortar, angle);

		wireframeTarget1.AssignSpreadPos(4,tempVector.x, tempVector.y);

		//topMiddle
		tempVector = RotateAroundPoint(overshoot, mortar,  angle);
		wireframeTarget1.AssignSpreadPos(1, tempVector.x,tempVector.y);

		//bottomMiddle
		tempVector = RotateAroundPoint(undershoot, mortar,  angle);
		wireframeTarget1.AssignSpreadPos(7, tempVector.x, tempVector.y);


	//CENTER TOP
		//top left
		tempVector = RotateAroundPoint(overshoot, mortar,  angle-1f);
		wireframeTarget1.AssignSpreadPos (0, tempVector.x, tempVector.y);

		//top right
		tempVector = RotateAroundPoint(overshoot, mortar,  angle+1f);
		wireframeTarget1.AssignSpreadPos (2, tempVector.x, tempVector.y);


	// CENTER MIDDLE
		//mid left
		tempVector = RotateAroundPoint(center, mortar,  angle-1f);
		wireframeTarget1.AssignSpreadPos (3, tempVector.x, tempVector.y);

		//mid right
		tempVector = RotateAroundPoint(center, mortar,  angle+1f);
		wireframeTarget1.AssignSpreadPos (5, tempVector.x, tempVector.y);

	//CENTER BOT
		//bot left
		tempVector = RotateAroundPoint(undershoot, mortar,  angle-1f);
		wireframeTarget1.AssignSpreadPos (6, tempVector.x, tempVector.y);

		//bot right
		tempVector = RotateAroundPoint(undershoot, mortar,  angle+1f);
		wireframeTarget1.AssignSpreadPos (8, tempVector.x, tempVector.y);


	//TOP ROW
		tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle-2f);
		wireframeTarget1.AssignSpreadPos (9, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle-1f);
		wireframeTarget1.AssignSpreadPos (10, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle);
		wireframeTarget1.AssignSpreadPos (11, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle+1f);
		wireframeTarget1.AssignSpreadPos (12, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle+2f);
		wireframeTarget1.AssignSpreadPos (13, tempVector.x, tempVector.y);


	//LEFT ROW
		tempVector = RotateAroundPoint(overshoot, mortar,  angle-2f);
		wireframeTarget1.AssignSpreadPos (14, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(center, mortar,  angle-2f);
		wireframeTarget1.AssignSpreadPos (16, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(undershoot, mortar,  angle-2f);
		wireframeTarget1.AssignSpreadPos (18, tempVector.x, tempVector.y);

	//RIGHT ROW
		tempVector = RotateAroundPoint(overshoot, mortar,  angle+2f);
		wireframeTarget1.AssignSpreadPos (15, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(center, mortar,  angle+2f);
		wireframeTarget1.AssignSpreadPos (17, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(undershoot, mortar,  angle+2f);
		wireframeTarget1.AssignSpreadPos (19, tempVector.x, tempVector.y);

		//BOT ROW
		tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle-2f);
		wireframeTarget1.AssignSpreadPos (20, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle-1f);
		wireframeTarget1.AssignSpreadPos (21, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle);
		wireframeTarget1.AssignSpreadPos (22, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle+1f);
		wireframeTarget1.AssignSpreadPos (23, tempVector.x, tempVector.y);

		tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle+2f);
		wireframeTarget1.AssignSpreadPos (24, tempVector.x, tempVector.y);

		wireframeTarget1.ApplyPositionsToMeshes ();	
		} 
		else 
		{
			//CENTER POINT
			tempVector = RotateAroundPoint(center, mortar, angle);

			wireframeTarget2.AssignSpreadPos(4,tempVector.x, tempVector.y);

			//topMiddle
			tempVector = RotateAroundPoint(overshoot, mortar,  angle);
			wireframeTarget2.AssignSpreadPos(1, tempVector.x,tempVector.y);

			//bottomMiddle
			tempVector = RotateAroundPoint(undershoot, mortar,  angle);
			wireframeTarget2.AssignSpreadPos(7, tempVector.x, tempVector.y);


			//CENTER TOP
			//top left
			tempVector = RotateAroundPoint(overshoot, mortar,  angle-1f);
			wireframeTarget2.AssignSpreadPos (0, tempVector.x, tempVector.y);

			//top right
			tempVector = RotateAroundPoint(overshoot, mortar,  angle+1f);
			wireframeTarget2.AssignSpreadPos (2, tempVector.x, tempVector.y);


			// CENTER MIDDLE
			//mid left
			tempVector = RotateAroundPoint(center, mortar,  angle-1f);
			wireframeTarget2.AssignSpreadPos (3, tempVector.x, tempVector.y);

			//mid right
			tempVector = RotateAroundPoint(center, mortar,  angle+1f);
			wireframeTarget2.AssignSpreadPos (5, tempVector.x, tempVector.y);

			//CENTER BOT
			//bot left
			tempVector = RotateAroundPoint(undershoot, mortar,  angle-1f);
			wireframeTarget2.AssignSpreadPos (6, tempVector.x, tempVector.y);

			//bot right
			tempVector = RotateAroundPoint(undershoot, mortar,  angle+1f);
			wireframeTarget2.AssignSpreadPos (8, tempVector.x, tempVector.y);


			//TOP ROW
			tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle-2f);
			wireframeTarget2.AssignSpreadPos (9, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle-1f);
			wireframeTarget2.AssignSpreadPos (10, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle);
			wireframeTarget2.AssignSpreadPos (11, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle+1f);
			wireframeTarget2.AssignSpreadPos (12, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(heavyOvershoot, mortar,  angle+2f);
			wireframeTarget2.AssignSpreadPos (13, tempVector.x, tempVector.y);


			//LEFT ROW
			tempVector = RotateAroundPoint(overshoot, mortar,  angle-2f);
			wireframeTarget2.AssignSpreadPos (14, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(center, mortar,  angle-2f);
			wireframeTarget2.AssignSpreadPos (16, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(undershoot, mortar,  angle-2f);
			wireframeTarget2.AssignSpreadPos (18, tempVector.x, tempVector.y);

			//RIGHT ROW
			tempVector = RotateAroundPoint(overshoot, mortar,  angle+2f);
			wireframeTarget2.AssignSpreadPos (15, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(center, mortar,  angle+2f);
			wireframeTarget2.AssignSpreadPos (17, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(undershoot, mortar,  angle+2f);
			wireframeTarget2.AssignSpreadPos (19, tempVector.x, tempVector.y);

			//BOT ROW
			tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle-2f);
			wireframeTarget2.AssignSpreadPos (20, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle-1f);
			wireframeTarget2.AssignSpreadPos (21, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle);
			wireframeTarget2.AssignSpreadPos (22, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle+1f);
			wireframeTarget2.AssignSpreadPos (23, tempVector.x, tempVector.y);

			tempVector = RotateAroundPoint(heavyUndershoot, mortar,  angle+2f);
			wireframeTarget2.AssignSpreadPos (24, tempVector.x, tempVector.y);

			wireframeTarget2.ApplyPositionsToMeshes ();	
		}


	
    }


	Vector2 RotateAroundPoint(Vector2 pos, Vector2 point, float angle)
	{
		Vector2 dir = pos - point;

		dir = Quaternion.Euler (new Vector3 (0, 0, -angle)) * dir;

		pos = dir + point;

		return pos;
	}


/*	float  LookupDistanceFromMil(int tempMil, int addition)
	{
		if (mil < 800 || mil > 1579)
			return 0;

		float tempDist;
		for (int i = 0; i < 26; i++) {

			if((tempMil - (int)milPresets[i]) > 0)
				continue;
			
			tempDist = meterPresets [i];

			tempMil -= (int)milPresets [i];

			if(tempMil < 0)
				tempDist += (tempMil * averageMeterPerMil [i]);

			return tempDist + ;

		}
		return -1f;
	}
	*/

	float DifferenceInDistanceForExtraMil(float distance, int addition)
	{
		if (distance < 50f || distance > 1250)
			return -1;
		
		for (int i = 0; i < 26; i++) {
			if ((distance - meterPresets [i]) < 0)
				continue;

			return ( addition / averageMeterPerMil [i]);
		}
		return -1;


	}

    //lookup highest possible firing range and set the equivalent mil then and add averageMeterPerMil for the remaining meters
    //this is only an approach, since the averageMeterPerMil seems not to be exponential. At least with the corresponding rounded milPresets.
    public int CalcMil(float tempDistance)
    {
        //early out to prevent array out of index
        //also throw error if too short
        if (tempDistance < 50f)
        {
			UpdateUI(target1Active);
            ThrowUIError(GameUI.ErrorCode.tooShort);
            return -1;
        }
        int tempMil;
        for (int i = 0; i < 26; i++)
        {
            if ((tempDistance - meterPresets[i]) < 0)
                continue;

            tempMil = (int)milPresets[i];
            tempDistance -= meterPresets[i];
            if (tempDistance > 0)
            {
                tempMil -= (int)(tempDistance * (averageMeterPerMil[i]));
            }

            return tempMil;
        }
        return -1;
    }


    void CalcAngle()
    {
        angle = 0;

        //had to turn angle by 180
        /*  //Build north vektor on arty-position
             float nv_x = mortar.x - mortar.x;
             float nv_y = (target.y - 1) - target.y;
             float abs_nv = Mathf.Sqrt((Mathf.Pow(nv_x, 2) + (Mathf.Pow(nv_y, 2))));

             //#Build Targetvektor
             float tv_x = target.x - mortar.x;
             float tv_y = target.y - mortar.y;
             float abs_tv = Mathf.Sqrt((Mathf.Pow(tv_x, 2) + (Mathf.Pow(tv_y, 2))));
             */

        //Build north vektor on arty-position
        float nv_x = target.x - target.x;
        float nv_y = (mortar.y - 1) - mortar.y;
        float abs_nv = Mathf.Sqrt((Mathf.Pow(nv_x, 2) + (Mathf.Pow(nv_y, 2))));

        //#Build Targetvektor
        float tv_x = mortar.x - target.x;
        float tv_y = mortar.y - target.y;
        float abs_tv = Mathf.Sqrt((Mathf.Pow(tv_x, 2) + (Mathf.Pow(tv_y, 2))));


        //  #Skalar between nv and tv
        float skalar = nv_x * tv_x + nv_y * tv_y;


        if (mortar.x != target.x || mortar.y != target.y)
            angle = Mathf.Rad2Deg * (Mathf.Acos(skalar / (abs_nv * abs_tv)));


        // Shoot NE - QI
        if (target.x > mortar.x && target.y < mortar.y)
            angle = angle;

        // Shoot NW - QII
        else if (target.x < mortar.x && target.y < mortar.y)
            angle = 360 - angle;

        // Shoot SW - QIII
        else if (target.x < mortar.x && target.y > mortar.y)
            angle = 360 - angle;

        // Shoot SE - QIV
        else if (target.x > mortar.x && target.y > mortar.y)
            angle = angle;

        // Shoot direct North
        else if (target.x == mortar.x && target.y > mortar.y)
            angle = 0;

        // Shoot direct South
        else if (target.x == mortar.x && target.y < mortar.y)
            angle = 180;

        // Shoot direct East
        else if (target.x > mortar.x && target.y == mortar.y)
            angle = 90;

        // Shoot direct West
        else if (target.x < mortar.x && target.y == mortar.y)
            angle = 270;

        // Fail
        else
            angle = 999;


    }


    //obsolete
    Vector2 AddUp()
    {
        //Adds offset 2 for left keypad row, 5 for middle row, and 8 for right row 
        xValue = (xGrid - 1) * 9;
        xValue += 2f + ((((int)keypad - 1) % 3) * 3f);

        //Adds offset 2 for top keypad row, 5 for middle row, and 8 for bottom row 
        yValue = (yGrid - 1) * 9;
        yValue += (8.0f - (((int)(keypad - 1) / 3) * 3));


        //Add Subkeypads
        //if greater 5 -> add up, if smaller 5 -> subtract, if exactly 5 (center subkeypad) do nothing
        if (subKeypadX > 5)
            xValue += (subKeypadX * 0.1f - 0.5f);
        if (subKeypadX < 5)
            xValue -= (0.5f - subKeypadX * 0.1f);

        //same with y
        if (subKeypadY > 5)
            yValue += (subKeypadY * 0.1f - 0.5f);
        if (subKeypadY < 5)
            yValue -= (0.5f - subKeypadY * 0.1f);



        return new Vector2(xValue, yValue);
    }
}


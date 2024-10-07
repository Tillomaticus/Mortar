using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




/*
 * 
 *  First click, set mortar - then all left clicks are new target position
 *  then shift click sets new mortar 
 *  drag click to drag symbols
 *  
 *  Bug Inverted Angle
 *  Bug Wrong Distance Calculation
 * 
 * 
 * 
 *  
 * 
 */

public class Maps : MonoBehaviour {

    public Text mortarOutput;
    public Text targetOutput;
    public Text mousePosOutput;
    public CalculationNew calculation;
    public GameObject spriteObject;

	public GameObject wireframeGroup1;
	public GameObject wireframeGroup2;

    public GameObject mortarMarker;
    public GameObject mortarRangeMarker;
    public GameObject targetMarker;
	public GameObject targetMarker2;

    public GameObject bigLineV;
    public GameObject bigLineH;
    public GameObject keyPadLineV;
    public GameObject keyPadLineH;
    public GameObject SUBkeyPadLineV;
    public GameObject SUBkeyPadLineH;

    public GameObject gridCoordTemplate;
    public GameObject sphere;

    GameObject GridParent;
    GameObject KeyPadParent;
    GameObject SUBKeyParent;
    GameObject GridCoordParent;

    HeightCalculation heightCalc;


    float dragMapFactor = 0.15f;

    private float mapSize = 4096;
    float mapFactor;

    bool isFirstClick=true;
    bool isShiftClick = false;

    Camera camera;
    Vector3 newCameraPos;
    float cameraSizeMin = 1f;
    float cameraSizeMax = 20f;
    public float cameraSize = 20f;

    float currentMapScale;

    float scaleBasrah = 3200f;

    float deltaMouseWheel=0f;

    float currentMortarHeight = 0f;
    float currentTargetHeight = 0f;

	bool target1Active = true;
   


    // Use this for initialization
    void Start () {
        camera = this.GetComponent<Camera>();
        newCameraPos = camera.transform.position;
        calculation = this.GetComponent<CalculationNew>();
        heightCalc = this.GetComponent<HeightCalculation>();

		targetMarker2.transform.position = new Vector3 (0f, 0f, 0.5f);


	

        MapChanged(scaleBasrah,0);

		ToggleGridCoords ();
		ToggleWireframe ();


    }
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Alpha1) || Input.GetKeyDown (KeyCode.Keypad1))
			target1Active = true;
		if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
			target1Active=false;
    }



    public void MapChanged(float f, int mapNumber)
    {

        if (GridParent != null)
            Destroy(GridParent);

        if (KeyPadParent != null)
            Destroy(KeyPadParent);

        if (SUBKeyParent != null)
            Destroy(SUBKeyParent);

        currentMapScale = f;
        mapFactor = currentMapScale / mapSize;
        calculation.UpdateGridScale(mapFactor * 100f);

        GridParent = new GameObject("GridParent");
        GridCoordParent = new GameObject("GridCoordParent");
        GridCoordParent.transform.parent = GridParent.transform;

        KeyPadParent = new GameObject("KeyPadParent");
        KeyPadParent.transform.parent = GridParent.transform;

        SUBKeyParent = new GameObject("SUBKeypads");
        SUBKeyParent.transform.parent = GridParent.transform;

        CreateGrid();
        KeyPadParent.SetActive(false);
        SUBKeyParent.SetActive(false);


        //mapfactor * 100, because 100 is the factor for the map (4096) to unity space conversion (40,96)
        calculation.UpdateGridScale(mapFactor * 100f);
        SetMortarRangeMarkerScaling();

        heightCalc.SetCorrectHeightmap(mapNumber);


        //reset height offset, incase people forget
        calculation.SetManualHeightOffset(0);

    }

    void SetMortarRangeMarkerScaling()
    {
        float f = currentMapScale / 40.96f;
        float g = 1250f / f / 5f;
        mortarRangeMarker.transform.localScale = new Vector3(g, g, 1f);
    }

    public void QuitGame()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            Screen.fullScreen = false;
        else
            Application.Quit();
    }

    private void FixedUpdate()
    {
        isShiftClick = false;

        if (Input.GetKey(KeyCode.LeftShift))
            isShiftClick = true;

        deltaMouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (deltaMouseWheel != 0f)
            RescaleMap(deltaMouseWheel);

        if (Input.GetMouseButton(0))
            DragMap();
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            DragMapWithKeys();


        GetMouseOverMap();


     //   if (Input.GetKeyDown(KeyCode.Escape))
     //       Application.Quit();


        if(Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            dragMapFactor += 0.01f;

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            dragMapFactor -= 0.01f;

        if (Input.GetKeyDown(KeyCode.C))
            ToggleGridCoords();

        if (Input.GetKeyDown(KeyCode.X))
            ToggleRangeMarker();

		if (Input.GetKeyDown (KeyCode.V))
			ToggleWireframe ();


        dragMapFactor = Mathf.Clamp(dragMapFactor, 0.0001f, 0.25f);
    }

	public void ToggleWireframe()
	{
			wireframeGroup1.SetActive (!wireframeGroup1.activeSelf);

			wireframeGroup2.SetActive (!wireframeGroup2.activeSelf);
	
	}

    public void ToggleGridCoords()
    {
        if(GridCoordParent.activeSelf)
            GridCoordParent.SetActive(false);
        else
            GridCoordParent.SetActive(true);
    }

    public void ToggleRangeMarker()
    {
        if (mortarRangeMarker.activeSelf)
            mortarRangeMarker.SetActive(false);
        else
            mortarRangeMarker.SetActive(true);
    }

    void GetMouseOverMap()
    {
        float xAxis;
        float yAxis;
        Ray ray;
        RaycastHit hit;

        Vector3 rayVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ray = new Ray(rayVector, Vector3.forward * 11f);


        //BUG HERE subkeypads -2 at red lines
        if (Physics.Raycast(ray, out hit))
        {
            xAxis = hit.point.x * 100f * mapFactor;
            yAxis = hit.point.y * 100f * mapFactor;

            //X AXIS
            //divide by size of biggrid then subtract the biggrid
            int xBig = (int)(xAxis / 300f);
            float f = xAxis - xBig * 300;

            //divide the rest by keypad, then subtract the keypad, increase to get keypad 1-3 instead of 0-2
            int xKeypad = (int)(f / 100);
            f = f - (xKeypad * 100);
            xKeypad++;

            //divide the rest by subkeypad, increase like top // f can be 3, (if we are at 99+m (last meter failure) 
            int xSubKeypad = (int) (f / 33);
            if (xSubKeypad >= 3)
                xSubKeypad = 2;
            f = f - (xSubKeypad * 33);

            xSubKeypad++;

            //Y AXIS
            int yBig = (int)(yAxis / 300f);
            yBig *= -1;
            float g = (yAxis*-1) - yBig * 300;
            int yKeypad = (int)(g / 100);
            g = g - (yKeypad * 100);
            yKeypad = 2 - yKeypad;


            int ySubKeypad = (int)(g / 33);
            if (ySubKeypad >= 3)
                ySubKeypad = 2;
            g = g - (ySubKeypad * 33);
            ySubKeypad = 2 - ySubKeypad;

            //offset, because yGrid starts with 1 not with 0
            yBig += 1;
            string gridString = calculation.letters[xBig] + "" + yBig + " K" + (xKeypad + yKeypad * 3) + "-" + (xSubKeypad + ySubKeypad * 3);

            mousePosOutput.text = "Mouse Position: " + gridString;
            //if mouseclick
            if (Input.GetMouseButton(1))
                ApplyClick(hit, gridString);
        }

    }

    void ApplyClick(RaycastHit hit, string s)
    {
		if (isFirstClick || isShiftClick)
			SetMortarMarker (hit, s);
		else 
			SetMortarTarget (hit, s);
	

        if (!isFirstClick)
            calculation.StartCalculation();
    }

    void SetMortarMarker(RaycastHit hit, string s)
    {
        mortarMarker.transform.position = new Vector3(hit.point.x, hit.point.y, 0.5f);
        currentMortarHeight = heightCalc.GetHeightFromTexturePoint(mortarMarker.transform.position);
        calculation.SetMortarPosition(mortarMarker.transform.position, currentMortarHeight);
        mortarRangeMarker.transform.position = mortarMarker.transform.position;

        mortarOutput.text = "Mortar @ " + s ;
        isFirstClick = false;
    }

	void SetMortarTarget(RaycastHit hit, string s)
    {
		if (target1Active) 
		{
			targetMarker.transform.position = new Vector3 (hit.point.x, hit.point.y, 0.5f);
			currentTargetHeight = heightCalc.GetHeightFromTexturePoint (targetMarker.transform.position);
			calculation.SetTargetPosition (targetMarker.transform.position, currentTargetHeight,true);
		} else 
		{
			targetMarker2.transform.position = new Vector3 (hit.point.x, hit.point.y, 0.5f);
			currentTargetHeight = heightCalc.GetHeightFromTexturePoint (targetMarker2.transform.position);
			calculation.SetTargetPosition (targetMarker2.transform.position, currentTargetHeight,false);
		}

        string newString = string.Format("{0:F1}", currentMortarHeight-currentTargetHeight);
        targetOutput.text = "Target @ " + s;
    
    }

    public void SetMortarMarkerByPosition(Vector2 vec2Pos)
    {
        mortarMarker.transform.position = new Vector3(vec2Pos.x, vec2Pos.y, 0.5f);
        mortarRangeMarker.transform.position = mortarMarker.transform.position;
    }

	public void SetMortarTargetByPosition(Vector2 vec2Pos, bool isTarget1)
    {
		if(isTarget1)
        	targetMarker.transform.position = new Vector3(vec2Pos.x, vec2Pos.y, 0.5f);
		else
			targetMarker2.transform.position = new Vector3(vec2Pos.x, vec2Pos.y, 0.5f);
    }




    void RescaleMap(float deltaMouseWheel)
    {
        if (deltaMouseWheel < 0f)
            cameraSize++;
        else
            cameraSize--;

        cameraSize = Mathf.Clamp(cameraSize, cameraSizeMin, cameraSizeMax);
        camera.orthographicSize = cameraSize;

        DragMap();

        if (cameraSize < 8)
            SUBKeyParent.SetActive(true);
        else
            SUBKeyParent.SetActive(false);

        if (cameraSize < 17)
            KeyPadParent.SetActive(true);
        else
            KeyPadParent.SetActive(false);

        mortarMarker.transform.localScale = new Vector3(0.25f * cameraSize, 0.25f * cameraSize, 1f);
		targetMarker.transform.localScale = new Vector3(0.25f * cameraSize, 0.25f * cameraSize, 1f);
		targetMarker2.transform.localScale = new Vector3(0.25f * cameraSize, 0.25f * cameraSize, 1f);
    }

    void DragMap()
    {
        //camera x boundaries = 0,40,96
        //camera y boundaries = -scale, -mapsize*0.01 + scale

        newCameraPos.x -= Input.GetAxis("Mouse X") * dragMapFactor * cameraSize;


        newCameraPos.y -= Input.GetAxis("Mouse Y") * dragMapFactor * cameraSize;

        newCameraPos.z = -1;

        newCameraPos.x = Mathf.Clamp(newCameraPos.x, 0f, mapSize * 0.01f);
        newCameraPos.y = Mathf.Clamp(newCameraPos.y, (-mapSize * 0.01f) + cameraSize, (-cameraSize));

        camera.transform.position = newCameraPos;

    }

    void DragMapWithKeys()
    {
        //camera x boundaries = 0,40,96
        //camera y boundaries = -scale, -mapsize*0.01 + scale

        newCameraPos.x -= Input.GetAxis("Horizontal") * dragMapFactor* cameraSize;


        newCameraPos.y -= Input.GetAxis("Vertical") * dragMapFactor* cameraSize;

        newCameraPos.z = -1;

        newCameraPos.x = Mathf.Clamp(newCameraPos.x, 0f, mapSize* 0.01f);
        newCameraPos.y = Mathf.Clamp(newCameraPos.y, (-mapSize* 0.01f) +cameraSize, (-cameraSize));

        camera.transform.position = newCameraPos;
    }




private void CreateGrid()
    {
        float mapScaleFactor = mapSize / currentMapScale;
        CreateBigLineV(mapScaleFactor);
        CreateBigLineH(mapScaleFactor);
        CreateKeypadLineV(mapScaleFactor);
        CreateKeypadLineH(mapScaleFactor);
        CreateGridCoordinates(mapScaleFactor);


        CreateSUBKeypadLineV(mapScaleFactor);
        CreateSUBKeypadLineH(mapScaleFactor);

    }

    void CreateBigLineV(float mapScaleFactor)
    {
        //for every grid make new line
        for (int i = 1; i < 27; i++)
        {
            //if distance greater than map stop
            if (mapScaleFactor * 300 * i * 0.01f >= mapSize * 0.01f)
                break;

            GameObject newLine = Instantiate(bigLineV) as GameObject;
            newLine.transform.parent = GridParent.transform;
            // set line to (scalefactor * ingameScale(big grid 300m) * own interation * scale down for unity (/100)
            newLine.transform.position = new Vector3(mapScaleFactor * 300 * i * 0.01f, newLine.transform.position.y, 0);
        }
    }

    void CreateBigLineH(float mapScaleFactor)
    {
        for (int i = 1; i < 27; i++)
        {
            if (-mapScaleFactor * 300 * i * 0.01f <= -mapSize * 0.01f)
                break;
            GameObject newLine = Instantiate(bigLineH) as GameObject;
            newLine.transform.parent = GridParent.transform;
            newLine.transform.position = new Vector3(newLine.transform.position.x, -mapScaleFactor * 300 * i * 0.01f, 0);
        }
    }

    void CreateGridCoordinates(float mapScaleFactor)
    {


        for (int i = 1; i < 27; i++)
        {

            //for every grid make new line
            for (int j = 1; j < 27; j++)
            {
                gridCoordTemplate = Instantiate(gridCoordTemplate) as GameObject;
                gridCoordTemplate.transform.position = new Vector3(mapScaleFactor * 300 * i * 0.01f - (mapScaleFactor * 100 * 2 * 0.01f), -mapScaleFactor * 300 * (j-1) * 0.01f + (-mapScaleFactor * 100 * 1 * 0.01f), 0);
                gridCoordTemplate.GetComponent<TextMesh>().text = calculation.letters[i-1] + j.ToString();
                gridCoordTemplate.name = calculation.letters[i - 1] + j.ToString();
                gridCoordTemplate.transform.parent = GridCoordParent.transform;


                gridCoordTemplate.GetComponent<TextMesh>().fontSize = (int) ((scaleBasrah / currentMapScale) * 120);
            


                //if distance greater than map stop
                if (mapScaleFactor * 300 * (j+1) * 0.01f >= mapSize * 0.01f )
                    break;
                }

            if (-mapScaleFactor * 300 * (i+1) * 0.01f  <= -mapSize * 0.01f)
                break;
        }
    }

    void CreateKeypadLineV(float mapScaleFactor)
    {

        for (int i = 1; i < 77; i++)
        {
            //if laying on bigger grid, skip
            if (i % 3 == 0)
                continue;

            if (mapScaleFactor * 100 * i * 0.01f >= mapSize * 0.01f)
                break;
            GameObject newLine = Instantiate(keyPadLineV) as GameObject;
            newLine.transform.parent = KeyPadParent.transform;
            newLine.transform.position = new Vector3(mapScaleFactor * 100 * i * 0.01f, newLine.transform.position.y, 0);
        }
    }

    void CreateKeypadLineH(float mapScaleFactor)
    {
        for (int i = 1; i < 77; i++)
        {
            if (i % 3 == 0)
                continue;


            if (-mapScaleFactor * 100 * i * 0.01f <= -mapSize * 0.01f)
                break;
            GameObject newLine = Instantiate(keyPadLineH) as GameObject;
            newLine.transform.parent = KeyPadParent.transform;
            newLine.transform.position = new Vector3(newLine.transform.position.x, -mapScaleFactor * 100 * i * 0.01f, 0);
        }
    }

    void CreateSUBKeypadLineV(float mapScaleFactor)
    {
        for (int i = 1; i < 235; i++)
        {
            if (i % 3 == 0)
                continue;

            if (mapScaleFactor * 33.33f * i * 0.01f >= mapSize * 0.01f)
                break;
            GameObject newLine = Instantiate(SUBkeyPadLineV) as GameObject;
            newLine.transform.parent = SUBKeyParent.transform;
            newLine.transform.position = new Vector3(mapScaleFactor * 33.33f * i * 0.01f, newLine.transform.position.y, 0);
        }
    }

    void CreateSUBKeypadLineH(float mapScaleFactor)
    {
        for (int i = 1; i < 235; i++)
        {
            if (i % 3 == 0)
                continue;

            if (-mapScaleFactor * 33.33f * i * 0.01f <= -mapSize * 0.01f)
                break;
            GameObject newLine = Instantiate(SUBkeyPadLineH) as GameObject;
            newLine.transform.parent = SUBKeyParent.transform;
            newLine.transform.position = new Vector3(newLine.transform.position.x, -mapScaleFactor * 33.33f * i * 0.01f, 0);
        }
    }

}

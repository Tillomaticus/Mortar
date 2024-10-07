using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Storage
{
	public float targetX;
	public float targetY;
	public float mortarX;
	public float mortarY;
	public float distance;
	public float angle;
	public int mil;

	public Storage()
	{
		//have to get rid of vector2 because its not serializable outside of unity
		targetX = 20.48f;
		targetY = -20.48f;
		mortarX = 20.48f;
		mortarY = -20.48f;
        distance =0f;

		angle=0f;
		mil = 0;
	}
}

public class TargetStore : MonoBehaviour {

	public bool inputBlocked;
	CalculationNew calc;
	//SaveLoadStorage saveLoadStorage;

	public UnityEngine.UI.Button[] buttonArray = new UnityEngine.UI.Button[3];

	SaveLoadStorage saveLoadStorage;

	int currentTargetID =0;

	List<Storage> storageList = new List<Storage>();
	Storage[] storageArray = new Storage[3];


	void Start () {
		calc = GetComponent<CalculationNew> ();

		saveLoadStorage = GetComponent<SaveLoadStorage> ();

		InitArrays ();
		ChangeCurrentID (1);
	}

	
	// Update is called once per frame
	void Update () {
		if (inputBlocked)
			return;
	/*	if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
			PressButtonViaKeyboard(1);
		if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
			PressButtonViaKeyboard(2);
		if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
			PressButtonViaKeyboard(3); */
	}

	void InitArrays()
	{
		for (int i = 0; i < storageArray.Length; i++) 
			storageArray [i] = new Storage ();


	}

	public void UpdateStorage(Vector2 mortar, Vector2 target, float distance, float angle, int mil)
	{
		storageArray [currentTargetID].targetX = target.x;
		storageArray [currentTargetID].targetY = target.y;		
		storageArray [currentTargetID].mortarX = mortar.x;
		storageArray [currentTargetID].mortarY = mortar.y;
		storageArray [currentTargetID].distance = distance;
		storageArray [currentTargetID].angle = angle;
		storageArray [currentTargetID].mil = mil;
	}

	public void ChangeCurrentID(int newID)
	{
		if (inputBlocked)
			return;
		SaveToStorage ();
		buttonArray [currentTargetID].interactable = true;
		currentTargetID = newID-1;
		buttonArray [newID - 1].interactable = false;
		LoadFromStorage ();
	}

	void SaveToStorage()
	{
		calc.UpdateTargetStore();
	}
	void LoadFromStorage()
	{
		calc.UpdateFromTargetStore (
			new Vector2(storageArray [currentTargetID].mortarX,storageArray [currentTargetID].mortarY) , 
			new Vector2(storageArray [currentTargetID].targetX, storageArray [currentTargetID].targetY),
			storageArray [currentTargetID].distance,
			storageArray [currentTargetID].angle,
			storageArray [currentTargetID].mil);
	}

	
	void PressButtonViaKeyboard(int i)
	{
		ChangeCurrentID (i);
	}

	public void SaveToFile()
	{
		if (inputBlocked)
			return;
		//block input and release in saveLoadStorage when saving is done
		inputBlocked = true;
		saveLoadStorage.Save(storageArray);
	}

	public void LoadFromFile()
	{
		if (inputBlocked)
			return;
		//block input and release when loading is done (insertFromStorage)
		inputBlocked = true;
		saveLoadStorage.Load ();

	}

	public void InsertFromStorage(Storage[] newStorageArray)
	{
		inputBlocked = false;
		if (newStorageArray == null)
			return;

		if (newStorageArray.Length > 3)
			throw new UnityException("DataArray in File to large!");
		storageArray = newStorageArray;

		LoadFromStorage (); 

	}




}

using System.Collections;
using System.Collections.Generic;
using SimpleFileBrowser;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class SaveLoadStorage : MonoBehaviour {

	public static List<Storage> savedStorage = new List<Storage>();


	TargetStore targetStore;

	void Start()
	{
		targetStore = this.GetComponent<TargetStore> ();


		FileBrowser.SetFilters( true, new FileBrowser.Filter( "Targets", ".sav" ));
		FileBrowser.SetDefaultFilter( ".sav" );
		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );
		FileBrowser.AddQuickLink( null, "MortarCalc", Application.dataPath );


		// Coroutine example
	
		//StartCoroutine( ShowLoadDialogCoroutine() );
	}

	IEnumerator ShowSaveDialogCoroutine(Storage[] storageArray)
	{
		yield return FileBrowser.WaitForSaveDialog( false, Application.dataPath,"Save As", "Save" );
		if(FileBrowser.Success)
			StartSaveProcess (FileBrowser.Result, storageArray);

		targetStore.inputBlocked = false;
	}

	public void Save(Storage[] storageArray)
	{
		StartCoroutine(ShowSaveDialogCoroutine(storageArray));
	}

	void StartSaveProcess(string filePath, Storage[] storageArray)
	{
		for (int i = 0; i < storageArray.Length; i++) {
			SaveLoadStorage.savedStorage.Add (storageArray[i]);
		}

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (filePath);
		bf.Serialize(file, SaveLoadStorage.savedStorage);
		file.Close();
	}
		

	public void Load()
	{
		ShowLoadDialog ();
	}

	void ShowLoadDialog()
	{
		Storage storageArray = null;
		FileBrowser.ShowLoadDialog ((path) => LoadingDone (path), Canceled, false, Application.dataPath, "Load", "Load"); 
	}

	void Canceled()
	{
		targetStore.inputBlocked = false;
	}

	void LoadingDone(string path)
	{
		if (path == null)
			return;
		targetStore.InsertFromStorage (LoadFromFile (path));
	}

	static Storage[] LoadFromFile(string path) {
		if(File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(path, FileMode.Open);
			SaveLoadStorage.savedStorage = (List<Storage>)bf.Deserialize(file);
			file.Close();

			Storage[] storageArray = new Storage[9];
			int i = 0;
			foreach (Storage storage in savedStorage) {
				storageArray [i] = storage;
				i++;
				if (i > 8)
					break;
			}

			return storageArray;
		}
		return null;
	}
		
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public GameObject angleText;
    public  GameObject distanceText;
    public GameObject milText;

	public GameObject angleText2;
	public  GameObject distanceText2;
	public GameObject milText2;
    public GameObject manualHeightoffsetDisplay;


	public GameObject angleonMarker;
	public GameObject milOnMarker;

	public GameObject angleonMarker2;
	public GameObject milOnMarker2;

    public enum ErrorCode {tooShort, tooLong };


    string distance = "Distance: ";
    string angle = "Angle: ";
    string mil = "MIL: ";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ThrowError(ErrorCode errorCode, bool isTarget1)
    {
        switch(errorCode)
        {
		case ErrorCode.tooShort:
			{
				if (isTarget1)
				{
					milText.GetComponent<Text> ().text = "TOO SHORT!"; //OUT OF EFFECTIVE RANGE!";
					milOnMarker.GetComponent<Text> ().text = "TOO SHORT!"; //OUT OF EFFECTIVE RANGE!";
				}
				else
				{
					milText2.GetComponent<Text> ().text = "TOO SHORT!"; // OUT OF EFFECTIVE RANGE!";
					milOnMarker2.GetComponent<Text> ().text = "TOO SHORT!"; //OUT OF EFFECTIVE RANGE!";
				}
			}
                break;

		case ErrorCode.tooLong:
			{
				if (isTarget1)
				{
					milText.GetComponent<Text> ().text = "TOO LONG!"; //OUT OF EFFECTIVE RANGE!";
					milOnMarker.GetComponent<Text> ().text = "TOO LONG!"; //OUT OF EFFECTIVE RANGE!";
				}
				else
				{
					milText2.GetComponent<Text> ().text = "TOO LONG!"; //OUT OF EFFECTIVE RANGE!";
					milOnMarker2.GetComponent<Text> ().text = "TOO LONG!"; //OUT OF EFFECTIVE RANGE!";
				}
			}
                break;

            default:
                break;
        }
    }

	public void UpdateDistanceText(int newDistance, float heightDifference, bool target1)
    {
        string newString = string.Format("{0:F1}", heightDifference);
		if(target1)
        	distanceText.GetComponent<Text>().text = "Distance: " + newDistance.ToString() + "m" + "\nHeightdiff: " + newString + "m";
		else
			distanceText2.GetComponent<Text>().text = "Distance: " + newDistance.ToString() + "m" + "\nHeightdiff: " + newString + "m";
    }

	public void UpdateAngleText(float newAngle, bool target1)
    {
        string newString = string.Format("{0:F1}", newAngle);
		if (target1) {
			angleText.GetComponent<Text> ().text = "Angle: " + newString + "°";
			angleonMarker.GetComponent<Text> ().text = newString + "°";
		}
		else
		{
			angleText2.GetComponent<Text> ().text = newString + "°";
			angleonMarker2.GetComponent<Text> ().text = newString + "°";
		//	angleText2.GetComponent<Text>().text = "Angle: " + newString + "°";
		}
    }
	public void UpdateMilText(int newMil, bool target1)
    {
		if (target1) {
			milText.GetComponent<Text> ().text = "MIL: " + newMil.ToString () + "°";
			milOnMarker.GetComponent<Text> ().text = newMil.ToString () + " mil";
		} 
		else 
		{
			milText2.GetComponent<Text> ().text = newMil.ToString () + "°";
			milOnMarker2.GetComponent<Text> ().text = newMil.ToString () + " mil";
		}
		//milText2.GetComponent<Text>().text = "MIL: " + newMil.ToString() + "°";
    }

    public void UpdateManualHeightOffset(float f)
    {
		if (f != 0f) {
			manualHeightoffsetDisplay.GetComponent<Text> ().color = Color.red;
			manualHeightoffsetDisplay.GetComponent<Outline> ().enabled = true;
		} else 
		{
			manualHeightoffsetDisplay.GetComponent<Text> ().color = Color.white;
			manualHeightoffsetDisplay.GetComponent<Outline> ().enabled = false;
		}


        manualHeightoffsetDisplay.GetComponent<Text>().text = f.ToString() + "m";
    }
}

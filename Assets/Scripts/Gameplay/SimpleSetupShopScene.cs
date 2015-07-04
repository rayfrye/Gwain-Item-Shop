using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SimpleSetupShopScene : MonoBehaviour 
{
	public GameObject Items;

	public GameData gameData;

	#region uiobjects
	public GameObject ListAllItems_Values;
	public GameObject ListAllItems_SampleValue;

	public GameObject ListAllItems_Buttons;
	public GameObject ListAllItems_SampleButton;
	#endregion uiobjects

	// Use this for initialization
	void Start () 
	{
		getComponents ();
		copyAllItemButtons ();
	}

	void getComponents()
	{
		gameData = GameObject.Find ("GameData").GetComponent<GameData> ();
		ListAllItems_Values = GameObject.Find ("ListAllItems_Values");
		ListAllItems_SampleValue = GameObject.Find ("ListAllItems_SampleValue");

		ListAllItems_Buttons = GameObject.Find ("ListAllItems_Buttons");
		ListAllItems_SampleButton = GameObject.Find ("ListAllItems_SampleButton");
	}

	void copyAllItemButtons()
	{
		for (int i = 0; i < gameData.allItems.Count; i++) 
		{
			GameObject newValue_Name = (GameObject) Instantiate (ListAllItems_SampleValue);
			newValue_Name.transform.parent = ListAllItems_Values.transform;
			newValue_Name.GetComponent<Text>().text = gameData.allItems[i].name;

			GameObject newValue_Type = (GameObject) Instantiate (ListAllItems_SampleValue);
			newValue_Type.transform.parent = ListAllItems_Values.transform;
			newValue_Type.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].itemType.name;

			GameObject newValue_Desc = (GameObject) Instantiate (ListAllItems_SampleValue);
			newValue_Desc.transform.parent = ListAllItems_Values.transform;
			newValue_Desc.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].desc;

			GameObject newValue_Cost = (GameObject) Instantiate (ListAllItems_SampleValue);
			newValue_Cost.transform.parent = ListAllItems_Values .transform;
			newValue_Cost.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].cost.ToString (); 

			GameObject newButton = (GameObject) Instantiate(ListAllItems_SampleButton);
			newButton.transform.parent = ListAllItems_Buttons.transform;
		}

		Destroy (ListAllItems_SampleValue);
		Destroy (ListAllItems_SampleButton);
	}

}

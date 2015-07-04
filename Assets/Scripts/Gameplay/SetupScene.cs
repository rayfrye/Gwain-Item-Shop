using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SetupScene : MonoBehaviour 
{
	public GameObject Items;

	public GameData gameData;

	#region uiobjects
	public GameObject ListAllItems;
	public GameObject AllItemsButton;
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
		ListAllItems = GameObject.Find ("ListAllItems");
		AllItemsButton = GameObject.Find ("AllItems_Name");
	}

	void copyAllItemButtons()
	{
		for (int i = 0; i < gameData.allItems.Count; i++) 
		{
			GameObject newButton_Name = (GameObject) Instantiate (AllItemsButton);
			newButton_Name.transform.parent = ListAllItems.transform;
			newButton_Name.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].name;

			GameObject newButton_Type = (GameObject) Instantiate (AllItemsButton);
			newButton_Type.transform.parent = ListAllItems.transform;
			newButton_Type.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].itemType.name;

			GameObject newButton_Desc = (GameObject) Instantiate (AllItemsButton);
			newButton_Desc.transform.parent = ListAllItems.transform;
			newButton_Desc.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].desc;

			GameObject newButton_Cost = (GameObject) Instantiate (AllItemsButton);
			newButton_Cost.transform.parent = ListAllItems.transform;
			newButton_Cost.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].cost.ToString ();
		}
	}

}

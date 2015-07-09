using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SimpleSetupShopScene : MonoBehaviour 
{
	#region commonComponents
	public SaveData saveData;
	public ReadCSV readCSV;
	#endregion commonComponents

	#region gameobjects
		public GameObject Items;
		public GameData gameData;
		public Text remainingGoldText;

		List<GameObject> storeInvGameObjects = new List<GameObject> ();
	#endregion gameobjects

	#region uiobjects
		#region ListAllItems
			public GameObject ListAllItems_Values;
			public GameObject ListAllItems_SampleValue;

			public GameObject ListAllItems_Buttons;
			public GameObject ListAllItems_SampleButton;

			public GameObject ListAllItems_ScrollContent;
			public GameObject ListAllItems_ScrollView;
			public GameObject ListAllItems_ScrollBar;
		#endregion ListAllItems

		#region ListStoreInv
			public GameObject ListStoreInv_Values;
			public GameObject ListStoreInv_SampleValue;
					
			public GameObject ListStoreInv_Buttons;
			public GameObject ListStoreInv_SampleButton;

			public GameObject ListStoreInv_ScrollContent;
			public GameObject ListStoreInv_ScrollView;
			public GameObject ListStoreInv_ScrollBar;
		#endregion ListStoreInv

		public GameObject StartGame_Button;
	#endregion uiobjects

	// Use this for initialization
	void Start () 
	{
		getComponents ();
		setupMasterItems ();
		setupStoreInv ();
		setupGold ();

	}

	void getComponents()
	{
		GameObject functions = GameObject.Find ("Functions");
		saveData = functions.GetComponent<SaveData> ();
		readCSV = functions.GetComponent<ReadCSV> ();

		gameData = GameObject.Find ("GameData").GetComponent<GameData> ();

		remainingGoldText = GameObject.Find ("RemainingGold").GetComponent<Text> ();

		ListAllItems_Values = GameObject.Find ("ListAllItems_Values");
		ListAllItems_SampleValue = GameObject.Find ("ListAllItems_SampleValue");

		ListAllItems_Buttons = GameObject.Find ("ListAllItems_Buttons");
		ListAllItems_SampleButton = GameObject.Find ("ListAllItems_SampleButton");

		ListStoreInv_Values = GameObject.Find ("ListStoreInv_Values");
		ListStoreInv_SampleValue = GameObject.Find ("ListStoreInv_SampleValue");
		
		ListStoreInv_Buttons = GameObject.Find ("ListStoreInv_Buttons");
		ListStoreInv_SampleButton = GameObject.Find ("ListStoreInv_SampleButton");

		ListAllItems_ScrollContent = GameObject.Find ("ListAllItems_ScrollContent");
		ListStoreInv_ScrollContent = GameObject.Find ("ListStoreInv_ScrollContent");

		ListAllItems_ScrollView = GameObject.Find ("ListAllItems_ScrollView");
		ListStoreInv_ScrollView = GameObject.Find ("ListStoreInv_ScrollView");

		ListAllItems_ScrollBar = GameObject.Find ("ListAllItems_ScrollBar");
		ListStoreInv_ScrollBar = GameObject.Find ("ListStoreInv_ScrollBar");

		StartGame_Button = GameObject.Find ("StartGame_Button");
		StartGame_Button.GetComponent<Button>().onClick.AddListener(delegate { startGame(); });

	}

	void setupMasterItems()
	{
		ListAllItems_SampleValue.SetActive(true);
		ListAllItems_SampleButton.SetActive(true);
		Vector2 ListAllItems_ScrollContent_Vector2 = 
			new Vector2 (
				ListAllItems_ScrollContent.GetComponent<RectTransform> ().sizeDelta.x
				, Mathf.Max (
					ListStoreInv_Buttons.GetComponent<GridLayoutGroup>().cellSize.y * gameData.allItems.Count
					,ListAllItems_ScrollView.GetComponent<RectTransform>().sizeDelta.y
				)
			);

		ListAllItems_ScrollContent.GetComponent<RectTransform> ().sizeDelta = ListAllItems_ScrollContent_Vector2;
		ListAllItems_ScrollBar.GetComponent<Scrollbar> ().value = 1;

		for (int i = 0; i < gameData.allItems.Count; i++) 
		{
			GameObject newValue_Name = (GameObject) Instantiate (ListAllItems_SampleValue);
			newValue_Name.transform.SetParent (ListAllItems_Values.transform);
			newValue_Name.GetComponent<Text>().text = gameData.allItems[i].name;

			GameObject newValue_Type = (GameObject) Instantiate (ListAllItems_SampleValue);
			newValue_Type.transform.SetParent (ListAllItems_Values.transform);
			newValue_Type.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].itemType.name;

			GameObject newValue_Desc = (GameObject) Instantiate (ListAllItems_SampleValue);
			newValue_Desc.transform.SetParent (ListAllItems_Values.transform);
			newValue_Desc.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].desc;

			GameObject newValue_Cost = (GameObject) Instantiate (ListAllItems_SampleValue);
			newValue_Cost.transform.SetParent (ListAllItems_Values .transform);
			newValue_Cost.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].cost.ToString (); 

			GameObject newButton = (GameObject) Instantiate(ListAllItems_SampleButton);
			newButton.transform.SetParent (ListAllItems_Buttons.transform);
			int param = gameData.allItems[i].id;
			newButton.GetComponent<Button>().onClick.AddListener(delegate { addStoreInvItem(param); });
		}

		ListAllItems_SampleValue.SetActive(false);
		ListAllItems_SampleButton.SetActive(false);
	}

	void setupStoreInv()
	{
		ListStoreInv_SampleValue.SetActive(true);
		ListStoreInv_SampleButton.SetActive(true);

		Vector2 ListStoreInv_ScrollContent_Vector2 = 
			new Vector2 (
				ListStoreInv_ScrollContent.GetComponent<RectTransform> ().sizeDelta.x
				, Mathf.Max (
				ListStoreInv_Buttons.GetComponent<GridLayoutGroup>().cellSize.y * gameData.player.itemCount.Count
				,ListStoreInv_ScrollView.GetComponent<RectTransform>().sizeDelta.y
				)
			);
		
		ListStoreInv_ScrollContent.GetComponent<RectTransform> ().sizeDelta = ListStoreInv_ScrollContent_Vector2;
		ListStoreInv_ScrollContent.GetComponent<RectTransform> ().position = new Vector3 (
				ListStoreInv_ScrollContent.GetComponent<RectTransform> ().position.x
				, ListStoreInv_ScrollContent.GetComponent<RectTransform> ().position.y - ListStoreInv_ScrollContent_Vector2.y
				, ListStoreInv_ScrollContent.GetComponent<RectTransform> ().position.z
		);
		ListStoreInv_ScrollBar.GetComponent<Scrollbar> ().value = 1;

		for (int k = 0; k < gameData.player.itemCount.Count; k++) 
		{
			int i = gameData.player.itemCount.ElementAt (k).Key;

			GameObject newValue_Count = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Count.transform.SetParent(ListStoreInv_Values.transform);
			newValue_Count.GetComponent<Text>().text = gameData.player.itemCount.Values.ToList ()[k].ToString ();

			GameObject newValue_Name = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Name.transform.SetParent (ListStoreInv_Values.transform);
			newValue_Name.GetComponent<Text>().text = gameData.allItems[i].name;
			
			GameObject newValue_Type = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Type.transform.SetParent (ListStoreInv_Values.transform);
			newValue_Type.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].itemType.name;
			
			GameObject newValue_Desc = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Desc.transform.SetParent (ListStoreInv_Values.transform);
			newValue_Desc.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].desc;
			
			GameObject newValue_Cost = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Cost.transform.SetParent (ListStoreInv_Values .transform);
			newValue_Cost.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].cost.ToString (); 
			
			GameObject newButton = (GameObject) Instantiate(ListStoreInv_SampleButton);
			newButton.transform.SetParent (ListStoreInv_Buttons.transform);
			int param = gameData.allItems[i].id;
			newButton.GetComponent<Button>().onClick.AddListener(delegate { removeStoreInvItem(param); });

			storeInvGameObjects.Add (newValue_Count);
			storeInvGameObjects.Add (newValue_Name);
			storeInvGameObjects.Add (newValue_Type);
			storeInvGameObjects.Add (newValue_Desc);
			storeInvGameObjects.Add (newValue_Cost);
			storeInvGameObjects.Add (newButton);
		}
		
		ListStoreInv_SampleValue.SetActive(false);
		ListStoreInv_SampleButton.SetActive(false);
	}
	
	void setupGold()
	{
		remainingGoldText.text = "Remaining Gold: " + gameData.player.gold;
	}

	void destroyStoreInvObjects()
	{
		foreach (GameObject go in storeInvGameObjects) 
		{
			Destroy (go);
		}
	}

	void addStoreInvItem(int itemID)
	{
		if (gameData.allItems [itemID].cost <= gameData.player.gold) 
		{
			destroyStoreInvObjects ();

			if (gameData.player.itemCount.ContainsKey (itemID)) 
			{
				gameData.player.itemCount [itemID]++;
				setupStoreInv ();
				gameData.player.gold -= gameData.allItems [itemID].cost;
				setupGold ();
			}
			else
			{
				gameData.player.itemCount.Add (itemID, 1);
				setupStoreInv ();
				gameData.player.gold -= gameData.allItems [itemID].cost;
				setupGold ();
			}
		}
		else
		{
			Debug.Log("Need more cash homie");
		}
	}

	void removeStoreInvItem(int itemID)
	{
		destroyStoreInvObjects();

		if (gameData.player.itemCount [itemID] == 1) 
		{
			gameData.player.gold += gameData.allItems [itemID].cost;
			gameData.player.itemCount.Remove (itemID);
			setupStoreInv ();
			setupGold ();
		}
		else
		{
			gameData.player.itemCount[itemID]--;
			gameData.player.gold += gameData.allItems [itemID].cost;
			setupStoreInv ();
			setupGold ();
		}
	}

	public void startGame()
	{
		StartCoroutine ("saveGame");

		Application.LoadLevel ("Simple Run Shop");
	}

	IEnumerator saveGame()
	{
		if (saveData.saveData (gameData.player, 1)) 
		{
			yield return null;
		}
	}
}

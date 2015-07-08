using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SimpleRunShopScene : MonoBehaviour 
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
	List<GameObject> dialogueGameObjects = new List<GameObject>();
	#endregion gameobjects
	
	#region uiobjects
	
	#region ListAllItems
	public GameObject ListStoreInv_Values;
	public GameObject ListStoreInv_SampleValue;
	
	public GameObject ListStoreInv_Buttons;
	public GameObject ListStoreInv_SampleButton;
	#endregion ListAllItems

	#region dialogueGameObjects
	public GameObject NPCDialogue_Text;
	public GameObject Dialogue_ResponsePanel;
	public GameObject PlayerResponse_SampleButton;
	#endregion dialogueGameObjects
	
	public GameObject StartGame_Button;
	public GameObject NextCustomer_Button;
	#endregion uiobjects

	int currentNPCID;
	NPC currentNPC;
	int currentNPCIDDialogueIndex;

	List<string> currentNPCDialogue = new List<string> ();
	List<string> currentNPCResponses = new List<string> ();
	Dictionary<int,List<int>> currentNPCActions = new Dictionary<int,List<int>> ();

	// Use this for initialization
	void Start () 
	{
		getComponents ();
		loadPlayerData ();
		setupStoreInv ();
		setupGold ();

		currentNPCID = 0;
		currentNPCIDDialogueIndex = 0;
	
		createNextNPC ();
	}
	
	void getComponents()
	{
		GameObject functions = GameObject.Find ("Functions");
		saveData = functions.GetComponent<SaveData> ();
		readCSV = functions.GetComponent<ReadCSV> ();

		gameData = GameObject.Find ("GameData").GetComponent<GameData> ();

		remainingGoldText = GameObject.Find ("RemainingGold").GetComponent<Text> ();
		
		ListStoreInv_Values = GameObject.Find ("ListStoreInv_Values");
		ListStoreInv_SampleValue = GameObject.Find ("ListStoreInv_SampleValue");
		
		ListStoreInv_Buttons = GameObject.Find ("ListStoreInv_Buttons");
		ListStoreInv_SampleButton = GameObject.Find ("ListStoreInv_SampleButton");
		
		StartGame_Button = GameObject.Find ("StartGame_Button");
		//StartGame_Button.GetComponent<Button>().onClick.AddListener(delegate { startGame(); });

		NextCustomer_Button = GameObject.Find ("NextCustomer_Button");
		NextCustomer_Button.GetComponent<Button>().onClick.AddListener(delegate { createNextNPC(); });

		NPCDialogue_Text = GameObject.Find ("NPCDialogue_Text");
		Dialogue_ResponsePanel = GameObject.Find ("Dialogue_ResponsePanel");
		PlayerResponse_SampleButton = GameObject.Find ("PlayerResponse_SampleButton");
	}

	void loadPlayerData()
	{
		int saveGameNum = int.Parse (readCSV.getSingleDimCSVData ("./Assets/Resources/SaveData/LastLoadedGame.csv")[0]);
		string[,] playerData = readCSV.getMultiDimCSVData ("./Assets/Resources/SaveData/SaveGame" + saveGameNum + "/player.csv");
		gameData.player.name = playerData[0, 0];
		gameData.player.gold = int.Parse (playerData[0, 1]);

		string[,] itemData = readCSV.getMultiDimCSVData ("./Assets/Resources/SaveData/SaveGame" + saveGameNum + "/storeInventory.csv");

		for (int row = 0; row < itemData.GetLength (0); row++) 
		{
			gameData.player.itemCount.Add (int.Parse (itemData[row,0]),int.Parse (itemData[row,1]));
		}
	}
	
	void setupStoreInv()
	{
		ListStoreInv_SampleValue.SetActive(true);
		ListStoreInv_SampleButton.SetActive(true);
		
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
			newButton.GetComponent<Button>().onClick.AddListener(delegate { trySellingToNPC(param); });
			
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
	
	void destroyGameObjects(List<GameObject> gos)
	{
		foreach (GameObject go in gos) 
		{
			Destroy (go);
		}
	}
	
	void addStoreInvItem(int itemID)
	{
		if (gameData.allItems [itemID].cost <= gameData.player.gold) 
		{
			destroyGameObjects(storeInvGameObjects);
			
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
		destroyGameObjects(storeInvGameObjects);
		
		if (gameData.player.itemCount [itemID] == 1) 
		{
			gameData.player.gold += gameData.allItems [itemID].cost;
			gameData.player.itemCount.Remove (itemID);
		}
		else
		{
			gameData.player.itemCount[itemID]--;
			gameData.player.gold += gameData.allItems [itemID].cost;
		}

		setupStoreInv ();
		setupGold ();
		completeNPCTransaction(gameData.allItems [itemID].cost, itemID);

		createNextNPC ();
	}

	void completeNPCTransaction(int cost, int itemID)
	{
		currentNPC.gold -= cost;

		if (currentNPC.itemCount.ContainsKey (itemID)) 
		{
			currentNPC.itemCount[itemID]++;
		}
		else 
		{
			currentNPC.itemCount.Add (itemID, 1);
		}
	}

	void trySellingToNPC(int itemID)
	{
		bool npcNeedsItem = true;

		npcNeedsItem = testItem (itemID,currentNPC.itemTypeNeed);

		if (!npcNeedsItem)
		{
			writeDialogueToScreen (1);
		}
		else 
		{
			removeStoreInvItem(itemID);
		}
	}

	bool testItem(int item1, int item2)
	{
		if (gameData.allItems[item1].itemType != gameData.allItems[item2].itemType) 
		{
			return false;
		}
		else 
		{
			return true;
		}
	}

	void debugNPCGoldAndInv()
	{
		string s = currentNPC.gold + "\n";

		for (int i = 0; i < currentNPC.itemCount.Count; i++) 
		{
			s += currentNPC.itemCount.ElementAt (i).Key + " " + currentNPC.itemCount.ElementAt (i).Value + "\n";
		}

		Debug.Log (s);
	}

	void createNextNPC()
	{
		clearNPCDialogue ();

		currentNPCID++;
		currentNPC = ScriptableObject.CreateInstance <NPC> ();

		currentNPC.id = currentNPCID;
		currentNPC.name = "Customer " + currentNPCID;
		currentNPC.gold = Random.Range (10, 100);

		currentNPC.itemTypeNeed = Random.Range (0, gameData.itemTypes.Count);
		currentNPC.questDifficultyLevel = Random.Range (0, 10);

		currentNPC.dialogueIDs.Add (4);
		List<int> responseIDs = new List<int> ();
		responseIDs.Add (5);
		responseIDs.Add (6);
		currentNPC.dialogueReponseIDs.Add (4, responseIDs);

		currentNPCDialogue.Add (currentNPC.name + ": " + "Hello, I'm looking for " + gameData.itemTypes[currentNPC.itemTypeNeed].name + ".");
		currentNPCDialogue.Add (currentNPC.name + ": " + "That's not what I'm looking for. I want " + gameData.itemTypes[currentNPC.itemTypeNeed].name + ".");

		currentNPCResponses.Add ("I don't have any of those.");
		List<int> responses1 = new List<int> ();
		responses1.Add (1);
		currentNPCActions.Add (0,responses1);

		currentNPCResponses.Add ("Oh I've got one of those.");
		List<int> responses2 = new List<int> ();
		responses2.Add (2);
		currentNPCActions.Add (1,responses2);

		writeDialogueToScreen (0);
	}

	void clearNPCDialogue()
	{
		currentNPCDialogue.Clear ();
		currentNPCResponses.Clear ();
		currentNPCActions.Clear ();
	}

	void writeDialogueToScreen(int dialogueTextID)
	{
		//debugNPCGoldAndInv ();

		destroyGameObjects (dialogueGameObjects);

		NPCDialogue_Text.GetComponent<Text>().text = currentNPCDialogue[dialogueTextID];

		PlayerResponse_SampleButton.SetActive (true);

		for (int i = 0; i < currentNPCResponses.Count; i++)
		{
			GameObject newResponse_Button = (GameObject)Instantiate (PlayerResponse_SampleButton);
			newResponse_Button.transform.SetParent (Dialogue_ResponsePanel.transform);

			newResponse_Button.transform.GetComponentInChildren<Text> ().text = currentNPCResponses[i];

			int param = i;
			newResponse_Button.GetComponent<Button>().onClick.AddListener(delegate { dialogueAction(param); });
			
			dialogueGameObjects.Add(newResponse_Button);
		}

		PlayerResponse_SampleButton.SetActive (false);
	}
	
	public void startGame()
	{
		StartCoroutine ("saveGame");
	}
	
	IEnumerator saveGame()
	{
		if (saveData.saveData (gameData.player, 1)) 
		{
			yield return null;
		}
	}

	void dialogueAction(int dialogueTextID)
	{
		List<int> dialogueIDs = currentNPCActions.ElementAt (dialogueTextID).Value;

		foreach (int dialogueID in dialogueIDs) 
		{

			switch(dialogueID)
			{
			case 0:
			{
				print (0);
				break;
			}
			case 1:
			{
				createNextNPC();
				break;
			}
			case 2:
			{
				print (2);
				break;
			}
			default:
			{
				break;
			}
			}
		}
	}
}

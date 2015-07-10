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
	public Text NPCGoldText;
	public Slider markupSlider;
	public Text markupValue;
	public int previousMarkupValue;
	public Text previousQuestOutcome;
	
	List<GameObject> storeInvGameObjects = new List<GameObject> ();
	List<GameObject> dialogueGameObjects = new List<GameObject>();
	#endregion gameobjects
	
	#region uiobjects
	
	#region ListStoreInv
	public GameObject ListStoreInv_Values;
	public GameObject ListStoreInv_SampleValue;
	
	public GameObject ListStoreInv_Buttons;
	public GameObject ListStoreInv_SampleButton;
	
	public GameObject ListStoreInv_ScrollContent;
	public GameObject ListStoreInv_ScrollView;
	public GameObject ListStoreInv_ScrollBar;
	#endregion ListStoreInv
	
	#region dialogueGameObjects
	public GameObject NPCDialogue_Text;
	public GameObject Dialogue_ResponsePanel;
	public GameObject PlayerResponse_SampleButton;
	#endregion dialogueGameObjects

	#region sprites
	GameObject Waypoint_Door;
	GameObject Waypoint_Counter;
	GameObject CustomerSprite;
	GameObject PlayerSprite;
	bool customerWalking;
	#endregion sprites

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
		ListStoreInv_ScrollBar.GetComponent<Scrollbar> ().value = 1;

		currentNPCID = 0;
		currentNPCIDDialogueIndex = 0;
	
		createNextNPC ();
		setupGold ();
	}

	void LateUpdate()
	{
		markupValue.text = "Currently selling items at " + markupSlider.value + "% of market value.";

		if (previousMarkupValue != markupSlider.value) 
		{
			setupStoreInv();
			previousMarkupValue = (int) markupSlider.value;
		}

		if (customerWalking) 
		{
			print ("walking");

			if(walkFromDoorToCounter())
			{
				customerWalking = false;
			}
		}
	}

	void getComponents()
	{
		GameObject functions = GameObject.Find ("Functions");
		saveData = functions.GetComponent<SaveData> ();
		readCSV = functions.GetComponent<ReadCSV> ();

		gameData = GameObject.Find ("GameData").GetComponent<GameData> ();

		remainingGoldText = GameObject.Find ("RemainingGold").GetComponent<Text> ();
		NPCGoldText = GameObject.Find ("NPCGold").GetComponent<Text> ();
		markupSlider = GameObject.Find ("MarkupSlider").GetComponent<Slider> ();
		markupValue = GameObject.Find ("MarkupValue").GetComponent<Text> ();
		markupSlider.value = 120;
		previousMarkupValue = 120;

		previousQuestOutcome = GameObject.Find ("PreviousQuestOutcome").transform.GetComponentInChildren<Text> ();
		
		ListStoreInv_Values = GameObject.Find ("ListStoreInv_Values");
		ListStoreInv_SampleValue = GameObject.Find ("ListStoreInv_SampleValue");
		
		ListStoreInv_Buttons = GameObject.Find ("ListStoreInv_Buttons");
		ListStoreInv_SampleButton = GameObject.Find ("ListStoreInv_SampleButton");
		
		ListStoreInv_ScrollContent = GameObject.Find ("ListStoreInv_ScrollContent");
		
		ListStoreInv_ScrollView = GameObject.Find ("ListStoreInv_ScrollView");
		
		ListStoreInv_ScrollBar = GameObject.Find ("ListStoreInv_ScrollBar");

		NPCDialogue_Text = GameObject.Find ("NPCDialogue_Text");
		Dialogue_ResponsePanel = GameObject.Find ("Dialogue_ResponsePanel");
		PlayerResponse_SampleButton = GameObject.Find ("PlayerResponse_SampleButton");

		Waypoint_Door = GameObject.Find ("Waypoint_Door");
		Waypoint_Counter = GameObject.Find ("Waypoint_Counter");
		CustomerSprite = GameObject.Find ("Customer");
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
		destroyGameObjects(storeInvGameObjects);

		Vector2 ListStoreInv_ScrollContent_Vector2 = 
			new Vector2 (
				ListStoreInv_ScrollContent.GetComponent<RectTransform> ().sizeDelta.x
				, Mathf.Max (
				ListStoreInv_Buttons.GetComponent<GridLayoutGroup>().cellSize.y * gameData.player.itemCount.Count
				,ListStoreInv_ScrollView.GetComponent<RectTransform>().sizeDelta.y
				)
			);
		
		ListStoreInv_ScrollContent.GetComponent<RectTransform> ().sizeDelta = ListStoreInv_ScrollContent_Vector2;
		/*ListStoreInv_ScrollContent.GetComponent<RectTransform> ().position = new Vector3 (
			ListStoreInv_ScrollContent.GetComponent<RectTransform> ().position.x
			, ListStoreInv_ScrollContent.GetComponent<RectTransform> ().position.y - ListStoreInv_ScrollContent_Vector2.y
			, ListStoreInv_ScrollContent.GetComponent<RectTransform> ().position.z
		);*/
		//ListStoreInv_ScrollBar.GetComponent<Scrollbar> ().value = 1;
		
		for (int k = 0; k < gameData.player.itemCount.Count; k++) 
		{
			int i = gameData.player.itemCount.ElementAt (k).Key;
			
			GameObject newValue_Count = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Count.transform.SetParent(ListStoreInv_Values.transform);
			newValue_Count.GetComponent<Text>().text = gameData.player.itemCount.Values.ToList ()[k].ToString ();
			newValue_Count.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			
			GameObject newValue_Type = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Type.transform.SetParent (ListStoreInv_Values.transform);
			newValue_Type.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].itemType.name;
			newValue_Type.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);

			GameObject newValue_Name = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Name.transform.SetParent (ListStoreInv_Values.transform);
			newValue_Name.GetComponent<Text>().text = gameData.allItems[i].name;
			newValue_Name.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			
			GameObject newValue_Desc = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Desc.transform.SetParent (ListStoreInv_Values.transform);
			newValue_Desc.transform.GetComponentInChildren<Text>().text = gameData.allItems[i].desc;
			newValue_Desc.transform.GetComponentInChildren<Text>().fontSize = 11;
			newValue_Desc.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			
			GameObject newValue_Cost = (GameObject) Instantiate (ListStoreInv_SampleValue);
			newValue_Cost.transform.SetParent (ListStoreInv_Values .transform);
			float costF = gameData.allItems[i].cost * (markupSlider.value / 100);
			int cost = (int) costF;
			newValue_Cost.transform.GetComponentInChildren<Text>().text = cost+"g"; 
			newValue_Cost.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			
			GameObject newButton = (GameObject) Instantiate(ListStoreInv_SampleButton);
			newButton.transform.SetParent (ListStoreInv_Buttons.transform);
			int param = gameData.allItems[i].id;
			newButton.GetComponent<Button>().onClick.AddListener(delegate { trySellingToNPC(param); });
			newButton.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			
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
		remainingGoldText.text = gameData.player.gold+"g";
		NPCGoldText.text = currentNPC.gold + "g";
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
		completeNPCTransaction(gameData.allItems [itemID].cost, itemID);

		//createNextNPC ();
	}

	int adjustedCost(int cost)
	{
		float costF = cost * (markupSlider.value / 100);
		 return (int) costF;
	}

	void completeNPCTransaction(int cost, int itemID)
	{
		currentNPC.gold -= adjustedCost (cost);

		if (currentNPC.itemCount.ContainsKey (itemID)) 
		{
			currentNPC.itemCount[itemID]++;
		}
		else 
		{
			currentNPC.itemCount.Add (itemID, 1);
		}

		setupGold ();
	}

	void trySellingToNPC(int itemID)
	{
		bool npcNeedsItem = true;

		npcNeedsItem = testItem (itemID,currentNPC.itemTypeNeed);
		if (npcNeedsItem)
		{
			if(currentNPC.gold > adjustedCost(gameData.allItems[itemID].cost))
			{
				removeStoreInvItem(itemID);
			}
			else
			{
				Debug.Log ("NPC out of gold.");
			}
		}
		else 
		{
			Debug.Log ("NPC already has item.");
			//writeDialogueToScreen (1);
		}
	}

	bool testItem(int item1, int item2)
	{
		//if (gameData.allItems[item1].itemType != gameData.allItems[item2].itemType) 
		if(currentNPC.itemCount.ContainsKey (item1))
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
		CustomerSprite.transform.position = Waypoint_Door.transform.position;

		clearNPCDialogue ();

		currentNPCID++;
		currentNPC = ScriptableObject.CreateInstance <NPC> ();

		currentNPC.id = currentNPCID;
		currentNPC.name = "Customer " + currentNPCID;
		currentNPC.gold = Random.Range (10, 100);
		NPCGoldText.text = currentNPC.gold+"g";

		currentNPC.itemTypeNeed = Random.Range (0, gameData.itemTypes.Count);
		currentNPC.questDifficultyLevel = Random.Range (0, 10);
		currentNPC.level = 0;

		currentNPC.dialogueIDs.Add (4);
		List<int> responseIDs = new List<int> ();
		responseIDs.Add (5);
		responseIDs.Add (6);
		currentNPC.dialogueReponseIDs.Add (4, responseIDs);

		currentNPC.currentQuest = gameData.quests[Random.Range (0, gameData.quests.Count)];

		currentNPCDialogue.Add ("Hello, I'm going on a " + currentNPC.currentQuest.difficultyDesc() + " quest.");

		currentNPCResponses.Add ("Alright, I think you're good!");
		List<int> responses2 = new List<int> ();
		responses2.Add (1);
		currentNPCActions.Add (1,responses2);

		customerWalking = true;

		writeDialogueToScreen (0);
	}

	bool walkFromDoorToCounter()
	{
		CustomerSprite.transform.position = Vector3.MoveTowards (CustomerSprite.transform.position, Waypoint_Counter.transform.position, 20*Time.deltaTime);

		if ((Waypoint_Counter.transform.position - CustomerSprite.transform.position).sqrMagnitude < 1) 
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}

	void clearNPCDialogue()
	{
		currentNPCDialogue.Clear ();
		currentNPCResponses.Clear ();
		currentNPCActions.Clear ();
	}

	bool questResult(int questBonus, int difficultyPct)
	{
		bool result = true;

		int diceRoll = Random.Range (0, 20);

		int itemBonus = 0;

		for (int i = 0; i < currentNPC.itemCount.Count; i++) 
		{
			itemBonus+= gameData.allItems[currentNPC.itemCount.ElementAt(i).Key].questBonus;
		}

		int adjustedRoll = diceRoll + itemBonus + questBonus;

		if (adjustedRoll < difficultyPct) 
		{
			result = false;
		}

		string printOutcome = "Success!";

		if (!result) {
			printOutcome = "Failure";
		}

		string s = "Quest Result: "
			+ "\n\tDice Roll: " + diceRoll 
			+ "\n\tItem Bonus: " + itemBonus
			+ "\n\tNPC Quest Bonus: " + questBonus
			+ "\n\tAdjusted Roll: " + adjustedRoll
			+ "\n\tDifficulty Pct: " + difficultyPct 
			+ "\n\tResult: " + printOutcome;

		previousQuestOutcome.text = s;

		return result;
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
			newResponse_Button.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);

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
				questResult(currentNPC.questBonus,currentNPC.currentQuest.difficultyPct());
				createNextNPC();
				break;
			}
			case 2:
			{
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameData : MonoBehaviour 
{
	#region GameData

		#region itemdata
			public Dictionary<int,Item> allItems = new Dictionary<int,Item>();
			public Dictionary<int,ItemType> itemTypes = new Dictionary<int, ItemType>();
		#endregion itemdata

		#region playerdata
			public Player player;
		#endregion playerdata

		#region dialogueData
			public Dictionary<int,string> dialogueText = new Dictionary<int, string>();
			public Dictionary<int,List<int>> dialogueActions = new Dictionary<int,List<int>> ();
		#endregion dialogueData

		#region NPCData
			public List<NPC> npcs = new List<NPC> ();
		#endregion NPCData

	#endregion GameData

	#region functionscripts
		public ReadCSV readCSV;
	#endregion functionscripts

	// Use this for initialization
	void Start () 
	{
		getComponents ();

		loadData ();
	}

	void getComponents()
	{
		GameObject functions = GameObject.Find ("Functions");

		readCSV = functions.GetComponent<ReadCSV> ();
	}

	void loadData()
	{
		loadItemTypes ();
		loadItems ();
		loadPlayer ();
		loadSceneData();


	}

	void loadSceneData()
	{
		switch (Application.loadedLevelName)
		{
		case "Simple Setup Shop":
		{
			setupSimpleShopInventoryScene ();
			break;
		}
		case "Simple Run Shop":
		{
			setupSimpleRunShopInventoryScene();
			loadDialogueData();
			loadNPCData();
			break;
		}
		default:
		{
			break;
		}
		}
	}

	void loadItemTypes()
	{
		string[,] itemTypesFromFile = readCSV.getMultiDimCSVData ("./Assets/Resources/CSV/ItemTypes.csv");
		
		for (int row = 0; row < itemTypesFromFile.GetLength (0); row++) 
		{
			ItemType newItemType = ScriptableObject.CreateInstance<ItemType>();

			newItemType.id = int.Parse (itemTypesFromFile[row,0]);
			newItemType.name = itemTypesFromFile[row,1];
				
			itemTypes.Add(newItemType.id,newItemType);
		}
	}

	void loadItems()
	{
		string[,] allItemsFromFile = readCSV.getMultiDimCSVData ("./Assets/Resources/CSV/Items.csv");

		for (int row = 0; row < allItemsFromFile.GetLength (0); row++) 
		{
			Item newItem = ScriptableObject.CreateInstance<Item>();

			newItem.id = int.Parse (allItemsFromFile[row,0]);
			newItem.name = allItemsFromFile[row,1];
			newItem.desc = allItemsFromFile[row,2];
			newItem.cost = int.Parse (allItemsFromFile[row,3]);
			newItem.itemType = itemTypes[int.Parse (allItemsFromFile[row,4])];

			allItems.Add(newItem.id,newItem);
		}
	}

	void loadPlayer()
	{
		string[,] playerDataFromFile = readCSV.getMultiDimCSVData ("./Assets/Resources/CSV/Player.csv");
		player = ScriptableObject.CreateInstance<Player>();

		player.name = playerDataFromFile [0, 0];
		player.gold = int.Parse (playerDataFromFile [0, 1]);
	}

	void loadNPCData()
	{
		string[,] npcDataFromFile = readCSV.getMultiDimCSVData ("./Assets/Resources/CSV/NPCs.csv");
		string[,] npcDialogueDataFromFile = readCSV.getMultiDimCSVData ("./Assets/Resources/CSV/NPCDialogue.csv");
		string[,] npcDialogueResponseDataFromFile = readCSV.getMultiDimCSVData ("./Assets/Resources/CSV/NPCDialogueResponses.csv");

		for (int row = 0; row < npcDataFromFile.GetLength (0); row++) 
		{
			NPC newNPC = ScriptableObject.CreateInstance<NPC>();
			
			newNPC.id = int.Parse (npcDataFromFile[row,0]);
			newNPC.name = npcDataFromFile[row,1];
			newNPC.gold = int.Parse (npcDataFromFile[row,2]);

			for(int i = 0; i < 7; i++)
			{
				newNPC.itemCount.Add (i,1);
			}

			for(int i = 0; i < npcDialogueDataFromFile.GetLength (0); i++)
			{
				if(npcDialogueDataFromFile[i,0] == newNPC.id.ToString ())
				{
					newNPC.dialogueIDs.Add (int.Parse (npcDialogueDataFromFile[i,1]));

					List<int> dialogueResponseIDs = new List<int>();

					for(int l = 0; l < npcDialogueResponseDataFromFile.GetLength (0); l++)
					{
						if(npcDialogueResponseDataFromFile[l,0] == npcDialogueDataFromFile[i,1])
						{
							dialogueResponseIDs.Add (int.Parse (npcDialogueResponseDataFromFile[l,1]));
						}
					}
					
					newNPC.dialogueReponseIDs.Add (int.Parse (npcDialogueDataFromFile[i,1]),dialogueResponseIDs);

				}
			}
			
			npcs.Add(newNPC);
		}
	}

	void loadDialogueData()
	{
		string[,] dialogueDataFromFile = readCSV.getMultiDimCSVData ("./Assets/Resources/CSV/Dialogue.csv");
		string[,] dialogueActionsDataFromFile = readCSV.getMultiDimCSVData ("./Assets/Resources/CSV/dialogueActions.csv");
		
		for (int row = 0; row < dialogueDataFromFile.GetLength (0); row++)
		{
			dialogueText.Add(int.Parse (dialogueDataFromFile[row,0]),dialogueDataFromFile[row,1]);

			List<int> dialogueActionIDs = new List<int>();
			
			for(int l = 0; l < dialogueActionsDataFromFile.GetLength (0); l++)
			{
				if(dialogueActionsDataFromFile[l,0] == dialogueDataFromFile[row,0])
				{
					dialogueActionIDs.Add (int.Parse (dialogueActionsDataFromFile[l,1]));
				}
			}

			dialogueActions.Add (int.Parse (dialogueDataFromFile[row,0]),dialogueActionIDs);
		}
	}
	
	void setupSimpleShopInventoryScene()
	{
		gameObject.AddComponent<SimpleSetupShopScene> ();
	}

	void setupSimpleRunShopInventoryScene()
	{
		gameObject.AddComponent<SimpleRunShopScene> ();
	}
}

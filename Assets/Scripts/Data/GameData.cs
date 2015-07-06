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
		setupSimpleShopInventoryScene ();
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

		player.gold = int.Parse (playerDataFromFile [0, 1]);
		player.name = playerDataFromFile [0, 0];

	}

	void setupSimpleShopInventoryScene()
	{
		gameObject.AddComponent<SimpleSetupShopScene> ();
	}
}

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SaveData : MonoBehaviour 
{
	public bool saveData(Player player, int saveGameNum)
	{
		string[] playerSaveData = new string[2];

		playerSaveData[0] = "name,gold";

		playerSaveData[1] = player.name + "," + player.gold.ToString ();

		File.WriteAllLines("./Assets/Resources/SaveData/SaveGame"+saveGameNum+"/player.csv",playerSaveData);

		string[] playerItemData = new string[player.itemCount.Count+1];
		playerItemData [0] = "itemID,itemCount";

		for (int i = 0; i < player.itemCount.Count; i++) 
		{
			playerItemData[i+1] = player.itemCount.ElementAt (i).Key + "," + player.itemCount.ElementAt (i).Value;
		}

		File.WriteAllLines("./Assets/Resources/SaveData/SaveGame"+saveGameNum+"/storeInventory.csv",playerItemData);

		File.WriteAllText("./Assets/Resources/SaveData/LastLoadedGame.csv","last loaded level\n" + saveGameNum.ToString ());

		return true;
	}	
}

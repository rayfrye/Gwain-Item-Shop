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
		string playerSaveData = "";

		playerSaveData = player.name + "," + player.gold.ToString ();

		File.WriteAllText("./Assets/Resources/SaveData/SaveGame"+saveGameNum+"/player.csv",playerSaveData);

		string[] playerItemData = new string[player.itemCount.Count];

		for (int i = 0; i < player.itemCount.Count; i++) 
		{
			playerItemData[i] = player.itemCount.ElementAt (i).Key + "," + player.itemCount.ElementAt (i).Value;
		}

		File.WriteAllLines("./Assets/Resources/SaveData/SaveGame"+saveGameNum+"/storeInventory.csv",playerItemData);

		return true;
	}	
}

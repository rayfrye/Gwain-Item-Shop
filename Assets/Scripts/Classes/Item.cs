using UnityEngine;
using System.Collections;

public class Item : ScriptableObject 
{

	public int id;
	public string name;
	public string desc;
	
	public ItemType itemType;

	public int cost;

	public int questBonus;

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : ScriptableObject 
{
	public int id;
	public string name;
	public int gold;

	public int itemTypeNeed;
	public int questDifficultyLevel;

	public Dictionary<int,int> itemCount = new Dictionary<int, int>();
	public List<int> dialogueIDs = new List<int>();
	public Dictionary<int,List<int>> dialogueReponseIDs = new Dictionary<int, List<int>>();
}

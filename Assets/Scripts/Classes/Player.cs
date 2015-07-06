using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : ScriptableObject 
{
	public string name;
	public int gold;

	public Dictionary<int,int> itemCount = new Dictionary<int, int>();

}

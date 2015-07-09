using UnityEngine;
using System.Collections;

public class Quest: ScriptableObject 
{
	public int id;
	public string name;

	public int difficultyLevel;

	public string difficultyDesc()
	{
		switch (difficultyLevel) 
		{
		case 0:
		{
			return "Easy";
			break;
		}
		case 1:
		{
			return "Pretty Easy";
			break;
		}
		case 2:
		{
			return "Normal";
			break;
		}
		case 3:
		{
			return "Pretty Hard";
			break;
		}
		case 4:
		{
			return "Very Difficult";
			break;
		}
		default:
		{
			return "ehhhhh";
			break;
		}
		}
	}

	public int difficultyPct()
	{
		switch (difficultyLevel) 
		{
		case 0:
		{
			return 10;
			break;
		}
		case 1:
		{
			return 30;
			break;
		}
		case 2:
		{
			return 50;
			break;
		}
		case 3:
		{
			return 70;
			break;
		}
		case 4:
		{
			return 90;
			break;
		}
		default:
		{
			return 1;
			break;
		}
		}
	}
}

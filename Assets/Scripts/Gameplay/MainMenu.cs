using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour 
{
	public void StartGame()
	{
		Application.LoadLevel ("Shop");
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestTableBuilder : MonoBehaviour 
{

	Canvas canvas;
	public Font font;

	// Use this for initialization
	void Start () 
	{
		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();

		Table tbl = ScriptableObject.CreateInstance <Table> ();
		tbl.headers.Add ("Column 1");
		tbl.headers.Add ("Column 2");

		List<string> row1Values = new List<string>();
		row1Values.Add ("A");
		row1Values.Add ("1");
		tbl.values.Add (row1Values);

		List<string> row2Values = new List<string>();
		row2Values.Add ("B");
		row2Values.Add ("2");
		tbl.values.Add (row2Values);

		tbl.pos = new Vector2 (0, 0);
		tbl.size = new Vector2 (500,400); 
		tbl.rowHeight = 50;

		tbl.font = font;
		tbl.fontRowColor = Color.black;
		tbl.fontHeaderColor = Color.black;

		tbl.createTable ();
	}

	public void doAction(string action)
	{
		Debug.Log ("did thing" + action);
	}

}

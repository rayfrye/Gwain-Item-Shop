using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Table : ScriptableObject
{
	public List<string> headers = new List<string>();
	public List<List<string>> values = new List<List<string>>();

	public Vector2 pos = new Vector2();
	public Vector2 size = new Vector2();
	public int rowHeight;

	public Font font;

	public Color32 fontRowColor;
	public Color32 fontHeaderColor;

	public void createTable()
	{
		if (headers.Count > 0 && values.Count > 0) 
		{
			GameObject mainPanel_go = tblPanel();

			GameObject tblScrollContent_go = tblScrollContent(tblScrollView(mainPanel_go));

			GameObject tblHeaderContainer_go = tblHeaderContainer(mainPanel_go,headers.Count);

			for(int col = 0; col < headers.Count; col++)
			{
				GameObject tblHeaders_go = tblRowColumns(tblHeaderContainer_go,headers[col]);
			}

			GameObject tblValueContainer_go = tblValueContainer(tblScrollContent_go);

			for (int row = 0; row < values.Count; row ++)
			{
				GameObject tblRows_go = tblRows(tblValueContainer_go, headers.Count);

				for(int col = 0; col < headers.Count; col++)
				{
					GameObject tblRowColumns_go = tblRowColumns(tblRows_go,values[row][col]);
				}
			}
		}
		else
		{
			Debug.Log ("Table has not been configured correctly");
		}
	}

	GameObject tblPanel()
	{
		GameObject t = new GameObject();
		t.name = "tblPanel";
		t.transform.SetParent (GameObject.Find ("Canvas").transform);
		RectTransform rectTransform = t.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2 (0, 1);
		rectTransform.anchorMax = new Vector2 (0, 1);
		rectTransform.sizeDelta = size;
		rectTransform.localPosition = pos;

		return t;
	}

	GameObject tblScrollView(GameObject parent)
	{
		GameObject t = new GameObject();
		t.name = "tblScrollView";
		t.transform.SetParent (parent.transform);
		RectTransform rectTransform = t.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2 (0, 1);
		rectTransform.anchorMax = new Vector2 (0, 1);
		Vector2 localSize = new Vector2 (size.x, size.y - rowHeight);
		rectTransform.sizeDelta = localSize;
		Vector2 localPos = new Vector2 (pos.x, pos.y - rowHeight/2);
		rectTransform.localPosition = localPos;

		ScrollRect scrollRect = t.AddComponent<ScrollRect> ();

		t.AddComponent<Mask> ();

		t.AddComponent<Image> ();

		return t;
	}

	GameObject tblScrollContent(GameObject parent)
	{
		GameObject t = new GameObject();
		t.name = "tblContentView";
		t.transform.SetParent (parent.transform);

		RectTransform rectTransform = t.AddComponent<RectTransform>();
		parent.GetComponent<ScrollRect> ().content = rectTransform;
		rectTransform.anchorMin = new Vector2 (0, 1);
		rectTransform.anchorMax = new Vector2 (0, 1);

		float height = Mathf.Max (size.y - rowHeight, values.Count * rowHeight);
		Vector2 localSize = new Vector2 (size.x, height);
		rectTransform.sizeDelta = localSize;
		Vector2 localPos = new Vector2 (pos.x, pos.y - height/2);
		rectTransform.localPosition = localPos;
		
		return t;
	}

	GameObject tblHeaderContainer(GameObject parent, int colCount)
	{
		GameObject t = new GameObject();
		t.name = "tblHeaders";
		t.transform.SetParent (parent.transform);
		RectTransform rectTransform = t.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2 (0, 1);
		rectTransform.anchorMax = new Vector2 (0, 1);

		rectTransform.sizeDelta = size;
		rectTransform.localPosition = pos;

		GridLayoutGroup gridLayoutGroup = t.AddComponent<GridLayoutGroup> ();
		
		gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		gridLayoutGroup.constraintCount = headers.Count;
		
		gridLayoutGroup.cellSize = new Vector2 (size.x/colCount, rowHeight);
		
		return t;
	}

	GameObject tblValueContainer (GameObject parent)
	{
		GameObject t = new GameObject();
		t.name = "tblValues";
		t.transform.SetParent (parent.transform);
		RectTransform rectTransform = t.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2 (0, 1);
		rectTransform.anchorMax = new Vector2 (0, 1);

		float height = Mathf.Max (size.y - rowHeight, values.Count * rowHeight);
		Vector2 localSize = new Vector2 (size.x, height);
		rectTransform.sizeDelta = localSize;
		Vector2 localPos = new Vector2 (pos.x, pos.y);
		rectTransform.localPosition = localPos;
		
		GridLayoutGroup gridLayoutGroup = t.AddComponent<GridLayoutGroup> ();

		gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		gridLayoutGroup.constraintCount = 1;

		gridLayoutGroup.cellSize = new Vector2 (size.x, rowHeight);

		return t;
	}

	GameObject tblRows (GameObject parent, int colCount)
	{
		GameObject t = new GameObject();
		t.name = "tblRow";
		t.transform.SetParent (parent.transform);
		RectTransform rectTransform = t.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2 (0, 1);
		rectTransform.anchorMax = new Vector2 (0, 1);
		
		GridLayoutGroup gridLayoutGroup = t.AddComponent<GridLayoutGroup> ();
		
		gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		gridLayoutGroup.constraintCount = headers.Count;

		gridLayoutGroup.cellSize = new Vector2 (size.x/colCount, rowHeight);

		t.AddComponent<Image> ();
		
		Button btn = t.AddComponent<Button> ();
		TestTableBuilder testTableBuilder = GameObject.Find ("Script").GetComponent<TestTableBuilder> ();
		btn.GetComponent<Button>().onClick.AddListener(delegate { testTableBuilder.doAction("good"); });
		
		return t;
	}

	GameObject tblRowColumns(GameObject parent, string rowValue)
	{
		GameObject t = new GameObject();
		t.name = "tblCol";
		t.transform.SetParent (parent.transform);
		RectTransform rectTransform = t.AddComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2 (0, 1);
		rectTransform.anchorMax = new Vector2 (0, 1);
		rectTransform.localPosition = pos;
		
		Text text = t.AddComponent<Text> ();

		text.font = font;
		text.color = fontRowColor;
		text.text = rowValue;
		
		return t;
	}
}

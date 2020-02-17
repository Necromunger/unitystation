using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwitchButton))]
public class SwitchButtonEditor : Editor
{
	private SwitchButton switchButton;
	private bool isSelecting;

	void OnEnable()
	{
		SceneView.duringSceneGui += OnScene;
	}

	void OnDisable()
	{
		SceneView.duringSceneGui -= OnScene;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (!isSelecting)
		{
			if (GUILayout.Button("Begin Selecting SwitchableBehavior"))
			{
				isSelecting = true;
				switchButton = (SwitchButton)target;
			}
		}
		else
		{
			if (GUILayout.Button("Stop Selecting SwitchableBehavior"))
			{
				isSelecting = false;
				switchButton = null;
			}
		}
	}

	void OnScene(SceneView scene)
	{
		//skip if not selecting
		if (!isSelecting || switchButton == null)
			return;

		Event e = Event.current;
		if (e == null)
			return;

		if (HasPressedEscapeKey(e))
		{
			isSelecting = false;
			return;
		}

		if (HasPressedLeftClick(e))
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
			RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

			// scan all hit objects for switchable controllers
			for (int i = 0; i < hits.Length; i++)
			{
				var switchableBehavior = hits[i].transform.GetComponent<SwitchableBehavior>();
				if (switchableBehavior != null)
					ToggleSwitchableBehavior(switchButton, switchableBehavior);
			}
		}

		//return selection to switch
		Selection.activeGameObject = switchButton.gameObject;
	}

	private void ToggleSwitchableBehavior(SwitchButton switchButton, SwitchableBehavior switchableBehavior)
	{
		if (switchButton.switchableBehaviors == null)
			switchButton.switchableBehaviors = new SwitchableBehavior[0];

		if (switchButton.switchableBehaviors.Contains(switchableBehavior))
		{
			var list = switchButton.switchableBehaviors.ToList();
			list.Remove(switchableBehavior);
			switchButton.switchableBehaviors = list.ToArray();
		}
		else
		{
			var list = switchButton.switchableBehaviors.ToList();
			list.Add(switchableBehavior);
			switchButton.switchableBehaviors = list.ToArray();
		}
	}

	private bool HasPressedEscapeKey(Event e)
	{
		return e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape;
	}

	private bool HasPressedLeftClick(Event e)
	{
		return e.type == EventType.MouseDown && e.button == 0;
	}
}

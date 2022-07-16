using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(FloorManager))]
public class FloorGeneratorInspector : Editor
{
	private void DeleteRooms(FloorManager floorManager)
	{
		for (int i = floorManager.transform.childCount-1; i >= 0; i--)
		{
			var child = floorManager.transform.GetChild(i);
			if (child.GetComponent<Room>() is not null)
				DestroyImmediate(child.gameObject);
		}
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var floorManager = Selection.activeGameObject.GetComponent<FloorManager>();

		if (GUILayout.Button(new GUIContent("Generate Floor", @"For testing. "
			+ "Floor is regenerated at runtime. Please do not "
			+ "commit any changes from clicking this button.")))
		{
			DeleteRooms(floorManager);

			var floor = FloorGenerator.GenerateFloor(
				floorManager.CurrentFloor, 
				floorManager.RoomCount, 
				floorManager.FloorSize, 
				floorManager.ProportionWallsRemoved
			);

			for (int r = 0; r < floorManager.FloorSize; r++)
				for (int c = 0; c < floorManager.FloorSize; c++)
					if (floor[r][c] is not null)
					{
						var position = new Vector2(c-floorManager.FloorSize/2, -r+floorManager.FloorSize/2);
						var room = Instantiate(floor[r][c], position * Room.ROOM_SIZE, Quaternion.identity, floorManager.transform);
						room.name = $"Room [{r}, {c}]";
						Undo.RegisterCreatedObjectUndo(room, room.name);
					}
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}
		if (GUILayout.Button(new GUIContent("Delete Rooms")))
		{
			DeleteRooms(floorManager);
		}
	}
}

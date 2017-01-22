using UnityEngine;
using System.Collections;

/// <summary>
/// this class is just for demo purposes (for now)
/// </summary>
public class BuildingCreator : SingletonBehavior<BuildingCreator> {

    private Plane _dragPlane;

    protected override void Init()
    {
        base.Init();
        _dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public bool CreateBuilding(string templateID)
    {
        if (GameManager.Instance.AvailableBuilders > 0)
        {
            return TryBuild(templateID);
        }
        else
        {
            GameManager.Instance.ShowMessage("All Workers are currently busy!");
            return false;
        }
    }

	private bool TryBuild(string templateID)
	{
		//snap to nearest cell to the center of the screen
		float distance = 0f;
		Ray ray = FallbackEventReceiver.Instance.MainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		if (_dragPlane.Raycast(ray, out distance))
		{
			GridObject gridObject = GameManager.Instance.CreateBuilding(templateID);

			if (gridObject != null)
			{
                gridObject.transform.position = GridManager.Instance.GetCell(ray.GetPoint(distance)).transform.position;
                gridObject.SetCoordinates(GridManager.Instance.GetGridCoordinates(gridObject.transform.position));
                GridManager.Instance.SelectObject(gridObject);
				GridManager.Instance.VisualizePlacement(gridObject);

				return true;
			}
		}

		return false;
	}

    public bool CreateAtPosition(string templateID, Vector2Int coords)
    {
        return TryBuildAtPosition(templateID, coords);
    }

    private bool TryBuildAtPosition(string templateID, Vector2Int coords)
    {
        GridObject building = GameManager.Instance.CreateBuilding(templateID);

        if (building != null)
        {
            building.SetCoordinates(coords);
            GridManager.Instance.SelectObject(building);
            GridManager.Instance.VisualizePlacement(building);
            return true;
        }

        return false;
    }
}

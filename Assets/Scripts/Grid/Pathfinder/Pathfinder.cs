using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

namespace Pathfinding
{
	public delegate void PathfindingComplete(bool success, PathData pathData);

	public class PathData
	{
		public readonly Vector3[] waypoints;
		public readonly List<Node> nodeData;

		private PathData() {}

		public PathData(Vector3[] _waypoints, List<Node> _nodeData)
		{
			waypoints = _waypoints;
			nodeData = _nodeData;
		}
	}

	public class PathRequestData
	{
		public readonly GridCell startCell;
		public readonly GridCell endCell;
		public readonly PathfindingComplete callback;
		public readonly List<string> ignoreTemplateList = null;

		public PathRequestData(GridCell _startCell, GridCell _endCell, PathfindingComplete _callback, List<string> _ignoreTemplateList)
		{
			startCell = _startCell;
			endCell = _endCell;
			callback = _callback;
			ignoreTemplateList = _ignoreTemplateList;
		}
	}

	public class Pathfinder : SingletonBehavior<Pathfinder>
	{
		public Node[,] Nodes
		{
			get
			{
				return _nodeGrid;
			}
		}

		private Node[,] _nodeGrid;
		public static readonly Vector2Int[] Movements = new Vector2Int[] {  new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
			new Vector2Int(-1, 0), new Vector2Int(1, 0),
			new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)};

		private GridCell[,] _grid;
		private int _gridWidth;
		private int _gridHeight;

		private Queue<int> _pathRequestQueue;
		private Dictionary<int, PathRequestData> _pathRequestDictionary;
		private Job _pathfinderJob = null;

		public void Initialize()
		{
			_gridWidth = GridManager.Instance.GridSize;
			_gridHeight = GridManager.Instance.GridSize;

			_grid = GridManager.Instance.Grid;

			_nodeGrid = new Node[_gridWidth, _gridHeight];

			for(int x = 0; x < _gridWidth; ++x)
			{
				for(int y = 0; y < _gridHeight; ++y)
				{
					_nodeGrid[x, y] = new Node();
					_nodeGrid[x, y].CellObj = _grid[x, y];
				}
			}

			_pathRequestQueue = new Queue<int>();
			_pathRequestDictionary = new Dictionary<int, PathRequestData>();

			_pathfinderJob = Job.Create(FindPathRoutine());
		}

		void OnDestroy()
		{
			if(_pathfinderJob != null)
				_pathfinderJob.Kill();
		}

		public void FindPath(int requestId, Vector3 startPoint, Vector3 endPoint, PathfindingComplete callback, List<string> ignoreTemplateList = null)
		{
			GridCell startCell = GridManager.Instance.GetCell(startPoint);
			GridCell endCell = GridManager.Instance.GetCell(endPoint);

			if(startCell == null || endCell == null)
			{
				PathData pathData = new PathData(new Vector3[1] {endPoint}, new List<Node>(1) {Nodes[endCell.GridPosition.x, endCell.GridPosition.y]});
				callback(false, pathData);
				return;
			}

			PathRequestData prd = new PathRequestData(startCell, endCell, callback, ignoreTemplateList);

			if(_pathRequestDictionary.ContainsKey(requestId))
				_pathRequestDictionary[requestId] = prd;
			else
			{
				_pathRequestQueue.Enqueue(requestId);
				_pathRequestDictionary.Add(requestId, prd);
			}
		}

		private IEnumerator FindPathRoutine()
		{
			while(true)
			{
				if(_pathRequestDictionary.Count > 0)
				{
					int currentRequest = _pathRequestQueue.Dequeue();
					FindPathInternal(currentRequest);
					yield return null;
				}
				else
				{
					yield return new WaitForSeconds(0.2f);
				}
			}
		}

		private void FindPathInternal(int requestId)
		{
			bool pathFound = false;

			PathRequestData prd = _pathRequestDictionary[requestId];
			PathData pathData = null;

			Node startNode = Nodes[prd.startCell.GridPosition.x, prd.startCell.GridPosition.y];
			Node endNode = Nodes[prd.endCell.GridPosition.x, prd.endCell.GridPosition.y];

			Heap<Node> openSet = new Heap<Node>(_gridWidth * _gridHeight);
			HashSet<Node> closeSet = new HashSet<Node>();

			openSet.Add(startNode);

			while(openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
				closeSet.Add(currentNode);

				if(currentNode == endNode)
				{
					var retracedPath = RetracePath(startNode, endNode);
					Vector3[] waypoints = PathToVector3Array(retracedPath);

					if(waypoints == null)
					{
						retracedPath.Add(endNode);
						waypoints = new Vector3[1] {endNode.CellObj.transform.position};
					}

					pathData = new PathData(waypoints, retracedPath);
					prd.callback(true, pathData);
					pathFound = true;
					break;
				}


				foreach(Node neighbour in GetNeighbours(currentNode))
				{
					if(closeSet.Contains(neighbour))
						continue;

					int hCostToNeighbor = neighbour.CellObj.Cost;

					if(prd.ignoreTemplateList != null && neighbour.CellObj.IsOccupied)
					{
						if(prd.ignoreTemplateList.Count(template => template.Equals(neighbour.CellObj.Occupant.TemplateID)) > 0)
							hCostToNeighbor = 1;
					}

					int movementCostToNeighbour = currentNode.GCost + hCostToNeighbor;

					if(movementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
					{
						neighbour.GCost = movementCostToNeighbour;
						neighbour.HCost = GetDistance(neighbour, endNode);

						neighbour.Parent = currentNode;

						if(!openSet.Contains(neighbour))
							openSet.Add(neighbour);
						else
							openSet.UpdateItem(neighbour);
					}
				}
			}

			if(!pathFound)
			{
				pathData = new PathData(new Vector3[1] {endNode.CellObj.transform.position}, new List<Node>(1) {endNode});
				prd.callback(false, pathData);
			}

			_pathRequestDictionary.Remove(requestId);
		}

		private List<Node> GetNeighbours(Node node)
		{
			List<Node> neighbours = new List<Node>(8);

			foreach(Vector2Int moveVector in Movements)
			{
				int xIndex = node.CellObj.GridPosition.x + moveVector.x;
				int yIndex = node.CellObj.GridPosition.y + moveVector.y;

				bool outOfBounds = (xIndex < 0) || (yIndex < 0) || (xIndex >= _gridWidth) || (yIndex >= _gridHeight);

				if(!outOfBounds)
					neighbours.Add(_nodeGrid[xIndex, yIndex]);
			}

			return neighbours;
		}

		private List<Node> RetracePath(Node startNode, Node endNode)
		{
			List<Node> path = new List<Node>();
			Node currentNode = endNode;

			while(currentNode != startNode)
			{
				path.Add(currentNode);
				currentNode = currentNode.Parent;
			}

			path.Reverse();
			return path;
		}

		private Vector3[] PathToVector3Array(List<Node> path)
		{
			if(path.Count == 0)
				return null;

			Vector3[] waypoints = new Vector3[path.Count];

			for(int i = 0; i < path.Count; ++i)
			{
				// this can happen if a CellObj is destroyed while running the pathfinder (i.e. changing scene or anything that destroys the grid)
				if(path[i].CellObj == null)
					break;

				waypoints[i] = path[i].CellObj.transform.position;
			}

			return waypoints;
		}

		private int GetDistance(Node nodeA, Node nodeB)
		{
			int distance = Mathf.Abs(nodeA.CellObj.GridPosition.x - nodeB.CellObj.GridPosition.x) + Mathf.Abs(nodeA.CellObj.GridPosition.y - nodeB.CellObj.GridPosition.y);
			return distance;
		}
	}
}

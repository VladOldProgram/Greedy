using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;
using System.Drawing;

namespace Greedy
{
	public class DijkstraData
	{
		public Point? Previous { get; set; }
		public int Price { get; set; }
	}

	public class DijkstraPathFinder
	{
		public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start, 
			IEnumerable<Point> targets)
		{
			HashSet<Point> chests = new HashSet<Point>(targets);
			HashSet<Point> pointsToOpen = new HashSet<Point>();
			HashSet<Point> visitedPoints = new HashSet<Point>();
			var path = new Dictionary<Point, DijkstraData>();
			path[start] = new DijkstraData { Price = 0, Previous = null };
			pointsToOpen.Add(start);

			while (true)
			{
				Point? pointToOpen = null;
				int minPrice = int.MaxValue;
				foreach (var point in pointsToOpen)
				{
					if (path[point].Price < minPrice)
					{
						minPrice = path[point].Price;
						pointToOpen = point;
					}
				}

				if (pointToOpen == null) yield break;
				if (chests.Contains(pointToOpen.Value)) yield return GetPathFromPoints(path, pointToOpen.Value);

				var incidentNodes = GetIncidentNodes(pointToOpen.Value, state);
				foreach (var incidentNode in incidentNodes)
				{
					var currentCost = path[pointToOpen.Value].Price + state.CellCost[incidentNode.X, incidentNode.Y];
					if (!path.ContainsKey(incidentNode) || path[incidentNode].Price > currentCost)
						path[incidentNode] = new DijkstraData { Previous = pointToOpen, Price = currentCost };
					if (!visitedPoints.Contains(incidentNode))
						pointsToOpen.Add(incidentNode);
				}

				pointsToOpen.Remove(pointToOpen.Value);
				visitedPoints.Add(pointToOpen.Value);
			}
		}

		public PathWithCost GetPathFromPoints(Dictionary<Point, DijkstraData> points, Point end)
		{
			var path = new List<Point>();
			Point? currentPoint = end;

			while (currentPoint != null)
			{
				path.Add(currentPoint.Value);
				currentPoint = points[currentPoint.Value].Previous;
			}
			path.Reverse();
			PathWithCost resultPath = new PathWithCost(points[end].Price, path.ToArray());

			return resultPath;
		}

		public IEnumerable<Point> GetIncidentNodes(Point node, State state)
		{
			return new Point[]
			{
				new Point(node.X, node.Y + 1),
				new Point(node.X + 1, node.Y),
				new Point(node.X, node.Y - 1),
				new Point(node.X - 1, node.Y)
			}.Where(point => state.InsideMap(point) && !state.IsWallAt(point));
		}
	}
}
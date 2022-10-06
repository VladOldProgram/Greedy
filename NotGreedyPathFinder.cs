using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;

namespace Greedy
{
    public class NotGreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            Dictionary<Point, Dictionary<Point, PathWithCost>> allPaths = new Dictionary<Point, Dictionary<Point, PathWithCost>>();
            var dijkstraPathFinder = new DijkstraPathFinder();
            var startPointWithTargets = new List<Point>() { state.Position };
            foreach (var chest in state.Chests)
                startPointWithTargets.Add(chest);
            foreach (var newStartPoint in startPointWithTargets)
            {
                var targets = startPointWithTargets.Where(p => !p.Equals(newStartPoint));
                allPaths[newStartPoint] = dijkstraPathFinder.GetPathsByDijkstra(state, newStartPoint, targets)
                    .ToDictionary(key => key.End, value => value);
            }

            var allPathsCosts = new List<Tuple<int, List<Point>>>();
            GetAllPathsCosts(allPathsCosts, allPaths, 0, state.InitialEnergy, new List<Point> { state.Position });

            return allPathsCosts
                .OrderByDescending(t => t.Item1)
                .FirstOrDefault()
                .Item2.Skip(1)
                .ToList();
        }

        private void GetAllPathsCosts
            (
            List<Tuple<int, List<Point>>> allPathsCosts, 
            Dictionary<Point, Dictionary<Point, PathWithCost>> allPaths, 
            int currentChestsCount, 
            int currentEnergy, 
            List<Point> rearrangingPoints
            )
        {
            foreach (var e in allPaths[rearrangingPoints[rearrangingPoints.Count - 1]].Values
                .Where(p => !rearrangingPoints.Contains(p.End)).ToList())
            {
                if (e.Cost > currentEnergy) return;
                if (rearrangingPoints.Contains(e.End)) continue;

                var currentPath = new List<Point>(rearrangingPoints);
                currentPath.AddRange(e.Path.Skip(1).ToList());
                if (e.Cost == currentEnergy)
                {
                    allPathsCosts.Add(Tuple.Create(currentChestsCount + 1, currentPath));
                    return;
                }

                GetAllPathsCosts(allPathsCosts, allPaths, currentChestsCount + 1, currentEnergy - e.Cost, currentPath);
            }
        }
    }
}
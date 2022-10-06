using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;

namespace Greedy
{
    public class GreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            if (state.Chests.Count() == 0) return new List<Point>();
            if (state.Goal > state.Chests.Count()) return new List<Point>();

            var dijkstraPathFinder = new DijkstraPathFinder();
            var targets = new List<Point>(state.Chests);
            var currentStartPosition = state.Position;
            int currentGoal = state.Goal;
            int currentGeneralCost = 0;
            var pathToCompleteGoal = new List<Point>();

            while (currentGoal > 0)
            {
                var currentPathWithCost = dijkstraPathFinder.GetPathsByDijkstra(state, currentStartPosition, targets)
                    .FirstOrDefault();
                if (currentPathWithCost == null)
                    return new List<Point>();
                currentGeneralCost += currentPathWithCost.Cost;
                if (currentGeneralCost > state.Energy)
                    return new List<Point>();
                currentStartPosition = currentPathWithCost.End;
                targets.Remove(currentStartPosition);
                currentGoal--;
                pathToCompleteGoal.AddRange(currentPathWithCost.Path.Skip(1).ToList());
            }

            return pathToCompleteGoal;
        }
    }
}
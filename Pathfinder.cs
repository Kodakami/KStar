using System;
using System.Collections.Generic;
using System.Linq;

namespace KStar
{
    public sealed class Pathfinder<T>
        where T : notnull
    {
        private readonly T _target;
        private readonly HashSet<T> _examinedValues;
        private readonly KnownNodeList _knownUnexaminedNodes;

        private readonly INodeProvider<T> _nodeProvider;

        public IReadOnlyList<T> Path = Array.Empty<T>();
        public bool IsComplete { get; private set; }
        public bool IsCompleteAndPathExists => IsComplete && Path.Count > 0;
        public int ExaminedNodeCount { get; private set; }

        public Pathfinder(INodeProvider<T> nodeProvider, T start, T target)
        {
            _nodeProvider = nodeProvider;
            
            _target = target;

            // Set capacity to handle the total number of nodes, to avoid resizing.
            int capacity = _nodeProvider.GetNodeCount();
            _examinedValues = new HashSet<T>(capacity);
            _knownUnexaminedNodes = new KnownNodeList(capacity);

            // Add the starting node as the first known unexamined node.
            _knownUnexaminedNodes.AddIfBetterNode(NodeCostDistance<T>.Create(start, _nodeProvider.GetMinDistanceToTarget(start, target)));
        }
        public void ProcessNodes(int numberOfNodes)
        {
            if (!IsComplete)
            {
                for (int i = 0; i < numberOfNodes; i++)
                {
                    // If processing this node resulted in completion,
                    if (ProcessBestNode())
                    {
                        IsComplete = true;
                        return;
                    }
                }
            }
        }
        public void ProcessAllNodes()
        {
            while (!IsComplete)
            {
                if (ProcessBestNode())
                {
                    IsComplete = true;
                }
            }
        }
        // Returns true if path was found or no path exists.
        private bool ProcessBestNode()
        {
            // Don't call this if finished!
            
            // Get the best node to examine (the lowest cost distance). This removes it from the list of known unexamined nodes.
            var examinedNode = _knownUnexaminedNodes.TakeBestNode();

            if (examinedNode != null)
            {
                // Purely for the purpose of checking exactly how many nodes to get to the finish. Don't want to add an unnecessary node to the list.
                ExaminedNodeCount++;

                if (examinedNode.Value.Equals(_target))
                {
                    // Destination reached!

                    Path = CreatePathFromFinalNode(examinedNode);
                    return true;
                }

                // Add to list of examined nodes (already removed from unexamined nodes list).
                _examinedValues.Add(examinedNode.Value);

                // Get all adjacent nodes to this one.
                var adjacentNodes = _nodeProvider.GetAdjacentNodes(examinedNode);
                
                // For each adjacent node to this one...
                foreach (var adjacentNode in adjacentNodes)
                {
                    // If we haven't checked the node before,
                    if (!_examinedValues.Contains(adjacentNode.Value))
                    {
                        // Add the node to the list of known unexamined nodes (if it's a better route to that node).
                        _knownUnexaminedNodes.AddIfBetterNode(NodeCostDistance<T>.Create(
                            adjacentNode,
                            _nodeProvider.GetMinDistanceToTarget(
                                adjacentNode.Value,
                                _target)
                            )
                        );
                    }
                }

                return false;
            }
            
            // Otherwise, we're out of options. No path exists!
            return true;
        }
        private IReadOnlyList<T> CreatePathFromFinalNode(Node<T> finalNode)
        {
            var path = new List<Node<T>>();

            var prevNode = finalNode;
            while(prevNode != null)
            {
                path.Add(prevNode);
                prevNode = prevNode.ParentNode;
            }

            return path.Select(n => n.Value).ToArray();
        }

        // TODO: More efficient code if performance becomes a bottleneck.
        private sealed class KnownNodeList
        {
            // All known unexamined nodes.
            private readonly List<NodeCostDistance<T>> _nodes;

            public KnownNodeList(int capacity)
            {
                _nodes = new List<NodeCostDistance<T>>(capacity);
            }

            public void AddIfBetterNode(NodeCostDistance<T> nodeCostDistance)
            {
                // Since this is the only method for accessing the known node list, there can't be multiple duplicate values (only potentially one).

                // Find the index of a node with the same value. -1 if none exist.
                int index = _nodes.FindIndex(ncd => ncd.Node.Value.Equals(nodeCostDistance.Node.Value));

                // If there is a duplicate node,
                if (index >= 0)
                {
                    // If the duplicate node's cost distance is greater than our new node's,
                    if (_nodes[index].CostDistance > nodeCostDistance.CostDistance)
                    {
                        // Replace it.
                        _nodes[index] = nodeCostDistance;
                    }
                    
                    // Otherwise, the duplicate node is superior or at least equivalent, don't bother doing anything else.
                    return;
                }

                // Otherwise, this node is new to us, so add it to the list.
                _nodes.Add(nodeCostDistance);
            }
            public Node<T>? TakeBestNode()
            {
                if (_nodes.Count > 0)
                {
                    // Get the node with the least cost distance (the A* heuristic).
                    // NOTE: I read somewhere that OrderByDecending().Last() is better than OrderBy().First(). Not sure why.
                    var bestNode = _nodes.OrderByDescending(n => n.CostDistance).Last();
                    _nodes.Remove(bestNode);

                    return bestNode.Node;
                }
                return null;
            }
        }
    }

}

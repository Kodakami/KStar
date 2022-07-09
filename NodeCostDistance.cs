namespace KStar
{
    public struct NodeCostDistance<T>
        where T : notnull
    {
        public Node<T> Node;
        public float CostDistance;

        /// <summary>
        /// Create a new Node with associated cost distance (combined distance from parent and minimum distance to target).
        /// </summary>
        /// <param name="parentNode">The node which revealed this node.</param>
        /// <param name="value">The unique value this node wraps around.</param>
        /// <param name="distanceFromParent">The distance between this node and the parent node.</param>
        /// <param name="minDistanceToTargetNode">The distance between this node and the target node if no obstructions exist.</param>
        public static NodeCostDistance<T> Create(Node<T> thisNode, float minDistanceToTargetNode)
        {
            return new NodeCostDistance<T>()
            {
                Node = thisNode,
                
                // Total cost distance is the distance from the start + the minimum distance to the finish.
                CostDistance = minDistanceToTargetNode + thisNode.DistanceFromStart
            };
        }
        /// <summary>
        /// Create a new Node with associated cost distance. This overload is for the starting node in a path.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minDistanceToTargetNode"></param>
        /// <returns></returns>
        public static NodeCostDistance<T> Create(T value, float minDistanceToTargetNode)
        {
            return new NodeCostDistance<T>()
            {
                Node = new Node<T>(value),
                CostDistance = minDistanceToTargetNode
            };
        }
    }
}

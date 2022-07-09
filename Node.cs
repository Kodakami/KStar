namespace KStar
{
    /// <summary>
    /// A wrapper around any object with unique meaning (such as a coordinate pair, a tile, a city, etc...), which supplies node network connection data.
    /// </summary>
    public sealed class Node<T>
        where T : notnull
    {
        public Node(T value)
        {
            ParentNode = null;
            DistanceFromStart = 0;
            Value = value;
        }

        public Node(Node<T> parentNode, T value, float distanceFromParent)
        {
            ParentNode = parentNode;
            Value = value;

            // Total distance from the starting node is the parent's distance from the start + this node's distance to the parent.
            DistanceFromStart = distanceFromParent + parentNode.DistanceFromStart;
        }

        /// <summary>
        /// The node through which this node was discovered.
        /// </summary>
        public Node<T>? ParentNode { get; }

        /// <summary>
        /// The total distance travelled from the start node to get to this node.
        /// </summary>
        public float DistanceFromStart { get; }

        /// <summary>
        /// The unique value of this node (such as a coordinate pair, a tile, a city, etc...). This value will be passed to the NodeProvider to get adjacent nodes.
        /// </summary>
        public T Value { get; }

        // NOTE: We don't store MinDistanceToTarget here, since the information will only be used when combined with DistanceFromParent, becoming CostDistance (which is the key in the KnownNodeList).
    }
}

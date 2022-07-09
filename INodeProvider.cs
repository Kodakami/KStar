using System.Collections.Generic;

namespace KStar
{
    /// <summary>
    /// Interface for an object which can supply adjacent nodes for a given unique value.
    /// </summary>
    public interface INodeProvider<T>
        where T : notnull
    {
        /// <summary>
        /// Create new nodes for each adjacent value (such as adjacent tiles for each tile).
        /// </summary>
        IReadOnlyCollection<Node<T>> GetAdjacentNodes(Node<T> node);
        
        /// <summary>
        /// Get the distance between value and targetValue, assuming no obstacles. This should factor in movement rules like grid movement.
        /// </summary>
        float GetMinDistanceToTarget(T value, T targetValue);

        /// <summary>
        /// Return the total number of nodes.
        /// </summary>
        int GetNodeCount();
    }
}

using UnityEngine;

namespace GeneticAlgorithmForSpecies.Structures
{
    // TODO remove T

    /// <summary>
    /// CustomBehaviour extends <c>MonoBehaviour</c> in order to allow conditional updates.
    /// </summary>
    /// <typeparam name="T">The type of the condition's target</typeparam>
    public abstract class CustomBehaviour<T> : MonoBehaviour
    {
        private System.Predicate<T> Condition = (T _) => { return true; };
        private T target = default;

        /// <summary>
        /// This function initalizes the condition and the target based on which the <c>CustomUpdate()</c> will be called.
        /// </summary>
        /// <param name="Condition">Specifies the predicate to be satisfied.</param>
        /// <param name="target">Specifies a reference to the target that will be analised.</param>
        /// <typeparam name="T">The type of the target</typeparam>
        public void Init(System.Predicate<T> Condition, ref T target)
        {
            this.Condition = Condition;
            this.target = target;
        }

        private void FixedUpdate()
        {
            if (Condition(target))
            {
                CustomUpdate();
            }
        }

        /// <summary>
        /// This function is called each time the condition is true, if CustomBehaviour is enabled.
        /// </summary>
        /// <remarks>
        /// If no condition is set, by default, it will always be true.
        /// This function is synchronized with <c>FixedUpdate()</c>.
        /// </remarks>
        protected abstract void CustomUpdate();
    }
}

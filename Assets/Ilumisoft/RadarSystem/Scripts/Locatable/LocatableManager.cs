using System.Collections.Generic;
using UnityEngine.Events;

namespace Ilumisoft.RadarSystem
{
    /// <summary>
    /// Holds a list of all locatable objects and provides events for when a locatable has been added or removed
    /// </summary>
    public static class LocatableManager
    {
        // List of all locatable objects
        public static List<LocatableComponent> Locatables { get; private set; } = new List<LocatableComponent>();

        // Event that triggers when a locatable has been added
        public static UnityAction<LocatableComponent> OnLocatableAdded { get; set; }

        // Event that triggers when a locatable has been removed
        public static UnityAction<LocatableComponent> OnLocatableRemoved { get; set; }

        // Method to register a locatable
        public static void Register(LocatableComponent locatable)
        {
            // Add the locatable to the list if it's not already there
            if (!Locatables.Contains(locatable))
            {
                Locatables.Add(locatable);

                // Trigger the OnLocatableAdded event
                OnLocatableAdded?.Invoke(locatable);
            }
        }

        // Method to unregister a locatable
        public static void Unregister(LocatableComponent locatable)
        {
            // Remove the locatable from the list if it exists
            if (Locatables.Contains(locatable))
            {
                Locatables.Remove(locatable);

                // Trigger the OnLocatableRemoved event
                OnLocatableRemoved?.Invoke(locatable);
            }
        }
    }
}

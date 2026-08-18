using UnityEngine;

namespace Awakening.Interaction
{
    /// <summary>
    /// Contract interface for any interactable world entity (NPCs, Chests, Campfires, Portals, Doors).
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Prompt displayed to the player (e.g. "[F] Open Chest", "[F] Talk to Elder").
        /// </summary>
        string InteractionPrompt { get; }

        /// <summary>
        /// Determines if the entity can be interacted with right now.
        /// </summary>
        bool CanInteract(GameObject interactor);

        /// <summary>
        /// Executes the interaction logic.
        /// </summary>
        void Interact(GameObject interactor);
    }
}

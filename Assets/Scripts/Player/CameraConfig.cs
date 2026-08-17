using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Configuration data for Third-Person Orbital Camera.
    /// ScriptableObject enables live tuning of sensitivity, damping, and collision parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCameraConfig", menuName = "Awakening/Player/Camera Config")]
    public class CameraConfig : ScriptableObject
    {
        [Header("Sensitivity & Speed")]
        [Tooltip("Horizontal and vertical mouse look sensitivity")]
        public float mouseSensitivity = 1.2f;

        [Tooltip("Smooth sharpness for camera rotation")]
        public float rotationSharpness = 25.0f;

        [Tooltip("Smooth sharpness for camera position follow")]
        public float followSharpness = 15.0f;

        [Header("Distance & Framing")]
        [Tooltip("Target focus offset relative to player pivot (usually head/chest level)")]
        public Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);

        [Tooltip("Default distance from target in meters")]
        public float defaultDistance = 4.5f;

        [Tooltip("Minimum allowable zoom distance")]
        public float minDistance = 1.2f;

        [Tooltip("Maximum allowable zoom distance")]
        public float maxDistance = 7.0f;

        [Header("Pitch Clamping (Vertical Angle)")]
        [Tooltip("Minimum downward pitch angle in degrees")]
        public float minPitch = -35.0f;

        [Tooltip("Maximum upward pitch angle in degrees")]
        public float maxPitch = 70.0f;

        [Header("Collision Avoidance")]
        [Tooltip("Radius of the collision sphere to prevent wall clipping")]
        public float collisionRadius = 0.25f;

        [Tooltip("Small buffer distance from collided surface")]
        public float collisionOffset = 0.15f;

        [Tooltip("Layer mask for solid obstacles that occlude the camera")]
        public LayerMask collisionLayers = ~0; // Default to everything
    }
}

using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Presentation Layer managing the 3D Character Model, Animator Controller, and Equipment Sockets.
    /// Decouples visual representation from core physics and combat systems (CORE SYSTEMS FROZEN).
    /// </summary>
    public class PlayerVisualPresentation : MonoBehaviour
    {
        public static PlayerVisualPresentation Instance { get; private set; }

        [Header("Model Configuration")]
        [SerializeField] private RuntimeAnimatorController _animatorController;

        [Header("Equipment Sockets")]
        [SerializeField] private Transform _rightHandSocket;
        [SerializeField] private Transform _leftHandSocket;
        [SerializeField] private Transform _spineSocket;

        public Transform RightHandSocket => _rightHandSocket;
        public Transform LeftHandSocket => _leftHandSocket;
        public Transform SpineSocket => _spineSocket;
        public Animator CharacterAnimator { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetupVisualModel();
        }

        private void SetupVisualModel()
        {
            // 1. Hide root primitive capsule renderer if present
            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer != null)
            {
                rootRenderer.enabled = false;
            }

            // 2. Locate or instantiate 3D Humanoid Model
            Transform modelChild = transform.Find("CharacterModel");
            if (modelChild == null)
            {
                // Attempt to load X Bot from Resources or instantiate attached model
                GameObject modelPrefab = Resources.Load<GameObject>("Models/X Bot");
                if (modelPrefab != null)
                {
                    GameObject modelInstance = Instantiate(modelPrefab, transform);
                    modelInstance.name = "CharacterModel";
                    modelInstance.transform.localPosition = new Vector3(0, -0.9f, 0); // Align feet with bottom of CharacterController
                    modelInstance.transform.localRotation = Quaternion.identity;
                    modelChild = modelInstance.transform;
                }
            }

            if (modelChild != null)
            {
                CharacterAnimator = modelChild.GetComponent<Animator>();
                if (CharacterAnimator == null)
                {
                    CharacterAnimator = modelChild.gameObject.AddComponent<Animator>();
                }
            }
            else
            {
                CharacterAnimator = GetComponentInChildren<Animator>();
            }

            // 3. Configure Animator and apply controller
            if (CharacterAnimator != null)
            {
                CharacterAnimator.applyRootMotion = false;

                if (CharacterAnimator.runtimeAnimatorController == null && _animatorController != null)
                {
                    CharacterAnimator.runtimeAnimatorController = _animatorController;
                }
            }

            // 4. Initialize Hand & Gear Sockets
            SetupEquipmentSockets(modelChild);
        }

        private void SetupEquipmentSockets(Transform modelRoot)
        {
            if (modelRoot == null) return;

            // Search for humanoid hand bones
            if (_rightHandSocket == null)
            {
                Transform rightHandBone = FindDeepChild(modelRoot, "mixamorig:RightHand") 
                                       ?? FindDeepChild(modelRoot, "RightHand")
                                       ?? FindDeepChild(modelRoot, "Hand.R");

                if (rightHandBone != null)
                {
                    GameObject socketObj = new GameObject("Socket_RightHand");
                    socketObj.transform.SetParent(rightHandBone);
                    socketObj.transform.localPosition = Vector3.zero;
                    socketObj.transform.localRotation = Quaternion.identity;
                    _rightHandSocket = socketObj.transform;
                }
            }

            if (_leftHandSocket == null)
            {
                Transform leftHandBone = FindDeepChild(modelRoot, "mixamorig:LeftHand")
                                      ?? FindDeepChild(modelRoot, "LeftHand")
                                      ?? FindDeepChild(modelRoot, "Hand.L");

                if (leftHandBone != null)
                {
                    GameObject socketObj = new GameObject("Socket_LeftHand");
                    socketObj.transform.SetParent(leftHandBone);
                    socketObj.transform.localPosition = Vector3.zero;
                    socketObj.transform.localRotation = Quaternion.identity;
                    _leftHandSocket = socketObj.transform;
                }
            }

            // Fallback sockets directly on player root if no humanoid bone found
            if (_rightHandSocket == null)
            {
                GameObject fallbackRight = new GameObject("Socket_RightHand_Fallback");
                fallbackRight.transform.SetParent(transform, false);
                fallbackRight.transform.localPosition = new Vector3(0.38f, -0.05f, 0.35f);
                fallbackRight.transform.localRotation = Quaternion.identity;
                _rightHandSocket = fallbackRight.transform;
            }

            if (_leftHandSocket == null)
            {
                GameObject fallbackLeft = new GameObject("Socket_LeftHand_Fallback");
                fallbackLeft.transform.SetParent(transform, false);
                fallbackLeft.transform.localPosition = new Vector3(-0.38f, -0.05f, 0.35f);
                fallbackLeft.transform.localRotation = Quaternion.identity;
                _leftHandSocket = fallbackLeft.transform;
            }
        }

        private Transform FindDeepChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name.Equals(childName, System.StringComparison.OrdinalIgnoreCase))
                    return child;

                Transform result = FindDeepChild(child, childName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}

using Awakening.Equipment;
using Awakening.Items;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Equipment
{
    /// <summary>
    /// Presentation layer component that automatically instantiates and binds 3D visual weapon and shield meshes
    /// to the player's humanoid bone sockets whenever equipment changes.
    /// Supports Iron Longswords, Goblin Daggers, Hunter Bows, and Arcane Staffs.
    /// </summary>
    public class WeaponVisualManager : MonoBehaviour
    {
        public static WeaponVisualManager Instance { get; private set; }

        private EquipmentSystem _equipmentSystem;
        private PlayerVisualPresentation _visualPresentation;

        private GameObject _currentWeaponInstance;
        private GameObject _currentShieldInstance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            _equipmentSystem = GetComponent<EquipmentSystem>() ?? EquipmentSystem.Instance ?? FindFirstObjectByType<EquipmentSystem>();
            _visualPresentation = GetComponent<PlayerVisualPresentation>() ?? PlayerVisualPresentation.Instance ?? FindFirstObjectByType<PlayerVisualPresentation>();

            if (_equipmentSystem != null)
            {
                _equipmentSystem.OnEquipmentChanged += UpdateEquippedVisuals;
            }

            // Initial visual update
            UpdateEquippedVisuals();
        }

        private void OnDestroy()
        {
            if (_equipmentSystem != null)
            {
                _equipmentSystem.OnEquipmentChanged -= UpdateEquippedVisuals;
            }
        }

        public void UpdateEquippedVisuals()
        {
            if (_visualPresentation == null)
            {
                _visualPresentation = GetComponent<PlayerVisualPresentation>() ?? PlayerVisualPresentation.Instance;
            }

            // Destroy previous weapon mesh
            if (_currentWeaponInstance != null)
            {
                Destroy(_currentWeaponInstance);
                _currentWeaponInstance = null;
            }

            if (_currentShieldInstance != null)
            {
                Destroy(_currentShieldInstance);
                _currentShieldInstance = null;
            }

            if (_equipmentSystem == null || _visualPresentation == null) return;

            ItemData equippedWeapon = _equipmentSystem.EquippedWeapon;

            // If no weapon equipped, spawn default starter sword for immediate visual satisfaction
            if (equippedWeapon == null)
            {
                equippedWeapon = ItemData.CreateIronLongswordPreset();
            }

            Transform rightHand = _visualPresentation.RightHandSocket ?? transform;
            Transform leftHand = _visualPresentation.LeftHandSocket ?? transform;

            // Generate appropriate 3D Weapon Model
            if (equippedWeapon != null)
            {
                string id = equippedWeapon.itemID;
                string weaponType = equippedWeapon.weaponType;

                if (id.Contains("BOW") || weaponType == "Bow")
                {
                    _currentWeaponInstance = Create3DBowMesh(leftHand);
                }
                else if (id.Contains("STAFF") || weaponType == "Staff")
                {
                    _currentWeaponInstance = Create3DStaffMesh(rightHand);
                }
                else if (id.Contains("DAGGER") || weaponType == "Dagger")
                {
                    _currentWeaponInstance = Create3DDaggerMesh(rightHand);
                }
                else
                {
                    // Default Longsword
                    _currentWeaponInstance = Create3DLongswordMesh(rightHand);
                }
            }
        }

        #region Procedural 3D Weapon Mesh Generators
        private GameObject Create3DLongswordMesh(Transform parent)
        {
            GameObject swordRoot = new GameObject("Visual_IronLongsword");
            swordRoot.transform.SetParent(parent, false);
            swordRoot.transform.localPosition = new Vector3(0.05f, 0.05f, 0.05f);
            swordRoot.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
            swordRoot.transform.localScale = Vector3.one;

            // 1. Blade (Metallic Steel)
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Blade";
            blade.transform.SetParent(swordRoot.transform, false);
            blade.transform.localPosition = new Vector3(0f, 0.55f, 0f);
            blade.transform.localScale = new Vector3(0.04f, 0.95f, 0.12f);
            SetMaterial(blade, new Color(0.85f, 0.88f, 0.92f), 0.9f);
            DestroyCollider(blade);

            // 2. Crossguard (Gold / Brass Accent)
            GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            guard.name = "Crossguard";
            guard.transform.SetParent(swordRoot.transform, false);
            guard.transform.localPosition = new Vector3(0f, 0.06f, 0f);
            guard.transform.localScale = new Vector3(0.06f, 0.04f, 0.32f);
            SetMaterial(guard, new Color(0.85f, 0.65f, 0.2f), 0.7f);
            DestroyCollider(guard);

            // 3. Handle / Hilt (Dark Leather)
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Handle";
            handle.transform.SetParent(swordRoot.transform, false);
            handle.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            handle.transform.localScale = new Vector3(0.04f, 0.12f, 0.04f);
            SetMaterial(handle, new Color(0.25f, 0.15f, 0.1f), 0.1f);
            DestroyCollider(handle);

            // 4. Pommel (Gold Sphere)
            GameObject pommel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pommel.name = "Pommel";
            pommel.transform.SetParent(swordRoot.transform, false);
            pommel.transform.localPosition = new Vector3(0f, -0.23f, 0f);
            pommel.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            SetMaterial(pommel, new Color(0.85f, 0.65f, 0.2f), 0.8f);
            DestroyCollider(pommel);

            return swordRoot;
        }

        private GameObject Create3DDaggerMesh(Transform parent)
        {
            GameObject daggerRoot = new GameObject("Visual_GoblinDagger");
            daggerRoot.transform.SetParent(parent, false);
            daggerRoot.transform.localPosition = new Vector3(0.03f, 0.03f, 0.03f);
            daggerRoot.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);

            // Blade
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Blade";
            blade.transform.SetParent(daggerRoot.transform, false);
            blade.transform.localPosition = new Vector3(0f, 0.28f, 0f);
            blade.transform.localScale = new Vector3(0.03f, 0.45f, 0.09f);
            SetMaterial(blade, new Color(0.5f, 0.55f, 0.5f), 0.6f);
            DestroyCollider(blade);

            // Guard
            GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            guard.name = "Guard";
            guard.transform.SetParent(daggerRoot.transform, false);
            guard.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            guard.transform.localScale = new Vector3(0.05f, 0.03f, 0.18f);
            SetMaterial(guard, new Color(0.3f, 0.25f, 0.2f), 0.2f);
            DestroyCollider(guard);

            // Handle
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Handle";
            handle.transform.SetParent(daggerRoot.transform, false);
            handle.transform.localPosition = new Vector3(0f, -0.07f, 0f);
            handle.transform.localScale = new Vector3(0.035f, 0.08f, 0.035f);
            SetMaterial(handle, new Color(0.2f, 0.15f, 0.1f), 0.1f);
            DestroyCollider(handle);

            return daggerRoot;
        }

        private GameObject Create3DStaffMesh(Transform parent)
        {
            GameObject staffRoot = new GameObject("Visual_ArcaneStaff");
            staffRoot.transform.SetParent(parent, false);
            staffRoot.transform.localPosition = new Vector3(0.05f, 0.1f, 0.05f);
            staffRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            // Shaft (Elm Wood)
            GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.name = "Shaft";
            shaft.transform.SetParent(staffRoot.transform, false);
            shaft.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            shaft.transform.localScale = new Vector3(0.04f, 0.85f, 0.04f);
            SetMaterial(shaft, new Color(0.35f, 0.22f, 0.12f), 0.2f);
            DestroyCollider(shaft);

            // Glowing Arcane Orb (Crystal)
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "ArcaneOrb";
            orb.transform.SetParent(staffRoot.transform, false);
            orb.transform.localPosition = new Vector3(0f, 1.3f, 0f);
            orb.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
            SetMaterial(orb, new Color(0.2f, 0.7f, 1.0f), 0.9f);
            DestroyCollider(orb);

            return staffRoot;
        }

        private GameObject Create3DBowMesh(Transform parent)
        {
            GameObject bowRoot = new GameObject("Visual_HunterBow");
            bowRoot.transform.SetParent(parent, false);
            bowRoot.transform.localPosition = new Vector3(0.05f, 0.05f, 0.05f);
            bowRoot.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            // Central Limb
            GameObject center = GameObject.CreatePrimitive(PrimitiveType.Cube);
            center.name = "Grip";
            center.transform.SetParent(bowRoot.transform, false);
            center.transform.localScale = new Vector3(0.04f, 0.25f, 0.05f);
            SetMaterial(center, new Color(0.4f, 0.25f, 0.15f), 0.3f);
            DestroyCollider(center);

            // Upper Arc
            GameObject upper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            upper.name = "UpperLimb";
            upper.transform.SetParent(bowRoot.transform, false);
            upper.transform.localPosition = new Vector3(0f, 0.3f, 0.12f);
            upper.transform.localRotation = Quaternion.Euler(-30f, 0f, 0f);
            upper.transform.localScale = new Vector3(0.035f, 0.45f, 0.04f);
            SetMaterial(upper, new Color(0.55f, 0.35f, 0.2f), 0.3f);
            DestroyCollider(upper);

            // Lower Arc
            GameObject lower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lower.name = "LowerLimb";
            lower.transform.SetParent(bowRoot.transform, false);
            lower.transform.localPosition = new Vector3(0f, -0.3f, 0.12f);
            lower.transform.localRotation = Quaternion.Euler(30f, 0f, 0f);
            lower.transform.localScale = new Vector3(0.035f, 0.45f, 0.04f);
            SetMaterial(lower, new Color(0.55f, 0.35f, 0.2f), 0.3f);
            DestroyCollider(lower);

            return bowRoot;
        }

        private void SetMaterial(GameObject obj, Color color, float metallic = 0.5f)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                Shader shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Diffuse");
                Material mat = new Material(shader);
                mat.color = color;
                if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
                if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.6f);
                rend.material = mat;
            }
        }

        private void DestroyCollider(GameObject obj)
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }
        #endregion
    }
}

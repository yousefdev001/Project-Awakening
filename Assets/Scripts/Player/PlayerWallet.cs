using System;
using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Player currency wallet managing Gold balance, earning, and spending.
    /// </summary>
    public class PlayerWallet : MonoBehaviour
    {
        public static PlayerWallet Instance { get; private set; }

        [Header("Initial Balance")]
        [SerializeField] private int _gold = 50;

        public int Gold => _gold;

        public event Action<int, int> OnGoldChanged; // (currentGold, changeAmount)

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
            OnGoldChanged?.Invoke(_gold, 0);
        }

        public void AddGold(int amount)
        {
            if (amount <= 0) return;

            _gold += amount;
            Debug.Log($"<color=#FFD700>[PlayerWallet]</color> Collected <b>+{amount} Gold</b>! Total: <b>{_gold} Gold</b>.");
            OnGoldChanged?.Invoke(_gold, amount);
        }

        public bool SpendGold(int amount)
        {
            if (amount <= 0) return true;

            if (_gold >= amount)
            {
                _gold -= amount;
                Debug.Log($"<color=#FFAA00>[PlayerWallet]</color> Spent <b>-{amount} Gold</b>. Remaining: <b>{_gold} Gold</b>.");
                OnGoldChanged?.Invoke(_gold, -amount);
                return true;
            }

            Debug.LogWarning($"[PlayerWallet] Insufficient funds! Needed {amount} Gold, but only have {_gold} Gold.");
            return false;
        }
    }
}

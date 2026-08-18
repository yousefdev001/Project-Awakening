using UnityEngine;

namespace Awakening.NPCs
{
    /// <summary>
    /// ScriptableObject defining an NPC's identity, role, and dialogue lines.
    /// Data-Driven: Writing new story dialogues requires zero C# script changes.
    /// </summary>
    [CreateAssetMenu(fileName = "NewNPCData", menuName = "Awakening/NPCs/NPC Data")]
    public class NPCData : ScriptableObject
    {
        [Header("Identity")]
        public string npcID = "NPC_ELDER";
        public string npcName = "Eldrin";
        public string npcRole = "Village Elder";
        public Color themeColor = new Color(0.3f, 0.7f, 1.0f);

        [Header("Dialogue Content")]
        [TextArea(2, 4)]
        public string greeting = "Greetings, Awakened traveler. The ancient prophecy spoke of your arrival.";

        [TextArea(2, 4)]
        public string[] dialogueLines = new string[]
        {
            "Darkness stirs deep within the Whispering Forest. The Goblin Chief has unified the scattered tribes.",
            "Take up arms, master your profession's skills, and purge the nest before the village falls to ruin.",
            "Visit Garrick at the forge to hone your blades, and Lyra for healing draughts before venturing forth."
        };

        #region MVP NPC Presets Factory
        public static NPCData CreateElderPreset()
        {
            var data = ScriptableObject.CreateInstance<NPCData>();
            data.npcID = "NPC_ELDER";
            data.npcName = "Elder Eldrin";
            data.npcRole = "Village Elder & Lorekeeper";
            data.themeColor = new Color(0.35f, 0.75f, 1.0f);
            data.greeting = "Greetings, newly Awakened hero. May the celestial light guide your blade.";
            data.dialogueLines = new string[]
            {
                "The Ancient Circle has chosen you. Your awakened profession will be the shield of this realm.",
                "To the north lies the Whispering Forest, overrun by aggressive wolves and goblin raiders.",
                "Slay the monsters, gather valuable monster materials, and prepare yourself for the Goblin Nest."
            };
            return data;
        }

        public static NPCData CreateBlacksmithPreset()
        {
            var data = ScriptableObject.CreateInstance<NPCData>();
            data.npcID = "NPC_BLACKSMITH";
            data.npcName = "Garrick Ironhand";
            data.npcRole = "Master Blacksmith";
            data.themeColor = new Color(0.95f, 0.45f, 0.15f);
            data.greeting = "Ha! Welcome to the forge. Need steel that won't shatter when you hit a goblin skull?";
            data.dialogueLines = new string[]
            {
                "Bring me Wolf Pelts, Goblin Scrap Daggers, and iron ores from the forest.",
                "A true warrior knows that matching their favored weapon to their awakened rank yields 20% more power!",
                "Keep your blades sharp and your armor tight. Those beasts out there don't fight fair."
            };
            return data;
        }

        public static NPCData CreateMerchantPreset()
        {
            var data = ScriptableObject.CreateInstance<NPCData>();
            data.npcID = "NPC_MERCHANT";
            data.npcName = "Lyra the Alchemist";
            data.npcRole = "Potions & Rare Curios";
            data.themeColor = new Color(0.85f, 0.35f, 0.95f);
            data.greeting = "Welcome, traveler! Seeking remedies for deep wounds or mana draughts for your spells?";
            data.dialogueLines = new string[]
            {
                "I brew the finest Health and Mana potions in the province using forest herbs and Slime Jelly.",
                "Don't venture into the Goblin Nest without at least 3 Health Potions in your bag!",
                "I also pay top coin for rare materials like Wolf Fangs and ancient crystals."
            };
            return data;
        }
        #endregion
    }
}

using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerNameInput : MonoBehaviour
    {
        [SerializeField] private TMP_InputField playerNameInputField = null;

        public static string playerName = "";

        private string NAME_KEY = "PlayerName";
        private void Start()
        {
            if (PlayerPrefs.HasKey(NAME_KEY))
            {
                playerName = PlayerPrefs.GetString(NAME_KEY);
                playerNameInputField.text = playerName;
            }
        }

        public void SetPlayerName(string newPlayerName)
        {
            playerName = newPlayerName;
            PlayerPrefs.SetString(NAME_KEY, playerName);
        }

    }
}
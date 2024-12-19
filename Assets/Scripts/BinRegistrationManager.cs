using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Record;
using UnityEngine.UIElements;

namespace CompanySystem
{
    public class BinRegistrationManager : MonoBehaviour
    {
        public TMP_InputField codeInputField;
        public TMP_InputField informationInputField;

        public GameObject binTable;

        // Register new bin to system
        public void AddNewBin()
        {
            BinRecord newBin = new BinRecord(codeInputField.text, informationInputField.text);
            var binData = new Dictionary<string, object>
            {
                { "code", newBin.Code },
                { "information", newBin.Information },
                { "numberoftags", newBin.NumberOfTags },
                { "active", newBin.Active }
            };

            StartCoroutine(FirebaseServices.WriteData("bins", binData, true, "code", message =>
            {
                if (message.Contains("successfully"))
                {
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
                }
                else if (message.Contains("registered"))
                {
                    Debug.LogWarning(message);
                }
                else
                {
                    Debug.LogError(message);
                }
            }));
        }

        // Reset input field
        public void ResetInput()
        {
            if (codeInputField.text.Length > 0)
                codeInputField.text = codeInputField.text.Remove(0);
            if (informationInputField.text.Length > 0)
                informationInputField.text = informationInputField.text.Remove(0);
        }
        private void RefreshTable()
        {
            binTable.SetActive(false);
            binTable.SetActive(true);
        }
    }
}

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Record;

namespace CompanySystem
{
    public class BinRegistrationManager : MonoBehaviour
    {
        //public GoogleFormConnector connector; // Reference to the Google Form connector
        public TMP_InputField codeInputField;
        public TMP_InputField informationInputField;

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
                    Debug.Log(message);
                    ResetInput();
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
    }
}

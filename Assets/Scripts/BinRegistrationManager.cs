using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace CompanySystem
{
    public class BinRegistrationManager : MonoBehaviour
    {
        //public GoogleFormConnector connector; // Reference to the Google Form connector
        public TMP_InputField codeInputField;
        public TMP_InputField informationInputField;
        private string code; // Variable to store the bin code
        private string information; // Variable to store the additional information

        // Register new bin to system
        public void AddNewBin()
        {
            code = codeInputField.text;
            information = informationInputField.text;

            var binData = new Dictionary<string, object>
            {
                { "code", code },
                { "information", information },
                { "numberoftags", "0" },
                { "active", "1" }
            };
            StartCoroutine(FirebaseServices.WriteData("bins", binData));
            ResetInput();
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

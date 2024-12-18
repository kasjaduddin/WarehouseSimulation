using Record;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CompanySystem
{
    public class EditBinManager : MonoBehaviour
    {
        //public GoogleSheetsConnector connector; // Reference to the Google Form connector
        public TMP_InputField codeInputField;
        public TMP_InputField informationInputField;

        void OnEnable()
        {
            codeInputField.text = BinListManager.selectedRecord.Code.ToString();
            informationInputField.text = BinListManager.selectedRecord.Information.ToString();
        }
        public void EditBin()
        {
            BinRecord newBin = new BinRecord(codeInputField.text, informationInputField.text);
            string lastCode = BinListManager.selectedRecord.Code;

            var newBinData = new Dictionary<string, object>
    {
        { "id", BinListManager.selectedRecord.Id },
        { "code", newBin.Code },
        { "information", newBin.Information },
        { "numberoftags", newBin.NumberOfTags },
        { "active", newBin.Active }
    };

            StartCoroutine(FirebaseServices.ModifyData("bins", newBinData, true, lastCode, "code", message =>
            {
                Debug.Log(message);
                if (message.Contains("updated"))
                {
                    ResetInput();
                }
                else if (message.Contains("already exists"))
                {
                    Debug.LogWarning("Modification failed: Duplicate primary key found.");
                }
                else
                {
                    Debug.LogError("Modification failed: " + message);
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

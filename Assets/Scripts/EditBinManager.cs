using Record;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace CompanySystem
{
    public class EditBinManager : MonoBehaviour
    {
        public TMP_InputField codeInputField;
        public TMP_InputField informationInputField;

        public GameObject binTable;

        void OnEnable()
        {
            codeInputField.text = BinListManager.selectedRecord.Code.ToString();
            informationInputField.text = BinListManager.selectedRecord.Information.ToString();
        }
        public void EditBin()
        {
            BinRecord newBin = new BinRecord(codeInputField.text, informationInputField.text);
            string oldCode = BinListManager.selectedRecord.Code;

            var newBinData = new Dictionary<string, object>
            {
                { "id", BinListManager.selectedRecord.Id },
                { "code", newBin.Code },
                { "information", newBin.Information },
                { "numberoftags", newBin.NumberOfTags },
                { "active", newBin.Active }
            };
            
            StartCoroutine(FirebaseServices.ModifyData("bins", newBinData, true, oldCode, "code", message =>
            {
                if (message.Contains("successfully"))
                {
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
                    BinListManager.ResetSelectedRecord();
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

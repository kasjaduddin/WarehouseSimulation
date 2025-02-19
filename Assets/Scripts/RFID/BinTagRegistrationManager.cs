using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Rfid
{
    public class BinTagRegistrationManager : MonoBehaviour
    {
        private TMP_Dropdown binCodeDropdown;
        private TextMeshProUGUI description;
        private TextMeshProUGUI tags;

        private void OnEnable()
        {
            binCodeDropdown = transform.Find("Bin Code Dropdown").GetComponent<TMP_Dropdown>();
            description = transform.Find("Description").GetComponent<TextMeshProUGUI>();
            tags = transform.Find("Tags").GetComponent<TextMeshProUGUI>();
            Invoke("GetBinCodes", 0.1f);
        }

        private void OnDisable()
        {
            binCodeDropdown.options.Clear();
        }

        public void GetBinCodes()
        {
            StartCoroutine(FirebaseServices.ReadData("bins", data =>
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        binCodeDropdown.options.Add(new TMP_Dropdown.OptionData(item["code"].ToString()));
                    }
                    binCodeDropdown.captionText.text = binCodeDropdown.options[0].text;
                    Invoke("LoadBinData", 0.1f);
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        public void LoadBinData()
        {
            StartCoroutine(FirebaseServices.ReadData("bins", data =>
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item["code"].ToString() == binCodeDropdown.captionText.text)
                        {
                            description.text = item["information"].ToString();
                            tags.text = item["number_of_tags"].ToString();
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }
    }
}
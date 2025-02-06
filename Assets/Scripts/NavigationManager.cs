using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CompanySystem
{
    public class NavigationManager : MonoBehaviour
    {
        struct Navbar
        {
            public Button masterDataBinButton;
            public Button masterDataItemButton;
            public Button transactionsButton;
            public Button reservationsButton;
        }

        [SerializeField] List<GameObject> pages = new List<GameObject>();
        Navbar navbar;

        // Start is called before the first frame update
        void Start()
        {
            navbar.masterDataBinButton = gameObject.transform.Find("Navbar").Find("Master Data Bin Button").GetComponent<Button>();
            navbar.masterDataItemButton = gameObject.transform.Find("Navbar").Find("Master Data Item Button").GetComponent<Button>();
            navbar.transactionsButton = gameObject.transform.Find("Navbar").Find("Transactions Button").GetComponent<Button>();
            navbar.reservationsButton = gameObject.transform.Find("Navbar").Find("Reservations Button").GetComponent<Button>();

            navbar.masterDataBinButton.onClick.AddListener(() => OpenPage(pages[0], "Bin"));
            navbar.masterDataItemButton.onClick.AddListener(() => OpenPage(pages[1], "Item"));
            navbar.transactionsButton.onClick.AddListener(() => OpenPage(pages[2], "Transaction"));
            navbar.reservationsButton.onClick.AddListener(() => OpenPage(pages[3], "Reservation"));
        }

        private void OpenPage(GameObject selectedPage, string pageName)
        {
            foreach (var page in pages)
            {
                if (page == selectedPage)
                {
                    foreach (Transform child in page.transform)
                    {
                        if (child == page.transform.Find($"{pageName} List"))
                            child.gameObject.SetActive(true);
                        else
                            child.gameObject.SetActive(false);
                    }
                    page.SetActive(true);
                }
                else
                {
                    page.SetActive(false);
                }
            }
        }
    }
}
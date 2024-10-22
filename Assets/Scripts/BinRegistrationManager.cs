using UnityEngine;

public class BinRegistrationManager : MonoBehaviour
{
    public GoogleFormConnector connector;
    // Start is called before the first frame update
    private string code;
    private string information;
    void Start()
    {
        AddNewBin();
    }
    void AddNewBin()
    {
        code = "A1";
        information = "Box";

        StartCoroutine(connector.WriteData(code, information));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

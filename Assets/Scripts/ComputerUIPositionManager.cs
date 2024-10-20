using UnityEngine;
public class ComputerUIPositionManager : MonoBehaviour
{
    private Camera mainCamera; // User camera
    [SerializeField]
    private GameObject screen; // Computer screen game object
    [SerializeField]
    private GameObject keyboard; // Computer keyboard game object
    // Start is called before the first frame update
    void Start()
    {
        // Set main camera
        mainCamera = Camera.main;

        if (screen != null)
        {
            // Set initial static position and rotation
            screen.transform.position = new Vector3(screen.transform.position.x, screen.transform.position.y, screen.transform.position.z);
            screen.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (keyboard != null)
            keyboard.transform.position = new Vector3(keyboard.transform.position.x, keyboard.transform.position.y, keyboard.transform.position.z);        
    }
    private void OnEnable()
    {
        Invoke("SetScreenPosition", 0.1f);
        Invoke("SetKeyboardPosition", 0.1f);
    }
    // Set screen position
    public void SetScreenPosition()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 screenPosition = new Vector3(screen.transform.position.x, cameraPosition.y, cameraPosition.z + 0.4f);
        screen.transform.position = screenPosition;
    }
    public void SetKeyboardPosition()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 keyboardPosition = new Vector3(keyboard.transform.position.x, cameraPosition.y - 0.2f, cameraPosition.z + 0.35f);
        keyboard.transform.position = keyboardPosition;
    }
}

using System;
using System.Collections;
using UnityEngine;
public class ComputerUIPositionManager : MonoBehaviour
{
    private Camera mainCamera; // User camera
    [SerializeField]
    private GameObject screen; // Computer screen game object
    [SerializeField]
    private GameObject keyboard; // Computer keyboard game object

    [Serializable]
    public struct ComputerUIOffset
    {
        public float x;
        public float y;
        public float z;
    }

    [SerializeField]
    ComputerUIOffset screenOffset = new ComputerUIOffset();

    // Start is called before the first frame update
    void Start()
    {
        // Set main camera
        mainCamera = Camera.main;

        // Set initial screen static position and rotation
        if (screen != null)
        {
            screen.transform.position = new Vector3(screen.transform.position.x, screen.transform.position.y, screen.transform.position.z);
            screen.transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        // Set initial keyboard static position and rotation
        if (keyboard != null)
            keyboard.transform.position = new Vector3(keyboard.transform.position.x, keyboard.transform.position.y, keyboard.transform.position.z);        
    }

    // OnEnable is called when the object becomes enabled and active

    private void OnEnable()
    {
        StartCoroutine(SetScreenPosition());
    }

    // Set screen position
    private IEnumerator SetScreenPosition()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 screenPosition = new Vector3(cameraPosition.x + screenOffset.x, cameraPosition.y + screenOffset.y, cameraPosition.z + screenOffset.z);
        screen.transform.position = screenPosition;

        SetKeyboardPosition();
    }

    // Set keyboard position
    private void SetKeyboardPosition()
    {
        Vector3 screenPosition = screen.transform.position;
        Vector3 keyboardPosition = new Vector3(screenPosition.x, screenPosition.y - 0.2f, screenPosition.z);
        keyboard.transform.position = keyboardPosition;
    }
}

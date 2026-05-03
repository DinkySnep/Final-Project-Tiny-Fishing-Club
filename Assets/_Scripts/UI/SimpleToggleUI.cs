using UnityEngine;

public class SimpleToggleUI : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel;

    private bool isOpen = false;

    public void Toggle()
    {
        isOpen = !isOpen;
        targetPanel.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;
    }
    void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }
}
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text inventoryText;

    private bool isOpen = false;

    public void Toggle()
    {
        isOpen = !isOpen;
        panel.SetActive(isOpen);

        if (isOpen)
        {
            Refresh();
            Time.timeScale = 0f; // pause
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void Refresh()
    {
        var fishList = PlayerData.Instance.inventory;

        if (fishList.Count == 0)
        {
            inventoryText.text = "No fish caught yet.";
            return;
        }

        var sorted = fishList
            .OrderByDescending(f => f.data.rarity)
            .ThenByDescending(f => f.weight)
            .ToList();

        StringBuilder sb = new StringBuilder();

        foreach (var fish in sorted)
        {
            sb.Append(fish.data.fishName);
            sb.Append(", ");
            sb.Append(fish.data.rarity);
            sb.Append(", ");
            sb.Append(fish.weight.ToString("F1"));
            sb.Append(" kg");
            sb.AppendLine();
        }

        inventoryText.text = sb.ToString();
    }
}
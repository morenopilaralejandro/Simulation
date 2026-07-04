using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Match;

public class MatchRankRewardIU : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text itemNameText;

    /// <summary>
    /// Sets up the reward UI.
    /// </summary>
    /// <param name="matchRank">The match rank.</param>
    /// <param name="itemId">The item ID.</param>
    public void Setup(MatchRank matchRank, string itemId)
    {
        iconImage.sprite = IconManager.Instance.MatchRank.GetIcon(matchRank);
        if (string.IsNullOrEmpty(itemId)) 
        {
            itemNameText.text = "";
            return;
        }
        var item = ItemFactory.CreateById(itemId);
        itemNameText.text = item.ItemName;
    }
}

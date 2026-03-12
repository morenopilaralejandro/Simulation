using System;

[Serializable]
public class DialogKit
{
    public string KitId { get; set; } = "none";
    public string VariantId { get; set; } = "home";
    public string RoleId { get; set; } = "field";

    /// <summary>
    /// Unique key for looking up the correct portrait.
    /// Example: "plate_armor_gold_captain"
    /// </summary>
    public string PortraitAddress => $"{KitId}_{VariantId}_{RoleId}";

    public bool IsValid => !string.IsNullOrEmpty(KitId);
}

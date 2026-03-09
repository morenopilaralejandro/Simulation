public class DialogChoice
{
    public int Index { get; set; }
    public string RawText { get; set; }
    public string LocalizationKey { get; set; }
    public bool IsYes { get; set; }
    public bool IsNo { get; set; }
    public bool IsYesNoChoice { get; set; }
    public string ResolvedText { get; set; }
}

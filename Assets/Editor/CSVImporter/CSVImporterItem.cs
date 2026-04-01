using UnityEngine;
using UnityEditor;
using System.IO;
using Simulation.Enums.Item;

public class CSVImporterItem
{
    [MenuItem("Tools/Import CSV/Item")]
    public static void ImportItemsFromCSV()
    {
        string assetFolder = "Assets/Addressables/Items/Data";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Item CSV File", defaultPath, "csv");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("No CSV file selected.");
            return;
        }

        AssetDatabaseManager.CreateFolderFromPath(assetFolder);

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV file does not contain enough lines.");
            return;
        }

        // Get CSV header index mapping
        string[] headers = lines[0].Split(',');

        // Base
        int itemIdIndex         = System.Array.IndexOf(headers, "ItemId");
        int categoryIndex       = System.Array.IndexOf(headers, "Category");
        int spriteTypeIndex     = System.Array.IndexOf(headers, "SpriteType");
        int spriteColorIndex    = System.Array.IndexOf(headers, "SpriteColor");
        int usageContextIndex   = System.Array.IndexOf(headers, "UsageContext");
        int isSellableIndex     = System.Array.IndexOf(headers, "IsSellable");
        int priceBuyGoldIndex   = System.Array.IndexOf(headers, "PriceBuyGold");
        int priceSellGoldIndex  = System.Array.IndexOf(headers, "PriceSellGold");
        int isDiscardableIndex  = System.Array.IndexOf(headers, "IsDiscardable");
        int isStackableIndex    = System.Array.IndexOf(headers, "IsStackable");
        int maxStackIndex       = System.Array.IndexOf(headers, "MaxStack");

        // Equipment
        int equipmentTypeIndex      = System.Array.IndexOf(headers, "EquipmentType");
        int equipmentHpIndex        = System.Array.IndexOf(headers, "EquipmentHp");
        int equipmentSpIndex        = System.Array.IndexOf(headers, "EquipmentSp");
        int equipmentKickIndex      = System.Array.IndexOf(headers, "EquipmentKick");
        int equipmentBodyIndex      = System.Array.IndexOf(headers, "EquipmentBody");
        int equipmentControlIndex   = System.Array.IndexOf(headers, "EquipmentControl");
        int equipmentGuardIndex     = System.Array.IndexOf(headers, "EquipmentGuard");
        int equipmentSpeedIndex     = System.Array.IndexOf(headers, "EquipmentSpeed");
        int equipmentStaminaIndex   = System.Array.IndexOf(headers, "EquipmentStamina");
        int equipmentTechniqueIndex = System.Array.IndexOf(headers, "EquipmentTechnique");
        int equipmentLuckIndex      = System.Array.IndexOf(headers, "EquipmentLuck");
        int equipmentCourageIndex   = System.Array.IndexOf(headers, "EquipmentCourage");

        // Formation
        int formationIdIndex = System.Array.IndexOf(headers, "FormationId");

        // Important
        int importantPlaceHolderIndex = System.Array.IndexOf(headers, "ImportantPlaceHolder");

        // Kit
        int kitIdIndex = System.Array.IndexOf(headers, "KitId");

        // Material
        int materialPlaceHolderIndex = System.Array.IndexOf(headers, "MaterialPlaceHolder");

        // Misc
        int miscPlaceHolderIndex = System.Array.IndexOf(headers, "MiscPlaceHolder");

        // Move
        int moveIdIndex = System.Array.IndexOf(headers, "MoveId");

        // Recovery
        int recoveryAmountHpIndex = System.Array.IndexOf(headers, "RecoveryAmountHp");
        int recoveryAmountSpIndex = System.Array.IndexOf(headers, "RecoveryAmountSp");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            string itemId = values[itemIdIndex].Trim();
            ItemCategory category = EnumManager.StringToEnum<ItemCategory>(values[categoryIndex].Trim());
            ItemData itemData = ItemDataFactory.CreateByCategory(category);

            // Base
            itemData.ItemId         = values[itemIdIndex].Trim();
            itemData.Category       = category;
            itemData.SpriteType     = EnumManager.StringToEnum<ItemSpriteType>(values[spriteTypeIndex].Trim());
            itemData.SpriteColor    = EnumManager.StringToEnum<ItemSpriteColor>(values[spriteColorIndex].Trim());
            itemData.UsageContext   = EnumManager.StringToEnum<ItemUsageContext>(values[usageContextIndex].Trim());
            itemData.IsSellable     = CSVImporterParser.ParseBool(values[isSellableIndex].Trim());
            itemData.PriceBuyGold   = int.Parse(values[priceBuyGoldIndex].Trim());
            itemData.PriceSellGold  = int.Parse(values[priceSellGoldIndex].Trim());
            itemData.IsDiscardable  = CSVImporterParser.ParseBool(values[isDiscardableIndex].Trim());
            itemData.IsStackable    = CSVImporterParser.ParseBool(values[isStackableIndex].Trim());
            itemData.MaxStack       = int.Parse(values[maxStackIndex].Trim());

            switch (itemData)
            {
                case ItemDataEquipment itemDataEquipment:
                    itemDataEquipment.EquipmentType          = EnumManager.StringToEnum<EquipmentType>(values[equipmentTypeIndex].Trim());
                    itemDataEquipment.EquipmentHp            = int.Parse(values[equipmentHpIndex].Trim());
                    itemDataEquipment.EquipmentSp            = int.Parse(values[equipmentSpIndex].Trim());
                    itemDataEquipment.EquipmentKick          = int.Parse(values[equipmentKickIndex].Trim());
                    itemDataEquipment.EquipmentBody          = int.Parse(values[equipmentBodyIndex].Trim());
                    itemDataEquipment.EquipmentControl       = int.Parse(values[equipmentControlIndex].Trim());
                    itemDataEquipment.EquipmentGuard         = int.Parse(values[equipmentGuardIndex].Trim());
                    itemDataEquipment.EquipmentSpeed         = int.Parse(values[equipmentSpeedIndex].Trim());
                    itemDataEquipment.EquipmentStamina       = int.Parse(values[equipmentStaminaIndex].Trim());
                    itemDataEquipment.EquipmentTechnique     = int.Parse(values[equipmentTechniqueIndex].Trim());
                    itemDataEquipment.EquipmentLuck          = int.Parse(values[equipmentLuckIndex].Trim());
                    itemDataEquipment.EquipmentCourage       = int.Parse(values[equipmentCourageIndex].Trim());
                    break;

                case ItemDataFormation itemDataFormation:
                    itemDataFormation.FormationId = values[formationIdIndex].Trim();
                    break;

                case ItemDataImportant itemDataImportant:
                    itemDataImportant.PlaceHolder = CSVImporterParser.ParseBool(values[importantPlaceHolderIndex].Trim());
                    break;

                case ItemDataKit itemDataKit:
                    itemDataKit.KitId = values[kitIdIndex].Trim();
                    break;

                case ItemDataMaterial itemDataMaterial:
                    itemDataMaterial.PlaceHolder = CSVImporterParser.ParseBool(values[materialPlaceHolderIndex].Trim());
                    break;

                case ItemDataMisc itemDataMisc:
                    itemDataMisc.PlaceHolder = CSVImporterParser.ParseBool(values[miscPlaceHolderIndex].Trim());
                    break;

                case ItemDataMove itemDataMove:
                    itemDataMove.MoveId = values[moveIdIndex].Trim();
                    break;

                case ItemDataRecovery itemDataRecovery:
                    itemDataRecovery.RecoveryAmountHp = int.Parse(values[recoveryAmountHpIndex].Trim());
                    itemDataRecovery.RecoveryAmountSp = int.Parse(values[recoveryAmountSpIndex].Trim());
                    break;
            }

            string safeName = "item" + "-" + category.ToString().ToLower() + "-" + itemId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(itemData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Item ScriptableObjects created from CSV.");
    }
}

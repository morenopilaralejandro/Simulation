using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Match;
using Aremoreno.Enums.Localization;

public class MatchChain
{
    #region Components

    private MatchChainComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private MatchChainComponentNodes nodesComponent;
    private MatchChainComponentPersistence persistenceComponent;

    #endregion

    #region Initialize

    public MatchChain(MatchChainData data, MatchChainSaveData saveData = null) 
    {
        attributesComponent = new MatchChainComponentAttributes(data);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Match_Chain,
            data.MatchChainId,
            new[] { LocalizationField.Name}
        );
        nodesComponent = new MatchChainComponentNodes(data, this, saveData);
        persistenceComponent = new MatchChainComponentPersistence(data, this, saveData);
    }

    #endregion

    #region API

    // attributesComponent
    public string MatchChainId => attributesComponent.MatchChainId;

    // localizationComponent
    public string MatchChainName => localizationStringComponent.GetString(LocalizationField.Name);

    //nodesComponent
    public List<MatchChainNode> Nodes => nodesComponent.Nodes;
    public void SortNodexByIndex() => nodesComponent.SortNodexByIndex();

    //persistenceComponent
    public void Import(MatchChainSaveData saveData) => persistenceComponent.Import(saveData);
    public MatchChainSaveData Export() => persistenceComponent.Export();
    public int SelectedIndex => persistenceComponent.SelectedIndex;
    public void SetSelectedIndex(int intValue) => persistenceComponent.SetSelectedIndex(intValue);

    #endregion
}

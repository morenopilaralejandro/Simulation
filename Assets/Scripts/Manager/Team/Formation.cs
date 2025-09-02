using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Localization;

public class Formation
{
    [SerializeField] private string formationId;
    public string FormationId => formationId;

    [SerializeField] private ComponentLocalizationString stringLocalizationComponent;

    [SerializeField] private List<FormationCoord> formationCoords = new();
    public List<FormationCoord> FormationCoords => formationCoords;

    [SerializeField] private int kickoff0;
    public int Kickoff0 => kickoff0;

    [SerializeField] private int kickoff1;
    public int Kickoff1 => kickoff1;

    public void Initialize(FormationData formationData)
    {
        formationId = formationData.FormationId;

        stringLocalizationComponent = new ComponentLocalizationString(
            LocalizationEntity.Formation,
            formationData.FormationId,
            new [] { LocalizationField.Name }
        );
    
        for (int i = 0; i < formationData.CoordIds.Count; i++) 
        {
            if (!string.IsNullOrEmpty(formationData.CoordIds[i])) {
                FormationCoordData formationCoordData = FormationCoordManager.Instance.GetFormationCoordData(formationData.CoordIds[i]);
                formationCoords.Add(
                    new FormationCoord (
                        formationData.CoordIds[i],
                        new Vector3 (formationCoordData.X, formationCoordData.Y, formationCoordData.Z),
                        formationData.Positions[i]
                    )
                );
            }
        }

        kickoff0 = formationData.Kickoff0;
        kickoff1 = formationData.Kickoff1;
    }

    public string FormationName => stringLocalizationComponent.GetString(LocalizationField.Name);
}

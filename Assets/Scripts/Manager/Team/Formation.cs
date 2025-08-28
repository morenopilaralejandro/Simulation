using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using Simulation.Enums.Character;

public class Formation : MonoBehaviour
{
    [SerializeField] private string formationId;
    public string FormationId => formationId;

    [SerializeField] private LocalizedString localizedName;
    public LocalizedString LocalizedName => localizedName;

    [SerializeField] private List<FormationCoord> formationCoords;
    public List<FormationCoord> FormationCoords => formationCoords;

    [SerializeField] private int kickoff0;
    public int Kickoff0 => kickoff0;

    [SerializeField] private int kickoff1;
    public int Kickoff1 => kickoff1;

    [SerializeField] private bool isRomazed = false;
    [SerializeField] private string stringTableNameLocalized = "FormationNamesLocalized";
    [SerializeField] private string stringTableNameRomanized = "FormationNamesRomanized";


    public void Initialize(FormationData formationData)
    {
        formationId = formationData.FormationId;
    
        for (int i = 0; i < formationData.CoordIds.Count; i++) 
        {
            //FormationCoordData formationCoordData  = TeamManager.Instance.GetFormationCoordDataById(formationData.CoordIds[i]);
            FormationCoordData formationCoordData = null;
            formationCoords.Add(
                new FormationCoord (
                    formationData.CoordIds[i],
                    new Vector3 (formationCoordData.X, formationCoordData.Y, formationCoordData.Z),
                    formationData.Positions[i]
                )
            );
        }

        kickoff0 = formationData.Kickoff0;
        kickoff1 = formationData.Kickoff1;

        SetName();
    }

    private void SetName()
    {
        localizedName = new LocalizedString(
            isRomazed ? stringTableNameRomanized : stringTableNameLocalized,
            formationId
        );
    }
}

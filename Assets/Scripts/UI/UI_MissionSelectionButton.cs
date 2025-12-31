using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MissionSelectionButton : UI_Button
{
    private UI_MissionSelection missionUI;
    [SerializeField] private Mission myMission;
    private TextMeshProUGUI myText;
    
    private void OnValidate()
    {
        if (myMission != null)
            gameObject.name = "Button - Select Mission: " + myMission.missionName;
    }

    protected override void Start()
    {
        base.Start();
        myText = GetComponentInChildren<TextMeshProUGUI>(true);
        myText.text = myMission.missionName;
        missionUI = GetComponentInParent<UI_MissionSelection>(true);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        missionUI.UpdateMissionDescription(myMission.missionDescription);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        missionUI.UpdateMissionDescription("Choose Mission");
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //Set mission in the game
        MissionManager.instance.SetCurrentMission(myMission);
    }
}
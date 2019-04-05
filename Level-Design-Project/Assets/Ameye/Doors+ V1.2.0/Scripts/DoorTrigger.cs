// DoorTrigger.cs
// Created by Alexander Ameye
// Version 1.2.0

using UnityEngine;
using RuntimeScriptField;
using AlexDoorSystem;

[RequireComponent(typeof(BoxCollider), typeof(SphereCollider))]
public class DoorTrigger : MonoBehaviour, IHierarchyIcon
{
   public string EditorIconPath { get { return transform.gameObject.name; } }
    public int Id;

    #region Zone Settings
    public enum TypeOfCollider { Cubic, Spherical }
    public TypeOfCollider ColliderType;
    #endregion

    #region Player Requirements
    public bool CorrectTag, CorrectName, CorrectView, CorrectButton, CorrectScript, CorrectGameObject;
    public bool HasTag, HasName, IsLookingAt, HasPressed, HasScript, IsGameObject;
    //public bool RequiresPadLockUnlock, RequiresKeyPadUnlock;
    public bool PadLock, KeyPad = true;
    [TagSelector]
    public string PlayerTag = "Untagged";
    public string PlayerName, Character;
    public GameObject LookObject, IsObject;
    public ComponentReference Script;
    #endregion

    [HideInInspector]
    public bool AllGood = true;
    [HideInInspector]
    public bool ShowTriggerZones;

    private DoorRotation _doorrotation;
    private DoorDetection _doordetection;
    private DoorSound _doorsound;

    private void Start()
    {
        transform.gameObject.layer = LayerMask.NameToLayer("Trigger Zones");
    }

    private void OnTriggerEnter(Collider other)
    {
        _doorrotation = transform.parent.transform.parent.transform.GetComponentInChildren<DoorRotation>();
        _doorsound = transform.root.GetComponentInChildren<DoorSound>() != null ? transform.root.GetComponentInChildren<DoorSound>() : null;
        _doordetection = GameObject.FindGameObjectWithTag("Player").GetComponent<DoorDetection>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (LookObject)
            _doordetection.CheckUIPrefabs(LookObject);

        if (_doorrotation.RotationPending || _doorrotation.CurrentRotationBlockIndex >= _doorrotation.RotationTimeline.Count || _doorrotation.CurrentRotationBlockIndex != Id) return;
        
        CorrectTag = (!HasTag || other.CompareTag(PlayerTag));
        CorrectName = (!HasName || other.name == PlayerName);
        CorrectView = (!IsLookingAt || _doordetection.CheckIfLookingAt(LookObject));

        if(Character.Length == 1)
        {
            CorrectButton = (!HasPressed || Input.GetKey(Character));
        }
            
        else if (Character.Length > 1)
        {
            CorrectButton = (!HasPressed || Input.GetButton(Character));
        }

        else
        {
            CorrectButton = (!HasPressed);
        }

        CorrectScript = (!HasScript || other.gameObject.GetComponent(Script.script.Name) != null);
        CorrectGameObject = (!IsGameObject || other.gameObject == IsObject);

        //HERE
        //if(RequiresPadLockdUnlock) PadLock = get variable from padlock script
        //else PadLock = true;
        //PadLock = true;
        //KeyPad = true;
        //if(RequiresKeyPadUnlcok) KeyPad = get variable from keypad script
        //else KeyPad = true;

        AllGood = CorrectTag && CorrectName && CorrectView && CorrectScript && CorrectGameObject;

        if (CorrectTag && CorrectName && CorrectView && CorrectScript && CorrectButton && CorrectGameObject)
        {
            if (transform.gameObject.name == "Move Trigger")
            {
                if (_doorsound != null) PlaySoundStart();
                StartCoroutine(_doorrotation.Rotate());
                if (_doorsound != null) PlaySoundEnd();
            }

            else if (transform.gameObject.name == "Close Trigger")
            {
                if (_doorrotation.DoorIsClosing())
                {
                    if (_doorsound != null) PlaySoundStart();
                    StartCoroutine(_doorrotation.Rotate());
                    if (_doorsound != null) PlaySoundEnd();
                }
            }

            else if (transform.gameObject.name == "Open Trigger")
            {
                if (_doorrotation.DoorIsOpening())
                {
                    if (_doorsound != null) PlaySoundStart();
                    StartCoroutine(_doorrotation.Rotate());
                    if (_doorsound != null) PlaySoundEnd();
                }
            }

            else if (transform.gameObject.name == "Front Trigger")
            {
                if (_doorsound != null) PlaySoundStart();
                StartCoroutine(_doorrotation.Swing(true));
                if (_doorsound != null) PlaySoundEnd();
            }

            else if (transform.gameObject.name == "Back Trigger")
            {
                if (_doorsound != null) PlaySoundStart();
                StartCoroutine(_doorrotation.Swing(false));
                if (_doorsound != null) PlaySoundEnd();
            }
        }

        if (transform.gameObject.name == "Open Trigger" && _doorsound != null)
        {
            if (!AllGood && CorrectButton && _doorrotation.DoorIsOpening())
            {
                _doorsound.Play("locked");
                AllGood = false;
            }
        }

        if (transform.gameObject.name == "Close Trigger" && _doorsound != null)
        {
            if (!AllGood && CorrectButton && _doorrotation.DoorIsClosing())
            {
                _doorsound.Play("locked");
                AllGood = false;
            }
        }

        if (transform.gameObject.name == "Move Trigger" && _doorsound != null)
        {
            if (!AllGood && CorrectButton)
            {
                _doorsound.Play("locked");
                AllGood = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_doorrotation.ResetOnLeave) return;
        
        bool doorWasMoved = _doorrotation.transform.rotation == _doorrotation.EndRotation * _doorrotation.RotationOffset;
        bool hingeWasMoved = _doorrotation.transform.parent.transform.rotation == _doorrotation.EndRotation;

        if (_doorrotation.PivotPosition == DoorRotation.PositionOfPivot.CorrectlyPositioned)
        {
            if (_doorrotation.AngleConvention == 0 && doorWasMoved)
                StartCoroutine(RotationTools.Rotate(_doorrotation.gameObject, -_doorrotation.InitialYRotation + _doorrotation.RotationTimeline[0].FinalAngle, _doorrotation.RotationTimeline[0].InitialAngle - _doorrotation.InitialYRotation, 2, false, 0, _doorrotation.ShortestWay));
            else if (_doorrotation.AngleConvention == 1 && doorWasMoved)
                StartCoroutine(RotationTools.Rotate(_doorrotation.gameObject, -_doorrotation.InitialYRotation - _doorrotation.RotationTimeline[0].FinalAngle, -_doorrotation.RotationTimeline[0].InitialAngle - _doorrotation.InitialYRotation, 2, false, 0, _doorrotation.ShortestWay));
        }

        if (_doorrotation.PivotPosition == DoorRotation.PositionOfPivot.Centered)
        {
            if (_doorrotation.AngleConvention == 0 && hingeWasMoved)
                StartCoroutine(RotationTools.Rotate(_doorrotation.transform.parent.gameObject, _doorrotation.RotationTimeline[0].FinalAngle, _doorrotation.RotationTimeline[0].InitialAngle, 2, false, 0, _doorrotation.ShortestWay));
            else if (_doorrotation.AngleConvention == 1 && hingeWasMoved)
                StartCoroutine(RotationTools.Rotate(_doorrotation.transform.parent.gameObject, -_doorrotation.RotationTimeline[0].FinalAngle, -_doorrotation.RotationTimeline[0].InitialAngle, 2, false, 0, _doorrotation.ShortestWay));
        }

        _doorrotation.CurrentRotationBlockIndex = 0;
        _doorrotation.TimesRotated = 0;
    }

    private void PlaySoundStart()
    {
        if (_doorrotation.transform.GetComponent<DoorSound>() == null) return;
        
        string TriggerName = transform.gameObject.name;
        
        if (TriggerName == "Move Trigger" || TriggerName == "Front Trigger") _doorrotation.transform.GetComponent<DoorSound>().Play("opening");
        else if (TriggerName == "Back Trigger") _doorrotation.transform.GetComponent<DoorSound>().Play("closing");
        else if (_doorrotation.DoorIsOpening()) _doorrotation.transform.GetComponent<DoorSound>().Play("opening");
        else  _doorrotation.transform.GetComponent<DoorSound>().Play("closing");
    }

    private void PlaySoundEnd()
    {
        if (_doorrotation.transform.GetComponent<DoorSound>() == null) return;
        
        string TriggerName = transform.gameObject.name;

        if (TriggerName == "Move Trigger" || TriggerName == "Front Trigger") _doorrotation.transform.GetComponent<DoorSound>().Play("opened");
        else if (TriggerName == "Back Trigger") _doorrotation.transform.GetComponent<DoorSound>().Play("closed");
        else if (_doorrotation.DoorIsOpening()) _doorrotation.transform.GetComponent<DoorSound>().Play("opened");
        else _doorrotation.transform.GetComponent<DoorSound>().Play("closed");
    }
}
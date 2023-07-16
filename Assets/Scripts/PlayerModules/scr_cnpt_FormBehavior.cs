using System;
using System.Collections.Generic;
using UnityEngine;

public enum enum_forms
{
    Slime,
    Spider,
    Firefly
}


public class scr_cnpt_FormBehavior : MonoBehaviour
{
    public static scr_cnpt_FormBehavior instance = null;

    private Dictionary<enum_forms, scr_PlayerFormBase> enumToForm = new Dictionary<enum_forms, scr_PlayerFormBase>();
    
    [SerializeField]private List<scr_PlayerFormBase> formList;

    public scr_PlayerFormBase _currentForm;

    public static bool canChangeForm = true;

    InputManager input;

    public delegate void PressEvent();
    public static event PressEvent FormIsChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        input = InputManager.instance;

        // enumToForm = new Dictionary<enum_forms, scr_cnpt_Form_Abstract>()
        // {
        //     {enum_forms.Slime, new scr_cnpt_Slime(this)},
        //     {enum_forms.Spider, new scr_cnpt_Spider(this)},
        //     {enum_forms.Firefly, new scr_cnpt_Firefly(this)}
        // };
        
        
        for (int i = 0; i < formList.Count; i++)
        {
            enumToForm.Add((enum_forms)i,formList[i]);
        }

        _currentForm = enumToForm[enum_forms.Slime];

    }

    public void NextForm(enum_forms form)
    {
        if (!canChangeForm) return;
        
        if (enumToForm[form].GetType() != _currentForm.GetType())
        {
            _currentForm = enumToForm[form];
            FormIsChanged();
        }
        
    }
}

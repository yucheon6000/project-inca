using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void OnClick();
    public void OnHoverEnter();
    public void OnHoverExit();
    public bool IsInteractable();
}

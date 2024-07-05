using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType { Hitable, Selectable }

public interface InteractableObject
{
    public bool IsInteractableType(InteractableType type);
}

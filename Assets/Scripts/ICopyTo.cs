using System.Collections.Generic;
using UnityEngine;

public interface ICopyTo
{
    void CopyTo(Object oldObject, Object newObject, ref List<Component> componentsToDestroy);
}
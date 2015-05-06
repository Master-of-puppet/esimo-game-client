using UnityEngine;
using System.Collections;

public abstract class EYourButtonController {
    public abstract void UpdateButton();
    public abstract void UpdateDeck();
    public abstract void UpdateButtonDiscard();
    public virtual void UpdateButtonAddMeld()
    {

    }
}

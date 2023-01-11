using UnityEngine;

namespace LittleBit.Modules.UI
{
    public abstract class ShopTrigger : MonoBehaviour, IIntent, IInteractive
    {
        public bool Interactable { get; private set; } = true;
        public void SetInteractable(bool interactable)
        {
            Interactable = interactable;
        }
    }
}
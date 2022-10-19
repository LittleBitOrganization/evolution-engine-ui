using UnityEngine.Events;

namespace LittleBit.Modules.UI
{
    public interface ICloseButtonLayout
    {
        void AddOnClickListener(UnityAction listener);
    }
}
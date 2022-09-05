using UnityEngine.Events;

namespace LittleBit.Modules.UI
{
    public abstract class CommonWindow
    {
        public bool IsOpen { get; protected set; }

        public UnityEvent OnOpen { get; private set; }
        public UnityEvent OnClose { get; private set; }

        
        public ShopTrigger WindowTrigger;

        protected CommonWindow()
        {
            OnOpen = new UnityEvent();
            OnClose = new UnityEvent();
        }
        
        public abstract void OpenShop(WindowContext windowContext);
        public abstract void Close();
    }
}
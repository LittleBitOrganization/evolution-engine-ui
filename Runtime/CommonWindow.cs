using System;

namespace LittleBit.Modules.UI
{
    public abstract class CommonWindow
    {
        public bool IsOpen { get; protected set; }

        public abstract event Action OnOpen;
        public abstract event Action OnClose;
        public abstract event Action OnHidden;
        
        public IIntent Intent;
        
        
        public abstract void OpenShop(WindowContext windowContext);
        public abstract void Close();
        public abstract void Hide();
    }
}
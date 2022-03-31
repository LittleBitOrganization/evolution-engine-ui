using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LittleBit.Modules.UI
{
    public abstract partial class CommonUIService : IService
    {
        protected Dictionary<int,List<CommonWindow>> _windows;

        public CommonUIService()
        {
            _windows = new Dictionary<int, List<CommonWindow>>();
        }
        
        public void HideWindowsInLayer(int layer = 0)
        {
            foreach (var window in _windows[layer])
            {
                window.Hide();
            }
        }
        public void HideAllWindows()
        {
            foreach (var windowLayer in _windows)
            {
                foreach (var window in windowLayer.Value)
                {
                    HideWindow(window);
                }
            }
        }
        public void OpenWindow<T>(WindowContext context, int layer) where T : CommonWindow
        {
            CommonWindow window = _windows[layer].Find(x => x.GetType() == typeof(T));
            OpenWindow(window,context, layer);
        }
        protected void InitShop(CommonWindow shop, int layer, UnityAction onOpen ,UnityAction onClose)
        {
            if (!_windows.ContainsKey(layer))
            {
                _windows[layer] = new List<CommonWindow>();
            }
            _windows[layer].Add(shop);
            
            shop.OnOpen.AddListener(onOpen);
            shop.OnHidden.AddListener(onClose);   
            shop.OnHidden.AddListener(OnHiddenWindow);
        }

        protected abstract void OnHiddenWindow();
        
        protected virtual void OnRaycastHit(GameObject go)
        {
            if (go.TryGetComponent(out ShopTrigger shopTrigger))
            {
                if(!shopTrigger.Interactable) return;
            }
        }
        protected virtual void OpenWindow(CommonWindow window, WindowContext context, int layer)
        {
            if (window.WindowTrigger == context.WindowTrigger && context.WindowTrigger != null) return;

            CloseOpeningWindowExceptWindow(window, layer);
        
            if (window.IsOpen == false)
            {
                window.OpenShop(context);
            }
            else
            {
                window.Hide();
                window.OpenShop(context);
            }
        }
        protected virtual void SubscribeToRaycastService(IRaycastService raycastService)
        {
            raycastService.AddOnRaycastHitListener(OnRaycastHit);
        }
        
        private void CloseOpeningWindowExceptWindow(CommonWindow currentShop, int layer)
        {
            foreach (var window in _windows[layer])
            {
                if (window.IsOpen && window != currentShop)
                {
                    HideWindow(window);
                }
            }
        }
        private void HideWindow(CommonWindow window)
        {
            window.Hide();
        }
    }
}
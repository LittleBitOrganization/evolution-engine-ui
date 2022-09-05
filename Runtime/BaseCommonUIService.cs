using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LittleBit.Modules.UI
{
    public abstract partial class BaseCommonUIService : IService
    {
        protected Dictionary<int,List<CommonWindow>> _windows;

        public BaseCommonUIService()
        {
            _windows = new Dictionary<int, List<CommonWindow>>();
        }
        
        public void CloseWindowsInLayer(int layer = 0)
        {
            foreach (var window in _windows[layer])
            {
                CloseWindow(window);
            }

            OnCloseWindowsInLayer(layer);
        }
        
        public void CloseAllWindows()
        {
            foreach (var windowLayer in _windows)
            {
                foreach (var window in windowLayer.Value)
                {
                    CloseWindow(window);
                }
            }

            OnCloseAllWindows();
        }
        public void OpenWindow<T>(WindowContext context, int layer = 0) where T : CommonWindow
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
            shop.OnClose.AddListener(onClose);   
            shop.OnClose.AddListener(()=>
            {
                OnCloseWindow(layer);
            });
        }
        
        protected abstract void OnCloseWindow(int layer);
        protected abstract void OnCloseWindowsInLayer(int layer);
        protected abstract void OnCloseAllWindows();
        protected abstract void OnOpenWindow(CommonWindow commonWindow, int layer);

        protected virtual void OnRaycastHit(GameObject go)
        {
            if (go.TryGetComponent(out ShopTrigger shopTrigger))
            {
                if(!shopTrigger.Interactable) return;
            }
        }
        protected virtual void OpenWindow(CommonWindow window, WindowContext context, int layer = 0)
        {
            if (window.WindowTrigger == context.WindowTrigger && context.WindowTrigger != null) return;

            CloseOpeningWindowExceptWindow(window, layer);
        
            if (window.IsOpen == false)
            {
                window.OpenShop(context);
            }
            else
            {
                window.Close();
                window.OpenShop(context);
            }
            OnOpenWindow(window,layer);
        }
        
        private void CloseOpeningWindowExceptWindow(CommonWindow currentShop, int layer)
        {
            foreach (var window in _windows[layer])
            {
                if (window.IsOpen && window != currentShop)
                {
                    CloseWindow(window);
                }
            }
        }
        private void CloseWindow(CommonWindow window)
        {
            window.Close();
        }
    }
}
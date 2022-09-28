using System;
using System.Collections.Generic;
using System.Linq;
using LittleBit.Modules.CoreModule.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LittleBit.Modules.UI
{
    public abstract class BaseCommonUIService : IService
    {
        protected Dictionary<int,List<CommonWindow>> _windows;

        
        private Dictionary<int,LStack<OpenWindowCommand>> _stackCommonWindowInLayer;

        public BaseCommonUIService()
        {
            _windows = new Dictionary<int, List<CommonWindow>>();
            _stackCommonWindowInLayer = new Dictionary<int, LStack<OpenWindowCommand>>();
        }
        
        protected void InitShop(CommonWindow shop, int layer, UnityAction onOpen ,UnityAction onClose)
        {
            if (!_windows.ContainsKey(layer))
            {
                _windows[layer] = new List<CommonWindow>();
                _stackCommonWindowInLayer[layer] = new LStack<OpenWindowCommand>();
            }
            _windows[layer].Add(shop);

            shop.OnOpen += onOpen.Invoke;
            shop.OnClose += onClose.Invoke;   
            shop.OnClose += () =>  OnCloseWindow(shop,layer);
        }

        public void OpenWindow<T>(WindowContext context, int layer = 0) where T : CommonWindow
        {
            OpenWindow(typeof(T),context,layer);
        }
        public void OpenWindow(Type typeCommonWindow, WindowContext context, int layer = 0)
        {
            CommonWindow window = _windows[layer].Find(x => x.GetType() == typeCommonWindow);
            OpenWindow(window,context, layer);
        }
        
        protected virtual void OnCloseWindow(CommonWindow commonWindow, int layer)
        {
            RemoveWindowFromStack(commonWindow, layer);
            OpenLastWindow(layer);
        }
        
        protected abstract void OnOpenWindow(CommonWindow commonWindow, WindowContext windowContext, int layer);

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

            HideOpeningWindow(layer);
            HideWindow(window);
                
            PushToStackWindow(window,context,layer);
            OnOpenWindow(window, context, layer);
        }
        
        private void HideOpeningWindow(int layer)
        {
            if (!_stackCommonWindowInLayer.ContainsKey(layer)) return;

            if (_stackCommonWindowInLayer[layer].TryPeek(out var command))
                command.Hide();
            
        }
        internal void CloseWindow(CommonWindow window)
        {
            window.Close();
        }

        internal void HideWindow(CommonWindow window)
        {
            if(window.IsOpen)
                window.Hide();
        }
        
        private void PushToStackWindow(CommonWindow commonWindow, WindowContext windowContext , int layer)
        {
            var command = new OpenWindowCommand(this, commonWindow, windowContext);
            _stackCommonWindowInLayer[layer].Push(command);
            command.Open();
        }

        private void OpenLastWindow(int layer)
        {
            var openWindowCommand = _stackCommonWindowInLayer[layer];
            if(openWindowCommand.TryPeek(out var command) )
                command.Open();
        }

        private void RemoveWindowFromStack(CommonWindow commonWindow, int layer)
        {
            var openWindowCommand = _stackCommonWindowInLayer[layer].Last(v => v.CommonWindow == commonWindow);
            _stackCommonWindowInLayer[layer].Remove(openWindowCommand);
        }
        
        public void CloseWindowsInLayer(int layer)
        {
            while (_stackCommonWindowInLayer[layer].Count > 0)
            {
                var command = _stackCommonWindowInLayer[layer].Peek();
                CloseWindow(command.CommonWindow);
            }
            _stackCommonWindowInLayer[layer].Clear();
        }
    }
}

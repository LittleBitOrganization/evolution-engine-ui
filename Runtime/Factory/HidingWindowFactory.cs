using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LittleBit.Modules.UI.Factory
{
    public class HidingWindowFactory<TTargetLayout> : CommonWindow
        where TTargetLayout : Layout
    {
        private readonly string _layoutStyle;
        private readonly ILayoutBuilderService _layoutBuilderService;
        protected readonly IRootLayout _rootLayout;
        private TTargetLayout _layout;

        public override event Action OnOpen;
        public override event Action OnClose;
        public override event Action OnHidden;

        public HidingWindowFactory(
            string layoutStyle,
            ILayoutBuilderService layoutBuilderService,
            IRootLayout rootLayout)
        {
            _layoutStyle = layoutStyle;
            _layoutBuilderService = layoutBuilderService;
            _rootLayout = rootLayout;
            ConstructUi();
        }

        private void ConstructUi()
        {
            _layout = _layoutBuilderService.BuildLayoutOfType<TTargetLayout>(_rootLayout, _layoutStyle);
            _layout.GetComponentsInChildren<ICloseButtonLayout>(true).ToList().ForEach(b => b.AddOnClickListener(Close));

            foreach (var layoutGroup in _layout.GetComponentsInChildren<LayoutGroup>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }

            _layout.gameObject.SetActive(false);
        }

        public override void OpenShop(WindowContext windowContext)
        {
            IsOpen = true;
            Intent = windowContext.Intent;
            Show(_layout);
            IContextLayout contextLayout = _layout.GetComponentInChildren<IContextLayout>();
            
            contextLayout?.SetContext(windowContext);
            
            OnOpen?.Invoke();
        }

        public override void Close()
        {
            CloseWindow();
            OnClose?.Invoke();
            Intent = null;
        }

        public override void Hide()
        {
            CloseWindow();
            OnHidden?.Invoke();
        }
        
        private void CloseWindow()
        {
            _layout.gameObject.SetActive(false);
            IsOpen = false;
        }

        protected virtual void Show(Layout layout)
        {
            _layout.gameObject.SetActive(true);
        }
    }
}

namespace LittleBit.Modules.UI
{
    internal class OpenWindowCommand
    {
        private readonly BaseCommonUIService _commonUIService;
        private readonly CommonWindow _commonWindow;
        private readonly WindowContext _windowContext;

        public CommonWindow CommonWindow => _commonWindow;
        public WindowContext WindowContext => _windowContext;
        public OpenWindowCommand(BaseCommonUIService commonUIService, CommonWindow commonWindow,
            WindowContext windowContext)
        {
            _commonUIService = commonUIService;
            _commonWindow = commonWindow;
            _windowContext = windowContext;
        }

        internal void Open()
        {
            _commonUIService.OpenWindow(_commonWindow.GetType(),_windowContext);
        }

        internal void Hide()
        {
            _commonUIService.HideWindow(_commonWindow);
        }
    }
}
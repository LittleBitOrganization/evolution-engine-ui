namespace LittleBit.Modules.UI
{
    public class WindowContext
    {
        public ShopTrigger WindowTrigger { get; }
        public int Mode { get; set; } = 0;

        public WindowContext(ShopTrigger windowTrigger, int mode = 0)
        {
            WindowTrigger = windowTrigger;
            Mode = mode;
        }
    }
}
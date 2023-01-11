namespace LittleBit.Modules.UI
{
    public class WindowContext
    {
        public IIntent Intent { get; }
        public int Mode { get; set; } = 0;

        public WindowContext(IIntent intent, int mode = 0)
        {
            Intent = intent;
            Mode = mode;
        }
    }
    
}
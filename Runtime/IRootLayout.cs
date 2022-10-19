using LittleBit.Modules.CoreModule.MonoInterfaces;

namespace LittleBit.Modules.UI
{
    public interface IRootLayout : ILayout
    {
        public ICanvasFadeComponent CanvasFadeComponent { get; }
    }
}
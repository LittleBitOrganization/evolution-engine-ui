using UnityEngine;

namespace LittleBit.Modules.UI
{
    [CreateAssetMenu(order = 1, fileName = "Layout Styles Config", menuName = "Configs/Layout Styles Config")]
    public class LayoutStylesConfig : ScriptableObject
    {
        [SerializeField] private Style[] _styles;
        
        public Style[] Styles => _styles;
    }
}
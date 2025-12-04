using TMPro;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    public class PropertyStringComponent : IComponentData
    {
        public string Value;
        public TextMeshPro OutputText;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace MNP.Core.DataStruct
{
    public class MNProject
    {
        public string Name;
        public string Description;
        public DateTime CreateTime;
        public List<MNObject> Objects;
        public int TotalPropertyCount
        {
            get => Objects.Select(x => x.Animations.TotalPropertyCount).Sum();
        }
    }
}

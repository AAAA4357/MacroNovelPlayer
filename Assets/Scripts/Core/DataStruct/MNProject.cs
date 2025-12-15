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
        public MNResource Resource;
        public float TotalTime;
        public int TotalPropertyCount
        {
            get => Objects.Sum(x => x.Animations.TotalPropertyCount);
        }
        public int TotalStringCount
        {
            get => Objects.Sum(x => x.Animations.AnimationPropertyStringList.Count);
        }
    }
}

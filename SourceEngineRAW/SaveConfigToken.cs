using PaintDotNet;
using System;
using System.Collections.Generic;

namespace SourceEngineRAW
{
    [Serializable]
    internal class SourceEngineRAWSaveConfigToken : SaveConfigToken
    {
        public bool AllowAnyResolution { get; set; }

        public SourceEngineRAWSaveConfigToken()
        {
            AllowAnyResolution = false;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}

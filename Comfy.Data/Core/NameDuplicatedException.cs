using System;

namespace Comfy.Data.Core
{
    [Serializable]
    public class NameDuplicatedException : Exception
    {
        public NameDuplicatedException() { }
        public NameDuplicatedException(string name) : base(name) { }
    }
}

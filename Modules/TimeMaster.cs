namespace Modules
{
    using System;

    public interface TimeMaster
    {
        DateTime Today { get; }
        DateTime Now { get; }
    }
}
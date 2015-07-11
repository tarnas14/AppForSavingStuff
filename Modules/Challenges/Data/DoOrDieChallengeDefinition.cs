﻿namespace Modules.Challenges.Data
{
    using System;
    using System.Collections.Generic;
    using UI;

    public class DoOrDieChallengeDefinition
    {
        public string Name { get; set; }
        public ICollection<int> Cycle { get; set; }
        public ICollection<Challenge> Definition { get; set; }
        public DateTime ChallengeStart { get; set; }
    }
}
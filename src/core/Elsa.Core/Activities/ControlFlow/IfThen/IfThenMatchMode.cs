﻿namespace Elsa.Activities.ControlFlow
{
    public enum IfThenMatchMode
    {
        /// <summary>
        /// Yields the outcome of the first condition evaluating to true.
        /// </summary>
        MatchFirst,
        
        /// <summary>
        /// Yields the outcome of all conditions evaluating to true.
        /// </summary>
        MatchAny
    }
}
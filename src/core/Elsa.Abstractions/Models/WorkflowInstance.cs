using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Elsa.Comparers;
using Elsa.Converters;
using NodaTime;

namespace Elsa.Models
{
    public class WorkflowInstance : Entity, ITenantScope, ICorrelationScope
    {
        private HashSet<BlockingActivity> _blockingActivities = new(BlockingActivityEqualityComparer.Instance);

        public WorkflowInstance()
        {
            Variables = new Variables();
            Activities = new List<ActivityInstance>();
            ScheduledActivities = new Stack<ScheduledActivity>();
            PostScheduledActivities = new Stack<ScheduledActivity>();
            ParentActivities = new Stack<string>();
        }
        
        public string DefinitionId { get; set; } = default!;
        public string? TenantId { get; set; }
        public int Version { get; set; }
        public WorkflowStatus WorkflowStatus { get; set; }
        public string? CorrelationId { get; set; }
        public string? ContextType { get; set; }
        public string? ContextId { get; set; }
        public string? Name { get; set; }
        public Instant CreatedAt { get; set; }
        public Instant? LastExecutedAt { get; set; }
        public Instant? FinishedAt { get; set; }
        public Instant? CancelledAt { get; set; }
        public Instant? FaultedAt { get; set; }
        public Variables Variables { get; set; }
        public object? Output { get; set; }
        public ICollection<ActivityInstance> Activities { get; set; }

        public HashSet<BlockingActivity> BlockingActivities
        {
            get => _blockingActivities;
            set => _blockingActivities = new HashSet<BlockingActivity>(value, BlockingActivityEqualityComparer.Instance);
        }
        
        public WorkflowFault? Fault { get; set; }
        public Stack<ScheduledActivity> ScheduledActivities { get; set; }
        public Stack<ScheduledActivity> PostScheduledActivities { get; set; }
        public Stack<string> ParentActivities { get; set; }
    }
}
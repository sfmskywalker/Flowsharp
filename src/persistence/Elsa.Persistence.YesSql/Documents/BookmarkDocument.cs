﻿namespace Elsa.Persistence.YesSql.Documents
{
    public class BookmarkDocument : YesSqlDocument
    {
        public string BookmarkId { get; set; } = default!;
        public string? TenantId { get; set; }
        public string Hash { get; set; } = default!;
        public string Model { get; set; } = default!;
        public string ModelType { get; set; } = default!;
        public string ActivityType { get; set; } = default!;
        public string ActivityId { get; set; } = default!;
        public string WorkflowInstanceId { get; set; } = default!;
    }
}
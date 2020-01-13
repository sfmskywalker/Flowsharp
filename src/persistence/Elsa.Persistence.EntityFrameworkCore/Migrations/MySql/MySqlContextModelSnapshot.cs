﻿// <auto-generated />
using System;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Elsa.Persistence.EntityFrameworkCore.Migrations.MySql
{
    [DbContext(typeof(MySqlContext))]
    partial class MySqlContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ActivityDefinitionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ActivityId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Left")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Top")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("WorkflowDefinitionVersionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionVersionId");

                    b.ToTable("ActivityDefinitions");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ActivityInstanceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ActivityId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Output")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("State")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Type")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("WorkflowInstanceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowInstanceId");

                    b.ToTable("ActivityInstances");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.BlockingActivityEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ActivityId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ActivityType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("WorkflowInstanceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowInstanceId");

                    b.ToTable("BlockingActivities");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ConnectionDefinitionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DestinationActivityId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Outcome")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SourceActivityId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("WorkflowDefinitionVersionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowDefinitionVersionId");

                    b.ToTable("ConnectionDefinitions");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowDefinitionVersionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("DefinitionId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsLatest")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsSingleton")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Variables")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("VersionId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("WorkflowDefinitionVersions");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowInstanceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("AbortedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CorrelationId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DefinitionId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ExecutionLog")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Fault")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("FaultedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("FinishedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Input")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("InstanceId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Scopes")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("StartedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("WorkflowInstances");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ActivityDefinitionEntity", b =>
                {
                    b.HasOne("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowDefinitionVersionEntity", "WorkflowDefinitionVersion")
                        .WithMany("Activities")
                        .HasForeignKey("WorkflowDefinitionVersionId");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ActivityInstanceEntity", b =>
                {
                    b.HasOne("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowInstanceEntity", "WorkflowInstance")
                        .WithMany("Activities")
                        .HasForeignKey("WorkflowInstanceId");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.BlockingActivityEntity", b =>
                {
                    b.HasOne("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowInstanceEntity", "WorkflowInstance")
                        .WithMany("BlockingActivities")
                        .HasForeignKey("WorkflowInstanceId");
                });

            modelBuilder.Entity("Elsa.Persistence.EntityFrameworkCore.Entities.ConnectionDefinitionEntity", b =>
                {
                    b.HasOne("Elsa.Persistence.EntityFrameworkCore.Entities.WorkflowDefinitionVersionEntity", "WorkflowDefinitionVersion")
                        .WithMany("Connections")
                        .HasForeignKey("WorkflowDefinitionVersionId");
                });
#pragma warning restore 612, 618
        }
    }
}

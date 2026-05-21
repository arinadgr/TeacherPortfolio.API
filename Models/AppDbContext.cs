using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Academicresult> Academicresults { get; set; }

    public virtual DbSet<Academicyear> Academicyears { get; set; }

    public virtual DbSet<Contest> Contests { get; set; }

    public virtual DbSet<Departmentactivity> Departmentactivities { get; set; }

    public virtual DbSet<Departmentactivitytype> Departmentactivitytypes { get; set; }

    public virtual DbSet<Digitalresource> Digitalresources { get; set; }

    public virtual DbSet<Educationaltechnology> Educationaltechnologies { get; set; }

    public virtual DbSet<Eventdirection> Eventdirections { get; set; }

    public virtual DbSet<Eventformat> Eventformats { get; set; }

    public virtual DbSet<Eventlevel> Eventlevels { get; set; }

    public virtual DbSet<Experiencesharing> Experiencesharings { get; set; }

    public virtual DbSet<Experiencesharingform> Experiencesharingforms { get; set; }

    public virtual DbSet<Expertactivity> Expertactivities { get; set; }

    public virtual DbSet<Gradetype> Gradetypes { get; set; }

    public virtual DbSet<Graduatework> Graduateworks { get; set; }

    public virtual DbSet<Methodicalmaterial> Methodicalmaterials { get; set; }

    public virtual DbSet<Methodicalmaterialtype> Methodicalmaterialtypes { get; set; }

    public virtual DbSet<Participationresult> Participationresults { get; set; }

    public virtual DbSet<Qualificationcategory> Qualificationcategories { get; set; }

    public virtual DbSet<Studentachievement> Studentachievements { get; set; }

    public virtual DbSet<Studentgroup> Studentgroups { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<AcademicPerformance> AcademicPerformances { get; set; }
    public virtual DbSet<GraduationResult> GraduationResults { get; set; }
    public virtual DbSet<ElectronicResource> ElectronicResources { get; set; }
    public virtual DbSet<TeacherContest> TeacherContests { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Academicresult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("academicresults_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.Academicresults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("academicresults_academicyearid_fkey");

            entity.HasOne(d => d.Group).WithMany(p => p.Academicresults).HasConstraintName("academicresults_groupid_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.Academicresults).HasConstraintName("academicresults_subjectid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Academicresults).HasConstraintName("academicresults_teacherid_fkey");
        });

        modelBuilder.Entity<Academicyear>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("academicyears_pkey");
        });

        modelBuilder.Entity<Contest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contests_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.Contests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contests_academicyearid_fkey");

            entity.HasOne(d => d.Level).WithMany(p => p.Contests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contests_levelid_fkey");

            entity.HasOne(d => d.Result).WithMany(p => p.Contests).HasConstraintName("contests_resultid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Contests).HasConstraintName("contests_teacherid_fkey");
        });

        modelBuilder.Entity<Departmentactivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("departmentactivities_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.Departmentactivities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("departmentactivities_academicyearid_fkey");

            entity.HasOne(d => d.Activitytype).WithMany(p => p.Departmentactivities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("departmentactivities_activitytypeid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Departmentactivities).HasConstraintName("departmentactivities_teacherid_fkey");
        });

        modelBuilder.Entity<Departmentactivitytype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("departmentactivitytypes_pkey");
        });

        modelBuilder.Entity<Digitalresource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("digitalresources_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.Digitalresources).HasConstraintName("digitalresources_academicyearid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Digitalresources).HasConstraintName("digitalresources_teacherid_fkey");
        });

        modelBuilder.Entity<Educationaltechnology>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("educationaltechnologies_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Educationaltechnologies).HasConstraintName("educationaltechnologies_teacherid_fkey");
        });

        modelBuilder.Entity<Eventdirection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventdirections_pkey");
        });

        modelBuilder.Entity<Eventformat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventformats_pkey");
        });

        modelBuilder.Entity<Eventlevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventlevels_pkey");
        });

        modelBuilder.Entity<Experiencesharing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("experiencesharing_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Format).WithMany(p => p.Experiencesharings).HasConstraintName("experiencesharing_formatid_fkey");

            entity.HasOne(d => d.Level).WithMany(p => p.Experiencesharings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("experiencesharing_levelid_fkey");

            entity.HasOne(d => d.Sharingform).WithMany(p => p.Experiencesharings).HasConstraintName("experiencesharing_sharingformid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Experiencesharings).HasConstraintName("experiencesharing_teacherid_fkey");
        });

        modelBuilder.Entity<Experiencesharingform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("experiencesharingforms_pkey");
        });

        modelBuilder.Entity<Expertactivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("expertactivities_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.Expertactivities).HasConstraintName("expertactivities_academicyearid_fkey");

            entity.HasOne(d => d.Level).WithMany(p => p.Expertactivities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expertactivities_levelid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Expertactivities).HasConstraintName("expertactivities_teacherid_fkey");
        });

        modelBuilder.Entity<Gradetype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("gradetypes_pkey");
        });

        modelBuilder.Entity<Graduatework>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("graduateworks_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.Graduateworks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("graduateworks_academicyearid_fkey");

            entity.HasOne(d => d.Grade).WithMany(p => p.Graduateworks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("graduateworks_gradeid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Graduateworks).HasConstraintName("graduateworks_teacherid_fkey");
        });

        modelBuilder.Entity<Methodicalmaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("methodicalmaterials_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.Methodicalmaterials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("methodicalmaterials_academicyearid_fkey");

            entity.HasOne(d => d.Materialtype).WithMany(p => p.Methodicalmaterials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("methodicalmaterials_materialtypeid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Methodicalmaterials).HasConstraintName("methodicalmaterials_teacherid_fkey");
        });

        modelBuilder.Entity<Methodicalmaterialtype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("methodicalmaterialtypes_pkey");
        });

        modelBuilder.Entity<Participationresult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("participationresults_pkey");
        });

        modelBuilder.Entity<Qualificationcategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("qualificationcategories_pkey");
        });

        modelBuilder.Entity<Studentachievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("studentachievements_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Academicyear).WithMany(p => p.Studentachievements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("studentachievements_academicyearid_fkey");

            entity.HasOne(d => d.Direction).WithMany(p => p.Studentachievements).HasConstraintName("studentachievements_directionid_fkey");

            entity.HasOne(d => d.Level).WithMany(p => p.Studentachievements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("studentachievements_levelid_fkey");

            entity.HasOne(d => d.Result).WithMany(p => p.Studentachievements).HasConstraintName("studentachievements_resultid_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Studentachievements).HasConstraintName("studentachievements_teacherid_fkey");
        });

        modelBuilder.Entity<Studentgroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("studentgroups_pkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Studentgroups).HasConstraintName("studentgroups_teacherid_fkey");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subjects_pkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Subjects).HasConstraintName("subjects_teacherid_fkey");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("teachers_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Qualificationcategory).WithMany(p => p.Teachers).HasConstraintName("teachers_qualificationcategoryid_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.Teacher).HasConstraintName("teachers_userid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

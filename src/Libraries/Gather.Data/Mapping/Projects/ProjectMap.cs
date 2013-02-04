using System.Data.Entity.ModelConfiguration;
using Gather.Core.Domain.Projects;

namespace Gather.Data.Mapping.Projects
{
    public class ProjectMap : EntityTypeConfiguration<Project>
    {
        public ProjectMap()
        {
            ToTable("Project");
            HasKey(x => x.Id);
            Property(x => x.EmailAddress).HasMaxLength(1000);
            Property(x => x.Equipment).HasMaxLength(1000);
            Property(x => x.GettingThere).HasMaxLength(1000);
            Property(x => x.Equipment).HasMaxLength(1000);
            Property(x => x.Latitude).HasPrecision(25, 18);
            Property(x => x.Longitude).HasPrecision(25, 18);
            Property(x => x.Name).HasMaxLength(140);
            Property(x => x.Skills).HasMaxLength(1000);
            Property(x => x.Telephone).HasMaxLength(30);
            Property(x => x.VolunteerBenefits).HasMaxLength(1000);
            Property(x => x.Website).HasMaxLength(300);
            Property(x => x.TwitterProfile).HasMaxLength(1000);
            Ignore(x => x.EmailDisclosureLevel);
            Ignore(x => x.TelephoneDisclosureLevel);
            Ignore(x => x.WebsiteDisclosureLevel);
            Ignore(x => x.Status);
            Ignore(x => x.Active);
            Ignore(x => x.Deleted);

            HasMany(x => x.Categories)
                .WithMany()
                .Map(m => m.ToTable("Project_Category_Mapping"));

            HasMany(x => x.Owners)
                .WithMany()
                .Map(m => m.ToTable("Project_Owner_Mapping"));

            HasMany(x => x.Volunteers)
                .WithMany()
                .Map(m => m.ToTable("Project_Volunteer_Mapping"));
        }
    }
}
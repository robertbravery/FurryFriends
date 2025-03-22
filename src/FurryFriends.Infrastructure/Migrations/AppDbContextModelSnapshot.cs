﻿// <auto-generated />
using System;
using FurryFriends.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FurryFriends.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Breed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("SpeciesId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SpeciesId");

                    b.ToTable("Breeds", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(0);

                    b.Property<int>("ClientType")
                        .HasColumnType("int")
                        .HasColumnOrder(3);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasColumnOrder(14);

                    b.Property<DateTime?>("DeactivatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<TimeOnly?>("PreferredContactTime")
                        .HasColumnType("time")
                        .HasColumnOrder(15);

                    b.Property<int>("ReferralSource")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt")
                        .HasColumnOrder(13);

                    b.HasKey("Id");

                    b.ToTable("Clients", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Pet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<int>("BreedId")
                        .HasColumnType("int");

                    b.Property<Guid?>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("DeactivatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DietaryRestrictions")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("FavoriteActivities")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Gender")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSterilized")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVaccinated")
                        .HasColumnType("bit");

                    b.Property<string>("MedicalConditions")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SpecialNeeds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("BreedId");

                    b.HasIndex("ClientId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Pets", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Species", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Species", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.ContributorAggregate.Contributor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Contributors");
                });

            modelBuilder.Entity("FurryFriends.Core.LocationAggregate.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Countries", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.LocationAggregate.Locality", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("LocalityName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("RegionID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("RegionID");

                    b.ToTable("Localities", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.LocationAggregate.Region", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("RegionName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CountryID");

                    b.ToTable("Regions", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.PetWalkerAggregate.PetWalker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Biography")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("DailyPetWalkLimit")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<bool>("HasFirstAidCertificatation")
                        .HasColumnType("bit");

                    b.Property<bool>("HasInsurance")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("YearsOfExperience")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PetWalkers", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.PetWalkerAggregate.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PhotoType")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Photos", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.PetWalkerAggregate.ServiceArea", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("LocalityID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("LocalityID");

                    b.HasIndex("UserID");

                    b.ToTable("ServiceAreas", (string)null);
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Breed", b =>
                {
                    b.HasOne("FurryFriends.Core.ClientAggregate.Species", "Species")
                        .WithMany("Breeds")
                        .HasForeignKey("SpeciesId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Species");
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Client", b =>
                {
                    b.OwnsOne("FurryFriends.Core.ValueObjects.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("ClientId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("nvarchar(50)")
                                .HasColumnName("City")
                                .HasColumnOrder(8);

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)")
                                .HasColumnName("Country")
                                .HasColumnOrder(10);

                            b1.Property<string>("StateProvinceRegion")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)")
                                .HasColumnName("StateProvinceRegion")
                                .HasColumnOrder(9);

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasColumnName("Street")
                                .HasColumnOrder(7);

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasMaxLength(5)
                                .HasColumnType("nvarchar(5)")
                                .HasColumnName("ZipCode")
                                .HasColumnOrder(11);

                            b1.HasKey("ClientId");

                            b1.ToTable("Clients");

                            b1.WithOwner()
                                .HasForeignKey("ClientId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.Email", "Email", b1 =>
                        {
                            b1.Property<Guid>("ClientId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("EmailAddress")
                                .IsRequired()
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)")
                                .HasColumnName("Email")
                                .HasColumnOrder(4);

                            b1.HasKey("ClientId");

                            b1.ToTable("Clients");

                            b1.WithOwner()
                                .HasForeignKey("ClientId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("ClientId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("nvarchar(50)")
                                .HasColumnName("FirstName")
                                .HasColumnOrder(1);

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("nvarchar(50)")
                                .HasColumnName("LastName")
                                .HasColumnOrder(2);

                            b1.HasKey("ClientId");

                            b1.ToTable("Clients");

                            b1.WithOwner()
                                .HasForeignKey("ClientId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.PhoneNumber", "PhoneNumber", b1 =>
                        {
                            b1.Property<Guid>("ClientId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("CountryCode")
                                .IsRequired()
                                .HasMaxLength(3)
                                .HasColumnType("nvarchar(3)")
                                .HasColumnName("PhoneCountryCode")
                                .HasColumnOrder(5);

                            b1.Property<string>("Number")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("nvarchar(20)")
                                .HasColumnName("PhoneNumber")
                                .HasColumnOrder(6);

                            b1.HasKey("ClientId");

                            b1.ToTable("Clients");

                            b1.WithOwner()
                                .HasForeignKey("ClientId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Email")
                        .IsRequired();

                    b.Navigation("Name")
                        .IsRequired();

                    b.Navigation("PhoneNumber")
                        .IsRequired();
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Pet", b =>
                {
                    b.HasOne("FurryFriends.Core.ClientAggregate.Breed", "BreedType")
                        .WithMany("Pets")
                        .HasForeignKey("BreedId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FurryFriends.Core.ClientAggregate.Client", null)
                        .WithMany("ActivePets")
                        .HasForeignKey("ClientId");

                    b.HasOne("FurryFriends.Core.ClientAggregate.Client", "Owner")
                        .WithMany("Pets")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BreedType");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("FurryFriends.Core.ContributorAggregate.Contributor", b =>
                {
                    b.OwnsOne("FurryFriends.Core.ValueObjects.Name", "Name", b1 =>
                        {
                            b1.Property<int>("ContributorId")
                                .HasColumnType("int");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("FirstName");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("LastName");

                            b1.HasKey("ContributorId");

                            b1.ToTable("Contributors");

                            b1.WithOwner()
                                .HasForeignKey("ContributorId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.PhoneNumber", "PhoneNumber", b1 =>
                        {
                            b1.Property<int>("ContributorId")
                                .HasColumnType("int");

                            b1.Property<string>("CountryCode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Number")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ContributorId");

                            b1.ToTable("Contributors");

                            b1.WithOwner()
                                .HasForeignKey("ContributorId");
                        });

                    b.Navigation("Name")
                        .IsRequired();

                    b.Navigation("PhoneNumber");
                });

            modelBuilder.Entity("FurryFriends.Core.LocationAggregate.Locality", b =>
                {
                    b.HasOne("FurryFriends.Core.LocationAggregate.Region", "Region")
                        .WithMany("Localities")
                        .HasForeignKey("RegionID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Region");
                });

            modelBuilder.Entity("FurryFriends.Core.LocationAggregate.Region", b =>
                {
                    b.HasOne("FurryFriends.Core.LocationAggregate.Country", "Country")
                        .WithMany("Regions")
                        .HasForeignKey("CountryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("FurryFriends.Core.PetWalkerAggregate.PetWalker", b =>
                {
                    b.OwnsOne("FurryFriends.Core.ValueObjects.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("PetWalkerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("nvarchar(50)")
                                .HasColumnName("City");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)")
                                .HasColumnName("Country");

                            b1.Property<string>("StateProvinceRegion")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)")
                                .HasColumnName("StateProvinceRegion");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasColumnName("Street");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasMaxLength(5)
                                .HasColumnType("nvarchar(5)")
                                .HasColumnName("ZipCode");

                            b1.HasKey("PetWalkerId");

                            b1.ToTable("PetWalkers");

                            b1.WithOwner()
                                .HasForeignKey("PetWalkerId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.Email", "Email", b1 =>
                        {
                            b1.Property<Guid>("PetWalkerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("EmailAddress")
                                .IsRequired()
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)")
                                .HasColumnName("Email");

                            b1.HasKey("PetWalkerId");

                            b1.HasIndex("EmailAddress")
                                .IsUnique();

                            b1.ToTable("PetWalkers");

                            b1.WithOwner()
                                .HasForeignKey("PetWalkerId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("PetWalkerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("FirstName")
                                .HasColumnOrder(1);

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("LastName")
                                .HasColumnOrder(2);

                            b1.HasKey("PetWalkerId");

                            b1.ToTable("PetWalkers");

                            b1.WithOwner()
                                .HasForeignKey("PetWalkerId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.PhoneNumber", "PhoneNumber", b1 =>
                        {
                            b1.Property<Guid>("PetWalkerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("CountryCode")
                                .IsRequired()
                                .HasMaxLength(3)
                                .HasColumnType("nvarchar(3)")
                                .HasColumnName("PhoneCountryCode");

                            b1.Property<string>("Number")
                                .IsRequired()
                                .HasMaxLength(15)
                                .HasColumnType("nvarchar(15)")
                                .HasColumnName("PhoneNumber");

                            b1.HasKey("PetWalkerId");

                            b1.ToTable("PetWalkers");

                            b1.WithOwner()
                                .HasForeignKey("PetWalkerId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.Compensation", "Compensation", b1 =>
                        {
                            b1.Property<Guid>("PetWalkerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Currency")
                                .IsRequired()
                                .HasMaxLength(3)
                                .HasColumnType("char")
                                .HasColumnName("Currency")
                                .IsFixedLength();

                            b1.Property<decimal>("HourlyRate")
                                .HasColumnType("decimal(18,2)")
                                .HasColumnName("HourlyRate");

                            b1.HasKey("PetWalkerId");

                            b1.ToTable("PetWalkers");

                            b1.WithOwner()
                                .HasForeignKey("PetWalkerId");
                        });

                    b.OwnsOne("FurryFriends.Core.ValueObjects.GenderType", "Gender", b1 =>
                        {
                            b1.Property<Guid>("PetWalkerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Gender")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasDefaultValue(0)
                                .HasColumnName("Gender");

                            b1.HasKey("PetWalkerId");

                            b1.ToTable("PetWalkers");

                            b1.WithOwner()
                                .HasForeignKey("PetWalkerId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Compensation")
                        .IsRequired();

                    b.Navigation("Email")
                        .IsRequired();

                    b.Navigation("Gender")
                        .IsRequired();

                    b.Navigation("Name")
                        .IsRequired();

                    b.Navigation("PhoneNumber")
                        .IsRequired();
                });

            modelBuilder.Entity("FurryFriends.Core.PetWalkerAggregate.Photo", b =>
                {
                    b.HasOne("FurryFriends.Core.PetWalkerAggregate.PetWalker", "User")
                        .WithMany("Photos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("FurryFriends.Core.PetWalkerAggregate.ServiceArea", b =>
                {
                    b.HasOne("FurryFriends.Core.LocationAggregate.Locality", "Locality")
                        .WithMany()
                        .HasForeignKey("LocalityID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FurryFriends.Core.PetWalkerAggregate.PetWalker", "User")
                        .WithMany("ServiceAreas")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Locality");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Breed", b =>
                {
                    b.Navigation("Pets");
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Client", b =>
                {
                    b.Navigation("ActivePets");

                    b.Navigation("Pets");
                });

            modelBuilder.Entity("FurryFriends.Core.ClientAggregate.Species", b =>
                {
                    b.Navigation("Breeds");
                });

            modelBuilder.Entity("FurryFriends.Core.LocationAggregate.Country", b =>
                {
                    b.Navigation("Regions");
                });

            modelBuilder.Entity("FurryFriends.Core.LocationAggregate.Region", b =>
                {
                    b.Navigation("Localities");
                });

            modelBuilder.Entity("FurryFriends.Core.PetWalkerAggregate.PetWalker", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("ServiceAreas");
                });
#pragma warning restore 612, 618
        }
    }
}

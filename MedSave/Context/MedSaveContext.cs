using System.Configuration;
using MedSave.Model;
using Microsoft.EntityFrameworkCore;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace MedSave.Context;

public class MedSaveContext : DbContext
{

    private readonly IConfiguration _configuration;

    public MedSaveContext(DbContextOptions<MedSaveContext> options, IConfiguration configuration) : base(options)
    {
    }
    
    public DbSet<ActiveIngredient> ActiveIngredient { get; set; }
    public DbSet<AddressManufacturer> AddressManufacturer { get; set; }
    public DbSet<AddressStock> AddressStock { get; set; }
    public DbSet<BatchMedicine> BatchMedicine { get; set; }
    public DbSet<CategoryMedicine> CategoryMedicine { get; set; }
    public DbSet<City> City { get; set; }
    public DbSet<ContactManufacturer> ContactManufacturer { get; set; }
    public DbSet<ContactUser> ContactUser { get; set; }
    public DbSet<HealthcareProviders> HealthcareProviders { get; set; }
    public DbSet<Manufacturer> Manufacturer { get; set; }
    public DbSet<MedicineActiveIngr> MedicineActiveIngr { get; set; }
    public DbSet<MedicineDispense> MedicineDispense { get; set; }
    public DbSet<MedicinePharmForm> MedicinePharmForm { get; set; }
    public DbSet<Medicines> Medicines { get; set; }
    public DbSet<MovementType> MovementType { get; set; }
    public DbSet<Neighbourhood> Neighbourhood { get; set; }
    public DbSet<PharmaceuticalForm> PharmaceuticalForm { get; set; }
    public DbSet<ProfileUser> ProfileUser { get; set; }
    public DbSet<ProviderType> ProviderType { get; set; }
    public DbSet<RoleUser> RoleUser { get; set; }
    public DbSet<States> States { get; set; }
    public DbSet<Stock> Stock { get; set; }
    public DbSet<StockMovement> StockMovement { get; set; }
    public DbSet<UnitMeasure> UnitMeasure { get; set; }
    public DbSet<UsersSys> UsersSys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveIngredient>(entity =>
        {
            entity.ToTable("ACTIVE_INGREDIENT");

            entity.HasKey(e => e.ActIngreId)
                  .HasName("PK_ACTIVE_INGREDIENT");

            entity.Property(e => e.ActIngreId)
                .HasColumnName("ACT_INGRE_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ActIngredient)
                .HasColumnName("ACT_INGREDIENT")
                .HasColumnType("VARCHAR2(200)")
                .IsRequired();
        });

        modelBuilder.Entity<AddressManufacturer>(entity =>
        {
            entity.ToTable("ADDRESS_MANUFACTURER");

            entity.HasKey(e => e.AddressIdManufacturer)
                .HasName("PK_ADDRESS_MANUFACTURER");

            entity.Property(e => e.AddressIdManufacturer)
                .HasColumnName("ADDRESS_ID_MANUFACTURER")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Complement)
                .HasColumnName("COMPLEMENT")
                .HasColumnType("VARCHAR2(255)");

            entity.Property(e => e.NumberManu)
                .HasColumnName("NUMBER_MANU")
                .HasColumnType("NUMBER(7)")
                .IsRequired();

            entity.Property(e => e.AddressDescription)
                .HasColumnName("ADDRESS_DESCRIPTION")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.Property(e => e.Cep)
                .HasColumnName("CEP")
                .HasColumnType("NUMBER(8)")
                .IsRequired();

            entity.Property(e => e.NeighId)
                .HasColumnName("NEIGH_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.Neighbourhood)
                .WithOne()
                .HasForeignKey<AddressManufacturer>(e => e.NeighId)
                .HasConstraintName("FK_NEIGHBOURHOOD_ADDRESS_MANUFACTURER");
        });

        modelBuilder.Entity<AddressStock>(entity =>
        {
            entity.ToTable("ADDRESS_STOCK");

            entity.HasKey(e => e.AddressIdStock)
                .HasName("PK_ADDRESS_STOCK");

            entity.Property(e => e.AddressIdStock)
                .HasColumnName("ADDRESS_ID_STOCK")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Complement)
                .HasColumnName("COMPLEMENT")
                .HasColumnType("VARCHAR2(255)");

            entity.Property(e => e.NumberStock)
                .HasColumnName("NUMBER_STOCK")
                .HasColumnType("NUMBER(7)")
                .IsRequired();

            entity.Property(e => e.AddressDescription)
                .HasColumnName("ADDRESS_DESCRIPTION")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.Property(e => e.Cep)
                .HasColumnName("CEP")
                .HasColumnType("NUMBER(8)")
                .IsRequired();

            entity.Property(e => e.NeighId)
                .HasColumnName("NEIGH_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.Neighbourhood)
                .WithOne()
                .HasForeignKey<AddressStock>(e => e.NeighId)
                .HasConstraintName("FK_NEIGHBOURHOOD_ADDRESS_STOCK");
        });

        modelBuilder.Entity<BatchMedicine>(entity =>
        {
            entity.ToTable("BATCH_MEDICINE");

            entity.HasKey(e => e.BatchId)
                .HasName("PK_BATCH_MEDICINE");

            entity.Property(e => e.BatchId)
                .HasColumnName("BATCH_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.BatchNumber)
                .HasColumnName("BATCH_NUMBER")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.Property(e => e.CurrentQuantity)
                .HasColumnName("CURRENT_QUANTITY")
                .HasColumnType("NUMERIC")
                .IsRequired();

            entity.Property(e => e.ManufacturingDate)
                .HasColumnName("MANUFACTURING_DATE")
                .HasColumnType("DATE")
                .IsRequired();
            
            entity.Property(e => e.ExpirationDate)
                .HasColumnName("EXPIRATION_DATE")
                .HasColumnType("DATE")
                .IsRequired();

            entity.Property(e => e.ManufacId)
                .HasColumnName("MANUFAC_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.Manufacturer)
                .WithMany()
                .HasForeignKey(e => e.ManufacId)
                .HasConstraintName("FK_MANUFACTURER_BATCH_MEDICINE");
        });

        modelBuilder.Entity<CategoryMedicine>(entity =>
        {
            entity.ToTable("CATEGORY_MEDICINE");

            entity.HasKey(e => e.CategoryMedId)
                .HasName("PK_CATEGORY_MEDICINE");

            entity.Property(e => e.CategoryMedId)
                .HasColumnName("CATEGORY_MED_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Category)
                .HasColumnName("CATEGORY")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("CITY");

            entity.HasKey(e => e.CityId)
                .HasName("PK_CITY");

            entity.Property(e => e.CityId)
                .HasColumnName("CITY_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.NameCity)
                .HasColumnName("NAME_CITY")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();
            
            entity.Property(e => e.StateId)
                .HasColumnName("STATE_ID")
                .HasColumnType("NUMBER")
                .IsRequired();
            
            entity.HasOne(e => e.States)
                .WithMany()
                .HasForeignKey(e => e.StateId)
                .HasConstraintName("FK_STATES_CITY");
        });

        modelBuilder.Entity<ContactManufacturer>(entity =>
        {
            entity.ToTable("CONTACT_MANUFACTURER");

            entity.HasKey(e => e.ContactManuId)
                .HasName("PK_CONTACT_MANUFACTURER");

            entity.Property(e => e.ContactManuId)
                .HasColumnName("CONTACT_MANU_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.EmailManu)
                .HasColumnName("EMAIL_MANU")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.HasIndex(e => e.EmailManu)
                .IsUnique()
                .HasDatabaseName("UK_CONTACT_MANU_EMAIL");

            entity.Property(e => e.PhoneNumberManu)
                .HasColumnName("PHONE_NUMBER_MANU")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasIndex(e => e.PhoneNumberManu)
                .IsUnique()
                .HasDatabaseName("UK_CONTACT_MANU_PHONE");
        });

        modelBuilder.Entity<ContactUser>(entity =>
        {
            entity.ToTable("CONTACT_USER");

            entity.HasKey(e => e.ContactUserId)
                .HasName("PK_CONTACT_USER");

            entity.Property(e => e.ContactUserId)
                .HasColumnName("CONTACT_USER_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.EmailUser)
                .HasColumnName("EMAIL_USER")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.HasIndex(e => e.EmailUser)
                .IsUnique()
                .HasDatabaseName("UK_CONTACT_USER_EMAIL");

            entity.Property(e => e.PhoneNumberUser)
                .HasColumnName("PHONE_NUMBER_USER")
                .HasColumnType("NUMBER(15)")
                .IsRequired();

            entity.HasIndex(e => e.PhoneNumberUser)
                .IsUnique()
                .HasDatabaseName("UK_CONTACT_USER_PHONE");
        });

        modelBuilder.Entity<HealthcareProviders>(entity =>
        {
            entity.ToTable("HEALTHCARE_PROVIDERS");

            entity.HasKey(e => e.HealthcareProviderId)
                .HasName("PK_HEALTHCARE_PROVIDERS");

            entity.Property(e => e.HealthcareProviderId)
                .HasColumnName("HEALTHCARE_PROVIDER_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ProviderName)
                .HasColumnName("PROVIDER_NAME")
                .HasColumnType("VARCHAR2(30)")
                .IsRequired();

            entity.Property(e => e.HealthcareProviderName)
                .HasColumnName("HEALTHCARE_PROVIDER_NAME")
                .HasColumnType("VARCHAR2(100)")
                .IsRequired();

            entity.Property(e => e.ProviderTypeId)
                .HasColumnName("PROVIDER_TYPE_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.ProviderType)
                .WithMany()
                .HasForeignKey(e => e.ProviderTypeId)
                .HasConstraintName("FK_PROVIDER_TYPE_HEALTHCARE_PROVIDERS");
            
            entity.Property(e => e.AddressIdStock)
                .HasColumnName("ADDRESS_ID_STOCK")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.AddressStock) 
                .WithOne()
                .HasForeignKey<HealthcareProviders>(e => e.AddressIdStock)
                .HasConstraintName("FK_ADDRESS_STOCK_HEALTHCARE_PROVIDERS");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.ToTable("MANUFACTURER");

            entity.HasKey(e => e.ManufacId)
                .HasName("PK_MANUFACTURER");

            entity.Property(e => e.ManufacId)
                .HasColumnName("MANUFAC_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.NameManu)
                .HasColumnName("NAME_MANU")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.Property(e => e.Cnpj)
                .HasColumnName("CNPJ")
                .HasColumnType("NUMBER(14)")
                .IsRequired();

            entity.HasIndex(e => e.Cnpj)
                .IsUnique()
                .HasDatabaseName("UK_MANUFACTURER_CNPJ");

            entity.Property(e => e.ContactManuId)
                .HasColumnName("CONTACT_MANU_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.ContactManufacturer)
                .WithOne()
                .HasForeignKey<Manufacturer>(e => e.ContactManuId)
                .HasConstraintName("FK_CONTACT_MANUFACTURER_MANUFACTURER");

            entity.Property(e => e.AddressIdManufacturer)
                .HasColumnName("ADDRESS_ID_MANUFACTURER")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.AddressManufacturer)
                .WithOne()
                .HasForeignKey<Manufacturer>(e => e.AddressIdManufacturer)
                .HasConstraintName("FK_ADDRESS_MANUFACTURER_MANUFACTURER");
        });
        
        modelBuilder.Entity<MedicineActiveIngr>(entity =>
        {
            entity.ToTable("MEDICINE_ACTIVE_INGR");

            entity.HasKey(e => e.MedActiveIngrId)
                .HasName("PK_MEDICINE_ACTIVE_INGR");

            entity.Property(e => e.MedActiveIngrId)
                .HasColumnName("MED_ACTIVE_INGR_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MedicineId)
                .HasColumnName("MEDICINE_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.Medicines)
                .WithMany()
                .HasForeignKey(e => e.MedicineId)
                .HasConstraintName("FK_MEDICINES_MEDICINE_ACTIVE_INGR");

            entity.Property(e => e.ActIngreId)
                .HasColumnName("ACT_INGRE_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.ActiveIngredient)
                .WithMany()
                .HasForeignKey(e => e.ActIngreId)
                .HasConstraintName("FK_ACTIVE_INGREDIENT_MEDICINE_ACTIVE_INGR");
        });

        modelBuilder.Entity<MedicineDispense>(entity =>
        {
            entity.ToTable("MEDICINE_DISPENSE");

            entity.HasKey(e => e.DispensationId)
                .HasName("PK_MEDICINE_DISPENSE");

            entity.Property(e => e.DispensationId)
                .HasColumnName("DISPENSATION_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DateDispensation)
                .HasColumnName("DATE_DISPENSATION")
                .HasColumnType("DATE")
                .IsRequired();

            entity.Property(e => e.QuantityDispensed)
                .HasColumnName("QUANTITY_DISPENSED")
                .HasColumnType("NUMBER(5)")
                .IsRequired();

            entity.Property(e => e.Destination)
                .HasColumnName("DESTINATION")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.Property(e => e.Observation)
                .HasColumnName("OBSERVATION")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired(false);

            entity.Property(e => e.UserId)
                .HasColumnName("USER_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.UsersSys)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_USERS_SYS_MEDICINE_DISPENSE");
        });

        modelBuilder.Entity<MedicinePharmForm>(entity =>
        {
            entity.ToTable("MEDICINE_PHARM_FORM");

            entity.HasKey(e => e.MedPharmFormId)
                .HasName("PK_MEDICINE_PHARM_FORM");

            entity.Property(e => e.MedPharmFormId)
                .HasColumnName("MED_PHARM_FORM_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MedicineId)
                .HasColumnName("MEDICINE_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.Medicines)
                .WithMany()
                .HasForeignKey(e => e.MedicineId)
                .HasConstraintName("FK_MEDICINES_MEDICINE_PHARM_FORM");

            entity.Property(e => e.PharmFormId)
                .HasColumnName("PHARM_FORM_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.PharmaceuticalForm)
                .WithMany()
                .HasForeignKey(e => e.PharmFormId)
                .HasConstraintName("FK_PHARMACEUTICAL_FORM_MEDICINE_PHARM_FORM");
        });

        modelBuilder.Entity<Medicines>(entity =>
        {
            entity.ToTable("MEDICINES");

            entity.HasKey(e => e.MedicineId)
                .HasName("PK_MEDICINES");

            entity.Property(e => e.MedicineId)
                .HasColumnName("MEDICINE_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.NameMedication)
                .HasColumnName("NAME_MEDICATION")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.Property(e => e.AnvisaCode)
                .HasColumnName("ANVISA_CODE")
                .HasColumnType("VARCHAR2(30)")
                .IsRequired();

            entity.HasIndex(e => e.AnvisaCode)
                .IsUnique()
                .HasDatabaseName("UK_ANVISA_CODE");

            entity.Property(e => e.StatusMed)
                .HasColumnName("STATUS_MED")
                .HasColumnType("VARCHAR2(20)")
                .IsRequired();

            entity.Property(e => e.CategoryMedId)
                .HasColumnName("CATEGORY_MED_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.CategoryMedicine)
                .WithMany()
                .HasForeignKey(e => e.CategoryMedId)
                .HasConstraintName("FK_CATEGORY_MEDICINE_MEDICINES");

            entity.Property(e => e.UnitMeaId)
                .HasColumnName("UNIT_MEA_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.UnitMeasure)
                .WithMany()
                .HasForeignKey(e => e.UnitMeaId)
                .HasConstraintName("FK_UNIT_MEASURE_MEDICINES");
        });

        modelBuilder.Entity<MovementType>(entity =>
        {
            entity.ToTable("MOVEMENT_TYPE");

            entity.HasKey(e => e.MovementTypeId)
                .HasName("PK_MOVEMENT_TYPE");

            entity.Property(e => e.MovementTypeId)
                .HasColumnName("MOVEMENT_TYPE_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TypeName)
                .HasColumnName("TYPE_NAME")
                .HasColumnType("VARCHAR2(30)")
                .IsRequired();
        });

        modelBuilder.Entity<Neighbourhood>(entity =>
        {
            entity.ToTable("NEIGHBOURHOOD");

            entity.HasKey(e => e.NeighId)
                .HasName("PK_NEIGHBOURHOOD");

            entity.Property(e => e.NeighId)
                .HasColumnName("NEIGH_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.NeighName)
                .HasColumnName("NEIGH_NAME")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.Property(e => e.CityId)
                .HasColumnName("CITY_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.City)
                .WithMany()
                .HasForeignKey(e => e.CityId)
                .HasConstraintName("FK_CITY_NEIGHBOURHOOD");
        });

        modelBuilder.Entity<PharmaceuticalForm>(entity =>
        {
            entity.ToTable("PHARMACEUTICAL_FORM");

            entity.HasKey(e => e.PharmFormId)
                .HasName("PK_PHARMACEUTICAL_FORM");

            entity.Property(e => e.PharmFormId)
                .HasColumnName("PHARM_FORM_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PharmaForm)
                .HasColumnName("PHARMA_FORM")
                .HasColumnType("VARCHAR2(100)")
                .IsRequired();
        });

        modelBuilder.Entity<ProfileUser>(entity =>
        {
            entity.ToTable("PROFILE_USER");

            entity.HasKey(e => e.ProfUserId)
                .HasName("PK_PROFILE_USER");

            entity.Property(e => e.ProfUserId)
                .HasColumnName("PROF_USER_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserProfile)
                .HasColumnName("USER_PROFILE")
                .HasColumnType("VARCHAR2(50)")
                .IsRequired();
        });

        modelBuilder.Entity<ProviderType>(entity =>
        {
            entity.ToTable("PROVIDER_TYPE");

            entity.HasKey(e => e.ProviderTypeId)
                .HasName("PK_PROVIDER_TYPE");

            entity.Property(e => e.ProviderTypeId)
                .HasColumnName("PROVIDER_TYPE_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ProviderName)
                .HasColumnName("PROVIDER_NAME")
                .HasColumnType("VARCHAR2(100)")
                .IsRequired();
        });
        
        modelBuilder.Entity<RoleUser>(entity =>
        {
            entity.ToTable("ROLE_USER");

            entity.HasKey(e => e.RoleUserId)
                .HasName("PK_ROLE_USER");

            entity.Property(e => e.RoleUserId)
                .HasColumnName("ROLE_USER_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserRole)
                .HasColumnName("USER_ROLE")
                .HasColumnType("VARCHAR2(100)")
                .IsRequired();
        });

        modelBuilder.Entity<States>(entity =>
        {
            entity.ToTable("STATES");

            entity.HasKey(e => e.StateId)
                .HasName("PK_STATES");

            entity.Property(e => e.StateId)
                .HasColumnName("STATE_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.StateName)
                .HasColumnName("STATE_NAME")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("STOCK");

            entity.HasKey(e => e.StockId)
                .HasName("PK_STOCK");

            entity.Property(e => e.StockId)
                .HasColumnName("STOCK_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Quantity)
                .HasColumnName("QUANTITY")
                .HasColumnType("NUMBER(4)")
                .IsRequired();

            entity.Property(e => e.BatchId)
                .HasColumnName("BATCH_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.BatchMedicine)
                .WithOne()
                .HasForeignKey<Stock>(e => e.BatchId)
                .HasConstraintName("FK_BATCH_MEDICINE_STOCK");

            entity.Property(e => e.MedicineId)
                .HasColumnName("MEDICINE_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.Medicines)
                .WithMany()
                .HasForeignKey(e => e.MedicineId)
                .HasConstraintName("FK_MEDICINES_STOCK");

            entity.Property(e => e.HealthcareProviderId)
                .HasColumnName("HEALTHCARE_PROVIDER_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.HealthcareProviders) 
                .WithMany()
                .HasForeignKey(e => e.HealthcareProviderId)
                .HasConstraintName("FK_HEALTHCARE_PROVIDER_STOCK");
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.ToTable("STOCK_MOVEMENT");

            entity.HasKey(e => e.StockMovementId)
                .HasName("PK_STOCK_MOVEMENT");

            entity.Property(e => e.StockMovementId)
                .HasColumnName("STOCK_MOVEMENT_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.QuantityDispensed)
                .HasColumnName("QUANTITY_DISPENSED")
                .HasColumnType("NUMBER(6)")
                .IsRequired();

            entity.Property(e => e.DateMoviment)
                .HasColumnName("DATE_MOVIMENT")
                .HasColumnType("DATE")
                .IsRequired();

            entity.Property(e => e.MovementTypeId)
                .HasColumnName("MOVEMENT_TYPE_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.MovementType)
                .WithMany()
                .HasForeignKey(e => e.MovementTypeId)
                .HasConstraintName("FK_MOVEMENT_TYPE_STOCK_MOVEMENT");

            entity.Property(e => e.StockId)
                .HasColumnName("STOCK_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.Stock)
                .WithMany()
                .HasForeignKey(e => e.StockId)
                .HasConstraintName("FK_STOCK_STOCK_MOVEMENT");

            entity.Property(e => e.DispensationId)
                .HasColumnName("DISPENSATION_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.MedicineDispense)
                .WithMany()
                .HasForeignKey(e => e.DispensationId)
                .HasConstraintName("FK_MEDICINE_DISPENSE_STOCK_MOVEMENT");

            entity.Property(e => e.UserId)
                .HasColumnName("USER_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.UsersSys)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_USERS_SYS_STOCK_MOVEMENT");
        });

        modelBuilder.Entity<UnitMeasure>(entity =>
        {
            entity.ToTable("UNIT_MEASURE");

            entity.HasKey(e => e.UnitMeaId)
                .HasName("PK_UNIT_MEASURE");

            entity.Property(e => e.UnitMeaId)
                .HasColumnName("UNIT_MEA_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UnitMeasureMedicine)
                .HasColumnName("UNIT_MEASURE_MEDICINE")
                .HasColumnType("VARCHAR2(20)")
                .IsRequired();
        });

        modelBuilder.Entity<UsersSys>(entity =>
        {
            entity.ToTable("USERS_SYS");

            entity.HasKey(e => e.UserId)
                .HasName("PK_USERS_SYS");

            entity.Property(e => e.UserId)
                .HasColumnName("USER_ID")
                .HasColumnType("NUMBER")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.NameUser)
                .HasColumnName("NAME_USER")
                .HasColumnType("VARCHAR2(150)")
                .IsRequired();

            entity.Property(e => e.Email)
                .HasColumnName("EMAIL")
                .HasColumnType("VARCHAR(50)")
                .IsRequired();
            
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("UK_USERS_SYS_EMAIL");

            entity.Property(e => e.PasswordUser)
                .HasColumnName("PASSWORD_USER")
                .HasColumnType("VARCHAR2(255)")
                .IsRequired();

            entity.Property(e => e.RoleUserId)
                .HasColumnName("ROLE_USER_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.RoleUser)
                .WithMany()
                .HasForeignKey(e => e.RoleUserId)
                .HasConstraintName("FK_ROLE_USER_USERS_SYS");

            entity.Property(e => e.ProfUserId)
                .HasColumnName("PROF_USER_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.ProfileUser)
                .WithMany()
                .HasForeignKey(e => e.ProfUserId)
                .HasConstraintName("FK_PROFILE_USER_USERS_SYS");

            entity.Property(e => e.ContactUserId)
                .HasColumnName("CONTACT_USER_ID")
                .HasColumnType("NUMBER")
                .IsRequired();

            entity.HasOne(e => e.ContactUser)
                .WithOne()
                .HasForeignKey<UsersSys>(e => e.ContactUserId)
                .HasConstraintName("FK_CONTACT_USER_USERS_SYS");
        });
    }
}
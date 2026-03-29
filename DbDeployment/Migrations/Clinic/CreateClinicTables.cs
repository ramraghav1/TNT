using FluentMigrator;

namespace DbDeployment.Migrations.Clinic
{
    [Migration(202603070001)]
    public class CreateClinicTables : Migration
    {
        public override void Up()
        {
            // ──────────────────────────────────────────
            // 1. Tenants — multi-tenant root
            // ──────────────────────────────────────────
            Create.Table("tenants")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("slug").AsString(100).NotNullable().Unique()
                .WithColumn("email").AsString(200).Nullable()
                .WithColumn("phone").AsString(30).Nullable()
                .WithColumn("address").AsString(500).Nullable()
                .WithColumn("logo_url").AsString(500).Nullable()
                .WithColumn("timezone").AsString(50).NotNullable().WithDefaultValue("Asia/Kathmandu")
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            // ──────────────────────────────────────────
            // 2. Practitioners — doctors / therapists
            // ──────────────────────────────────────────
            Create.Table("practitioners")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable().ForeignKey("tenants", "id")
                .WithColumn("first_name").AsString(100).NotNullable()
                .WithColumn("last_name").AsString(100).NotNullable()
                .WithColumn("email").AsString(200).Nullable()
                .WithColumn("phone").AsString(30).Nullable()
                .WithColumn("specialization").AsString(200).Nullable()
                .WithColumn("bio").AsString(1000).Nullable()
                .WithColumn("avatar_url").AsString(500).Nullable()
                .WithColumn("role").AsString(50).NotNullable().WithDefaultValue("Practitioner")
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("idx_practitioners_tenant")
                .OnTable("practitioners").OnColumn("tenant_id");

            // ──────────────────────────────────────────
            // 3. Patients / Clients
            // ──────────────────────────────────────────
            Create.Table("patients")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable().ForeignKey("tenants", "id")
                .WithColumn("first_name").AsString(100).NotNullable()
                .WithColumn("last_name").AsString(100).NotNullable()
                .WithColumn("email").AsString(200).Nullable()
                .WithColumn("phone").AsString(30).Nullable()
                .WithColumn("date_of_birth").AsDate().Nullable()
                .WithColumn("gender").AsString(20).Nullable()
                .WithColumn("address").AsString(500).Nullable()
                .WithColumn("emergency_contact_name").AsString(200).Nullable()
                .WithColumn("emergency_contact_phone").AsString(30).Nullable()
                .WithColumn("medical_notes").AsString(4000).Nullable()
                .WithColumn("allergies").AsString(1000).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("idx_patients_tenant")
                .OnTable("patients").OnColumn("tenant_id");

            // ──────────────────────────────────────────
            // 4. Clinic Services (massage, physio, etc.)
            // ──────────────────────────────────────────
            Create.Table("clinic_services")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable().ForeignKey("tenants", "id")
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("description").AsString(1000).Nullable()
                .WithColumn("duration_minutes").AsInt32().NotNullable().WithDefaultValue(60)
                .WithColumn("price").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("currency").AsString(3).NotNullable().WithDefaultValue("NPR")
                .WithColumn("category").AsString(100).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("idx_clinic_services_tenant")
                .OnTable("clinic_services").OnColumn("tenant_id");

            // ──────────────────────────────────────────
            // 5. Appointments
            // ──────────────────────────────────────────
            Create.Table("appointments")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable().ForeignKey("tenants", "id")
                .WithColumn("patient_id").AsInt64().NotNullable().ForeignKey("patients", "id")
                .WithColumn("practitioner_id").AsInt64().NotNullable().ForeignKey("practitioners", "id")
                .WithColumn("service_id").AsInt64().NotNullable().ForeignKey("clinic_services", "id")
                .WithColumn("appointment_date").AsDate().NotNullable()
                .WithColumn("start_time").AsTime().NotNullable()
                .WithColumn("end_time").AsTime().NotNullable()
                .WithColumn("status").AsString(30).NotNullable().WithDefaultValue("Scheduled")
                .WithColumn("notes").AsString(2000).Nullable()
                .WithColumn("recurrence_rule").AsString(200).Nullable()
                .WithColumn("recurrence_group_id").AsGuid().Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("idx_appointments_tenant")
                .OnTable("appointments").OnColumn("tenant_id");
            Create.Index("idx_appointments_date")
                .OnTable("appointments").OnColumn("appointment_date");
            Create.Index("idx_appointments_practitioner")
                .OnTable("appointments").OnColumn("practitioner_id");
            Create.Index("idx_appointments_patient")
                .OnTable("appointments").OnColumn("patient_id");

            // ──────────────────────────────────────────
            // 6. Invoices
            // ──────────────────────────────────────────
            Create.Table("invoices")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable().ForeignKey("tenants", "id")
                .WithColumn("invoice_number").AsString(50).NotNullable()
                .WithColumn("patient_id").AsInt64().NotNullable().ForeignKey("patients", "id")
                .WithColumn("appointment_id").AsInt64().Nullable().ForeignKey("appointments", "id")
                .WithColumn("amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("tax").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("discount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("total").AsDecimal(18, 2).NotNullable()
                .WithColumn("currency").AsString(3).NotNullable().WithDefaultValue("NPR")
                .WithColumn("status").AsString(30).NotNullable().WithDefaultValue("Pending")
                .WithColumn("payment_method").AsString(50).Nullable()
                .WithColumn("paid_at").AsDateTime().Nullable()
                .WithColumn("notes").AsString(1000).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("idx_invoices_tenant")
                .OnTable("invoices").OnColumn("tenant_id");
            Create.Index("idx_invoices_patient")
                .OnTable("invoices").OnColumn("patient_id");
            Create.Index("uq_invoices_number_tenant")
                .OnTable("invoices")
                .OnColumn("tenant_id").Ascending()
                .OnColumn("invoice_number").Ascending()
                .WithOptions().Unique();
        }

        public override void Down()
        {
            Delete.Table("invoices");
            Delete.Table("appointments");
            Delete.Table("clinic_services");
            Delete.Table("patients");
            Delete.Table("practitioners");
            Delete.Table("tenants");
        }
    }
}

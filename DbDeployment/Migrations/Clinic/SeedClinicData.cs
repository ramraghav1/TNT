using FluentMigrator;

namespace DbDeployment.Migrations.Clinic
{
    [Migration(202603070002)]
    public class SeedClinicData : Migration
    {
        public override void Up()
        {
            // ── Tenant 1: Healing Hands Wellness ──
            Execute.Sql(@"
                INSERT INTO tenants (name, slug, email, phone, address, timezone, is_active)
                VALUES ('Healing Hands Wellness', 'healing-hands', 'admin@healinghands.com', '+977-1-4567890', 'Thamel, Kathmandu', 'Asia/Kathmandu', true);
            ");

            // ── Tenant 2: Peak Physio Clinic ──
            Execute.Sql(@"
                INSERT INTO tenants (name, slug, email, phone, address, timezone, is_active)
                VALUES ('Peak Physio Clinic', 'peak-physio', 'info@peakphysio.com', '+977-1-5551234', 'Lazimpat, Kathmandu', 'Asia/Kathmandu', true);
            ");

            // ── Tenant 3: Serenity Massage Studio ──
            Execute.Sql(@"
                INSERT INTO tenants (name, slug, email, phone, address, timezone, is_active)
                VALUES ('Serenity Massage Studio', 'serenity-massage', 'hello@serenity.com', '+977-1-6005678', 'Jhamsikhel, Lalitpur', 'Asia/Kathmandu', true);
            ");

            // ── Practitioners for Tenant 1 ──
            Execute.Sql(@"
                INSERT INTO practitioners (tenant_id, first_name, last_name, email, phone, specialization, role)
                SELECT t.id, 'Dr. Ram', 'Sharma', 'ram@healinghands.com', '+977-9851001001', 'Physiotherapy', 'Admin'
                FROM tenants t WHERE t.slug = 'healing-hands';

                INSERT INTO practitioners (tenant_id, first_name, last_name, email, phone, specialization, role)
                SELECT t.id, 'Sita', 'Gurung', 'sita@healinghands.com', '+977-9851001002', 'Massage Therapy', 'Practitioner'
                FROM tenants t WHERE t.slug = 'healing-hands';

                INSERT INTO practitioners (tenant_id, first_name, last_name, email, phone, specialization, role)
                SELECT t.id, 'Maya', 'Thapa', 'maya@healinghands.com', '+977-9851001003', 'Acupuncture', 'Receptionist'
                FROM tenants t WHERE t.slug = 'healing-hands';
            ");

            // ── Practitioners for Tenant 2 ──
            Execute.Sql(@"
                INSERT INTO practitioners (tenant_id, first_name, last_name, email, phone, specialization, role)
                SELECT t.id, 'Dr. Anil', 'Basnet', 'anil@peakphysio.com', '+977-9852002001', 'Sports Rehabilitation', 'Admin'
                FROM tenants t WHERE t.slug = 'peak-physio';

                INSERT INTO practitioners (tenant_id, first_name, last_name, email, phone, specialization, role)
                SELECT t.id, 'Binita', 'KC', 'binita@peakphysio.com', '+977-9852002002', 'Orthopedic Physio', 'Practitioner'
                FROM tenants t WHERE t.slug = 'peak-physio';
            ");

            // ── Clinic Services for Tenant 1 ──
            Execute.Sql(@"
                INSERT INTO clinic_services (tenant_id, name, description, duration_minutes, price, currency, category)
                SELECT t.id, 'Deep Tissue Massage', 'Full body deep tissue massage therapy', 60, 2500, 'NPR', 'Massage'
                FROM tenants t WHERE t.slug = 'healing-hands';

                INSERT INTO clinic_services (tenant_id, name, description, duration_minutes, price, currency, category)
                SELECT t.id, 'Physiotherapy Session', 'Standard physio assessment and treatment', 45, 3000, 'NPR', 'Physiotherapy'
                FROM tenants t WHERE t.slug = 'healing-hands';

                INSERT INTO clinic_services (tenant_id, name, description, duration_minutes, price, currency, category)
                SELECT t.id, 'Acupuncture Treatment', 'Traditional acupuncture for pain relief', 30, 2000, 'NPR', 'Acupuncture'
                FROM tenants t WHERE t.slug = 'healing-hands';

                INSERT INTO clinic_services (tenant_id, name, description, duration_minutes, price, currency, category)
                SELECT t.id, 'Hot Stone Massage', 'Relaxing hot stone massage therapy', 90, 4000, 'NPR', 'Massage'
                FROM tenants t WHERE t.slug = 'healing-hands';
            ");

            // ── Clinic Services for Tenant 2 ──
            Execute.Sql(@"
                INSERT INTO clinic_services (tenant_id, name, description, duration_minutes, price, currency, category)
                SELECT t.id, 'Sports Injury Rehab', 'Comprehensive sports injury rehabilitation', 60, 3500, 'NPR', 'Physiotherapy'
                FROM tenants t WHERE t.slug = 'peak-physio';

                INSERT INTO clinic_services (tenant_id, name, description, duration_minutes, price, currency, category)
                SELECT t.id, 'Post-Surgery Recovery', 'Post-surgical physiotherapy program', 45, 4000, 'NPR', 'Physiotherapy'
                FROM tenants t WHERE t.slug = 'peak-physio';
            ");

            // ── Patients for Tenant 1 ──
            Execute.Sql(@"
                INSERT INTO patients (tenant_id, first_name, last_name, email, phone, date_of_birth, gender, address)
                SELECT t.id, 'Hari', 'Prasad', 'hari@email.com', '+977-9841001001', '1985-03-15', 'Male', 'Baneshwor, Kathmandu'
                FROM tenants t WHERE t.slug = 'healing-hands';

                INSERT INTO patients (tenant_id, first_name, last_name, email, phone, date_of_birth, gender, address)
                SELECT t.id, 'Gita', 'Devi', 'gita@email.com', '+977-9841001002', '1990-07-22', 'Female', 'Koteshwor, Kathmandu'
                FROM tenants t WHERE t.slug = 'healing-hands';

                INSERT INTO patients (tenant_id, first_name, last_name, email, phone, date_of_birth, gender, address)
                SELECT t.id, 'Bikash', 'Shrestha', 'bikash@email.com', '+977-9841001003', '1978-11-05', 'Male', 'Patan, Lalitpur'
                FROM tenants t WHERE t.slug = 'healing-hands';
            ");

            // ── Patients for Tenant 2 ──
            Execute.Sql(@"
                INSERT INTO patients (tenant_id, first_name, last_name, email, phone, date_of_birth, gender, address)
                SELECT t.id, 'Suresh', 'Tamang', 'suresh@email.com', '+977-9842002001', '1995-01-10', 'Male', 'Bhaktapur'
                FROM tenants t WHERE t.slug = 'peak-physio';

                INSERT INTO patients (tenant_id, first_name, last_name, email, phone, date_of_birth, gender, address)
                SELECT t.id, 'Anita', 'Rai', 'anita@email.com', '+977-9842002002', '1988-09-30', 'Female', 'New Baneshwor, Kathmandu'
                FROM tenants t WHERE t.slug = 'peak-physio';
            ");

            // ── Sample Appointments for Tenant 1 ──
            Execute.Sql(@"
                INSERT INTO appointments (tenant_id, patient_id, practitioner_id, service_id, appointment_date, start_time, end_time, status, notes)
                SELECT t.id, p.id, pr.id, s.id, CURRENT_DATE + INTERVAL '1 day', '09:00', '10:00', 'Scheduled', 'First visit - back pain'
                FROM tenants t
                JOIN patients p ON p.tenant_id = t.id AND p.first_name = 'Hari'
                JOIN practitioners pr ON pr.tenant_id = t.id AND pr.first_name LIKE '%Ram%'
                JOIN clinic_services s ON s.tenant_id = t.id AND s.name = 'Physiotherapy Session'
                WHERE t.slug = 'healing-hands';

                INSERT INTO appointments (tenant_id, patient_id, practitioner_id, service_id, appointment_date, start_time, end_time, status, notes)
                SELECT t.id, p.id, pr.id, s.id, CURRENT_DATE + INTERVAL '1 day', '10:30', '11:30', 'Scheduled', 'Follow-up massage'
                FROM tenants t
                JOIN patients p ON p.tenant_id = t.id AND p.first_name = 'Gita'
                JOIN practitioners pr ON pr.tenant_id = t.id AND pr.first_name = 'Sita'
                JOIN clinic_services s ON s.tenant_id = t.id AND s.name = 'Deep Tissue Massage'
                WHERE t.slug = 'healing-hands';

                INSERT INTO appointments (tenant_id, patient_id, practitioner_id, service_id, appointment_date, start_time, end_time, status, notes)
                SELECT t.id, p.id, pr.id, s.id, CURRENT_DATE, '14:00', '14:30', 'Completed', 'Weekly acupuncture session'
                FROM tenants t
                JOIN patients p ON p.tenant_id = t.id AND p.first_name = 'Bikash'
                JOIN practitioners pr ON pr.tenant_id = t.id AND pr.first_name LIKE '%Ram%'
                JOIN clinic_services s ON s.tenant_id = t.id AND s.name = 'Acupuncture Treatment'
                WHERE t.slug = 'healing-hands';
            ");

            // ── Sample Invoices for Tenant 1 ──
            Execute.Sql(@"
                INSERT INTO invoices (tenant_id, invoice_number, patient_id, amount, tax, discount, total, currency, status, payment_method, notes)
                SELECT t.id, 'INV-HH-001', p.id, 3000, 390, 0, 3390, 'NPR', 'Paid', 'Cash', 'Physio session payment'
                FROM tenants t
                JOIN patients p ON p.tenant_id = t.id AND p.first_name = 'Hari'
                WHERE t.slug = 'healing-hands';

                INSERT INTO invoices (tenant_id, invoice_number, patient_id, amount, tax, discount, total, currency, status, notes)
                SELECT t.id, 'INV-HH-002', p.id, 2500, 325, 250, 2575, 'NPR', 'Pending', 'Massage session - discount applied'
                FROM tenants t
                JOIN patients p ON p.tenant_id = t.id AND p.first_name = 'Gita'
                WHERE t.slug = 'healing-hands';
            ");
        }

        public override void Down()
        {
            Execute.Sql("DELETE FROM invoices WHERE tenant_id IN (SELECT id FROM tenants WHERE slug IN ('healing-hands', 'peak-physio', 'serenity-massage'));");
            Execute.Sql("DELETE FROM appointments WHERE tenant_id IN (SELECT id FROM tenants WHERE slug IN ('healing-hands', 'peak-physio', 'serenity-massage'));");
            Execute.Sql("DELETE FROM patients WHERE tenant_id IN (SELECT id FROM tenants WHERE slug IN ('healing-hands', 'peak-physio', 'serenity-massage'));");
            Execute.Sql("DELETE FROM clinic_services WHERE tenant_id IN (SELECT id FROM tenants WHERE slug IN ('healing-hands', 'peak-physio', 'serenity-massage'));");
            Execute.Sql("DELETE FROM practitioners WHERE tenant_id IN (SELECT id FROM tenants WHERE slug IN ('healing-hands', 'peak-physio', 'serenity-massage'));");
            Execute.Sql("DELETE FROM tenants WHERE slug IN ('healing-hands', 'peak-physio', 'serenity-massage');");
        }
    }
}

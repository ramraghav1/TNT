using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(202602250002)]
    public class InsertNepalAdministrativeDivisions : Migration
    {
        public override void Up()
        {
            // Nepal administrative divisions:
            //   Level 1 = Province (7)
            //   Level 2 = District (77)
            //   Level 3 = Local Level (753 - Municipalities, Rural Municipalities, etc.)
            //
            // We use CTEs with RETURNING to capture auto-generated IDs
            // so child rows can reference their parent.

            Execute.Sql(@"
DO $$
DECLARE
    v_country_id BIGINT;

    -- Province IDs
    v_koshi BIGINT; v_madhesh BIGINT; v_bagmati BIGINT; v_gandaki BIGINT;
    v_lumbini BIGINT; v_karnali BIGINT; v_sudurpashchim BIGINT;

    -- District IDs (Koshi)
    v_bhojpur BIGINT; v_dhankuta BIGINT; v_ilam BIGINT; v_jhapa BIGINT;
    v_khotang BIGINT; v_morang BIGINT; v_okhaldhunga BIGINT; v_panchthar BIGINT;
    v_sankhuwasabha BIGINT; v_solukhumbu BIGINT; v_sunsari BIGINT;
    v_taplejung BIGINT; v_terhathum BIGINT; v_udayapur BIGINT;

    -- District IDs (Madhesh)
    v_bara BIGINT; v_dhanusha BIGINT; v_mahottari BIGINT; v_parsa BIGINT;
    v_rautahat BIGINT; v_saptari BIGINT; v_sarlahi BIGINT; v_siraha BIGINT;

    -- District IDs (Bagmati)
    v_bhaktapur BIGINT; v_chitwan BIGINT; v_dhading BIGINT; v_dolakha BIGINT;
    v_kathmandu BIGINT; v_kavrepalanchok BIGINT; v_lalitpur BIGINT;
    v_makwanpur BIGINT; v_nuwakot BIGINT; v_ramechhap BIGINT;
    v_rasuwa BIGINT; v_sindhuli BIGINT; v_sindhupalchok BIGINT;

    -- District IDs (Gandaki)
    v_baglung BIGINT; v_gorkha BIGINT; v_kaski BIGINT; v_lamjung BIGINT;
    v_manang BIGINT; v_mustang BIGINT; v_myagdi BIGINT; v_nawalpur BIGINT;
    v_parbat BIGINT; v_syangja BIGINT; v_tanahun BIGINT;

    -- District IDs (Lumbini)
    v_arghakhanchi BIGINT; v_banke BIGINT; v_bardiya BIGINT; v_dang BIGINT;
    v_gulmi BIGINT; v_kapilvastu BIGINT; v_palpa BIGINT;
    v_parasi BIGINT; v_pyuthan BIGINT; v_rolpa BIGINT;
    v_rupandehi BIGINT; v_rukum_east BIGINT;

    -- District IDs (Karnali)
    v_dailekh BIGINT; v_dolpa BIGINT; v_humla BIGINT; v_jajarkot BIGINT;
    v_jumla BIGINT; v_kalikot BIGINT; v_mugu BIGINT; v_rukum_west BIGINT;
    v_salyan BIGINT; v_surkhet BIGINT;

    -- District IDs (Sudurpashchim)
    v_achham BIGINT; v_baitadi BIGINT; v_bajhang BIGINT; v_bajura BIGINT;
    v_dadeldhura BIGINT; v_darchula BIGINT; v_doti BIGINT;
    v_kailali BIGINT; v_kanchanpur BIGINT;

BEGIN
    -- Get Nepal country ID
    SELECT id INTO v_country_id FROM countries WHERE iso3_code = 'NPL';
    IF v_country_id IS NULL THEN
        RAISE EXCEPTION 'Nepal not found in countries table';
    END IF;

    -- ========================================================
    -- LEVEL 1: PROVINCES
    -- ========================================================
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Koshi Province', 'P1', 1, NULL) RETURNING id INTO v_koshi;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Madhesh Province', 'P2', 1, NULL) RETURNING id INTO v_madhesh;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Bagmati Province', 'P3', 1, NULL) RETURNING id INTO v_bagmati;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Gandaki Province', 'P4', 1, NULL) RETURNING id INTO v_gandaki;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Lumbini Province', 'P5', 1, NULL) RETURNING id INTO v_lumbini;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Karnali Province', 'P6', 1, NULL) RETURNING id INTO v_karnali;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Sudurpashchim Province', 'P7', 1, NULL) RETURNING id INTO v_sudurpashchim;

    -- ========================================================
    -- LEVEL 2: DISTRICTS
    -- ========================================================

    -- Koshi Province (14 districts)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Bhojpur', 'D01', 2, v_koshi) RETURNING id INTO v_bhojpur;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Dhankuta', 'D02', 2, v_koshi) RETURNING id INTO v_dhankuta;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Ilam', 'D03', 2, v_koshi) RETURNING id INTO v_ilam;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Jhapa', 'D04', 2, v_koshi) RETURNING id INTO v_jhapa;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Khotang', 'D05', 2, v_koshi) RETURNING id INTO v_khotang;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Morang', 'D06', 2, v_koshi) RETURNING id INTO v_morang;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Okhaldhunga', 'D07', 2, v_koshi) RETURNING id INTO v_okhaldhunga;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Panchthar', 'D08', 2, v_koshi) RETURNING id INTO v_panchthar;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Sankhuwasabha', 'D09', 2, v_koshi) RETURNING id INTO v_sankhuwasabha;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Solukhumbu', 'D10', 2, v_koshi) RETURNING id INTO v_solukhumbu;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Sunsari', 'D11', 2, v_koshi) RETURNING id INTO v_sunsari;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Taplejung', 'D12', 2, v_koshi) RETURNING id INTO v_taplejung;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Terhathum', 'D13', 2, v_koshi) RETURNING id INTO v_terhathum;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Udayapur', 'D14', 2, v_koshi) RETURNING id INTO v_udayapur;

    -- Madhesh Province (8 districts)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Bara', 'D15', 2, v_madhesh) RETURNING id INTO v_bara;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Dhanusha', 'D16', 2, v_madhesh) RETURNING id INTO v_dhanusha;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Mahottari', 'D17', 2, v_madhesh) RETURNING id INTO v_mahottari;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Parsa', 'D18', 2, v_madhesh) RETURNING id INTO v_parsa;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Rautahat', 'D19', 2, v_madhesh) RETURNING id INTO v_rautahat;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Saptari', 'D20', 2, v_madhesh) RETURNING id INTO v_saptari;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Sarlahi', 'D21', 2, v_madhesh) RETURNING id INTO v_sarlahi;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Siraha', 'D22', 2, v_madhesh) RETURNING id INTO v_siraha;

    -- Bagmati Province (13 districts)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Bhaktapur', 'D23', 2, v_bagmati) RETURNING id INTO v_bhaktapur;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Chitwan', 'D24', 2, v_bagmati) RETURNING id INTO v_chitwan;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Dhading', 'D25', 2, v_bagmati) RETURNING id INTO v_dhading;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Dolakha', 'D26', 2, v_bagmati) RETURNING id INTO v_dolakha;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Kathmandu', 'D27', 2, v_bagmati) RETURNING id INTO v_kathmandu;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Kavrepalanchok', 'D28', 2, v_bagmati) RETURNING id INTO v_kavrepalanchok;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Lalitpur', 'D29', 2, v_bagmati) RETURNING id INTO v_lalitpur;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Makwanpur', 'D30', 2, v_bagmati) RETURNING id INTO v_makwanpur;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Nuwakot', 'D31', 2, v_bagmati) RETURNING id INTO v_nuwakot;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Ramechhap', 'D32', 2, v_bagmati) RETURNING id INTO v_ramechhap;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Rasuwa', 'D33', 2, v_bagmati) RETURNING id INTO v_rasuwa;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Sindhuli', 'D34', 2, v_bagmati) RETURNING id INTO v_sindhuli;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Sindhupalchok', 'D35', 2, v_bagmati) RETURNING id INTO v_sindhupalchok;

    -- Gandaki Province (11 districts)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Baglung', 'D36', 2, v_gandaki) RETURNING id INTO v_baglung;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Gorkha', 'D37', 2, v_gandaki) RETURNING id INTO v_gorkha;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Kaski', 'D38', 2, v_gandaki) RETURNING id INTO v_kaski;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Lamjung', 'D39', 2, v_gandaki) RETURNING id INTO v_lamjung;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Manang', 'D40', 2, v_gandaki) RETURNING id INTO v_manang;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Mustang', 'D41', 2, v_gandaki) RETURNING id INTO v_mustang;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Myagdi', 'D42', 2, v_gandaki) RETURNING id INTO v_myagdi;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Nawalpur', 'D43', 2, v_gandaki) RETURNING id INTO v_nawalpur;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Parbat', 'D44', 2, v_gandaki) RETURNING id INTO v_parbat;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Syangja', 'D45', 2, v_gandaki) RETURNING id INTO v_syangja;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Tanahun', 'D46', 2, v_gandaki) RETURNING id INTO v_tanahun;

    -- Lumbini Province (12 districts)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Arghakhanchi', 'D47', 2, v_lumbini) RETURNING id INTO v_arghakhanchi;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Banke', 'D48', 2, v_lumbini) RETURNING id INTO v_banke;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Bardiya', 'D49', 2, v_lumbini) RETURNING id INTO v_bardiya;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Dang', 'D50', 2, v_lumbini) RETURNING id INTO v_dang;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Gulmi', 'D51', 2, v_lumbini) RETURNING id INTO v_gulmi;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Kapilvastu', 'D52', 2, v_lumbini) RETURNING id INTO v_kapilvastu;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Palpa', 'D53', 2, v_lumbini) RETURNING id INTO v_palpa;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Parasi', 'D54', 2, v_lumbini) RETURNING id INTO v_parasi;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Pyuthan', 'D55', 2, v_lumbini) RETURNING id INTO v_pyuthan;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Rolpa', 'D56', 2, v_lumbini) RETURNING id INTO v_rolpa;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Rupandehi', 'D57', 2, v_lumbini) RETURNING id INTO v_rupandehi;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Rukum (East)', 'D58', 2, v_lumbini) RETURNING id INTO v_rukum_east;

    -- Karnali Province (10 districts)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Dailekh', 'D59', 2, v_karnali) RETURNING id INTO v_dailekh;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Dolpa', 'D60', 2, v_karnali) RETURNING id INTO v_dolpa;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Humla', 'D61', 2, v_karnali) RETURNING id INTO v_humla;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Jajarkot', 'D62', 2, v_karnali) RETURNING id INTO v_jajarkot;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Jumla', 'D63', 2, v_karnali) RETURNING id INTO v_jumla;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Kalikot', 'D64', 2, v_karnali) RETURNING id INTO v_kalikot;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Mugu', 'D65', 2, v_karnali) RETURNING id INTO v_mugu;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Rukum (West)', 'D66', 2, v_karnali) RETURNING id INTO v_rukum_west;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Salyan', 'D67', 2, v_karnali) RETURNING id INTO v_salyan;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Surkhet', 'D68', 2, v_karnali) RETURNING id INTO v_surkhet;

    -- Sudurpashchim Province (9 districts)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Achham', 'D69', 2, v_sudurpashchim) RETURNING id INTO v_achham;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Baitadi', 'D70', 2, v_sudurpashchim) RETURNING id INTO v_baitadi;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Bajhang', 'D71', 2, v_sudurpashchim) RETURNING id INTO v_bajhang;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Bajura', 'D72', 2, v_sudurpashchim) RETURNING id INTO v_bajura;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Dadeldhura', 'D73', 2, v_sudurpashchim) RETURNING id INTO v_dadeldhura;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Darchula', 'D74', 2, v_sudurpashchim) RETURNING id INTO v_darchula;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Doti', 'D75', 2, v_sudurpashchim) RETURNING id INTO v_doti;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Kailali', 'D76', 2, v_sudurpashchim) RETURNING id INTO v_kailali;
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES (v_country_id, 'Kanchanpur', 'D77', 2, v_sudurpashchim) RETURNING id INTO v_kanchanpur;

    -- ========================================================
    -- LEVEL 3: LOCAL LEVELS (Municipalities / Rural Municipalities)
    -- ========================================================
    -- Using bulk INSERT per district for performance

    -- ---- KOSHI PROVINCE ----

    -- Bhojpur (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bhojpur Municipality', 'L0101', 3, v_bhojpur),
    (v_country_id, 'Shadanand Municipality', 'L0102', 3, v_bhojpur),
    (v_country_id, 'Hatuwagadhi Rural Municipality', 'L0103', 3, v_bhojpur),
    (v_country_id, 'Ramprasad Rai Rural Municipality', 'L0104', 3, v_bhojpur),
    (v_country_id, 'Aamchok Rural Municipality', 'L0105', 3, v_bhojpur),
    (v_country_id, 'Tyamke Maiyunm Rural Municipality', 'L0106', 3, v_bhojpur),
    (v_country_id, 'Arun Rural Municipality', 'L0107', 3, v_bhojpur),
    (v_country_id, 'Pauwadungma Rural Municipality', 'L0108', 3, v_bhojpur),
    (v_country_id, 'Salpasilichho Rural Municipality', 'L0109', 3, v_bhojpur);

    -- Dhankuta (7 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Dhankuta Municipality', 'L0201', 3, v_dhankuta),
    (v_country_id, 'Pakhribas Municipality', 'L0202', 3, v_dhankuta),
    (v_country_id, 'Mahalaxmi Municipality', 'L0203', 3, v_dhankuta),
    (v_country_id, 'Sahidbhumi Municipality', 'L0204', 3, v_dhankuta),
    (v_country_id, 'Chaubise Rural Municipality', 'L0205', 3, v_dhankuta),
    (v_country_id, 'Sangurigadhi Rural Municipality', 'L0206', 3, v_dhankuta),
    (v_country_id, 'Khalsa Chhintang Sahidbhumi Rural Municipality', 'L0207', 3, v_dhankuta);

    -- Ilam (6 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Ilam Municipality', 'L0301', 3, v_ilam),
    (v_country_id, 'Deumai Municipality', 'L0302', 3, v_ilam),
    (v_country_id, 'Mai Municipality', 'L0303', 3, v_ilam),
    (v_country_id, 'Suryodaya Municipality', 'L0304', 3, v_ilam),
    (v_country_id, 'Phakphokthum Rural Municipality', 'L0305', 3, v_ilam),
    (v_country_id, 'Mangsebung Rural Municipality', 'L0306', 3, v_ilam);

    -- Jhapa (15 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Mechinagar Municipality', 'L0401', 3, v_jhapa),
    (v_country_id, 'Bhadrapur Municipality', 'L0402', 3, v_jhapa),
    (v_country_id, 'Birtamod Municipality', 'L0403', 3, v_jhapa),
    (v_country_id, 'Arjundhara Municipality', 'L0404', 3, v_jhapa),
    (v_country_id, 'Kankai Municipality', 'L0405', 3, v_jhapa),
    (v_country_id, 'Shivasatakshi Municipality', 'L0406', 3, v_jhapa),
    (v_country_id, 'Damak Municipality', 'L0407', 3, v_jhapa),
    (v_country_id, 'Gauradhaha Municipality', 'L0408', 3, v_jhapa),
    (v_country_id, 'Buddhashanti Rural Municipality', 'L0409', 3, v_jhapa),
    (v_country_id, 'Haldibari Rural Municipality', 'L0410', 3, v_jhapa),
    (v_country_id, 'Jhapa Rural Municipality', 'L0411', 3, v_jhapa),
    (v_country_id, 'Barhadashi Rural Municipality', 'L0412', 3, v_jhapa),
    (v_country_id, 'Kachankawal Rural Municipality', 'L0413', 3, v_jhapa),
    (v_country_id, 'Gaurigunj Rural Municipality', 'L0414', 3, v_jhapa),
    (v_country_id, 'Kamal Rural Municipality', 'L0415', 3, v_jhapa);

    -- Khotang (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Diktel Rupakot Majhuwagadhi Municipality', 'L0501', 3, v_khotang),
    (v_country_id, 'Halesi Tuwachung Municipality', 'L0502', 3, v_khotang),
    (v_country_id, 'Khotehang Rural Municipality', 'L0503', 3, v_khotang),
    (v_country_id, 'Diprung Chuichumma Rural Municipality', 'L0504', 3, v_khotang),
    (v_country_id, 'Ainselukhark Rural Municipality', 'L0505', 3, v_khotang),
    (v_country_id, 'Rawabesi Rural Municipality', 'L0506', 3, v_khotang),
    (v_country_id, 'Jantedhunga Rural Municipality', 'L0507', 3, v_khotang),
    (v_country_id, 'Barahapokhari Rural Municipality', 'L0508', 3, v_khotang);

    -- Morang (17 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Biratnagar Metropolitan City', 'L0601', 3, v_morang),
    (v_country_id, 'Sundar Haraicha Municipality', 'L0602', 3, v_morang),
    (v_country_id, 'Belbari Municipality', 'L0603', 3, v_morang),
    (v_country_id, 'Pathari Shanishchare Municipality', 'L0604', 3, v_morang),
    (v_country_id, 'Urlabari Municipality', 'L0605', 3, v_morang),
    (v_country_id, 'Rangeli Municipality', 'L0606', 3, v_morang),
    (v_country_id, 'Letang Municipality', 'L0607', 3, v_morang),
    (v_country_id, 'Ratuwamai Municipality', 'L0608', 3, v_morang),
    (v_country_id, 'Sunbarshi Municipality', 'L0609', 3, v_morang),
    (v_country_id, 'Jahada Rural Municipality', 'L0610', 3, v_morang),
    (v_country_id, 'Budhiganga Rural Municipality', 'L0611', 3, v_morang),
    (v_country_id, 'Kanepokhari Rural Municipality', 'L0612', 3, v_morang),
    (v_country_id, 'Gramthan Rural Municipality', 'L0613', 3, v_morang),
    (v_country_id, 'Katahari Rural Municipality', 'L0614', 3, v_morang),
    (v_country_id, 'Dhanpalthan Rural Municipality', 'L0615', 3, v_morang),
    (v_country_id, 'Kerabari Rural Municipality', 'L0616', 3, v_morang),
    (v_country_id, 'Miklajung Rural Municipality', 'L0617', 3, v_morang);

    -- Okhaldhunga (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Siddhicharan Municipality', 'L0701', 3, v_okhaldhunga),
    (v_country_id, 'Khijidemba Rural Municipality', 'L0702', 3, v_okhaldhunga),
    (v_country_id, 'Chisankhugadhi Rural Municipality', 'L0703', 3, v_okhaldhunga),
    (v_country_id, 'Molung Rural Municipality', 'L0704', 3, v_okhaldhunga),
    (v_country_id, 'Sunkoshi Rural Municipality', 'L0705', 3, v_okhaldhunga),
    (v_country_id, 'Champadevi Rural Municipality', 'L0706', 3, v_okhaldhunga),
    (v_country_id, 'Manebhanjyang Rural Municipality', 'L0707', 3, v_okhaldhunga),
    (v_country_id, 'Likhu Rural Municipality', 'L0708', 3, v_okhaldhunga);

    -- Panchthar (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Phidim Municipality', 'L0801', 3, v_panchthar),
    (v_country_id, 'Phalelung Rural Municipality', 'L0802', 3, v_panchthar),
    (v_country_id, 'Hilihang Rural Municipality', 'L0803', 3, v_panchthar),
    (v_country_id, 'Kummayak Rural Municipality', 'L0804', 3, v_panchthar),
    (v_country_id, 'Miklajung Rural Municipality', 'L0805', 3, v_panchthar),
    (v_country_id, 'Falgunanda Rural Municipality', 'L0806', 3, v_panchthar),
    (v_country_id, 'Tumbewa Rural Municipality', 'L0807', 3, v_panchthar),
    (v_country_id, 'Yangwarak Rural Municipality', 'L0808', 3, v_panchthar);

    -- Sankhuwasabha (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Khandbari Municipality', 'L0901', 3, v_sankhuwasabha),
    (v_country_id, 'Chainpur Municipality', 'L0902', 3, v_sankhuwasabha),
    (v_country_id, 'Dharmadevi Municipality', 'L0903', 3, v_sankhuwasabha),
    (v_country_id, 'Madi Municipality', 'L0904', 3, v_sankhuwasabha),
    (v_country_id, 'Panchakhapan Municipality', 'L0905', 3, v_sankhuwasabha),
    (v_country_id, 'Makalu Rural Municipality', 'L0906', 3, v_sankhuwasabha),
    (v_country_id, 'Silichong Rural Municipality', 'L0907', 3, v_sankhuwasabha),
    (v_country_id, 'Bhotkhola Rural Municipality', 'L0908', 3, v_sankhuwasabha),
    (v_country_id, 'Sabhapokhari Rural Municipality', 'L0909', 3, v_sankhuwasabha),
    (v_country_id, 'Chichila Rural Municipality', 'L0910', 3, v_sankhuwasabha);

    -- Solukhumbu (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Solududhkunda Municipality', 'L1001', 3, v_solukhumbu),
    (v_country_id, 'Dudhakaushika Rural Municipality', 'L1002', 3, v_solukhumbu),
    (v_country_id, 'Necha Salyan Rural Municipality', 'L1003', 3, v_solukhumbu),
    (v_country_id, 'Mahakulung Rural Municipality', 'L1004', 3, v_solukhumbu),
    (v_country_id, 'Sotang Rural Municipality', 'L1005', 3, v_solukhumbu),
    (v_country_id, 'Thulung Dudhkoshi Rural Municipality', 'L1006', 3, v_solukhumbu),
    (v_country_id, 'Mapya Dudhkoshi Rural Municipality', 'L1007', 3, v_solukhumbu),
    (v_country_id, 'Khumbu Pasang Lhamu Rural Municipality', 'L1008', 3, v_solukhumbu);

    -- Sunsari (12 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Dharan Sub-Metropolitan City', 'L1101', 3, v_sunsari),
    (v_country_id, 'Itahari Sub-Metropolitan City', 'L1102', 3, v_sunsari),
    (v_country_id, 'Inaruwa Municipality', 'L1103', 3, v_sunsari),
    (v_country_id, 'Duhabi Municipality', 'L1104', 3, v_sunsari),
    (v_country_id, 'Ramdhuni Municipality', 'L1105', 3, v_sunsari),
    (v_country_id, 'Barahachhetra Municipality', 'L1106', 3, v_sunsari),
    (v_country_id, 'Dewanganj Rural Municipality', 'L1107', 3, v_sunsari),
    (v_country_id, 'Gadhi Rural Municipality', 'L1108', 3, v_sunsari),
    (v_country_id, 'Barju Rural Municipality', 'L1109', 3, v_sunsari),
    (v_country_id, 'Koshi Rural Municipality', 'L1110', 3, v_sunsari),
    (v_country_id, 'Harinagara Rural Municipality', 'L1111', 3, v_sunsari),
    (v_country_id, 'Bhokraha Narsingh Rural Municipality', 'L1112', 3, v_sunsari);

    -- Taplejung (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Phungling Municipality', 'L1201', 3, v_taplejung),
    (v_country_id, 'Sirijangha Rural Municipality', 'L1202', 3, v_taplejung),
    (v_country_id, 'Aathrai Tribeni Rural Municipality', 'L1203', 3, v_taplejung),
    (v_country_id, 'Pathivara Yangwarak Rural Municipality', 'L1204', 3, v_taplejung),
    (v_country_id, 'Phaktanglung Rural Municipality', 'L1205', 3, v_taplejung),
    (v_country_id, 'Mikwakhola Rural Municipality', 'L1206', 3, v_taplejung),
    (v_country_id, 'Meringden Rural Municipality', 'L1207', 3, v_taplejung),
    (v_country_id, 'Maiwakhola Rural Municipality', 'L1208', 3, v_taplejung),
    (v_country_id, 'Sidingba Rural Municipality', 'L1209', 3, v_taplejung);

    -- Terhathum (6 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Myanglung Municipality', 'L1301', 3, v_terhathum),
    (v_country_id, 'Laligurans Municipality', 'L1302', 3, v_terhathum),
    (v_country_id, 'Aathrai Rural Municipality', 'L1303', 3, v_terhathum),
    (v_country_id, 'Chhathar Rural Municipality', 'L1304', 3, v_terhathum),
    (v_country_id, 'Phedap Rural Municipality', 'L1305', 3, v_terhathum),
    (v_country_id, 'Menchayam Rural Municipality', 'L1306', 3, v_terhathum);

    -- Udayapur (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Triyuga Municipality', 'L1401', 3, v_udayapur),
    (v_country_id, 'Katari Municipality', 'L1402', 3, v_udayapur),
    (v_country_id, 'Chaudandigadhi Municipality', 'L1403', 3, v_udayapur),
    (v_country_id, 'Belaka Municipality', 'L1404', 3, v_udayapur),
    (v_country_id, 'Udayapurgadhi Rural Municipality', 'L1405', 3, v_udayapur),
    (v_country_id, 'Rautamai Rural Municipality', 'L1406', 3, v_udayapur),
    (v_country_id, 'Tapli Rural Municipality', 'L1407', 3, v_udayapur),
    (v_country_id, 'Limchungbung Rural Municipality', 'L1408', 3, v_udayapur);

    -- ---- MADHESH PROVINCE ----

    -- Bara (16 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Kalaiya Sub-Metropolitan City', 'L1501', 3, v_bara),
    (v_country_id, 'Jeetpur Simara Sub-Metropolitan City', 'L1502', 3, v_bara),
    (v_country_id, 'Kolhabi Municipality', 'L1503', 3, v_bara),
    (v_country_id, 'Nijgadh Municipality', 'L1504', 3, v_bara),
    (v_country_id, 'Mahagadhimai Municipality', 'L1505', 3, v_bara),
    (v_country_id, 'Simraungadh Municipality', 'L1506', 3, v_bara),
    (v_country_id, 'Pacharauta Municipality', 'L1507', 3, v_bara),
    (v_country_id, 'Pheta Rural Municipality', 'L1508', 3, v_bara),
    (v_country_id, 'Bishrampur Rural Municipality', 'L1509', 3, v_bara),
    (v_country_id, 'Prasauni Rural Municipality', 'L1510', 3, v_bara),
    (v_country_id, 'Adarsha Kotwal Rural Municipality', 'L1511', 3, v_bara),
    (v_country_id, 'Karaiyamai Rural Municipality', 'L1512', 3, v_bara),
    (v_country_id, 'Devtal Rural Municipality', 'L1513', 3, v_bara),
    (v_country_id, 'Parwanipur Rural Municipality', 'L1514', 3, v_bara),
    (v_country_id, 'Baragadhi Rural Municipality', 'L1515', 3, v_bara),
    (v_country_id, 'Suwarna Rural Municipality', 'L1516', 3, v_bara);

    -- Dhanusha (16 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Janakpur Sub-Metropolitan City', 'L1601', 3, v_dhanusha),
    (v_country_id, 'Chhireshwornath Municipality', 'L1602', 3, v_dhanusha),
    (v_country_id, 'Ganeshman Charnath Municipality', 'L1603', 3, v_dhanusha),
    (v_country_id, 'Dhanushadham Municipality', 'L1604', 3, v_dhanusha),
    (v_country_id, 'Nagarain Municipality', 'L1605', 3, v_dhanusha),
    (v_country_id, 'Bideha Municipality', 'L1606', 3, v_dhanusha),
    (v_country_id, 'Mithila Municipality', 'L1607', 3, v_dhanusha),
    (v_country_id, 'Shahid Nagar Municipality', 'L1608', 3, v_dhanusha),
    (v_country_id, 'Sabaila Municipality', 'L1609', 3, v_dhanusha),
    (v_country_id, 'Kamala Municipality', 'L1610', 3, v_dhanusha),
    (v_country_id, 'Hansapur Municipality', 'L1611', 3, v_dhanusha),
    (v_country_id, 'Janaknandani Rural Municipality', 'L1612', 3, v_dhanusha),
    (v_country_id, 'Bateshwar Rural Municipality', 'L1613', 3, v_dhanusha),
    (v_country_id, 'Mukhiyapatti Musaharmiya Rural Municipality', 'L1614', 3, v_dhanusha),
    (v_country_id, 'Aaurahi Rural Municipality', 'L1615', 3, v_dhanusha),
    (v_country_id, 'Laxminya Rural Municipality', 'L1616', 3, v_dhanusha);

    -- Mahottari (15 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Jaleshwar Municipality', 'L1701', 3, v_mahottari),
    (v_country_id, 'Bardibas Municipality', 'L1702', 3, v_mahottari),
    (v_country_id, 'Gaushala Municipality', 'L1703', 3, v_mahottari),
    (v_country_id, 'Loharpatti Municipality', 'L1704', 3, v_mahottari),
    (v_country_id, 'Ramgopalpur Municipality', 'L1705', 3, v_mahottari),
    (v_country_id, 'Aurahi Municipality', 'L1706', 3, v_mahottari),
    (v_country_id, 'Balwa Municipality', 'L1707', 3, v_mahottari),
    (v_country_id, 'Manara Siswa Municipality', 'L1708', 3, v_mahottari),
    (v_country_id, 'Matihani Municipality', 'L1709', 3, v_mahottari),
    (v_country_id, 'Bhangaha Municipality', 'L1710', 3, v_mahottari),
    (v_country_id, 'Ekdanra Rural Municipality', 'L1711', 3, v_mahottari),
    (v_country_id, 'Samsi Rural Municipality', 'L1712', 3, v_mahottari),
    (v_country_id, 'Sonama Rural Municipality', 'L1713', 3, v_mahottari),
    (v_country_id, 'Mahottari Rural Municipality', 'L1714', 3, v_mahottari),
    (v_country_id, 'Pipra Rural Municipality', 'L1715', 3, v_mahottari);

    -- Parsa (14 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Birgunj Metropolitan City', 'L1801', 3, v_parsa),
    (v_country_id, 'Pokhariya Municipality', 'L1802', 3, v_parsa),
    (v_country_id, 'Bahudarmai Municipality', 'L1803', 3, v_parsa),
    (v_country_id, 'Parsagadhi Municipality', 'L1804', 3, v_parsa),
    (v_country_id, 'Bindabasini Rural Municipality', 'L1805', 3, v_parsa),
    (v_country_id, 'Chhipaharmai Rural Municipality', 'L1806', 3, v_parsa),
    (v_country_id, 'Dhobini Rural Municipality', 'L1807', 3, v_parsa),
    (v_country_id, 'Jagarnathpur Rural Municipality', 'L1808', 3, v_parsa),
    (v_country_id, 'Jirabhawani Rural Municipality', 'L1809', 3, v_parsa),
    (v_country_id, 'Kalikamai Rural Municipality', 'L1810', 3, v_parsa),
    (v_country_id, 'Pakaha Mainpur Rural Municipality', 'L1811', 3, v_parsa),
    (v_country_id, 'Paterwa Sugauli Rural Municipality', 'L1812', 3, v_parsa),
    (v_country_id, 'Sakhuwa Prasauni Rural Municipality', 'L1813', 3, v_parsa),
    (v_country_id, 'Thori Rural Municipality', 'L1814', 3, v_parsa);

    -- Rautahat (18 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Gaur Municipality', 'L1901', 3, v_rautahat),
    (v_country_id, 'Baudhimai Municipality', 'L1902', 3, v_rautahat),
    (v_country_id, 'Brindaban Municipality', 'L1903', 3, v_rautahat),
    (v_country_id, 'Chandrapur Municipality', 'L1904', 3, v_rautahat),
    (v_country_id, 'Dewahi Gonahi Municipality', 'L1905', 3, v_rautahat),
    (v_country_id, 'Garuda Municipality', 'L1906', 3, v_rautahat),
    (v_country_id, 'Gadhimai Municipality', 'L1907', 3, v_rautahat),
    (v_country_id, 'Gujara Municipality', 'L1908', 3, v_rautahat),
    (v_country_id, 'Ishanath Municipality', 'L1909', 3, v_rautahat),
    (v_country_id, 'Katahariya Municipality', 'L1910', 3, v_rautahat),
    (v_country_id, 'Madhav Narayan Municipality', 'L1911', 3, v_rautahat),
    (v_country_id, 'Maulapur Municipality', 'L1912', 3, v_rautahat),
    (v_country_id, 'Paroha Municipality', 'L1913', 3, v_rautahat),
    (v_country_id, 'Phatuwa Bijayapur Municipality', 'L1914', 3, v_rautahat),
    (v_country_id, 'Rajpur Municipality', 'L1915', 3, v_rautahat),
    (v_country_id, 'Durga Bhagwati Rural Municipality', 'L1916', 3, v_rautahat),
    (v_country_id, 'Yamunamai Rural Municipality', 'L1917', 3, v_rautahat),
    (v_country_id, 'Rajdevi Municipality', 'L1918', 3, v_rautahat);

    -- Saptari (15 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Rajbiraj Municipality', 'L2001', 3, v_saptari),
    (v_country_id, 'Kanchanrup Municipality', 'L2002', 3, v_saptari),
    (v_country_id, 'Dakneshwori Municipality', 'L2003', 3, v_saptari),
    (v_country_id, 'Bode Barsain Municipality', 'L2004', 3, v_saptari),
    (v_country_id, 'Shambhunath Municipality', 'L2005', 3, v_saptari),
    (v_country_id, 'Surunga Municipality', 'L2006', 3, v_saptari),
    (v_country_id, 'Hanumannagar Kankalini Municipality', 'L2007', 3, v_saptari),
    (v_country_id, 'Khadak Municipality', 'L2008', 3, v_saptari),
    (v_country_id, 'Saptakoshi Municipality', 'L2009', 3, v_saptari),
    (v_country_id, 'Agnisaira Krishnasawaran Rural Municipality', 'L2010', 3, v_saptari),
    (v_country_id, 'Mahadeva Rural Municipality', 'L2011', 3, v_saptari),
    (v_country_id, 'Rupani Rural Municipality', 'L2012', 3, v_saptari),
    (v_country_id, 'Tilathi Koiladi Rural Municipality', 'L2013', 3, v_saptari),
    (v_country_id, 'Rajgadh Rural Municipality', 'L2014', 3, v_saptari),
    (v_country_id, 'Bishnupur Rural Municipality', 'L2015', 3, v_saptari);

    -- Sarlahi (20 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Malangawa Municipality', 'L2101', 3, v_sarlahi),
    (v_country_id, 'Haripur Municipality', 'L2102', 3, v_sarlahi),
    (v_country_id, 'Ishworpur Municipality', 'L2103', 3, v_sarlahi),
    (v_country_id, 'Lalbandi Municipality', 'L2104', 3, v_sarlahi),
    (v_country_id, 'Barahathwa Municipality', 'L2105', 3, v_sarlahi),
    (v_country_id, 'Godaita Municipality', 'L2106', 3, v_sarlahi),
    (v_country_id, 'Bagmati Municipality', 'L2107', 3, v_sarlahi),
    (v_country_id, 'Hariwan Municipality', 'L2108', 3, v_sarlahi),
    (v_country_id, 'Balara Municipality', 'L2109', 3, v_sarlahi),
    (v_country_id, 'Kabilasi Municipality', 'L2110', 3, v_sarlahi),
    (v_country_id, 'Chandranagar Rural Municipality', 'L2111', 3, v_sarlahi),
    (v_country_id, 'Chakraghatta Rural Municipality', 'L2112', 3, v_sarlahi),
    (v_country_id, 'Dhankaul Rural Municipality', 'L2113', 3, v_sarlahi),
    (v_country_id, 'Bramhapuri Rural Municipality', 'L2114', 3, v_sarlahi),
    (v_country_id, 'Ramnagar Rural Municipality', 'L2115', 3, v_sarlahi),
    (v_country_id, 'Kaudena Rural Municipality', 'L2116', 3, v_sarlahi),
    (v_country_id, 'Bishnu Rural Municipality', 'L2117', 3, v_sarlahi),
    (v_country_id, 'Parsa Rural Municipality', 'L2118', 3, v_sarlahi),
    (v_country_id, 'Basbariya Rural Municipality', 'L2119', 3, v_sarlahi),
    (v_country_id, 'Nabil Rural Municipality', 'L2120', 3, v_sarlahi);

    -- Siraha (17 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Lahan Municipality', 'L2201', 3, v_siraha),
    (v_country_id, 'Siraha Municipality', 'L2202', 3, v_siraha),
    (v_country_id, 'Dhangadhimai Municipality', 'L2203', 3, v_siraha),
    (v_country_id, 'Mirchaiya Municipality', 'L2204', 3, v_siraha),
    (v_country_id, 'Golbazar Municipality', 'L2205', 3, v_siraha),
    (v_country_id, 'Kalyanpur Municipality', 'L2206', 3, v_siraha),
    (v_country_id, 'Karjanha Municipality', 'L2207', 3, v_siraha),
    (v_country_id, 'Sukhipur Municipality', 'L2208', 3, v_siraha),
    (v_country_id, 'Bhagwanpur Rural Municipality', 'L2209', 3, v_siraha),
    (v_country_id, 'Aurahi Rural Municipality', 'L2210', 3, v_siraha),
    (v_country_id, 'Bishnupur Rural Municipality', 'L2211', 3, v_siraha),
    (v_country_id, 'Bariyarpatti Rural Municipality', 'L2212', 3, v_siraha),
    (v_country_id, 'Lakshmipur Patari Rural Municipality', 'L2213', 3, v_siraha),
    (v_country_id, 'Naraha Rural Municipality', 'L2214', 3, v_siraha),
    (v_country_id, 'Sakhuwanankarkatti Rural Municipality', 'L2215', 3, v_siraha),
    (v_country_id, 'Arnama Rural Municipality', 'L2216', 3, v_siraha),
    (v_country_id, 'Navarajpur Rural Municipality', 'L2217', 3, v_siraha);

    -- ---- BAGMATI PROVINCE ----

    -- Bhaktapur (4 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bhaktapur Municipality', 'L2301', 3, v_bhaktapur),
    (v_country_id, 'Madhyapur Thimi Municipality', 'L2302', 3, v_bhaktapur),
    (v_country_id, 'Suryabinayak Municipality', 'L2303', 3, v_bhaktapur),
    (v_country_id, 'Changunarayan Municipality', 'L2304', 3, v_bhaktapur);

    -- Chitwan (7 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bharatpur Metropolitan City', 'L2401', 3, v_chitwan),
    (v_country_id, 'Ratnanagar Municipality', 'L2402', 3, v_chitwan),
    (v_country_id, 'Khairahani Municipality', 'L2403', 3, v_chitwan),
    (v_country_id, 'Madi Municipality', 'L2404', 3, v_chitwan),
    (v_country_id, 'Rapti Municipality', 'L2405', 3, v_chitwan),
    (v_country_id, 'Kalika Municipality', 'L2406', 3, v_chitwan),
    (v_country_id, 'Ichchhakamana Rural Municipality', 'L2407', 3, v_chitwan);

    -- Dhading (13 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Dhunibesi Municipality', 'L2501', 3, v_dhading),
    (v_country_id, 'Nilkantha Municipality', 'L2502', 3, v_dhading),
    (v_country_id, 'Benighat Rorang Rural Municipality', 'L2503', 3, v_dhading),
    (v_country_id, 'Gajuri Rural Municipality', 'L2504', 3, v_dhading),
    (v_country_id, 'Galchi Rural Municipality', 'L2505', 3, v_dhading),
    (v_country_id, 'Gangajamuna Rural Municipality', 'L2506', 3, v_dhading),
    (v_country_id, 'Jwalamukhi Rural Municipality', 'L2507', 3, v_dhading),
    (v_country_id, 'Khaniyabas Rural Municipality', 'L2508', 3, v_dhading),
    (v_country_id, 'Netrawati Dabjong Rural Municipality', 'L2509', 3, v_dhading),
    (v_country_id, 'Rubi Valley Rural Municipality', 'L2510', 3, v_dhading),
    (v_country_id, 'Siddhalek Rural Municipality', 'L2511', 3, v_dhading),
    (v_country_id, 'Thakre Rural Municipality', 'L2512', 3, v_dhading),
    (v_country_id, 'Tripurasundari Rural Municipality', 'L2513', 3, v_dhading);

    -- Dolakha (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bhimeshwar Municipality', 'L2601', 3, v_dolakha),
    (v_country_id, 'Jiri Municipality', 'L2602', 3, v_dolakha),
    (v_country_id, 'Kalinchok Rural Municipality', 'L2603', 3, v_dolakha),
    (v_country_id, 'Melung Rural Municipality', 'L2604', 3, v_dolakha),
    (v_country_id, 'Bigu Rural Municipality', 'L2605', 3, v_dolakha),
    (v_country_id, 'Gaurishankar Rural Municipality', 'L2606', 3, v_dolakha),
    (v_country_id, 'Baiteshwar Rural Municipality', 'L2607', 3, v_dolakha),
    (v_country_id, 'Sailung Rural Municipality', 'L2608', 3, v_dolakha),
    (v_country_id, 'Tamakoshi Rural Municipality', 'L2609', 3, v_dolakha);

    -- Kathmandu (11 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Kathmandu Metropolitan City', 'L2701', 3, v_kathmandu),
    (v_country_id, 'Kageshwori Manahara Municipality', 'L2702', 3, v_kathmandu),
    (v_country_id, 'Kirtipur Municipality', 'L2703', 3, v_kathmandu),
    (v_country_id, 'Gokarneshwar Municipality', 'L2704', 3, v_kathmandu),
    (v_country_id, 'Chandragiri Municipality', 'L2705', 3, v_kathmandu),
    (v_country_id, 'Tokha Municipality', 'L2706', 3, v_kathmandu),
    (v_country_id, 'Tarkeshwar Municipality', 'L2707', 3, v_kathmandu),
    (v_country_id, 'Nagarjun Municipality', 'L2708', 3, v_kathmandu),
    (v_country_id, 'Budhanilkantha Municipality', 'L2709', 3, v_kathmandu),
    (v_country_id, 'Tarakeshwar Municipality', 'L2710', 3, v_kathmandu),
    (v_country_id, 'Dakshinkali Municipality', 'L2711', 3, v_kathmandu);

    -- Kavrepalanchok (13 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Dhulikhel Municipality', 'L2801', 3, v_kavrepalanchok),
    (v_country_id, 'Banepa Municipality', 'L2802', 3, v_kavrepalanchok),
    (v_country_id, 'Panauti Municipality', 'L2803', 3, v_kavrepalanchok),
    (v_country_id, 'Panchkhal Municipality', 'L2804', 3, v_kavrepalanchok),
    (v_country_id, 'Namobuddha Municipality', 'L2805', 3, v_kavrepalanchok),
    (v_country_id, 'Mandandeupur Municipality', 'L2806', 3, v_kavrepalanchok),
    (v_country_id, 'Khanikhola Rural Municipality', 'L2807', 3, v_kavrepalanchok),
    (v_country_id, 'Chaunri Deurali Rural Municipality', 'L2808', 3, v_kavrepalanchok),
    (v_country_id, 'Temal Rural Municipality', 'L2809', 3, v_kavrepalanchok),
    (v_country_id, 'Bethanchok Rural Municipality', 'L2810', 3, v_kavrepalanchok),
    (v_country_id, 'Bhumlu Rural Municipality', 'L2811', 3, v_kavrepalanchok),
    (v_country_id, 'Mahabharat Rural Municipality', 'L2812', 3, v_kavrepalanchok),
    (v_country_id, 'Roshi Rural Municipality', 'L2813', 3, v_kavrepalanchok);

    -- Lalitpur (6 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Lalitpur Metropolitan City', 'L2901', 3, v_lalitpur),
    (v_country_id, 'Godawari Municipality', 'L2902', 3, v_lalitpur),
    (v_country_id, 'Mahalaxmi Municipality', 'L2903', 3, v_lalitpur),
    (v_country_id, 'Konjyosom Rural Municipality', 'L2904', 3, v_lalitpur),
    (v_country_id, 'Bagmati Rural Municipality', 'L2905', 3, v_lalitpur),
    (v_country_id, 'Mahankal Rural Municipality', 'L2906', 3, v_lalitpur);

    -- Makwanpur (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Hetauda Sub-Metropolitan City', 'L3001', 3, v_makwanpur),
    (v_country_id, 'Thaha Municipality', 'L3002', 3, v_makwanpur),
    (v_country_id, 'Bhimphedi Rural Municipality', 'L3003', 3, v_makwanpur),
    (v_country_id, 'Makwanpurgadhi Rural Municipality', 'L3004', 3, v_makwanpur),
    (v_country_id, 'Manahari Rural Municipality', 'L3005', 3, v_makwanpur),
    (v_country_id, 'Raksirang Rural Municipality', 'L3006', 3, v_makwanpur),
    (v_country_id, 'Bakaiya Rural Municipality', 'L3007', 3, v_makwanpur),
    (v_country_id, 'Bagmati Rural Municipality', 'L3008', 3, v_makwanpur),
    (v_country_id, 'Kailash Rural Municipality', 'L3009', 3, v_makwanpur),
    (v_country_id, 'Indrasarowar Rural Municipality', 'L3010', 3, v_makwanpur);

    -- Nuwakot (12 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bidur Municipality', 'L3101', 3, v_nuwakot),
    (v_country_id, 'Belkotgadhi Municipality', 'L3102', 3, v_nuwakot),
    (v_country_id, 'Kakani Rural Municipality', 'L3103', 3, v_nuwakot),
    (v_country_id, 'Dupcheshwar Rural Municipality', 'L3104', 3, v_nuwakot),
    (v_country_id, 'Shivapuri Rural Municipality', 'L3105', 3, v_nuwakot),
    (v_country_id, 'Kispang Rural Municipality', 'L3106', 3, v_nuwakot),
    (v_country_id, 'Myagang Rural Municipality', 'L3107', 3, v_nuwakot),
    (v_country_id, 'Tadi Rural Municipality', 'L3108', 3, v_nuwakot),
    (v_country_id, 'Likhu Rural Municipality', 'L3109', 3, v_nuwakot),
    (v_country_id, 'Suryagadhi Rural Municipality', 'L3110', 3, v_nuwakot),
    (v_country_id, 'Tarkeshwar Rural Municipality', 'L3111', 3, v_nuwakot),
    (v_country_id, 'Panchakanya Rural Municipality', 'L3112', 3, v_nuwakot);

    -- Ramechhap (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Manthali Municipality', 'L3201', 3, v_ramechhap),
    (v_country_id, 'Ramechhap Municipality', 'L3202', 3, v_ramechhap),
    (v_country_id, 'Umakunda Rural Municipality', 'L3203', 3, v_ramechhap),
    (v_country_id, 'Khandadevi Rural Municipality', 'L3204', 3, v_ramechhap),
    (v_country_id, 'Gokulganga Rural Municipality', 'L3205', 3, v_ramechhap),
    (v_country_id, 'Doramba Rural Municipality', 'L3206', 3, v_ramechhap),
    (v_country_id, 'Likhu Tamakoshi Rural Municipality', 'L3207', 3, v_ramechhap),
    (v_country_id, 'Sunapati Rural Municipality', 'L3208', 3, v_ramechhap);

    -- Rasuwa (5 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Uttargaya Rural Municipality', 'L3301', 3, v_rasuwa),
    (v_country_id, 'Kalika Rural Municipality', 'L3302', 3, v_rasuwa),
    (v_country_id, 'Naukunda Rural Municipality', 'L3303', 3, v_rasuwa),
    (v_country_id, 'Parvati Kunda Rural Municipality', 'L3304', 3, v_rasuwa),
    (v_country_id, 'Aamachhodingmo Rural Municipality', 'L3305', 3, v_rasuwa);

    -- Sindhuli (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Kamalamai Municipality', 'L3401', 3, v_sindhuli),
    (v_country_id, 'Dudhauli Municipality', 'L3402', 3, v_sindhuli),
    (v_country_id, 'Sunkoshi Rural Municipality', 'L3403', 3, v_sindhuli),
    (v_country_id, 'Golanjor Rural Municipality', 'L3404', 3, v_sindhuli),
    (v_country_id, 'Phikkal Rural Municipality', 'L3405', 3, v_sindhuli),
    (v_country_id, 'Tinpatan Rural Municipality', 'L3406', 3, v_sindhuli),
    (v_country_id, 'Marin Rural Municipality', 'L3407', 3, v_sindhuli),
    (v_country_id, 'Hariharpurgadhi Rural Municipality', 'L3408', 3, v_sindhuli),
    (v_country_id, 'Ghyanglekh Rural Municipality', 'L3409', 3, v_sindhuli);

    -- Sindhupalchok (12 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Chautara Sangachokgadhi Municipality', 'L3501', 3, v_sindhupalchok),
    (v_country_id, 'Bahrabise Municipality', 'L3502', 3, v_sindhupalchok),
    (v_country_id, 'Melamchi Municipality', 'L3503', 3, v_sindhupalchok),
    (v_country_id, 'Panchpokhari Thangpal Rural Municipality', 'L3504', 3, v_sindhupalchok),
    (v_country_id, 'Helambu Rural Municipality', 'L3505', 3, v_sindhupalchok),
    (v_country_id, 'Balefi Rural Municipality', 'L3506', 3, v_sindhupalchok),
    (v_country_id, 'Sunkoshi Rural Municipality', 'L3507', 3, v_sindhupalchok),
    (v_country_id, 'Indrawati Rural Municipality', 'L3508', 3, v_sindhupalchok),
    (v_country_id, 'Jugal Rural Municipality', 'L3509', 3, v_sindhupalchok),
    (v_country_id, 'Lisankhu Pakhar Rural Municipality', 'L3510', 3, v_sindhupalchok),
    (v_country_id, 'Tripurasundari Rural Municipality', 'L3511', 3, v_sindhupalchok),
    (v_country_id, 'Bhotekoshi Rural Municipality', 'L3512', 3, v_sindhupalchok);

    -- ---- GANDAKI PROVINCE ----

    -- Baglung (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Baglung Municipality', 'L3601', 3, v_baglung),
    (v_country_id, 'Galkot Municipality', 'L3602', 3, v_baglung),
    (v_country_id, 'Jaimini Municipality', 'L3603', 3, v_baglung),
    (v_country_id, 'Dhorpatan Municipality', 'L3604', 3, v_baglung),
    (v_country_id, 'Bareng Rural Municipality', 'L3605', 3, v_baglung),
    (v_country_id, 'Kathekhola Rural Municipality', 'L3606', 3, v_baglung),
    (v_country_id, 'Taman Khola Rural Municipality', 'L3607', 3, v_baglung),
    (v_country_id, 'Tara Khola Rural Municipality', 'L3608', 3, v_baglung),
    (v_country_id, 'Nisikhola Rural Municipality', 'L3609', 3, v_baglung),
    (v_country_id, 'Badigad Rural Municipality', 'L3610', 3, v_baglung);

    -- Gorkha (11 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Gorkha Municipality', 'L3701', 3, v_gorkha),
    (v_country_id, 'Palungtar Municipality', 'L3702', 3, v_gorkha),
    (v_country_id, 'Sulikot Rural Municipality', 'L3703', 3, v_gorkha),
    (v_country_id, 'Siranchok Rural Municipality', 'L3704', 3, v_gorkha),
    (v_country_id, 'Ajirkot Rural Municipality', 'L3705', 3, v_gorkha),
    (v_country_id, 'Aarughat Rural Municipality', 'L3706', 3, v_gorkha),
    (v_country_id, 'Dharche Rural Municipality', 'L3707', 3, v_gorkha),
    (v_country_id, 'Bhimsen Thapa Rural Municipality', 'L3708', 3, v_gorkha),
    (v_country_id, 'Shahid Lakhan Rural Municipality', 'L3709', 3, v_gorkha),
    (v_country_id, 'Chum Nubri Rural Municipality', 'L3710', 3, v_gorkha),
    (v_country_id, 'Barpak Sulikot Rural Municipality', 'L3711', 3, v_gorkha);

    -- Kaski (5 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Pokhara Metropolitan City', 'L3801', 3, v_kaski),
    (v_country_id, 'Annapurna Rural Municipality', 'L3802', 3, v_kaski),
    (v_country_id, 'Machhapuchhre Rural Municipality', 'L3803', 3, v_kaski),
    (v_country_id, 'Madi Rural Municipality', 'L3804', 3, v_kaski),
    (v_country_id, 'Rupa Rural Municipality', 'L3805', 3, v_kaski);

    -- Lamjung (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Besisahar Municipality', 'L3901', 3, v_lamjung),
    (v_country_id, 'Madhya Nepal Municipality', 'L3902', 3, v_lamjung),
    (v_country_id, 'Rainas Municipality', 'L3903', 3, v_lamjung),
    (v_country_id, 'Sundarbazar Municipality', 'L3904', 3, v_lamjung),
    (v_country_id, 'Dordi Rural Municipality', 'L3905', 3, v_lamjung),
    (v_country_id, 'Dudhpokhari Rural Municipality', 'L3906', 3, v_lamjung),
    (v_country_id, 'Kwholasothar Rural Municipality', 'L3907', 3, v_lamjung),
    (v_country_id, 'Marsyangdi Rural Municipality', 'L3908', 3, v_lamjung);

    -- Manang (4 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Chame Rural Municipality', 'L4001', 3, v_manang),
    (v_country_id, 'Narpa Bhumi Rural Municipality', 'L4002', 3, v_manang),
    (v_country_id, 'Nashong Rural Municipality', 'L4003', 3, v_manang),
    (v_country_id, 'Manang Ngishyang Rural Municipality', 'L4004', 3, v_manang);

    -- Mustang (5 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Gharapjhong Rural Municipality', 'L4101', 3, v_mustang),
    (v_country_id, 'Thasang Rural Municipality', 'L4102', 3, v_mustang),
    (v_country_id, 'Baragung Muktichhetra Rural Municipality', 'L4103', 3, v_mustang),
    (v_country_id, 'Lo-Ghekar Damodarkunda Rural Municipality', 'L4104', 3, v_mustang),
    (v_country_id, 'Dalome Rural Municipality', 'L4105', 3, v_mustang);

    -- Myagdi (6 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Beni Municipality', 'L4201', 3, v_myagdi),
    (v_country_id, 'Annapurna Rural Municipality', 'L4202', 3, v_myagdi),
    (v_country_id, 'Dhaulagiri Rural Municipality', 'L4203', 3, v_myagdi),
    (v_country_id, 'Mangala Rural Municipality', 'L4204', 3, v_myagdi),
    (v_country_id, 'Malika Rural Municipality', 'L4205', 3, v_myagdi),
    (v_country_id, 'Raghuganga Rural Municipality', 'L4206', 3, v_myagdi);

    -- Nawalpur (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Kawasoti Municipality', 'L4301', 3, v_nawalpur),
    (v_country_id, 'Gaindakot Municipality', 'L4302', 3, v_nawalpur),
    (v_country_id, 'Madhyabindu Municipality', 'L4303', 3, v_nawalpur),
    (v_country_id, 'Devchuli Municipality', 'L4304', 3, v_nawalpur),
    (v_country_id, 'Hupsekot Rural Municipality', 'L4305', 3, v_nawalpur),
    (v_country_id, 'Binayi Tribeni Rural Municipality', 'L4306', 3, v_nawalpur),
    (v_country_id, 'Bulingtar Rural Municipality', 'L4307', 3, v_nawalpur),
    (v_country_id, 'Baudikali Rural Municipality', 'L4308', 3, v_nawalpur);

    -- Parbat (7 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Kushma Municipality', 'L4401', 3, v_parbat),
    (v_country_id, 'Phalebas Municipality', 'L4402', 3, v_parbat),
    (v_country_id, 'Jaljala Rural Municipality', 'L4403', 3, v_parbat),
    (v_country_id, 'Modi Rural Municipality', 'L4404', 3, v_parbat),
    (v_country_id, 'Mahashila Rural Municipality', 'L4405', 3, v_parbat),
    (v_country_id, 'Bihadi Rural Municipality', 'L4406', 3, v_parbat),
    (v_country_id, 'Paiyun Rural Municipality', 'L4407', 3, v_parbat);

    -- Syangja (11 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Putalibazar Municipality', 'L4501', 3, v_syangja),
    (v_country_id, 'Bhirkot Municipality', 'L4502', 3, v_syangja),
    (v_country_id, 'Waling Municipality', 'L4503', 3, v_syangja),
    (v_country_id, 'Chapakot Municipality', 'L4504', 3, v_syangja),
    (v_country_id, 'Galyang Municipality', 'L4505', 3, v_syangja),
    (v_country_id, 'Harinas Rural Municipality', 'L4506', 3, v_syangja),
    (v_country_id, 'Biruwa Rural Municipality', 'L4507', 3, v_syangja),
    (v_country_id, 'Aandhikhola Rural Municipality', 'L4508', 3, v_syangja),
    (v_country_id, 'Phedikhola Rural Municipality', 'L4509', 3, v_syangja),
    (v_country_id, 'Arjunchaupari Rural Municipality', 'L4510', 3, v_syangja),
    (v_country_id, 'Kaligandaki Rural Municipality', 'L4511', 3, v_syangja);

    -- Tanahun (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bhanu Municipality', 'L4601', 3, v_tanahun),
    (v_country_id, 'Byas Municipality', 'L4602', 3, v_tanahun),
    (v_country_id, 'Shuklagandaki Municipality', 'L4603', 3, v_tanahun),
    (v_country_id, 'Bhimad Municipality', 'L4604', 3, v_tanahun),
    (v_country_id, 'Bandipur Rural Municipality', 'L4605', 3, v_tanahun),
    (v_country_id, 'Rishing Rural Municipality', 'L4606', 3, v_tanahun),
    (v_country_id, 'Ghiring Rural Municipality', 'L4607', 3, v_tanahun),
    (v_country_id, 'Myagde Rural Municipality', 'L4608', 3, v_tanahun),
    (v_country_id, 'Devghat Rural Municipality', 'L4609', 3, v_tanahun),
    (v_country_id, 'Aanbu Khaireni Rural Municipality', 'L4610', 3, v_tanahun);

    -- ---- LUMBINI PROVINCE ----

    -- Arghakhanchi (6 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Sandhikharka Municipality', 'L4701', 3, v_arghakhanchi),
    (v_country_id, 'Sitganga Municipality', 'L4702', 3, v_arghakhanchi),
    (v_country_id, 'Bhumikasthan Municipality', 'L4703', 3, v_arghakhanchi),
    (v_country_id, 'Chhatradev Rural Municipality', 'L4704', 3, v_arghakhanchi),
    (v_country_id, 'Panini Rural Municipality', 'L4705', 3, v_arghakhanchi),
    (v_country_id, 'Malarani Rural Municipality', 'L4706', 3, v_arghakhanchi);

    -- Banke (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Nepalgunj Sub-Metropolitan City', 'L4801', 3, v_banke),
    (v_country_id, 'Kohalpur Municipality', 'L4802', 3, v_banke),
    (v_country_id, 'Narainapur Rural Municipality', 'L4803', 3, v_banke),
    (v_country_id, 'Rapti Sonari Rural Municipality', 'L4804', 3, v_banke),
    (v_country_id, 'Baijanath Rural Municipality', 'L4805', 3, v_banke),
    (v_country_id, 'Khajura Rural Municipality', 'L4806', 3, v_banke),
    (v_country_id, 'Duduwa Rural Municipality', 'L4807', 3, v_banke),
    (v_country_id, 'Janaki Rural Municipality', 'L4808', 3, v_banke);

    -- Bardiya (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Gulariya Municipality', 'L4901', 3, v_bardiya),
    (v_country_id, 'Madhuwan Municipality', 'L4902', 3, v_bardiya),
    (v_country_id, 'Rajapur Municipality', 'L4903', 3, v_bardiya),
    (v_country_id, 'Thakurbaba Municipality', 'L4904', 3, v_bardiya),
    (v_country_id, 'Bansgadhi Municipality', 'L4905', 3, v_bardiya),
    (v_country_id, 'Barbardiya Municipality', 'L4906', 3, v_bardiya),
    (v_country_id, 'Badhaiyatal Rural Municipality', 'L4907', 3, v_bardiya),
    (v_country_id, 'Geruwa Rural Municipality', 'L4908', 3, v_bardiya);

    -- Dang (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Tulsipur Sub-Metropolitan City', 'L5001', 3, v_dang),
    (v_country_id, 'Ghorahi Sub-Metropolitan City', 'L5002', 3, v_dang),
    (v_country_id, 'Lamahi Municipality', 'L5003', 3, v_dang),
    (v_country_id, 'Gadhawa Rural Municipality', 'L5004', 3, v_dang),
    (v_country_id, 'Rajpur Rural Municipality', 'L5005', 3, v_dang),
    (v_country_id, 'Shantinagar Rural Municipality', 'L5006', 3, v_dang),
    (v_country_id, 'Rapti Rural Municipality', 'L5007', 3, v_dang),
    (v_country_id, 'Banglachuli Rural Municipality', 'L5008', 3, v_dang),
    (v_country_id, 'Dangisharan Rural Municipality', 'L5009', 3, v_dang),
    (v_country_id, 'Babai Rural Municipality', 'L5010', 3, v_dang);

    -- Gulmi (12 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Musikot Municipality', 'L5101', 3, v_gulmi),
    (v_country_id, 'Resunga Municipality', 'L5102', 3, v_gulmi),
    (v_country_id, 'Isma Rural Municipality', 'L5103', 3, v_gulmi),
    (v_country_id, 'Kaligandaki Rural Municipality', 'L5104', 3, v_gulmi),
    (v_country_id, 'Gulmi Darbar Rural Municipality', 'L5105', 3, v_gulmi),
    (v_country_id, 'Satyawati Rural Municipality', 'L5106', 3, v_gulmi),
    (v_country_id, 'Chandrakot Rural Municipality', 'L5107', 3, v_gulmi),
    (v_country_id, 'Ruru Rural Municipality', 'L5108', 3, v_gulmi),
    (v_country_id, 'Madane Rural Municipality', 'L5109', 3, v_gulmi),
    (v_country_id, 'Malika Rural Municipality', 'L5110', 3, v_gulmi),
    (v_country_id, 'Chhatrakot Rural Municipality', 'L5111', 3, v_gulmi),
    (v_country_id, 'Dhurkot Rural Municipality', 'L5112', 3, v_gulmi);

    -- Kapilvastu (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Kapilvastu Municipality', 'L5201', 3, v_kapilvastu),
    (v_country_id, 'Buddhabhumi Municipality', 'L5202', 3, v_kapilvastu),
    (v_country_id, 'Shivaraj Municipality', 'L5203', 3, v_kapilvastu),
    (v_country_id, 'Maharajgunj Municipality', 'L5204', 3, v_kapilvastu),
    (v_country_id, 'Krishnanagar Municipality', 'L5205', 3, v_kapilvastu),
    (v_country_id, 'Banganga Municipality', 'L5206', 3, v_kapilvastu),
    (v_country_id, 'Mayadevi Rural Municipality', 'L5207', 3, v_kapilvastu),
    (v_country_id, 'Yashodhara Rural Municipality', 'L5208', 3, v_kapilvastu),
    (v_country_id, 'Suddhodhan Rural Municipality', 'L5209', 3, v_kapilvastu),
    (v_country_id, 'Bijaynagar Rural Municipality', 'L5210', 3, v_kapilvastu);

    -- Palpa (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Tansen Municipality', 'L5301', 3, v_palpa),
    (v_country_id, 'Rampur Municipality', 'L5302', 3, v_palpa),
    (v_country_id, 'Nisdi Rural Municipality', 'L5303', 3, v_palpa),
    (v_country_id, 'Purbakhola Rural Municipality', 'L5304', 3, v_palpa),
    (v_country_id, 'Rambha Rural Municipality', 'L5305', 3, v_palpa),
    (v_country_id, 'Mathagadhi Rural Municipality', 'L5306', 3, v_palpa),
    (v_country_id, 'Tinau Rural Municipality', 'L5307', 3, v_palpa),
    (v_country_id, 'Bagnaskali Rural Municipality', 'L5308', 3, v_palpa),
    (v_country_id, 'Rainadevi Chhahara Rural Municipality', 'L5309', 3, v_palpa),
    (v_country_id, 'Ribdikot Rural Municipality', 'L5310', 3, v_palpa);

    -- Parasi (7 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bardaghat Municipality', 'L5401', 3, v_parasi),
    (v_country_id, 'Ramgram Municipality', 'L5402', 3, v_parasi),
    (v_country_id, 'Sunwal Municipality', 'L5403', 3, v_parasi),
    (v_country_id, 'Susta Rural Municipality', 'L5404', 3, v_parasi),
    (v_country_id, 'Palhinandan Rural Municipality', 'L5405', 3, v_parasi),
    (v_country_id, 'Pratappur Rural Municipality', 'L5406', 3, v_parasi),
    (v_country_id, 'Sarawal Rural Municipality', 'L5407', 3, v_parasi);

    -- Pyuthan (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Pyuthan Municipality', 'L5501', 3, v_pyuthan),
    (v_country_id, 'Swargadwari Municipality', 'L5502', 3, v_pyuthan),
    (v_country_id, 'Gaumukhi Rural Municipality', 'L5503', 3, v_pyuthan),
    (v_country_id, 'Mandavi Rural Municipality', 'L5504', 3, v_pyuthan),
    (v_country_id, 'Mallarani Rural Municipality', 'L5505', 3, v_pyuthan),
    (v_country_id, 'Naubahini Rural Municipality', 'L5506', 3, v_pyuthan),
    (v_country_id, 'Jhimruk Rural Municipality', 'L5507', 3, v_pyuthan),
    (v_country_id, 'Sarumarani Rural Municipality', 'L5508', 3, v_pyuthan),
    (v_country_id, 'Airawati Rural Municipality', 'L5509', 3, v_pyuthan);

    -- Rolpa (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Rolpa Municipality', 'L5601', 3, v_rolpa),
    (v_country_id, 'Triveni Rural Municipality', 'L5602', 3, v_rolpa),
    (v_country_id, 'Runtigadhi Rural Municipality', 'L5603', 3, v_rolpa),
    (v_country_id, 'Lungri Rural Municipality', 'L5604', 3, v_rolpa),
    (v_country_id, 'Sunchhahari Rural Municipality', 'L5605', 3, v_rolpa),
    (v_country_id, 'Sunil Smriti Rural Municipality', 'L5606', 3, v_rolpa),
    (v_country_id, 'Thawang Rural Municipality', 'L5607', 3, v_rolpa),
    (v_country_id, 'Madi Rural Municipality', 'L5608', 3, v_rolpa),
    (v_country_id, 'Gangadev Rural Municipality', 'L5609', 3, v_rolpa),
    (v_country_id, 'Pariwartan Rural Municipality', 'L5610', 3, v_rolpa);

    -- Rupandehi (16 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Butwal Sub-Metropolitan City', 'L5701', 3, v_rupandehi),
    (v_country_id, 'Siddharthanagar Municipality', 'L5702', 3, v_rupandehi),
    (v_country_id, 'Tilottama Municipality', 'L5703', 3, v_rupandehi),
    (v_country_id, 'Lumbini Sanskritik Municipality', 'L5704', 3, v_rupandehi),
    (v_country_id, 'Devdaha Municipality', 'L5705', 3, v_rupandehi),
    (v_country_id, 'Sainamaina Municipality', 'L5706', 3, v_rupandehi),
    (v_country_id, 'Rohini Rural Municipality', 'L5707', 3, v_rupandehi),
    (v_country_id, 'Marchawari Rural Municipality', 'L5708', 3, v_rupandehi),
    (v_country_id, 'Siyari Rural Municipality', 'L5709', 3, v_rupandehi),
    (v_country_id, 'Gaidahawa Rural Municipality', 'L5710', 3, v_rupandehi),
    (v_country_id, 'Kanchan Rural Municipality', 'L5711', 3, v_rupandehi),
    (v_country_id, 'Suddhodhan Rural Municipality', 'L5712', 3, v_rupandehi),
    (v_country_id, 'Omsatiya Rural Municipality', 'L5713', 3, v_rupandehi),
    (v_country_id, 'Mayadevi Rural Municipality', 'L5714', 3, v_rupandehi),
    (v_country_id, 'Kotahimai Rural Municipality', 'L5715', 3, v_rupandehi),
    (v_country_id, 'Sammarimai Rural Municipality', 'L5716', 3, v_rupandehi);

    -- Rukum East (3 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bhume Rural Municipality', 'L5801', 3, v_rukum_east),
    (v_country_id, 'Putha Uttarganga Rural Municipality', 'L5802', 3, v_rukum_east),
    (v_country_id, 'Sisne Rural Municipality', 'L5803', 3, v_rukum_east);

    -- ---- KARNALI PROVINCE ----

    -- Dailekh (11 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Narayan Municipality', 'L5901', 3, v_dailekh),
    (v_country_id, 'Dullu Municipality', 'L5902', 3, v_dailekh),
    (v_country_id, 'Chamunda Bindrasaini Municipality', 'L5903', 3, v_dailekh),
    (v_country_id, 'Aathbis Municipality', 'L5904', 3, v_dailekh),
    (v_country_id, 'Bhagawatimai Rural Municipality', 'L5905', 3, v_dailekh),
    (v_country_id, 'Bhairabi Rural Municipality', 'L5906', 3, v_dailekh),
    (v_country_id, 'Dungeshwar Rural Municipality', 'L5907', 3, v_dailekh),
    (v_country_id, 'Gurans Rural Municipality', 'L5908', 3, v_dailekh),
    (v_country_id, 'Mahabu Rural Municipality', 'L5909', 3, v_dailekh),
    (v_country_id, 'Naumule Rural Municipality', 'L5910', 3, v_dailekh),
    (v_country_id, 'Thantikandh Rural Municipality', 'L5911', 3, v_dailekh);

    -- Dolpa (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Thuli Bheri Municipality', 'L6001', 3, v_dolpa),
    (v_country_id, 'Tripurasundari Municipality', 'L6002', 3, v_dolpa),
    (v_country_id, 'Dolpo Buddha Rural Municipality', 'L6003', 3, v_dolpa),
    (v_country_id, 'She Phoksundo Rural Municipality', 'L6004', 3, v_dolpa),
    (v_country_id, 'Jagadulla Rural Municipality', 'L6005', 3, v_dolpa),
    (v_country_id, 'Kaike Rural Municipality', 'L6006', 3, v_dolpa),
    (v_country_id, 'Mudkechula Rural Municipality', 'L6007', 3, v_dolpa),
    (v_country_id, 'Chharka Tangsong Rural Municipality', 'L6008', 3, v_dolpa);

    -- Humla (7 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Simkot Rural Municipality', 'L6101', 3, v_humla),
    (v_country_id, 'Namkha Rural Municipality', 'L6102', 3, v_humla),
    (v_country_id, 'Kharpunath Rural Municipality', 'L6103', 3, v_humla),
    (v_country_id, 'Sarkegad Rural Municipality', 'L6104', 3, v_humla),
    (v_country_id, 'Chankheli Rural Municipality', 'L6105', 3, v_humla),
    (v_country_id, 'Adanchuli Rural Municipality', 'L6106', 3, v_humla),
    (v_country_id, 'Tanjakot Rural Municipality', 'L6107', 3, v_humla);

    -- Jajarkot (7 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bheri Municipality', 'L6201', 3, v_jajarkot),
    (v_country_id, 'Chhedagad Municipality', 'L6202', 3, v_jajarkot),
    (v_country_id, 'Nalgad Municipality', 'L6203', 3, v_jajarkot),
    (v_country_id, 'Junichande Rural Municipality', 'L6204', 3, v_jajarkot),
    (v_country_id, 'Kuse Rural Municipality', 'L6205', 3, v_jajarkot),
    (v_country_id, 'Barekot Rural Municipality', 'L6206', 3, v_jajarkot),
    (v_country_id, 'Shiwalaya Rural Municipality', 'L6207', 3, v_jajarkot);

    -- Jumla (8 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Chandannath Municipality', 'L6301', 3, v_jumla),
    (v_country_id, 'Kankasundari Rural Municipality', 'L6302', 3, v_jumla),
    (v_country_id, 'Sinja Rural Municipality', 'L6303', 3, v_jumla),
    (v_country_id, 'Hima Rural Municipality', 'L6304', 3, v_jumla),
    (v_country_id, 'Tila Rural Municipality', 'L6305', 3, v_jumla),
    (v_country_id, 'Guthichaur Rural Municipality', 'L6306', 3, v_jumla),
    (v_country_id, 'Tatopani Rural Municipality', 'L6307', 3, v_jumla),
    (v_country_id, 'Patarasi Rural Municipality', 'L6308', 3, v_jumla);

    -- Kalikot (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Khandachakra Municipality', 'L6401', 3, v_kalikot),
    (v_country_id, 'Raskot Municipality', 'L6402', 3, v_kalikot),
    (v_country_id, 'Tilagufa Municipality', 'L6403', 3, v_kalikot),
    (v_country_id, 'Pachal Jharna Rural Municipality', 'L6404', 3, v_kalikot),
    (v_country_id, 'Sanni Triveni Rural Municipality', 'L6405', 3, v_kalikot),
    (v_country_id, 'Narharinath Rural Municipality', 'L6406', 3, v_kalikot),
    (v_country_id, 'Shubha Kalika Rural Municipality', 'L6407', 3, v_kalikot),
    (v_country_id, 'Mahawai Rural Municipality', 'L6408', 3, v_kalikot),
    (v_country_id, 'Palata Rural Municipality', 'L6409', 3, v_kalikot);

    -- Mugu (4 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Chhayanath Rara Municipality', 'L6501', 3, v_mugu),
    (v_country_id, 'Mugum Karmarong Rural Municipality', 'L6502', 3, v_mugu),
    (v_country_id, 'Soru Rural Municipality', 'L6503', 3, v_mugu),
    (v_country_id, 'Khatyad Rural Municipality', 'L6504', 3, v_mugu);

    -- Rukum West (6 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Musikot Municipality', 'L6601', 3, v_rukum_west),
    (v_country_id, 'Chaurjahari Municipality', 'L6602', 3, v_rukum_west),
    (v_country_id, 'Aathbiskot Municipality', 'L6603', 3, v_rukum_west),
    (v_country_id, 'Banfikot Rural Municipality', 'L6604', 3, v_rukum_west),
    (v_country_id, 'Triveni Rural Municipality', 'L6605', 3, v_rukum_west),
    (v_country_id, 'Sani Bheri Rural Municipality', 'L6606', 3, v_rukum_west);

    -- Salyan (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Sharada Municipality', 'L6701', 3, v_salyan),
    (v_country_id, 'Bangad Kupinde Municipality', 'L6702', 3, v_salyan),
    (v_country_id, 'Bagchaur Municipality', 'L6703', 3, v_salyan),
    (v_country_id, 'Chhatreshwori Rural Municipality', 'L6704', 3, v_salyan),
    (v_country_id, 'Darma Rural Municipality', 'L6705', 3, v_salyan),
    (v_country_id, 'Kalimati Rural Municipality', 'L6706', 3, v_salyan),
    (v_country_id, 'Kapurkot Rural Municipality', 'L6707', 3, v_salyan),
    (v_country_id, 'Kumakh Rural Municipality', 'L6708', 3, v_salyan),
    (v_country_id, 'Siddha Kumakh Rural Municipality', 'L6709', 3, v_salyan),
    (v_country_id, 'Tribeni Rural Municipality', 'L6710', 3, v_salyan);

    -- Surkhet (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Birendranagar Municipality', 'L6801', 3, v_surkhet),
    (v_country_id, 'Bheriganga Municipality', 'L6802', 3, v_surkhet),
    (v_country_id, 'Gurbhakot Municipality', 'L6803', 3, v_surkhet),
    (v_country_id, 'Lekbeshi Municipality', 'L6804', 3, v_surkhet),
    (v_country_id, 'Panchapuri Municipality', 'L6805', 3, v_surkhet),
    (v_country_id, 'Barahatal Rural Municipality', 'L6806', 3, v_surkhet),
    (v_country_id, 'Chaukune Rural Municipality', 'L6807', 3, v_surkhet),
    (v_country_id, 'Chingad Rural Municipality', 'L6808', 3, v_surkhet),
    (v_country_id, 'Simta Rural Municipality', 'L6809', 3, v_surkhet);

    -- ---- SUDURPASHCHIM PROVINCE ----

    -- Achham (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Mangalsen Municipality', 'L6901', 3, v_achham),
    (v_country_id, 'Sanfebagar Municipality', 'L6902', 3, v_achham),
    (v_country_id, 'Kamalbazar Municipality', 'L6903', 3, v_achham),
    (v_country_id, 'Panchadewal Binayak Municipality', 'L6904', 3, v_achham),
    (v_country_id, 'Bannigadhi Jayagadh Rural Municipality', 'L6905', 3, v_achham),
    (v_country_id, 'Chaurpati Rural Municipality', 'L6906', 3, v_achham),
    (v_country_id, 'Dhakari Rural Municipality', 'L6907', 3, v_achham),
    (v_country_id, 'Mellekh Rural Municipality', 'L6908', 3, v_achham),
    (v_country_id, 'Ramaroshan Rural Municipality', 'L6909', 3, v_achham),
    (v_country_id, 'Turmakhand Rural Municipality', 'L6910', 3, v_achham);

    -- Baitadi (10 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Dasharathchanda Municipality', 'L7001', 3, v_baitadi),
    (v_country_id, 'Patan Municipality', 'L7002', 3, v_baitadi),
    (v_country_id, 'Melauli Municipality', 'L7003', 3, v_baitadi),
    (v_country_id, 'Purchaudi Municipality', 'L7004', 3, v_baitadi),
    (v_country_id, 'Dogadakedar Rural Municipality', 'L7005', 3, v_baitadi),
    (v_country_id, 'Dilasaini Rural Municipality', 'L7006', 3, v_baitadi),
    (v_country_id, 'Pancheshwar Rural Municipality', 'L7007', 3, v_baitadi),
    (v_country_id, 'Sigas Rural Municipality', 'L7008', 3, v_baitadi),
    (v_country_id, 'Shivanath Rural Municipality', 'L7009', 3, v_baitadi),
    (v_country_id, 'Surnaya Rural Municipality', 'L7010', 3, v_baitadi);

    -- Bajhang (12 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Jayaprithvi Municipality', 'L7101', 3, v_bajhang),
    (v_country_id, 'Bungal Municipality', 'L7102', 3, v_bajhang),
    (v_country_id, 'Talkot Rural Municipality', 'L7103', 3, v_bajhang),
    (v_country_id, 'Masta Rural Municipality', 'L7104', 3, v_bajhang),
    (v_country_id, 'Khaptad Chhanna Rural Municipality', 'L7105', 3, v_bajhang),
    (v_country_id, 'Thalara Rural Municipality', 'L7106', 3, v_bajhang),
    (v_country_id, 'Bitthadchir Rural Municipality', 'L7107', 3, v_bajhang),
    (v_country_id, 'Surma Rural Municipality', 'L7108', 3, v_bajhang),
    (v_country_id, 'Chhabis Pathibhera Rural Municipality', 'L7109', 3, v_bajhang),
    (v_country_id, 'Durgathali Rural Municipality', 'L7110', 3, v_bajhang),
    (v_country_id, 'Kedarsyu Rural Municipality', 'L7111', 3, v_bajhang),
    (v_country_id, 'Saipal Rural Municipality', 'L7112', 3, v_bajhang);

    -- Bajura (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Badimalika Municipality', 'L7201', 3, v_bajura),
    (v_country_id, 'Triveni Municipality', 'L7202', 3, v_bajura),
    (v_country_id, 'Budhiganga Municipality', 'L7203', 3, v_bajura),
    (v_country_id, 'Budhinanda Municipality', 'L7204', 3, v_bajura),
    (v_country_id, 'Gaumul Rural Municipality', 'L7205', 3, v_bajura),
    (v_country_id, 'Pandav Gupha Rural Municipality', 'L7206', 3, v_bajura),
    (v_country_id, 'Himali Rural Municipality', 'L7207', 3, v_bajura),
    (v_country_id, 'Khaptad Chhededaha Rural Municipality', 'L7208', 3, v_bajura),
    (v_country_id, 'Swami Kartik Khapar Rural Municipality', 'L7209', 3, v_bajura);

    -- Dadeldhura (7 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Amargadhi Municipality', 'L7301', 3, v_dadeldhura),
    (v_country_id, 'Parshuram Municipality', 'L7302', 3, v_dadeldhura),
    (v_country_id, 'Aalitaal Rural Municipality', 'L7303', 3, v_dadeldhura),
    (v_country_id, 'Bhageshwar Rural Municipality', 'L7304', 3, v_dadeldhura),
    (v_country_id, 'Ganyapadhura Rural Municipality', 'L7305', 3, v_dadeldhura),
    (v_country_id, 'Navadurga Rural Municipality', 'L7306', 3, v_dadeldhura),
    (v_country_id, 'Ajaymeru Rural Municipality', 'L7307', 3, v_dadeldhura);

    -- Darchula (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Mahakali Municipality', 'L7401', 3, v_darchula),
    (v_country_id, 'Shailyashikhar Municipality', 'L7402', 3, v_darchula),
    (v_country_id, 'Malikarjun Rural Municipality', 'L7403', 3, v_darchula),
    (v_country_id, 'Marma Rural Municipality', 'L7404', 3, v_darchula),
    (v_country_id, 'Lekam Rural Municipality', 'L7405', 3, v_darchula),
    (v_country_id, 'Naugad Rural Municipality', 'L7406', 3, v_darchula),
    (v_country_id, 'Duhun Rural Municipality', 'L7407', 3, v_darchula),
    (v_country_id, 'Apihimal Rural Municipality', 'L7408', 3, v_darchula),
    (v_country_id, 'Byas Rural Municipality', 'L7409', 3, v_darchula);

    -- Doti (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Dipayal Silgadhi Municipality', 'L7501', 3, v_doti),
    (v_country_id, 'Shikhar Municipality', 'L7502', 3, v_doti),
    (v_country_id, 'Purbichauki Rural Municipality', 'L7503', 3, v_doti),
    (v_country_id, 'Badikedar Rural Municipality', 'L7504', 3, v_doti),
    (v_country_id, 'Jorayal Rural Municipality', 'L7505', 3, v_doti),
    (v_country_id, 'Sayal Rural Municipality', 'L7506', 3, v_doti),
    (v_country_id, 'Aadarsha Rural Municipality', 'L7507', 3, v_doti),
    (v_country_id, 'K.I. Singh Rural Municipality', 'L7508', 3, v_doti),
    (v_country_id, 'Bogatan Fudsil Rural Municipality', 'L7509', 3, v_doti);

    -- Kailali (13 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Dhangadhi Sub-Metropolitan City', 'L7601', 3, v_kailali),
    (v_country_id, 'Tikapur Municipality', 'L7602', 3, v_kailali),
    (v_country_id, 'Ghodaghodi Municipality', 'L7603', 3, v_kailali),
    (v_country_id, 'Lamkichuha Municipality', 'L7604', 3, v_kailali),
    (v_country_id, 'Bhajani Municipality', 'L7605', 3, v_kailali),
    (v_country_id, 'Godawari Municipality', 'L7606', 3, v_kailali),
    (v_country_id, 'Gauriganga Municipality', 'L7607', 3, v_kailali),
    (v_country_id, 'Janaki Rural Municipality', 'L7608', 3, v_kailali),
    (v_country_id, 'Bardagoriya Rural Municipality', 'L7609', 3, v_kailali),
    (v_country_id, 'Mohanyal Rural Municipality', 'L7610', 3, v_kailali),
    (v_country_id, 'Kailari Rural Municipality', 'L7611', 3, v_kailali),
    (v_country_id, 'Joshipur Rural Municipality', 'L7612', 3, v_kailali),
    (v_country_id, 'Chure Rural Municipality', 'L7613', 3, v_kailali);

    -- Kanchanpur (9 local levels)
    INSERT INTO administrative_divisions (country_id, name, code, division_level, parent_id) VALUES
    (v_country_id, 'Bhimdatt Municipality', 'L7701', 3, v_kanchanpur),
    (v_country_id, 'Punarbas Municipality', 'L7702', 3, v_kanchanpur),
    (v_country_id, 'Bedkot Municipality', 'L7703', 3, v_kanchanpur),
    (v_country_id, 'Mahakali Municipality', 'L7704', 3, v_kanchanpur),
    (v_country_id, 'Shuklaphanta Municipality', 'L7705', 3, v_kanchanpur),
    (v_country_id, 'Belauri Municipality', 'L7706', 3, v_kanchanpur),
    (v_country_id, 'Krishnapur Municipality', 'L7707', 3, v_kanchanpur),
    (v_country_id, 'Beldandi Rural Municipality', 'L7708', 3, v_kanchanpur),
    (v_country_id, 'Laljhadi Rural Municipality', 'L7709', 3, v_kanchanpur);

END $$;
");
        }

        public override void Down()
        {
            Execute.Sql("DELETE FROM administrative_divisions WHERE country_id = (SELECT id FROM countries WHERE iso3_code = 'NPL')");
        }
    }
}

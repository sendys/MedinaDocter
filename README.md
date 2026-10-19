# MedinaDokter

> Dokumentasi utama proyek SIMRS — fokus pengembangan awal: **modul dokter rawat inap**.

## 1. Tentang Proyek

**MedinaDokter** adalah proyek pengembangan modul Sistem Informasi Manajemen Rumah Sakit (SIMRS) yang berfokus pada dukungan alur kerja klinis dokter, dimulai dari pelayanan rawat inap.

Dokumentasi ini menjadi titik awal untuk mencatat tujuan proyek, ruang lingkup fitur, alur pelayanan, rancangan data, prinsip keamanan, dan rencana pengembangan. Detail teknologi, konfigurasi, dan struktur kode harus diperbarui sesuai implementasi aktual repositori.

## 2. Tujuan

- Memudahkan dokter melihat daftar pasien rawat inap yang menjadi tanggung jawabnya.
- Mendukung dokumentasi Rekam Medis Elektronik (RME) secara terstruktur.
- Mendukung asesmen awal, diagnosis, CPPT/visite, instruksi medis, dan resume medis.
- Menjaga kesinambungan informasi klinis antarprofesi dan antarunit.
- Menyediakan pencatatan aktivitas serta jejak audit untuk perubahan data medis.
- Menyiapkan fondasi integrasi dengan farmasi, laboratorium, radiologi, keperawatan, dan layanan penunjang lainnya.

## 3. Ruang Lingkup Pengembangan

### Tahap awal — Modul dokter rawat inap

1. **Dashboard dokter**
   - Ringkasan pasien dalam perawatan.
   - Daftar pasien yang perlu visite.
   - Catatan klinis yang masih perlu dilengkapi.
   - Informasi hasil pemeriksaan terbaru dan tindak lanjut.

2. **Daftar pasien rawat inap**
   - Pencarian pasien berdasarkan identitas atau nomor registrasi.
   - Filter berdasarkan DPJP, ruang, dan status perawatan.
   - Informasi ruang, tempat tidur, tanggal masuk, dan status kunjungan.
   - Akses ke detail RME berdasarkan episode rawat inap yang tepat.

3. **Asesmen awal medis**
   - Keluhan utama dan anamnesis.
   - Riwayat penyakit dan riwayat pengobatan yang relevan.
   - Alergi.
   - Pemeriksaan fisik dan tanda vital.
   - Asesmen, diagnosis awal, serta rencana perawatan.

4. **Diagnosis**
   - Diagnosis utama dan diagnosis sekunder.
   - Kode diagnosis sesuai standar yang digunakan rumah sakit.
   - Riwayat dan perubahan diagnosis yang dapat ditelusuri.

5. **CPPT dan visite**
   - Catatan perkembangan pasien terintegrasi.
   - Dokumentasi SOAP: Subjective, Objective, Assessment, Plan.
   - Catatan visite dan evaluasi respons terapi.
   - Identitas profesi/petugas, waktu pencatatan, serta status validasi.

6. **Instruksi medis**
   - Instruksi terapi dan tindakan.
   - Permintaan pemeriksaan penunjang.
   - Resep atau instruksi obat sesuai kewenangan.
   - Konsultasi antarspesialis.
   - Status tindak lanjut dan riwayat perubahan instruksi.

7. **Hasil pemeriksaan**
   - Akses hasil laboratorium dan radiologi.
   - Akses hasil pemeriksaan penunjang lainnya.
   - Penandaan dan tindak lanjut hasil kritis sesuai prosedur rumah sakit.

8. **Resume medis dan pemulangan**
   - Ringkasan perjalanan penyakit.
   - Diagnosis akhir dan tindakan yang dilakukan.
   - Kondisi pasien saat pulang atau dipindahkan.
   - Terapi pulang, edukasi, dan rencana kontrol.

### Integrasi lanjutan

Integrasi dengan farmasi, laboratorium, radiologi/PACS, keperawatan, konsultasi, billing, serta proses penjaminan dan klaim dikembangkan berdasarkan kesiapan sistem yang sudah ada.

## 4. Alur Kerja Klinis

Alur umum pelayanan dokter rawat inap:

1. Pasien terdaftar sebagai pasien rawat inap.
2. Dokter membuka daftar pasien dan memilih episode rawat inap yang benar.
3. Dokter meninjau riwayat RME, alergi, obat, serta hasil pemeriksaan yang tersedia.
4. Dokter mengisi asesmen awal medis dan diagnosis.
5. Dokter membuat instruksi medis atau permintaan pemeriksaan sesuai kebutuhan.
6. Dokter melakukan visite dan mendokumentasikan perkembangan melalui CPPT.
7. Dokter mengevaluasi kondisi dan menentukan tindak lanjut.
8. Saat pasien pulang atau dipindahkan, dokter melengkapi dokumentasi akhir dan resume medis sesuai ketentuan.

Keputusan klinis tetap berada pada tenaga medis yang berwenang. Sistem membantu dokumentasi, koordinasi, pengingat, dan penelusuran informasi.

## 5. Konsep Menu

```text
MedinaDokter
├── Dashboard
├── Pasien Rawat Inap
│   └── Detail Pasien / RME
│       ├── Ringkasan Pasien
│       ├── Asesmen Awal Medis
│       ├── Diagnosis
│       ├── CPPT / Visite
│       ├── Instruksi Medis
│       ├── Hasil Pemeriksaan
│       ├── Konsultasi
│       └── Resume Medis
├── Tugas Klinis
└── Riwayat Aktivitas
```

Nama menu dan pembagian halaman dapat disesuaikan dengan pola navigasi serta modul yang sudah tersedia di repositori.

## 6. Prinsip Data dan Relasi

- Setiap catatan klinis harus terhubung dengan pasien dan **episode/kunjungan rawat inap yang tepat**, bukan hanya identitas pasien.
- Gunakan nomor registrasi atau primary key kunjungan sebagai penghubung sesuai desain database yang berlaku.
- Data ruang dan tempat tidur harus mencerminkan riwayat penempatan/perpindahan pasien jika fitur tersebut tersedia.
- Hubungan dokter dengan pasien harus mencatat penugasan DPJP dan/atau dokter yang membuat catatan.
- Hindari membuat tabel atau sumber data duplikat apabila fungsi yang sama sudah tersedia di SIMRS.
- Seluruh perubahan skema harus menggunakan mekanisme migrasi dan ditinjau dampaknya terhadap data lama.

### Kandidat entitas

Nama berikut merupakan konsep awal, **bukan pernyataan bahwa tabel tersebut sudah ada**:

| Entitas | Tanggung jawab |
|---|---|
| Registrasi/kunjungan rawat inap | Episode perawatan pasien |
| Penempatan rawat inap | Ruang, tempat tidur, dan perpindahan |
| Penugasan DPJP | Relasi dokter dengan episode perawatan |
| Asesmen awal medis | Dokumentasi asesmen saat awal perawatan |
| Diagnosis | Diagnosis klinis dan kode terkait |
| CPPT | Catatan perkembangan pasien terintegrasi |
| Instruksi medis | Instruksi dan status tindak lanjut |
| Resep | Permintaan obat dan detailnya, bila dikelola modul ini |
| Konsultasi | Permintaan konsultasi dan respons |
| Resume medis | Ringkasan akhir episode perawatan |
| Audit log | Riwayat aktivitas dan perubahan data |

Nama tabel, kolom, foreign key, dan kardinalitas final harus mengikuti hasil pemeriksaan skema database aktual.

## 7. Status dan Validasi Catatan

Status yang dapat dipertimbangkan:

- **Draf** — catatan masih dapat diedit oleh pengguna yang berwenang.
- **Final/ditandatangani** — catatan telah divalidasi melalui mekanisme autentikasi yang berlaku.
- **Dikoreksi/adendum** — koreksi terhadap catatan final dilakukan dengan mekanisme yang menjaga catatan asli dan riwayat koreksi.

Penerapan status harus disesuaikan dengan kebijakan rumah sakit dan regulasi yang berlaku. Jangan menghapus atau menimpa catatan final secara diam-diam. Catat pelaku, waktu, alasan, dan perubahan yang relevan.

## 8. Keamanan dan Privasi

Karena sistem mengelola informasi kesehatan, keamanan perlu menjadi bagian dari desain sejak awal:

- Autentikasi pengguna dan kontrol akses berbasis peran serta kewenangan klinis.
- Pemeriksaan akses di sisi server, bukan hanya menyembunyikan tombol di antarmuka.
- Pembatasan akses berdasarkan unit, penugasan, dan kebutuhan pelayanan sesuai kebijakan.
- Audit akses dan perubahan data medis yang relevan.
- Perlindungan data saat dikirim dan disimpan sesuai kemampuan lingkungan.
- Validasi input dan perlindungan terhadap akses langsung ke data pasien lain.
- Pengelolaan sesi, rahasia aplikasi, backup, serta prosedur pemulihan.
- Hindari menaruh data pasien nyata, kredensial, token, atau rahasia di README, log pengembangan, maupun repositori publik.

## 9. Rencana Pengembangan

### P1 — Fondasi

- Verifikasi struktur proyek, teknologi, autentikasi, dan database yang sudah digunakan.
- Petakan tabel pasien, registrasi, rawat inap, dokter, ruang, dan tempat tidur.
- Bangun daftar pasien rawat inap dengan filter dan pemeriksaan hak akses.
- Bangun halaman detail RME berbasis episode rawat inap.

### P2 — Dokumentasi inti

- Asesmen awal medis.
- Diagnosis.
- CPPT dan visite.
- Draf, finalisasi, serta audit perubahan.

### P3 — Instruksi dan hasil

- Instruksi medis.
- Integrasi order dan hasil laboratorium/radiologi.
- Konsultasi antarspesialis.
- Integrasi resep dengan modul farmasi jika tersedia.

### P4 — Penyelesaian perawatan

- Resume medis.
- Dokumentasi pemulangan atau pemindahan.
- Pemeriksaan kelengkapan dokumentasi dan laporan operasional.

Setiap tahap perlu dilengkapi pengujian fungsional, pengujian hak akses, validasi relasi data, dan uji regresi terhadap fitur SIMRS yang sudah berjalan.

## 10. Kriteria Penerimaan Awal

- Dokter hanya dapat mengakses data sesuai hak akses dan kewenangannya.
- Pasien yang dipilih terhubung dengan episode rawat inap yang benar.
- Catatan klinis tersimpan dengan identitas pembuat dan waktu pencatatan.
- Data draf dapat dilanjutkan tanpa membuat catatan ganda.
- Catatan final terlindungi dari perubahan tanpa proses koreksi yang dapat diaudit.
- Riwayat diagnosis, CPPT, dan instruksi dapat ditelusuri.
- Kegagalan penyimpanan ditampilkan secara jelas dan tidak menghasilkan duplikasi diam-diam.
- Data yang ditampilkan berasal dari sumber SIMRS yang benar, bukan data contoh di lingkungan produksi.

## 11. Teknologi dan Konfigurasi

Informasi berikut perlu dikonfirmasi dari repositori aktual sebelum diisi:

- Bahasa pemrograman dan framework:
- Versi runtime:
- Database dan versinya:
- Frontend/UI:
- Mekanisme autentikasi dan otorisasi:
- Cara menjalankan proyek secara lokal:
- Perintah migrasi dan pengujian:
- Lingkungan pengembangan, staging, dan produksi:
- Sistem integrasi eksternal:

Jangan mencantumkan kredensial, private key, token, atau nilai rahasia di bagian ini. Gunakan file environment lokal dan dokumentasikan nama variabelnya saja.

## 12. Kontribusi dan Perubahan

- Ikuti pola arsitektur dan konvensi kode yang sudah ada.
- Gunakan migrasi untuk perubahan skema database.
- Sertakan validasi dan otorisasi pada endpoint atau proses yang mengakses data klinis.
- Tambahkan pengujian untuk alur utama dan skenario akses tidak sah.
- Dokumentasikan perubahan struktur data dan integrasi.
- Hindari menggunakan data pasien nyata pada fixture, screenshot, dan laporan bug.

## 13. Catatan Proyek

README ini adalah dokumen awal kebutuhan dan arah pengembangan MedinaDokter. Perbarui bagian teknologi, struktur database, instruksi instalasi, dan status fitur setelah pemeriksaan repositori dan implementasi aktual.

---

**Nama proyek:** MedinaDokter  
**Fokus awal:** Modul dokter rawat inap dan RME  
**Dokumen:** `README.md`  
**Lingkungan yang terlihat pada referensi:** `dev`  

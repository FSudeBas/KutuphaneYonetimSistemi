# Kütüphane Yönetim Sistemi (Windows Forms + MySQL)

Uludağ Üniversitesi Yönetim Bilişim Sistemleri – Nesne Yönelimli Programlama dersi kapsamında geliştirilen masaüstü **Kütüphane Yönetim Sistemi**.


## 🎥 Proje Tanıtım Videosu (YouTube)

[YouTube - Kütüphane Yönetim Sistemi Tanıtım](https://youtu.be/ZK63lw26JEE)

##  Özellikler

### Roller
- **Admin**: Tüm modüllere erişim (Kitaplar, Üyeler, Kategoriler, Emanet, Talepler, Raporlar)
- **Görevli**: Kitaplar, Üyeler, Emanet, Talepler (Raporlar/Kategoriler kısıtlı olabilir)
- **Üye**: Kitap listesini görüntüler, kitap talebi oluşturur, kendi emanet ve talep durumlarını görür

### Modüller
- **Kitaplar**: Listeleme, ekleme, güncelleme, silme (üye sadece görüntüler)
- **Üyeler**: Üye yönetimi
- **Kategoriler**: Kategori ekleme/düzenleme
- **Emanet**: Emanet ver / iade al, aktif/geçmiş emanet takibi, gecikme hesaplama
- **Talepler**: Üyelerin kitap taleplerini görüntüleme, **Onayla/Reddet**
- **Üye Paneli**: Üyenin kendi emanetleri ve taleplerinin durumu (TabControl ile)

##  Çalışma Mantığı (Event Tabanlı)
- **Form_Load**: Verileri yükleme
- **DataGridView CellClick**: Seçilen kaydın ID’sini alma
- **Button Click**: Kaydet / Güncelle / Sil / Onayla / Reddet işlemleri (SQL)

##  Veritabanı (MySQL) – Temel Tablolar
- `books` : BookId, Title, Author, Publisher, PublishYear, ISBN, Stock, CategoryId
- `categories` : CategoryId, CategoryName
- `members` : MemberId, (ad/soyad/mail/telefon vb.)
- `borrows` : BorrowId, MemberId, BookId, BorrowDate, DueDate, ReturnDate
- `book_requests` : RequestId, MemberId, RequestedTitle, RequestedAuthor, RequestedPublisher, Note, Status, RequestDate

## ⚙️ Kurulum
1. Projeyi klonla:
   ```bash
   git clone https://github.com/FSudeBas/KutuphaneYonetimSistemi.git

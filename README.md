# 🌐 AğanBrowser

> **Yapay zekası buluta gitmeyen, Türkçe, açık kaynak masaüstü tarayıcı.**

AğanBrowser, Microsoft Edge Chromium motorunu (WebView2) kullanan, yerel yapay zeka
asistanı **Ağan** ile entegre çalışan, gizlilik odaklı bir Windows tarayıcısıdır.
Tüm AI sorguları cihazınızda çalışan [Ollama](https://ollama.com) sunucusuna gider —
hiçbir veri buluta gönderilmez.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6?logo=windows)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![WebView2](https://img.shields.io/badge/WebView2-1.0.2903.40-0078D4)](https://developer.microsoft.com/microsoft-edge/webview2/)

---

## ✨ Özellikler

### 🤖 Yerel Yapay Zeka Asistanı — Ağan
- **Ollama** ile tamamen yerel LLM entegrasyonu (bulut yok, veri sızıntısı yok)
- **Sayfa bağlamı analizi** — "Bu sayfada ne yapacağım?" diye sorun, aktif sayfanın
  HTML'ini okuyup adım adım anlatsın
- **Streaming yanıt** — kelime kelime yazma, sohbet hissi
- Model seçici (kurulu Ollama modellerini otomatik listeler)
- Türkçe asistan kişiliği (Tamamen anonim, kullanıcıdan veri öğrenmez)

### 🌐 Tarayıcı Çekirdeği
- Çoklu sekme (yatay / dikey mod)
- Adres çubuğu + Google arama entegrasyonu
- HTTPS/HTTP güvenlik göstergesi
- Favicon desteği, özel sağ tık menüsü
- Oturum kaydetme (kapatınca sekmeler hatırlanır)
- Klavye kısayolları (`Ctrl+T`, `Ctrl+W`, `Ctrl+L`, `Ctrl+Tab`, `F5`)
- PDF / HTML olarak sayfa kaydetme
- DevTools açma

### 🛡️ Agresif Reklam ve İzleyici Engelleyici
- **Topluluk Listeleri:** EasyList ve EasyPrivacy desteği ile binlerce reklam ağına karşı tam koruma.
- **Akıllı Filtreleme:** 3. parti zararlı `Script` ve `Iframe`leri otomatik tespit edip engeller.
- **URL Temizleyici:** `utm_*`, `fbclid` gibi takip parametrelerini sayfa yüklenmeden temizler.
- **Beyaz Liste:** Sorun çıkaran siteleri tek tıkla "Muaf Tut" butonu ile korumaya alın.
- **Kozmetik Filtreleme:** CSS ile reklam kutularını gizleme ve canlı engelleme sayacı.

### 🔐 Şifre Yöneticisi
- **Windows DPAPI** ile şifrelenmiş kasa (`passwords.dat`)
- Site bazlı kayıt
- Otomatik doldurma (aktif sayfadaki form alanlarına)

### 🧩 Uzantı Sistemi
- WebView2 tarayıcı uzantıları desteği
- Kurulum: yerleşik klasör, yerel klasör, ZIP, GitHub deposu

### 🎨 Arayüz
- Koyu / Açık tema (anlık geçiş, WebView içeriğine de yansır)
- Sol / Sağ panel (açılır-kapanır)
- Yer imleri, geçmiş, kısayollar
- İndirme yöneticisi (duraklat/devam/iptal)

---


## 📅 Sürüm Notları

### v1.1.0 - "Gizlilik ve Hız" Güncellemesi
Bu sürümde odak noktamız, tarayıcının arka planında çalışan reklam ve izleyici engelleme mekanizmasını çok daha agresif ve akıllı hale getirmek oldu. Ayrıca arayüzdeki gereksiz kalabalıklar temizlendi.

**🛡️ Reklam Engelleyici Artık Çok Daha Güçlü:**
- **Topluluk Listeleri Desteği:** Kendi sınırlı listemiz yerine, dünyanın en büyük reklam listeleri olan `EasyList` ve `EasyPrivacy` altyapısı eklendi. Binlerce reklam ağı tek bir motorla engelleniyor.
- **URL Temizleyici (Tracking Strip):** Reklamverenlerin sizi izlemek için kullandığı `utm_source`, `fbclid`, `gclid` gibi parametreler, sayfa yüklenmeden önce URL'den otomatik olarak siliniyor.
- **Akıllı Kaynak Kontrolü:** Sadece alan adına bakılmıyor; sayfaya dışarıdan yüklenen 3. parti `Script` ve `Iframe`ler özel olarak analiz edilip engelleniyor.
- **Beyaz Liste (Allowlist):** Reklam engelleyicinin bir siteyi bozması durumunda, üst bara eklenen **"🛡️ Muaf Tut"** butonu ile o site tek tıkla beyaz listeye alınabiliyor. Ayarlar kalıcı olarak saklanıyor.

**🧹 Arayüz ve Performans Temizliği:**
- Sol paneldeki gereksiz uzantı listesi ve kısayollar kaldırıldı, arayüz sadeleştirildi.
- Yapay zekanın (Ağan) sizden izinsiz veri öğrenmesini engellemek için arka plandaki "Bilgi Haznesi" ve "Çalışma Alanları" gibi hafıza modülleri tamamen kaldırıldı. Ağan artık tamamen anonim bir asistan.
- Yapay zekanın yanıt üretirken takılmasına neden olan `AganSystemPrompt` çakışmaları giderildi.
- Uygulama kapatılırken arka planda "hayalet süreç" kalmasını engelleyen `Environment.Exit` güvencesi eklendi.

---
## 🚀 Hızlı Başlangıç

### Gereksinimler

| Bileşen | Not |
|---------|-----|
| **Windows 10 / 11** | WPF + WebView2 bağımlılığı |
| **.NET 8 SDK** | [İndir](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **WebView2 Runtime** | Windows 11'de hazır; Windows 10'da [indir](https://developer.microsoft.com/microsoft-edge/webview2/) |
| **Ollama** | AI özellikleri için (opsiyonel) — [indir](https://ollama.com) |

### Kurulum

```bash
# 1. Repoyu klonla
git clone https://github.com/KOSMO471/AganBrowser.git
cd AganBrowser

# 2. Bir Ollama modeli indir (AI için)
ollama pull qwen2.5-coder:7b

# 3. Derle
build.bat

# 4. Çalıştır
run.bat

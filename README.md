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
- Türkçe asistan kişiliği

### 🌐 Tarayıcı Çekirdeği
- Çoklu sekme (yatay / dikey mod)
- Adres çubuğu + Google arama entegrasyonu
- HTTPS/HTTP güvenlik göstergesi
- Favicon desteği, özel sağ tık menüsü
- Oturum kaydetme (kapatınca sekmeler hatırlanır)
- Klavye kısayolları (`Ctrl+T`, `Ctrl+W`, `Ctrl+L`, `Ctrl+Tab`, `F5`)
- PDF / HTML olarak sayfa kaydetme
- DevTools açma

### 🛡️ Reklam Engelleyici
- 70+ reklam/tracker alan adı engelleme
- Kozmetik filtreleme (CSS ile reklam kutularını gizleme)
- Aç/kapa düğmesi, canlı sayaç

### 🔐 Şifre Yöneticisi
- **Windows DPAPI** ile şifrelenmiş kasa (`passwords.dat`)
- Site bazlı kayıt
- Otomatik doldurma (aktif sayfadaki form alanlarına)

### 🧩 Uzantı Sistemi
- WebView2 tarayıcı uzantıları desteği
- Kurulum: yerleşik klasör, yerel klasör, ZIP, GitHub deposu
- Örnek uzantı: "Sayfa Bilgisi"

### 🎨 Arayüz
- Koyu / Açık tema (anlık geçiş, WebView içeriğine de yansır)
- Sol / Sağ panel (açılır-kapanır)
- Yer imleri, geçmiş, kısayollar
- İndirme yöneticisi (duraklat/devam/iptal)

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
1. Repoyu klonla
git clone https://github.com/KOSMO471/AganBrowser.git
cd AganBrowser

2. Bir Ollama modeli indir (AI için)
ollama pull qwen2.5-coder:7b

3. Derle
build.bat

4. Çalıştır
run.bat

# 4. Çalıştır
run.bat

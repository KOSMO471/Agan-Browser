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
[![Version](https://img.shields.io/badge/version-1.2.0-blue?logo=github)](https://github.com/KOSMO471/Agan-Browser/releases)
[![Release](https://img.shields.io/github/v/release/KOSMO471/Agan-Browser?color=purple&label=release)](https://github.com/KOSMO471/Agan-Browser/releases)

---

## ✨ Özellikler

### 🤖 Yerel Yapay Zeka Asistanı — Ağan
- **Ollama** ile tamamen yerel LLM entegrasyonu (bulut yok, veri sızıntısı yok)
- **Sayfa bağlamı analizi** — "Bu sayfada ne yapacağım?" diye sorun, aktif sayfanın
  HTML'ini okuyup adım adım anlatsın
- **Streaming yanıt** — kelime kelime yazma, sohbet hissi
- Model seçici (kurulu Ollama modellerini otomatik listeler)
- Türkçe asistan kişiliği (Tamamen anonim, kullanıcıdan veri öğrenmez)
- **🧠 15 Farklı Kişilik** — Ağan'ın karakterini siz belirleyin (Esprili, Resmi, Filozof, Robot...)

### 🌐 Tarayıcı Çekirdeği
- Çoklu sekme (yatay / dikey mod)
- Adres çubuğu + Google arama entegrasyonu
- HTTPS/HTTP güvenlik göstergesi
- Favicon desteği, özel sağ tık menüsü
- **Sekme sürükle-bırak** ile sıralama
- Oturum kaydetme (kapatınca sekmeler hatırlanır)
- Klavye kısayolları (`Ctrl+T`, `Ctrl+W`, `Ctrl+L`, `Ctrl+Tab`, `F5`)
- PDF / HTML olarak sayfa kaydetme
- DevTools açma

### 🕶️ Gizli Mod (Incognito)
- İzole WebView2 profili (geçmiş, çerez, şifre bırakmaz)
- Mor temalı özel karşılama sayfası
- Sekmelerde `🕶️` ikonu ile görsel ayrım
- `%TEMP%` klasöründe çalışır, oturum kapanınca silinir

### 🖱️ Akıllı Sağ Tık Menüsü
- **Seçileni Ağan'a Sor** / **Türkçeye Çevir** / **Detaylı Açıkla**
- **PiP'de Aç** (video küçük pencerede)
- **Resmi Kaydet**, **Sayfayı PDF Kaydet**
- **Öğeyi İncele** (DevTools)

### 🛡️ Agresif Reklam ve İzleyici Engelleyici
- **Topluluk Listeleri:** EasyList ve EasyPrivacy desteği ile binlerce reklam ağına karşı tam koruma.
- **Akıllı Filtreleme:** 3. parti zararlı `Script` ve `Iframe`leri otomatik tespit edip engeller.
- **URL Temizleyici:** `utm_*`, `fbclid` gibi takip parametrelerini sayfa yüklenmeden temizler.
- **Beyaz Liste:** Sorun çıkaran siteleri tek tıkla "Muaf Tut" butonu ile korumaya alın.
- **Kozmetik Filtreleme:** CSS ile reklam kutularını gizleme ve canlı engelleme sayacı.

### 📖 Oku Sonra
- Makaleleri tek tıkla kaydedin, dilediğinizde açın
- Okunabilir özel HTML şablonu (serif font, geniş satır aralığı)
- `%APPDATA%\AganBrowser\ReadLater\` klasöründe saklanır

### 🔐 Şifre Yöneticisi
- **Windows DPAPI** ile şifrelenmiş kasa (`passwords.dat`)
- Site bazlı kayıt
- Otomatik doldurma (aktif sayfadaki form alanlarına)

### 🧩 Uzantı Sistemi
- WebView2 tarayıcı uzantıları desteği
- Kurulum: yerleşik klasör, yerel klasör, ZIP, GitHub deposu

### 🎨 Arayüz
- **5 Farklı Tema:** Dark, Dracula, Nord, Light, Sepya (anlık geçiş, WebView içeriğine de yansır)
- Sol / Sağ panel (açılır-kapanır)
- Yer imleri, geçmiş, kısayollar
- İndirme yöneticisi (duraklat/devam/iptal)
- **Canlı saatli, yer imli, günün sözlü Yeni Sekme sayfası**
- İlk kullanım için interaktif Onboarding

---


## 📅 Sürüm Notları

### v1.2.0 - "Kişilik ve Gizlilik" Güncellemesi
Bu sürümde Ağan sadece bir asistan değil; size özel bir karaktere bürünen, gizli modda iz bırakmadan çalışan ve tarayıcı deneyimini baştan sona kişiselleştirebilen bir yol arkadaşına dönüştü.

#### 🧠 Ağan Artık Kişilik Sahibi — 15 Farklı Karakter

Bu sürümün yıldızı, Ağan'ın ruh halini ve konuşma tarzını tamamen sizin kontrolünüze veren **Kişilik Sistemi** oldu.

| # | Kişilik | Karakter |
|---|---------|----------|
| 1 | 😄 **Esprili** | Cevaplarına mizah katar |
| 2 | 🤵 **Resmi** | Profesyonel ve ciddi dil |
| 3 | ✂️ **Kısa ve Öz** | Gereksiz detaya girmez |
| 4 | 👨‍🏫 **Öğretmen** | Sabırlı ve eğitici anlatım |
| 5 | 🔥 **Motivatör** | Enerjik ve pozitif |
| 6 | 🤔 **Sokratik** | Sorular sorarak düşündürür |
| 7 | 🏛️ **Filozof** | Derin ve sorgulayıcı |
| 8 | 🧸 **Çocuk** | 5 yaşındaki bir çocuğa anlatır gibi |
| 9 | ⚙️ **Teknik** | Mühendislik terimleriyle detaylı |
| 10 | 💖 **Empatik** | Şefkatli ve destekleyici |
| 11 | 🧐 **Eleştirel** | Artıları ve eksileriyle analiz eder |
| 12 | 🔬 **Bilim İnsanı** | Kanıta dayalı, nesnel |
| 13 | 🎖️ **Lider** | Kararlı ve yönlendirici |
| 14 | ⚽ **Sporcu** | Disiplin ve azim vurgusu |
| 15 | 🤖 **Robot** | Duygusuz, tamamen mantıksal |

**🖱️ Nasıl Kullanılır?**
- Üst bardaki **`🧠 Kişilik`** butonuna tıklayın
- İstediğiniz özellikleri işaretleyin *(birden fazla seçilebilir)*
- Seçimler `personality_state.json` dosyasına kalıcı olarak kaydedilir
- Değişiklik anında sohbet yanıtlarına yansır

#### 🕶️ Gizli Mod (Incognito) — İz Bırakmadan Gezinme

Tarayıcı geçmişinizde hiçbir iz kalmadan, tamamen izole bir profilde gezinin.

**🔒 Neler Yapmıyor?**
- ❌ Geçmişe kaydetmez
- ❌ Çerez bırakmaz
- ❌ Şifre kaydetmez *(autosave kapalı)*
- ❌ İndirmeler kaydedilmez *(`%TEMP%` klasörü, oturum kapanınca silinir)*
- ✅ Ayrı bir WebView2 profili kullanır *(mevcut çerezlerinizden izole)*

**🎨 Görsel Ayrım:**
- Mor/lila tonlarında özel karşılama sayfası (`Incognito.html`)
- Sekme başlığında **🕶️** ikonu
- Üst barda **`🕶️ Gizli`** butonu ile tek tıkla açılır

#### 🎨 5 Farklı Tema — Tarayıcı Sizin Zevkinize Uyuyor

| Tema | Renk Paleti |
|------|-------------|
| 🌙 **Dark** | Klasik koyu (`#1A1A1A`) |
| 🧛 **Dracula** | Mor-pembe tonlar (`#282A36`) |
| ❄️ **Nord** | Soğuk mavi-gri (`#2E3440`) |
| ☀️ **Light** | Klasik açık (`#F5F5F5`) |
| 📜 **Sepya** | Kitap sayfası sıcaklığı (`#F4ECD8`) |

> **💡 Not:** Tema değişikliği artık sadece WPF arayüzüne değil, `--bg`, `--surface`, `--text`, `--accent` gibi CSS değişkenleriyle **açık sayfalara da** otomatik uygulanıyor.

#### 🖱️ Akıllı Sağ Tık — Metni Seç, Ağan'a Sor

- **🤖 Seçileni Ağan'a Sor** — *"Bu metin ne anlatıyor? Açıkla:"*
- **🌐 Türkçeye Çevir** — *"Şu metni Türkçeye çevir:"*
- **💡 Açıkla (Detaylı)** — *"Bu metni detaylıca, örneklerle açıkla:"*
- **📺 PiP'de Aç** — Videoları küçük pencerede izleyin
- **🖼️ Resmi Kaydet** — Sağ tıklanan resmi diske kaydedin
- **📄 Sayfayı PDF Kaydet** — DevTools açmadan PDF çıktısı
- **🔍 Öğeyi İncele** — DevTools'u açın

#### 📖 Oku Sonra — Makaleleri Kaydedin

- **📖 Oku Sonra** butonu sol panelde
- Makale **özel bir HTML şablonuyla** kaydedilir (serif font, geniş satır aralığı)
- `%APPDATA%\AganBrowser\ReadLater\` klasöründe saklanır
- `index.json` ile liste yönetimi

#### 🎯 Yenilenen Yeni Sekme Sayfası

- ⏰ **Canlı saat** (gradient renkli)
- 📅 **Türkçe tarih** (gün, ay, yıl)
- 🔍 **Arama kutusu** (URL veya Google araması)
- 📌 **Hızlı Erişim** — Yer imleriniz burada listelenir
- 💭 **Günün Sözü** — `quotes.json`'dan her gün farklı bir söz (130+ söz)

#### 👋 İlk Kullanım Deneyimi (Onboarding)

- İlk açılışta kullanıcıyı karşılayan interaktif tanıtım sayfası
- *"Harika, Başlayalım!"* butonuyla `first_run.flag` yazılır

#### 📌 Sekme Sürükle-Bırak

- Sekmeleri farenizle tutup istediğiniz sıraya taşıyın
- Sürükleme sırasında minimum hareket eşiği kontrolü

#### ⌨️ Klavye Kısayolları

| Kısayol | İşlev |
|---------|-------|
| <kbd>Ctrl</kbd> + <kbd>T</kbd> | Yeni sekme |
| <kbd>Ctrl</kbd> + <kbd>W</kbd> | Sekmeyi kapat |
| <kbd>Ctrl</kbd> + <kbd>L</kbd> | Adres çubuğuna odaklan |
| <kbd>Ctrl</kbd> + <kbd>Tab</kbd> | Sonraki sekmeye geç |
| <kbd>Ctrl</kbd> + <kbd>R</kbd> / <kbd>F5</kbd> | Sayfayı yenile |

#### 🛠️ Diğer İyileştirmeler

- 🎨 **Görsel Dönüştürücü** — PNG/JPG/BMP/GIF arası format dönüşümü
- 📥 **İndirme Yöneticisi** — İndirmeleri gerçek zamanlı takip, duraklat/devam/iptal
- 🎭 **FaviconChanged** — Sekmelerde site simgeleri
- 🔄 **Dikey/Yatay sekme modu** — `⬍` butonuyla anlık geçiş

#### 🔧 Teknik Notlar

- **WebView2 Environment** artık `AreBrowserExtensionsEnabled = true` ile paylaşımlı kullanılıyor *(gizli mod hariç)*
- **Virtual Host Mapping** (`agan.app`) ile iç HTML sayfaları *(NewTab, Agan, Incognito, Onboarding)* dosya sisteminden değil, sanal host üzerinden yükleniyor — daha hızlı ve güvenli
- **Fallback** mekanizması: `Assets` klasörü yoksa inline HTML string'lerle çalışmaya devam ediyor
- **`PersonalityService`** ve **`ReadLaterService`** eklendi
- **`personality.json`** artık `EmbeddedResource` olarak da derleniyor

#### ⚠️ Kaldırılanlar / Değişenler

- Sol paneldeki **uzantı listesi ve kısayollar** kaldırıldı, arayüz sade tutulmaya devam ediyor
- AI hafıza modülleri *(Bilgi Haznesi, Çalışma Alanları)* tamamen kaldırıldı — **Ağan artık tamamen anonim**
- `Environment.Exit(0)` güvencesiyle hayalet süreç sorunu giderildi
- `AganSystemPrompt` çakışmaları giderildi

#### 🐛 Bilinen Sorunlar

- Gizli mod sekmelerinde **uzantılar devre dışı** *(tasarım gereği)*
- `Agan.html` sayfası yüklenirken Ollama bağlantısı henüz kurulmamışsa "Bağlı" durumu gecikebilir
- PDF kaydetme işlemi büyük sayfalarda birkaç saniye sürebilir

#### 🙏 Teşekkürler

Bu sürüm, kullanıcı geri bildirimleri ve topluluk katkılarıyla şekillendi.
Ağan'ı bir asistan olmaktan çıkarıp **kişiliği olan bir yol arkadaşına**
dönüştürme yolculuğunda bize eşlik ettiğiniz için teşekkürler.

> **Bir sonraki sürümde görüşmek üzere — Ağan sizi bekliyor.** 🤖💙

---

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
git clone https://github.com/KOSMO471/Agan-Browser.git
cd Agan-Browser

# 2. Bir Ollama modeli indir (AI için)
ollama pull qwen2.5-coder:7b

# 3. Derle
build.bat

# 4. Çalıştır
run.bat
```

---

## 📁 Proje Yapısı

```
AganBrowser/
├── Assets/
│   ├── Agan.html              # AI sohbet arayüzü
│   ├── NewTab.html            # Yeni sekme (saat, arama, sözler)
│   ├── Incognito.html         # Gizli mod karşılama
│   ├── Onboarding.html        # İlk kullanım tanıtımı
│   ├── quotes.json            # Günün sözleri (130+)
│   ├── personality.json       # 15 kişilik tanımı
│   ├── AdblockLists/          # EasyList & EasyPrivacy
│   └── Extensions/            # Yerleşik uzantılar
├── Models/                    # Veri modelleri
├── Services/                  # İş mantığı (AI, AdBlock, Şifre...)
├── Themes/                    # 5 tema XAML
├── App.xaml.cs                # Uygulama giriş noktası
├── MainWindow.xaml.cs         # Ana pencere + sekme mantığı
├── build.bat                  # Derleme scripti
└── run.bat                    # Çalıştırma scripti
```

---

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Lütfen:

1. Bu repoyu fork'layın
2. Yeni bir feature branch açın (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit'leyin (`git commit -m 'Yeni özellik eklendi'`)
4. Branch'i push'layın (`git push origin feature/yeni-ozellik`)
5. **Pull Request** açın

---

## 📄 Lisans

Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır.

---

## 🔗 Bağlantılar

- 🐛 **Hata Bildir:** [Issues](https://github.com/KOSMO471/Agan-Browser/issues)
- 💬 **Tartışma:** [Discussions](https://github.com/KOSMO471/Agan-Browser/discussions)
- 📥 **Son Sürüm:** [Releases](https://github.com/KOSMO471/Agan-Browser/releases)
- 🌐 **Ollama:** [ollama.com](https://ollama.com)
- 🧩 **WebView2:** [Microsoft Docs](https://developer.microsoft.com/microsoft-edge/webview2/)

---

> **⚠️ Not:** `personality.json`, `quotes.json`, `Incognito.html` ve `Onboarding.html` dosyalarının **`Assets/` klasöründe** bulunması gerekir. `build.bat` çalıştırıldığında bu dosyaların kopyalandığını otomatik kontrol eder.

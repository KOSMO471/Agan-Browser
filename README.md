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
[![Version](https://img.shields.io/badge/version-1.3.0-blue?logo=github)](https://github.com/KOSMO471/Agan-Browser/releases)
[![Release](https://img.shields.io/github/v/release/KOSMO471/Agan-Browser?color=purple&label=release)](https://github.com/KOSMO471/Agan-Browser/releases)
[![Tests](https://img.shields.io/badge/tests-manual%20passed-brightgreen)]()

---

## ✨ Özellikler

### 🤖 Yerel Yapay Zeka Asistanı — Ağan
- **Ollama** ile tamamen yerel LLM entegrasyonu (bulut yok, veri sızıntısı yok)
- **Sayfa bağlamı analizi** — "Bu sayfada ne yapacağım?" diye sorun, aktif sayfanın
  HTML'ini okuyup adım adım anlatsın
- **Streaming yanıt** — kelime kelime yazma, sohbet hissi
- Model seçici (kurulu Ollama modellerini listeler, **seçim kalıcı**)
- Türkçe asistan kişiliği (tamamen anonim, kullanıcıdan veri öğrenmez)
- **🧠 15 Farklı Kişilik** — Ağan'ın karakterini siz belirleyin (Esprili, Resmi, Filozof, Robot...)
- **Akıllı sağ tık** — seçili metni Ağan'a sor, çevirt, detaylı açıklat

### 🌐 Tarayıcı Çekirdeği
- Çoklu sekme (yatay / dikey mod), sürükle-bırak sıralama
- Adres çubuğu + Google arama entegrasyonu
- HTTPS/HTTP güvenlik göstergesi
- Favicon desteği, özel sağ tık menüsü
- Oturum kaydetme (kapatınca sekmeler hatırlanır)
- Klavye kısayolları (`Ctrl+T`, `Ctrl+W`, `Ctrl+L`, `Ctrl+Tab`, `F5`, `Ctrl+R`) — **WebView2 odaktayken de çalışır**
- PDF / HTML olarak sayfa kaydetme
- DevTools açma
- **Durum çubuğu** — link hover, sayfa yükleme, indirme durumu canlı

### 🕶️ Gizli Mod (Incognito)
- İzole WebView2 profili (geçmiş, çerez, şifre bırakmaz)
- Mor temalı özel karşılama sayfası
- Sekmelerde `🕶️` ikonu ile görsel ayrım
- `%TEMP%` klasöründe çalışır, oturum kapanınca silinir
- **Reklam engelleyici gizli modda da aktiftir**
- Gizli URL'ler `session.json`'a **yazılmaz**

### 🖱️ Akıllı Sağ Tık Menüsü
- **Seçileni Ağan'a Sor** / **Türkçeye Çevir** / **Detaylı Açıkla**
- **PiP'de Aç** (video küçük pencerede)
- **Resmi Kaydet**, **Sayfayı PDF Kaydet**
- **Öğeyi İncele** (DevTools)

### 🛡️ Reklam ve İzleyici Engelleyici
- **Topluluk Listeleri:** EasyList ve EasyPrivacy desteği
- **Seçici filtreleme:** sadece `Script`, `Document`, `XmlHttpRequest`, `Image` context'lerini dinler (performans)
- **URL Temizleyici:** `utm_*`, `fbclid`, `gclid` gibi takip parametrelerini temizler
- **Beyaz Liste:** tek tıkla "Muaf Tut"
- **Kozmetik Filtreleme:** CSS ile reklam kutularını gizleme + canlı sayaç
- **Toggle:** aç/kapa → tüm sekmelerde filtre ekle/kaldır + reload

### 📖 Oku Sonra (PDF arşivi)
- Sayfayı **tam PDF olarak** kaydet (site görünümü, resimler korunur)
- `%APPDATA%\AganBrowser\ReadLater\` klasöründe arşiv + `index.json`
- Kayıt sonrası tam yol dialogda gösterilir
- Saniyede birden fazla kayıtta bile çakışma yok (`yyyyMMdd_HHmmss_fff.pdf`)

### 🔐 Şifre Yöneticisi
- **Windows DPAPI** ile şifrelenmiş kasa (`passwords.dat`)
- Site bazlı kayıt, otomatik doldurma

### 🧩 Uzantı Sistemi
- WebView2 tarayıcı uzantıları desteği
- Kurulum: yerleşik klasör, yerel klasör, ZIP, GitHub deposu

### 🎨 Arayüz
- **5 Farklı Tema:** Dark, Dracula, Nord, Light, Sepya — **açılışta korunur**, WebView içeriğine yansır
- Sol / Sağ panel (açılır-kapanır)
- Yer imleri, geçmiş, sekmeler mini listesi
- İndirme yöneticisi
- **Canlı saatli, yer imli, günün sözlü Yeni Sekme sayfası**
- İlk kullanım için interaktif Onboarding

---

## 📅 Sürüm Notları

### v1.3.0 — "Oku Sonra ve Durum Çubuğu" Özellik Sürümü

Bu sürümde mevcut özellikler deneyim açısından iyileştirildi: Oku Sonra artık tam PDF
olarak arşivleniyor, durum çubuğu gerçek zamanlı bilgi gösteriyor ve model seçimi
kalıcı hale getirildi.

#### 📖 Oku Sonra → PDF

Önceden `body.innerHTML` alınıp özel bir HTML şablonuna gömülüyordu. Bu yöntem:
- Sitenin `<head>` CSS'ini kaybediyordu → çoğu sayfa bozuk görünüyordu
- Resimler çevrimdışı açılamıyordu
- Tablo/grid düzeni çöküyordu

**Artık:** `PrintToPdfAsync` ile **tam sayfa PDF** üretiliyor. Site görünümü,
resimler, fontlar korunur; dosya tek başına çevrimdışı açılabilir.

- Konum: `%APPDATA%\AganBrowser\ReadLater\`
- Dosya adı: `YYYYMMDD_HHmmss_fff.pdf` (saniyede birden fazla kayıtta bile çakışmaz)
- Kayıt sonrası tam yol dialogda gösterilir
- `index.json` ile liste yönetimi (PDF mevcut değilse otomatik filtrelenir)

#### 🖥️ Durum Çubuğu (StatusText)

Önceden `"Hazır"` sabit yazıyordu. Artık **öncelik sırasına göre** canlı bilgi gösteriyor:

1. **Aktif indirme** → `⬇ dosya.pdf - %42`
2. **Link hover** → `https://hedef-adres.com` (WebView2 `StatusBarText` event'i)
3. **Navigasyon** → `⏳ Yükleniyor...` / `✓ Hazır` / `⚠ Yüklenemedi (kod)`
4. Boşta → son durum

İndirme takibi `DownloadItem.PropertyChanged` üzerinden (`_downloadService.Downloads.CollectionChanged` ile senkron).

#### 🧠 Model Seçimi Kalıcı

Model ComboBox'tan seçilen model `settings.json`'a yazılıyor. Uygulama kapatılıp
açıldığında seçim korunuyor. Önceden her açılışta `qwen2.5-coder:7b`'ye dönüyordu.

#### 🧪 Test

| Senaryo | Durum |
|---------|-------|
| Oku Sonra → PDF üretildi ve açıldı | ✅ |
| PDF çevrimdışı açılabiliyor | ✅ |
| Oku Sonra liste `index.json`'dan yüklendi | ✅ |
| Status bar indirme başlayınca `⬇` gösteriyor | ✅ |
| Status bar link hover'da URL gösteriyor | ✅ |
| Status bar sayfa geçişinde `⏳` → `✓` | ✅ |
| Model seç → kapat → aç → korundu | ✅ |

---

### v1.2.1 — "Kararlılık" Düzeltme Sürümü

Bu sürüm, v1.2.0'da tespit edilen hataların giderilmesine ve tarayıcı çekirdeğinin
performansının iyileştirilmesine odaklandı. Yeni özellik eklenmedi; mevcut deneyim
sağlamlaştırıldı.

#### 🔧 Düzeltmeler

| # | Sorun | Çözüm |
|---|-------|-------|
| 1 | Tema seçimi diskte kayıtlı olsa da uygulama hep Dark açılıyordu | `ThemeService.Initialize()` çağrısı `App()` constructor'ından `OnStartup`'a taşındı |
| 2 | Gizli sekmede favicon geldiğinde `🕶️` işareti kayboluyor, gizli URL'ler `session.json`'a sızıyordu | `TabItem.Tag` üç amaç için kullanılıyordu; `_incognitoTabs` ve `_tabContextUrls` ayrı tutuldu |
| 3 | "Özetle", "Metin Üret", "Çeviri Yap", sağ tık → "Ağan'a Sor" sessizce çalışmıyordu | `Agan.html`'e `chat` handler'ı eklendi |
| 4 | AdBlocker YouTube videolarını, GitHub repolarını engelliyordu | Cross-origin `Script`/`Iframe` engelleme mantığı kaldırıldı; sadece kara listedeki domainler engellenir |
| 5 | `example.com` beyaz listedeyse `notexample.com` da muaf sayılıyordu | `EndsWith("." + d)` ile suffix eşleşmesi düzeltildi |
| 6 | WebView2 odaktayken Ctrl+T/W/L çalışmıyordu | `AreBrowserAcceleratorKeysEnabled = false` |
| 7 | Sağ tık menüsü pencere sol-üstünde açılıyordu | `Placement = MousePoint` |
| 8 | `Environment.Exit(0)` ile sert kapanış, `SaveSession` hatasında kilitlenme | `Application.Shutdown()` + try/catch |
| 9 | Sekme sürükleme iptal edilince `_draggedTab` sızıntısı | `try/finally` |
| 10 | `GetSharedEnvironmentAsync` yarış durumu → iki ayrı çerez profili | `_sharedEnvTask` cache |
| 11 | `localhost:3000`, `file://`, ondalık (`1.5`) Google aramasına düşüyordu | `NormalizeUrl` regex + scheme kontrolü |
| 12 | `DialogHelper.Info/Error` tema dışı beyaz `MessageBox` | Temalı özel pencere |
| 13 | Özetleme sırasında `Task.Delay(500)` umut temelli bekleme | `_aganReadyTasks` await |
| 14 | Uzun Ollama yanıtı sırasında sekme kapanınca stream devam ediyordu | `CancellationToken` + `_aganCancellation` |
| 15 | OAuth/ödeme popup'ları normal sekmeye dönüştürülüyordu | `about:blank`/boş URI için `e.Handled = false` |
| 16 | `Ctrl+Tab` derleme hatası (`ModifierKeys` vs `Key`) | `ModifierKeys.Control` |
| 17 | `CoreWebView2WebResourceContext.SubFrame` derleme hatası | `Document` ile değiştirildi |
| 18 | Adblock toggle ilk saniye yanlış gösteriyordu | `IsEnabled` diske persist |

#### ⚡ Optimizasyonlar

| # | Optimizasyon | Kazanç |
|---|-------------|--------|
| 1 | `_coreToTab` ters arama tablosu — 11 yerdeki `FirstOrDefault` O(1)'e indi | 20+ sekmede UI donması azaldı |
| 2 | Adblock filtre seçici — `All` yerine `Document+Script+XHR+Image`; kapalıyken hiç filter eklenmez | YouTube/Instagram'da %10-20 sayfa yükleme hızlanması |
| 3 | `ApplyThemeToHtmlAsync` çift guard — sadece `agan.app` host'una | Harici sitelerin renk bozulması bitti |
| 4 | `UpdateAdBlockLabel` 3 sn + `_activeHost` cache + `_lastBlockedCount` | Sürekli CPU %2-5 düşüş |
| 5 | `_coreMainDomain` cache — her istekte URI parse yok | Medya yoğun sayfalarda %1-3 |
| 6 | `RefreshPageInfoAsync` panel kapalıyken erken return | Büyük sayfalarda 50-500ms tasarruf |
| 7 | `HistoryService.Add` debounce (5 sn timer + dirty flag) | Disk I/O %90 azaldı |
| 8 | `RefreshTabsMini` delta sync | Sekme sürükle-bırak takılması bitti |
| 9 | `RegisterTab`/`UnregisterTab` helper'ları | Senkronizasyon hatası riski sıfır |

#### 🎨 Davranış Değişiklikleri

- Adblock toggle artık tüm sekmeleri reload eder — reklamlar anında yok olur
- Gizli modda adblock aktif — önceden çalışmıyordu
- Durum çubuğu öncelik sırası: indirme → link hover → navigasyon

#### 🧹 Temizlik

- `ShortcutService.cs`, `ShortcutItem.cs`, `ChatResponse`, `ThemeService.Toggle`, `DownloadService.Pause/Resume/Cancel`, `ReadLaterService.Delete/Folder`, `MainWindow.LoadExtensionsAsync` kaldırıldı (hepsi ölü koddu)

#### 🧪 Test

Tüm düzeltmeler ve optimizasyonlar **elle test edildi ve geçti**:

| Senaryo | Durum |
|---------|-------|
| YouTube video oynatımı (adblock açık) | ✅ |
| GitHub repo sayfası yüklenmesi | ✅ |
| 20+ sekme, hızlı Ctrl+Tab | ✅ |
| Adblock toggle → tüm sekmeler reload | ✅ |
| Gizli modda YouTube → reklamlar engellendi | ✅ |
| Tema seç → kapat → aç → korundu | ✅ |
| Ağan özetleme / sağ tık → Ağan'a sor | ✅ |
| Status bar: `⬇` / hover / `⏳`→`✓` | ✅ |
| Ctrl+T/W/L WebView2 odaktayken | ✅ |
| Sağ tık menüsü fare konumunda | ✅ |
| `localhost:3000`, `file:///` normalize | ✅ |
| Sağ panel kapalı → `PageInfoText` boş, açınca dolu | ✅ |
| `history.json` 5 sn aralıklı yazım | ✅ |
| Gizli sekme `session.json`'a sızmıyor | ✅ |

---

### v1.2.0 — "Kişilik ve Gizlilik" Güncellemesi

Bu sürümde Ağan sadece bir asistan değil; size özel bir karaktere bürünen, gizli modda
iz bırakmadan çalışan ve tarayıcı deneyimini baştan sona kişiselleştirebilen bir yol
arkadaşına dönüştü.

#### 🧠 Ağan Artık Kişilik Sahibi — 15 Farklı Karakter

Bu sürümün yıldızı, Ağan'ın ruh halini ve konuşma tarzını tamamen sizin kontrolünüze
veren **Kişilik Sistemi** oldu.

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

#### 🎨 5 Farklı Tema

| Tema | Renk Paleti |
|------|-------------|
| 🌙 **Dark** | Klasik koyu (`#1A1A1A`) |
| 🧛 **Dracula** | Mor-pembe tonlar (`#282A36`) |
| ❄️ **Nord** | Soğuk mavi-gri (`#2E3440`) |
| ☀️ **Light** | Klasik açık (`#F5F5F5`) |
| 📜 **Sepya** | Kitap sayfası sıcaklığı (`#F4ECD8`) |

> **💡 Not:** Tema değişikliği artık sadece WPF arayüzüne değil, `--bg`, `--surface`,
> `--text`, `--accent` gibi CSS değişkenleriyle **açık sayfalara da** otomatik uygulanıyor.

#### 🖱️ Akıllı Sağ Tık — Metni Seç, Ağan'a Sor

- **🤖 Seçileni Ağan'a Sor** — *"Bu metin ne anlatıyor? Açıkla:"*
- **🌐 Türkçeye Çevir** — *"Şu metni Türkçeye çevir:"*
- **💡 Açıkla (Detaylı)** — *"Bu metni detaylıca, örneklerle açıkla:"*
- **📺 PiP'de Aç** — Videoları küçük pencerede izleyin
- **🖼️ Resmi Kaydet** — Sağ tıklanan resmi diske kaydedin
- **📄 Sayfayı PDF Kaydet** — DevTools açmadan PDF çıktısı
- **🔍 Öğeyi İncele** — DevTools'u açın

#### 📖 Oku Sonra

- **📖 Oku Sonra** butonu sol panelde
- Makale özel bir HTML şablonuyla kaydedilir *(v1.3.0'da PDF'e geçildi)*
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
- 📥 **İndirme Yöneticisi** — İndirmeleri gerçek zamanlı takip
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

#### 🐛 Bilinen Sorunlar (v1.2.0 itibarıyla)

- Gizli mod sekmelerinde **uzantılar devre dışı** *(tasarım gereği)*
- `Agan.html` sayfası yüklenirken Ollama bağlantısı henüz kurulmamışsa "Bağlı" durumu gecikebilir
- PDF kaydetme işlemi büyük sayfalarda birkaç saniye sürebilir

---

### v1.1.0 — "Gizlilik ve Hız" Güncellemesi

Bu sürümde odak noktamız, tarayıcının arka planında çalışan reklam ve izleyici
engelleme mekanizmasını çok daha agresif ve akıllı hale getirmek oldu. Ayrıca
arayüzdeki gereksiz kalabalıklar temizlendi.

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

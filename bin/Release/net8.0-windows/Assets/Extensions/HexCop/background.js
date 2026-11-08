chrome.runtime.onInstalled.addListener(() => {
  chrome.contextMenus.create({
    id: "agan-tum-gorseller",
    title: "Ağan: Bu sayfadaki tüm görselleri indir",
    contexts: ["page"]
  });
  chrome.contextMenus.create({
    id: "agan-bu-gorsel",
    title: "Ağan: Bu görseli indir",
    contexts: ["image"]
  });
});

chrome.contextMenus.onClicked.addListener(async (info, tab) => {
  if (info.menuItemId === "agan-tum-gorseller" && tab?.id) {
    await tumGorselleriIndir(tab.id);
  } else if (info.menuItemId === "agan-bu-gorsel" && info.srcUrl) {
    await tekGorselIndir(info.srcUrl);
  }
});

chrome.runtime.onMessage.addListener((mesaj, sender, cevap) => {
  if (mesaj?.tip === "toplu-indir" && Array.isArray(mesaj.gorseller)) {
    topluIndir(mesaj.gorseller).then((sonuc) => cevap(sonuc));
    return true;
  }
  return false;
});

async function tumGorselleriIndir(tabId) {
  try {
    const [sonuc] = await chrome.scripting.executeScript({
      target: { tabId: tabId },
      func: () =>
        Array.from(document.images)
          .map((img) => img.src)
          .filter((src) => src && (src.startsWith("http://") || src.startsWith("https://")))
    });

    const gorseller = sonuc?.result || [];
    if (gorseller.length === 0) {
      rozetGoster("0", "#E74C3C");
      setTimeout(rozetTemizle, 3000);
      return;
    }

    const indirilen = await topluIndir(gorseller);
    rozetGoster(String(indirilen), indirilen > 0 ? "#27AE60" : "#E74C3C");
    setTimeout(rozetTemizle, 4000);
  } catch (err) {
    console.error("Toplu indirme hatası:", err);
    rozetGoster("!", "#E74C3C");
    setTimeout(rozetTemizle, 4000);
  }
}

async function tekGorselIndir(url) {
  try {
    await chrome.downloads.download({
      url: url,
      conflictAction: "uniquify",
      saveAs: false
    });
    rozetGoster("1", "#27AE60");
    setTimeout(rozetTemizle, 2500);
  } catch (err) {
    console.error("İndirme hatası:", err);
    rozetGoster("!", "#E74C3C");
    setTimeout(rozetTemizle, 2500);
  }
}

async function topluIndir(gorseller) {
  let basarili = 0;
  for (const url of gorseller) {
    try {
      await chrome.downloads.download({
        url: url,
        conflictAction: "uniquify",
        saveAs: false
      });
      basarili++;
    } catch (err) {
      console.warn("Görsel atlandı:", url, err);
    }
  }
  return basarili;
}

function rozetGoster(metin, renk) {
  chrome.action.setBadgeText({ text: metin });
  chrome.action.setBadgeBackgroundColor({ color: renk });
}

function rozetTemizle() {
  chrome.action.setBadgeText({ text: "" });
}
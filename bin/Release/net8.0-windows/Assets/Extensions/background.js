chrome.action.onClicked.addListener(async (tab) => {
  const [result] = await chrome.scripting.executeScript({
    target: { tabId: tab.id },
    func: () => {
      const title = document.title || "(başlık yok)";
      const url = location.href;
      const words = (document.body.innerText || "").trim().split(/\s+/).filter(Boolean).length;
      const images = document.images.length;
      const links = document.links.length;
      return { title, url, words, images, links };
    }
  });
  if (!result || !result.result) return;
  const r = result.result;
  await chrome.scripting.executeScript({
    target: { tabId: tab.id },
    func: (d) => {
      const old = document.getElementById("__agan_page_info__");
      if (old) old.remove();
      const box = document.createElement("div");
      box.id = "__agan_page_info__";
      box.style.cssText = "position:fixed;top:16px;right:16px;z-index:2147483647;background:#1A1A1A;color:#fff;padding:14px 18px;border-radius:10px;border:1px solid #1177BB;font-family:'Segoe UI',sans-serif;font-size:13px;box-shadow:0 4px 20px rgba(0,0,0,0.4);max-width:340px;line-height:1.5;";
      box.innerHTML =
        "<div style='font-weight:bold;color:#1E8EDB;margin-bottom:6px;'>Sayfa Bilgisi</div>" +
        "<div><b>Başlık:</b> " + d.title.replace(/[<>&]/g, "") + "</div>" +
        "<div><b>URL:</b> " + d.url.replace(/[<>&]/g, "") + "</div>" +
        "<div><b>Kelime:</b> " + d.words + "</div>" +
        "<div><b>Görsel:</b> " + d.images + "</div>" +
        "<div><b>Bağlantı:</b> " + d.links + "</div>" +
        "<div style='margin-top:8px;font-size:11px;color:#D0D0D0;'>Kapatmak için tıkla</div>";
      box.addEventListener("click", () => box.remove());
      document.body.appendChild(box);
      setTimeout(() => box.remove(), 8000);
    },
    args: [r]
  });
});
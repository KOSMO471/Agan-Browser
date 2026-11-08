let isDark = false;
chrome.action.onClicked.addListener(async (tab) => {
  isDark = !isDark;
  await chrome.scripting.executeScript({
    target: { tabId: tab.id },
    func: (dark) => {
      const id = "__agan_dark_mod__";
      const old = document.getElementById(id);
      if (old) old.remove();
      if (!dark) {
        document.documentElement.style.filter = "";
        document.querySelectorAll("img,video,canvas,svg").forEach(e => e.style.filter = "");
        return;
      }
      document.documentElement.style.filter = "invert(1) hue-rotate(180deg)";
      document.querySelectorAll("img,video,canvas,svg").forEach(e => {
        e.style.filter = "invert(1) hue-rotate(180deg)";
      });
      const badge = document.createElement("div");
      badge.id = id;
      badge.textContent = "🌙 Koyu Mod Açık";
      badge.style.cssText = "position:fixed;bottom:16px;right:16px;z-index:2147483647;background:#1A1A1A;color:#fff;padding:8px 14px;border-radius:8px;font-family:'Segoe UI',sans-serif;font-size:12px;border:1px solid #1177BB;box-shadow:0 4px 12px rgba(0,0,0,.4);";
      document.body.appendChild(badge);
      setTimeout(() => badge.remove(), 4000);
    },
    args: [isDark]
  });
});
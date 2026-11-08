async function yukle() {
  const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
  const [result] = await chrome.scripting.executeScript({
    target: { tabId: tab.id },
    func: () => Array.from(document.images).map(img => ({ src: img.src, alt: img.alt || "" }))
  });
  const liste = document.getElementById("liste");
  const gorseller = (result?.result || []).filter(g => g.src && g.src.startsWith("http"));
  if (gorseller.length === 0) {
    liste.innerHTML = '<div id="bos">Bu sayfada görsel bulunamadı.</div>';
    return;
  }
  gorseller.forEach((g, i) => {
    const div = document.createElement("div");
    div.className = "gorsel";
    const img = document.createElement("img");
    img.src = g.src;
    img.alt = g.alt;
    const span = document.createElement("span");
    span.textContent = "Görsel " + (i + 1);
    span.style.fontSize = "12px";
    const btn = document.createElement("button");
    btn.textContent = "İndir";
    btn.onclick = () => {
      chrome.downloads.download({ url: g.src, conflictAction: "uniquify" });
    };
    div.appendChild(img);
    div.appendChild(span);
    div.appendChild(btn);
    liste.appendChild(div);
  });
}
yukle();
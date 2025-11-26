window.downloadFileFromStream = async (fileName, contentStreamReference) => {
  const arrayBuffer = await contentStreamReference.arrayBuffer();
  const blob = new Blob([arrayBuffer]);
  const url = URL.createObjectURL(blob);
  const anchorElement = document.createElement('a');
  anchorElement.href = url;
  anchorElement.download = fileName ?? '';
  anchorElement.click();
  anchorElement.remove();
  URL.revokeObjectURL(url);
}

window.getSelectedFileInfos = (inputId) => {
  const input = document.getElementById(inputId);
  if (!input || !input.files) return [];
  return Array.from(input.files).map(f => ({ name: f.name, size: f.size, type: f.type }));
}

window.uploadFilesAndDownload = async (inputId, endpoint) => {
  const input = document.getElementById(inputId);
  if (!input || !input.files || input.files.length === 0) {
    throw new Error('No files selected');
  }

  const form = new FormData();
  for (let i = 0; i < input.files.length; i++) {
    form.append('files', input.files[i], input.files[i].name);
  }

  let resp;
  try {
    resp = await fetch(endpoint, { method: 'POST', body: form });
  } catch (err) {
    // Common causes: network failure, CORS/preflight blocked, or untrusted dev certs
    const msg = err && err.message ? err.message : String(err);
    throw new Error(`Network error: Failed to fetch. Possible causes: CORS denied, dev certificate untrusted, or network failure. Original: ${msg}`);
  }

  if (!resp.ok) {
    const text = await resp.text();
    throw new Error(`Upload failed: ${resp.status} ${text}`);
  }

  const blob = await resp.blob();
  const contentDisposition = resp.headers.get('content-disposition');
  let fileName = 'archive.zip';
  if (contentDisposition) {
    const match = /filename\*=UTF-8''([^;]+)|filename="?([^\"]+)"?/.exec(contentDisposition);
    if (match) fileName = decodeURIComponent(match[1] || match[2]);
  }

  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = fileName;
  a.click();
  a.remove();
  URL.revokeObjectURL(url);
  return true;
}
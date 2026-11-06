(() => {
  const message = document.getElementById('message');
  const input = document.getElementById('fileInput');
  const welcomeHero = document.getElementById('welcomeHero');
  const chatThread = document.getElementById('chatThread');
  const contentSection = document.querySelector('.content');
  const centerScroll = document.getElementById('centerScroll');
  const chatAttachedFiles = document.getElementById('chatAttachedFiles');

  const files = []; // Session files: { file, time, id, url }
  const pendingChatFiles = []; // Files waiting to be sent: { file, time, id, url }
  let currentConversationId = null;
  let isSending = false;

  // Get anti-forgery token
  function getToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
  }

  function escapeHtml(text) {
    if (!text) return '';
    return text.replace(/[&<>"']/g, c => ({
      '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
    }[c]));
  }

  function formatTime(date) {
    const d = date ? new Date(date) : new Date();
    const hh = String(d.getHours()).padStart(2, '0');
    const mm = String(d.getMinutes()).padStart(2, '0');
    return `${hh}:${mm}`;
  }

  function getFileInfo(file) {
    const nameLower = (file.name || file.fileName || '').toLowerCase();
    const type = file.type || file.contentType || '';
    if (nameLower.endsWith('.mp3') || type.startsWith('audio/')) {
      return { icon: 'bi-file-earmark-music-fill', color: '#e03131', bg: '#ffe3e3', type: 'Audio' };
    }
    if (nameLower.endsWith('.mp4') || type.startsWith('video/')) {
      return { icon: 'bi-file-earmark-play-fill', color: '#f59f00', bg: '#fff3bf', type: 'Video' };
    }
    if (/\.(png|jpe?g)$/.test(nameLower) || type.startsWith('image/')) {
      return { icon: 'bi-file-earmark-image-fill', color: '#2f9e44', bg: '#d3f9d8', type: 'Image' };
    }
    if (nameLower.endsWith('.pdf') || type === 'application/pdf') {
      return { icon: 'bi-file-earmark-pdf-fill', color: '#e03131', bg: '#ffe3e3', type: 'PDF' };
    }
    if (nameLower.endsWith('.docx') || nameLower.endsWith('.doc')) {
      return { icon: 'bi-file-earmark-word-fill', color: '#1971c2', bg: '#d0ebff', type: 'Word' };
    }
    return { icon: 'bi-file-earmark-text-fill', color: '#495057', bg: '#e9ecef', type: 'Document' };
  }

  // Mở modal xem trước tệp (hỗ trợ ảnh, video, audio, pdf, doc)
  function previewFile(item) {
    const file = item.file || item;
    const fileName = file.name || file.fileName || 'Attached file';
    const fileSize = file.size || file.fileSize || 0;
    const fileType = file.type || file.contentType || '';
    const info = getFileInfo(file);
    const modalEl = document.getElementById('filePreviewModal');
    if (!modalEl || !window.bootstrap) return;

    document.getElementById('filePreviewModalLabel').textContent = fileName;
    document.getElementById('filePreviewMeta').textContent =
      `Size: ${(fileSize / 1024).toFixed(1)} KB · Type: ${info.type} · ${item.time ? 'Uploaded at: ' + item.time : ''}`;

    const body = document.getElementById('filePreviewBody');
    const downloadBtn = document.getElementById('fileDownloadBtn');
    const fileUrl = item.url || (file instanceof File ? URL.createObjectURL(file) : '');

    if (downloadBtn) {
      downloadBtn.href = fileUrl;
      downloadBtn.setAttribute('download', fileName);
    }

    const nameLower = fileName.toLowerCase();
    if (/\.(png|jpe?g)$/.test(nameLower) || fileType.startsWith('image/')) {
      body.innerHTML = `<img src="${fileUrl}" alt="${escapeHtml(fileName)}" class="img-fluid rounded" style="max-height: 480px; object-fit: contain;">`;
    } else if (nameLower.endsWith('.mp4') || fileType.startsWith('video/')) {
      body.innerHTML = `
        <video controls autoplay class="w-100 rounded" style="max-height: 440px;">
          <source src="${fileUrl}" type="${fileType || 'video/mp4'}">
          Your browser does not support video playback.
        </video>`;
    } else if (nameLower.endsWith('.mp3') || fileType.startsWith('audio/')) {
      body.innerHTML = `
        <div class="py-4 text-center w-100">
          <i class="bi bi-music-note-beamed text-danger" style="font-size: 52px;"></i>
          <h6 class="mt-3 mb-3 fw-bold">${escapeHtml(fileName)}</h6>
          <audio controls autoplay class="w-75">
            <source src="${fileUrl}" type="${fileType || 'audio/mpeg'}">
            Your browser does not support audio playback.
          </audio>
        </div>`;
    } else {
      body.innerHTML = `
        <div class="py-4 text-center">
          <i class="bi ${info.icon}" style="font-size: 54px; color: ${info.color};"></i>
          <h6 class="mt-3 fw-bold">${escapeHtml(fileName)}</h6>
          <p class="text-muted small">This file type cannot be previewed here. Use the <b>Download</b> button below to open it on your device.</p>
        </div>`;
    }

    const modal = new window.bootstrap.Modal(modalEl);
    modal.show();
  }

  // Render files waiting to be sent in the chat composer
  function renderPendingChatFiles() {
    if (!chatAttachedFiles) return;
    if (pendingChatFiles.length === 0) {
      chatAttachedFiles.style.display = 'none';
      chatAttachedFiles.innerHTML = '';
      return;
    }
    chatAttachedFiles.style.display = 'flex';
    chatAttachedFiles.innerHTML = '';

    pendingChatFiles.forEach((item, index) => {
      const info = getFileInfo(item.file);
      const chip = document.createElement('div');
      chip.className = 'chat-attach-chip';
      chip.innerHTML = `
        <i class="bi ${info.icon}" style="color:${info.color}; font-size:12px;"></i>
        <span title="${escapeHtml(item.file.name)}">${escapeHtml(item.file.name)}</span>
        <button type="button" class="chat-attach-remove" title="Remove this file" aria-label="Remove file">&times;</button>
      `;
      chip.querySelector('.chat-attach-remove').onclick = () => {
        pendingChatFiles.splice(index, 1);
        renderPendingChatFiles();
      };
      chatAttachedFiles.appendChild(chip);
    });
  }

  // Render the attachment list in the session files panel
  function renderFiles() {
    const lists = [
      document.getElementById('fileList'),
      document.getElementById('fileListMobile')
    ].filter(Boolean);

    const counts = [
      document.getElementById('fileCount'),
      document.getElementById('fileCountMobile')
    ].filter(Boolean);

    const empties = [
      document.getElementById('emptyFiles'),
      document.getElementById('emptyFilesMobile')
    ].filter(Boolean);

    const wrappers = [
      document.getElementById('fileHistoryWrapper'),
      document.getElementById('fileHistoryWrapperMobile')
    ].filter(Boolean);

    const clearBtns = [
      document.getElementById('clearFilesBtn'),
      document.getElementById('clearFilesBtnMobile')
    ].filter(Boolean);

    lists.forEach(list => {
      list.innerHTML = '';
      files.forEach((item, index) => {
        const file = item.file;
        const info = getFileInfo(file);
        const fileName = file.name || file.fileName || 'File';
        const fileSize = file.size || file.fileSize || 0;
        const row = document.createElement('div');
        row.className = 'file-row';
        row.innerHTML = `
          <div class="file-row-icon" style="background:${info.bg}; color:${info.color};">
            <i class="bi ${info.icon}"></i>
          </div>
          <div class="file-row-info">
            <span class="file-row-name" title="${escapeHtml(fileName)}">${escapeHtml(fileName)}</span>
            <div class="file-row-meta">
              <span>${(fileSize / 1048576).toFixed(2)} MB</span>
              <span>·</span>
              <span><i class="bi bi-clock me-1"></i>${item.time || ''}</span>
            </div>
          </div>
          <div class="file-row-actions">
            <button type="button" class="file-action-btn preview-btn" title="Preview file">
              <i class="bi bi-eye"></i>
            </button>
            <button type="button" class="file-action-btn delete-btn" title="Remove file from list">
              <i class="bi bi-trash3"></i>
            </button>
          </div>
        `;

        row.querySelector('.preview-btn').onclick = () => previewFile(item);
        row.querySelector('.delete-btn').onclick = () => {
          files.splice(index, 1);
          renderFiles();
        };
        list.appendChild(row);
      });
    });

    counts.forEach(c => c.textContent = files.length);
    const hasFiles = files.length > 0;
    empties.forEach(e => e.style.display = hasFiles ? 'none' : 'block');
    wrappers.forEach(w => w.style.display = hasFiles ? 'block' : 'none');
    clearBtns.forEach(btn => btn.style.display = hasFiles ? 'inline-block' : 'none');
  }

  function addFiles(selected, fromChatBar = false) {
    const nowTime = formatTime(new Date());
    for (const file of selected) {
      if (file.size > 10 * 1024 * 1024) {
        alert(`${file.name} exceeds the 10 MB file size limit.`);
        continue;
      }
      const fileUrl = URL.createObjectURL(file);
      const item = { file, time: nowTime, id: Date.now() + Math.random(), url: fileUrl };

      // Add the file to the session files panel
      if (!files.some(f => (f.file.name || f.file.fileName) === file.name && (f.file.size || f.file.fileSize) === file.size)) {
        files.unshift(item);
      }

      // Queue the file when it was selected from the chat composer
      if (fromChatBar) {
        pendingChatFiles.push(item);
      }
    }
    renderFiles();
    renderPendingChatFiles();
  }

  // Clear all files in the right panel
  ['clearFilesBtn', 'clearFilesBtnMobile'].forEach(id => {
    const btn = document.getElementById(id);
    if (btn) {
      btn.addEventListener('click', () => {
        if (files.length === 0) return;
        if (confirm('Are you sure you want to remove all files from this list?')) {
          files.length = 0;
          pendingChatFiles.length = 0;
          renderFiles();
          renderPendingChatFiles();
        }
      });
    }
  });

  let uploadSourceIsChat = false;
  const openFiles = (fromChat = false) => {
    uploadSourceIsChat = fromChat;
    input.click();
  };

  const uploadBtn = document.getElementById('uploadButton');
  if (uploadBtn) uploadBtn.addEventListener('click', () => openFiles(true));

  ['headerUpload', 'chooseInline', 'chooseButton', 'chooseInlineMobile', 'chooseButtonMobile'].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.addEventListener('click', () => openFiles(false));
  });

  input.addEventListener('change', () => {
    addFiles([...input.files], uploadSourceIsChat);
    input.value = '';
    uploadSourceIsChat = false;
  });

  // Handle drag and drop for both desktop and mobile drop zones
  ['dropzone', 'dropzoneMobile'].forEach(id => {
    const dz = document.getElementById(id);
    if (!dz) return;
    ['dragenter', 'dragover'].forEach(n => dz.addEventListener(n, e => {
      e.preventDefault();
      dz.classList.add('drag');
    }));
    ['dragleave', 'drop'].forEach(n => dz.addEventListener(n, e => {
      e.preventDefault();
      dz.classList.remove('drag');
    }));
    dz.addEventListener('drop', e => {
      if (e.dataTransfer && e.dataTransfer.files) {
        addFiles([...e.dataTransfer.files], false);
      }
    });
  });

  // Count characters and auto-expand the composer
  function adjustTextareaHeight() {
    message.style.height = 'auto';
    const newHeight = Math.min(message.scrollHeight, 140);
    message.style.height = `${Math.max(newHeight, 38)}px`;
  }

  message.addEventListener('input', () => {
    const len = message.value.length;
    document.getElementById('charCount').textContent = len.toLocaleString('en-US') + ' / 8,000';
    adjustTextareaHeight();
  });

  // Select a sample scenario
  document.querySelectorAll('.example').forEach(btn => {
    btn.addEventListener('click', () => {
      message.value = btn.dataset.example || '';
      message.dispatchEvent(new Event('input'));
      message.focus();
    });
  });

  // Render chat messages
  function renderChatMessage(msg) {
    if (msg.role === 'user' || msg.sender === 'user') {
      const userDiv = document.createElement('div');
      userDiv.className = 'chat-msg-user';

      let attachmentsHtml = '';
      const atts = msg.attachments || [];
      if (atts.length > 0) {
        attachmentsHtml = '<div class="chat-msg-attachments">';
        atts.forEach(att => {
          const fileName = att.fileName || att.file?.name || '';
          const nameLower = fileName.toLowerCase();
          const contentType = att.contentType || att.file?.type || '';
          const isImg = /\.(png|jpe?g)$/.test(nameLower) || contentType.startsWith('image/');
          const fileUrl = att.url || (att.file instanceof File ? URL.createObjectURL(att.file) : '');

          if (isImg && fileUrl) {
            attachmentsHtml += `<img src="${fileUrl}" alt="${escapeHtml(fileName)}" class="chat-attachment-img" data-att-url="${fileUrl}">`;
          } else {
            const info = getFileInfo(att.file || att);
            attachmentsHtml += `
              <div class="chat-attachment-card" data-att-url="${fileUrl}">
                <i class="bi ${info.icon}"></i>
                <span class="text-truncate" style="max-width:180px;">${escapeHtml(fileName)}</span>
              </div>
            `;
          }
        });
        attachmentsHtml += '</div>';
      }

      userDiv.innerHTML = `
        <div class="chat-bubble-user">
          ${attachmentsHtml}
          <div class="chat-user-row">
            <span class="chat-user-text">${escapeHtml(msg.content || msg.text || '')}</span>
            <span class="chat-user-time">${msg.time || formatTime()}</span>
          </div>
        </div>
      `;

      // Click to preview
      userDiv.querySelectorAll('.chat-attachment-img, .chat-attachment-card').forEach(el => {
        el.onclick = () => {
          const url = el.dataset.attUrl;
          if (url) window.open(url, '_blank');
        };
      });

      chatThread.appendChild(userDiv);
    } else {
      const botDiv = document.createElement('div');
      botDiv.className = 'chat-msg-bot';
      const riskLevel = msg.analysis?.riskLevel;
      const isHigh = riskLevel === 'High' || msg.high === true;
      const isMedium = riskLevel === 'Medium';
      const badgeClass = isHigh ? 'danger' : isMedium ? 'warning' : 'safe';
      const badgeText = isHigh
        ? 'High-risk warning'
        : isMedium ? 'Suspicious indicators found' : 'No clear indicators found';
      const advice = msg.analysis?.advice || msg.advice || '';

      botDiv.innerHTML = `
        <div class="chat-bot-avatar" aria-hidden="true">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
            <path d="M12 2.5 20 5v6.2c0 5-3.2 8.4-8 10.3-4.8-1.9-8-5.3-8-10.3V5l8-2.5Z"/>
            <path d="m8.7 11.8 2.2 2.2 4.5-4.8"/>
          </svg>
        </div>
        <div class="chat-bubble-bot">
          <div class="chat-bot-header">
            <span class="chat-bot-title">
              <i class="bi bi-shield-check text-primary"></i> Safeline AI Assistant
            </span>
            <span class="chat-bot-badge ${badgeClass}">
              <i class="bi ${isHigh ? 'bi-exclamation-triangle-fill me-1' : isMedium ? 'bi-shield-exclamation me-1' : 'bi-check-circle-fill me-1'}"></i>
              ${badgeText}
            </span>
          </div>
          <div class="chat-bot-text">${msg.content || msg.text || ''}</div>
          ${advice ? `
          <div class="chat-bot-tips">
            <b><i class="bi bi-lightbulb-fill text-warning me-1"></i> Safety recommendation:</b>
            <div class="mt-1">${advice}</div>
          </div>
          ` : ''}
          <div class="text-end mt-2 text-muted" style="font-size:10px;">${msg.time || formatTime()}</div>
        </div>
      `;
      chatThread.appendChild(botDiv);
    }
  }

  // Load conversation history from backend
  async function loadConversations(searchQuery = '') {
    try {
      const url = searchQuery
        ? `/Chat?handler=Conversations&search=${encodeURIComponent(searchQuery)}`
        : '/Chat?handler=Conversations';

      const response = await fetch(url);
      const data = await response.json();

      if (data.success) {
        renderHistoryList(data.conversations);
      }
    } catch (error) {
      console.error('Error loading conversations:', error);
    }
  }

  // Render history list
  function renderHistoryList(conversations) {
    const lists = [
      document.getElementById('historyList'),
      document.getElementById('historyListMobile')
    ].filter(Boolean);

    const counts = [
      document.getElementById('historyCount'),
      document.getElementById('historyCountMobile')
    ].filter(Boolean);

    const clearBtns = [
      document.getElementById('clearHistoryBtn'),
      document.getElementById('clearHistoryBtnMobile')
    ].filter(Boolean);

    lists.forEach(list => {
      list.innerHTML = '';
      if (!conversations || conversations.length === 0) {
        list.innerHTML = '<small class="text-muted text-center py-3 d-block" style="font-size:11px;">No conversations yet</small>';
        return;
      }

      conversations.forEach(conv => {
        const item = document.createElement('div');
        const isActive = currentConversationId === conv.id;
        item.className = `history-item ${isActive ? 'active' : ''}`;
        item.innerHTML = `
          <button type="button" class="history-item-content" data-id="${conv.id}">
            <span class="history-item-icon"><i class="bi bi-chat-left-text"></i></span>
            <span class="history-item-title" title="${escapeHtml(conv.title)}">${escapeHtml(conv.title)}</span>
            <span class="history-item-time">${conv.time}</span>
          </button>
          <button type="button" class="history-del-btn" data-id="${conv.id}" title="Delete this conversation">
            <i class="bi bi-trash3"></i>
          </button>
        `;

        item.querySelector('.history-item-content').onclick = () => loadConversation(conv.id);
        item.querySelector('.history-del-btn').onclick = (e) => {
          e.stopPropagation();
          deleteConversation(conv.id);
        };
        list.appendChild(item);
      });
    });

    const cVal = conversations ? conversations.length : 0;
    counts.forEach(c => c.textContent = cVal);
    clearBtns.forEach(btn => {
      btn.style.display = cVal > 0 ? 'inline-block' : 'none';
    });
  }

  // Load specific conversation messages
  async function loadConversation(conversationId) {
    try {
      const response = await fetch(`/Chat?handler=Messages&conversationId=${conversationId}`);
      const data = await response.json();

      if (data.success) {
        currentConversationId = conversationId;
        chatThread.innerHTML = '';
        welcomeHero.style.display = 'none';
        contentSection.classList.add('chatting');

        if (data.messages) {
          data.messages.forEach(msg => {
            renderChatMessage(msg);
            // Also populate files panel if message has attachments
            if (msg.attachments && msg.attachments.length > 0) {
              msg.attachments.forEach(att => {
                if (!files.some(f => (f.file.fileName || f.file.name) === att.fileName)) {
                  files.unshift({ file: att, time: msg.time, url: att.url, id: att.id });
                }
              });
            }
          });
          renderFiles();
        }

        setTimeout(() => {
          centerScroll.scrollTo({ top: centerScroll.scrollHeight, behavior: 'smooth' });
        }, 50);

        // Close mobile offcanvas
        const offcanvas = bootstrap.Offcanvas.getInstance(document.getElementById('sidebarOffcanvas'));
        if (offcanvas) offcanvas.hide();

        loadConversations();
      }
    } catch (error) {
      console.error('Error loading conversation:', error);
    }
  }

  // Delete conversation
  async function deleteConversation(conversationId) {
    if (!confirm('Are you sure you want to delete this conversation?')) return;

    try {
      const response = await fetch('/Chat?handler=DeleteConversation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': getToken()
        },
        body: JSON.stringify({ conversationId })
      });

      const data = await response.json();
      if (data.success) {
        if (currentConversationId === conversationId) {
          resetChat();
        }
        loadConversations();
      }
    } catch (error) {
      console.error('Error deleting conversation:', error);
      alert('Something went wrong while deleting the conversation.');
    }
  }

  // Clear all conversations
  async function clearAllConversations() {
    if (!confirm('Are you sure you want to delete all conversations?')) return;
    try {
      const response = await fetch('/Chat?handler=ClearAllConversations', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': getToken()
        }
      });
      const data = await response.json();
      if (data.success) {
        resetChat();
        loadConversations();
      }
    } catch (error) {
      console.error('Error clearing conversations:', error);
      alert('Something went wrong while clearing the conversation list.');
    }
  }

  // Clear all history buttons
  ['clearHistoryBtn', 'clearHistoryBtnMobile'].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.onclick = clearAllConversations;
  });

  // Send message with files
  async function sendMessage() {
    if (isSending) return;

    const text = message.value.trim();
    const hasFiles = pendingChatFiles.length > 0;

    if (!text && !hasFiles) {
      message.focus();
      return;
    }

    isSending = true;

    // Prepare FormData
    const formData = new FormData();
    if (currentConversationId) {
      formData.append('conversationId', currentConversationId);
    }
    formData.append('message', text);

    pendingChatFiles.forEach(item => {
      formData.append('files', item.file);
    });

    // Clear input immediately
    message.value = '';
    message.style.height = '38px';
    message.dispatchEvent(new Event('input'));
    pendingChatFiles.length = 0;
    renderPendingChatFiles();

    // Show loading spinner
    welcomeHero.style.display = 'none';
    contentSection.classList.add('chatting');

    const loadingDiv = document.createElement('div');
    loadingDiv.className = 'text-center py-3';
    loadingDiv.innerHTML = '<div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">Analyzing...</span></div>';
    chatThread.appendChild(loadingDiv);
    centerScroll.scrollTo({ top: centerScroll.scrollHeight, behavior: 'smooth' });

    try {
      const response = await fetch('/Chat?handler=SendMessage', {
        method: 'POST',
        headers: {
          'RequestVerificationToken': getToken()
        },
        body: formData
      });

      const data = await response.json();

      // Remove loading
      loadingDiv.remove();

      if (data.success) {
        if (!currentConversationId) {
          currentConversationId = data.conversationId;
        }

        // Render user message
        renderChatMessage(data.userMessage);

        // Render assistant response
        renderChatMessage(data.assistantMessage);

        // Add attachments to files list if any
        if (data.userMessage.attachments && data.userMessage.attachments.length > 0) {
          data.userMessage.attachments.forEach(att => {
            if (!files.some(f => (f.file.fileName || f.file.name) === att.fileName)) {
              files.unshift({ file: att, time: data.userMessage.time, url: att.url, id: att.id });
            }
          });
          renderFiles();
        }

        // Scroll to bottom
        setTimeout(() => {
          centerScroll.scrollTo({ top: centerScroll.scrollHeight, behavior: 'smooth' });
        }, 50);

        // Reload conversation list
        loadConversations();
      } else {
        alert(data.error || 'Something went wrong.');
      }
    } catch (error) {
      loadingDiv.remove();
      console.error('Error sending message:', error);
      alert('Something went wrong while sending your message.');
    } finally {
      isSending = false;
      message.focus();
    }
  }

  // Reset to new chat
  function resetChat() {
    currentConversationId = null;
    message.value = '';
    message.style.height = '38px';
    pendingChatFiles.length = 0;
    renderPendingChatFiles();
    message.dispatchEvent(new Event('input'));
    chatThread.innerHTML = '';
    welcomeHero.style.display = 'block';
    contentSection.classList.remove('chatting');
    message.focus();
    loadConversations();
  }

  // Form submit
  document.getElementById('analyzer').addEventListener('submit', e => {
    e.preventDefault();
    sendMessage();
  });

  // Keyboard shortcuts
  message.addEventListener('keydown', e => {
    // Send with Ctrl/Cmd + Enter or Enter without Shift
    if (e.key === 'Enter') {
      if (e.ctrlKey || e.metaKey || !e.shiftKey) {
        e.preventDefault();
        sendMessage();
      }
      // Shift + Enter inserts a new line (textarea default)
    }
  });

  document.addEventListener('keydown', e => {
    if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
      e.preventDefault();
      resetChat();
    }
  });

  // New conversation buttons
  ['newChat', 'newChatMobile'].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.addEventListener('click', resetChat);
  });

  // Search
  ['search', 'searchMobile'].forEach(id => {
    const s = document.getElementById(id);
    if (s) {
      let debounceTimeout;
      s.addEventListener('input', (e) => {
        clearTimeout(debounceTimeout);
        debounceTimeout = setTimeout(() => {
          loadConversations(e.target.value);
        }, 200);
      });
    }
  });

  // Initialize
  renderFiles();
  loadConversations();
})();

document.addEventListener('DOMContentLoaded', function() {
  // 要素の取得
  const chatToggleBtn = document.getElementById('chat-toggle-btn');
  const chatWindow = document.getElementById('chat-window');
  const chatCloseBtn = document.getElementById('chat-close-btn');
  const chatForm = document.getElementById('chat-form');
  const messageInput = document.getElementById('message-input');
  const chatMessages = document.getElementById('chat-messages');

  // チャットウィンドウの表示/非表示を切り替える
  function toggleChat() {
    chatWindow.classList.toggle('hidden');
    if (!chatWindow.classList.contains('hidden')) {
      messageInput.focus();
      // メッセージエリアを最下部にスクロール
      chatMessages.scrollTop = chatMessages.scrollHeight;
    }
  }

  // メッセージを追加する
  function addMessage(text, isSent) {
    const messageDiv = document.createElement('div');
    messageDiv.classList.add('message');
    messageDiv.classList.add(isSent ? 'sent' : 'received');

    const messageContent = document.createElement('div');
    messageContent.classList.add('message-content');
    messageContent.textContent = text;

    messageDiv.appendChild(messageContent);
    chatMessages.appendChild(messageDiv);

    // メッセージエリアを最下部にスクロール
    chatMessages.scrollTop = chatMessages.scrollHeight;
  }

  // 自動応答を送信する
  function sendAutoResponse() {
    setTimeout(() => {
      addMessage('ありがとうございます。メッセージを受け取りました。すぐにご連絡いたします。', false);
    }, 1000);
  }

  // イベントリスナーの設定
  chatToggleBtn.addEventListener('click', toggleChat);
  chatCloseBtn.addEventListener('click', toggleChat);

  chatForm.addEventListener('submit', function(e) {
    e.preventDefault();
    const message = messageInput.value.trim();
    
    if (message) {
      addMessage(message, true);
      messageInput.value = '';
      sendAutoResponse();
    }
  });
});
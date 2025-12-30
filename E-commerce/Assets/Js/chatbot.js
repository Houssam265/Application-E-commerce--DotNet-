(function(){
  var root = document.createElement('div');
  root.id = 'chatbot-widget';
  root.innerHTML = [
    '<button id="chatbot-toggle" aria-label="Ouvrir le chat">',
      '<svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">',
        '<path d="M21 15a4 4 0 0 1-4 4H7l-4 4V7a4 4 0 0 1 4-4h10a4 4 0 0 1 4 4z"></path>',
      '</svg>',
    '</button>',
    '<div id="chatbot-panel">',
      '<div class="cb-header">',
        '<div class="cb-title">',
          '<span>Assistant</span>',
        '</div>',
        '<div class="cb-actions">',
          '<button id="cb-minimize" title="Fermer">',
            '<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">',
              '<line x1="5" y1="12" x2="19" y2="12"/>',
            '</svg>',
          '</button>',
        '</div>',
      '</div>',
      '<div class="cb-body" id="cb-body"></div>',
      '<div class="cb-typing" id="cb-typing"><span>Assistant est en train d\'écrire…</span></div>',
      '<div class="cb-input">',
        '<textarea id="cb-input" placeholder="Votre message... (Entrée pour envoyer, Shift+Entrée pour saut)"></textarea>',
        '<button id="cb-send">Envoyer</button>',
      '</div>',
    '</div>'
  ].join('');

  function mount(){
    if(!document.getElementById('chatbot-widget')){
      document.body.appendChild(root);
      bind();
      restore();
    }
  }

  function bind(){
    var toggle = document.getElementById('chatbot-toggle');
    var panel = document.getElementById('chatbot-panel');
    var close = document.getElementById('cb-minimize');
    var send = document.getElementById('cb-send');
    var input = document.getElementById('cb-input');

    toggle.addEventListener('click', function(){
      panel.classList.toggle('open');
      localStorage.setItem('cb-open', panel.classList.contains('open') ? '1' : '0');
    });
    close.addEventListener('click', function(){
      panel.classList.remove('open');
      localStorage.setItem('cb-open','0');
    });
    send.addEventListener('click', doSend);

    input.addEventListener('keydown', function(e){
      if(e.key === 'Enter' && !e.shiftKey){
        e.preventDefault();
        doSend();
      }
    });
  }

  function restore(){
    var opened = localStorage.getItem('cb-open') === '1';
    if(opened){
      document.getElementById('chatbot-panel').classList.add('open');
    }
    var saved = JSON.parse(localStorage.getItem('cb-history')||'[]');
    saved.forEach(function(m){ addMessage(m.role, m.text); });
    if(saved.length === 0){
      addMessage('bot', 'Bonjour, je suis votre assistant virtuel. Comment puis-je vous aider ?');
    }
  }

  function persist(role, text){
    try{
      var arr = JSON.parse(localStorage.getItem('cb-history')||'[]');
      arr.push({ role: role, text: text, t: Date.now() });
      if(arr.length > 50) arr = arr.slice(-50);
      localStorage.setItem('cb-history', JSON.stringify(arr));
    }catch(e){}
  }

  function addMessage(role, text){
    var body = document.getElementById('cb-body');
    var wrap = document.createElement('div');
    wrap.className = 'cb-msg ' + (role === 'user' ? 'user' : 'bot');
    var b = document.createElement('div');
    b.className = 'bubble';
    b.textContent = text;
    wrap.appendChild(b);
    body.appendChild(wrap);
    body.scrollTop = body.scrollHeight;
  }

  function setTyping(show){
    var t = document.getElementById('cb-typing');
    if(show) t.classList.add('show'); else t.classList.remove('show');
  }

  function doSend(){
    var input = document.getElementById('cb-input');
    var text = (input.value||'').trim();
    if(!text) return;
    addMessage('user', text);
    persist('user', text);
    input.value = '';

    setTyping(true);

    fetch(window.location.origin + '/ChatbotService.asmx/SendMessage', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json; charset=utf-8' },
      body: JSON.stringify({ message: text })
    })
    .then(function(r){ return r.json(); })
    .then(function(data){
      var payload = data && data.d ? data.d : data;
      if(typeof payload === 'string'){
        try { payload = JSON.parse(payload); } catch(e) {}
      }
      var reply = payload && payload.Reply ? payload.Reply : 'Désolé, je n\'ai pas pu répondre.';
      addMessage('bot', reply);
      persist('bot', reply);
    })
    .catch(function(){
      var msg = 'Erreur de connexion au service du chatbot.';
      addMessage('bot', msg);
      persist('bot', msg);
    })
    .finally(function(){ setTyping(false); });
  }

  if(document.readyState === 'loading'){
    document.addEventListener('DOMContentLoaded', mount);
  } else {
    mount();
  }
})();

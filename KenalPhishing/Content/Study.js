// @ts-nocheck
var eduModalElement = document.getElementById('eduModal');
var myModal = null;

if (typeof window['bootstrap'] !== 'undefined' && eduModalElement) {
    myModal = new window['bootstrap'].Modal(eduModalElement);
}

function foundRedFlag(element) {
    if (!element) return;
    var msg = document.getElementById('successMsg');
    if (msg) msg.classList.remove('d-none');
    var urlBar = document.querySelector('.url-bar');
    if (urlBar) {
        urlBar.style.backgroundColor = '#fee2e2';
        urlBar.style.border = '1px solid #dc2626';
    }
    var urlText = document.querySelector('.url-bar span');
    if (urlText) urlText.style.color = '#dc2626';
    element.style.border = "2px dashed #dc2626";
    element.style.backgroundColor = "#fee2e2";
    element.style.color = "#dc2626";
    element.style.padding = "2px 6px";
    element.style.borderRadius = "5px";
}

function finalDecision(type) {
    if (!myModal) return;
    if (type === 'phishing') {
        showProfessionalResult('success', 'Tahniah! Keputusan Tepat.', 'Hebat!',
            'Anda berjaya mengesan taktik pancingan data (phishing) ini. Dengan melaporkan halaman ini, anda telah menyelamatkan diri dari ancaman siber.',
            'Scammer selalunya menggunakan taktik <b>desakan (urgency)</b> atau <b>tawaran palsu</b> untuk membuat mangsa panik. Sikap berwaspada anda adalah langkah keselamatan yang terbaik.',
            'Seterusnya');
    } else {
        showProfessionalResult('danger', 'Anda Terjerat!', 'Perhatian',
            'Anda telah mengesahkan simulasi ini sebagai laman web yang selamat. Ini adalah satu kesilapan dan data anda berisiko dicuri.',
            'Jangan mudah percaya hanya pada rupa visual dan logo rasmi. Scammer boleh menyalin reka bentuk asal dengan mudah. Sentiasa perhatikan ejaan e-mel dan pautan URL.',
            'Cuba Semula');
    }
}

function showProfessionalResult(status, title, subtitle, mainText, adviceText, btnText) {
    var h = document.getElementById('mHeader');
    var advBox = document.getElementById('mAdviceBox');
    var btnP = document.getElementById('btnPrimary');
    var t = document.getElementById('mTitle');
    var sub = document.getElementById('mSubtitle');
    var txt = document.getElementById('mText');
    var advTxt = document.getElementById('mAdviceText');
    var btnPText = document.getElementById('btnPrimaryText');
    var btnS = document.getElementById('btnSecondary');
    var icon = document.getElementById('mIcon');
    var btnPIcon = document.getElementById('btnPrimaryIcon');

    if (!h || !advBox || !btnP || !t || !sub || !txt || !advTxt || !btnPText || !btnS || !icon || !btnPIcon) return;

    h.className = "p-4 text-center position-relative transition-all";
    advBox.className = "p-3 mb-4 rounded-3 d-flex gap-3 align-items-start border";
    btnP.className = "btn py-3 fw-bold rounded-pill shadow-sm transition-all";

    t.innerText = title;
    sub.innerText = subtitle;
    txt.innerHTML = "<b>Situasi:</b><br/>" + mainText;
    advTxt.innerHTML = adviceText;
    btnPText.innerText = btnText;

    if (status === 'success') {
        h.classList.add('bg-success-gradient');
        icon.setAttribute("name", "shield-checkmark");
        sub.className = "badge text-uppercase tracking-wide mb-3 px-3 py-2 rounded-pill bg-success text-white";
        advBox.classList.add('advice-success');
        btnP.classList.add('btn-success');
        btnPIcon.setAttribute("name", "arrow-forward-outline");
        btnS.classList.add('d-none');
    } else if (status === 'danger') {
        h.classList.add('bg-danger-gradient');
        icon.setAttribute("name", "close-circle");
        sub.className = "badge text-uppercase tracking-wide mb-3 px-3 py-2 rounded-pill bg-danger text-white";
        advBox.classList.add('advice-danger');
        btnP.classList.add('btn-danger');
        btnPIcon.setAttribute("name", "refresh-outline");
        btnS.classList.remove('d-none');
    }

    if (myModal) myModal.show();
}

function closeModalAndProceed() {
    var btnPText = document.getElementById('btnPrimaryText');
    if (!btnPText) return;
    if (btnPText.innerText === 'Seterusnya') {
        var nextBtn = document.getElementById('btnNextPage');
        if (nextBtn && nextBtn.getAttribute('href')) {
            window.location.href = nextBtn.getAttribute('href');
        } else {
            if (myModal) myModal.hide();
        }
    } else {
        resetSimulation();
    }
}

function resetSimulation() {
    if (myModal) myModal.hide();
    var msg = document.getElementById('successMsg');
    if (msg) msg.classList.add('d-none');
}
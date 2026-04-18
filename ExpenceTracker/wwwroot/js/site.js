document.addEventListener("DOMContentLoaded", function () {
    // 1. Theme toggle
    const themeToggleBtn = document.getElementById('theme-toggle');
    if (themeToggleBtn) {
        let currentTheme = localStorage.getItem('theme');
        if (!currentTheme) {
            currentTheme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        }
        document.documentElement.setAttribute('data-bs-theme', currentTheme);
        themeToggleBtn.innerText = currentTheme === 'dark' ? '☀️' : '🌙';

        themeToggleBtn.addEventListener('click', function () {
            currentTheme = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-bs-theme', currentTheme);
            localStorage.setItem('theme', currentTheme);
            themeToggleBtn.innerText = currentTheme === 'dark' ? '☀️' : '🌙';
        });
    }

    // 2. Badge pill toggle
    document.querySelectorAll('.badge-toggle').forEach(btn => {
        function toggleBadge(el) {
            el.classList.toggle('selected');
            el.setAttribute('aria-pressed', el.classList.contains('selected'));
            if (el.classList.contains('selected')) {
                el.style.backgroundColor = el.getAttribute('data-color') || 'var(--primary-color)';
                el.style.borderColor = el.getAttribute('data-color') || 'var(--primary-color)';
            } else {
                el.style.backgroundColor = 'transparent';
                el.style.borderColor = el.getAttribute('data-color') || 'var(--primary-color)';
            }
            updateBadgeIds();
        }
        btn.addEventListener('click', function () { toggleBadge(this); });
        btn.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                toggleBadge(this);
            }
        });
        
        // initialize styles
        if (btn.classList.contains('selected')) {
            btn.style.backgroundColor = btn.getAttribute('data-color') || 'var(--primary-color)';
            btn.style.borderColor = btn.getAttribute('data-color') || 'var(--primary-color)';
        } else {
            btn.style.backgroundColor = 'transparent';
            btn.style.borderColor = btn.getAttribute('data-color') || 'var(--primary-color)';
            btn.style.color = btn.getAttribute('data-color') || 'var(--text-color)';
        }
    });

    function updateBadgeIds() {
        const selected = Array.from(document.querySelectorAll('.badge-toggle.selected'))
                              .map(b => b.getAttribute('data-id'));
        const input = document.getElementById('BadgeIds');
        if (input) {
            input.value = selected.join(',');
        }
    }

    // 3. Delete confirmation
    document.querySelectorAll('.btn-delete').forEach(btn => {
        btn.addEventListener('click', function (e) {
            const msg = this.getAttribute('data-confirm') || 'Are you sure?';
            if (!confirm(msg)) {
                e.preventDefault();
            }
        });
    });

    // 4. Quick Entry Enhancements
    const toastSuccess = document.querySelector('.toast-success');
    if (toastSuccess) {
        setTimeout(() => {
            toastSuccess.style.opacity = '0';
            setTimeout(() => toastSuccess.remove(), 300);
        }, 3000);

        const nameInput = document.getElementById('NameInput');
        if (nameInput) nameInput.focus();
    }

    document.addEventListener('keydown', function(e) {
        if (e.ctrlKey && e.key === 'Enter') {
            const form = document.querySelector('form'); // any open form
            if (form) {
                // if it's the expense form, it will submit
                form.submit();
            }
        }
    });
});

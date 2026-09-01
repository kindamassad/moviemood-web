document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.password-field').forEach(function(wrapper) {
        const input = wrapper.querySelector('input');
        const toggle = wrapper.querySelector('.password-toggle');
        const icon = toggle?.querySelector('i');

        if (!input || !toggle || !icon) {
            return;
        }

        toggle.addEventListener('click', function() {
            const showPassword = input.type === 'password';
            input.type = showPassword ? 'text' : 'password';
            icon.classList.toggle('fa-eye', !showPassword);
            icon.classList.toggle('fa-eye-slash', showPassword);
            toggle.setAttribute('aria-label', showPassword ? 'Hide password' : 'Show password');
        });
    });
});

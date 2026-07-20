$(function () {
    // ---- Show/Hide password (Nielsen: user control & freedom) ----
    $('.toggle-eye').on('click', function () {
        var target = $('#' + $(this).data('target'));
        var isPwd = target.attr('type') === 'password';
        target.attr('type', isPwd ? 'text' : 'password');
        $(this).text(isPwd ? '🙈' : '👁');
    });

    // ---- Password strength meter (visibility of system status) ----
    var $pwd = $('#Password');
    if ($pwd.length) {
        var $fill = $('#strengthFill');
        var $label = $('#strengthLabel');
        // build 4 segments
        for (var i = 0; i < 4; i++) $fill.append('<div class="seg"></div>');

        $pwd.on('input', function () {
            var val = $(this).val();
            var score = 0;
            if (val.length >= 8) score++;
            if (/[A-Z]/.test(val) && /[a-z]/.test(val)) score++;
            if (/\d/.test(val)) score++;
            if (/[^A-Za-z0-9]/.test(val)) score++;

            var colors = ['#E4DCCC', '#C0392B', '#E67E22', '#E67E22', '#2E7D32'];
            var labels = ['', 'Weak', 'Fair', 'Good', 'Strong'];

            $fill.find('.seg').each(function (idx) {
                $(this).css('background', idx < score ? colors[score] : '#E4DCCC');
            });
            $label.text(labels[score]).css('color', colors[score]);
        });
    }

    // ---- File upload feedback ----
    $('#fileInput').on('change', function () {
        var name = this.files.length ? this.files[0].name : '';
        $('#fileName').text(name);
    });

    // ---- Error prevention: disable submit until terms accepted ----
    var $submitBtn = $('#submitBtn');
    var $terms = $('#AcceptTerms');
    function toggleSubmit() {
        if ($terms.length) $submitBtn.prop('disabled', !$terms.is(':checked'));
    }
    if ($terms.length) {
        toggleSubmit();
        $terms.on('change', toggleSubmit);
    }
});

// Placeholder wiring for social buttons — real OAuth handled server-side
function socialSignup(provider) {
    if (provider === 'Apple') {
        alert('Apple Sign-In requires an Apple Developer account and additional setup — coming soon.');
        return;
    }
    document.forms[0].action = '/Account/ExternalLogin';
    var input = document.createElement('input');
    input.type = 'hidden'; input.name = 'provider'; input.value = provider;
    document.forms[0].appendChild(input);
    document.forms[0].submit();
}
function socialLogin(provider) {
    socialSignup(provider);
}
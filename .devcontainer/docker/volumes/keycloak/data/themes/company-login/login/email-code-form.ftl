<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=true; section>
    <#if section = "header">
        ${msg("emailAuthenticatorCodeTitle","Email Verification")?no_esc}
    <#elseif section = "form">
        <div id="resend-message" class="alert alert-success" style="display:none;" role="alert">
            ${msg("emailAuthenticatorCodeResent","Verification code has been resent to your email")?no_esc}
        </div>
        <p class="mb-3">A verification code has been sent to your email. Please enter it below.</p>
        <form action="${url.loginAction}" method="post" id="email-code-form">
            <div class="mb-3">
                <label for="emailCode" class="form-label">${msg("emailAuthenticatorCode","Verification Code")}</label>
                <input type="text" id="emailCode" name="emailCode" class="form-control" autocomplete="off" autofocus />
            </div>
            <div class="d-grid gap-2">
                <button type="submit" name="login" value="true" class="btn btn-primary">${msg("doSubmit")}</button>
                <button type="submit" name="resend" value="true" id="resend-btn" class="btn btn-secondary">${msg("emailAuthenticatorResend","Resend Code")}</button>
                <button type="submit" name="cancel" value="true" class="btn btn-outline-secondary">${msg("doCancel")}</button>
            </div>
        </form>
        <script>
            document.getElementById('resend-btn').addEventListener('click', function() {
                sessionStorage.setItem('resendClicked', 'true');
            });
            if (sessionStorage.getItem('resendClicked') === 'true' && !${message?has_content?c}) {
                document.getElementById('resend-message').style.display = 'block';
                sessionStorage.removeItem('resendClicked');
            } else if (${message?has_content?c}) {
                sessionStorage.removeItem('resendClicked');
            }
        </script>
    </#if>
</@layout.registrationLayout>

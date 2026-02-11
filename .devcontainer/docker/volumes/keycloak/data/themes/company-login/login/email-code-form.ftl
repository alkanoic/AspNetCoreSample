<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=true; section>
    <#if section = "header">
        ${msg("emailAuthenticatorCodeTitle","Email Verification")?no_esc}
    <#elseif section = "form">
        <div id="resend-message" style="display:none; padding: 10px; margin-bottom: 15px; background-color: #d4edda; border: 1px solid #c3e6cb; color: #155724; border-radius: 4px;">
            ${msg("emailAuthenticatorCodeResent","Verification code has been resent to your email")?no_esc}
        </div>
        <p>A verification code has been sent to your email. Please enter it below.</p>
        <form action="${url.loginAction}" method="post" id="email-code-form">
            <div>
                <label for="emailCode">${msg("emailAuthenticatorCode","Verification Code")}</label>
                <input type="text" id="emailCode" name="emailCode" autocomplete="off" autofocus />
            </div>
            <div>
                <button type="submit" name="login" value="true">${msg("doSubmit")}</button>
                <button type="submit" name="resend" value="true" id="resend-btn">${msg("emailAuthenticatorResend","Resend Code")}</button>
                <button type="submit" name="cancel" value="true">${msg("doCancel")}</button>
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

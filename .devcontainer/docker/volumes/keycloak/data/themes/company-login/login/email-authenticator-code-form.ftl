<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('totp'); section>
    <#if section = "header">
        ${msg("emailAuthenticatorCodeTitle","Email Verification")?no_esc}
    <#elseif section = "form">
        <p>${msg("emailAuthenticatorCodeMessage","Please enter the verification code sent to your email")?no_esc}</p>
        <form action="${url.loginAction}" method="post">
            <div>
                <label for="code">${msg("emailAuthenticatorCode","Verification Code")}</label>
                <input type="text" id="code" name="code" autocomplete="off" autofocus />
            </div>
            <div>
                <button type="submit">${msg("doSubmit")}</button>
                <button type="submit" name="resend" value="true">${msg("emailAuthenticatorResend","Resend Code")}</button>
            </div>
        </form>
    </#if>
</@layout.registrationLayout>

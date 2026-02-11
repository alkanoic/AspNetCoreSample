<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('totp'); section>
    <#if section = "header">
        ${msg("emailAuthenticatorSetupTitle")?no_esc}
    <#elseif section = "form">
        <p>${msg("emailAuthenticatorSetupMessage")?no_esc}</p>
        <form action="${url.loginAction}" method="post">
            <input type="hidden" name="mode" value="email"/>
            <div>
                <button type="submit" name="submitAction" value="Send">${msg("emailAuthenticatorSendCode","Send Code")}</button>
                <button type="submit" name="submitAction" value="Cancel">${msg("doCancel")}</button>
            </div>
        </form>
    </#if>
</@layout.registrationLayout>

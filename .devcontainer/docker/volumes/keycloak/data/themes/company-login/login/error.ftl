<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=false; section>
    <#if section = "form">
    <div id="kc-error-message">
        <p class="instruction">${(message.summary)!''}</p>
    </div>
    </#if>
</@layout.registrationLayout>

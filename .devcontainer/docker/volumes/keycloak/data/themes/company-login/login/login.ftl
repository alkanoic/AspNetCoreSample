<#import "template.ftl" as layout>
<@layout.registrationLayout; section>
    <#if section = "form">
    <form id="kc-form-login" action="${url.loginAction}" method="post">
        <div class="mb-3">
            <label for="company" class="form-label">${msg('company')}</label>
            <input id="company" name="company" type="text" class="form-control" autofocus="autofocus" />
        </div>
        <div class="mb-3">
            <label for="username" class="form-label">${msg('username')}</label>
            <input id="username" name="username" type="text" class="form-control" />
        </div>
        <div class="mb-3">
            <label for="password" class="form-label">${msg('password')}</label>
            <input id="password" name="password" type="password" class="form-control" />
        </div>
        <div class="d-grid">
            <button type="submit" class="btn btn-primary btn-lg">${msg('doLogIn')}</button>
        </div>
    </form>
    </#if>
</@layout.registrationLayout>

<#macro registrationLayout bodyClass="" displayInfo=false displayMessage=true displayRequiredFields=false showAnotherWayIfPresent=true>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="robots" content="noindex, nofollow">
    <title>${msg("loginTitle",(realm.displayName!''))}</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <div class="container">
        <div class="row justify-content-center align-items-center min-vh-100">
            <div class="col-md-6 col-lg-4">
                <#if realm.internationalizationEnabled  && locale.supported?size gt 1>
                    <div id="kc-locale" style="position: absolute; top: 20px; right: 20px;">
                        <div id="kc-locale-wrapper">
                            <div class="btn-group">
                                <#list locale.supported as l>
                                    <a href="${l.url}" class="btn btn-sm btn-outline-secondary ${(locale.currentLanguageTag == l.languageTag)?then('active', '')}">${l.label}</a>
                                </#list>
                            </div>
                        </div>
                    </div>
                </#if>
                <div class="card shadow">
                    <div class="card-body p-4">
                        <div id="kc-header" class="text-center mb-4">
                            <h1 class="h3">${kcSanitize(msg("loginTitleHtml",(realm.displayNameHtml!'')))?no_esc}</h1>
                        </div>
                        <#if displayMessage && message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
                            <div class="alert alert-${(message.type == 'error')?then('danger', (message.type == 'warning')?then('warning', 'success'))} alert-dismissible fade show" role="alert">
                                ${kcSanitize(message.summary)?no_esc}
                            </div>
                        </#if>
                        <#nested "form">
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
</#macro>

<#-- Custom login form with company code field -->
<#-- Based on Keycloak base theme login.ftl -- modify names: company, username, password -->
<!DOCTYPE html>
<html>
    <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>${msg('loginTitle')!"Sign in"}</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
  </head>
  <body class="bg-light">
    <div class="container">
      <div class="row justify-content-center align-items-center min-vh-100">
        <div class="col-md-6 col-lg-4">
          <div class="card shadow">
            <div class="card-body p-4">
              <h1 class="card-title text-center mb-4">${msg('loginTitle')!"Sign in"}</h1>
              <#if message?has_content>
                <div class="alert alert-${message.type == 'error'?then('danger', 'success')} alert-dismissible fade show" role="alert">
                  ${message.summary}
                </div>
              </#if>
              <form id="kc-form-login" action="${url.loginAction}" method="post">
                <div class="mb-3">
                  <label for="company" class="form-label">企業コード</label>
                  <input id="company" name="company" type="text" class="form-control" autofocus="autofocus" />
                </div>
                <div class="mb-3">
                  <label for="username" class="form-label">ユーザー名</label>
                  <input id="username" name="username" type="text" class="form-control" />
                </div>
                <div class="mb-3">
                  <label for="password" class="form-label">パスワード</label>
                  <input id="password" name="password" type="password" class="form-control" />
                </div>
                <div class="d-grid">
                  <button type="submit" class="btn btn-primary btn-lg">${msg('doLogIn')!"Login"}</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
  </body>
</html>

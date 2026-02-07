<#-- Custom login form with company code field -->
<#-- Based on Keycloak base theme login.ftl -- modify names: company, username, password -->
<#assign msg = .global>
<!DOCTYPE html>
<html>
  <head>
    <meta charset="UTF-8" />
    <title>${msg['loginTitle']!"Sign in"}</title>
  </head>
  <body>
    <div id="kc-login">
      <h1>${msg['loginTitle']!"Sign in"}</h1>
      <form id="kc-form-login" action="${url.loginAction}" method="post">
        <div>
          <label for="company">企業コード</label>
          <input id="company" name="company" type="text" autofocus="autofocus" />
        </div>
        <div>
          <label for="username">ユーザー名</label>
          <input id="username" name="username" type="text" />
        </div>
        <div>
          <label for="password">パスワード</label>
          <input id="password" name="password" type="password" />
        </div>
        <div>
          <input type="submit" value="${msg['doLogIn']!"Login"}" />
        </div>
      </form>
    </div>
  </body>
</html>

package com.example.keycloak.auth;

import org.keycloak.authentication.AuthenticationFlowContext;
import org.keycloak.authentication.Authenticator;
public class CompanyUsernameAuthenticator implements Authenticator {
    @Override
    public void authenticate(AuthenticationFlowContext context) {
        Object form = context.getHttpRequest().getDecodedFormParameters();
        try {
            java.lang.reflect.Method getFirst = form.getClass().getMethod("getFirst", Object.class);
            String company = (String) getFirst.invoke(form, "company");
            String username = (String) getFirst.invoke(form, "username");
            if (company != null && company.length() > 0 && username != null) {
                String combined = company + username;
                java.lang.reflect.Method putSingle = form.getClass().getMethod("putSingle", Object.class, Object.class);
                putSingle.invoke(form, "username", combined);
            }
        } catch (Exception e) {
            // if reflection fails, do nothing and continue
        }
        context.success();
    }

    @Override
    public void action(AuthenticationFlowContext context) {
        context.success();
    }

    @Override
    public boolean requiresUser() { return false; }

    @Override
    public boolean configuredFor(org.keycloak.models.KeycloakSession session, org.keycloak.models.RealmModel realm, org.keycloak.models.UserModel user) { return true; }

    @Override
    public void setRequiredActions(org.keycloak.models.KeycloakSession session, org.keycloak.models.RealmModel realm, org.keycloak.models.UserModel user) { }

    @Override
    public void close() { }
}

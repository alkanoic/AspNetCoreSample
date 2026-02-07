package com.example.keycloak.auth;

import org.keycloak.authentication.Authenticator;
import org.keycloak.authentication.AuthenticatorFactory;
import org.keycloak.models.AuthenticationExecutionModel.Requirement;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.KeycloakSessionFactory;

import java.util.Collections;

public class CompanyUsernameAuthenticatorFactory implements AuthenticatorFactory {
    public static final String PROVIDER_ID = "company-username-authenticator";

    @Override
    public Authenticator create(KeycloakSession session) {
        return new CompanyUsernameAuthenticator();
    }

    @Override
    public void init(org.keycloak.Config.Scope config) { }

    @Override
    public void postInit(KeycloakSessionFactory factory) { }

    @Override
    public void close() { }

    @Override
    public String getId() { return PROVIDER_ID; }

    @Override
    public String getReferenceCategory() { return null; }

    @Override
    public boolean isConfigurable() { return false; }

    @Override
    public String getDisplayType() { return "Company Username Authenticator"; }

    @Override
    public boolean isUserSetupAllowed() { return false; }

    @Override
    public Requirement[] getRequirementChoices() { return new Requirement[] { Requirement.REQUIRED, Requirement.ALTERNATIVE, Requirement.DISABLED }; }

    @Override
    public String getHelpText() { return "Prepends company code to username from the login form"; }

    @Override
    public java.util.List<org.keycloak.provider.ProviderConfigProperty> getConfigProperties() { return Collections.emptyList(); }
}

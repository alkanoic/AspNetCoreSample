package com.example.keycloak.auth;

import java.util.logging.Level;
import java.util.logging.Logger;
import org.keycloak.authentication.AuthenticationFlowContext;
import org.keycloak.authentication.Authenticator;
public class CompanyUsernameAuthenticator implements Authenticator {
    private static final Logger logger = Logger.getLogger(CompanyUsernameAuthenticator.class.getName());
    @Override
    public void authenticate(AuthenticationFlowContext context) {
        // Render/login step — present the login form as a challenge so action() is invoked on POST.
        logger.fine("CompanyUsernameAuthenticator: authenticate() called — presenting challenge for form submission");
        try {
            Object formProvider = context.form();
            Object resp = null;
            try {
                java.lang.reflect.Method m = formProvider.getClass().getMethod("createForm", String.class);
                resp = m.invoke(formProvider, "login.ftl");
            } catch (NoSuchMethodException nsme) {
                try {
                    java.lang.reflect.Method m2 = formProvider.getClass().getMethod("createForm");
                    resp = m2.invoke(formProvider);
                } catch (Exception ignore) {
                    // createForm unavailable in this Keycloak version
                }
            }

            if (resp != null) {
                // invoke context.challenge(resp) via reflection to avoid linkage issues
                try {
                    for (java.lang.reflect.Method cm : context.getClass().getMethods()) {
                        if (cm.getName().equals("challenge") && cm.getParameterCount() == 1) {
                            cm.invoke(context, resp);
                            return;
                        }
                    }
                } catch (Throwable t) {
                    logger.log(Level.WARNING, "CompanyUsernameAuthenticator: failed to invoke challenge reflectively", t);
                }
            }
        } catch (Throwable t) {
            logger.log(Level.WARNING, "CompanyUsernameAuthenticator: unexpected error while creating challenge", t);
        }
    }

    @Override
    public void action(AuthenticationFlowContext context) {
        Object form = null;
        try {
            java.lang.reflect.Method getDecoded = context.getHttpRequest().getClass().getMethod("getDecodedFormParameters");
            form = getDecoded.invoke(context.getHttpRequest());
        } catch (NoSuchMethodException nsme) {
            try {
                java.lang.reflect.Method getForm = context.getHttpRequest().getClass().getMethod("getFormParameters");
                form = getForm.invoke(context.getHttpRequest());
            } catch (Exception ignore) {
            }
        } catch (Exception e) {
        }

        String company = null;
        String username = null;
        String password = null;
        boolean validationSuccess = false;
        
        if (form != null) {
            try {
                logger.info("CompanyUsernameAuthenticator: form class=" + form.getClass().getName());
                java.lang.reflect.Method getFirst = null;
                try {
                    getFirst = form.getClass().getMethod("getFirst", Object.class);
                } catch (NoSuchMethodException ex) {
                    try {
                        getFirst = form.getClass().getMethod("getFirst", String.class);
                    } catch (NoSuchMethodException ignore) {
                    }
                }

                if (getFirst != null) {
                    company = (String) getFirst.invoke(form, "company");
                    username = (String) getFirst.invoke(form, "username");
                    password = (String) getFirst.invoke(form, "password");
                }

                logger.info(String.format("CompanyUsernameAuthenticator: got company='%s', username='%s'", company, username));
                if (company != null && company.trim().length() > 0 && username != null && password != null) {
                    String combined = company.trim() + "-" + username.trim();
                    logger.info(String.format("CompanyUsernameAuthenticator: replaced username with '%s'", combined));
                    
                    // Validate credentials
                    org.keycloak.models.UserModel user = context.getSession().users().getUserByUsername(context.getRealm(), combined);
                    if (user != null) {
                        logger.info("CompanyUsernameAuthenticator: user found, validating password");
                        boolean passwordValid = false;
                        try {
                            Object passwordCredential = org.keycloak.models.UserCredentialModel.password(password);
                            
                            // Try multiple approaches to validate password
                            try {
                                // Approach 1: Use user.credentialManager()
                                java.lang.reflect.Method credMgr = user.getClass().getMethod("credentialManager");
                                Object userCredMgr = credMgr.invoke(user);
                                
                                // Find the correct isValid method and check parameter types
                                for (java.lang.reflect.Method m : userCredMgr.getClass().getMethods()) {
                                    if (m.getName().equals("isValid") && m.getParameterCount() == 1) {
                                        Class<?> paramType = m.getParameterTypes()[0];
                                        logger.info("CompanyUsernameAuthenticator: found isValid with param type: " + paramType.getName());
                                        
                                        // Try with array
                                        if (paramType.isArray()) {
                                            org.keycloak.credential.CredentialInput[] credArray = new org.keycloak.credential.CredentialInput[]{(org.keycloak.credential.CredentialInput)passwordCredential};
                                            passwordValid = (Boolean) m.invoke(userCredMgr, (Object)credArray);
                                            logger.info("CompanyUsernameAuthenticator: validated using user.credentialManager().isValid(array)");
                                            break;
                                        }
                                        // Try with List
                                        else if (java.util.List.class.isAssignableFrom(paramType)) {
                                            java.util.List<org.keycloak.credential.CredentialInput> credList = java.util.Arrays.asList((org.keycloak.credential.CredentialInput)passwordCredential);
                                            passwordValid = (Boolean) m.invoke(userCredMgr, credList);
                                            logger.info("CompanyUsernameAuthenticator: validated using user.credentialManager().isValid(list)");
                                            break;
                                        }
                                        // Try with single credential
                                        else if (paramType.isInstance(passwordCredential)) {
                                            passwordValid = (Boolean) m.invoke(userCredMgr, passwordCredential);
                                            logger.info("CompanyUsernameAuthenticator: validated using user.credentialManager().isValid()");
                                            break;
                                        }
                                    }
                                }
                            } catch (Exception e1) {
                                logger.log(Level.WARNING, "CompanyUsernameAuthenticator: user.credentialManager() failed", e1);
                            }
                            
                            if (!passwordValid) {
                                // Approach 2: Use session.users().validCredentials()
                                try {
                                    Object userProvider = context.getSession().users();
                                    for (java.lang.reflect.Method m : userProvider.getClass().getMethods()) {
                                        if (m.getName().equals("validCredentials") && m.getParameterCount() == 3) {
                                            passwordValid = (Boolean) m.invoke(userProvider, context.getRealm(), user, passwordCredential);
                                            logger.info("CompanyUsernameAuthenticator: validated using users().validCredentials()");
                                            break;
                                        }
                                    }
                                } catch (Exception e2) {
                                    logger.info("CompanyUsernameAuthenticator: users().validCredentials() failed: " + e2.getMessage());
                                }
                            }
                            
                            if (!passwordValid) {
                                // Approach 3: Use session.getProvider(PasswordCredentialProvider.class)
                                try {
                                    Class<?> passwordProviderClass = Class.forName("org.keycloak.credential.PasswordCredentialProvider");
                                    java.lang.reflect.Method getProviderMethod = context.getSession().getClass().getMethod("getProvider", Class.class);
                                    Object passwordProvider = getProviderMethod.invoke(context.getSession(), passwordProviderClass);
                                    if (passwordProvider != null) {
                                        for (java.lang.reflect.Method m : passwordProvider.getClass().getMethods()) {
                                            if (m.getName().equals("isValid") && m.getParameterCount() == 3) {
                                                passwordValid = (Boolean) m.invoke(passwordProvider, context.getRealm(), user, passwordCredential);
                                                logger.info("CompanyUsernameAuthenticator: validated using PasswordCredentialProvider");
                                                break;
                                            }
                                        }
                                    }
                                } catch (Exception e3) {
                                    logger.info("CompanyUsernameAuthenticator: PasswordCredentialProvider failed: " + e3.getMessage());
                                }
                            }
                            
                            if (passwordValid) {
                                logger.info("CompanyUsernameAuthenticator: authentication successful");
                                context.setUser(user);
                                context.success();
                                validationSuccess = true;
                            } else {
                                logger.info("CompanyUsernameAuthenticator: password validation failed");
                            }
                        } catch (Exception e) {
                            logger.log(Level.SEVERE, "CompanyUsernameAuthenticator: password validation error", e);
                        }
                    } else {
                        logger.info("CompanyUsernameAuthenticator: user not found");
                    }
                }
            } catch (Exception e) {
                logger.log(Level.SEVERE, "CompanyUsernameAuthenticator: exception during validation", e);
            }
        }
        
        if (validationSuccess) {
            return;
        }
        
        // Show error and re-display form
        logger.info("CompanyUsernameAuthenticator: showing error form");
        try {
            Object formProvider = context.form();
            
            // Try to set error message
            for (java.lang.reflect.Method m : formProvider.getClass().getMethods()) {
                if (m.getName().equals("setError") && m.getParameterCount() == 1) {
                    try {
                        m.invoke(formProvider, "invalidUserMessage");
                        break;
                    } catch (Exception e) {
                        logger.log(Level.WARNING, "CompanyUsernameAuthenticator: setError invocation failed", e);
                    }
                }
            }
            
            Object resp = null;
            try {
                java.lang.reflect.Method createFormMethod = formProvider.getClass().getMethod("createForm", String.class);
                resp = createFormMethod.invoke(formProvider, "login.ftl");
            } catch (NoSuchMethodException nsme) {
                try {
                    java.lang.reflect.Method createFormMethod = formProvider.getClass().getMethod("createForm");
                    resp = createFormMethod.invoke(formProvider);
                } catch (Exception e) {
                    logger.log(Level.SEVERE, "CompanyUsernameAuthenticator: createForm failed", e);
                }
            } catch (Exception e) {
                logger.log(Level.SEVERE, "CompanyUsernameAuthenticator: createForm with template failed", e);
            }
            
            if (resp != null) {
                for (java.lang.reflect.Method cm : context.getClass().getMethods()) {
                    if (cm.getName().equals("failureChallenge") && cm.getParameterCount() == 2) {
                        cm.invoke(context, org.keycloak.authentication.AuthenticationFlowError.INVALID_CREDENTIALS, resp);
                        logger.info("CompanyUsernameAuthenticator: failureChallenge invoked");
                        return;
                    }
                }
            }
        } catch (Exception e) {
            logger.log(Level.SEVERE, "CompanyUsernameAuthenticator: failed to show error form", e);
        }
        
        logger.warning("CompanyUsernameAuthenticator: falling back to context.failure");
        context.failure(org.keycloak.authentication.AuthenticationFlowError.INVALID_CREDENTIALS);
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

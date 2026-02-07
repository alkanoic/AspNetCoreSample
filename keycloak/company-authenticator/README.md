# Company Authenticator

Build and install custom Keycloak authenticator that combines `company` + `username` into the internal Keycloak username before authentication.

Build:

```bash
cd keycloak/company-authenticator
mvn -DskipTests package
```

Deploy (Devcontainer):

1. Copy the produced JAR from `target/company-authenticator-1.0.0.jar` to the Keycloak providers directory exposed to the container, e.g.:

```bash
mkdir -p .devcontainer/docker/volumes/keycloak/providers
cp target/company-authenticator-1.0.0.jar .devcontainer/docker/volumes/keycloak/providers/
```

2. Restart Keycloak container (devcontainer) so it picks up the provider.

3. In Keycloak admin console (realm Test) add a custom execution to the Browser flow using provider id `company-username-authenticator` and place it before `Username Password Form`.

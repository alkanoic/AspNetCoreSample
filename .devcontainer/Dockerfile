FROM mcr.microsoft.com/devcontainers/dotnet:6.0-focal

RUN apt-get update && apt-get install -y iputils-ping mysql-client

# install nvm
RUN curl https://raw.githubusercontent.com/creationix/nvm/master/install.sh | bash && \
    . $NVM_DIR/nvm.sh && \
    nvm install --lts

# install azure cli
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# install trivy
RUN wget https://github.com/aquasecurity/trivy/releases/download/v0.18.3/trivy_0.18.3_Linux-64bit.deb && \
    dpkg -i trivy_0.18.3_Linux-64bit.deb && \
    rm trivy_0.18.3_Linux-64bit.deb

USER vscode

# install bun
RUN curl -fsSL https://bun.sh/install | bash

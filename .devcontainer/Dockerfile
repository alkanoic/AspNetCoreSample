FROM mcr.microsoft.com/devcontainers/dotnet:8.0-jammy

RUN apt-get update && \
    apt-get install -y iputils-ping mysql-client curl wget ca-certificates gnupg

# install azure cli
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# install node.js
RUN mkdir -p /etc/apt/keyrings && \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | sudo gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg && \
    NODE_MAJOR=20 && \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | sudo tee /etc/apt/sources.list.d/nodesource.list && \
    apt-get update && \
    apt-get install -y nodejs

# install dotnet workload aspire
RUN dotnet workload install aspire

# install trivy
RUN apt-get install wget apt-transport-https gnupg lsb-release && \
    wget -qO - https://aquasecurity.github.io/trivy-repo/deb/public.key | apt-key add - && \
    echo deb https://aquasecurity.github.io/trivy-repo/deb $(lsb_release -sc) main | tee -a /etc/apt/sources.list.d/trivy.list && \
    apt-get update && \
    apt-get install trivy

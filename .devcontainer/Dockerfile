FROM mcr.microsoft.com/devcontainers/dotnet:9.0-noble

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

# install cyclonedx for npm
RUN npm install -g @cyclonedx/cyclonedx-npm

# install trivy
RUN apt-get install -y wget apt-transport-https gnupg lsb-release && \
    wget -qO - https://aquasecurity.github.io/trivy-repo/deb/public.key | apt-key add - && \
    echo deb https://aquasecurity.github.io/trivy-repo/deb $(lsb_release -sc) main | tee -a /etc/apt/sources.list.d/trivy.list && \
    apt-get update && \
    apt-get install -y trivy

# install protoc
RUN apt-get install -y protobuf-compiler && \
    wget https://github.com/grpc/grpc-web/releases/download/1.5.0/protoc-gen-grpc-web-1.5.0-linux-x86_64 && \
    mv ./protoc-gen-grpc-web-1.5.0-linux-x86_64 /usr/local/bin/protoc-gen-grpc-web && \
    chmod +x /usr/local/bin/protoc-gen-grpc-web

RUN wget https://github.com/fullstorydev/grpcurl/releases/download/v1.8.9/grpcurl_1.8.9_linux_x86_64.tar.gz && \
    tar -xzf grpcurl_1.8.9_linux_x86_64.tar.gz && \
    sudo mv grpcurl /usr/local/bin/grpcurl && \
    chmod +x /usr/local/bin/grpcurl && \
    rm grpcurl_1.8.9_linux_x86_64.tar.gz && \
    rm LICENSE

# install mysqldef
ARG SQLDEF_VERSION=v1.0.1

# RUN curl -OL https://github.com/k0kubun/sqldef/releases/download/${SQLDEF_VERSION}/mysqldef_linux_amd64.tar.gz && \
#     tar xf mysqldef_linux_amd64.tar.gz -C /usr/local/bin/ && \
#     rm mysqldef_linux_amd64.tar.gz

# install psqldef
RUN curl -OL https://github.com/k0kubun/sqldef/releases/download/${SQLDEF_VERSION}/psqldef_linux_amd64.tar.gz && \
    tar xf psqldef_linux_amd64.tar.gz -C /usr/local/bin/ && \
    rm psqldef_linux_amd64.tar.gz

# install mssqldef
# RUN curl -OL https://github.com/k0kubun/sqldef/releases/download/${SQLDEF_VERSION}/mssqldef_linux_amd64.tar.gz && \
#     tar xf mssqldef_linux_amd64.tar.gz -C /usr/local/bin/ && \
#     rm mssqldef_linux_amd64.tar.gz

RUN curl -fsSL https://packages.redis.io/gpg | sudo gpg --dearmor -o /usr/share/keyrings/redis-archive-keyring.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/redis-archive-keyring.gpg] https://packages.redis.io/deb $(lsb_release -cs) main" | tee /etc/apt/sources.list.d/redis.list && \
    apt-get update && \
    apt-get install -y redis

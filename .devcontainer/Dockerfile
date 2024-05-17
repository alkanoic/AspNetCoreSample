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
RUN curl -OL https://github.com/k0kubun/sqldef/releases/download/v0.17.8/mysqldef_linux_amd64.tar.gz && \
    tar xf mysqldef_linux_amd64.tar.gz -C /usr/local/bin/ && \
    rm mysqldef_linux_amd64.tar.gz

# setting local machine keploy
curl --silent --location "https://github.com/keploy/keploy/releases/latest/download/keploy_linux_amd64.tar.gz" | tar xz -C /tmp && \
    sudo mkdir -p /usr/local/bin && \
    sudo mv /tmp/keploy /usr/local/bin

keploy record -c "dotnet run"

# settings docker keploy
docker network create keploy-network
alias keploy='sudo docker run --pull always --name keploy-v2 -p 16789:16789 --privileged --pid=host -it -v "$(pwd)":/files -v /sys/fs/cgroup:/sys/fs/cgroup -v /sys/kernel/debug:/sys/kernel/debug -v /sys/fs/bpf:/sys/fs/bpf -v /var/run/docker.sock:/var/run/docker.sock --rm ghcr.io/keploy/keploy'

tagname=codegen.result
docker build -t $tagname .
# docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --network aspnetcoresample_devcontainer_default codegen.result

keploy record -c "docker run --name $tagname -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --network aspnetcoresample_devcontainer_default $tagname" --containerName $tagname

keploy test -c "docker run --name $tagname -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --network aspnetcoresample_devcontainer_default $tagname" --containerName $tagname --delay 5

# docker build -t aspnetcoremvc -f Dockerfile_keploy .
# docker build -t aspnetcorewebapi -f Dockerfile_webapi_keploy .
keploy record -c "docker run -p 8082:8080 --name aspnetcoremvc --network aspnetcoresample_devcontainer_default aspnetcoremvc" --containerName "aspnetcoremvc"
# docker run -p 8082:8080 --rm --name aspnetcoremvc --network aspnetcoresample_devcontainer_default aspnetcoremvc
keploy record -c "docker run -p 8083:8080 --name aspnetcorewebapi --network aspnetcoresample_devcontainer_default aspnetcorewebapi" --containerName "aspnetcorewebapi"

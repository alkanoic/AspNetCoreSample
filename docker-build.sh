# 単体ビルド

docker build -t aspnetcoresample-mvc-x86:latest .

# マルチプラットフォームビルド

# QMENUのエミュレーションをインストール
# docker run --privileged --rm tonistiigi/binfmt --install all
# docker buildx create --use
# docker buildx inspect --bootstrap

docker buildx build --platform linux/arm64 -t aspnetcoresample-mvc-arm64:latest .
docker buildx build --platform linux/amd64 -t aspnetcoresample-mvc-amd64:latest .

# 同時複数プラットフォームビルド（すごく重い）

# docker-containerのdriverを追加し、初期化
# docker buildx create --name mybuilder --driver docker-container --use
# docker buildx inspect --bootstrap

docker buildx build --platform linux/arm64,linux/amd64 -t aspnetcoresample-mvc:latest .

FROM nvidia/cuda:12.8.1-devel-ubuntu24.04 AS build

RUN apt-get update && apt-get install -y --no-install-recommends ca-certificates cmake g++ git libgomp1 libssl-dev make && rm -rf /var/lib/apt/lists/*

ARG LLAMA_COMMIT=2459f68b5c0eb26261fd5a81682004b93cd645ba
WORKDIR /src
RUN git init && git remote add origin https://github.com/PrismML-Eng/llama.cpp.git && git fetch --depth 1 origin "${LLAMA_COMMIT}" && git checkout --detach FETCH_HEAD && test "$(git rev-parse HEAD)" = "${LLAMA_COMMIT}"
RUN cmake -S . -B build -DCMAKE_BUILD_TYPE=Release -DGGML_CUDA=ON -DCMAKE_CUDA_ARCHITECTURES=89 -DGGML_NATIVE=OFF -DLLAMA_BUILD_TESTS=OFF -DLLAMA_BUILD_UI=OFF -DLLAMA_OPENSSL=OFF && cmake --build build --target llama-server -j 4
RUN mkdir -p /opt/llama/lib && find build -name '*.so*' -exec cp -P {} /opt/llama/lib/ \; && cp build/bin/llama-server /opt/llama/

FROM nvidia/cuda:12.8.1-runtime-ubuntu24.04
RUN apt-get update && apt-get install -y --no-install-recommends awscli ca-certificates curl libgomp1 && rm -rf /var/lib/apt/lists/*
COPY --from=build /opt/llama /opt/llama
COPY entrypoint.sh /opt/llama/entrypoint.sh
RUN chmod +x /opt/llama/entrypoint.sh && mkdir -p /models
ENV LD_LIBRARY_PATH=/opt/llama/lib
ENV LLAMA_ARG_HOST=0.0.0.0
ENV LLAMA_ARG_PORT=8080
ENV NVIDIA_DRIVER_CAPABILITIES=compute,utility
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=10s --start-period=10m --retries=3 CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["/opt/llama/entrypoint.sh"]

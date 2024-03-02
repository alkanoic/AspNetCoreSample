<template>
  <div>
    <Card></Card>
    <p>
      Last result: <b>{{ result }}</b>
    </p>

    <qrcode-stream
      :paused="paused"
      @detect="onDetect"
      @camera-on="onCameraOn"
      @camera-off="onCameraOff"
      @error="onError"
    >
      <div v-show="showScanConfirmation" class="scan-confirmation">check</div>
    </qrcode-stream>
  </div>
</template>
<script setup lang="ts">
  import { QrcodeStream } from "vue-qrcode-reader";

  import { ref } from "vue";

  const paused = ref(false);
  const result = ref("");
  const showScanConfirmation = ref(false);

  const onCameraOn = () => {
    showScanConfirmation.value = false;
  };

  const onCameraOff = () => {
    showScanConfirmation.value = true;
  };

  const onError = (err) => {
    console.error(err);
  };

  const onDetect = async (detectedCodes) => {
    result.value = JSON.stringify(detectedCodes.map((code) => code.rawValue));
    paused.value = true;

    await timeout(500);

    paused.value = false;
  };

  const timeout = (ms) => {
    return new Promise((resolve) => {
      setTimeout(resolve, ms);
    });
  };
</script>

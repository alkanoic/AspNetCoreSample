<template>
  <div>
    <StopCard></StopCard>
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
      <div v-show="showScanConfirmation" class="scan-confirmation">
        <div class="icon-confirmation">
          <font-awesome-icon :icon="['fas', 'check']" :class="['fa-2x']" />
        </div>
      </div>
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
<style scoped>
  .scan-confirmation {
    width: 100%;
    height: 100%;

    background-color: rgba(255, 255, 255, 0.8);
  }
  .icon-confirmation {
    color: green;
    font-size: large;

    position: absolute;
    top: 30%;
    left: 50%;
  }
</style>

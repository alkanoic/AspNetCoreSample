<template>
    <div class="container">
        <ul>
            <li v-for="(textarea, index) in textareas" :key="index">
                <textarea v-model="textarea.content" class="textarea"
                    :style="{ height: textarea.height + 'px' }"></textarea>
                <!-- ドラッグ用のハンドルバー -->
                <div v-if="index < textareas.length - 1" class="drag-handle" @mousedown="startDrag(index)"></div>
            </li>
        </ul>
    </div>
</template>

<script>
import { ref, onMounted } from "vue";

export default {
    setup() {
        // ヘッダー領域を除いた高さ
        const headerHeight = 96; // ヘッダー高さはレイアウトで指定
        const containerHeight = ref(window.innerHeight - headerHeight);

        // テキストエリアの内容と高さを管理
        const textareas = ref([
            { content: "", height: 0 },
            { content: "", height: 0 },
            { content: "", height: 0 },
        ]);

        // 初期設定でテキストエリアの高さを均等に設定
        onMounted(() => {
            updateHeights();
            window.addEventListener("resize", handleResize);
        });

        // 画面サイズ変更に対応
        const handleResize = () => {
            containerHeight.value = window.innerHeight - headerHeight;
            updateHeights();
        };

        // テキストエリアの高さを均等に割り当てる
        const updateHeights = () => {
            const heightPerTextarea = containerHeight.value / textareas.value.length - 8;
            textareas.value.forEach((textarea) => {
                textarea.height = heightPerTextarea;
            });
        };

        // ドラッグ開始
        const startDrag = (index) => {
            const initialY = event.clientY;
            const initialHeight = textareas.value[index].height;
            const nextInitialHeight = textareas.value[index + 1].height;

            const onMouseMove = (event) => {
                const deltaY = event.clientY - initialY;
                const newHeight = initialHeight + deltaY;
                const nextNewHeight = nextInitialHeight - deltaY;

                // テキストエリアの高さを調整
                if (newHeight > 50 && nextNewHeight > 50) {
                    textareas.value[index].height = newHeight;
                    textareas.value[index + 1].height = nextNewHeight;
                }
            };

            const onMouseUp = () => {
                document.removeEventListener("mousemove", onMouseMove);
                document.removeEventListener("mouseup", onMouseUp);
            };

            document.addEventListener("mousemove", onMouseMove);
            document.addEventListener("mouseup", onMouseUp);
        };

        return {
            textareas,
            startDrag,
        };
    },
};
</script>

<style scoped>
.container {
    height: calc(100vh - 96px);
    overflow: hidden;
    display: flex;
    flex-direction: column;
}

ul {
    list-style: none;
    padding: 0;
    margin: 0;
    flex: 1;
    display: flex;
    flex-direction: column;
}

li {
    flex: 1;
    display: flex;
    flex-direction: column;
}

.textarea {
    width: 100%;
    box-sizing: border-box;
    resize: none;
    /* デフォルトのリサイズを無効にする */
    border: 1px solid #ccc;
}

.drag-handle {
    height: 8px;
    background-color: #ddd;
    cursor: ns-resize;
    margin: 2px;
}
</style>

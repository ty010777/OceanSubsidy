<template>
    <div class="step-line-wrapper">
        <div class="step-line">
            <div class="step-line-item active">
                <div class="step-line-item-title">核定計畫</div>
                <div class="tag tag-white">審核中</div>
            </div>
        </div>
        <div class="step-line">
            <div class="step-line-item">
                <div class="step-line-item-title">每月進度</div>
                <div class="tag"></div>
            </div>
        </div>
        <div class="step-line">
            <div class="step-line-item">
                <div class="step-line-item-title">階段報告</div>
                <div class="tag"></div>
            </div>
            <div class="step-line-item">
                <div class="step-line-item-title">請款核銷</div>
                <div class="tag"></div>
            </div>
        </div>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] },
        step: { required: true, type: Number }
    });
</script>

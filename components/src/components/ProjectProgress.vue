<template>
    <div class="step-line-wrapper">
        <div class="step-line">
            <div class="step-line-item" :class="{ active: step === 1 }" @click="goto('Audit')" role="button">
                <div class="step-line-item-title">核定計畫</div>
                <template v-if="step === 1">
                    <div class="tag tag-white" v-if="[1,4].includes(progress[props.type].changeStatus)">編輯中</div>
                    <div class="tag tag-white" v-else-if="progress[props.type].changeStatus === 2">審核中</div>
                </template>
            </div>
        </div>
        <div class="step-line" v-if="!excludes.includes(4)">
            <div class="step-line-item" :class="{ active: step === 4 }" @click="goto('Progress')" role="button">
                <div class="step-line-item-title">每月進度</div>
                <div class="tag tag-white" v-if="step === 4">編輯中</div>
            </div>
        </div>
        <div class="step-line">
            <div class="step-line-item" :class="{ active: step === 5 }" @click="goto('Report')" role="button">
                <div class="step-line-item-title">階段報告</div>
                <div class="tag tag-white" v-if="step === 5">編輯中</div>
            </div>
            <div class="step-line-item" :class="{ active: step === 6 }" @click="goto('Payment')" role="button">
                <div class="step-line-item-title">請款核銷</div>
                <div class="tag tag-white" v-if="step === 6">編輯中</div>
            </div>
        </div>
    </div>
</template>

<script setup>
    const progress = useProgressStore();

    const props = defineProps({
        excludes: { default: [], type: Array },
        id: { type: [Number, String] },
        step: { required: true, type: Number },
        type: { required: true, type: String }
    });

    const goto = (name) => {
        location.href = `${name}.aspx?ID=${props.id}`;
    };
</script>

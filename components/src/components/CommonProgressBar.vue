<template>
    <div class="application-step">
        <div class="step-item"
            :class="{ active: editable && (item.step < progress[type].step || item.step === step) }"
            @click="goto(item)"
            :key="item"
            role="button"
            v-for="(item) in steps"
        >
            <div class="step-content">
                <div class="step-label">{{ item.title }}</div>
                <div class="step-status edit" v-if="editable && item.step === step">編輯中</div>
                <div class="step-status" v-else-if="item.step < progress[type].step">已完成</div>
            </div>
        </div>
    </div>
</template>

<script setup>
    const editable = computed(() => isProjectEditable(props.type, progress[props.type].status, props.step));

    const progress = useProgressStore();

    const props = defineProps({
        id: { type: Number },
        review: { default: false, type: Boolean },
        step: { required: true, type: Number },
        steps: { required: true, type: Array },
        type: { required: true, type: Number }
    });

    const goto = (item) => {
        if (props.review) {
            emit("click", item.step);
        } else if (item.step <= progress[props.type].step) {
            location.href = `${item.url}.aspx?ID=${props.id}`;
        }
    };

    const emit = defineEmits(["click"]);
</script>

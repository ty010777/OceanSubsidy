<template>
    <div class="top-wrapper">
        <div class="top-block">
            <div class="d-flex align-items-center gap-3">
                <span>［{{ store[props.type].type }}］{{ store[props.type].id }}/{{ store[props.type].name }}</span>
                <span class="tag tag-light-gray" v-if="store[type].status === 91">計畫已結案</span>
                <span class="tag tag-light-pink" v-else-if="store[type].status === 99">計畫已終止</span>
                <span class="tag tag-pale-green" v-else-if="todo">待辦事項：{{ todo }}</span>
            </div>
            <a class="btn btn-teal-dark" :href="`../AuditRecords.aspx?ProjectID=${id}`" target="_blank">
                查核紀錄
            </a>
        </div>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] },
        type: { type: String }
    });

    const store = useProgressStore();
    const todo = ref();

    onMounted(() => {
        api.system("getProjectTodoTask", { ID: props.id }).subscribe((res) => todo.value = res?.TaskName);
    });
</script>

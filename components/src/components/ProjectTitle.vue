<template>
    <div class="top-wrapper">
        <div class="top-block">
            <div class="d-flex align-items-center gap-3">
                <span>［{{ store[props.type].type }}］{{ store[props.type].id }}/{{ store[props.type].name }}</span>
<!--
                <span class="tag tag-pale-green">待辦事項：請填寫契約資料</span>
-->
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
</script>

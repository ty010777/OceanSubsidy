<template>
    <div class="d-flex align-items-center justify-content-between flex-wrap gap-2" v-if="prop.count">
        <nav class="pagination" aria-label="Pagination">
            <button aria-label="Previous page" class="nav-button" @click="click(page - 1)" :disabled="page === 1" type="button">
                <i class="fas fa-chevron-left"></i>
            </button>
            <button class="pagination-item" @click="click(1)" type="button" v-if="data.from > 1">
                <span class="page-number">1</span>
            </button>
            <button class="pagination-item ellipsis" type="button" v-if="data.from > 2">
                <span>...</span>
            </button>
            <button class="pagination-item" :class="{ active: page === i }" @click="click(i)" :key="i" type="button" v-for="(i) in data.pages">
                <span class="page-number">{{ i }}</span>
            </button>
            <button class="pagination-item ellipsis" type="button" v-if="max > data.to + 1">
                <span>...</span>
            </button>
            <button class="pagination-item" @click="click(max)" type="button" v-if="max > data.to">
                <span class="page-number">{{ max }}</span>
            </button>
            <button aria-label="Next page" class="nav-button" @click="click(page + 1)" type="button" :disabled="page === max">
                <i class="fas fa-chevron-right"></i>
            </button>
        </nav>
        <div class="page-number-control">
            <div class="page-number-control-item">
                <span>跳到</span>
                <select @change="click($event.target.value)" class="form-select jump-to-page">
                    <option :key="item" :selected="item === page" :value="item" v-for="(item) in max">{{ item }}</option>
                </select>
                <span>頁</span>
                <span>,</span>
            </div>
            <div class="page-number-control-item">
                <span>每頁顯示</span>
                <select @change="click(page, $event.target.value)" class="form-select">
                    <option :value="item" :selected="item === pageSize" v-for="(item) in [10,20,30]">{{ item }}</option>
                </select>
                <span>筆</span>
            </div>
        </div>
    </div>
</template>

<script setup>
    const data = {};

    const prop = defineProps({
        count: { required: true, type: Number },
        page: { required: true, type: Number },
        pageSize: { required: true, type: Number },
        slots: { default: 3, type: Number }
    });

    const max = computed(() => Math.ceil(prop.count / prop.pageSize));

    const click = (page, size) => {
        if (size) {
            page = Math.min(page, Math.ceil(prop.count / size));
        }

        if (1 <= page && page <= max.value) {
            emit("change", page, size || prop.pageSize);
        }
    };

    const emit = defineEmits(["change"]);

    watchEffect(() => {
        data.from = prop.page - parseInt(prop.slots / 2);
        data.to = prop.page + parseInt((prop.slots - 1) / 2);
        data.pages = [];

        for (let i = data.from; i <= data.to; i++) {
            if (0 < i && i <= max.value) {
                data.pages.push(i);
            }
        }
    });
</script>

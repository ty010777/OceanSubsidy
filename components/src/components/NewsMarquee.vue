<template>
    <div class="notice mb-3" v-if="list.length">
        <div class="notice-content">
            <div class="notice-list" ref="marquee">
                <h3 class="notice-title" style="margin-bottom:0.5rem;" :key="item" v-for="(item) in list"><tw-date :value="item.EnableTime"></tw-date> {{ item.Title }}</h3>
                <h3 class="notice-title" v-if="list.length > 1"><tw-date :value="list[0].EnableTime"></tw-date> {{ list[0].Title }}</h3>
            </div>
        </div>
        <div class="notice-action">
            <a class="btn-link" :href="url">全部公告</a>
        </div>
    </div>
</template>

<style scoped>
    .notice-content {
        height: 1rem;
        overflow: hidden;
        position: relative;
    }

    .notice-list {
        position: absolute;
        top: 0;
        transition: transform 1s ease-in-out;
        width: 100%;
    }

    .notice-title {
        height: 1rem;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }
</style>

<script setup>
    let index = 0;

    const list = ref([]);
    const marquee = ref();
    const url = computed(() => api.toUrl("/OFS/Information/NewsList.aspx"));

    onMounted(() => {
        api.system("getPublishedNewsList").subscribe((res) => {
            list.value = res.List;

            if (list.value.length > 1) {
                setInterval(() => {
                    index++;
                    marquee.value.style.transition = "transform 1s ease-in-out";
                    marquee.value.style.transform = `translateY(-${index * 1.5}rem)`;

                    if (index === list.value.length) {
                        setTimeout(() => {
                            marquee.value.style.transition = "none";
                            marquee.value.style.transform = "translateY(0)";
                            index = 0;
                        }, 1000);
                    }
                }, 5000);
            }
        });
    });
</script>

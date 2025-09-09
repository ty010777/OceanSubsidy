<template>
    <div class="block rounded-4 mt-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`" alt="logo">
                    <span>列表</span>
                </h4>
            </div>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table class="table teal-table" aria-label="公告列表">
                <thead>
                    <tr>
                        <th scope="col" style="width:220px">公告時間</th>
                        <th scope="col" class="text-start">標題</th>
                        <th scope="col" style="width:300px">發布單位</th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in rows">
                        <td data-th="公告時間:"><tw-date :value="item.EnableTime"></tw-date></td>
                        <td data-th="公告標題:" class="text-start">
                            <a class="link-black" :href="`NewsDetail.aspx?ID=${item.ID}`">{{ item.Title }}</a>
                        </td>
                        <td data-th="發布單位:" class="text-start">{{ item.UserOrg }} {{ item.UserName }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <list-pager :count="list.length" @change="change" :page="page" :page-size="size"></list-pager>
    </div>
</template>

<script setup>
    const list = ref([]);
    const page = ref(1);
    const size = ref(10);

    const rows = computed(() => list.value.slice((page.value - 1) * size.value, page.value * size.value));

    const change = (to, newSize) => {
        page.value = parseInt(to);
        size.value = parseInt(newSize);
    };

    onMounted(() => {
        api.system("getPublishedNewsList").subscribe((res) => list.value = res.List);
    });
</script>

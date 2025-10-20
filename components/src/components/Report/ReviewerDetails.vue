<template>
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`">
                    <span>審查計畫</span>
                </h4>
                <span>共 <span class="text-teal">{{ list.length }}</span> 筆資料</span>
            </div>
            <button class="btn btn-teal-dark" @click="download" type="button"><i class="fas fa-download"></i>匯出</button>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th>
                            <div class="hstack align-items-center justify-content-start"><span>委員姓名</span></div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start"><span>審查階段</span></div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start"><span>類別</span></div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center"><span>計畫編號</span></div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start"><span>計畫名稱</span></div>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in rows">
                        <td data-th="委員姓名:" class="text-start">{{ item.CommitteeUser }}</td>
                        <td data-th="審查階段:" class="text-start">{{ type == 1 ? "申請計畫" : "執行計畫" }}／{{ item.ReviewStage }}</td>
                        <td data-th="類別:" class="text-start">{{ item.FieldName }}</td>
                        <td data-th="計畫編號:">{{ item.ProjectID }}</td>
                        <td data-th="計畫名稱:" class="text-start">{{ item.ProjectName }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <list-pager :count="list.length" @change="change" :page="page" :page-size="size"></list-pager>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: String },
        type: { type: String }
    });

    const form = ref({ ID: props.id, Type: props.type });
    const list = ref([]);
    const page = ref(1);
    const size = ref(10);

    const rows = computed(() => list.value.slice((page.value - 1) * size.value, page.value * size.value));

    const change = (to, newSize) => {
        page.value = parseInt(to);
        size.value = parseInt(newSize);
    };

    const download = () => {
        api.system("downloadReviewerDetails", form.value).subscribe((res) => {
            const link = document.createElement("a");

            link.download = "審查計畫.xlsx";
            link.href = api.download(res.filename);
            link.click();
        });
    };

    onMounted(() => {
        api.system("getReviewerDetails", form.value).subscribe((res) => {
            list.value = res.List;
            page.value = 1;
        });
    });
</script>

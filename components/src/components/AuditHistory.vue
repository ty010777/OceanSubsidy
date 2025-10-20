<template>
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`">
                    <span>查核紀錄</span>
                </h4>
            </div>
        </div>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th>執行單位</th>
                        <td>
                            {{ name }}
                        </td>
                    </tr>
                    <tr>
                        <th>風險評估</th>
                        <td>
                            <span class="text-pink">{{ risks[risk] }}</span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>計畫編號／計畫名稱</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>查核日期／人員</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>風險評估</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>查核意見</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>執行單位回覆</span>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in list">
                        <td data-th="計畫編號／計畫名稱:" class="text-start">
                            <a class="link-black" :href="`AuditRecords.aspx?ProjectID=${item.ProjectID}`" target="_blank">{{ item.ProjectID }}／{{ item.ProjectName }}</a>
                        </td>
                        <td data-th="查核日期／人員:" class="text-start"><tw-date :value="item.CheckDate"></tw-date>／{{ item.ReviewerName }}</td>
                        <td data-th="風險評估:">{{ risks[item.lv] }}</td>
                        <td data-th="查核意見:" class="text-start">{{ item.ReviewerComment }}</td>
                        <td data-th="執行單位回覆:" class="text-start">{{ item.ExecutorComment }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script setup>
    const props = defineProps({
        name: { type: String }
    });

    const list = ref([]);
    const risk = ref(0);
    const risks = ["無", "低風險", "中風險", "高風險"];

    onMounted(() => {
        api.system("getAuditHistory", { Name: props.name }).subscribe((res) => {
            list.value = res.List;

            list.value.forEach((record) => {
                switch (record.Risk) {
                    case "Low":
                        record.lv = 1;
                        if (risk.value < 1) {
                            risk.value = 1;
                        }
                        break;
                    case "Medium":
                        record.lv = 2;
                        if (risk.value < 2) {
                            risk.value = 2;
                        }
                        break;
                    case "High":
                        record.lv = 3;
                        if (risk.value < 3) {
                            risk.value = 3;
                        }
                        break;
                }
            });
        });
    });
</script>

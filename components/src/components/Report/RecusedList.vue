<template>
    <div class="search bg-gray mt-4">
        <h3 class="text-teal">
            <i class="fa-solid fa-magnifying-glass"></i>
            查詢
        </h3>
        <div class="search-form">
            <div class="row g-3">
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">年度</div>
                    <input-select :empty-value="0" :options="years" v-model="form.Year"></input-select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">委員姓名, 任職單位, 職稱</div>
                    <input-text placeholder="請輸入委員姓名, 任職單位, 職稱" v-model.trim="form.Keyword"></input-text>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">計畫名稱</div>
                    <input-text placeholder="請輸入計畫名稱" v-model.trim="form.Name"></input-text>
                </div>
            </div>
            <div class="row g-3">
                <div class="col-12">
                    <div class="fs-16 text-gray mb-2">計畫申請單位</div>
                    <input-text placeholder="請輸入計畫申請單位" v-model.trim="form.Org"></input-text>
                </div>
            </div>
            <button class="btn btn-teal-dark d-table mx-auto" @click="submit" type="button">
                <i class="fa-solid fa-magnifying-glass"></i>
                查詢
            </button>
        </div>
    </div>
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`" />
                    <span>列表</span>
                </h4>
                <span>共 <span class="text-teal">{{ list.length }}</span> 筆資料</span>
            </div>
            <button class="btn btn-teal-dark" @click="download" type="button"><i class="fas fa-download"></i>匯出</button>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th width="80">年度</th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>計畫名稱</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center">
                                <span>計畫申請單位</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>委員姓名</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>任職單位</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>職稱</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>應迴避之具體理由及事證</span>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in rows">
                        <td data-th="年度:">{{ item.Year }}</td>
                        <td data-th="計畫名稱:" class="text-start">
                            <a class="link-black" :href="`../SCI/SciApplication.aspx?ProjectID=${item.ProjectID}`" target="_blank">{{ item.ProjectNameTw }}</a>
                        </td>
                        <td data-th="計畫申請單位:" class="text-start">{{ item.OrgName }}</td>
                        <td data-th="委員姓名:">{{ item.RecusedName }}</td>
                        <td data-th="任職單位:" class="text-start">{{ item.EmploymentUnit }}</td>
                        <td data-th="職稱:">{{ item.JobTitle }}</td>
                        <td data-th="應迴避之具體理由及事證:" class="text-start">{{ item.RecusedReason }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <list-pager :count="list.length" @change="change" :page="page" :page-size="size"></list-pager>
    </div>
</template>

<script setup>
    const form = ref({ Year: 0, Keyword: "", Name: "", Org: "" });
    const list = ref([]);
    const page = ref(1);
    const size = ref(10);
    const years = [];

    const rows = computed(() => list.value.slice((page.value - 1) * size.value, page.value * size.value));

    const change = (to, newSize) => {
        page.value = parseInt(to);
        size.value = parseInt(newSize);
    };

    const download = () => {
        api.system("downloadRecusedList", form.value).subscribe((res) => {
            const link = document.createElement("a");

            link.download = "迴避審查委員名單.xlsx";
            link.href = api.download(res.filename);
            link.click();
        });
    };

    const submit = () => {
        api.system("getRecusedList", form.value).subscribe((res) => {
            list.value = res.List;
            page.value = 1;
        });
    };

    for (let y = new Date().getFullYear() - 1911; y >= 114; y--) {
        years.push({ text: `${y} 年`, value: y });
    }

    onMounted(submit);
</script>

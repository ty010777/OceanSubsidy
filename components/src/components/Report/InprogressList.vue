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
                    <select class="form-select" v-model="form.year">
                        <option :value="undefined">全部</option>
                        <option :key="item" :value="item" v-for="(item) in yearList">{{ item }}年</option>
                    </select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">類別</div>
                    <select class="form-select" v-model="form.category">
                        <option :value="undefined">全部</option>
                        <option :key="item" :value="item" v-for="(item) in categoryList">{{ item }}</option>
                    </select>
                </div>
                <div class="col-12 col-lg-4">
                    <div class="fs-16 text-gray mb-2">主管單位</div>
                    <select class="form-select" v-model="form.unit">
                        <option :value="undefined">全部</option>
                        <option :key="item" :value="item" v-for="(item) in unitList">{{ item }}</option>
                    </select>
                </div>
            </div>
            <div class="row g-3">
                <div class="col-12 col-lg-6">
                    <div class="fs-16 text-gray mb-2">申請補助單位</div>
                    <input-text v-model.trim="form.org"></input-text>
                </div>
                <div class="col-12 col-lg-6">
                    <div class="fs-16 text-gray mb-2">計畫編號, 計畫名稱</div>
                    <input-text v-model.trim="form.name"></input-text>
                </div>
            </div>
            <button class="btn btn-teal-dark d-table mx-auto" type="button">
                <i class="fa-solid fa-magnifying-glass"></i>
                查詢
            </button>
        </div>
    </div>
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`" alt="logo">
                    <span>列表</span>
                </h4>
                <span>共 <span class="text-teal">{{ filterList.length }}</span> 筆資料</span>
            </div>
            <button class="btn btn-teal-dark" type="button"><i class="fas fa-download"></i>匯出</button>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th width="80">年度</th>
                        <th>
                            <div class="hstack align-items-center">
                                <span>類別</span>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>計畫名稱</span>
                            </div>
                        </th>
                        <th width="240">
                            <div class="hstack align-items-center">
                                <span>申請單位</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>本會補助金額</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>配合款</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>實支金額</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>累積經費執行率</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-end">
                                <span>本會已撥金額</span>
                            </div>
                        </th>
                    </tr>
                    <tr class="total-row">
                        <td colspan="4" style="text-align:end">總計</td>
                        <td class="text-end">{{ approved.toLocaleString() }}</td>
                        <td class="text-end">{{ filterList.reduce((sum, item) => sum + item.OtherAmount, 0).toLocaleString() }}</td>
                        <td class="text-end">{{ spend.toLocaleString() }}</td>
                        <td class="text-end">{{ toPercent(spend, approved).toLocaleString() }}%</td>
                        <td class="text-end">{{ filterList.reduce((sum, item) => sum + item.PaymentAmount, 0).toLocaleString() }}</td>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in rows">
                        <td data-th="年度:">{{ item.Year }}</td>
                        <td data-th="類別:">{{ item.Category }}</td>
                        <td data-th="計畫名稱:" style="text-align:left">
                            <a class="link-black" :href="path(item)" target="_blank">{{ item.ProjectName }}</a>
                        </td>
                        <td data-th="申請單位:" style="text-align:left">{{ item.OrgName }}</td>
                        <td data-th="本會補助金額:" class="text-end">{{ item.ApprovedAmount.toLocaleString() }}</td>
                        <td data-th="配合款:" class="text-end">{{ item.OtherAmount.toLocaleString() }}</td>
                        <td data-th="實支金額:" class="text-end">{{ item.SpendAmount.toLocaleString() }}</td>
                        <td data-th="累積經費執行率:" class="text-end">{{ toPercent(item.SpendAmount, item.ApprovedAmount).toLocaleString() }}%</td>
                        <td data-th="本會已撥金額:" class="text-end">{{ item.PaymentAmount.toLocaleString() }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <list-pager :count="filterList.length" @change="change" :page="page" :page-size="size"></list-pager>
    </div>
</template>

<script setup>
    const categoryList = ref([]);
    const form = ref({});
    const list = ref([]);
    const page = ref(1);
    const size = ref(10);
    const unitList = ref([]);
    const yearList = ref([]);

    const approved = computed(() => filterList.value.reduce((sum, item) => sum + item.ApprovedAmount, 0));
    const rows = computed(() => filterList.value.slice((page.value - 1) * size.value, page.value * size.value));
    const spend = computed(() => filterList.value.reduce((sum, item) => sum + item.SpendAmount, 0));

    const filterList = computed(() => {
        let data = list.value;

        if (form.value.year) {
            data = data.filter((i) => i.Year === form.value.year);
        }

        if (form.value.category) {
            data = data.filter((i) => i.Category === form.value.category);
        }

        if (form.value.unit) {
            data = data.filter((i) => i.SupervisoryUnit === form.value.unit);
        }

        if (form.value.org) {
            data = data.filter((i) => i.OrgName?.includes(form.value.org));
        }

        if (form.value.name) {
            data = data.filter((i) => i.ProjectID.includes(form.value.name) || i.ProjectName?.includes(form.value.name));
        }

        return data;
    });

    const change = (to, newSize) => {
        page.value = parseInt(to);
        size.value = parseInt(newSize);
    };

    const path = (item) => {
        if (item.ProjectID.includes("CUL")) {
            return `../CUL/Audit.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("EDC")) {
            return `../EDC/Audit.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("MUL")) {
            return `../MUL/Audit.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("LIT")) {
            return `../LIT/Audit.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("ACC")) {
            return `../ACC/Audit.aspx?ID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("CLB")) {
            return `../Clb/ClbApproved.aspx?ProjectID=${item.ProjectID}`;
        } else if (item.ProjectID.includes("SCI")) {
            return `../SCI/SciInprogress_Approved.aspx?ProjectID=${item.ProjectID}`;
        }
    };

    const toPercent = (value1, value2) => {
        if (value2) {
            return Math.round(value1 * 100 / value2 * 100) / 100;
        }

        return 0;
    };

    onMounted(() => {
        api.system("getInprogressList").subscribe((res) => {
            list.value = res.List;

            list.value.forEach((item) => {
                if (!yearList.value.includes(item.Year)) {
                    yearList.value.push(item.Year);
                }

                if (!categoryList.value.includes(item.Category)) {
                    categoryList.value.push(item.Category);
                }

                if (!unitList.value.includes(item.SupervisoryUnit)) {
                    unitList.value.push(item.SupervisoryUnit);
                }
            });

            yearList.value.sort((a, b) => b - a);

            const currentYear = new Date().getFullYear() - 1911;
            form.value.year = yearList.value.includes(currentYear) ? currentYear : yearList.value[0];
        });
    });
</script>
